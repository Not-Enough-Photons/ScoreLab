namespace NEP.ScoreLab.Data
{
    public class PackedHighScore : PackedValue
    {
        public PackedHighScore(string name, int score)
        {
            Name = name;
            _score = score;
        }

        public override PackedType PackedValueType => PackedType.HighScore;

        public int Score => _score <= 0 ? 0 : _score;
        public string Barcode => _barcode;
        
        private int _score;
        private string _barcode;
    }
}