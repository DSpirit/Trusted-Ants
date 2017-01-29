using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.SettingClasses;
using SimpleJson;
using UnityEngine;

namespace Assets.Scripts
{
    public class Serializer : MonoBehaviour
    {
        

        public static void SaveToJson<T>(T data, string custom = "settings.json")
        {
            string xmlFileName = custom;
            var ser = JsonUtility.ToJson(data);
            File.WriteAllText(xmlFileName, ser);
        }

        public static Setting LoadSettings(string custom = "settings.json")
        {
            string xmlFileName = custom;
            Setting s = null;
            try
            {
				string f = File.ReadAllText(xmlFileName);
                s = (Setting) JsonUtility.FromJson(f, typeof (Setting));
            }
            catch
            {
                Debug.Log("No Settings found, creating default settings...");
                SaveToJson(new Setting());
                LoadSettings();
            }
            
            return s;
        }

        public static T Load<T>(string filename) where T : class
        {
            
            if (File.Exists(filename))
            {
                try
                {
                    using (Stream stream = File.OpenRead(filename))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        return formatter.Deserialize(stream) as T;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            return default(T);
        }

        public static void Save<T>(string filename, T data) where T : class
        {
            
            using (Stream stream = File.OpenWrite(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
        }
    }
}
