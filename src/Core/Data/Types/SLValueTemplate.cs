namespace NEP.ScoreLab.Core.Data
{
    public class SLValueTemplate
    {
        public SLValueTemplate()
        {

        }

        public string id { get; set; }
        public string name { get; set; }
        public int score { get; set; }
        public float multiplier { get; set; }
        public float maxDuration { get; set; }
        public bool useDuration { get; set; }
        public bool useCondition { get; set; }
        public bool stack { get; set; }
    }
}