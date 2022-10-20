using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using NEP.ScoreLab.Core;
using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.UI
{
    public class UIModule : MonoBehaviour
    {
        public enum UIModuleType
        {
            Main,
            Descriptor
        }

        public Action OnModuleEnabled;
        public Action OnModuleDisabled;
        public Action OnModuleDecayed;
        public Action OnModulePostDecayed;

        public UIModuleType ModuleType;

        public PackedValue PackedValue { get => _packedValue; }

        public virtual bool CanDecay { get => transform.Find("-Persist") == null; }
        public float DecayTime { get => _decayTime; }
        public float PostDecayTime { get => _postDecayTime; }

        protected TextMeshProUGUI _title { get; private set; }
        protected TextMeshProUGUI _value { get; private set; }
        protected Slider _timeBar { get; private set; }

        protected PackedValue _packedValue { get; private set; }

        protected bool _canDecay { get; private set; }
        protected float _decayTime { get; private set; }
        protected float _postDecayTime { get; private set; }

        protected float _tDecay { get; private set; }
        protected float _tPostDecay { get; private set; }

        private string Path_Root => name;
        protected virtual string Path_TitleText { get => "Title"; }
        protected virtual string Path_ValueText { get => "Value"; }
        protected virtual string Path_TimeBar { get => "TimeBar"; }

        private bool _reachedDecay = false;
        private bool _reachedPostDecay = false;

        public virtual void OnModuleEnable() { OnModuleEnabled?.Invoke(); }

        public virtual void OnModuleDisable() { OnModuleDisabled?.Invoke(); }

        public virtual void OnUpdate() { }

        public void AssignPackedData(PackedValue packedValue) => _packedValue = packedValue;

        public void SetDecayTime(float decayTime)
        {
            this._decayTime = decayTime;
            this._tDecay = this._decayTime;
        }

        public void SetPostDecayTime(float postDecayTime)
        {
            this._postDecayTime = postDecayTime;
            this._tPostDecay = this._postDecayTime;
        }

        protected void SetText(TextMeshProUGUI text, string value)
        {
            if(text == null)
            {
                return;
            }

            text.text = value;
        }

        protected void SetBarValue(Slider timeBar, float value)
        {
            timeBar.value = value;
        }

        protected void SetMaxValueToBar(Slider timeBar, float value)
        {
            if(timeBar == null)
            {
                return;
            }

            timeBar.maxValue = value;
        }

        protected void UpdateDecay()
        {
            if (!CanDecay)
            {
                return;
            }

            if(_tDecay < 0f)
            {
                if (!_reachedDecay)
                {
                    OnModuleDecayed?.Invoke();
                    _reachedDecay = true;
                }

                _tPostDecay -= Time.deltaTime;

                if (_tPostDecay < 0f)
                {
                    if (!_reachedPostDecay)
                    {
                        OnModulePostDecayed?.Invoke();
                        _reachedPostDecay = true;
                    }

                    _tDecay = _decayTime;
                    _tPostDecay = _postDecayTime;

                    _reachedDecay = false;
                    _reachedPostDecay = false;

                    gameObject.SetActive(false);

                    return;
                }

                return;
            }

            _tDecay -= Time.deltaTime;
        }

        private void Awake()
        {
            Transform titleTran = transform.Find(Path_TitleText);
            Transform valueTran = transform.Find(Path_ValueText);
            Transform timeBarTran = transform.Find(Path_TimeBar);

            _title = titleTran?.GetComponent<TextMeshProUGUI>();
            _value = valueTran?.GetComponent<TextMeshProUGUI>();
            _timeBar = timeBarTran?.GetComponent<Slider>();
        }

        private void OnEnable() => OnModuleEnable();
        private void OnDisable() => OnModuleEnable();
        private void Update() => OnUpdate();
    }
}