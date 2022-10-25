using UnityEngine;
using UnityEngine.UI;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

using TMPro;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIScoreModule : UIModule
    {
        public UIScoreModule(System.IntPtr ptr) : base(ptr) { }

        private PackedScore _packedScore { get => (PackedScore)_packedValue; }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_value, ScoreTracker.Instance.Score.ToString());
            }
            else if (ModuleType == UIModuleType.Descriptor)
            {
                if (_packedValue == null)
                {
                    return;
                }

                SetText(_title, _packedScore.Name);
                SetText(_value, _packedScore.AccumulatedScore.ToString());
            }
        }

        public override void OnModuleDisable()
        {

        }

        public override void OnUpdate()
        {
            UpdateDecay();
        }
    }
}