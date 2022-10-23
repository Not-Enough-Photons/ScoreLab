using System;
using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedMultiplier : PackedValue
    {
        public PackedMultiplier(string eventType, string name, float multiplier, float timer, string condition)
        {
            this.eventType = eventType;
            Name = name;
            Multiplier = multiplier;
            Timer = timer;
            Condition = condition;
        }

        public PackedMultiplier(string eventType)
        {
            var data = (PackedMultiplier)DataManager.PackedValues.Get(eventType);

            this.Name = data.Name;
            this.Multiplier = data.Multiplier;
            this.Timer = data.Timer;
            this.Condition = data.Condition;
            this.condition = API.GameConditions.GetCondition(data.Condition);
            this.eventType = eventType;

            if (Timer != 0f)
            {
                _timed = true;
            }
        }

        public override PackedType PackedValueType => PackedType.Multiplier;
        public float Multiplier;
        public float Timer;
        public float Elapsed;
        public string Condition;
        public Func<bool> condition { get; }

        private bool _timed;
        private bool _timeBegin;

        public override void OnValueCreated()
        {
            ScoreTracker.Instance.AddMultiplier(Multiplier);
            ScoreTracker.Instance.ActiveValues.Add(this);
            API.Multiplier.OnMultiplierAdded?.Invoke(this);
        }

        public override void OnValueRemoved()
        {
            ScoreTracker.Instance.RemoveMultiplier(Multiplier);
            ScoreTracker.Instance.ActiveValues.Remove(this);
            API.Multiplier.OnMultiplierRemoved?.Invoke(this);
        }

        public override void OnUpdate()
        {
            if (condition != null)
            {
                if (!condition())
                {
                    ScoreTracker.Instance.Remove(this);
                }
            }

            if (_timed)
            {
                if (!_timeBegin)
                {
                    API.Multiplier.OnMultiplierTimeBegin?.Invoke(this);
                    _timeBegin = true;
                }

                Elapsed += Time.deltaTime;

                if (Elapsed > Timer)
                {
                    API.Multiplier.OnMultiplierTimeExpired?.Invoke(this);
                    ScoreTracker.Instance.Remove(this);
                    Elapsed = 0f;
                }
            }
        }
    }
}