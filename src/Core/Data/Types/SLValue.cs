namespace NEP.ScoreLab.Core.Data
{
    [System.Serializable]
    public class SLValue
    {
        public SLValue(SLScoreType scoreType)
        {
            var dictionary = DataManager.scoreValues;
            SLValueTemplate valueTemplate = dictionary[scoreType];

            this.scoreType = scoreType;

            name = valueTemplate.name;
            score = valueTemplate.score;
            type = SLValueType.Score;
            stack = valueTemplate.stack;

            duration = maxDuration;

            CreateScore(this);
        }

        public SLValue(SLMultiplierType multiplierType)
        {
            var dictionary = DataManager.multiplierValues;
            SLValueTemplate valueTemplate = dictionary[multiplierType];

            this.multiplierType = multiplierType;

            name = valueTemplate.name;
            score = valueTemplate.score;
            multiplier = valueTemplate.multiplier;
            maxDuration = valueTemplate.maxDuration;
            duration = maxDuration;
            type = SLValueType.Multiplier;
            stack = valueTemplate.stack;

            CreateMultiplier(this);
        }

        public string name;
        public int score;
        public float multiplier;
        public bool stack;

        public SLScoreType scoreType;
        public SLMultiplierType multiplierType;

        public bool cleaned = false;

        public SLValueType type;

        public float maxDuration = 5f;
        public float duration;

        public void CreateScore(SLValue value)
        {
            if (value.type == SLValueType.Score)
            {
                API.OnScorePreAdded?.Invoke(value);
                API.OnScoreAdded?.Invoke(value);
            }
        }

        public void DestroyScore(SLValue value)
        {
            if (value.type == SLValueType.Score)
            {
                API.OnScorePreRemoved?.Invoke(value);
                API.OnScoreRemoved?.Invoke(value);
            }
        }

        public void CreateMultiplier(SLValue value)
        {
            if (value.type == SLValueType.Multiplier)
            {
                API.OnMultiplierPreAdded?.Invoke(value);
                API.OnMultiplierAdded?.Invoke(value);
                API.OnMultiplierChanged?.Invoke(value);
            }
        }

        public void DestroyMultiplier(SLValue value)
        {
            if (value.type == SLValueType.Multiplier)
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
                if (type == SLValueType.Score)
                {
                    DestroyScore(this);
                }
                else if (type == SLValueType.Multiplier)
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