using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace PrismaticStatue
{
    internal class SpedUpMachineGroup
    {
        internal List<SpedUpMachineWrapper> Machines;
        internal GameLocation Location;

        /// <summary>
        /// Unique Id is created by group locationid + all speedup statues location sum
        /// </summary>
        internal string UniqueId;

        /// <summary>
        /// Tiles of this machinegroup, keep this here in order to know what MachineGroup this is
        /// </summary>
        internal HashSet<Vector2> Tiles;

        internal int n_statues;

        internal SpedUpMachineGroup(IMachine[] MachinesToSpeedup, HashSet<Vector2> tiles, int n_statues)
        {
            this.n_statues = n_statues;
            this.Tiles = tiles;

            // Get machine list of this machine 
            this.Machines = GetMachineList(MachinesToSpeedup);
            this.Location = MachinesToSpeedup[0].Location;
        }

        /// <summary>
        /// Checks whether <see cref="Tiles"/> are up to date, by checking if the number of actual placed statues match the internal value
        /// </summary>
        /// <returns></returns>
        internal bool TilesMatchNStatues()
        {
            return this.Tiles.Count(tile => tile.ContainsObject(ModEntry.Instance.SpeedupStatueID, Location)) == n_statues;
        }

        internal bool IsMachineGroup(HashSet<Vector2> GroupTiles, GameLocation GroupLocation)
        {
            if (this.Location != GroupLocation)
                return false;

            // If more Tiles old than new tiles, check if all new tiles are a subset of old tiles
            if (Tiles.Count > GroupTiles.Count)
            {
                return GroupTiles.All(tile => Tiles.Contains(tile));
            }
            // If more (or equal) tiles new than old, check if old tiles are a subset of new tiles
            return Tiles.All(tile => GroupTiles.Contains(tile));
        }

        /// <summary>
        /// Updates machines in this group and speedup state.
        /// </summary>
        /// <param name="MachinesToSpeedup"></param>
        /// <param name="n_statues"></param>
        /// <returns>Returns true if this group should be deleted</returns>
        internal void UpdateGroup(IMachine[] MachinesToSpeedup, HashSet<Vector2> tiles, int n_statues)
        {
            UpdateMachineList(MachinesToSpeedup, tiles);
            this.UpdateNStatues(n_statues);
        }

        internal SpedUpMachineWrapper GetMachine(IMachine machine)
        {
            return Machines.Find(sm => sm.isSameMachine(machine));
        }

        internal bool ContainsTile(Vector2 tile)
        {
            return this.Tiles.Contains(tile);
        }


        /// <summary>
        /// Takes in new value of n_statues for this machine group, if it is different then all processing machines in this group are notified
        /// </summary>
        /// <param name="new_n_statues"></param>
        internal void UpdateNStatues(int new_n_statues)
        {
            if (new_n_statues == this.n_statues)
                return;

            foreach (SpedUpMachineWrapper machine in this.Machines)
            {
                machine.OnNStautesChange(new_n_statues);
            }
            
            this.n_statues = new_n_statues;
        }

        internal void RestoreAllMachines()
        {
            foreach (SpedUpMachineWrapper machineWrapper in this.Machines)
            {
                machineWrapper.RestoreSpeed();
            }
        }

        internal void OnTenMinutesTick()
        {
            foreach (SpedUpMachineWrapper machine in this.Machines)
            {
                machine.OnTenMinutesTick();
            }
        }



        /// <summary>
        /// Updates list of processing machines in this group and updates state of all machines in list
        /// </summary>
        /// <param name="machines"></param>
        public void UpdateMachineList(IMachine[] machines, HashSet<Vector2> tiles)
        {
            // Update machine list
            if (machines.Length != this.Machines.Count)
            {
                List<SpedUpMachineWrapper> machines_wrapped = machines
                    .Select(m => new SpedUpMachineWrapper(m, this.n_statues))
                    .Where<SpedUpMachineWrapper>(wrap => !wrap.isNull())
                    .ToList();

                //if this.Machines does not contain machine && machine contains machine -> add machine
                List<SpedUpMachineWrapper> AddMachines = machines_wrapped.Where(m => !this.Machines.Any(this_m => this_m.isSameMachine(m))).ToList();

                //if this.Machines contains machine && machine does not contain machine -> remove machine
                List<SpedUpMachineWrapper> RemoveMachines = this.Machines.Where(this_m => !machines_wrapped.Any(m => m.isSameMachine(this_m))).ToList();


                // First restore speed before deleting
                foreach (SpedUpMachineWrapper machine in RemoveMachines)
                {
                    machine.RestoreSpeed();
                }

                // Remove and add machines that should be removed and added resp.
                this.Machines = this.Machines.Except(RemoveMachines).Concat(AddMachines).ToList();
            }

            // Update tiles
            if (tiles.Count != this.Tiles.Count)
            {
                this.Tiles = tiles;
            }

            foreach (SpedUpMachineWrapper machine in this.Machines)
            {
                machine.UpdateState();
            }
        }

        public List<SpedUpMachineWrapper> GetMachineList(IMachine[] machines)
        {
            return machines
                .Select(machine => new SpedUpMachineWrapper(machine, this.n_statues))
                .Where(wrap => !wrap.isNull())
                .ToList();
        }
        public static SObject GetMachineEntity(IMachine machine)
        {
            try
            {
                // Get derived class of GenericObjectMachine, which is derived of BaseMachine<MachineT>
                var BaseMachineDerived = machine.GetType().GetProperty("Machine", BindingFlags.Public | BindingFlags.Instance).GetValue(machine, null);

                // Get underlying StardewValley.Object this machine refers to
                SObject MachineEntity = BaseMachineDerived.GetType().GetProperty("Machine", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(BaseMachineDerived, null) as SObject;

                return MachineEntity;
            }
            catch (Exception e)
            {
                AchtuurCore.Logger.TraceLog(
                    ModEntry.Instance.Monitor,
                    $"Failed to find underlying machine entity for {machine.MachineTypeID} at {machine.Location} ({machine.TileArea.X}, {machine.TileArea.Y})"
                );
                return null;
            }
        }
    }    
}
