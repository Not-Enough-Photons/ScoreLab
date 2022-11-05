using UnityEngine;
using UnityEngine.Animations;

using NEP.ScoreLab.Core;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIModuleAnimator : MonoBehaviour
    {
        public UIModuleAnimator(System.IntPtr ptr) : base(ptr) { }

        public Animator Animator;
        public Animation Animation;

        private UIModule _module;

        private void Awake()
        {
            _module = GetComponent<UIModule>();
            Animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            //API.UI.OnModuleEnabled += OnModuleEnabled;
            API.Value.OnValueTierReached += (data) => OnTierReached();
            API.UI.OnModuleDecayed += OnModuleDecayed;
        }

        private void OnDisable()
        {
            //API.UI.OnModuleEnabled -= OnModuleEnabled;
            API.Value.OnValueTierReached -= (data) => OnTierReached();
            API.UI.OnModuleDecayed -= OnModuleDecayed;
        }

        private void PlayAnimation(string name)
        {
            if (Animator == null)
            {
                return;
            }

            Animator.Play(name);
        }

        private void OnModuleEnabled(UIModule module)
        {
            if (_module != module)
            {
                return;
            }

            PlayAnimation("show");
        }

        private void OnTierReached()
        {
            PlayAnimation("tier_reached");
        }

        private void OnModuleDecayed(UIModule module)
        {
            if (_module != module)
            {
                return;
            }

            PlayAnimation("hide");
        }
    }
}
