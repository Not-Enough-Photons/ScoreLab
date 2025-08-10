using Il2CppInterop.Runtime.InteropTypes.Fields;

namespace NEP.ScoreLab.Data
{
    public class GradeDefinition
    {
        public string Grade => _grade;
        public int Threshold => _threshold;
        
        private string _grade;
        private int _threshold;
    }
}