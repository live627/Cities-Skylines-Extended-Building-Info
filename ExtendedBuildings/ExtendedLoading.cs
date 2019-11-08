using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ExtendedBuildings
{
    public class ExtendedLoading : LoadingExtensionBase
    {
        static GameObject buildingWindowGameObject;
        BuildingInfoWindow5 buildingWindow;
        CityServiceLabel serviceWindow;
        private LoadMode _mode;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            _mode = mode;

            bool found = false;
            buildingWindowGameObject = new GameObject("buildingWindowObject");
            for (int i = 0; i < UIView.library.m_DynamicPanels.Length; i++)
                switch (UIView.library.m_DynamicPanels[i].instance.GetComponent<BuildingWorldInfoPanel>())
                {
                    case ZonedBuildingWorldInfoPanel panel:
                        if (found) break;
                        var buildingInfo = panel.component;

                        buildingWindow = buildingWindowGameObject.AddComponent<BuildingInfoWindow5>();
                        buildingWindow.transform.parent = buildingInfo.transform;
                        buildingWindow.size = new Vector3(buildingInfo.size.x, buildingInfo.size.y);
                        buildingWindow.baseBuildingWindow = panel;
                        buildingWindow.position = new Vector3(0, 12);
                        buildingInfo.eventVisibilityChanged += BuildingInfo_eventVisibilityChanged;
                        found = true;
                        break;
                    case CityServiceWorldInfoPanel panel:
                        serviceWindow = buildingWindowGameObject.AddComponent<CityServiceLabel>();
                        serviceWindow.ServicePanel = panel;
                        panel.component.eventVisibilityChanged += ServiceBuildingInfo_eventVisibilityChanged;
                        break;
                }
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
