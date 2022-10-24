using System;
using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedMultiplier : PackedValue
    {
        public PackedMultiplier(string eventType, float decayTime, string name, float multiplier, string condition)
        {
            this.eventType = eventType;
            Name = name;
            Multiplier = multiplier;
            AccumulatedMultiplier = Multiplier;
            DecayTime = decayTime;
            Condition = condition;
            Condition = condition;
            this.condition = API.GameConditions.GetCondition(Condition);

            if (DecayTime != 0f)
            {
                _timed = true;
            }
        }

        public override PackedType PackedValueType => PackedType.Multiplier;
        public float Multiplier;
        public float AccumulatedMultiplier;
        public float Elapsed { get => _tDecay; }
        public string Condition;
        public Func<bool> condition { get; }

        private bool _timed;
        private bool _timeBegin;

        public override void OnValueCreated()
        {
            _tDecay = DecayTime;

            API.Multiplier.OnMultiplierAdded?.Invoke(this);
        }

        public override void OnValueRemoved()
        {
            AccumulatedMultiplier = Multiplier;
            _timeBegin = false;

            API.Multiplier.OnMultiplierRemoved?.Invoke(this);
        }

        public override void OnUpdate()
        {
            if (condition != null)
            {
                if (!condition())
                {
                    OnValueRemoved();
                }
            }

            OnUpdateDecay();
        }

        public override void OnUpdateDecay()
        {
            if (_timed)
            {
                if (!_timeBegin)
                {
                    API.Multiplier.OnMultiplierTimeBegin?.Invoke(this);
                    _timeBegin = true;
                }

                if (_tDecay <= 0f)
                {
                    API.Multiplier.OnMultiplierTimeExpired?.Invoke(this);
                    _tDecay = DecayTime;
                    ScoreTracker.Instance.Remove(this);
                }

                _tDecay -= Time.deltaTime;
            }
        }
    }
}