using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExpShare
{
    internal enum ExpShareRangeType
    {
        /// <summary>
        /// All players share exp globally
        /// </summary>
        Global,
        /// <summary>
        /// Players on the same map share exp
        /// </summary>
        Map,
        /// <summary>
        /// Players within a certain tile range share exp
        /// </summary>
        Tile
    }
    internal class ModConfig
    {

        /// <summary>
        /// If this is true, then two players on the same map will always count as being nearby
        /// </summary>
        public static ExpShareRangeType ExpShareType { get; set; }

        /// <summary>
        /// Other farmers must be within this range to count as nearby
        /// </summary>
        public static int NearbyPlayerTileRange { get; set; }

        /// <summary>
        /// Percentage of exp that goes to actor, rest of exp is divided equally between nearby players
        /// </summary>
        public static float ExpPercentageToActor { get; set; }

        /// <summary>
        /// Whether Exp sharing is enabled per vanilla skill
        /// </summary>
        public static bool[] VanillaSkillEnabled { get; set; }

        public ModConfig()
        {
            // Changable by player
            ModConfig.NearbyPlayerTileRange = 25;
            ModConfig.ExpPercentageToActor = 0.75f;
            ModConfig.ExpShareType = ExpShareRangeType.Tile;

            ModConfig.VanillaSkillEnabled = new[] {
                true,  // Farming
                false, // Fishing
                true,  // Foraging
                true,  // Mining
                true,  // Combat
            };

            // Unchangable by player
        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu(ModEntry instance)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: instance.ModManifest,
                reset: () => instance.Config = new ModConfig(),
                save: () => instance.Helper.WriteConfig(instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: instance.ModManifest,
                text: I18n.CfgGeneral_Name,
                tooltip: I18n.CfgGeneral_Desc
            );

            // exp percentage to actor
            configMenu.AddNumberOption(
                mod: instance.ModManifest,
                name: I18n.CfgExptoactor_Name,
                tooltip: I18n.CfgExptoactor_Desc,
                getValue: () => ExpPercentageToActor,
                setValue: value => ExpPercentageToActor = value,
                min: 25f / 100f,
                max: 75f / 100f,
                interval: 5f / 100f,
                formatValue: displayAsPercentage
             );

            // Exp share type
            configMenu.AddTextOption(
                mod: instance.ModManifest,
                name: I18n.CfgSharetype_Name,
                tooltip: I18n.CfgSharetype_Desc,
                getValue: getExpShareType,
                setValue: setExpShareType,
                allowedValues: new string[] { "Tile", "Map", "Global" }
             );

            // nearby player tile range
            configMenu.AddNumberOption(
                mod: instance.ModManifest,
                name: I18n.CfgNearbyplayertilerange_Name,
                tooltip: I18n.CfgNearbyplayertilerange_Desc,
                getValue: () => NearbyPlayerTileRange,
                setValue: value => NearbyPlayerTileRange = value,
                min: 10,
                max: 50,
                interval: 5
             );

            // Enable/disable menu
            configMenu.AddSectionTitle(
                mod: instance.ModManifest,
                text: I18n.CfgEnablesection,
                tooltip: null
            );

            // farming
            configMenu.AddBoolOption(
                mod: instance.ModManifest,
                name: I18n.CfgEnablefarming_Name,
                tooltip: I18n.CfgEnablefarming_Desc,
                getValue: () => VanillaSkillEnabled[0],
                setValue: value => VanillaSkillEnabled[0] = value
            );

            // fishing
            configMenu.AddBoolOption(
                mod: instance.ModManifest,
                name: I18n.CfgEnablefishing_Name,
                tooltip: I18n.CfgEnablefishing_Desc,
                getValue: () => VanillaSkillEnabled[1],
                setValue: value => VanillaSkillEnabled[1] = value
            );

            // foraging
            configMenu.AddBoolOption(
                mod: instance.ModManifest,
                name: I18n.CfgEnableforaging_Name,
                tooltip: I18n.CfgEnableforaging_Desc,
                getValue: () => VanillaSkillEnabled[2],
                setValue: value => VanillaSkillEnabled[2] = value
            );

            // mining
            configMenu.AddBoolOption(
                mod: instance.ModManifest,
                name: I18n.CfgEnablemining_Name,
                tooltip: I18n.CfgEnablemining_Desc,
                getValue: () => VanillaSkillEnabled[3],
                setValue: value => VanillaSkillEnabled[3] = value
            );

            // combat
            configMenu.AddBoolOption(
                mod: instance.ModManifest,
                name: I18n.CfgEnablecombat_Name,
                tooltip: I18n.CfgEnablecombat_Desc,
                getValue: () => VanillaSkillEnabled[4],
                setValue: value => VanillaSkillEnabled[4] = value
            );

        }

        private static string getExpShareType()
        {
            switch (ModConfig.ExpShareType)
            {
                case ExpShareRangeType.Tile: return "Tile";
                case ExpShareRangeType.Map: return "Map";
                case ExpShareRangeType.Global: return "Global";
            }
            // should be unreachable, if this ever appears then you made a mistake sir programmer
            return "Something went wrong... :(";
        }

        /// <summary>
        /// Displays <paramref name="value"/> as a percentage, rounded to two decimals.
        /// <c>ModConfig.displayAsPercentage(0.02542); // returns 2.54%</c>
        /// </summary>
        public static string displayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }

        private void setExpShareType(string option)
        {
            switch (option)
            {
                case "Map": ModConfig.ExpShareType = ExpShareRangeType.Map; break;
                case "Tile": ModConfig.ExpShareType = ExpShareRangeType.Tile; break;
                case "Global": ModConfig.ExpShareType = ExpShareRangeType.Global; break;
                default: ModConfig.ExpShareType = ExpShareRangeType.Tile; break;
            }
        }
    }

    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }
}
