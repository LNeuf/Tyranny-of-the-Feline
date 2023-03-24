using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Abilities;
using Matryoshka.Effect;
using Matryoshka.Effect.EffectSpawner;
using Matryoshka.Entity.Controller;
using Matryoshka.Game;
using Matryoshka.Lobby;
using Matryoshka.UI;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Matryoshka.Entity
{
    [Serializable]
    public struct AttackStruct
    {
        public AbilityType attackType;
        public Ability attack;
    }

    public enum PlayerState
    {
        Idle,
        Walk,
        Dead
    }
    
    public enum EntityType 
    {
        Big,
        Wazo,
        Salumon,
        Mittens,
        Minion_Furball,
        Catnip_plant,
        Cat_Tower
    }

    //

    public enum FacingDirection
    {
        None, // Reserved for not overriding
        Left,
        Right
    }

    [Serializable]
    public class Entity : NetworkBehaviour
    {
        private static float IntervalBetweenMittensAttacks = 2.0f;
        private static float InvervalSizeDecreasePerPhase = 0.2f;
        
        [SerializeField]
        public NetworkVariable<int> health;
        [SerializeField]
        public NetworkVariable<int> maxHealth;
        [SerializeField]
        private FacingDirection overrideFacingDirection = FacingDirection.None;
        private NetworkVariable<Vector2> moveDirection = new NetworkVariable<Vector2>(new Vector2(0, 0));
        private NetworkVariable<bool> isCurrentlyAttacking = new NetworkVariable<bool>(false);
        public GameObject healthBar;
        public GameObject shield;
        private bool isInvulnerable;
        public float damageReductionPercentage = 0f;
        public int Health;
        public float movementSpeed;
        public IController controller;
        public EntityType entityType;
        public List<AttackStruct> attacks;
        public Dictionary<AbilityType, float> cooldowns;
        private int timeHealAmount = 0;
        private int timeDamageAmount = 0;
        public bool currentlyDoingPhaseTransition = false;
        public int mittensPhase = 0;
        private bool isDead = true;
        private bool canSpawnCatnip = true;
        private bool canSpawnTowerA = false;
        private bool canSpawnTowerB = true;
        private bool nipOrTowerOnScreen = false;

        //

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Color defaultColor;

        public NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
        //
        public NetworkVariable<FacingDirection> currentFacingDirection = new NetworkVariable<FacingDirection>();
        //


        // Start is called before the first frame update
        void Start()
        {
            controller = this.gameObject.GetComponent<IController>();
            cooldowns = new Dictionary<AbilityType, float>();

            //

            spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            defaultColor = spriteRenderer.color;
            animator = this.gameObject.GetComponent<Animator>();

            isInvulnerable = false;
            maxHealth = new NetworkVariable<int>(Health);
            health = new NetworkVariable<int>(Health);

            isDead = false;
            //

            if (IsLocalPlayer)
            {
                GetComponent<Camera>().enabled = true;
            }

            if (IsServer)
            {
                if (overrideFacingDirection != FacingDirection.None)
                {
                    currentFacingDirection.Value = overrideFacingDirection;
                }
            }
            
        }

        public void SetInvulnerability(bool boolean)
        {
            isInvulnerable = boolean;
        }

        public void SetHealthBar(GameObject bar)
        {
            healthBar = bar;
        }

        public float GetHealthValue()
        {
            return (float)health.Value;
        }

        public int GetMaxHealthValue()
        {
            return maxHealth.Value;
        }

        public int GetMittensPhase(){
            return mittensPhase;
        }

        void UpdatePosition(Vector2 direction)
        {
            var position = transform.position;
            position = new Vector3(position.x + direction.x, position.y + direction.y, position.z);
            transform.position = position;
            if (entityType == EntityType.Big && networkPlayerState.Value == PlayerState.Walk)
            {
                LobbyManager.Singleton.PlaySoundClientRpc("PlayBigWalking");
            }
            if (entityType == EntityType.Salumon && networkPlayerState.Value == PlayerState.Walk)
            {
                LobbyManager.Singleton.PlaySoundClientRpc("PlaySalumonWalking");
            }
            if (entityType == EntityType.Wazo && networkPlayerState.Value == PlayerState.Walk)
            {
                LobbyManager.Singleton.PlaySoundClientRpc("PlayWazoWalking");
            }
            if (healthBar != null)
            {
                healthBar.transform.position = new Vector3(position.x, position.y + 1, position.z);
            }

            if (shield != null)
            {
                shield.transform.position = new Vector3(shield.transform.position.x + direction.x, 
                    shield.transform.position.y + direction.y, 
                    shield.transform.position.z);
            }
        }

        void Move()
        {
            UpdatePosition(moveDirection.Value * movementSpeed);
        }

        public void Dash(Vector2 direction)
        {
            UpdatePosition(direction);
        }

        public void SetMovementSpeed(float newMoveSpeed)
        {
            movementSpeed = newMoveSpeed;
        }
        
        public void SetAnimation(AbilityType abilityType)
        {
            if (animator == null)
                return;
            int abilityInt = (int) abilityType;
            animator.SetFloat("AttackState", abilityInt);
        }


        [ServerRpc]
        public void UpdateMoveDirectionServerRpc(Vector2 newMoveDirection)
        {
            if (overrideFacingDirection != FacingDirection.None)
            {
                currentFacingDirection.Value = overrideFacingDirection;
                return;
            }
            if (newMoveDirection.x > 0)
            {
                currentFacingDirection.Value = FacingDirection.Right;
            }
            else if (newMoveDirection.x < 0)
            {
                currentFacingDirection.Value = FacingDirection.Left;
            }
            moveDirection.Value = newMoveDirection;
        }

        [ServerRpc]
        public void CastAttackServerRpc(EntityInfo entityInfo)
        {
            if (!isCurrentlyAttacking.Value)
            {
                isCurrentlyAttacking.Value = true;
                StartCoroutine("CastAttack", entityInfo);
            }
        }

        [ServerRpc]
        public void UpdatePlayerStateServerRpc(PlayerState newState)
        {
            networkPlayerState.Value = newState;
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                int index = -1;
                
                for (int i = 0; i < GameManager.Singleton.playerObjects.Count; i++)
                {
                    if (entityType == GameManager.Singleton.playerObjects[i].GetComponent<Entity>().entityType)
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    GameManager.Singleton.playerObjects.RemoveAt(index);
                }
                
                GameManager.Singleton.playerObjects.Remove(gameObject);
            }
        }

        void FixedUpdate()
        {
            if(IsServer)
            {
                DecreaseCooldowns();
                Move();
                
                if(entityType == EntityType.Mittens && (health.Value < maxHealth.Value * 0.667)){
                    canSpawnTowerA = true;
                }
                else{
                   canSpawnTowerA = false; 
                }
                if (health.Value <= 0)
                {
                    if(entityType == EntityType.Mittens) {
                        if (!currentlyDoingPhaseTransition)
                        {
                            currentlyDoingPhaseTransition = true;
                            SetInvulnerability(true);
                            mittensPhase ++;
                            animator.SetFloat("Phase", mittensPhase - 0.5f);
                            if (mittensPhase == 4) {
                                networkPlayerState.Value = PlayerState.Dead;
                                gameObject.GetComponent<EnemyController>().canAttack = false;
                                GameManager.Singleton.shouldDespawnFurballs = true;
                                if (LobbyManager.Singleton != null)
                                {
                                    LobbyManager.Singleton.PlaySoundClientRpc("PlayMittensScream");
                                }
                            }
                            else
                            {
                                StartCoroutine(PhaseTransition(mittensPhase));
                            }
                        }
                    }
                    
                    else {
                        if (healthBar != null)
                        {
                            healthBar.GetComponent<NetworkObject>().Despawn();
                            healthBar = null;
                        }

                        if (!isDead)
                        {
                            if (entityType == EntityType.Big)
                            {
                                LobbyManager.Singleton.PlaySoundClientRpc("PlayBigDeath");
                            }

                            if (entityType == EntityType.Wazo)
                            {
                                LobbyManager.Singleton.PlaySoundClientRpc("PlayWazoDeath");
                            }

                            if (entityType == EntityType.Salumon)
                            {
                                LobbyManager.Singleton.PlaySoundClientRpc("PlaySalumonDeath");
                            }
                        }

                        isDead = true;
                        networkPlayerState.Value = PlayerState.Dead;

                        if (entityType == EntityType.Minion_Furball)
                        {
                            gameObject.GetComponent<NetworkObject>().Despawn();
                        } 
                        else
                        {
                            animator.SetBool("Dead", true);
                        }
                    }
                }
                
                if (entityType == EntityType.Minion_Furball && !NetworkManager.Singleton.ShutdownInProgress && !GameManager.Singleton.shouldDespawnFurballs)
                {
                    if (LobbyManager.Singleton != null)
                    {
                        LobbyManager.Singleton.PlaySoundClientRpc("PlayFurballSound");
                    }
                }

                if (entityType == EntityType.Minion_Furball && GameManager.Singleton.shouldDespawnFurballs)
                {
                    if (healthBar != null)
                    {
                        healthBar.GetComponent<NetworkObject>().Despawn();
                        healthBar = null;
                    }
                    networkPlayerState.Value = PlayerState.Dead;
                    gameObject.GetComponent<NetworkObject>().Despawn();
                }
            }

            if (IsClient && IsOwner)
            {
                AbilityType ability = AbilityType.None;
                if(!isCurrentlyAttacking.Value){
                    ability = controller.GetAbility();
                }

                Vector2 position = new Vector2(transform.position.x, transform.position.y);
                Vector2 mouseDirection = new Vector2(0,0);
                if (entityType == EntityType.Mittens) {
                    if(mittensPhase == 0){
                        mouseDirection = Vector2.down;
                    }
                    else if(mittensPhase == 1){
                        mouseDirection = Vector2.left;
                    }
                    else if(mittensPhase == 2){
                        mouseDirection = Vector2.right;
                    }
                    else if(mittensPhase == 3){
                        mouseDirection = Vector2.down;
                    }
                }
                else{
                    mouseDirection = controller.GetMouseDirection();
                }
                
                Vector2 movementDirection = controller.GetMovement();
                Vector2 mousePosition = controller.GetMousePosition();

                PlayerState playerState;
                if (networkPlayerState.Value == PlayerState.Dead)
                {
                    playerState = PlayerState.Dead;
                }
                else
                {
                    playerState = controller.GetPlayerState();
                }
                if (networkPlayerState.Value != playerState)
                {
                    UpdatePlayerStateServerRpc(playerState);
                }

                EntityInfo entityInfo = new EntityInfo(ability, position, mouseDirection, moveDirection.Value, entityType, mousePosition); 

                // force furball death fix
                if (entityType == EntityType.Minion_Furball && networkPlayerState.Value == PlayerState.Dead)
                {
                    ability = AbilityType.None;
                }
                
                if(!isCurrentlyAttacking.Value && ability != AbilityType.None)
                {
                    CastAttackServerRpc(entityInfo);
                }

                // force furball death fix
                if (entityType == EntityType.Minion_Furball && networkPlayerState.Value == PlayerState.Dead)
                {
                    movementDirection = moveDirection.Value;
                }
                
                if(movementDirection != moveDirection.Value)
                {
                    UpdateMoveDirectionServerRpc(movementDirection);
                }

            }
            //
            if (entityType != EntityType.Mittens)
            {
                changeDirection();
            }
            //

            Animate();
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            if (healthBar != null)
            {
                healthBar.transform.localScale = new Vector3(1.2f * ((float) health.Value / (float) maxHealth.Value),
                    healthBar.transform.localScale.y, healthBar.transform.localScale.z);
            }
        }

        private void DecreaseCooldowns()
        {
            Dictionary<AbilityType, float> newCooldowns = new Dictionary<AbilityType, float>();
            foreach(var cooldown in cooldowns)
            {
                float newCooldown = cooldown.Value - Time.deltaTime;
                if(newCooldown > 0)
                {
                    newCooldowns.Add(cooldown.Key, newCooldown);
                }
            
            }
            cooldowns = newCooldowns;
        }

        private Ability GetAttack(AbilityType attackType)
        {
            foreach(AttackStruct attackStruct in attacks)
            {
                if(attackStruct.attackType == attackType)
                {
                    return attackStruct.attack;
                }
            }
            return null;
        }

        private IEnumerator CastAttack(EntityInfo entityInfo)
        {
            Ability attack = GetAttack(entityInfo.currentAttack);
            if(attack == null || cooldowns.ContainsKey(entityInfo.currentAttack))
            {
                isCurrentlyAttacking.Value = false;
                yield break;
            }
            
            EffectManager.Singleton.Apply(attack.windUp, entityInfo);
            if (entityType == EntityType.Big && entityInfo.currentAttack == AbilityType.Special1)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayBigShieldWall");
                }
            }
            if(attack.windUpTime > 0f)
            {
                yield return new WaitForSeconds(attack.windUpTime);
                if (entityType == EntityType.Mittens && (currentlyDoingPhaseTransition || GetComponent<EnemyController>().sleeping))
                {
                    isCurrentlyAttacking.Value = false;
                    yield break;
                }
            }

            EffectManager.Singleton.Apply(attack.cast, entityInfo);
            if (entityType == EntityType.Wazo && entityInfo.currentAttack == AbilityType.Secondary)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayWazoDash");
                }
            }
            if(attack.castTime > 0f)
            {
                yield return new WaitForSeconds(attack.castTime);
                if (entityType == EntityType.Mittens && (currentlyDoingPhaseTransition || GetComponent<EnemyController>().sleeping))
                {
                    isCurrentlyAttacking.Value = false;
                    yield break;
                }
            }

            EffectManager.Singleton.Apply(attack.windDown, entityInfo);
            if (entityType == EntityType.Big && entityInfo.currentAttack == AbilityType.Special1)
            {
                if (LobbyManager.Singleton != null)
                {
                    LobbyManager.Singleton.PlaySoundClientRpc("PlayBigShieldWall");
                }
            }
            if(attack.windDownTime > 0f)
            {
                yield return new WaitForSeconds(attack.windDownTime);
                if (entityType == EntityType.Mittens && (currentlyDoingPhaseTransition || GetComponent<EnemyController>().sleeping))
                {
                    isCurrentlyAttacking.Value = false;
                    yield break;
                }
            }

            if (entityInfo.entityType == EntityType.Mittens)
            {
                gameObject.GetComponent<EnemyController>().StopMittensAttack(IntervalBetweenMittensAttacks - (mittensPhase * InvervalSizeDecreasePerPhase));
            }

            cooldowns.Add(entityInfo.currentAttack, attack.cooldownTime);
            if (entityInfo.entityType == EntityType.Big || entityInfo.entityType == EntityType.Wazo ||
                entityInfo.entityType == EntityType.Salumon)
            {
                GameManager.Singleton.StartCooldownClientRpc(entityInfo.entityType, entityInfo.currentAttack, attack.cooldownTime);
                //HUDUI.Singleton.StartCooldownTimer(entityType, entityInfo.currentAttack, attack.cooldownTime);
            }

            isCurrentlyAttacking.Value = false;

        }

        //

        public void Animate()
        {
            if (animator == null || entityType == EntityType.Mittens || entityType == EntityType.Minion_Furball || entityType == EntityType.Cat_Tower || entityType == EntityType.Catnip_plant)
                return;
            if (networkPlayerState.Value == PlayerState.Walk) 
            {
                animator.SetBool("Walk", true);
            }

            else if (networkPlayerState.Value == PlayerState.Idle)
            {
                animator.SetBool("Walk", false);
            }
            
            else if (networkPlayerState.Value == PlayerState.Dead)
            {
                // For death animation, probably despawn after
                animator.SetBool("Walk", false);
            }

        }

        public void DoDamage(int damage)
        {
            int damageValue = damage;
            float adjustedDamage = (1-damageReductionPercentage)*damageValue;
            damageValue = (int) adjustedDamage;
            if (entityType == EntityType.Big || entityType == EntityType.Wazo || entityType == EntityType.Salumon)
            {
                Entity mittens = GameManager.GetMittens().GetComponent<Entity>();
                damageValue *= (mittens.mittensPhase + 1);
            }
            if (!isInvulnerable)
            {
                health.Value -= damageValue;
                if (entityType == EntityType.Big || entityType == EntityType.Wazo || entityType == EntityType.Salumon)
                {
                    StartCoroutine("HitInvulnerableTime", damageValue);
                }
                else if (health.Value <= 0)
                {
                    switch (entityType)
                    {
                        case EntityType.Catnip_plant:
                            GetComponent<Effect.EffectController.Catnip>().DestroyedByPlayer();
                            break;
                        case EntityType.Cat_Tower:
                            GetComponent<CatTower>().Destroy();
                            break;
                    }
                }
                else if (entityType == EntityType.Cat_Tower)
                {
                    GetComponent<CatTower>().UpdateDamageFrame();
                    Debug.Log($"Tower now has {health.Value} lost {damageValue}");
                }
            }
        }

        private IEnumerator HitInvulnerableTime(int damage)
        {
            isInvulnerable = true;
            // lowest hit invulnerable time is 0.5 seconds
            if (Mathf.Log(damage, 500) >= 0.1f)
            {
                yield return new WaitForSeconds(Mathf.Log(damage, 50));
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            isInvulnerable = false;
        }

        public void Heal(int healAmount){
            health.Value += healAmount * maxHealth.Value / 100;
            if(health.Value > maxHealth.Value){
                health.Value = maxHealth.Value;
            }
        }

        public void HealToFull()
        {
            networkPlayerState.Value = PlayerState.Idle;
            health.Value = maxHealth.Value;
        }
        
        public void AreaHealStart(int healAmount, float duration){
            timeHealAmount = (int) (maxHealth.Value * healAmount / (100 * duration * 4)); 
            InvokeRepeating("AreaHealInternal", 0f, 0.25f);
        }

        private void AreaHealInternal() {
            health.Value += timeHealAmount;
            if(health.Value > maxHealth.Value){
                health.Value = maxHealth.Value;
            }
        }

        public void AreaHealEnd() {
            CancelInvoke("AreaHealInternal");
        }

        public void OverTimeDamageStart(int damageAmount, float duration){
            timeDamageAmount = (int) (damageAmount / (duration * 4));
            InvokeRepeating("OverTimeDamageInternal", 0f, 0.25f);
        }

        public void OverTimeDamageInternal(){
            health.Value -= timeDamageAmount;
            if (entityType == EntityType.Catnip_plant && health.Value <= 0)
            {
                GetComponent<Effect.EffectController.Catnip>().DestroyedByPlayer();
            }
        }

        public void OverTimeDamageEnd(){
            CancelInvoke("OverTimeDamageInternal");
        }

        //

        public void changeDirection()
        {
            if (currentFacingDirection.Value == FacingDirection.Right)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        public IEnumerator PhaseTransition(int phase)
        {
            if (LobbyManager.Singleton != null)
            {
                LobbyManager.Singleton.PlaySoundClientRpc("PlayMittensSpit");
            }

            // wait for current attack to finish
            while (isCurrentlyAttacking.Value)
            {
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1.3f); // wait for animation

            BulletSpawner spawner = gameObject.GetComponent<BulletSpawner>();
            
            if (phase == 1)
            {
                transform.position = new Vector3(19f, -1.75f, 0f);
                Vector2 clawpos = transform.position;
                clawpos += Utils.Utils.GetClawSwipeOffset(Vector2.left);
                GetComponent<EnemyController>().SetRangeFinderPosition(clawpos, phase);
                spawner.setMittensPosition(1);
            }
            else if (phase == 2)
            {
                UpdateMittensDirectionClientRpc(true);
                transform.position = new Vector3(-19f, -1.75f, 0f);
                Vector2 clawpos = transform.position;
                clawpos += Utils.Utils.GetClawSwipeOffset(Vector2.right);
                GetComponent<EnemyController>().SetRangeFinderPosition(clawpos, phase);
                spawner.setMittensPosition(3);
            }
            else if (phase == 3) {
                UpdateMittensDirectionClientRpc(false);
                transform.position = new Vector3(0f, 12f, 0f);
                Vector2 clawpos = transform.position;
                clawpos += Utils.Utils.GetClawSwipeOffset(Vector2.down);
                GetComponent<EnemyController>().SetRangeFinderPosition(clawpos, phase);
                spawner.setMittensPosition(0);
            }
            animator.SetFloat("Phase", phase);
            maxHealth.Value = 1000 * (phase + 1);
            HUDUI.Singleton.UpdateMittensMaxHealthClientRpc(maxHealth.Value);
            health.Value = maxHealth.Value;

            currentlyDoingPhaseTransition = false;

            yield return new WaitForSeconds(0.1f);
            SetInvulnerability(false);

            if (LobbyManager.Singleton != null)
            {
                LobbyManager.Singleton.PlaySoundClientRpc("PlayMittensTeleport");
            }
        }

        [ClientRpc]
        void UpdateMittensDirectionClientRpc(bool direction)
        {
            spriteRenderer.flipX = direction;
        }

        public bool IsDead(){
            return isDead;
        }

        public void CatnipSpawned(){
            canSpawnCatnip = false;
            StartCoroutine("CatnipCooldown");
        }

        public IEnumerator CatnipCooldown(){
            yield return new WaitForSeconds(30);
            canSpawnCatnip = true;
        }

        public bool CanSpawnCatnip(){
            return canSpawnCatnip;
        }

        public bool CanSpawnCatnipOrTower(){
            if(CanSpawnTower() || CanSpawnCatnip()){
                return true;
            }
            else{
                return false;
            }
        }

        public void TowerSpawned(){
            canSpawnTowerA = false;
            canSpawnTowerB = false;
            StartCoroutine("TowerCooldown");
        }

        public IEnumerator TowerCooldown(){
            yield return new WaitForSeconds(60);
            canSpawnTowerB = true;
        }

        public bool CanSpawnTower(){
            if(canSpawnTowerA && canSpawnTowerB){
                return true;
            }
            else{
                return false;
            }
        }
        public void SetNipOrTowerOnScreen(bool setter){
            nipOrTowerOnScreen = setter;
        }

        public bool GetNipOrTowerOnScreen(){
            return nipOrTowerOnScreen;
        }
        
        public void SetDamageReductionPercentage(float reducPercent){
            damageReductionPercentage = reducPercent;
        }
    }
}