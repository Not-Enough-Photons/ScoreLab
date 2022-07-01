namespace NEP.Scoreworks.Core.Data
{
    public class SWValueTemplate
    {
        public SWValueTemplate()
        {

        }

        public string id { get; set; }
        public string name { get; set; }
        public int score { get; set; }
        public float multiplier { get; set; }
        public float maxDuration { get; set; }
        public bool useDuration { get; set; }
        public bool stack { get; set; }
    }
}