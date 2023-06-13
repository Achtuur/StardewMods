using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SpaceShared;

namespace PrismaticStatue
{

    internal class ModConfig
    {
        private const int MaxAllowedStatues = 10;

        /// <summary>
        /// Factor for slow option, after 5 statues a 50% speedup is gained.
        /// </summary>
        private static readonly float SlowFactor = (float) Math.Pow(0.5f, 1f / 5f);

        /// <summary>
        /// Factor for medium option, after 3 statues a 50% speedup is gained.
        /// </summary>
        private static readonly float MediumFactor = (float) Math.Pow(0.5f, 1f / 3f);

        /// <summary>
        /// Factor for fast option, after 2 statues a 50% speedup is gained.
        /// </summary>
        private static readonly float FastFactor = (float) Math.Pow(0.5f, 1f / 2f);

        /// <summary>
        /// Maximum number of statues until no more speedup is provided, defaults to 5
        /// </summary>
        public int MaxStatues { get; set; }

        /// <summary>
        /// Factor that is applied to speedup formula, lower values = less diminishing returns. Defaults to 5.
        /// </summary>
        public float StatueSpeedupFactor { get; set; }

        public SButton OverlayButton { get; set; }

        private int TableTime { get; set; }

        public ModConfig()
        {
            this.MaxStatues = 5;
            this.StatueSpeedupFactor = MediumFactor;
            this.TableTime = 300;
            OverlayButton = SButton.L;
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
                    ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config);
                    // Re-register
                    configMenu.Unregister(ModEntry.Instance.ModManifest);
                    ModEntry.Instance.Config.createMenu();
                }
            );
            /// General settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );

            configMenu.AddKeybind(
               mod: ModEntry.Instance.ModManifest,
               name: I18n.CfgOverlaybutton_Name,
               tooltip: I18n.CfgOverlaybutton_Desc,
               getValue: () => this.OverlayButton,
               setValue: value => this.OverlayButton = value
            );

            /// max statue
            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgMaxstatues_Name,
                tooltip: I18n.CfgMaxstatues_Desc,
                min: 1,
                max: MaxAllowedStatues,
                interval: 1,
                getValue: () => this.MaxStatues,
                setValue: val => this.MaxStatues = val
            );

            /// statue factor
            configMenu.AddTextOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgStatuespeedupfactor_Name,
                tooltip: I18n.CfgStatuespeedupfactor_Desc,
                getValue: () => this.StatueSpeedupFactor.ToString(),
                setValue: val => this.StatueSpeedupFactor = float.Parse(val),
                allowedValues: new string[] { SlowFactor.ToString(), MediumFactor.ToString(), FastFactor.ToString()},
                formatAllowedValue: DisplayStatueFactorValues
            );


            /// Speedup table section
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_Speeduptable,
                tooltip: I18n.CfgSection_Speeduptable_Desc
            );

            /// test time
            configMenu.AddTextOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgTabletest_Name,
                tooltip: I18n.CfgTabletest_Desc,
                getValue: () => this.TableTime.ToString(),
                setValue: val => this.TableTime = int.Parse(val),
                allowedValues: new string[] { "30", "120", "200", "300", "480", "540", "4000", "6000", "8000", "10000"},
                formatAllowedValue: DisplayTableTimeValues
            );


            // Get table values
            List<string> statue_table_times = new List<string>();
            for (int i = 0; i <= this.MaxStatues; i++)
            {
                int minutes_left = SpedUpMachineWrapper.SpeedUpFunction(this.TableTime, i);
                statue_table_times.Add($"{FormatNStatues(i)}: {FormatMinutes(minutes_left)}");
            }


            // Create paragraphs for table values
            foreach(string entry in statue_table_times)
            {
                configMenu.AddParagraph(
                    mod: ModEntry.Instance.ModManifest,
                    text: () => entry
                );
            }

        }

        private static string DisplayStatueFactorValues(string expgain_option)
        {
            if (expgain_option == FastFactor.ToString())
                return "Fast";
            if (expgain_option == MediumFactor.ToString())
                return "Normal";
            if (expgain_option == SlowFactor.ToString())
                return "Slow";
            
            return "Something went wrong... :(";
        }

        private static string DisplayTableTimeValues(string tabletime_option)
        {
            string item_name;
            switch (tabletime_option)
            {
                case "30": 
                    item_name = "Copper Bar";
                    break;
                case "120":
                    item_name = "Iron Bar";
                    break;
                case "200":
                    item_name = "Cheese";
                    break;
                case "300":
                    item_name = "Gold Bar";
                    break;
                case "480":
                    item_name = "Iridum Bar";
                    break;
                case "540":
                    item_name = "Radioactive Bar";
                    break;
                case "4000":
                    item_name = "Jelly/Pickles/Aged Roe";
                    break;
                case "6000":
                    item_name = "Juice (keg)";
                    break;
                case "8000":
                    item_name = "Crystalarium (Diamond)";
                    break;
                case "10000":
                    item_name = "Wine (keg)";
                    break;
                default:
                    item_name = "Copper Bar";
                    break;
            }

            return $"{item_name} ({FormatMinutes(int.Parse(tabletime_option))})";
        }

        public static string DisplayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }

        private static string FormatNStatues(int n_statues)
        {
            switch (n_statues)
            {
                case 0:
                    return "No Statues";
                case 1:
                    return "1 Statue";
                default:
                    return $"{n_statues} Statues";
            }
        }

        private static string FormatMinutes(int minutes)
        {
            int days = (minutes >= 1600) ? minutes / 1600 : 0;
            minutes %= 1600;
            int hours = (minutes >= 60) ? minutes / 60 : 0;
            minutes %= 60;

            string time = "";

            if (days > 0)
            {
                time += $"{days}d" + ((minutes > 0 || hours > 0) ? " " : "");
            }

            if (hours > 0)
            {
                time += $"{hours}h" + ((minutes > 0) ? " " : "");
            }

            if (minutes > 0 || (days == 0 && hours == 0))
            {
                time += $"{minutes}m";
            }
            return time;
        }
    }

    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void Unregister(IManifest mod);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddImage(IManifest mod, Func<Texture2D> texture, Rectangle? texturePixelArea = null, int scale = Game1.pixelZoom);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}


