namespace NEP.ScoreLab.Data
{
    public class PackedHighScore : PackedValue
    {
        public PackedHighScore(string name, int bestScore) : base(name)
        {
            this.bestScore = bestScore;
        }

        public override PackedType packedType => PackedType.HighScore;
        public int bestScore;

        public override void OnValueCreated()
        {

        }

        public override void OnValueRemoved()
        {
            return;
        }
    }
}