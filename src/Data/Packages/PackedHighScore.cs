namespace NEP.ScoreLab.Data
{
    public class PackedHighScore : PackedValue
    {
        public PackedHighScore(string name, int bestScore)
        {
            this.bestScore = bestScore;
        }

        public override PackedType PackedValueType => PackedType.HighScore;
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