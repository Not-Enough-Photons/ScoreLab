using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using NEP.Scoreworks.Core;
using NEP.Scoreworks.Core.Data;
using NEP.Scoreworks.UI.Modules;

using StressLevelZero.Rig;

namespace NEP.Scoreworks.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public static UIManager instance;

        public UISettings hudSettings;

        public UIModule scoreModule;
        public UIModule multiplierModule;
        public UIModule highScoreModule;

        public UIRegion[] regions;

        public GameObject uiObject;

        public Transform rootCanvas;

        public UIRegion leftRegion;
        public UIRegion rightRegion;
        public UIRegion topRegion;
        public UIRegion bottomRegion;

        public Transform followTarget = null;
        public float followDistance = 2f;
        public float followLerp = 6f;

        public static Dictionary<SWScoreType, UIModule> scoreDictionary { get; private set; }
        public static Dictionary<SWMultiplierType, UIModule> multDictionary { get; private set; }

        private Transform chest;
        private Transform head;

        private void Start()
        {
            scoreDictionary = new Dictionary<SWScoreType, UIModule>();
            multDictionary = new Dictionary<SWMultiplierType, UIModule>();

            rootCanvas = transform.GetChild(0);

            InitializeRegions();

            InitializeTexts();

            // HUD settings
            ReadHUDSettings();

            // Follow distance and target
            RigManager rigManager = ModThatIsNotMod.Player.GetRigManager().GetComponent<RigManager>();
            chest = rigManager.physicsRig.m_chest;
            head = ModThatIsNotMod.Player.GetPlayerHead().transform; 

            followTarget = rigManager.physicsRig.m_chest;
            followDistance = 3f;
            followLerp = 6f;
        }

        private void OnEnable()
        {
            API.OnScoreAdded += UpdateScoreModules;
            API.OnScoreAdded += UpdateScoreSubmodules;
            API.OnScoreDuplicated += UpdateScoreSubmoduleDuplicates;
            API.OnHighScoreReached += UpdateHighScoreModule;

            API.OnMultiplierAdded += UpdateMultiplierModules;
            API.OnMultiplierAdded += UpdateMultiplierSubmodules;
            API.OnMultiplierDuplicated += UpdateMultiplierSubmoduleDuplicates;

            API.OnScoreRemoved += DisableScoreSubmodule;
            API.OnMultiplierRemoved += UpdateMultiplierModules;
            API.OnMultiplierRemoved += UpdateMultiplierSubmodules;
            API.OnMultiplierRemoved += DisableMultiplierSubmodule;

            API.OnMultiplierChanged += UpdateMultiplierModules;
        }

        private void OnDisable()
        {
            API.OnScoreAdded -= UpdateScoreModules;
            API.OnScoreAdded -= UpdateScoreSubmodules;
            API.OnScoreDuplicated -= UpdateScoreSubmoduleDuplicates;
            API.OnHighScoreReached -= UpdateHighScoreModule;

            API.OnMultiplierAdded -= UpdateMultiplierModules;
            API.OnMultiplierAdded -= UpdateMultiplierSubmodules;
            API.OnMultiplierDuplicated -= UpdateMultiplierSubmoduleDuplicates;

            API.OnScoreRemoved -= DisableScoreSubmodule;
            API.OnMultiplierRemoved -= UpdateMultiplierModules;
            API.OnMultiplierRemoved -= UpdateMultiplierSubmodules;
            API.OnMultiplierRemoved -= DisableMultiplierSubmodule;

            API.OnMultiplierChanged -= UpdateMultiplierModules;
        }

        private void InitializeRegions()
        {
            // Regions
            Transform leftRegion = rootCanvas.Find("Region_Left");
            Transform rightRegion = rootCanvas.Find("Region_Right");
            Transform topRegion = rootCanvas.Find("Region_Top");
            Transform bottomRegion = rootCanvas.Find("Region_Bottom");

            this.leftRegion = leftRegion.gameObject.AddComponent<UIRegion>();
            this.rightRegion = rightRegion.gameObject.AddComponent<UIRegion>();
            this.topRegion = topRegion.gameObject.AddComponent<UIRegion>();
            this.bottomRegion = bottomRegion.gameObject.AddComponent<UIRegion>();

            regions = new UIRegion[4]
            {
                this.leftRegion,
                this.rightRegion,
                this.topRegion,
                this.bottomRegion
            };

            for (int i = 0; i < regions.Length; i++)
            {
                if (regions[i] == null)
                {
                    continue;
                }

                for (int k = 0; k < regions[i].modules.Count; k++)
                {
                    UIModule module = regions[i].modules[k];

                    if (module.moduleType == UIModuleType.Module_Score)
                    {
                        scoreModule = module;
                    }

                    if (module.moduleType == UIModuleType.Module_Multiplier)
                    {
                        multiplierModule = module;
                    }

                    if (module.moduleType == UIModuleType.Module_HighScore)
                    {
                        highScoreModule = module;
                    }
                }
            }
        }

        private void ReadHUDSettings()
        {
            hudSettings = DataManager.ReadHUDSettings();
        }

        private void InitializeTexts()
        {
            if (scoreModule != null)
            {
                if (scoreModule.valueText)
                {
                    scoreModule.SetValueText(DataManager.OutputDisplayTextFromRegex(scoreModule.valueText.text));
                }

                if (scoreModule.subValueText)
                {
                    scoreModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(scoreModule.subValueText.text));
                }
            }

            if (multiplierModule != null)
            {
                if (multiplierModule.valueText)
                {
                    multiplierModule.SetValueText(DataManager.OutputDisplayTextFromRegex(multiplierModule.valueText.text));
                }

                if (multiplierModule.subValueText)
                {
                    multiplierModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(multiplierModule.subValueText.text));
                }
            }

            if (highScoreModule != null)
            {
                if (highScoreModule.valueText)
                {
                    highScoreModule.SetValueText(DataManager.OutputDisplayTextFromRegex(highScoreModule.valueText.text));
                }

                if (highScoreModule.subValueText)
                {
                    highScoreModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(highScoreModule.subValueText.text));
                }
            }
        }

        private void UpdateScoreModules(SWValue value)
        {
            if (value.type == SWValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                if (scoreModule.valueText != null)
                {
                    scoreModule.SetValueText(scoreModule.initialValueText);
                    scoreModule.SetValueText(DataManager.OutputDisplayTextFromRegex(scoreModule.valueText.text, value));
                }

                if (scoreModule.subValueText != null)
                {
                    scoreModule.SetSubValueText(scoreModule.initialSubValueText);
                    scoreModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(scoreModule.subValueText.text, value));
                }

                scoreModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateScoreSubmodules(SWValue value)
        {
            if (value.type == SWValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                if (scoreModule.submodules == null || scoreModule.submodules.Count <= 0)
                {
                    return;
                }

                var submodule = scoreModule.submodules.FirstOrDefault((mod) => !mod.isActiveAndEnabled);

                Text valueText = submodule.valueText;
                Text subValueText = submodule.subValueText;

                if (!scoreDictionary.ContainsKey(value.scoreType))
                {
                    if (submodule != null)
                    {
                        if (valueText != null)
                        {
                            submodule.SetValueText(submodule.initialValueText);
                            submodule.SetValueText(DataManager.OutputDisplayTextFromRegex(valueText.text, value));
                        }

                        if (subValueText != null)
                        {
                            submodule.SetSubValueText(submodule.initialSubValueText);
                            submodule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(subValueText.text, value));
                        }

                        submodule.SetDuration(value.maxDuration);

                        submodule.gameObject.SetActive(true);
                        scoreDictionary.Add(value.scoreType, submodule);
                    }
                }
            }
        }

        private void UpdateScoreSubmoduleDuplicates(SWValue value)
        {
            if (value.type == SWValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                if (scoreModule.submodules == null || scoreModule.submodules.Count <= 0)
                {
                    return;
                }

                var submodule = scoreDictionary[value.scoreType];

                if (submodule != null)
                {
                    if (submodule.valueText != null)
                    {
                        submodule.SetValueText(submodule.initialValueText);
                        submodule.SetValueText(DataManager.OutputDisplayTextFromRegex(submodule.valueText.text, value));
                    }

                    if (submodule.subValueText != null)
                    {
                        submodule.SetSubValueText(submodule.initialSubValueText);
                        submodule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(submodule.subValueText.text, value));
                    }

                    submodule.SetDuration(value.maxDuration);
                }
            }
        }

        private void DisableScoreSubmodule(SWValue value)
        {
            scoreDictionary.Remove(value.scoreType);
        }

        private void UpdateMultiplierModules(SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                if (multiplierModule.valueText != null)
                {
                    multiplierModule.SetValueText(multiplierModule.initialValueText);
                    multiplierModule.SetValueText(DataManager.OutputDisplayTextFromRegex(multiplierModule.valueText.text, value));
                }

                if (multiplierModule.subValueText != null)
                {
                    multiplierModule.SetSubValueText(multiplierModule.initialSubValueText);
                    multiplierModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(multiplierModule.subValueText.text, value));
                }


                multiplierModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateMultiplierSubmodules(SWValue value)
        {
            if (value.type == SWValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                if (multiplierModule.submodules == null || multiplierModule.submodules.Count <= 0)
                {
                    return;
                }

                var submodule = multiplierModule.submodules.FirstOrDefault((mod) => !mod.isActiveAndEnabled);

                if (!multDictionary.ContainsKey(value.multiplierType))
                {
                    if (submodule != null)
                    {
                        if (submodule.valueText != null)
                        {
                            submodule.SetValueText(submodule.initialValueText);
                            submodule.SetValueText(DataManager.OutputDisplayTextFromRegex(submodule.valueText.text, value));
                        }

                        if (submodule.subValueText != null)
                        {
                            submodule.SetSubValueText(submodule.initialSubValueText);
                            submodule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(submodule.subValueText.text, value));
                        }


                        submodule.SetSlider(value.maxDuration);
                        submodule.SetDuration(value.maxDuration);

                        submodule.gameObject.SetActive(true);
                        multDictionary.Add(value.multiplierType, submodule);
                    }
                }
            }
        }

        private void UpdateMultiplierSubmoduleDuplicates(SWValue value)
        {
            if (value.type == SWValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                if (multiplierModule.submodules == null || multiplierModule.submodules.Count <= 0)
                {
                    return;
                }

                var submodule = multDictionary[value.multiplierType];

                if (submodule != null)
                {
                    if (submodule.valueText != null)
                    {
                        submodule.SetValueText(submodule.initialValueText);
                        submodule.SetValueText(DataManager.OutputDisplayTextFromRegex(submodule.valueText.text, value));
                    }

                    if (submodule.subValueText != null)
                    {
                        submodule.SetSubValueText(submodule.initialSubValueText);
                        submodule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(submodule.subValueText.text, value));
                    }

                    submodule.SetSlider(value.maxDuration);
                    submodule.SetDuration(value.maxDuration);
                }
            }
        }

        private void DisableMultiplierSubmodule(SWValue value)
        {
            multDictionary.Remove(value.multiplierType);
        }

        private void UpdateHighScoreModule(SWValue value)
        {
            if (highScoreModule == null)
            {
                return;
            }

            if (highScoreModule.valueText != null)
            {
                highScoreModule.SetValueText(highScoreModule.initialValueText);
                highScoreModule.SetValueText(DataManager.OutputDisplayTextFromRegex(highScoreModule.valueText.text, value));
            }

            if (highScoreModule.subValueText != null)
            {
                highScoreModule.SetSubValueText(highScoreModule.initialSubValueText);
                highScoreModule.SetSubValueText(DataManager.OutputDisplayTextFromRegex(highScoreModule.subValueText.text, value));
            }

            highScoreModule.gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 targetAxis = hudSettings.followHead ? followTarget.forward : -followTarget.up;
            followTarget = hudSettings.followHead ? head : chest;

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + targetAxis * hudSettings.followDistance, hudSettings.followLerp * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    transform.GetChild(i).LookAt(followTarget);
                }
            }
        }
    }
}

