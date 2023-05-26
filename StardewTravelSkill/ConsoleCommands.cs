using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace StardewTravelSkill
{
    internal static class ConsoleCommands
    {
        internal static void Initialize(IModHelper helper)
        {
            helper.ConsoleCommands.Add(I18n.CmdSetlvl_Name(), I18n.CmdSetlevel_Desc(), StardewTravelSkill.ConsoleCommands.setLevel);
            helper.ConsoleCommands.Add(I18n.CmdSetmovespeed_Name(), I18n.CmdSetmovespeed_Desc(), StardewTravelSkill.ConsoleCommands.setLevelMovespeedBonus);
            helper.ConsoleCommands.Add(I18n.CmdSetsprintspeed_Name(), I18n.CmdSetsprintspeed_Desc(), StardewTravelSkill.ConsoleCommands.setSprintBonus);
        }
        public static void setLevel(string command, string[] args)
        {
            try
            {
                int lvl = int.Parse(args[0]);
                // Reduce xp to 0
                Game1.player.AddCustomSkillExperience(ModEntry.TravelSkill, -Game1.player.GetCustomSkillExperience(ModEntry.TravelSkill));

                // Add xp equivalent to level
                if (lvl != 0)
                {
                    Game1.player.AddCustomSkillExperience(ModEntry.TravelSkill, ModEntry.TravelSkill.ExperienceCurve[lvl - 1]);
                }

                ModEntry.Instance.Monitor.Log($"{Game1.player.Name}'s Travelling skill set to level {lvl}", LogLevel.Debug);
            }
            catch(Exception e)
            {
                ModEntry.Instance.Monitor.Log(I18n.CmdSetlevel_Errormsg(args[0]), LogLevel.Error);
            }
        }

        public static void setLevelMovespeedBonus(string command, string[] args)
        {
            try
            {
                float val = float.Parse(args[0]);
                ModConfig.LevelMovespeedBonus = val;
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log(I18n.CmdSetmovespeed_Errormsg(args[0]), LogLevel.Error);
            }
        }

        public static void setSprintBonus(string command, string[] args)
        {
            try
            {
                float val = float.Parse(args[0]);
                ModConfig.SprintMovespeedBonus = val;
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log(I18n.CmdSetsprintspeed_Errormsg(args[0]), LogLevel.Error);
            }
        }
    }
}
