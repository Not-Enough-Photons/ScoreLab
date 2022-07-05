﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEP.Scoreworks.Core.Data
{
    public static class DataManager
    {
        public static Dictionary<SWScoreType, SWValueTemplate> scoreValues { get; private set; }
        public static Dictionary<SWMultiplierType, SWValueTemplate> multiplierValues { get; private set; }

        public static Dictionary<string, SWHighScore> highScoreTable { get; private set; }

        public static void Initialize()
        {
            highScoreTable = new Dictionary<string, SWHighScore>();

            BuildScoreValues();
            BuildMultiplierValues();
            LoadHighScoreTable(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json");
        }

        // High Scores
        public static void SaveHighScore(SWHighScore score)
        {
            if (highScoreTable.ContainsKey(score.currentScene))
            {
                highScoreTable[score.currentScene].currentScene = ScoreworksManager.instance.currentScene;
                highScoreTable[score.currentScene].highScore = ScoreworksManager.instance.currentHighScore;
            }
            else
            {
                highScoreTable.Add(score.currentScene, score);
            }

            string serialized = JsonConvert.SerializeObject(highScoreTable, Formatting.Indented);

            System.IO.File.WriteAllText(MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_highscores.json", serialized);
        }

        public static SWHighScore RetrieveHighScore(string sceneName)
        {
            return highScoreTable[sceneName];
        }

        public static void LoadHighScoreTable(string path)
        {
            string data = System.IO.File.ReadAllText(path);
            highScoreTable = JsonConvert.DeserializeObject<Dictionary<string, SWHighScore>>(data);
        }

        public static void BuildScoreValues()
        {
            scoreValues = new Dictionary<SWScoreType, SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_score_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SWScoreType, SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SWScoreType));

            foreach (string name in enumNames)
            {
                SWScoreType scoreType = (SWScoreType)Enum.Parse(typeof(SWScoreType), name);
                SWValueTemplate template = dictionary[scoreType];
                scoreValues?.Add(scoreType, template);
            }
        }

        public static void BuildMultiplierValues()
        {
            multiplierValues = new Dictionary<SWMultiplierType, SWValueTemplate>();

            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/sw_mult_data.json";
            string file = System.IO.File.ReadAllText(path);

            var dictionary = JsonConvert.DeserializeObject<Dictionary<SWMultiplierType, SWValueTemplate>>(file);

            string[] enumNames = Enum.GetNames(typeof(SWMultiplierType));

            foreach (string name in enumNames)
            {
                Data.SWMultiplierType multType = (SWMultiplierType)Enum.Parse(typeof(SWMultiplierType), name);
                Data.SWValueTemplate template = dictionary[multType];
                multiplierValues?.Add(multType, template);
            }
        }

        // UI Data
        public static UI.UIPadding ReadPadding()
        {
            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json";
            string file = System.IO.File.ReadAllText(path);

            var hudData = JObject.Parse(file);

            var filePaddingSettings = hudData["padding"];

            if (filePaddingSettings["leftPadding"] == null)
            {
                return null;
            }

            float[] paddingLeft = new float[3]
            {
                (float)filePaddingSettings["leftPadding"][0],
                (float)filePaddingSettings["leftPadding"][1],
                (float)filePaddingSettings["leftPadding"][2],
            };

            if (filePaddingSettings["rightPadding"] == null)
            {
                return null;
            }

            float[] paddingRight = new float[3]
            {
                (float)filePaddingSettings["rightPadding"][0],
                (float)filePaddingSettings["rightPadding"][1],
                (float)filePaddingSettings["rightPadding"][2],
            };

            if (filePaddingSettings["topPadding"] == null)
            {
                return null;
            }

            float[] paddingTop = new float[3]
            {
                (float)filePaddingSettings["topPadding"][0],
                (float)filePaddingSettings["topPadding"][1],
                (float)filePaddingSettings["topPadding"][2],
            };

            if (filePaddingSettings["bottomPadding"] == null)
            {
                return null;
            }

            float[] paddingBottom = new float[3]
            {
                (float)filePaddingSettings["bottomPadding"][0],
                (float)filePaddingSettings["bottomPadding"][1],
                (float)filePaddingSettings["bottomPadding"][2],
            };

            var padding = new UI.UIPadding()
            {
                leftPadding = paddingLeft,
                rightPadding = paddingRight,
                topPadding = paddingTop,
                bottomPadding = paddingBottom,
            };

            return padding;
        }

        public static UI.UISettings ReadHUDSettings()
        {
            string path = MelonLoader.MelonUtils.UserDataDirectory + "/Scoreworks/hud_settings.json";
            string file = System.IO.File.ReadAllText(path);

            var hudData = JObject.Parse(file);

            var fileScaleSettings = hudData["settings"];

            UI.UISettings hudSettings = new UI.UISettings()
            {
                followDistance = (float)fileScaleSettings["followDistance"],
                followLerp = (float)fileScaleSettings["followLerp"],
            };

            return hudSettings;
        }
    }

}