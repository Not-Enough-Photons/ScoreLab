using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.ScoreLab.UI.Modules
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIModule : MonoBehaviour
    {
        public UIModule(System.IntPtr ptr) : base(ptr) { }
        public Core.Data.SWValue refValue;

        public UIModuleType moduleType;

        public Text nameText;
        public Text valueText;
        public Text subValueText;
        public Slider slider;

        public List<UIModule> submodules;

        public Animator anim;

        public bool useDuration = true;
        public float maxDuration = 1f;
        private float _duration = 0f;

        private float internal_delay = 0.25f;
        private float t_internal_delay;

        private void Awake()
        {
            submodules = new List<UIModule>();

            InitializeModule();
            InitializeSubmodules();
        }

        private void InitializeModule()
        {
            string moduleTypeName = "";

            if (transform.name.StartsWith("Module"))
            {
                moduleTypeName = transform.name.Substring(7);
            }

            if (moduleTypeName == "Score")
            {
                moduleType = UIModuleType.Module_Score;
            }
            else if (moduleTypeName == "Multiplier")
            {
                moduleType = UIModuleType.Module_Multiplier;
            }
            else if (moduleTypeName == "HighScore")
            {
                moduleType = UIModuleType.Module_HighScore;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform currentTransform = transform.GetChild(i);

                if (currentTransform == null)
                {
                    continue;
                }

                string name = currentTransform.name;

                if (name == "NameText")
                {
                    nameText = currentTransform.GetComponentInChildren<Text>();
                }

                if (name == "ValueText")
                {
                    valueText = currentTransform.GetComponentInChildren<Text>();
                }

                if (name == "SubValueText")
                {
                    subValueText = currentTransform.GetComponentInChildren<Text>();
                }

                slider = currentTransform.GetComponentInChildren<Slider>();

                if (GetComponentInChildren<Animator>() != null)
                {
                    anim = GetComponentInChildren<Animator>();
                }
            }

            useDuration = false;
        }

        private void InitializeSubmodules()
        {
            Transform submoduleList = transform.Find("List");

            if (submoduleList == null)
            {
                return;
            }

            for (int i = 0; i < submoduleList.childCount; i++)
            {
                Transform current = submoduleList.GetChild(i);
                UIModule submodule = current.gameObject.AddComponent<UIModule>();

                string name = current.name.Substring(10);

                if (name == "Score")
                {
                    submodule.moduleType = UIModuleType.Module_Score;
                }
                else if (name == "Multiplier")
                {
                    submodule.moduleType = UIModuleType.Module_Multiplier;
                }
                else if (name == "HighScore")
                {
                    submodule.moduleType = UIModuleType.Module_HighScore;
                }

                if (GetComponent<Animator>() != null)
                {
                    anim = GetComponent<Animator>();
                }

                submodule.useDuration = true;

                current.gameObject.SetActive(false);

                submodules.Add(submodule);
            }
        }

        private void OnEnable()
        {
            _duration = 0f;
            t_internal_delay = 0f;
        }

        private void OnDisable()
        {
            refValue = null;
        }

        private void Update()
        {
            if (useDuration)
            {
                _duration += Time.deltaTime;

                if (slider != null)
                {
                    slider.value -= Time.deltaTime;
                }

                if (_duration >= maxDuration)
                {
                    if (anim == null)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        anim.Play("Anim_FadeOut");

                        t_internal_delay += Time.deltaTime;

                        if (t_internal_delay >= internal_delay)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        public void SetDuration(float duration)
        {
            maxDuration = duration;
            _duration = 0f;
        }

        public void SetText(Text text, string value)
        {
            if (text != null)
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
            if (slider != null)
            {
                slider.minValue = min;
                slider.maxValue = max;
                slider.value = value;
            }
        }
    }
}

