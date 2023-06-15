using AchtuurCore.Patches;
using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pathoschild.Stardew.Automate;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;
using Sickhead.Engine.Util;

namespace PrismaticStatue.Patches
{
    internal class MachineGroupAutomatePatch : GenericPatcher
    {
        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            Type IMachineGroupType = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.MachineGroup");

            harmony.Patch(
                original: IMachineGroupType.GetMethod("Automate", BindingFlags.Public | BindingFlags.Instance),
                postfix: this.getHarmonyMethod(nameof(Postfix_Automate))
            );
        }

        private static void Postfix_Automate(object __instance)
        {
            if (!Context.IsWorldReady || __instance == null)
                return;

            // Get tiles property
            IReadOnlySet<Vector2> tiles = (IReadOnlySet<Vector2>) __instance.GetType().GetField("Tiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance, null);

            // Get machine list of this machine group
            IMachine[] Machines = (IMachine[]) __instance.GetType().GetProperty("Machines").GetValue(__instance, null);

            if (Machines == null)
                return;

            // Get machines that should be sped up and number of statues
            IMachine[] MachinesToSpeedup = Machines.Where(m => m.MachineTypeID != SpeedupStatue.TypeId).ToArray<IMachine>();
            int n_statues = Machines.Where(m => m.MachineTypeID == SpeedupStatue.TypeId).Count();

            // Check if SpedupMachineGroups contains machine group
            bool groupExists = UpdateExistingGroup(MachinesToSpeedup, tiles, n_statues);

            // If no machines to speed up, or group already exists -> do nothing else
            if (MachinesToSpeedup.Length <= 0 || groupExists || n_statues < 1)
                return;


            // Add machine group
            SpedUpMachineGroup machineGroup = new SpedUpMachineGroup(MachinesToSpeedup, tiles, n_statues);
            machineGroup.UpdateGroup(MachinesToSpeedup, tiles, n_statues);
            ModEntry.Instance.SpedupMachineGroups.Add(machineGroup);
        }

        /// <summary>
        /// Updates machine group if it already exists
        /// </summary>
        /// <param name="MachinesToSpeedup"></param>
        /// <param name="tiles"></param>
        /// <param name="n_statues"></param>
        /// <returns>true if group exists, false if it doesn't</returns>
        private static bool UpdateExistingGroup(IMachine[] MachinesToSpeedup, IReadOnlySet<Vector2> tiles, int n_statues)
        {
            if (MachinesToSpeedup.Length == 0)
                return false;

            var group = ModEntry.Instance.SpedupMachineGroups.Find(mgroup => mgroup.IsMachineGroup(tiles, MachinesToSpeedup[0].Location));

            if (group is null)
                return false;

            
            group.UpdateGroup(MachinesToSpeedup, tiles, n_statues);
            
            // Remove groups if statues is 0
            if (n_statues == 0)
            {
                ModEntry.Instance.SpedupMachineGroups.Remove(group);
            }

            return true;
        }
    }
}
