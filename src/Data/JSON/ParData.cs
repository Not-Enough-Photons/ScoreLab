using UnityEngine;
using NEP.ScoreLab.SDK;

namespace NEP.ScoreLab.Data
{
    [System.Serializable]
    public class ParData
    {
        public string Barcode => _barcode;
        public GradeDefinition[] Grades => _grades;
        public int Score => _score;
        public bool IsBaseGame => _isBaseGame;
        
        private string _barcode;
        private GradeDefinition[] _grades;
        private int _score;
        private bool _isBaseGame;
    }
}