namespace NEP.Scoreworks.Core.Data
{
    [System.Serializable]
    public struct SWValue
    {
        public string name;
        public int score;
        public float multiplier;

        public SWValueType type;

        public float maxDuration;
        private float _duration;

        private bool _flagForCleanup;

        public void Update()
        {
            _duration += UnityEngine.Time.deltaTime;

            if(_duration >= maxDuration)
            {
                if (!_flagForCleanup)
                {
                    ScoreworksManager.instance.swValues.Remove(this);
                    _flagForCleanup = true;
                }
            }
        }
    }
}
