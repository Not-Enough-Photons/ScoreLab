using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEP.ScoreLab.Data.Interfaces
{
    public interface ITimed
    {
        public float Timer { get; set; }
        void UpdateTimer();
    }
}
