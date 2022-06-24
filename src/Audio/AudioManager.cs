using System.Collections.Generic;
using System.IO;

using UnityEngine;

using AudioImportLib;

namespace NEP.Scoreworks.Audio
{
    public class AudioManager
    {
        public AudioManager()
        {
            Awake();
            Start();
        }

        public List<AudioClip> scoreGet;
        public List<AudioClip> multiplierGet;

        private AudioSource source;

        private void Awake()
        {
            scoreGet = new List<AudioClip>();
            multiplierGet = new List<AudioClip>();

            source = new GameObject("Scoreworks Audio Manager").AddComponent<AudioSource>();
        }

        private void Start()
        {
            string basePath = MelonLoader.MelonUtils.UserDataDirectory + "/";
            string soundPath = basePath + "Scoreworks/Audio/";

            foreach(string file in Directory.GetFiles(soundPath))
            {
                if (string.IsNullOrEmpty(file))
                {
                    continue;
                }

                AudioClip clip = API.LoadAudioClip(file, true);

                if (clip.name.StartsWith("sw_score"))
                {
                    scoreGet?.Add(clip);
                }
                else if (clip.name.StartsWith("sw_multiplier"))
                {
                    multiplierGet?.Add(clip);
                }
            }

            Core.ScoreworksManager.instance.OnScoreAdded += (data) => OnScoreAdded();
            Core.ScoreworksManager.instance.OnMultiplierAdded += (data) => OnMultiplierAdded();
        }

        private void OnScoreAdded()
        {
            if(source == null)
            {
                return;
            }

            int rand = Random.Range(0, scoreGet.Count);
            AudioClip random = scoreGet[rand];

            if(random == null)
            {
                return;
            }

            source.clip = random;
            source.Play();
        }

        private void OnMultiplierAdded()
        {
            if (source == null)
            {
                return;
            }

            int rand = Random.Range(0, multiplierGet.Count);
            AudioClip random = multiplierGet[rand];

            if (random == null)
            {
                return;
            }

            source.clip = random;
            source.pitch = 1f + (Core.ScoreworksManager.instance.currentMultiplier / 10f);
            source.Play();
        }
    }
}
