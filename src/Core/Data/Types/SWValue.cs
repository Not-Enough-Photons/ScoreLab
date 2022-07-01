namespace NEP.Scoreworks.Core.Data
{
    [System.Serializable]
    public class SWValue
    {
        public SWValue(SWScoreType scoreType)
        {
            var dictionary = DataManager.scoreValues;
            SWValueTemplate valueTemplate = dictionary[scoreType];

            this.name = valueTemplate.name;
            this.score = valueTemplate.score;
            this.type = SWValueType.Score;
            this.stack = valueTemplate.stack;

            this.maxDuration = 5f;

            AddToList(this);
        }

        public SWValue(SWMultiplierType multiplierType)
        {
            var dictionary = DataManager.multiplierValues;
            SWValueTemplate valueTemplate = dictionary[multiplierType];

            this.name = valueTemplate.name;
            this.score = valueTemplate.score;
            this.multiplier = valueTemplate.multiplier;
            this.maxDuration = valueTemplate.maxDuration;
            this.type = SWValueType.Multiplier;
            this.stack = valueTemplate.stack;

            AddToList(this);
        }

        public SWValue(string name, int score)
        {
            this.name = name;
            this.score = score;
            this.type = SWValueType.Score;
            this.maxDuration = 5f;

            AddToList(this);
        }

        public SWValue(string name, float multiplier, float duration)
        {
            this.name = name;
            this.multiplier = multiplier;
            this.type = SWValueType.Multiplier;
            this.maxDuration = duration;

            AddToList(this);
        }

        public string name;
        public int score;
        public float multiplier;
        public bool stack = false;

        public bool cleaned = false;

        public SWValueType type;

        public float maxDuration;
        private float _duration;

        public static void AddToList(SWValue value)
        {
            ScoreworksManager.swValues.Add(value);

            ScoreworksManager.instance.AddValues(value);

            if (value.type == SWValueType.Score)
            {
                API.OnScoreAdded?.Invoke(value);
            }

            if (value.type == SWValueType.Multiplier)
            {
                API.OnMultiplierAdded?.Invoke(value);
            }
        } 

        public static void RemoveFromList(SWValue value)
        {
            ScoreworksManager.swValues.Remove(value);

            if (value.type == SWValueType.Multiplier)
            {
                API.OnMultiplierRemoved?.Invoke(value);
            }

            if (value.type == SWValueType.Score)
            {
                API.OnScoreRemoved?.Invoke(value);
            }
        }

        public void Update()
        {
            _duration += UnityEngine.Time.deltaTime;

            if(_duration >= maxDuration)
            {
                RemoveFromList(this);
            }
        }
    }
}
