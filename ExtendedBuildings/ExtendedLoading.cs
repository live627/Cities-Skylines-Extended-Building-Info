using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtendedBuildings
{
    public class ExtendedLoading : LoadingExtensionBase
    {
        static GameObject buildingWindowGameObject;
        BuildingInfoWindow5 buildingWindow;
        ServiceInfoWindow2 serviceWindow;
        private LoadMode _mode;

        public class ExtendedLoadingException : Exception
        {
            public ExtendedLoadingException(string message) : base(message) { }
        }

        private static IEnumerable<UIPanel> UIPanelInstances => UIView.library.m_DynamicPanels.Select(p => p.instance).OfType<UIPanel>();

        private static string[] UIPanelNames => UIPanelInstances.Select(p => p.name).ToArray();

        private UIPanel GetPanel(string name)
        {
            return UIPanelInstances.FirstOrDefault(p => p.name == name);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            _mode = mode;

            buildingWindowGameObject = new GameObject("buildingWindowObject");

            var buildingInfo = UIView.Find<UIPanel>("(Library) ZonedBuildingWorldInfoPanel");
            if (buildingInfo == null)
                throw new ExtendedLoadingException("UIPanel not found (update broke the mod!): (Library) ZonedBuildingWorldInfoPanel\nAvailable panels are:\n" +
                    string.Join("  \n", UIPanelNames));

            buildingWindow = buildingWindowGameObject.AddComponent<BuildingInfoWindow5>();
            buildingWindow.transform.parent = buildingInfo.transform;
            buildingWindow.size = new Vector3(buildingInfo.size.x, buildingInfo.size.y);
            buildingWindow.baseBuildingWindow = buildingInfo.gameObject.transform.GetComponentInChildren<ZonedBuildingWorldInfoPanel>();
            buildingWindow.position = new Vector3(0, 12);
            buildingInfo.eventVisibilityChanged += BuildingInfo_eventVisibilityChanged;

            var serviceBuildingInfo = GetPanel("(Library) CityServiceWorldInfoPanel");//UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (serviceBuildingInfo == null)
            {
                throw new ExtendedLoadingException("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n" +
                    string.Join("  \n", UIPanelNames));
            }
            serviceWindow = buildingWindowGameObject.AddComponent<ServiceInfoWindow2>(); 
            serviceWindow.ServicePanel = serviceBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            
            serviceBuildingInfo.eventVisibilityChanged += ServiceBuildingInfo_eventVisibilityChanged;
        }

        private void ServiceBuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            serviceWindow.Update();
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            if (buildingWindow != null && buildingWindow.parent != null)
                buildingWindow.parent.eventVisibilityChanged -= BuildingInfo_eventVisibilityChanged;

            if (buildingWindowGameObject != null)
                GameObject.Destroy(buildingWindowGameObject);
        }

        private void BuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            buildingWindow.isEnabled = value;
            if (value)
                buildingWindow.Show();
            else
                buildingWindow.Hide();
        }
    }
}
