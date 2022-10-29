using UnityEngine;
using UnityEngine.UI;

using TMPro;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIMultiplierModule : UIModule
    {
        public UIMultiplierModule(System.IntPtr ptr) : base(ptr) { }

        private PackedMultiplier _packedMultiplier { get => (PackedMultiplier)_packedValue; }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (_packedValue == null)
            {
                return;
            }

            if (_packedMultiplier == null)
            {
                return;
            }

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_value, ScoreTracker.Instance.Multiplier.ToString());
            }
            else if (ModuleType == UIModuleType.Descriptor)
            {
                if (PackedValue.Stackable)
                {
                    if (PackedValue.TierEventType != null)
                    {
                        SetText(_title, _packedMultiplier.Name);
                        SetText(_value, _packedMultiplier.Multiplier);
                    }
                    else
                    {
                        SetText(_title, _packedMultiplier.Name);
                        SetText(_value, _packedMultiplier.AccumulatedMultiplier);
                    }
                }
                else
                {
                    SetText(_title, _packedMultiplier.Name);
                    SetText(_value, _packedMultiplier.Multiplier);
                }
            }

            if (_timeBar != null)
            {
                if (_packedMultiplier.Condition != null)
                {
                    _timeBar.gameObject.SetActive(false);
                }
                else
                {
                    _timeBar.gameObject.SetActive(true);
                    SetMaxValueToBar(_timeBar, _packedMultiplier.Elapsed);
                }
            }
        }

        public override void OnModuleDisable()
        {

        }

        public override void OnUpdate()
        {
            if (_packedMultiplier != null)
            {
                if (_packedMultiplier.condition != null)
                {
                    if (!_packedMultiplier.condition())
                    {
                        UpdateDecay();
                    }
                }
                else
                {
                    UpdateDecay();
                }
            }
            else
            {
                UpdateDecay();
            }

            if (_timeBar != null)
            {
                SetBarValue(_timeBar, _tDecay);
            }
        }
    }
}
