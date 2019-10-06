using ICities;
using System;

namespace ExtendedBuildings
{
    public class ExtendedBuildings : IUserMod
    {
        public string Name => "Extended Building Info";

        public string Description => "Displays level up requirements, and an optional random bulding name/company description.";

        public void OnSettingsUI(UIHelperBase helper)
        {
            Version version = typeof(Localization).Assembly.GetName().Version;
            DateTime date = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);
            UIHelperBase group = helper.AddGroup($"{Name} (v{version} - {date.ToString("MMMM dd, yyyy")})");
            group.AddDropdown("Display", Enum.GetNames(typeof(ModConfig.DisplayCategory)), (int)ModConfig.Instance.Display, sel =>
            {
                ModConfig.Instance.Display = (ModConfig.DisplayCategory)sel;
                ModConfig.Instance.Save();
            });
            group.AddSlider("Opacity", 0.5f, 1f, 0.05f, ModConfig.Instance.Opacity, sel =>
            {
                ModConfig.Instance.Opacity = sel;
                ModConfig.Instance.Save();
            });
            helper.AddCheckbox("Enable random building names", ModConfig.Instance.EnableNames, enableName =>
            {
                ModConfig.Instance.EnableNames = enableName;
                ModConfig.Instance.Save();
            });
            helper.AddCheckbox("Enable random building descriptions", ModConfig.Instance.EnableDescriptions, enableDesc =>
            {
                ModConfig.Instance.EnableDescriptions = enableDesc;
                ModConfig.Instance.Save();
            });
        }
    }
}
