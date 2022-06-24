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

        public class UIPadding
        {
            public float leftPadding;
            public float rightPadding;
            public float topPadding;
            public float bottomPadding;
            public float convergence;
        }

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

        public GameObject uiObject;

        public Transform rootCanvas;

        public Transform leftRegion;
        public Transform rightRegion;
        public Transform topRegion;
        public Transform bottomRegion;

        public Transform followTarget = null;
        public float followDistance = 2f;
        public float followLerp = 6f;

        private void Start()
        {
            rootCanvas = transform.GetChild(0);

            // Regions
            leftRegion = rootCanvas.Find("Region_Left");
            rightRegion = rootCanvas.Find("Region_Right");
            topRegion = rootCanvas.Find("Region_Top");
            bottomRegion = rootCanvas.Find("Region_Bottom");

            Transform scoreT = leftRegion.Find("Score");
            Transform multT = rightRegion.Find("Multipliers");

            InitializeScore(scoreT);
            InitializeMultipliers(scoreT);
            InitializeHighScore(scoreT);
            InitializeSubModuleScore(scoreT);
            InitializeSubModuleMultipliers(multT);

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

        private void InitializeScore(Transform root)
        {
            // Score

            Text scoreText = root.Find("ScoreText").GetComponent<Text>();

            scoreModule = root.gameObject.AddComponent<UIModule>();
            scoreModule.valueText = scoreText;
            scoreModule.useDuration = false;
        }

        private void InitializeMultipliers(Transform root)
        {
            // Multipliers
            Transform multiplierT = rightRegion.Find("Multipliers");
            Text multiplierText = multiplierT.Find("MultiplierText").GetComponent<Text>();

            multiplierModule = multiplierT.gameObject.AddComponent<UIModule>();
            multiplierModule.valueText = multiplierText;
            multiplierModule.useDuration = false;
        }

        private void InitializeHighScore(Transform root)
        {
            // High Score
            Transform highScoreT = root.Find("LevelBest");
            Text highScoreText = highScoreT.Find("ScoreBest").GetComponent<Text>();

            highScoreModule = highScoreT.gameObject.AddComponent<UIModule>();
            highScoreModule.valueText = highScoreText;
            highScoreModule.useDuration = false;
        }

        private void InitializeSubModuleScore(Transform root)
        {
            // Sub Module - Score Blocks
            scoreSubModules = new List<UIModule>();

            Transform scoreListT = root.Find("List");

            for (int i = 0; i < scoreListT.childCount; i++)
            {
                GameObject current = scoreListT.GetChild(i).gameObject;

                if (current)
                {
                    UIModule module = current.AddComponent<UIModule>();
                    module.subValueText = current.transform.Find("ScoreText").GetComponent<Text>();
                    scoreSubModules?.Add(module);
                    current.SetActive(false);
                }
            }
        }

        private void InitializeSubModuleMultipliers(Transform root)
        {
            // Sub Module - Score Blocks
            multiplierSubModules = new List<UIModule>();

            Transform multListT = root.Find("List");

            for (int i = 0; i < multListT.childCount; i++)
            {
                GameObject current = multListT.GetChild(i).gameObject;

                if (current)
                {
                    UIModule module = current.AddComponent<UIModule>();
                    module.subValueText = current.transform.Find("MultiplierText").GetComponent<Text>();
                    module.slider = current.transform.Find("MultiplierText/Slider").GetComponent<Slider>();
                    multiplierSubModules?.Add(module);
                    current.SetActive(false);
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
                };

                scoreModule.SetText(scoreModule.nameText, value.name);
                scoreModule.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
                scoreModule.SetDuration(value.maxDuration);
            }
        }

        private void UpdateScoreSubmodules(Core.Data.SWValue value)
        {
            if (value.type == Core.Data.SWValueType.Score)
            {
                UIModule scoreModule = scoreSubModules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                if (scoreModule == null)
                {
                    return;
                }

                scoreModule.SetText(scoreModule.nameText, value.name);
                scoreModule.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
                scoreModule.SetText(scoreModule.subValueText, value.name + " | " + value.score);
                scoreModule.SetDuration(value.maxDuration);

                if (value.cleaned)
                {
                    return;
                }

                scoreModule.gameObject.SetActive(true);
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
                UIModule multiplierModule = multiplierSubModules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                if (multiplierModule == null)
                {
                    return;
                }

                multiplierModule.SetText(multiplierModule.nameText, value.name);
                multiplierModule.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString());
                multiplierModule.SetText(multiplierModule.subValueText, value.name + " " + value.multiplier + "x");
                multiplierModule.SetSlider(value.maxDuration);
                multiplierModule.SetDuration(value.maxDuration);

                if (value.cleaned)
                {
                    return;
                }

                multiplierModule.gameObject.SetActive(true);
            }
        }

        private void UpdatePadding()
        {
            Vector3 lPadding = -leftRegion.right * (paddingSettings.leftPadding + paddingSettings.convergence);
            Vector3 rPadding = rightRegion.right * (paddingSettings.rightPadding + paddingSettings.convergence);
            Vector3 tPadding = topRegion.up * (paddingSettings.topPadding + paddingSettings.convergence);
            Vector3 bPadding = -bottomRegion.up * (paddingSettings.bottomPadding + paddingSettings.convergence);

            leftRegion.transform.localPosition = leftRegion.InverseTransformDirection(lPadding);
            rightRegion.transform.localPosition = rightRegion.InverseTransformDirection(rPadding);
            topRegion.transform.localPosition = topRegion.InverseTransformDirection(tPadding);
            bottomRegion.transform.localPosition = bottomRegion.InverseTransformDirection(bPadding);
        }

        private void UpdateRegionScale()
        {
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

            if(followTarget == null)
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

