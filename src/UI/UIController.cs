using UnityEngine;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class UIController : MonoBehaviour
    {
        public UIController(System.IntPtr ptr) : base(ptr) { }

        public UIModule ScoreModule { get; set; }
        public UIModule MultiplierModule { get; set; }
        public UIModule HighScoreModule { get; set; }

        public float Distance = 1f;
        public float Lerp = 24f;

        public Transform followTarget;

        private void Awake()
        {
            if(transform.Find("Main_Score") != null)
            {
                ScoreModule = transform.Find("Main_Score").GetComponent<UIScoreModule>();
            }

            if (transform.Find("Main_Multiplier"))
            {
                MultiplierModule = transform.Find("Main_Multiplier").GetComponent<UIMultiplierModule>();
            }
        }

        private void OnEnable()
        {
            UpdateModule(null, ScoreModule);

            API.Value.OnValueAdded += (data) => UpdateModule(data, ScoreModule);
            API.Value.OnValueTierReached += (data) => UpdateModule(data, ScoreModule);
            API.Value.OnValueAccumulated += (data) => UpdateModule(data, ScoreModule);
        }

        private void OnDisable()
        {
            API.Value.OnValueAdded -= (data) => UpdateModule(data, ScoreModule);
            API.Value.OnValueTierReached -= (data) => UpdateModule(data, ScoreModule);
            API.Value.OnValueAccumulated -= (data) => UpdateModule(data, ScoreModule);
        }

        private void Start()
        {
            if(BoneLib.Player.GetPhysicsRig() != null)
            {
                followTarget = BoneLib.Player.GetPhysicsRig().m_chest;
            }
        }
        
        // For being attached to a physical point on the body
        private void FixedUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector3 move = Vector3.Lerp(transform.position, followTarget.position + followTarget.forward * Distance, Lerp * Time.deltaTime);
            Quaternion lookRot = Quaternion.LookRotation(followTarget.forward);

            transform.position = move;
            transform.rotation = lookRot;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    //transform.GetChild(i).LookAt(followTarget);
                }
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void SetScoreModule(UIModule module)
        {
            this.ScoreModule = module;
        }

        public void SetMultiplierModule(UIModule module)
        {
            this.MultiplierModule = module;
        }

        public void UpdateModule(PackedValue data, UIModule module)
        {
            try
            {
                module.AssignPackedData(data);
                module.OnModuleEnable();
            }
            catch
            {

            }
        }
    }
}
