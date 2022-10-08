using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Core.Data;
using NEP.ScoreLab.UI.Modules;

using SLZ.Rig;

namespace NEP.ScoreLab.UI
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

        public static Dictionary<SLScoreType, UIModule> scoreDictionary { get; private set; }
        public static Dictionary<SLMultiplierType, UIModule> multDictionary { get; private set; }

        private Transform chest;
        private Transform head;

        private void Start()
        {
            scoreDictionary = new Dictionary<SLScoreType, UIModule>();
            multDictionary = new Dictionary<SLMultiplierType, UIModule>();

            rootCanvas = transform.GetChild(0);

            InitializeRegions();
            InitializeText();

            // Follow distance and target
            RigManager rigManager = FindObjectOfType<RigManager>();
            chest = rigManager.physicsRig.m_chest;
            head = rigManager.physicsRig.m_head.transform; 

            followTarget = rigManager.physicsRig.m_chest;
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

            // HUD settings
            ReadHUDSettings();
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

        private void InitializeText()
        {
            scoreModule.SetText(scoreModule.valueText, ScoreLabManager.instance.currentScore.ToString());
            multiplierModule.SetText(multiplierModule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
            highScoreModule.SetText(highScoreModule.nameText, ScoreLabManager.instance.currentScene);
            highScoreModule.SetText(highScoreModule.valueText, ScoreLabManager.instance.currentHighScore.ToString());
        }

        private void UpdateScoreModules(SLValue value)
        {
            if (value.type == SLValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                scoreModule.SetText(scoreModule.nameText, value.name);
                scoreModule.SetText(scoreModule.valueText, ScoreLabManager.instance.currentScore.ToString());
                scoreModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateScoreSubmodules(SLValue value)
        {
            if (value.type == SLValueType.Score)
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

                if (!scoreDictionary.ContainsKey(value.scoreType))
                {
                    if (submodule != null)
                    {
                        submodule.SetText(submodule.nameText, value.name);
                        submodule.SetText(submodule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
                        submodule.SetText(submodule.subValueText, value.name + " | " + value.score);
                        submodule.SetDuration(value.maxDuration);

                        submodule.gameObject.SetActive(true);
                        scoreDictionary.Add(value.scoreType, submodule);
                    }
                }
            }
        }

        private void UpdateScoreSubmoduleDuplicates(SLValue value)
        {
            if (value.type == SLValueType.Score)
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
                    submodule.SetText(submodule.nameText, value.name);
                    submodule.SetText(submodule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
                    submodule.SetText(submodule.subValueText, value.name + " | " + value.score);
                    submodule.SetDuration(value.maxDuration);
                }
            }
        }

        private void DisableScoreSubmodule(SLValue value)
        {
            scoreDictionary.Remove(value.scoreType);
        }

        private void UpdateMultiplierModules(SLValue value)
        {
            if (value.type == Core.Data.SLValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                multiplierModule.SetText(multiplierModule.nameText, value.name);
                multiplierModule.SetText(multiplierModule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
                multiplierModule.SetText(multiplierModule.subValueText, value.multiplier.ToString());
                multiplierModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateMultiplierSubmodules(SLValue value)
        {
            if (value.type == SLValueType.Multiplier)
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
                        submodule.SetText(submodule.nameText, value.name);
                        submodule.SetText(submodule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
                        submodule.SetText(submodule.subValueText, value.name + " " + value.multiplier.ToString());
                        submodule.SetSlider(value.maxDuration);
                        submodule.SetDuration(value.maxDuration);

                        submodule.gameObject.SetActive(true);
                        multDictionary.Add(value.multiplierType, submodule);
                    }
                }
            }
        }

        private void UpdateMultiplierSubmoduleDuplicates(SLValue value)
        {
            if (value.type == SLValueType.Multiplier)
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
                    submodule.SetText(submodule.nameText, value.name);
                    submodule.SetText(submodule.valueText, ScoreLabManager.instance.currentMultiplier.ToString());
                    submodule.SetText(submodule.subValueText, value.name + " " + value.multiplier.ToString());
                    submodule.SetSlider(value.maxDuration);
                    submodule.SetDuration(value.maxDuration);
                }
            }
        }

        private void DisableMultiplierSubmodule(SLValue value)
        {
            multDictionary.Remove(value.multiplierType);
        }

        private void UpdateHighScoreModule(SLValue value)
        {
            if (highScoreModule == null)
            {
                return;
            }

            highScoreModule.SetText(highScoreModule.nameText, ScoreLabManager.instance.currentScene);
            highScoreModule.SetText(highScoreModule.valueText, ScoreLabManager.instance.currentHighScore.ToString());

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

