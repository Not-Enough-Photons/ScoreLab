using UnityEngine;
using UnityEngine.UI;

using TMPro;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    public class UIMultiplierModule : UIModule
    {
        private PackedMultiplier _packedMultiplier { get => (PackedMultiplier)_packedValue; }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (_packedValue == null)
            {
                return;
            }

            if(_packedMultiplier == null)
            {
                return;
            }

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_value, ScoreTracker.Instance.Multiplier.ToString());
            }
            else if (ModuleType == UIModuleType.Descriptor)
            {
                SetText(_title, _packedMultiplier.name);
                SetText(_value, _packedMultiplier.multiplier.ToString());
            }

            if(_timeBar != null)
            {
                if(_packedMultiplier.condition != null)
                {
                    _timeBar.gameObject.SetActive(false);
                }
                else
                {
                    _timeBar.gameObject.SetActive(true);
                    SetMaxValueToBar(_timeBar, _packedMultiplier.timer);
                }
            }
        }

        public override void OnModuleDisable()
        {

        }

        public override void OnUpdate()
        {
            if(_packedMultiplier != null)
            {
                if(_packedMultiplier.condition != null)
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

            if(_timeBar != null)
            {
                SetBarValue(_timeBar, _tDecay);
            }
        }
    }
}
