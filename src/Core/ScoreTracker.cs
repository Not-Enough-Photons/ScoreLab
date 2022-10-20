using System.Collections.Generic;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.Core
{
    public class ScoreTracker
    {
        public ScoreTracker() => Initialize();

        public static ScoreTracker Instance { get; private set; }

        public List<PackedMultiplier> ActiveMultipliers { get; set; }

        public int Score
        {
            get => _score;
        }
        public float Multiplier
        {
            get => _multiplier;
        }

        private int _score = 0;
        private float _multiplier = 1f;

        public void Initialize()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            ActiveMultipliers = new List<PackedMultiplier>();
        }

        public void Update()
        {
            for(int i = 0; i < ActiveMultipliers.Count; i++)
            {
                ActiveMultipliers[i].OnUpdate();
            }
        }

        public void Add(PackedValue value) => value.OnValueCreated();
        public void Remove(PackedValue value) => value.OnValueRemoved();
        public void UpdateValue(PackedValue value) => value.OnUpdate();

        public void AddScore(int score) => _score += score;
        public void AddMultiplier(float multiplier) => _multiplier += multiplier;
        public void RemoveMultiplier(float multiplier) => _multiplier -= multiplier;

    }
}

