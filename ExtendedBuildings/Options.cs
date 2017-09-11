using System;
using System.IO;
using System.Xml.Serialization;
using ColossalFramework.Plugins;

namespace ExtendedBuildings
{
    public class EB_Options
    {
        private const string optionsFileName = "ExtendedBuildingInfoOptions.xml";
        private static EB_Options _instance;

        public static EB_Options Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateFromFile();
                }

                return _instance;
            }
        }

        //All option variables are put here.
        public bool enableNames;
        public bool enableDescriptions;

        public EB_Options()
        {
            enableNames = false;
            enableDescriptions = false;
        }


        public void Save()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EB_Options));
                TextWriter writer = new StreamWriter(optionsFileName);
                ser.Serialize(writer, this);
                writer.Close();
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Ex. Building Info: " + ex.Message);
            }
        }

        public static EB_Options CreateFromFile()
        {
            if (!File.Exists(optionsFileName)) return new EB_Options();

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EB_Options));
                TextReader reader = new StreamReader(optionsFileName);
                EB_Options instance = (EB_Options)ser.Deserialize(reader);
                reader.Close();

                return instance;
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Ex. Building Info: " + ex.Message);

                return new EB_Options();
            }
        }
    }
}