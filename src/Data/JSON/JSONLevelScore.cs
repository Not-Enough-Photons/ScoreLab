using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.ScoreLab.Data
{
    public class JSONPar
    {
        public class JSONGrade
        {
            public string grade;
            public int threshold;
        }

        public JSONGrade[] grades;
        public int score;
        public bool isBaseGame;
    }
}