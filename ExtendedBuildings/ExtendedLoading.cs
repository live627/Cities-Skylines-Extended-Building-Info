using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ExtendedBuildings
{
    public class ExtendedLoading : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame &&
                mode != LoadMode.LoadScenario && mode != LoadMode.NewGameFromScenario)
                return;
            
            for (int i = 0; i < UIView.library.m_DynamicPanels.Length; i++)
                switch (UIView.library.m_DynamicPanels[i].instance.GetComponent<BuildingWorldInfoPanel>())
                {
                    case ZonedBuildingWorldInfoPanel panel:
                        BuildingInfoWindow5 buildingWindow = panel.component.AddUIComponent<BuildingInfoWindow5>();
                        buildingWindow.size = panel.component.size;
                        buildingWindow.baseBuildingWindow = panel;
                        buildingWindow.position = new Vector3(0, 12);
                        break;
                    case CityServiceWorldInfoPanel panel:
                        CityServiceLabel serviceWindow = panel.component.AddUIComponent<CityServiceLabel>();
                        break;
                }
        }
    }
}
