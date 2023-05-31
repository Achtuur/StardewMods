using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore
{
    public static class Debug
    {
        /// <summary>
        /// Calls <see cref="IMonitor.Log"/> with <see cref="LogLevel.Debug"/>, if configuration is set to Build.
        /// </summary>
        /// <param name="monitor">IMonitor instance, should be accessible by ModEntry.Instance</param>
        /// <param name="debug_msg">Message to display</param>
        public static void DebugLog(IMonitor monitor, string debug_msg)
        {
            #if DEBUG
                monitor.Log(debug_msg, LogLevel.Debug);
            #endif
        }

        public static string GetSkillNameFromId(int skill_id)
        {
            switch (skill_id)
            {
                case 0: return "Farming";
                case 1: return "Fishing";
                case 2: return "Foraging";
                case 3: return "Mining";
                case 4: return "Combat";
                case 5: return "Luck";
                default: return "noSkill";

            }
        }
    }
}
