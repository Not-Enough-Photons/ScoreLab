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

        [System.Serializable]
        public class UIPadding
        {
            public float leftPadding;
            public float rightPadding;
            public float topPadding;
            public float bottomPadding;
            public float convergence;
        }

        [System.Serializable]
        public class UIScale
        {
            public float leftScale;
            public float rightScale;
            public float topScale;
            public float bottomScale;
            public float hudSize;
            public float followDistance;
            public float followLerp;
        }

        public static UIManager instance;

        public UIPadding paddingSettings;
        public UIScale scaleSettings;

        public UIModule scoreModule;
        public UIModule multiplierModule;
        public UIModule highScoreModule;

        public List<UIModule> scoreSubModules;
        public List<UIModule> multiplierSubModules;

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

        private void Start()
        {
            rootCanvas = transform.GetChild(0);

            InitializeRegions();

            UpdateHighScoreModule(null);

            // HUD settings
            ReadHUDSettings();

            // Follow distance and target
            followTarget = Player.GetPlayerHead().transform;
            followDistance = 3f;
            followLerp = 6f;
        }

        private void OnEnable()
        {
            ScoreworksManager.instance.OnScoreAdded += UpdateScoreModules;
            ScoreworksManager.instance.OnScoreAdded += UpdateScoreSubmodules;
            //ScoreworksManager.instance.OnScoreAdded += UpdateHighScoreModule;

            ScoreworksManager.instance.OnMultiplierAdded += UpdateMultiplierModules;
            ScoreworksManager.instance.OnMultiplierAdded += UpdateMultiplierSubmodules;

            ScoreworksManager.instance.OnMultiplierRemoved += UpdateMultiplierModules;
            ScoreworksManager.instance.OnMultiplierRemoved += UpdateMultiplierSubmodules;

            ScoreworksManager.instance.OnMultiplierChanged += UpdateMultiplierModules;
        }

        private void OnDisable()
        {
            ScoreworksManager.instance.OnScoreAdded -= UpdateScoreModules;
            ScoreworksManager.instance.OnScoreAdded -= UpdateScoreSubmodules;

            ScoreworksManager.instance.OnMultiplierAdded -= UpdateMultiplierModules;
            ScoreworksManager.instance.OnMultiplierAdded -= UpdateMultiplierSubmodules;

            ScoreworksManager.instance.OnMultiplierRemoved -= UpdateMultiplierModules;
            ScoreworksManager.instance.OnMultiplierRemoved -= UpdateMultiplierSubmodules;

            ScoreworksManager.instance.OnMultiplierChanged -= UpdateMultiplierModules;
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
            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json";
            string file = System.IO.File.ReadAllText(path);

            var hudData = JObject.Parse(file);

            var filePaddingSettings = hudData["padding"];

            UIPadding padding = new UIPadding()
            {
                leftPadding = (float)filePaddingSettings["leftPadding"],
                rightPadding = (float)filePaddingSettings["rightPadding"],
                topPadding = (float)filePaddingSettings["topPadding"],
                bottomPadding = (float)filePaddingSettings["bottomPadding"],
                convergence = (float)filePaddingSettings["convergence"],
            };

            paddingSettings = padding;

            var fileScaleSettings = hudData["size"];

            UIScale scale = new UIScale()
            {
                leftScale = (float)fileScaleSettings["leftScale"],
                rightScale = (float)fileScaleSettings["rightScale"],
                topScale = (float)fileScaleSettings["topScale"],
                bottomScale = (float)fileScaleSettings["bottomScale"],
                hudSize = (float)fileScaleSettings["hudScale"],
                followDistance = (float)fileScaleSettings["followDistance"],
                followLerp = (float)fileScaleSettings["followLerp"],
            };

            scaleSettings = scale;
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
                multiplierModule.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString() + "x");
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

        private void UpdateHighScoreModule(Core.Data.SWValue value)
        {
            if (highScoreModule == null)
            {
                return;
            }

            highScoreModule.SetText(highScoreModule.nameText, ScoreworksManager.instance.currentScene);
            highScoreModule.SetText(highScoreModule.valueText, ScoreworksManager.instance.currentHighScore.ToString());

            highScoreModule.gameObject.SetActive(true);
        }

        private void UpdatePadding()
        {
            Transform leftRegionT = this.leftRegion.transform;
            Transform rightRegionT = this.rightRegion.transform;
            Transform topRegionT = this.topRegion.transform;
            Transform bottomRegionT = this.bottomRegion.transform;

            Vector3 lPadding = -leftRegionT.right * (paddingSettings.leftPadding + paddingSettings.convergence);
            Vector3 rPadding = rightRegionT.right * (paddingSettings.rightPadding + paddingSettings.convergence);
            Vector3 tPadding = topRegionT.up * (paddingSettings.topPadding + paddingSettings.convergence);
            Vector3 bPadding = -bottomRegionT.up * (paddingSettings.bottomPadding + paddingSettings.convergence);

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

            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * scaleSettings.followDistance, scaleSettings.followLerp * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.GetChild(0).rotation = lookRot;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform current = transform.GetChild(i);

                if (current)
                {
                    current.LookAt(followTarget);
                }
            }
        }
    }
}

