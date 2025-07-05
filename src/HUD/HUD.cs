using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.HUD
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HUD : MonoBehaviour
    {
        public HUD(System.IntPtr ptr) : base(ptr) { }

        [HideFromIl2Cpp]
        public Module ScoreModule { get; set; }
        
        [HideFromIl2Cpp]
        public Module MultiplierModule { get; set; }
        
        [HideFromIl2Cpp]
        public Module HighScoreModule { get; set; }

        public Transform followTarget;

        private void Awake()
        {
            if(transform.Find("MainScore") != null)
            {
                ScoreModule = transform.Find("MainScore").GetComponent<ScoreModule>();
            }

            if (transform.Find("MainMultiplier"))
            {
                MultiplierModule = transform.Find("MainMultiplier").GetComponent<MultiplierModule>();
            }

            if (transform.Find("MainHighScore"))
            {
                HighScoreModule = transform.Find("MainHighScore").GetComponent<HighScoreModule>();
            }
        }

        private void OnEnable()
        {
            UpdateModule(null);
            UpdateModule(null);

            API.Value.OnValueAdded += UpdateModule;
            API.Value.OnValueTierReached += UpdateModule;
            API.Value.OnValueAccumulated += UpdateModule;
            API.Value.OnValueRemoved += UpdateModule;
        }

        private void OnDisable()
        {
            API.Value.OnValueAdded -= UpdateModule;
            API.Value.OnValueTierReached -= UpdateModule;
            API.Value.OnValueAccumulated -= UpdateModule;
            API.Value.OnValueRemoved -= UpdateModule;
        }

        private void Start()
        {
            if(BoneLib.Player.GetPhysicsRig() != null)
            {
                followTarget = BoneLib.Player.ControllerRig.m_head;
            }
        }
        
        // For being attached to a physical point on the body
        private void FixedUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * Settings.DistanceToCamera, Settings.MovementSmoothness * Time.unscaledDeltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        [HideFromIl2Cpp]
        public void SetScoreModule(Module module)
        {
            this.ScoreModule = module;
        }

        [HideFromIl2Cpp]
        public void SetMultiplierModule(Module module)
        {
            this.MultiplierModule = module;
        }

        [HideFromIl2Cpp]
        public void UpdateModule(PackedValue data)
        {
            if (data is PackedScore)
            {
                ScoreModule.AssignPackedData(data);
                ScoreModule.OnModuleEnable();
            }
            else if (data is PackedMultiplier)
            {
                MultiplierModule.AssignPackedData(data);
                MultiplierModule.OnModuleEnable();
            }
            else if (data is PackedHighScore)
            {
                HighScoreModule.AssignPackedData(data);
                HighScoreModule.OnModuleEnable();
            }
        }
    }
}
