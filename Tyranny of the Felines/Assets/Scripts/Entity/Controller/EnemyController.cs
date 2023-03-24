using Matryoshka.Abilities;
using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Matryoshka.Game;
using Matryoshka.Lobby;
using Matryoshka.ObjectPool;

namespace Matryoshka.Entity.Controller
{
    public class EnemyController : MonoBehaviour, IController
    {
        public bool canAttack = false;
        private int numberGraceInstances = 0;
        [SerializeField]
        private GameObject clawRangeFinderPref;

        private ClawSwipeRangeFinder clawRange;
        private bool hasTargetInClawRange => clawRange != null && clawRange.hasTargetInClawRange;

        public bool sleeping = false;
        private float last_furballs = 0;

        void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                canAttack = false;
                Debug.Log("clawr");
                clawRange = Instantiate(clawRangeFinderPref).GetComponent<ClawSwipeRangeFinder>();
                Vector2 clawpos = GameManager.Singleton.mittensObject.transform.position;
                clawpos += Utils.Utils.GetClawSwipeOffset(Vector2.down);
                SetRangeFinderPosition(clawpos, 0);
                StopMittensAttack(5.0f);
            }

        }

        public void Create() { }

        public Vector2 GetMovement()
        {
            return new Vector2(0, 0);
        }

        public Vector2 GetMouseDirection()
        {
            return new Vector2(0, 0);
        }

        //Should not get called but needed to implement
        public Vector2 GetMousePosition()
        {
            
            Vector2 position = new Vector2(0, 0);
            return position;
        }

        public AbilityType GetAbility()
        {
            // TEST MITTENS ATTACKS
            if (canAttack && !GetComponent<Entity>().currentlyDoingPhaseTransition && !sleeping)
            {

                if (GetPlayerState() == PlayerState.Dead)
                {
                    return AbilityType.None;
                }

                int randomNumber = 0;

                if(GetComponent<Entity>().entityType == EntityType.Mittens){
                    if (LobbyManager.Singleton != null)
                    {
                        LobbyManager.Singleton.PlaySoundClientRpc("PlayMittens");
                    }
                    if(GetComponent<Entity>().GetMittensPhase() == 0){
                        if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(2, 4);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern()){
                            randomNumber = UnityEngine.Random.Range(1, 4);
                        }
                        else if(!hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 4);
                            if(randomNumber == 1){
                                randomNumber = 0;
                            }
                        }
                        else{
                            randomNumber = UnityEngine.Random.Range(0, 4);
                        }
                        if (randomNumber == 0)
                        {
                            GetComponent<BulletSpawner>().FireRandomPattern();
                            
                        }
                        else if (randomNumber == 1)
                        {
                            return AbilityType.Primary;
                            
                        }
                        else if (randomNumber == 2)
                        {
                            return AbilityType.Secondary;
                            
                        }
                        else if (randomNumber == 3)
                        {
                            // Tail Slam
                            return AbilityType.Special3;
                        }
                    }

                    else if(GetComponent<Entity>().GetMittensPhase() == 1){
                        if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(2, 5);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern()){
                            randomNumber = UnityEngine.Random.Range(1, 5);
                        }
                        else if(!hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 5);
                            if(randomNumber == 1){
                                randomNumber = 0;
                            }
                        }
                        if (randomNumber == 0)
                        {
                            GetComponent<BulletSpawner>().FireRandomPattern();
                            
                        }
                        else if (randomNumber == 1)
                        {
                            return AbilityType.Primary;
                            
                        }
                        else if (randomNumber == 2)
                        {
                            return AbilityType.Secondary;
                            
                        }
                        else if (randomNumber == 3)
                        {
                            // Tail Slam
                            return AbilityType.Special3;
                        }
                        else if (randomNumber == 4)
                        {
                            // Impale
                            return AbilityType.Special1;
                        }
                    }

                    else if(GetComponent<Entity>().GetMittensPhase() == 2){
                        if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange && !GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(2, 5);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(2, 6);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern() && !GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(2, 5);
                        }
                        else if(!GetComponent<Entity>().CanSpawnCatnipOrTower() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 5);
                            if(randomNumber == 1){randomNumber = 0;}
                        }
                        else if(!hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 6);
                            if(randomNumber == 1){
                                randomNumber = 0;
                            }
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern()){
                            randomNumber = UnityEngine.Random.Range(1, 6);
                        }
                        else if(!GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(0, 5);
                        }

                        if (randomNumber == 0)
                        {
                            GetComponent<BulletSpawner>().FireRandomPattern();
                            
                        }
                        else if (randomNumber == 1)
                        {
                            return AbilityType.Primary;
                            
                        }
                        else if (randomNumber == 2)
                        {
                            return AbilityType.Secondary;
                            
                        }
                        else if (randomNumber == 3)
                        {
                            // Tail Slam
                            return AbilityType.Special3;
                        }
                        else if (randomNumber == 4)
                        {
                            // Impale
                            return AbilityType.Special1;
                        }
                        else if (randomNumber == 5)
                        {
                            GetComponent<Entity>().CatnipSpawned();
                            // Spawn Catnip
                            return AbilityType.Special2;
                        }
                    }

                    else if(GetComponent<Entity>().GetMittensPhase() == 3){
                        if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange && !GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(2, 5);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(2, 7);
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern() && !GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(1, 5);
                        }
                        else if(!GetComponent<Entity>().CanSpawnCatnipOrTower() && !hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 5);
                            if(randomNumber == 1){randomNumber = 0;}
                        }
                        else if(!hasTargetInClawRange){
                            randomNumber = UnityEngine.Random.Range(1, 7);
                            if(randomNumber == 1){
                                randomNumber = 0;
                            }
                        }
                        else if(GetComponent<BulletSpawner>().IsFiringPattern()){
                            randomNumber = UnityEngine.Random.Range(1, 7);
                        }
                        else if(!GetComponent<Entity>().CanSpawnCatnipOrTower()){
                            randomNumber = UnityEngine.Random.Range(0, 5);
                        }
                        if (randomNumber == 0)
                        {
                            GetComponent<BulletSpawner>().FireRandomPattern();
                            
                        }
                        else if (randomNumber == 1)
                        {
                            return AbilityType.Primary;
                            
                        }
                        else if (randomNumber == 2)
                        {
                            return AbilityType.Secondary;
                            
                        }
                        else if (randomNumber == 3)
                        {
                            // Tail Slam
                            return AbilityType.Special3;
                        }
                        else if (randomNumber == 4)
                        {
                            // Impale
                            return AbilityType.Special1;
                        }
                        else if (randomNumber == 5)
                        {
                            if(GetComponent<Entity>().GetNipOrTowerOnScreen()){
                                //If nip or tower on screen do impale
                                return AbilityType.Special1;
                            }
                            // Spawn Catnip
                            GetComponent<Entity>().CatnipSpawned();
                            return AbilityType.Special2;
                        }
                        else if (randomNumber == 6)
                        {
                            if(!GetComponent<Entity>().CanSpawnTower()){
                                if(GetComponent<Entity>().GetNipOrTowerOnScreen()){
                                    // Tail Slam if cant do nip or tower
                                    return AbilityType.Special3;
                                }
                                GetComponent<Entity>().CatnipSpawned();
                                return AbilityType.Special2;
                            }
                            else{
                                if(GetComponent<Entity>().GetNipOrTowerOnScreen()){
                                    // Tail Slam if cant do nip or tower
                                    return AbilityType.Special3;
                                }
                                GetComponent<Entity>().TowerSpawned();
                                // Spawn Tower
                                return AbilityType.Special4;
                            } 
                        }
                    }
                }
            } else if (sleeping)
            {
                if (Time.time - last_furballs >= 1)
                {
                    const float ArenaLeft = -21f;
                    const float ArenaRight = 21f;
                    const float ArenaTop = 15f;
                    const float ArenaBottom = -17f;
                    Vector2[] pos = new Vector2[] {new Vector2(ArenaLeft+1, 0), new Vector2(ArenaRight - 1, 0), new Vector2(0, ArenaTop - 1), new Vector2(0, ArenaBottom+1) };

                    foreach (var p in pos)
                    {
                        NetworkObject projectileObject = NetworkObjectPool.Singleton.GetNetworkObject(
                        GameManager.Singleton.furballPrefab,
                        p,
                        Quaternion.identity
                        );
                        projectileObject.Spawn(true);
                        IController e = projectileObject.gameObject.GetComponent<IController>();
                        if (e != null)
                        {
                            e.Create();
                        }
                    }
                    last_furballs = Time.time;
                }
            }
            return AbilityType.None;
        }

        public PlayerState GetPlayerState()
        {
            return GetComponent<Entity>().networkPlayerState.Value;
        }

        public void StopMittensAttack(float duration)
        {
                canAttack = false;
                numberGraceInstances++;
                StartCoroutine(Grace(duration));
        }

        private IEnumerator Grace(float duration)
        {
            yield return new WaitForSeconds(duration);
            numberGraceInstances--;
            if (numberGraceInstances <= 0)
            {
                numberGraceInstances = 0;
                canAttack = true;
            }
        }

        public void SetRangeFinderPosition(Vector2 position, int phase)
        {
            Debug.Log($"setpos {position} {phase}");
            clawRange.SetPosition(position, phase);
        }
    }
}
