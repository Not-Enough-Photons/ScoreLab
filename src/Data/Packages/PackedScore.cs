using System;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedScore : PackedValue
    {
        public PackedScore(string eventType, string name, int score, float decayTime = 10f)
        {
            this.eventType = eventType;
            Name = name;
            Score = score;
            DecayTime = decayTime;
            AccumulatedScore = this.Score;
        }

        public override PackedType PackedValueType => PackedType.Score;
        public int Score;
        public int AccumulatedScore;

        public override void OnValueCreated()
        {
            _tDecay = DecayTime;

            API.Score.OnScoreAdded?.Invoke(this);
        }

        public override void OnValueRemoved()
        {
            AccumulatedScore = Score;

            API.Score.OnScoreRemoved?.Invoke(this);
        }

        public override void OnUpdate()
        {
            OnUpdateDecay();
        }

        public override void OnUpdateDecay()
        {
            if (_tDecay <= 0f)
            {
                ScoreTracker.Instance.Remove(this);
            }

            _tDecay -= UnityEngine.Time.deltaTime;
        }
    }
}

