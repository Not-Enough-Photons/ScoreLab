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

        private HUDText _testGradeLetterText;
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
            
            _testGradeLetterText = transform.Find("Grade/Value").GetComponent<HUDText>();
        }

        public override void OnModuleEnable()
        {
            base.OnModuleEnable();

            if (ModuleType == UIModuleType.Main)
            {
                SetText(_title, ScoreTracker.Title);
                SetText(_value, ScoreTracker.HighScore);
            }
        }

        public override void OnUpdate()
        {
            UpdateDecay();

            // TODO: NOT release code
            // Just testing the grade system to see what it'll look like
            if (ScoreTracker.HighScore > 500 && ScoreTracker.HighScore < 1000)
            {
                SetText(_testGradeLetterText, _letters[3]);
            }
            else if (ScoreTracker.HighScore > 1000 && ScoreTracker.HighScore < 2000)
            {
                SetText(_testGradeLetterText, _letters[2]);
            }
            else if (ScoreTracker.HighScore > 2000 && ScoreTracker.HighScore < 3000)
            {
                SetText(_testGradeLetterText, _letters[1]);
            }
            else if (ScoreTracker.HighScore > 3000 && ScoreTracker.HighScore < 4000)
            {
                SetText(_testGradeLetterText, _letters[0]);
            }
            else
            {
                SetText(_testGradeLetterText, "F");
            }
            
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

        private void SetTweenValue(int value)
        {
            _targetValue = value;
            _rate = Mathf.Abs(_targetValue - _currentValue) / 1.0f;
        }
    }
}