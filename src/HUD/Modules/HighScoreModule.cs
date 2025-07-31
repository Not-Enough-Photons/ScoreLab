using System;

using MelonLoader;

using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.HUD
{
    [RegisterTypeInIl2Cpp]
    public class HighScoreModule : Module
    {
        public HighScoreModule(IntPtr intPtr) : base(intPtr) { }

        private float _targetValue;
        private float _currentValue;
        private float _rate = 4f;

        private GameObject _gradeObject;
        private HUDText _gradeLetter;
        private string[] _letters =
        {
            "A",
            "B",
            "C",
            "D"
        };
        
        private void Awake()
        {
            if (name == "HighScoreDescriptor")
            {
                ModuleType = UIModuleType.Descriptor;
            }

            _gradeObject = transform.Find("Grade").gameObject;
            _gradeLetter = _gradeObject.transform.Find("Value").GetComponent<HUDText>();
        }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_title, ScoreTracker.Title);
                SetText(_value, ScoreTracker.HighScore);
            }

            UpdateGrade();
        }

        public override void OnUpdate()
        {
            UpdateDecay();

            UpdateGrade();
            
            if (ModuleType == UIModuleType.Main)
            { 
                SetTweenValue(ScoreTracker.HighScore);
                _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _rate * Time.unscaledDeltaTime);
                if (Mathf.Approximately(_currentValue, _targetValue))
                {
                    _currentValue = _targetValue;
                }
              
                SetText(_title, ScoreTracker.Title);
                SetText(_value, _currentValue.ToString("N0"));
            }
        }

        private void UpdateGrade()
        {
            if (ScoreTracker.Grade == null)
            {
                _gradeObject.SetActive(false);
                return;
            }
            
            _gradeObject.SetActive(true);
            SetText(_gradeLetter, ScoreTracker.Grade.grade);
        }

        private void SetTweenValue(int value)
        {
            _targetValue = value;
            _rate = Mathf.Abs(_targetValue - _currentValue) / 1.0f;
        }
    }
}