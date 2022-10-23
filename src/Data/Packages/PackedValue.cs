namespace NEP.ScoreLab.Data
{
    [System.Serializable]
    public class PackedValue
    {
        public enum PackedType
        {
            Base,
            Score,
            Multiplier,
            HighScore,
            Misc
        }

        public PackedValue()
        {

        }

        public string Name;

        public virtual PackedType PackedValueType => PackedType.Base;

        public string eventType;

        public virtual void OnValueCreated() { }
        public virtual void OnValueRemoved() { }

        public virtual void OnUpdate() { }
        public virtual bool IsActive { get; }
    }
}

