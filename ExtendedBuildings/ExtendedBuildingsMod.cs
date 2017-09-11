using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ExtendedBuildings
{
    public class ExtendedBuildingsMod : IUserMod
    {
        public string Name
        {
            get
            {
                return "Extended Building Information";
            }
        }
        public string Description
        {
            get
            {
                return "Displays level up requirements, and an optional random bulding name/company description.";
            }
        }

        private void writeXmlSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\n";

            XmlWriter xmlWriter = XmlWriter.Create("ExtendedBuildingUISettings.xml", settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("settings");

            foreach (KeyValuePair<string, object> entry in userSettings)
            {
                xmlWriter.WriteStartElement(entry.Key);
                xmlWriter.WriteString(entry.Value.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void BuildingNameCheck(bool enableName)
        {
            //BuildingInfoWindow5.ShowName = enableName;

            userSettings["enableNames"] = enableName;
            writeXmlSettings();
        }

        private void BuildingDescCheck(bool enableDesc)
        {
            //BuildingInfoWindow5.ShowDescription = enableDesc;

            userSettings["enableDescriptions"] = enableDesc;
            writeXmlSettings();
        }

        public static Dictionary<string, object> userSettings = new Dictionary<string, object>();
        public void OnSettingsUI(UIHelperBase helper)
        {
            //XML initial load or creation
            if (userSettings.Count <= 0)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("ExtendedBuildingUISettings.xml");

                    bool nameEnable = Convert.ToBoolean(xmlDoc.SelectSingleNode("settings/enableNames").InnerText);
                    //BuildingInfoWindow5.ShowName = nameEnable;
                    userSettings.Add("enableNames", nameEnable);

                    bool descEnable = Convert.ToBoolean(xmlDoc.SelectSingleNode("settings/enableDescriptions").InnerText);
                    //BuildingInfoWindow5.ShowDescription = descEnable;
                    userSettings.Add("enableDescriptions", descEnable);
                }
                catch (FileNotFoundException)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                    settings.NewLineChars = "\n";

                    XmlWriter xmlWriter = XmlWriter.Create("ExtendedBuildingUISettings.xml", settings);

                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("settings");

                    xmlWriter.WriteStartElement("enableNames");
                    xmlWriter.WriteString("True");
                    xmlWriter.WriteEndElement();
                    userSettings.Add("enableNames", true);

                    xmlWriter.WriteStartElement("enableDescriptions");
                    xmlWriter.WriteString("True");
                    xmlWriter.WriteEndElement();
                    userSettings.Add("enableDescriptions", true);

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Close();
                }
            }

            UIHelperBase group = helper.AddGroup("Extended Building Info");

            group.AddCheckbox("Enable random building names", (bool) userSettings["enableNames"], BuildingNameCheck);
            group.AddCheckbox("Enable random building descriptions", (bool) userSettings["enableDescriptions"], BuildingDescCheck);
        }
    }
}
