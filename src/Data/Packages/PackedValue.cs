namespace NEP.ScoreLab.Data
{
    [System.Serializable]
    public abstract class PackedValue
    {
        public enum PackedType
        {
            Base,
            Score,
            Multiplier,
            HighScore,
            Misc
        }

        public PackedValue(string name)
        {
            this.name = name;
        }

        public string name;
        public virtual PackedType packedType => PackedType.Base;

        public abstract void OnValueCreated();
        public abstract void OnValueRemoved();

        public virtual void OnUpdate() { }
        public virtual bool IsActive { get; }
    }
}

