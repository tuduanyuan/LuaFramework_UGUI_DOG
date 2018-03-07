using UnityEngine;
namespace Utility {
    /// <summary>
    /// 本地化持久。放置本地化标签的东西
    /// </summary>
    public static class Data
    {
        public static string PlayerId { get; set; }
        private static string GetPlayerKey(string key)
        {
            return string.Format("{0}_{1}", PlayerId, key);
        }
        public static void Save(string key, string value)
        {
            Save_impl(GetPlayerKey(key), value);
        }
        public static string Load(string key)
        {
            return Load_impl(GetPlayerKey(key));
        }
        public static void Delete(string key)
        {
            Delete_impl(GetPlayerKey(key));
        }
        public static bool Has(string key)
        {
            return Has_impl(GetPlayerKey(key));
        }
        public static bool Check(string key)
        {
            if (!Has(key))
            {
                Save(key, "1");
                return true;
            }
            return false;
        }

        public static void SavePublic(string key, string value)
        {
            Save_impl(key, value);
        }
        public static string LoadPublic(string key)
        {
            return Load_impl(key);
        }
        public static void DeletePublic(string key)
        {
            Delete_impl(key);
        }
        public static bool HasPublic(string key)
        {
            return Has_impl(key);
        }
        public static bool CheckPublic(string key)
        {
            if (!HasPublic(key))
            {
                SavePublic(key, "1");
                return true;
            }
            return false;
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        private static void Save_impl(string key, string value)
        {
            if (value == null)
            {
                logger.error("SaveData value is null");
                return;
            }
            PlayerPrefs.SetString(key, value);
        }
        private static string Load_impl(string key)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : "";
        }
        private static void Delete_impl(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        private static bool Has_impl(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }

}

