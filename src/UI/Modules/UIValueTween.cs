using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIValueTween : MonoBehaviour
    {
        public UIValueTween(System.IntPtr ptr) : base(ptr) { }

        public int Value;
        public int TargetValue;
        public int Rate = 2;
        public TMPro.TextMeshProUGUI text;

        private int _targetValue;
        private int _previousValue;
        private int _value;

        private void Awake()
        {
            text = GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void SetValue(int value)
        {
            _previousValue = Value;
            _targetValue = value;
            _value = _previousValue;

            TargetValue = _targetValue;
        }

        public void Tween()
        {
            if (_value < _targetValue)
            {
                _value += Mathf.CeilToInt(Time.unscaledDeltaTime) * Rate;
            }

            if (_value >= _targetValue)
            {
                _value = _targetValue;
            }

            Value = _value;
        }

        private void Update()
        {
            Tween();

            if(text == null)
            {
                return;
            }

            text.text = $"{Value}";
        }
    }
}

