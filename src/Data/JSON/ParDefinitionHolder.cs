using Il2CppInterop.Runtime.InteropTypes.Fields;
using MelonLoader;
using NEP.ScoreLab.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

using NEP.ScoreLab.Data;

namespace NEP.ScoreLab.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ParDefinitionHolder(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Il2CppStringField Data;

        private void Awake()
        {
            string json = Data.Get();
            string barcode = string.Empty;

            JSONPar customPar = new JSONPar();

            // TODO:
            // Write a function to do all this JSON reading
            using (StringReader reader = new StringReader(json))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    JObject obj = JToken.ReadFrom(jsonReader) as JObject;

                    if (obj == null)
                    {
                        return;
                    }

                    barcode = obj["barcode"].Value<string>();
                        
                    JArray grades = obj["grades"] as JArray;

                    if (grades != null)
                    {
                        List<JSONPar.JSONGrade> gradeList = new List<JSONPar.JSONGrade>();
                            
                        foreach (var gradeObject in grades)
                        {
                            JSONPar.JSONGrade grade = new JSONPar.JSONGrade();
                            grade.grade = gradeObject["grade"].Value<string>();
                            grade.threshold = gradeObject["threshold"].Value<int>();
                            gradeList.Add(grade);
                        }

                        // Sort the grade list by least to most
                        // ...in case if map makers forget to do that
                        gradeList.Sort((first, second) =>
                        {
                            if (first.threshold == second.threshold)
                                return 0;
                            if (first.threshold >= second.threshold)
                                return 1;
                            if (first.threshold <= second.threshold)
                                return -1;

                            return 0;
                        });
                            
                        customPar.grades = gradeList.ToArray();
                    }
                }
            }
            
            ValueManager.ParTable.Add(barcode, customPar);
            ScoreTracker.SetLevelPar(customPar);
        }
    }
}