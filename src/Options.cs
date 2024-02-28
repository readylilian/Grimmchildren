using BepInEx.Logging;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace SlugTemplate
{
    public class Options : OptionInterface
    {
        private readonly ManualLogSource Logger;

        public Options(Plugin modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;

            // Bind a key to the option, as well as a default value
            testOption = config.Bind("test", false);
        }

        public readonly Configurable<bool> testOption;
        private UIelement[] UIArrPlayerOptions;


        public override void Initialize()
        {
            var opTab = new OpTab(this, "Options");

            // Adds created tabs to remix menu
            Tabs = new[]
            {
            opTab
        };

            // Painstakingly define the layout of the tab
            UIArrPlayerOptions = new UIelement[]
            {
            new OpLabel(10f, 550f, "Options", true),
            new OpLabel(10f, 520f, "Test Option")
            {
                description = "This option should always be saved if confirmed, and is accessible in other classes"
            },
            new OpCheckBox(testOption, 105f, 520f)
            };
            opTab.AddItems(UIArrPlayerOptions);
        }

        public override void Update()
        {
            // If you want anything to change in view based on the options (color/image/etc.), do that here
        }
    }
}