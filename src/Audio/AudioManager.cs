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
                string baseSoundName = file.Substring(2);

                if (baseSoundName.StartsWith("score"))
                {
                    AudioClip clip = API.LoadAudioClip(baseSoundName, true);
                    scoreGet?.Add(clip);
                }
                else if (baseSoundName.StartsWith("multiplier"))
                {
                    AudioClip clip = API.LoadAudioClip(baseSoundName, true);
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
