using ICities;

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


        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox("Enable random building names", EB_Options.Instance.enableNames, BuildingNameCheck);
            helper.AddCheckbox("Enable random building descriptions", EB_Options.Instance.enableDescriptions, BuildingDescCheck);
        }

        private void BuildingNameCheck(bool enableName)
        {
            EB_Options.Instance.enableNames = enableName;
            EB_Options.Instance.Save();

        }

        private void BuildingDescCheck(bool enableDesc)
        {
            EB_Options.Instance.enableDescriptions = enableDesc;
            EB_Options.Instance.Save();

        }
    }
}
