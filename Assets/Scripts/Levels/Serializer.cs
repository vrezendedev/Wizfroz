using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Serialize
{
    public static class Serializer
    {
        public static bool Serialize<T>(string extraPath, T serializable)
        {
            string path = Application.persistentDataPath + extraPath;

            try
            {
                if (File.Exists(path))
                    File.Delete(path);

                FileStream stream = File.Create(path);
                stream.Close();

                File.WriteAllText(path, JsonConvert.SerializeObject(serializable));
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }

        }

        public static T Deserialize<T>(string extraPath)
        {
            string path = Application.persistentDataPath + extraPath;

            if (!File.Exists(path)) throw new FileNotFoundException($"File with path [{path}] not found");

            try
            {
                T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                throw ex;
            }

        }
    }
}

