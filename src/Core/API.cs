using System;

using NEP.Scoreworks.Core.Data;

namespace NEP.Scoreworks.Core
{
    public static class API
    {
        public static Action<SWValue> OnScorePreAdded;
        public static Action<SWValue> OnScoreAdded;

        public static Action<SWValue> OnScorePreRemoved;
        public static Action<SWValue> OnScoreRemoved;
        public static Action<SWValue> OnScoreLateRemoved;

        public static Action<SWValue> OnMultiplierPreAdded;
        public static Action<SWValue> OnMultiplierAdded;

        public static Action<SWValue> OnMultiplierPreRemoved;
        public static Action<SWValue> OnMultiplierRemoved;
        public static Action<SWValue> OnMultiplierLateRemoved;

        public static Action<SWValue> OnScoreChanged;
        public static Action<SWValue> OnMultiplierChanged;

        public static Action<SWValue> OnHighScoreReached;

        public static Action<SWValue> OnScoreDuplicated;
        public static Action<SWValue> OnMultiplierDuplicated;
    }
}