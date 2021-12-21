using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NEP.Scoreworks.Core;

namespace NEP.Scoreworks.UI.Modules
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIModule : MonoBehaviour
    {
        public UIModule(System.IntPtr ptr) : base(ptr) { }

        public UIModuleType moduleType;

        public Text nameText;
        public Text valueText;
        public Text subValueText;
        public Slider slider;

        public bool useDuration = true;
        public float maxDuration = 1f;
        private float _duration = 0f;

        private void OnEnable()
        {
            _duration = 0f;
        }

        private void Update()
        {
            if (useDuration)
            {
                _duration += Time.deltaTime;

                if(slider != null)
                {
                    slider.value -= Time.deltaTime;
                }
                
                if(_duration >= maxDuration)
                {
                    gameObject.SetActive(false);
                    _duration = 0f;
                }
            }
        }

        public void SetDuration(float duration)
        {
            maxDuration = duration;
        }

        public void SetText(Text text, string value)
        {
            if(text != null)
            {
                text.text = value;
            }
        }

        public void SetSlider(float value)
        {
            SetSlider(0f, value, value);
        }

        private void SetSlider(float min, float max, float value)
        {
            if(slider != null)
            {
                slider.minValue = min;
                slider.maxValue = max;
                slider.value = value;
            }
        }
    }
}

