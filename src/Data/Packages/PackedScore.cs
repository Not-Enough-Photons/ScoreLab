using System;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.Data
{
    [Serializable]
    public class PackedScore : PackedValue
    {
        public PackedScore(string eventType, string name, int score)
        {
            this.eventType = eventType;
            this.Name = name;
            this.score = score;
        }

        public PackedScore(string eventType)
        {
            var data = (PackedScore)DataManager.PackedValues.Get(eventType);

            this.Name = data.Name;
            this.score = data.score;
            this.eventType = eventType;
        }

        public override PackedType PackedValueType => PackedType.Score;
        public int score;

        public override void OnValueCreated()
        {
            //UnityEngine.Debug.Log(ScoreTracker.Instance.CheckDuplicate(this));
            ScoreTracker.Instance.ActiveValues.Add(this);
            ScoreTracker.Instance.AddScore(score);
            API.Score.OnScoreAdded?.Invoke(this);
        }

        public override void OnValueRemoved()
        {
            return;
        }
    }
}

