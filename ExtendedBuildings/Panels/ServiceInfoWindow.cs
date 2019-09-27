using ColossalFramework.UI;
using System;
using System.Reflection;
using UnityEngine;

namespace ExtendedBuildings
{
    public class ServiceInfoWindow2 : MonoBehaviour
    {
        UILabel info;
        UILabel label1;
        FieldInfo baseSub;
        string[] strs = new string[3];
        private static readonly int LabelYOffset = 50;
        CityServiceWorldInfoPanel m_servicePanel;
        public CityServiceWorldInfoPanel servicePanel
        {
            get { return m_servicePanel; }
            set
            {
                info = value.Find<UILabel>("Info");

                label1 = info.AddUIComponent<UILabel>();
                label1.color = info.color;
                label1.textColor = info.textColor;
                label1.textScale = info.textScale;
                label1.anchor = UIAnchorStyle.Left | UIAnchorStyle.Bottom;
                label1.relativePosition = Vector3.zero;
                label1.font = info.font;
                m_servicePanel = value;
            }
        }

        int lastSelected;

        public void Update()
        {
            if (servicePanel == null)
                return;

            var buildingId = GetParentInstanceId().Building;
            if (enabled && info.isVisible && BuildingManager.instance != null && ((SimulationManager.instance.m_currentFrameIndex & 15u) == 15u || lastSelected != buildingId))
            {
                lastSelected = buildingId;
                Building data = BuildingManager.instance.m_buildings.m_buffer[buildingId];
                var service = data.Info.m_class.m_service;
                var productionRate = PlayerBuildingAI.GetProductionRate(data.m_productionRate, EconomyManager.instance.GetBudget(data.Info.m_class));
                if (data.m_fireIntensity != 0)
                    productionRate = 0;

                BuildingAI ai = data.Info.m_buildingAI;

                if (ai is FireStationAI)
                {
                    var strength = ((FireStationAI)ai).m_fireDepartmentAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "FireFighting") + ": " + strength.ToString();
                    var radius = ((FireStationAI)ai).m_fireDepartmentRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                    strs[2] = Localization.Get(LocalizationCategory.ServiceInfo, "KittensSaved") + ": " + GetLlamaSightings(1.4);
                }
                else if (ai is MonumentAI)
                {
                    var strength = ((MonumentAI)ai).m_entertainmentAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Entertainment") + ": " + strength.ToString();
                    var radius = ((MonumentAI)ai).m_entertainmentRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                    var tourism = ((MonumentAI)ai).m_attractivenessAccumulation * productionRate / 100;
                    strs[2] = Localization.Get(LocalizationCategory.ServiceInfo, "Attractiveness") + ": " + tourism;
                }
                else if (ai is HospitalAI)
                {
                    var strength = ((HospitalAI)ai).m_healthCareAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Healthcare") + ": " + strength.ToString();
                    var radius = ((HospitalAI)ai).m_healthCareRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                }
                else if (ai is CemeteryAI)
                {
                    var strength = ((CemeteryAI)ai).m_deathCareAccumulation * productionRate / 100;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Deathcare") + ": " + strength.ToString();
                    var radius = ((CemeteryAI)ai).m_deathCareRadius / 8;
                    strs[2] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                }
                else if (ai is ParkAI)
                {
                    var strength = (int)(((ParkAI)ai).m_entertainmentAccumulation * productionRate / 100);
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Entertainment") + ": " + strength.ToString();
                    var radius = (int)(((ParkAI)ai).m_entertainmentRadius / 8);
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString();
                    strs[2] = Localization.Get(LocalizationCategory.ServiceInfo, "Llamas") + ": " + GetLlamaSightings(2);
                }
                else if (ai is SchoolAI)
                {
                    var strength = ((SchoolAI)ai).m_educationAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Education") + ": " + strength.ToString();
                    var radius = ((SchoolAI)ai).m_educationRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                    strs[2] = Localization.Get(LocalizationCategory.ServiceInfo, "ClassesSkipped") + ": " + GetLlamaSightings(2);
                }
                else if (ai is PoliceStationAI)
                {
                    var strength = ((PoliceStationAI)ai).m_policeDepartmentAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Police") + ": " + strength.ToString();
                    var radius = ((PoliceStationAI)ai).m_policeDepartmentRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                }
                else if (ai is CargoStationAI)
                {
                    var strength = ((CargoStationAI)ai).m_cargoTransportAccumulation * productionRate / 100;
                    strs[0] = Localization.Get(LocalizationCategory.ServiceInfo, "Cargo") + ": " + strength.ToString();
                    var radius = ((CargoStationAI)ai).m_cargoTransportRadius / 8;
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius") + ": " + radius.ToString("F0");
                }

                //TODO calculate the best relative position for label1
                label1.text = string.Join(Environment.NewLine, strs);
            }
        }

        private string GetLlamaSightings(double scale)
        {
            return ((SimulationManager.instance.m_currentGameTime.DayOfYear * scale + GetParentInstanceId().Building) / 1000).ToString("F0");
        }

        private InstanceID GetParentInstanceId()
        {
            if (baseSub == null)
                baseSub = m_servicePanel.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (InstanceID)baseSub.GetValue(m_servicePanel);
        }
    }
}
