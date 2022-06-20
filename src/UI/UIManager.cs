using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using NEP.Scoreworks.Core;
using NEP.Scoreworks.UI.Modules;

namespace NEP.Scoreworks.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public struct UIPadding
        {
            public float leftPadding;
            public float rightPadding;
            public float topPadding;
            public float bottomPadding;

            public float convergence;
        }

        public static UIManager instance { get; private set; }

        public UIPadding paddingSettings;

        public UIModule scoreModule;
        public UIModule multiplierModule;
        public UIModule highScoreModule;

        public UIModule[] scoreSubModules;
        public UIModule[] multiplierSubModules;

        public Transform leftRegion;
        public Transform rightRegion;
        public Transform topRegion;
        public Transform bottomRegion;

        public Transform followTarget = null;
        public float followDistance = 2f;
        public float followLerp = 6f;

        private Vector3 lastTargetPos;
        private Vector3 lastTargetRot;
        
        private void Start()
        {
            Transform root = transform.Find("RootCanvas");

            leftRegion = root.Find("Region_Left");
            rightRegion = root.Find("Region_Right");
            topRegion = root.Find("Region_Top");
            bottomRegion = root.Find("Region_Bottom");

            Transform left_R_Score = leftRegion.Find("Score");
            Transform right_R_Multipliers = rightRegion.Find("Multipliers");
            Transform left_R_LevelBest = left_R_Score.Find("LevelBest");

            scoreModule = left_R_Score.gameObject.AddComponent<UIModule>();
            multiplierModule = right_R_Multipliers.gameObject.AddComponent<UIModule>();
            highScoreModule = left_R_LevelBest.gameObject.AddComponent<UIModule>();

            Transform scoreList = scoreModule.transform.Find("List");
            Transform multiplierList = multiplierModule.transform.Find("List");

            for(int i = 0; i < scoreList.childCount; i++)
            {
                Transform current = scoreList.GetChild(i);

                if (current)
                {
                    UIModule module = current.gameObject.AddComponent<UIModule>();

                    module.moduleType = UIModuleType.Module_Score;
                    module.subValueText = current.transform.Find("ScoreText").GetComponent<Text>();
                }
            }

            for (int i = 0; i < multiplierList.childCount; i++)
            {
                Transform current = multiplierList.GetChild(i);

                if (current)
                {
                    UIModule module = current.gameObject.AddComponent<UIModule>();

                    module.moduleType = UIModuleType.Module_Multiplier;
                    module.subValueText = current.transform.Find("MultiplierText").GetComponent<Text>();
                    module.slider = current.transform.Find("MultiplierText/Slider").GetComponent<Slider>();
                }
            }

            highScoreModule.moduleType = UIModuleType.Module_LastScore;
            highScoreModule.useDuration = false;

            paddingSettings = new UIPadding()
            {
                leftPadding = 50f,
                rightPadding = 100f,
                convergence = -400f
            };

            followTarget = GameObject.Find("Camera (eye)").transform;
            followDistance = 4f;
            followLerp = 6f;
        }

        private void OnEnable()
        {
            ScoreworksManager.OnScoreAdded += UpdateScoreModules;
            ScoreworksManager.OnScoreAdded += UpdateScoreSubmodules;

            ScoreworksManager.OnMultiplierAdded += UpdateMultiplierModules;
            ScoreworksManager.OnMultiplierAdded += UpdateMultiplierSubmodules;
            ScoreworksManager.OnMultiplierRemoved += UpdateMultiplierModules;
            ScoreworksManager.OnMultiplierRemoved += UpdateMultiplierSubmodules;
            ScoreworksManager.OnMultiplierChanged += UpdateMultiplierModules;
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

        private void Update()
        {
            UpdatePadding();

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * followDistance, followLerp * Time.deltaTime);
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

        private void LateUpdate()
        {
            lastTargetPos = followTarget.position;
            lastTargetRot = followTarget.eulerAngles;
        }
    }
}

