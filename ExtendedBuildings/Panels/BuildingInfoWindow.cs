namespace ExtendedBuildings
{
    using ColossalFramework;
    using ColossalFramework.Math;
    using ColossalFramework.UI;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class BuildingInfoWindow5 : UIPanel
    {
        const float vertPadding = 26;
        float barWidth;
        Dictionary<ImmaterialResourceManager.Resource, UIProgressBar> resourceBars;
        Dictionary<ImmaterialResourceManager.Resource, UILabel> resourceLabels;
        UITextField buildingName;

        public ZonedBuildingWorldInfoPanel baseBuildingWindow;
        FieldInfo baseSub;

        Dictionary<string, Markov> buildingNames = new Dictionary<string, Markov>();
        Dictionary<string, Markov> buildingDescriptions = new Dictionary<string, Markov>();

        UILabel descriptionLabel;

        ushort selectedBuilding;
        private List<UIProgressBar> aresourceBars;
        private List<UILabel> aresourceLabels;
        private string[] tooltips = new string[31];
        public static bool ShowDescription { get; set; } = true;
        public static bool ShowName { get; set; } = true;

        public override void Awake()
        {
            resourceBars = new Dictionary<ImmaterialResourceManager.Resource, UIProgressBar>(20);
            resourceLabels = new Dictionary<ImmaterialResourceManager.Resource, UILabel>(20);
            var resNaames = Enum.GetNames(typeof(ImmaterialResourceManager.Resource));
            for (var i = 0; i < 20; i += 1)
            {
                var res = (ImmaterialResourceManager.Resource)i;
                if (res != ImmaterialResourceManager.Resource.Abandonment && res != ImmaterialResourceManager.Resource.NoisePollution)
                {
                    var bar = AddUIComponent<UIProgressBar>();
                    bar.backgroundSprite = "LevelBarBackground";
                    bar.progressSprite = "LevelBarForeground";
                    resourceBars.Add(res, bar);
                    var label = AddUIComponent<UILabel>();
                    tooltips[i] = Localization.Get(LocalizationCategory.BuildingInfo, resNames[i]);
                    label.textScale = 0.5f;
                    label.size = new Vector2(100, 20);
                    resourceLabels.Add(res, label);
                }
            }
            aresourceBars = new List<UIProgressBar>();
            aresourceLabels = new List<UILabel>();
            var ares = new string[]
            {
                "Service", 
                "Education",
                "Happiness",
                resNaames[8], // Noise
                resNaames[18], // Abandonment
                "Pollution"
            };
            var adesc = new string[]
            {
                "ServiceDescription", 
                "EducationDescription",
                "HappinessDescription",
                resNaames[8], // Noise
                resNaames[18], // Abandonment
                "Pollution"
            };
            for (var i = 0; i < 6; i += 1)
            {
                var bar = AddUIComponent<UIProgressBar>();
                bar.backgroundSprite = "LevelBarBackground";
                bar.progressSprite = "LevelBarForeground";
                bar.progressColor = i < 4 ? Color.green : Color.red;
                bar.minValue = 0f;
                bar.maxValue = 1f;
                bar.tooltip = Localization.Get(LocalizationCategory.BuildingInfo, adesc[i]) + " 0/0";
                aresourceBars.Add(bar);
                var label = AddUIComponent<UILabel>();
                label.tooltip = Localization.Get(LocalizationCategory.BuildingInfo, adesc[i]);
                label.text = Localization.Get(LocalizationCategory.BuildingInfo, ares[i]);
                label.textScale = 0.75f;
                label.size = new Vector2(100, 20);
                aresourceLabels.Add(label);
            }
            
            buildingNames.Clear();
            LoadTextFiles();
            
            descriptionLabel = AddUIComponent<UILabel>();

            base.Awake();
        }

        private void LoadTextFiles()
        {
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

        public override void Start()
        {
            base.Start();

            backgroundSprite = "MenuPanel2";
            opacity = ModConfig.Instance.Opacity;
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            barWidth = size.x - 28;
            float y = ModConfig.Instance.Display == ModConfig.DisplayCategory.Compact ? 70 : 414;
            
            descriptionLabel.textScale = 0.6f;
            descriptionLabel.wordWrap = true;
            descriptionLabel.autoSize = false;
            descriptionLabel.autoHeight = true;
            descriptionLabel.width = barWidth;
            descriptionLabel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            
            y += vertPadding;
            height = y;
        }
        
        public override void Update()
        {
            var instanceId = GetParentInstanceId();
            if (instanceId.Type == InstanceType.Building && instanceId.Building != 0)
            {
                ushort building = instanceId.Building;
                if (baseBuildingWindow != null && enabled && isVisible && Singleton<BuildingManager>.exists && ((Singleton<SimulationManager>.instance.m_currentFrameIndex & 15u) == 15u || selectedBuilding != building))
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    UpdateBuildingInfo(building, instance.m_buildings.m_buffer[building]);
                    selectedBuilding = building;
                }
            }

            base.Update();
        }

        private void UpdateBuildingInfo(ushort buildingId, Building building)
        {
            var levelUpHelper = LevelUpHelper3.instance;
            var info = building.Info;
            var zone = info.m_class.GetZone();
            Building data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId];
            Singleton<ImmaterialResourceManager>.instance.CheckLocalResources(data.m_position, out ushort[] array, out int num);
            float x = 14, s = 7, y = s * 2, negativeX = 14f, maxHeight = y;
            int col = 0, onFactor = -1;
            levelUpHelper.GetEducationHappyScore(buildingId, out float education, out float happy, out float commute);
            foreach (var resBar in aresourceBars)
            {
                onFactor++;

                var label = aresourceLabels[onFactor];
                label.relativePosition = new Vector3(14 + (col * 230), y + s / 2);
                resBar.relativePosition = new Vector3(120 + (col * 230), y);
                resBar.size = new Vector2(100, 16);
                y += resBar.size.y + s;
                switch (onFactor)
                {
                    case 0:
                        label.text = zone == ItemClass.Zone.ResidentialHigh
                            || zone == ItemClass.Zone.ResidentialLow
                            || zone == ItemClass.Zone.CommercialHigh
                            || zone == ItemClass.Zone.CommercialLow
                            ? Localization.Get(LocalizationCategory.BuildingInfo, "LandValueProgress")
                            : Localization.Get(LocalizationCategory.BuildingInfo, "Service");
                        SetProgress(
                            resBar,
                            levelUpHelper.GetProperServiceScore(buildingId),
                            levelUpHelper.GetServiceThreshhold((ItemClass.Level)(Math.Max(-1, (int)data.Info.m_class.m_level - 1)), zone),
                            levelUpHelper.GetServiceThreshhold(data.Info.m_class.m_level, zone),
                            true);
                        break;
                    case 1:
                        SetProgress(
                            resBar,
                            education,
                            levelUpHelper.GetEducationThreshhold((ItemClass.Level)(Math.Max(-1, (int)data.Info.m_class.m_level - 1)), zone),
                            levelUpHelper.GetEducationThreshhold(data.Info.m_class.m_level, zone),
                            true);
                        label.text = zone == ItemClass.Zone.CommercialHigh || zone == ItemClass.Zone.CommercialLow
                            ? Localization.Get(LocalizationCategory.BuildingInfo, "Wealth")
                            : Localization.Get(LocalizationCategory.BuildingInfo, "Education");
                        break;
                    case 2:
                        label.text = Localization.Get(LocalizationCategory.BuildingInfo, "Happiness");
                        SetProgress(resBar, happy, 0, 100, true);
                        break;
                    case 3:
                        {
                            int max = 0;
                            int raw = 0;
                            var value = levelUpHelper.GetServiceScore(ImmaterialResourceManager.Resource.NoisePollution, zone, array, num, ref raw, ref max);
                            SetProgress(resBar, (float)value, 0, 100, false);
                            break;
                        }
                    case 4:
                        {
                            int max = 0;
                            int raw = 0;
                            var value = levelUpHelper.GetServiceScore(ImmaterialResourceManager.Resource.Abandonment, zone, array, num, ref raw, ref max);
                            SetProgress(resBar, (float)value, 0, 100, false);
                            break;
                        }
                    case 5:
                        SetProgress(resBar, (float)levelUpHelper.GetPollutionScore(data, zone), 0, 100, false);
                        break;
                }
                resBar.tooltip = string.Format("{0} ({1:P0})", tooltips[onFactor + 26], resBar.value);
                if (onFactor == 2)
                {
                    col = 1;
                    y = 14;
                }
                maxHeight = Math.Max(y, maxHeight);
            }
            
            int numFactors = 0;
            float newTop = maxHeight;
            foreach (var resBar in resourceBars)
            {
                if (levelUpHelper.GetFactor(zone, resBar.Key) != 0)
                    numFactors++;
            }

            y = newTop;
            int halfNumFactors = Mathf.CeilToInt(numFactors / 2);
            onFactor = 0;
            col = 0;
            foreach (var resBar in resourceBars)
            {
                onFactor++;

                var label = resourceLabels[resBar.Key];
                var factor = levelUpHelper.GetFactor(zone, resBar.Key);
                if (factor == 0)
                {
                    label.Hide();
                    resBar.Value.Hide();
                }
                else
                {
                    label.Show();
                    resBar.Value.Show();
                    int max = 0;
                    int raw = 0;
                    var value = levelUpHelper.GetServiceScore(resBar.Key, zone, array, num, ref raw, ref max);
                    switch (ModConfig.Instance.Display)
                    {
                        case ModConfig.DisplayCategory.Compact:
                            label.relativePosition = new Vector3(x, newTop + s);
                            resBar.Value.relativePosition = new Vector3(x, newTop + label.height + s);
                            resBar.Value.size = new Vector2(barWidth / halfNumFactors - s, 16);
                            x += resBar.Value.size.x + s;
                            break;
                        case ModConfig.DisplayCategory.Extended:
                            label.relativePosition = new Vector3(14 + (col * 230), y + s);
                            resBar.Value.relativePosition = new Vector3(120 + (col * 230), y);
                            resBar.Value.size = new Vector2(100, 16);
                            y += resBar.Value.size.y + s;
                            break;
                    }
                    SetProgress(resBar.Value, (float)value, 0, 100, factor > 0);
                    resBar.Value.tooltip = string.Format("{0} ({1:P0})", tooltips[(int)resBar.Key], resBar.Value.value);
                    if (onFactor == halfNumFactors)
                    {
                        col = 1;
                        x = 14;
                        y = newTop;
                        if (ModConfig.Instance.Display == ModConfig.DisplayCategory.Compact)
                            newTop += ((s * 3) + 16) * col;
                    }
                    maxHeight = Math.Max(y, maxHeight);
                }
            }
            switch (ModConfig.Instance.Display)
            {
                case ModConfig.DisplayCategory.Compact:
                    maxHeight = newTop + 16 + s * 4;
                    break;
                case ModConfig.DisplayCategory.Extended:
                    maxHeight = maxHeight + s;
                    break;
            };

            if (baseBuildingWindow != null)
            {
                if (buildingName == null)
                {
                    buildingName = baseBuildingWindow.Find<UITextField>("BuildingName");
                    buildingName.maxLength = 50;
                    buildingName.textScale = 0.87f;
                }
                if (buildingName != null)
                {
                    var bName = buildingName.text;
                    if (ModConfig.Instance.EnableNames)
                    {
                        if ((data.m_flags & Building.Flags.CustomName) == Building.Flags.None && !buildingName.hasFocus)
                        {
                            bName = GetName(buildingId, zone, data.Info.m_class.m_subService);
                            buildingName.text = bName;
                        }
                    }

                    if (ModConfig.Instance.EnableDescriptions)
                    {
                        descriptionLabel.text = GetDescription(bName, buildingId, zone, data.Info.m_class.m_subService);
                        descriptionLabel.Show();
                        descriptionLabel.relativePosition = new Vector3(14, maxHeight);
                        maxHeight += descriptionLabel.height + s * 2;
                    }
                    else
                        descriptionLabel.Hide();
                }
            }
            height = maxHeight;
        }

        private string GetDescription(string bName, ushort buildingId, ItemClass.Zone zone, ItemClass.SubService ss)
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
        
        private string GetName(ushort buildingId, ItemClass.Zone zone, ItemClass.SubService ss)
        {
            Randomizer randomizer = new Randomizer(Singleton<SimulationManager>.instance.m_metaData.m_gameInstanceIdentifier.GetHashCode() - buildingId);
            if (buildingId % 6 != 0)
            {
                if (!buildingNames.TryGetValue(ss.ToString(), out Markov markov))
                    buildingNames.TryGetValue(zone.ToString(), out markov);

                if (markov != null)
                    return markov.GetText(ref randomizer, 6, 16, true, true);
            }
            return buildingName.text;
        }

        private void SetProgress(UIProgressBar serviceBar, float val, float start, float target, bool isPositive)
        {
            float finalValue = (val - start) / (target - start);
            serviceBar.tooltip = string.Format(serviceBar.tooltip, LocaleFormatter.FormatPercentage((int)(finalValue * 100)));
            if (isPositive)
                serviceBar.progressColor = Color.Lerp(Color.yellow, Color.green, finalValue);
            else
                serviceBar.progressColor = Color.Lerp(Color.yellow, Color.red, finalValue);

            serviceBar.value = Mathf.Clamp01(finalValue);
        }

        private InstanceID GetParentInstanceId()
        {
            if (baseSub == null)
                baseSub = baseBuildingWindow.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (InstanceID)baseSub.GetValue(baseBuildingWindow);
        }
    }
}
