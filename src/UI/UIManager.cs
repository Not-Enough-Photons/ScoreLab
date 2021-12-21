using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using NEP.Scoreworks.Core;
using NEP.Scoreworks.UI.Modules;

namespace NEP.Scoreworks.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIManager : MonoBehaviour
    {
        public UIManager(System.IntPtr ptr) : base(ptr) { }

        public static UIManager instance { get; private set; }

        public UIModule scoreModule;
        public UIModule multiplierModule;
        public UIModule highScoreModule;

        public UIModule[] scoreSubModules;
        public UIModule[] multiplierSubModules;

        public Transform followTarget = null;
        public float followDistance = 2f;
        public float followLerp = 6f;

        private Vector3 lastTargetPos;
        private Vector3 lastTargetRot;

        private void OnEnable()
        {
            ScoreworksManager.OnScoreAdded += UpdateModule;
            ScoreworksManager.OnScoreAdded += UpdateSubModules;

            ScoreworksManager.OnMultiplierAdded += UpdateModule;
            ScoreworksManager.OnMultiplierAdded += UpdateSubModules;
        }

        private void UpdateModule(Core.Data.SWValue value)
        {
            switch (value.type)
            {
                case Core.Data.SWValueType.Score:
                    if(scoreModule == null)
                    {
                        return;
                    }

                    scoreModule.SetText(scoreModule.nameText, value.name);
                    scoreModule.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
                    scoreModule.SetDuration(value.maxDuration);
                    break;
                case Core.Data.SWValueType.Multiplier:
                    if(multiplierModule == null)
                    {
                        return;
                    }

                    multiplierModule.SetText(multiplierModule.nameText, value.name);
                    multiplierModule.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString() + "x");
                    multiplierModule.SetText(multiplierModule.subValueText, value.multiplier.ToString());
                    multiplierModule.SetDuration(value.maxDuration);
                    break;
            }
        }

        private void UpdateSubModules(Core.Data.SWValue value)
        {
            switch (value.type)
            {
                case Core.Data.SWValueType.Score:
                    UIModule scoreModule = scoreSubModules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                    if(scoreModule == null)
                    {
                        return;
                    }

                    scoreModule.SetText(scoreModule.nameText, value.name);
                    scoreModule.SetText(scoreModule.valueText, ScoreworksManager.instance.currentScore.ToString());
                    scoreModule.SetText(scoreModule.subValueText, value.name + " | " + value.score);
                    scoreModule.SetDuration(value.maxDuration);

                    scoreModule.gameObject.SetActive(true);
                    break;
                case Core.Data.SWValueType.Multiplier:
                    UIModule multiplierModule = multiplierSubModules.FirstOrDefault((current) => !current.gameObject.activeInHierarchy);

                    if(multiplierModule == null)
                    {
                        return;
                    }

                    multiplierModule.SetText(multiplierModule.nameText, value.name);
                    multiplierModule.SetText(multiplierModule.valueText, ScoreworksManager.instance.currentMultiplier.ToString());
                    multiplierModule.SetText(multiplierModule.subValueText, value.name + " " + value.multiplier + "x");
                    multiplierModule.SetSlider(value.maxDuration);
                    multiplierModule.SetDuration(value.maxDuration);

                    multiplierModule.gameObject.SetActive(true);
                    break;
            }
        }

        private void Update()
        {
            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * followDistance, followLerp * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;

        }

        private void LateUpdate()
        {
            lastTargetPos = followTarget.position;
            lastTargetRot = followTarget.eulerAngles;
        }
    }
}

