using System;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.Plugins;

namespace ExtendedBuildings
{
    public class ModConfig
    {
        public enum DisplayCategory { Compact, Extended };
        private const string optionsFileName = "ExtendedBuildingInfoOptions.xml";
        private static ModConfig _instance;

        public static ModConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateFromFile();

                return _instance;
            }
        }

        public float Opacity { get; internal set; } = 0.85f;
        public DisplayCategory Display { get; internal set; } = DisplayCategory.Compact;
        public bool EnableNames { get; internal set; } = true;
        public bool EnableDescriptions { get; internal set; } = true;

        public void Save()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ModConfig));
                using (TextWriter writer = new StreamWriter(optionsFileName))
                {
                    ser.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Ex. Building Info: " + ex.Message);
            }
        }

        public static ModConfig CreateFromFile()
        {
            if (!File.Exists(optionsFileName))
                return new ModConfig();

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ModConfig));
                using (TextReader reader = new StreamReader(optionsFileName))
                {
                    ModConfig instance = (ModConfig)ser.Deserialize(reader);

                    return instance;
                }
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Ex. Building Info: " + ex.Message);

                return new ModConfig();
            }
        }
    }
}