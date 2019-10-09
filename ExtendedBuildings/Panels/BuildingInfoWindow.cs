namespace ExtendedBuildings
{
    using ColossalFramework;
    using ColossalFramework.UI;
    using ICities;
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

        UILabel descriptionLabel;

        ushort selectedBuilding;
        private List<UIProgressBar> aresourceBars;
        private List<UILabel> aresourceLabels;
        private string[] tooltips = new string[32];
        public static bool ShowDescription { get; set; } = true;
        public static bool ShowName { get; set; } = true;

        public override void Awake()
        {
            resourceBars = new Dictionary<ImmaterialResourceManager.Resource, UIProgressBar>(20);
            resourceLabels = new Dictionary<ImmaterialResourceManager.Resource, UILabel>(20);
            var resNames = Enum.GetNames(typeof(ImmaterialResourceManager.Resource));
            for (var i = 0; i < 27; i += 1)
            {
                var res = (ImmaterialResourceManager.Resource)i;
                if (CanShowResource(res))
                {
                    var bar = AddUIComponent<UIProgressBar>();
                    bar.backgroundSprite = "LevelBarBackground";
                    bar.progressSprite = "LevelBarForeground";
                    resourceBars.Add(res, bar);
                    var label = AddUIComponent<UILabel>();
                    tooltips[i] = Localization.Get(LocalizationCategory.BuildingInfo, resNames[i]);
                    label.text = Localization.Get(LocalizationCategory.BuildingInfo, resNames[i]);
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
                resNames[8], // Noise
                resNames[18], // Abandonment
                "Pollution"
            };
            var adesc = new string[]
            {
                "ServiceDescription", 
                "EducationDescription",
                "HappinessDescription",
                resNames[8], // Noise
                resNames[18], // Abandonment
                "Pollution"
            };
            for (var i = 0; i < 6; i += 1)
            {
                var bar = AddUIComponent<UIProgressBar>();
                bar.backgroundSprite = "LevelBarBackground";
                bar.progressSprite = "LevelBarForeground";
                aresourceBars.Add(bar);
                var label = AddUIComponent<UILabel>();
                tooltips[i + 26] = Localization.Get(LocalizationCategory.BuildingInfo, adesc[i]);
                label.text = Localization.Get(LocalizationCategory.BuildingInfo, ares[i]);
                label.textScale = 0.75f;
                label.size = new Vector2(100, 20);
                aresourceLabels.Add(label);
            }
            descriptionLabel = AddUIComponent<UILabel>();

            base.Awake();

            ColossalFramework.Globalization.LocaleManager.eventLocaleChanged += OnLocaleChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            ColossalFramework.Globalization.LocaleManager.eventLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged()
        {
            var resNames = Enum.GetNames(typeof(ImmaterialResourceManager.Resource));
            for (var i = 0; i < 27; i += 1)
                if (CanShowResource((ImmaterialResourceManager.Resource)i))
                    tooltips[i] = resourceLabels[(ImmaterialResourceManager.Resource)i].text = Localization.Get(LocalizationCategory.BuildingInfo, resNames[i]);

            var ares = new string[]
            {
                "Service", 
                "Education",
                "Happiness",
                resNames[8], // Noise
                resNames[18], // Abandonment
                "Pollution"
            };
            var adesc = new string[]
            {
                "ServiceDescription", 
                "EducationDescription",
                "HappinessDescription",
                resNames[8], // Noise
                resNames[18], // Abandonment
                "Pollution"
            };
            for (var i = 0; i < 6; i += 1)
            {
                tooltips[i + 26] = Localization.Get(LocalizationCategory.BuildingInfo, adesc[i]);
                aresourceLabels[i].text = Localization.Get(LocalizationCategory.BuildingInfo, ares[i]);
            }
        }

        private static bool CanShowResource(ImmaterialResourceManager.Resource res)
            => res != ImmaterialResourceManager.Resource.Abandonment
                && res != ImmaterialResourceManager.Resource.Coverage
                && res != ImmaterialResourceManager.Resource.Density
                && res != ImmaterialResourceManager.Resource.NoisePollution;

        private static bool CanShowZonedResource(ImmaterialResourceManager.Resource res, ItemClass.Zone zone)
            => res == ImmaterialResourceManager.Resource.EducationElementary
                || res == ImmaterialResourceManager.Resource.EducationHighSchool
                || res == ImmaterialResourceManager.Resource.EducationUniversity
                || res == ImmaterialResourceManager.Resource.HealthCare
                || res == ImmaterialResourceManager.Resource.FireDepartment
                || res == ImmaterialResourceManager.Resource.PoliceDepartment
                || res == ImmaterialResourceManager.Resource.PublicTransport
                || res == ImmaterialResourceManager.Resource.DeathCare
                || res == ImmaterialResourceManager.Resource.Entertainment
                || res == ImmaterialResourceManager.Resource.CrimeRate
                || res == ImmaterialResourceManager.Resource.FireHazard
                || zone != ItemClass.Zone.ResidentialLow && zone != ItemClass.Zone.ResidentialHigh
                && res == ImmaterialResourceManager.Resource.CargoTransport
                || Singleton<LoadingManager>.instance.SupportsExpansion(Expansion.NaturalDisasters)
                && (res == ImmaterialResourceManager.Resource.DisasterCoverage
                || res == ImmaterialResourceManager.Resource.EarthquakeCoverage
                || res == ImmaterialResourceManager.Resource.FirewatchCoverage
                || res == ImmaterialResourceManager.Resource.RadioCoverage)
                || Singleton<LoadingManager>.instance.SupportsExpansion(Expansion.Parks)
                && (zone == ItemClass.Zone.ResidentialLow || zone == ItemClass.Zone.ResidentialHigh)
                && (res == ImmaterialResourceManager.Resource.Attractiveness
                || res == ImmaterialResourceManager.Resource.TourCoverage);

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
            if (!WorldInfoPanel.AnyWorldInfoPanelOpen())
                return;

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
            float x = 14, s = 7, y = s * 2, maxHeight = y;
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
                        SetProgress(resBar, levelUpHelper.GetPollutionScore(data, zone), 0, 100, false);
                        break;
                }
                resBar.tooltip = $"{tooltips[onFactor + 26]} ({resBar.value:P0})";
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
                if (CanShowZonedResource(resBar.Key, zone))
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
                if (CanShowZonedResource(resBar.Key, zone))
                {
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
                    SetProgress(resBar.Value, (float)value, 0, 100, levelUpHelper.IsResourcePositive(resBar.Key));
                    resBar.Value.tooltip = $"{tooltips[(int)resBar.Key]} ({resBar.Value.value:P0})";
                    if (onFactor == halfNumFactors)
                    {
                        col = 1;
                        x = 14;
                        y = newTop;
                        if (ModConfig.Instance.Display == ModConfig.DisplayCategory.Compact)
                            newTop += ((s * 3) + 16) * col;
                    }
                    maxHeight = Math.Max(y, maxHeight);
                    label.Show();
                    resBar.Value.Show();
                }
                else
                {
                    label.Hide();
                    resBar.Value.Hide();
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
                            bName = Localization.GetName(buildingId, zone, data.Info.m_class.m_subService);
                            buildingName.text = bName ?? buildingName.text;
                        }
                    }

                    if (ModConfig.Instance.EnableDescriptions)
                    {
                        descriptionLabel.text = Localization.GetDescription(bName, buildingId, zone, data.Info.m_class.m_subService);
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
