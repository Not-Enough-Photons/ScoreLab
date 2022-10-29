namespace NEP.ScoreLab.Data
{
    public struct JSONMult
    {
        public string EventType;
        public string TierEventType;

        public float DecayTime;
        public bool Stackable;

        public string Name;
        public float Multiplier;
        public string Condition;

        public JSONMult[] Tiers;
    }
}