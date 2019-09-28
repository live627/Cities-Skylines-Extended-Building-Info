using ColossalFramework.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ExtendedBuildings
{
    public enum LocalizationCategory
    {
        Markov,
        BuildingInfo,
        ServiceInfo,
    }

    class Localization
    {
        private static Dictionary<string, string> texts;
        private static string[] manifestResourceNames;
        private static readonly string locale;
        private static readonly System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

        static Localization()
        {
            locale = LocaleManager.instance.language;
            if (locale == null)
                locale = "en";
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            manifestResourceNames = assembly.GetManifestResourceNames();
            SetText();
        }

        public static string Get(LocalizationCategory cat, string name)
        {
            if (cat == LocalizationCategory.Markov)
                return GetResource(name);
            else
            {
                var key = cat.ToString().ToLower() + "_" + name.ToLower();
                if (!texts.ContainsKey(key))
                    return name;

                return texts[key];
            }
        }

        private static void SetText()
        {
            var res = GetResource("text").Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            texts = new Dictionary<string, string>();
            var header = "";
            foreach (var line in res)
            {
                if (line == null || line.Trim().Length == 0)
                    continue;

                var firstSpace = line.Trim().IndexOf(' ');
                if (firstSpace == -1)
                    header = line.Trim().Replace(":", "").ToLower();
                else
                    texts.Add(header + "_" + line.Substring(0, firstSpace).ToLower(), line.Substring(firstSpace + 1).Trim());
            }
        }

        private static string GetResource(string resourceName)
        {
            var rn = String.Format("ExtendedBuildings.Localization.{0}.{1}.txt", locale.Trim().ToLower(), resourceName);
            if (!manifestResourceNames.Contains(rn))
            {
                Debug.Log(string.Format("Embedded resource {0} not found. Reverting to English.", rn));
                rn = String.Format("ExtendedBuildings.Localization.{0}.{1}.txt", "en", resourceName);
            }

            using (Stream stream = assembly.GetManifestResourceStream(rn))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}