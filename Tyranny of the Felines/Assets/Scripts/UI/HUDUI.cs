using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Abilities;
using Matryoshka.Entity;
using UnityEngine;
using UnityEngine.UIElements;
using Matryoshka.Game;
using Matryoshka.Http;
using Unity.Netcode;
using UnityEngine.VFX;

namespace Matryoshka.UI
{
    public class HUDUI : NetworkBehaviour
    {
        public static HUDUI Singleton { get; private set; }
        public VisualElement HUD { get; private set; }
        private ProgressBar MittensHealth;
        private NetworkVariable<float> mittensHealthValue;
        private bool intialized = false;
        private Entity.Entity mittensObject;

        private VisualElement BigPrimaryCooldown;
        private VisualElement BigSecondaryCooldown;
        private VisualElement BigAbilityCooldown;
        private ProgressBar BigCharacterHealth;
        
        private VisualElement WazoPrimaryCooldown;
        private VisualElement WazoSecondaryCooldown;
        private VisualElement WazoAbilityCooldown;
        private ProgressBar WazoCharacterHealth;
        
        private VisualElement SalumonPrimaryCooldown;
        private VisualElement SalumonSecondaryCooldown;
        private VisualElement SalumonAbilityCooldown;
        private ProgressBar SalumonCharacterHealth;

        private NetworkVariable<float> bigHealthValue;
        private NetworkVariable<float> wazoHealthValue;
        private NetworkVariable<float> salumonHealthValue;

        private Entity.Entity bigObject;
        private Entity.Entity wazoObject;
        private Entity.Entity salumonObject;

        private NetworkVariable<bool> bigExist;
        private NetworkVariable<bool> wazoExist;
        private NetworkVariable<bool> salumonExist;

        private bool initialHUDSet = false;

        private IMGUIContainer bigHUDContainer;
        private IMGUIContainer wazoHUDContainer;
        private IMGUIContainer salumonHUDContainer;

        private VisualElement mittensPhase1;
        private VisualElement mittensPhase2;
        private VisualElement mittensPhase3;
        private VisualElement mittensPhase4;

        private NetworkVariable<int> phase;
        private int localPhase = -1;

        void Start()
        {
            Singleton = this;

            var root = GetComponent<UIDocument>().rootVisualElement;

            HUD = root.Q<VisualElement>(Constants.HUD);
            MittensHealth = root.Q<ProgressBar>(Constants.MittensHealthBar);

            BigPrimaryCooldown = root.Q<VisualElement>("BigPrimaryCooldown");
            BigSecondaryCooldown = root.Q<VisualElement>("BigSecondaryCooldown");
            BigAbilityCooldown = root.Q<VisualElement>("BigAbilityCooldown");
            BigCharacterHealth = root.Q<ProgressBar>("BigCharacterHealthBar");

            WazoPrimaryCooldown = root.Q<VisualElement>("WazoPrimaryCooldown");
            WazoSecondaryCooldown = root.Q<VisualElement>("WazoSecondaryCooldown");
            WazoAbilityCooldown = root.Q<VisualElement>("WazoAbilityCooldown");
            WazoCharacterHealth = root.Q<ProgressBar>("WazoCharacterHealthBar");

            SalumonPrimaryCooldown = root.Q<VisualElement>("SalumonPrimaryCooldown");
            SalumonSecondaryCooldown = root.Q<VisualElement>("SalumonSecondaryCooldown");
            SalumonAbilityCooldown = root.Q<VisualElement>("SalumonAbilityCooldown");
            SalumonCharacterHealth = root.Q<ProgressBar>("SalumonCharacterHealthBar");

            bigHUDContainer = root.Q<IMGUIContainer>("BigHUDContainer");
            wazoHUDContainer = root.Q<IMGUIContainer>("WazoHUDContainer");
            salumonHUDContainer = root.Q<IMGUIContainer>("SalumonHUDContainer");
            
            // initially hide the UI HUDs to avoid quick flash
            bigHUDContainer.style.display = DisplayStyle.None;
            wazoHUDContainer.style.display = DisplayStyle.None;
            salumonHUDContainer.style.display = DisplayStyle.None;

            mittensPhase1 = root.Q<VisualElement>("MittensIcon0");
            mittensPhase2 = root.Q<VisualElement>("MittensIcon1");
            mittensPhase3 = root.Q<VisualElement>("MittensIcon2");
            mittensPhase4 = root.Q<VisualElement>("MittensIcon3");

            MittensHealth.title = Constants.MittensName;
        }

