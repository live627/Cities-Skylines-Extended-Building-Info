using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Math;
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
        private static Dictionary<string, Markov> buildingNames = new Dictionary<string, Markov>();
        private static Dictionary<string, Markov> buildingDescriptions = new Dictionary<string, Markov>();
        private static string[] manifestResourceNames;
        private static string locale;
        private static System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

        static Localization()
        {
            OnLocaleChanged();
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
        }

        private static void OnLocaleChanged()
        {
            locale = LocaleManager.instance.language;
            if (locale == null)
                locale = "en";
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            manifestResourceNames = assembly.GetManifestResourceNames();
            SetText();
            LoadTextFiles();
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
            var rn = $"ExtendedBuildings.Localization.{locale.Trim().ToLower()}.{resourceName}.txt";
            if (!manifestResourceNames.Contains(rn))
            {
                Debug.Log($"Embedded resource {rn} not found. Reverting to English.");
                rn = $"ExtendedBuildings.Localization.{"en"}.{resourceName}.txt";
            }

            using (Stream stream = assembly.GetManifestResourceStream(rn))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static void LoadTextFiles()
        {
            buildingNames.Clear();
            var commercialName = new Markov("nameCommercial", false, 4);
            buildingNames.Add(ItemClass.Zone.CommercialHigh.ToString(), commercialName);
            buildingNames.Add(ItemClass.Zone.CommercialLow.ToString(), commercialName);
            var resName = new Markov("nameResidential", false, 4);
            buildingNames.Add(ItemClass.Zone.ResidentialHigh.ToString(), resName);
            buildingNames.Add(ItemClass.Zone.ResidentialLow.ToString(), resName);
            var indyName = new Markov("nameIndustrial", false, 4);
            buildingNames.Add(ItemClass.Zone.Industrial.ToString(), indyName);
            var officeName = new Markov("nameOffice", false, 4);
            buildingNames.Add(ItemClass.Zone.Office.ToString(), officeName);

            buildingNames.Add(ItemClass.SubService.IndustrialFarming.ToString(), new Markov("nameFarm", false, 4));
            buildingNames.Add(ItemClass.SubService.IndustrialForestry.ToString(), new Markov("nameForest", false, 4));
            buildingNames.Add(ItemClass.SubService.IndustrialOre.ToString(), new Markov("nameMine", false, 4));
            buildingNames.Add(ItemClass.SubService.IndustrialOil.ToString(), new Markov("nameOil", false, 4));

            buildingDescriptions.Clear();
            var commercialDescription = new Markov("descriptionsCommercial", false, 9);
            buildingDescriptions.Add(ItemClass.Zone.CommercialHigh.ToString(), commercialDescription);
            buildingDescriptions.Add(ItemClass.Zone.CommercialLow.ToString(), commercialDescription);
            var resDescription = new Markov("descriptionsResidential", false, 9);
            buildingDescriptions.Add(ItemClass.Zone.ResidentialHigh.ToString(), resDescription);
            buildingDescriptions.Add(ItemClass.Zone.ResidentialLow.ToString(), resDescription);
            var indyDescription = new Markov("descriptionsIndustrial", false, 9);
            buildingDescriptions.Add(ItemClass.Zone.Industrial.ToString(), indyDescription);
            var officeDescription = new Markov("descriptionsOffice", false, 9);
            buildingDescriptions.Add(ItemClass.Zone.Office.ToString(), officeDescription);


            buildingDescriptions.Add(ItemClass.SubService.IndustrialFarming.ToString(), new Markov("descriptionsFarm", false, 4));
            buildingDescriptions.Add(ItemClass.SubService.IndustrialForestry.ToString(), new Markov("descriptionsForest", false, 4));
            buildingDescriptions.Add(ItemClass.SubService.IndustrialOre.ToString(), new Markov("descriptionsMine", false, 4));
            buildingDescriptions.Add(ItemClass.SubService.IndustrialOil.ToString(), new Markov("descriptionsOil", false, 4));
        }

        public static string GetDescription(string bName, ushort buildingId, ItemClass.Zone zone, ItemClass.SubService ss)
        {
            Randomizer randomizer = new Randomizer(Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier.GetHashCode() - buildingId);
            var year = 2015 - buildingId % 200;
            if (!buildingDescriptions.TryGetValue(ss.ToString(), out Markov markov))
                buildingDescriptions.TryGetValue(zone.ToString(), out markov);

            if (markov != null)
            {
                var text = markov.GetText(ref randomizer, 100, 200, true);
                var cityName = Singleton<SimulationManager>.instance.m_metaData.m_CityName.Trim();
                text = text.Replace("COMPANY", bName).Replace("DATE", year.ToString()).Replace("SITY", cityName);
                return text;
            }
            return "";
        }

        public static string GetName(ushort buildingId, ItemClass.Zone zone, ItemClass.SubService ss)
        {
            Randomizer randomizer = new Randomizer(Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier.GetHashCode() - buildingId);
            if (buildingId % 6 != 0)
            {
                if (!buildingNames.TryGetValue(ss.ToString(), out Markov markov))
                    buildingNames.TryGetValue(zone.ToString(), out markov);

                if (markov != null)
                    return markov.GetText(ref randomizer, 6, 16, true, true);
            }
            return null;
        }
    }
}