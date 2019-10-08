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
        string[] strs;
        CityServiceWorldInfoPanel m_servicePanel;
        public CityServiceWorldInfoPanel ServicePanel
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
            if (ServicePanel == null)
                return;

            var buildingId = GetParentInstanceId().Building;
            if (enabled && info.isVisible && BuildingManager.instance != null && ((SimulationManager.instance.m_currentFrameIndex & 15u) == 15u || lastSelected != buildingId))
            {
                strs = new string[] { Environment.NewLine, "", "", "", "", "" };
                lastSelected = buildingId;
                Building data = BuildingManager.instance.m_buildings.m_buffer[buildingId];
                FindBuildingType(data);
                label1.text = String.Format(strs[5] == ""
                    ? "{0}{0}{0}{1} : {2}{0}{3} : {4}"
                    : "{0}{0}{0}{1} : {2}{0}{3} : {4}{0}{5}", strs);
            }
        }

        private void FindBuildingType(Building data)
        {
            var productionRate = PlayerBuildingAI.GetProductionRate(data.m_productionRate,
                EconomyManager.instance.GetBudget(data.Info.m_class));
            if (data.m_fireIntensity != 0)
                productionRate = 0;

            switch (data.Info.m_buildingAI)
            {
                case FireStationAI ai:
                    strs[2] = (ai.m_fireDepartmentAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "FireFighting");
                    strs[4] = (ai.m_fireDepartmentRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    strs[5] = Localization.Get(LocalizationCategory.ServiceInfo, "KittensSaved") + ": " + GetLlamaSightings(1.4);
                    break;
                case MonumentAI ai:
                    strs[2] = (ai.m_entertainmentAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Entertainment");
                    strs[4] = (ai.m_entertainmentRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    var tourism = ai.m_attractivenessAccumulation * productionRate / 100;
                    strs[5] = Localization.Get(LocalizationCategory.ServiceInfo, "Attractiveness") + ": " + tourism.ToString();
                    break;
                case HospitalAI ai:
                    strs[2] = (ai.m_healthCareAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Healthcare");
                    strs[4] = (ai.m_healthCareRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    break;
                case CemeteryAI ai:
                    strs[2] = (ai.m_deathCareAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Deathcare");
                    strs[4] = (ai.m_deathCareRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    break;
                case ParkAI ai:
                    strs[2] = (ai.m_entertainmentAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Entertainment");
                    strs[4] = (ai.m_entertainmentRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    strs[5] = Localization.Get(LocalizationCategory.ServiceInfo, "Llamas") + ": " + GetLlamaSightings(2);
                    break;
                case SchoolAI ai:
                    strs[2] = (ai.m_educationAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Education");
                    strs[4] = (ai.m_educationRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    strs[5] = Localization.Get(LocalizationCategory.ServiceInfo, "ClassesSkipped") + ": " + GetLlamaSightings(2);
                    break;
                case PoliceStationAI ai:
                    strs[2] = (ai.m_policeDepartmentAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Police");
                    strs[4] = (ai.m_policeDepartmentRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    break;
                case CargoStationAI ai:
                    strs[2] = (ai.m_cargoTransportAccumulation * productionRate / 100).ToString();
                    strs[1] = Localization.Get(LocalizationCategory.ServiceInfo, "Cargo");
                    strs[4] = (ai.m_cargoTransportRadius / 8).ToString("F0");
                    strs[3] = Localization.Get(LocalizationCategory.ServiceInfo, "Radius");
                    break;
            }
        }

        private string GetLlamaSightings(double scale) =>
            ((SimulationManager.instance.m_currentGameTime.DayOfYear * scale + GetParentInstanceId().Building) / 1000).ToString("F0");

        private InstanceID GetParentInstanceId()
        {
            if (baseSub == null)
                baseSub = m_servicePanel.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (InstanceID)baseSub.GetValue(m_servicePanel);
        }
    }
}
