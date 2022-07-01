using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using NEP.Scoreworks.Core;
using NEP.Scoreworks.UI.Modules;

using ModThatIsNotMod;

using Newtonsoft.Json.Linq;

namespace NEP.Scoreworks.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public static UIManager instance;

        public UIPadding paddingSettings;
        public UIScale scaleSettings;

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

        public bool useHead = true;

        private Transform playerHead = Player.GetPlayerHead().transform;
        private Transform playerTorso;

        private void Start()
        {
            StressLevelZero.Rig.RigManager rigManager = Player.GetRigManager().GetComponent<StressLevelZero.Rig.RigManager>();
            playerTorso = rigManager.physicsRig.m_chest;

            rootCanvas = transform.GetChild(0);

            InitializeRegions();

            // HUD settings
            ReadHUDSettings();

            // Update all texts for when the player changes HUDs
            UpdateModulesOnLoad();
        }

        private void OnEnable()
        {
            API.OnScoreAdded += UpdateScoreModules;
            API.OnScoreAdded += UpdateScoreSubmodules;
            API.OnHighScoreReached += UpdateHighScoreModule;

            API.OnMultiplierAdded += UpdateMultiplierModules;
            API.OnMultiplierAdded += UpdateMultiplierSubmodules;

            API.OnMultiplierRemoved += UpdateMultiplierModules;
            API.OnMultiplierRemoved += UpdateMultiplierSubmodules;

            API.OnMultiplierChanged += UpdateMultiplierModules;
        }

        private void OnDisable()
        {
            API.OnScoreAdded -= UpdateScoreModules;
            API.OnScoreAdded -= UpdateScoreSubmodules;
            API.OnHighScoreReached -= UpdateHighScoreModule;

            API.OnMultiplierAdded -= UpdateMultiplierModules;
            API.OnMultiplierAdded -= UpdateMultiplierSubmodules;

            API.OnMultiplierRemoved -= UpdateMultiplierModules;
            API.OnMultiplierRemoved -= UpdateMultiplierSubmodules;

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

                    print(module.name);

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
            paddingSettings = Core.Data.DataManager.ReadPadding();
            scaleSettings = Core.Data.DataManager.ReadScale();
        }

        private void UpdateScoreModules(Core.Data.SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                scoreModule.SetText(scoreModule.nameText, value.name);
                scoreModule.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
                scoreModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateScoreSubmodules(Core.Data.SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Score)
            {
                if (scoreModule == null)
                {
                    return;
                }

                if (scoreModule.submodules == null || scoreModule.submodules.Count <= 0)
                {
                    return;
                }

                UIModule submodule = scoreModule.submodules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                if (submodule == null)
                {
                    return;
                }

                submodule.SetText(submodule.nameText, value.name);
                submodule.SetText(submodule.valueText, ScoreworksManager.instance.currentScore.ToString());
                submodule.SetText(submodule.subValueText, value.name + " | " + value.score);
                submodule.SetDuration(value.maxDuration);

                submodule.gameObject.SetActive(true);
            }
        }

        private void UpdateMultiplierModules(Core.Data.SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                multiplierModule.SetText(multiplierModule.nameText, value.name);
                multiplierModule.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString());
                multiplierModule.SetText(multiplierModule.subValueText, value.multiplier.ToString());
                multiplierModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateMultiplierSubmodules(Core.Data.SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Multiplier)
            {
                if (multiplierModule == null)
                {
                    return;
                }

                UIModule submodule = multiplierModule.submodules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                if (submodule == null)
                {
                    return;
                }

                submodule.SetText(submodule.nameText, value.name);
                submodule.SetText(submodule.valueText, ScoreworksManager.instance.currentMultiplier.ToString());
                submodule.SetText(submodule.subValueText, value.name + " " + value.multiplier + "x");
                submodule.SetSlider(value.maxDuration);
                submodule.SetDuration(value.maxDuration);

                if (value.cleaned)
                {
                    return;
                }

                submodule.gameObject.SetActive(true);
            }
        }

        private void UpdateHighScoreModule(string currentScene, int newScore)
        {
            if (highScoreModule == null)
            {
                return;
            }

            highScoreModule.SetText(highScoreModule.nameText, ScoreworksManager.instance.currentScene);
            highScoreModule.SetText(highScoreModule.valueText, ScoreworksManager.instance.currentHighScore.ToString());

            highScoreModule.gameObject.SetActive(true);
        }

        private void UpdateModulesOnLoad()
        {
            scoreModule?.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
            multiplierModule?.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString());
            highScoreModule?.SetText(highScoreModule.nameText, ScoreworksManager.instance.currentScene);
            highScoreModule?.SetText(highScoreModule.valueText, ScoreworksManager.instance.currentHighScore.ToString());
        }

        private void UpdatePadding()
        {
            Transform leftRegionT = this.leftRegion.transform;
            Transform rightRegionT = this.rightRegion.transform;
            Transform topRegionT = this.topRegion.transform;
            Transform bottomRegionT = this.bottomRegion.transform;

            Vector3 paddingLeft = new Vector3(paddingSettings.leftPadding[0], paddingSettings.leftPadding[1], paddingSettings.leftPadding[2]);
            Vector3 paddingRight = new Vector3(paddingSettings.rightPadding[0], paddingSettings.rightPadding[1], paddingSettings.rightPadding[2]);
            Vector3 paddingTop = new Vector3(paddingSettings.topPadding[0], paddingSettings.topPadding[1], paddingSettings.topPadding[2]);
            Vector3 paddingBottom = new Vector3(paddingSettings.bottomPadding[0], paddingSettings.bottomPadding[1], paddingSettings.bottomPadding[2]);

            Vector3 lPadding = -leftRegionT.right + (leftRegionT.InverseTransformDirection(paddingLeft * 100));
            Vector3 rPadding = rightRegionT.right + (rightRegionT.InverseTransformDirection(paddingRight * 100));
            Vector3 tPadding = topRegionT.up + (topRegionT.InverseTransformDirection(paddingTop * 100));
            Vector3 bPadding = -bottomRegionT.up + (bottomRegionT.InverseTransformDirection(paddingBottom * 100));

            leftRegion.transform.localPosition = leftRegionT.InverseTransformDirection(lPadding);
            rightRegion.transform.localPosition = rightRegionT.InverseTransformDirection(rPadding);
            topRegion.transform.localPosition = topRegionT.InverseTransformDirection(tPadding);
            bottomRegion.transform.localPosition = bottomRegionT.InverseTransformDirection(bPadding);
        }

        private void UpdateRegionScale()
        {
            Transform leftRegion = this.leftRegion.transform;
            Transform rightRegion = this.rightRegion.transform;
            Transform topRegion = this.topRegion.transform;
            Transform bottomRegion = this.bottomRegion.transform;

            leftRegion.localScale = Vector3.one * scaleSettings.leftScale / 10f;
            rightRegion.localScale = Vector3.one * scaleSettings.rightScale / 10f;
            topRegion.localScale = Vector3.one * scaleSettings.topScale / 10f;
            bottomRegion.localScale = Vector3.one * scaleSettings.bottomScale / 10f;
            rootCanvas.localScale = new Vector3(-scaleSettings.hudSize / 100f, scaleSettings.hudSize / 100f, scaleSettings.hudSize / 100f);
        }

        public void Update()
        {
            UpdatePadding();
            UpdateRegionScale();
            
            followTarget = useHead ? playerHead : playerTorso;
        }

        public void FixedUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * scaleSettings.followDistance, scaleSettings.followLerp * Time.fixedDeltaTime);
            Quaternion lookRot = Quaternion.LookRotation(-followTarget.forward);

            transform.position = move;
            rootCanvas.rotation = lookRot;
        }
    }
}