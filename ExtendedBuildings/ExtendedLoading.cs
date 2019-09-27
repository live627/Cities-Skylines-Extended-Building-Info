﻿using ColossalFramework.UI;
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

        private static IEnumerable<UIPanel> UIPanelInstances
        {
            get
            {
                return UIView.library.m_DynamicPanels.Select(p => p.instance).OfType<UIPanel>();
            }
        }

        private static string[] UIPanelNames
        {
            get
            {
                return UIPanelInstances.Select(p => p.name).ToArray();
            }
        }

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
            {
                throw new ExtendedLoadingException("UIPanel not found (update broke the mod!): (Library) ZonedBuildingWorldInfoPanel\nAvailable panels are:\n" +
                    string.Join("  \n", UIPanelNames));
            }
            this.buildingWindow = buildingWindowGameObject.AddComponent<BuildingInfoWindow5>();
            this.buildingWindow.transform.parent = buildingInfo.transform;
            this.buildingWindow.size = new Vector3(buildingInfo.size.x, buildingInfo.size.y);
            this.buildingWindow.baseBuildingWindow = buildingInfo.gameObject.transform.GetComponentInChildren<ZonedBuildingWorldInfoPanel>();
            this.buildingWindow.position = new Vector3(0, 12);
            buildingInfo.eventVisibilityChanged += buildingInfo_eventVisibilityChanged;

            var serviceBuildingInfo = GetPanel("(Library) CityServiceWorldInfoPanel");//UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (serviceBuildingInfo == null)
            {
                throw new ExtendedLoadingException("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n" +
                    string.Join("  \n", UIPanelNames));
            }
            serviceWindow = buildingWindowGameObject.AddComponent<ServiceInfoWindow2>(); 
            serviceWindow.servicePanel = serviceBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            
            serviceBuildingInfo.eventVisibilityChanged += serviceBuildingInfo_eventVisibilityChanged;
        }

        /*
        public static void DebugPrint(string message)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, message);
        }
        */

        private void serviceBuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            serviceWindow.Update();
        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            if (buildingWindow != null)
            {
                if (this.buildingWindow.parent != null)
                {
                    this.buildingWindow.parent.eventVisibilityChanged -= buildingInfo_eventVisibilityChanged;
                }
            }

            if (buildingWindowGameObject != null)
            {
                GameObject.Destroy(buildingWindowGameObject);
            }
        }

        void buildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            this.buildingWindow.isEnabled = value;
            if (value)
            {
                this.buildingWindow.Show();
            }
            else
            {
                this.buildingWindow.Hide();
            }
        }

    }
}