        void OnEnable()
        {
            InitializeIfNotIntialized();
        }
        
        public void HideHud()
        {
            HUD.style.display = DisplayStyle.None;
        }

        public void StartCooldownTimer(EntityType entityType, AbilityType type, float time)
        {
            switch (entityType)
            {
                case EntityType.Big:
                    if (type == AbilityType.Primary)
                    {
                        BigPrimaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, BigPrimaryCooldown));
                    }
                    else if (type == AbilityType.Secondary)
                    {
                        BigSecondaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, BigSecondaryCooldown));
                    }
                    else if (type == AbilityType.Special1)
                    {
                        BigAbilityCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, BigAbilityCooldown));
                    }
                    break;
                case EntityType.Wazo:
                    if (type == AbilityType.Primary)
                    {
                        WazoPrimaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, WazoPrimaryCooldown));
                    }
                    else if (type == AbilityType.Secondary)
                    {
                        WazoSecondaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, WazoSecondaryCooldown));
                    }
                    else if (type == AbilityType.Special1)
                    {
                        WazoAbilityCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, WazoAbilityCooldown));
                    }
                    break;
                case EntityType.Salumon:
                    if (type == AbilityType.Primary)
                    {
                        SalumonPrimaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, SalumonPrimaryCooldown));
                    }
                    else if (type == AbilityType.Secondary)
                    {
                        SalumonSecondaryCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, SalumonSecondaryCooldown));
                    }
                    else if (type == AbilityType.Special1)
                    {
                        SalumonAbilityCooldown.style.backgroundColor = new Color(0,0,0,1);
                        StartCoroutine(DecreaseCooldown(time, SalumonAbilityCooldown));
                    }
                    break;
            }
        }

        [ClientRpc]
        public void UpdateMittensMaxHealthClientRpc(float newMaxHealth)
        {
            MittensHealth.highValue = newMaxHealth;
        }

        private IEnumerator DecreaseCooldown(float time, VisualElement cooldown)
        {
            while (cooldown.style.backgroundColor.value.a > 0)
            {
                yield return new WaitForSeconds(time/100);
                cooldown.style.backgroundColor = new Color(0, 0, 0, 
                    cooldown.style.backgroundColor.value.a - 0.01f);
            }

            cooldown.style.backgroundColor = new Color(0, 0, 0, 0);
        }

        public void InitializeIfNotIntialized()
        {
            if (NetworkManager.Singleton.IsServer && !intialized)
            {
                intialized = true;
                mittensHealthValue = new NetworkVariable<float>(1000f);
                bigHealthValue = new NetworkVariable<float>(200f);
                wazoHealthValue = new NetworkVariable<float>(100f);
                salumonHealthValue = new NetworkVariable<float>(120f);
                bigExist = new NetworkVariable<bool>(false);
                wazoExist = new NetworkVariable<bool>(false);
                salumonExist = new NetworkVariable<bool>(false);
                phase = new NetworkVariable<int>(0);
            }
        }

        void FixedUpdate()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (mittensObject == null)
                {
                    Init();
                }

                mittensHealthValue.Value = mittensObject.GetHealthValue();
                if (bigExist.Value)
                {
                    bigHealthValue.Value = bigObject.GetHealthValue();
                }

                if (wazoExist.Value)
                {
                    wazoHealthValue.Value = wazoObject.GetHealthValue();
                }

                if (salumonExist.Value)
                {
                    salumonHealthValue.Value = salumonObject.GetHealthValue();
                }

                phase.Value = mittensObject.mittensPhase;
            }

            if (!initialHUDSet)
            {
                InitializeHUD();
            }

            if (mittensHealthValue != null)
            {
                MittensHealth.value = (float) mittensHealthValue.Value;
            }

            if (bigHealthValue != null)
            {
                BigCharacterHealth.value = bigHealthValue.Value;
            }

            if (wazoHealthValue != null)
            {
                WazoCharacterHealth.value = wazoHealthValue.Value;
            }

            if (salumonHealthValue != null)
            {
                SalumonCharacterHealth.value = salumonHealthValue.Value;
            }

            if (phase != null)
            {
                switch (phase.Value)
                {
                    case 0:
                        if (localPhase == -1)
                        {
                            mittensPhase1.style.display = DisplayStyle.Flex;
                            localPhase = 0;
                        }
                        break;
                    case 1:
                        if (localPhase == 0)
                        {
                            StartCoroutine(StartSecondPhase());
                            localPhase = 1;
                        }
                        break;
                    case 2:
                        if (localPhase == 1)
                        {
                            StartCoroutine(StartThirdPhase());
                            localPhase = 2;
                        }
                        break;
                    case 3:
                        if (localPhase == 2)
                        {
                            StartCoroutine(StartFourthPhase());
                            localPhase = 3;
                        }
                        break;
                }
            }
            
        }

        private IEnumerator StartFourthPhase()
        {
            mittensPhase3.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.5f);
            mittensPhase4.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.5f);
            mittensPhase4.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase4.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase4.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase4.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase4.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase4.style.display = DisplayStyle.Flex;
        }
        
        private IEnumerator StartThirdPhase()
        {
            mittensPhase2.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.5f);
            mittensPhase3.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.5f);
            mittensPhase3.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase3.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase3.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase3.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase3.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase3.style.display = DisplayStyle.Flex;
        }

        private IEnumerator StartSecondPhase()
        {
            yield return new WaitForSeconds(0.5f);
            mittensPhase2.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.5f);
            mittensPhase2.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase2.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase2.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase2.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.2f);
            mittensPhase2.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.2f);
            mittensPhase2.style.display = DisplayStyle.Flex;
        }

        private void InitializeHUD()
        {
            switch (bigExist.Value, wazoExist.Value, salumonExist.Value)
            {
                // all 3
                case (true, true, true):
                    bigHUDContainer.style.display = DisplayStyle.Flex;
                    wazoHUDContainer.style.display = DisplayStyle.Flex;
                    salumonHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // big and wazo
                case (true, true, false):
                    bigHUDContainer.style.display = DisplayStyle.Flex;
                    wazoHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // big and salumon
                case (true, false, true):
                    salumonHUDContainer.style.top = new StyleLength(Length.Percent(12));
                    bigHUDContainer.style.display = DisplayStyle.Flex;
                    salumonHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // big
                case (true, false, false):
                    bigHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // wazo and salumon
                case (false, true, true):
                    salumonHUDContainer.style.top = new StyleLength(Length.Percent(12));
                    wazoHUDContainer.style.top = new StyleLength(Length.Percent(2));
                    wazoHUDContainer.style.display = DisplayStyle.Flex;
                    salumonHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // wazo
                case (false, true, false):
                    wazoHUDContainer.style.top = new StyleLength(Length.Percent(2));
                    wazoHUDContainer.style.display = DisplayStyle.Flex;
                    break;
                // salumon
                case (false, false, true):
                    salumonHUDContainer.style.top = new StyleLength(Length.Percent(2));
                    salumonHUDContainer.style.display = DisplayStyle.Flex;
                    break;
            }

            initialHUDSet = true;
        }

        private void Init()
        {
            if (GameManager.Singleton != null)
            {
                mittensObject = GameManager.GetMittens().GetComponent<Entity.Entity>();
                
                foreach (var player in GameManager.Singleton.playerObjects)
                {
                    if (player.GetComponent<Entity.Entity>().entityType == EntityType.Big)
                    {
                        bigExist.Value = true;
                    }

                    if (player.GetComponent<Entity.Entity>().entityType == EntityType.Wazo)
                    {
                        wazoExist.Value = true;
                    }

                    if (player.GetComponent<Entity.Entity>().entityType == EntityType.Salumon)
                    {
                        salumonExist.Value = true;
                    }
                }
                
                if (bigExist.Value)
                {
                    bigObject = GameManager.GetEntityWithType(EntityType.Big).GetComponent<Entity.Entity>();
                }

                if (wazoExist.Value)
                {
                    wazoObject = GameManager.GetEntityWithType(EntityType.Wazo).GetComponent<Entity.Entity>();
                }

                if (salumonExist.Value)
                {
                    salumonObject = GameManager.GetEntityWithType(EntityType.Salumon).GetComponent<Entity.Entity>();
                }
            }
        }
    }
}
