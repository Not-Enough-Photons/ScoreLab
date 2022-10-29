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
            Add(Create(eventType));
        }

        public void Add(PackedValue value)
        {
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                SetPackedScore((PackedScore)value);
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                SetPackedMultiplier((PackedMultiplier)value);
            }
        }

        public void Remove(PackedValue value)
        {
            if (value.PackedValueType == PackedValue.PackedType.Score)
            {
                ActiveValues.Remove(value);

                value.OnValueRemoved();

                API.Score.OnScoreRemoved?.Invoke((PackedScore)value);
            }
            else if (value.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                ActiveValues.Remove(value);
                PackedMultiplier mult = value as PackedMultiplier;
                RemoveMultiplier(mult.AccumulatedMultiplier);

                value.OnValueRemoved();

                API.Multiplier.OnMultiplierRemoved?.Invoke((PackedMultiplier)value);
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

        private void SetPackedScore(PackedScore score)
        {
            if (score == null)
            {
                return;
            }

            if (!CheckDuplicate(score))
            {
                InitializeValue(score);
                AddScore(score.Score);
                ActiveValues.Add(score);

                API.Score.OnScoreAdded?.Invoke(score);
                return;
            }

            if (score.Tiers != null)
            {
                var _scoreInList = GetClone<PackedScore>(score);
                var _tier = (PackedScore)_scoreInList.CurrentTier;

                _scoreInList.SetDecayTime(_tier.DecayTime);

                AddScore(_tier.Score);
                API.Score.OnScoreTierReached?.Invoke(_tier);
            }
            else if (score.Stackable)
            {
                var _scoreInList = GetClone<PackedScore>(score);

                AddScore(score.Score);

                _scoreInList.SetDecayTime(_scoreInList.DecayTime);
                _scoreInList.AccumulatedScore += _scoreInList.Score;

                API.Score.OnScoreAccumulated?.Invoke(_scoreInList);
            }
            else
            {
                PackedScore copy = CopyFromScore(score);
                InitializeValue(copy);
                AddScore(copy.Score);
                ActiveValues.Add(copy);

                API.Score.OnScoreAdded?.Invoke(copy);
            }
        }

        private void SetPackedMultiplier(PackedMultiplier multiplier)
        {
            if (multiplier == null)
            {
                return;
            }

            if (!CheckDuplicate(multiplier))
            {
                InitializeValue(multiplier);
                AddMultiplier(multiplier.Multiplier);
                ActiveValues.Add(multiplier);

                API.Multiplier.OnMultiplierAdded?.Invoke(multiplier);
                return;
            }

            if (multiplier.Tiers != null)
            {
                var _multInList = GetClone<PackedMultiplier>(multiplier);
                var _tier = (PackedMultiplier)_multInList.CurrentTier;

                _multInList.SetDecayTime(_tier.DecayTime);

                AddMultiplier(multiplier.Multiplier);
                API.Multiplier.OnMultiplierTierReached?.Invoke(_tier);
            }
            else if (multiplier.Stackable)
            {
                var _multInList = GetClone<PackedMultiplier>(multiplier);

                AddMultiplier(multiplier.Multiplier);

                _multInList.SetDecayTime(_multInList.DecayTime);
                _multInList.AccumulatedMultiplier += _multInList.Multiplier;

                API.Multiplier.OnMultiplierAccumulated?.Invoke(_multInList);
            }
            else
            {
                PackedMultiplier copy = CopyFromMult(multiplier);
                InitializeValue(copy);
                AddMultiplier(multiplier.Multiplier);
                ActiveValues.Add(copy);

                API.Multiplier.OnMultiplierAdded?.Invoke(copy);
            }
        }

        private PackedScore CopyFromScore(PackedScore original)
        {
            PackedScore score = new PackedScore()
            {
                eventType = original.eventType,
                Name = original.Name,
                Score = original.Score,
                EventAudio = original.EventAudio,
                DecayTime = original.DecayTime
            };

            return score;
        }

        private PackedMultiplier CopyFromMult(PackedMultiplier original)
        {
            PackedMultiplier score = new PackedMultiplier()
            {
                eventType = original.eventType,
                Name = original.Name,
                Multiplier = original.Multiplier,
                EventAudio = original.EventAudio,
                DecayTime = original.DecayTime,
                Condition = original.Condition
            };

            return score;
        }

        private PackedValue Create(string eventType)
        {
            var Event = DataManager.PackedValues.Get(eventType);

            if (Event.PackedValueType == PackedValue.PackedType.Score)
            {
                var scoreEvent = (PackedScore)Event;

                var score = new PackedScore()
                {
                    eventType = scoreEvent.eventType,
                    Stackable = scoreEvent.Stackable,
                    DecayTime = scoreEvent.DecayTime,
                    Name = scoreEvent.Name,
                    Score = scoreEvent.Score,
                    EventAudio = scoreEvent.EventAudio,
                    Tiers = scoreEvent.Tiers
                };

                return score;
            }
            else if (Event.PackedValueType == PackedValue.PackedType.Multiplier)
            {
                var multEvent = (PackedMultiplier)Event;

                var mult = new PackedMultiplier()
                {
                    eventType = multEvent.eventType,
                    Stackable = multEvent.Stackable,
                    DecayTime = multEvent.DecayTime,
                    Name = multEvent.Name,
                    Multiplier = multEvent.Multiplier,
                    Condition = multEvent.Condition,
                    EventAudio = multEvent.EventAudio,
                    Tiers = multEvent.Tiers
                };

                return mult;
            }

            return null;
        }

        private void InitializeValue(PackedValue value)
        {
            value.OnValueCreated();
            value.SetDecayTime(value.DecayTime);
        }

        private T GetClone<T>(PackedValue value) where T : PackedValue
        {
            return (T)ActiveValues.Find((match) => match.eventType == value.eventType);
        }
    }
}

