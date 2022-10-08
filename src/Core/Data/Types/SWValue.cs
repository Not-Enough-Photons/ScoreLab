namespace NEP.ScoreLab.Core.Data
{
    [System.Serializable]
    public class SWValue
    {
        public SWValue(SWScoreType scoreType)
        {
            var dictionary = DataManager.scoreValues;
            SLValueTemplate valueTemplate = dictionary[scoreType];

            this.scoreType = scoreType;

            name = valueTemplate.name;
            score = valueTemplate.score;
            type = SWValueType.Score;
            stack = valueTemplate.stack;

            duration = maxDuration;

            CreateScore(this);
        }

        public SWValue(SWMultiplierType multiplierType)
        {
            var dictionary = DataManager.multiplierValues;
            SLValueTemplate valueTemplate = dictionary[multiplierType];

            this.multiplierType = multiplierType;

            name = valueTemplate.name;
            score = valueTemplate.score;
            multiplier = valueTemplate.multiplier;
            maxDuration = valueTemplate.maxDuration;
            duration = maxDuration;
            type = SWValueType.Multiplier;
            stack = valueTemplate.stack;

            CreateMultiplier(this);
        }

        public string name;
        public int score;
        public float multiplier;
        public bool stack;

        public SWScoreType scoreType;
        public SWMultiplierType multiplierType;

        public bool cleaned = false;

        public SWValueType type;

        public float maxDuration = 5f;
        public float duration;

        public void CreateScore(SWValue value)
        {
            if (value.type == SWValueType.Score)
            {
                API.OnScorePreAdded?.Invoke(value);
                API.OnScoreAdded?.Invoke(value);
            }
        }

        public void DestroyScore(SWValue value)
        {
            if (value.type == SWValueType.Score)
            {
                API.OnScorePreRemoved?.Invoke(value);
                API.OnScoreRemoved?.Invoke(value);
            }
        }

        public void CreateMultiplier(SWValue value)
        {
            if (value.type == SWValueType.Multiplier)
            {
                API.OnMultiplierPreAdded?.Invoke(value);
                API.OnMultiplierAdded?.Invoke(value);
                API.OnMultiplierChanged?.Invoke(value);
            }
        }

        public void DestroyMultiplier(SWValue value)
        {
            if (value.type == SWValueType.Multiplier)
            {
                API.OnMultiplierPreRemoved?.Invoke(value);
                API.OnMultiplierRemoved?.Invoke(value);
                API.OnMultiplierChanged?.Invoke(value);
            }
        }

        public void Update()
        {
            duration -= UnityEngine.Time.deltaTime;

            if (duration <= 0f)
            {
                if (type == SWValueType.Score)
                {
                    DestroyScore(this);
                }
                else if (type == SWValueType.Multiplier)
                {
                    DestroyMultiplier(this);
                }
            }
        }

        public void ResetDuration()
        {
            duration = maxDuration;
        }
    }
}