using Il2CppInterop.Runtime.InteropTypes.Fields;
using MelonLoader;
using UnityEngine;

namespace NEP.ScoreLab.Core
{
    [RegisterTypeInIl2Cpp]
    public class HighScoreDefinition : MonoBehaviour
    {
        public HighScoreDefinition(System.IntPtr ptr) : base(ptr) { }

        public Il2CppStringField barcode;
        public Il2CppValueField<int> score;
        public Il2CppStringField grade;

        private void Awake()
        {
            string barcode = this.barcode.Get();
            int score = this.score.Get();
            string grade = this.grade.Get();
        }
    }
}