using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGreenhouseBTQPlus
{
    public enum Flags
    {
        Rainbow,
        Bi,
        Trans,
        Ace,
        Lesbian,
        Gay,
        Nonbinary,
        Pan,


    };
    internal class ModConfig
    {

        public Flags FlagEnabled;

        public ModConfig()
        {
            FlagEnabled = Flags.Rainbow;

        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
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
                save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSectionEnableflag,
                tooltip: null
            );

            //configMenu.AddTextOption(
            //    mod: ModEntry.Instance.ModManifest,
            //    name: I18n.CfgExpgain_Name,
            //    tooltip: I18n.CfgExpgain_Desc,
            //    getValue: () => StepsPerExp.ToString(),
            //    setValue: value => StepsPerExp = int.Parse(value),
            //    allowedValues: new string[] { "5", "10", "25", "50", "100" },
            //    formatAllowedValue: displayExpGainValues
            // );

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
