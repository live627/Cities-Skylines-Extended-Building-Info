using ICities;
using System;

namespace ExtendedBuildings
{
    public class ExtendedBuildings : IUserMod
    {
        public string Name => "Extended Building Information";

        public string Description => "Displays level up requirements, and an optional random bulding name/company description.";

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup(Name);
            
            group.AddDropdown("Display", Enum.GetNames(typeof(ModConfig.DisplayCategory)), (int)ModConfig.Instance.Display, sel =>
            {
                ModConfig.Instance.Display = (ModConfig.DisplayCategory)sel;
                ModConfig.Instance.Save();
            });

            float selectedValue = ModConfig.Instance.Opacity;
            group.AddSlider("Opacity", 0.5f, 1f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.Opacity = sel;
                ModConfig.Instance.Save();
            });
            helper.AddCheckbox("Enable random building names", ModConfig.Instance.EnableNames, BuildingNameCheck);
            helper.AddCheckbox("Enable random building descriptions", ModConfig.Instance.EnableDescriptions, BuildingDescCheck);
        }

        private void BuildingNameCheck(bool enableName)
        {
            ModConfig.Instance.EnableNames = enableName;
            ModConfig.Instance.Save();
        }

        private void BuildingDescCheck(bool enableDesc)
        {
            ModConfig.Instance.EnableDescriptions = enableDesc;
            ModConfig.Instance.Save();
        }
    }
}
