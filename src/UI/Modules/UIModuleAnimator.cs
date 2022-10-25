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

        private UIModule _module;

        private void Awake()
        {
            _module = GetComponent<UIModule>();
            Animator = GetComponent<Animator>();

            _module.OnModuleEnabled += _module.ModuleType == UIModule.UIModuleType.Main ? () => PlayAnimation("main_show") : () => PlayAnimation("descriptor_show");
            _module.OnModuleDecayed += _module.ModuleType == UIModule.UIModuleType.Main ? () => PlayAnimation("main_hide") : () => PlayAnimation("descriptor_hide");
        }

        private void PlayAnimation(string name)
        {
            if (Animator == null)
            {
                return;
            }

            Animator.Play(name);
        }
    }
}
