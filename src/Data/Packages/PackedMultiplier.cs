using System;
using UnityEngine;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [System.Serializable]
    public class PackedMultiplier : PackedValue
    {
        public PackedMultiplier(string name, float multiplier) : base(name)
        {
            this.name = name;
            this.multiplier = multiplier;
            _timed = false;
        }

        public PackedMultiplier(string name, float multiplier, float timer) : base(name)
        {
            this.name = name;
            this.multiplier = multiplier;
            this.timer = timer;
            _timed = true;
        }

        public PackedMultiplier(string name, float multiplier, Func<bool> condition) : base(name)
        {
            this.name = name;
            this.multiplier = multiplier;
            this.condition = condition;
        }

        public override PackedType packedType => PackedType.Multiplier;
        public float multiplier;
        public float timer;
        public float elapsed;
        public Func<bool> condition;

        private bool _timed;
        private bool _timeBegin;

        public override void OnValueCreated()
        {
            ScoreTracker.Instance.AddMultiplier(multiplier);
            ScoreTracker.Instance.ActiveMultipliers.Add(this);

            API.Multiplier.OnMultiplierAdded?.Invoke(this);
        }

        public override void OnValueRemoved()
        {
            ScoreTracker.Instance.RemoveMultiplier(multiplier);
            ScoreTracker.Instance.ActiveMultipliers.Remove(this);

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

                elapsed += Time.deltaTime;

                if (elapsed > timer)
                {
                    API.Multiplier.OnMultiplierTimeExpired?.Invoke(this);
                    ScoreTracker.Instance.Remove(this);
                    elapsed = 0f;
                }
            }
        }
    }
}