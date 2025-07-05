using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppUltEvents;
using MelonLoader;
using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;
using UnityEngine;

namespace NEP.ScoreLab.HUD
{
    [RegisterTypeInIl2Cpp]
    public class HUDEvents : MonoBehaviour
    {
        public HUDEvents(System.IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<UltEventHolder> OnScoreUpdateEvent;
        public Il2CppReferenceField<UltEventHolder> OnMultiplierUpdateEvent;
        public Il2CppReferenceField<UltEventHolder> OnHighScoreReached;

        private void Awake()
        {
            API.Value.OnValueAdded += OnScoreUpdateEvent_Callback;
            API.Value.OnValueAdded += OnMultiplierUpdateEvent_Callback;
            API.HighScore.OnHighScoreReached += OnHighScoreReached_Callback;
        }

        private void OnDestroy()
        {
            API.Value.OnValueAdded -= OnScoreUpdateEvent_Callback;
            API.Value.OnValueAdded -= OnMultiplierUpdateEvent_Callback;
            API.HighScore.OnHighScoreReached -= OnHighScoreReached_Callback;
        }

        [HideFromIl2Cpp]
        private void OnScoreUpdateEvent_Callback(PackedValue value)
        {
            if (value is not PackedScore)
            {
                return;
            }
            
            OnScoreUpdateEvent.Get()?.Invoke();
        }

        [HideFromIl2Cpp]
        private void OnMultiplierUpdateEvent_Callback(PackedValue value)
        {
            if (value is not PackedMultiplier)
            {
                return;
            }
            
            OnMultiplierUpdateEvent.Get()?.Invoke();
        }

        [HideFromIl2Cpp]
        private void OnHighScoreReached_Callback(PackedValue value)
        {
            if (value is not PackedHighScore)
            {
                return;
            }
            
            OnMultiplierUpdateEvent.Get()?.Invoke();
        }
    }
}