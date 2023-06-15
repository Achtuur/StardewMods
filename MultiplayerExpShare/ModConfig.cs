using SpaceCore;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Integrations;

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
        public ExpShareRangeType ExpShareType { get; set; }

        public bool ShareAllExpAtMaxLevel { get; set; }

        /// <summary>
        /// Other farmers must be within this range to count as nearby
        /// </summary>
        public int NearbyPlayerTileRange { get; set; }

        /// <summary>
        /// Percentage of exp that goes to actor, rest of exp is divided equally between nearby players
        /// </summary>
        public float ExpPercentageToActor { get; set; }

        /// <summary>
        /// Whether Exp sharing is enabled per vanilla skill
        /// </summary>
        public bool[] VanillaSkillEnabled { get; set; }

        public Dictionary<string, bool> SpaceCoreSkillEnabled { get; set; }

        public ModConfig()
        {
            // Changable by player
            this.NearbyPlayerTileRange = 25;
            this.ExpPercentageToActor = 0.75f;
            this.ExpShareType = ExpShareRangeType.Tile;
            this.ShareAllExpAtMaxLevel = true;

            this.VanillaSkillEnabled = new[] {
                true,  // Farming
                false, // Fishing
                true,  // Foraging
                true,  // Mining
                true,  // Combat
            };

            ResetSpaceCoreDict();

        }

        private void ResetSpaceCoreDict()
        {
            if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
                return;

            SpaceCoreSkillEnabled = new Dictionary<string, bool>();

            foreach (string s in SpaceCore.Skills.GetSkillList())
            {
                SpaceCoreSkillEnabled.Add(s, false);
            }
        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            
            // register mod
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Instance.Config = new ModConfig(),
                save: () => {
                    ModEntry.Instance.Helper.WriteConfig<ModConfig>(ModEntry.Instance.Config);
                    }
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgGeneral_Name,
                tooltip: I18n.CfgGeneral_Desc
            );

            // exp percentage to actor
            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
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
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgSharetype_Name,
                tooltip: I18n.CfgSharetype_Desc,
                getValue: getExpShareType,
                setValue: setExpShareType,
                allowedValues: new string[] { "Tile", "Map", "Global" }
             );

            // nearby player tile range
            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgNearbyplayertilerange_Name,
                tooltip: I18n.CfgNearbyplayertilerange_Desc,
                getValue: () => NearbyPlayerTileRange,
                setValue: value => NearbyPlayerTileRange = value,
                min: 10,
                max: 50,
                interval: 5
             );

            // share all exp at max level
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgShareallexpmaxlevel_Name,
                tooltip: I18n.CfgShareallexpmaxlevel_Desc,
                getValue: () => ShareAllExpAtMaxLevel,
                setValue: value => ShareAllExpAtMaxLevel = value
            );

            // Enable/disable menu
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgEnablesection,
                tooltip: null
            );

            // farming
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgEnablefarming_Name,
                tooltip: I18n.CfgEnablefarming_Desc,
                getValue: () => VanillaSkillEnabled[0],
                setValue: value => VanillaSkillEnabled[0] = value
            );

            // fishing
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgEnablefishing_Name,
                tooltip: I18n.CfgEnablefishing_Desc,
                getValue: () => VanillaSkillEnabled[1],
                setValue: value => VanillaSkillEnabled[1] = value
            );

            // foraging
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgEnableforaging_Name,
                tooltip: I18n.CfgEnableforaging_Desc,
                getValue: () => VanillaSkillEnabled[2],
                setValue: value => VanillaSkillEnabled[2] = value
            );

            // mining
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgEnablemining_Name,
                tooltip: I18n.CfgEnablemining_Desc,
                getValue: () => VanillaSkillEnabled[3],
                setValue: value => VanillaSkillEnabled[3] = value
            );

            // combat
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgEnablecombat_Name,
                tooltip: I18n.CfgEnablecombat_Desc,
                getValue: () => VanillaSkillEnabled[4],
                setValue: value => VanillaSkillEnabled[4] = value
            );

            // SPACECORE SKILLS

            if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            {
                // Enable/disable menu
                configMenu.AddSectionTitle(
                    mod: ModEntry.Instance.ModManifest,
                    text: I18n.CfgEnableSpacecoresection,
                    tooltip: null
                );

                // Add config for each skill
                foreach (string skill_name in SpaceCoreSkillEnabled.Keys)
                {
                    // [0] contains mod author, [1] contains skill name
                    var skill_name_split = skill_name.Split('.');

                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance.ModManifest,
                        name: () => $"Enable {skill_name_split[1]}",
                        tooltip: () => $"Whether to enable exp sharing for {skill_name_split[1]} skill (by {skill_name_split[0]})",
                        getValue: () => SpaceCoreSkillEnabled[skill_name],
                        setValue: value => SpaceCoreSkillEnabled[skill_name] = value
                    );
                }
            }


        }

        private string getExpShareType()
        {
            switch (this.ExpShareType)
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
                case "Map": ExpShareType = ExpShareRangeType.Map; break;
                case "Tile": ExpShareType = ExpShareRangeType.Tile; break;
                case "Global": ExpShareType = ExpShareRangeType.Global; break;
                default: ExpShareType = ExpShareRangeType.Tile; break;
            }
        }
    }
}
