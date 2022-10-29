namespace NEP.ScoreLab.Data
{
    public struct JSONScore
    {
        public string EventType;
        public string TierEventType;

        public float DecayTime;
        public bool Stackable;
        public string EventAudio;

        public string Name;
        public int Score;

        public JSONScore[] Tiers;
    }
}

