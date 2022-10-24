using System.Collections.Generic;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.Core
{
    public class ScoreTracker
    {
        public ScoreTracker() => Initialize();

        public static ScoreTracker Instance { get; private set; }

        public List<PackedValue> ActiveValues { get; private set; }

        public int Score
        {
            get => _score;
        }
        public int ScoreDifference
        {
            get => _scoreDifference;
        }
        public int LastScore
        {
            get => _lastScore;
        }
        public float Multiplier
        {
            get => _multiplier;
        }

        private int _score = 0;
        private int _scoreDifference = 0;
        private int _lastScore = 0;
        private float _multiplier = 1f;

        private float _baseMultiplier = 1f;

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            ActiveValues = new List<PackedValue>();
        }

        public void Update()
        {
            for (int i = 0; i < ActiveValues.Count; i++)
            {
                ActiveValues[i].OnUpdate();
            }
        }

        public void Add(string eventType)
        {
            var value = DataManager.PackedValues.Get(eventType);
            Add(value);
        }

        public void Add(PackedValue value)
        {
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                PackedScore score = (PackedScore)value;
                AddScore(score.Score);

                if (!CheckDuplicate(value))
                {
                    ActiveValues.Add(value);
                }
                else
                {
                    score.AccumulatedScore += score.Score;
                }

                value.OnValueCreated();
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                PackedMultiplier mult = (PackedMultiplier)value;
                AddMultiplier(mult.Multiplier);

                if (!CheckDuplicate(value))
                {
                    ActiveValues.Add(value);
                }
                else
                {
                    mult.AccumulatedMultiplier += mult.Multiplier;
                }

                value.OnValueCreated();
            }
        }

        public void Remove(PackedValue value)
        {
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                ActiveValues.Remove(value);

                value.OnValueRemoved();
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                ActiveValues.Remove(value);
                PackedMultiplier mult = value as PackedMultiplier;
                RemoveMultiplier(mult.AccumulatedMultiplier);

                value.OnValueRemoved();
            }
        }

        public void AddScore(int score)
        {
            _lastScore = _score;
            _score += UnityEngine.Mathf.RoundToInt(score * _multiplier);
            _scoreDifference = _score - _lastScore;
        }

        public void AddMultiplier(float multiplier)
        {
            _multiplier += multiplier;
        }

        public void RemoveMultiplier(float multiplier)
        {
            if (_multiplier < _baseMultiplier)
            {
                _multiplier = _baseMultiplier;
            }

            _multiplier -= multiplier;
        }

        public bool CheckDuplicate(PackedValue value)
        {
            foreach (var val in ActiveValues)
            {
                if (val.eventType == value.eventType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

