using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;
namespace PrismaticStatue
{
    public class SpedUpMachineWrapper
    {
        SObject entity;
        IMachine automateMachine;
        MachineState previousState;

        /// <summary>
        /// Initial MinutesUntilReady when this wrapper was constructed
        /// </summary>
        int intialMinutesUntilReady;

        /// <summary>
        /// Current MinutesUntilReady as if there was no speedup, decremented by 10 every ten minutes
        /// </summary>
        int currentMinutesUntilReady;

        int originalMinutesUntilReady;

        /// <summary>
        /// Amount of statues in the same group as this machine
        /// </summary>
        int n_statues;

        bool spedUp;
        
        public SpedUpMachineWrapper(IMachine machine, int n_statues)
        {
            this.automateMachine = machine;
            this.n_statues = n_statues;
            this.entity = SpedUpMachineGroup.GetMachineEntity(this.automateMachine);

            Initialise();
        }

        public void Initialise()
        {
            if (this.entity is null)
                return;


            this.spedUp = false;
            this.intialMinutesUntilReady = entity.MinutesUntilReady;
            this.currentMinutesUntilReady = entity.MinutesUntilReady;

            // start as disabled so if machine was processing during construction, next update will speed it up
            this.previousState = this.automateMachine.GetState();
        }

        public bool isNull()
        {
            return this.entity is null;
        }

        public bool isSpedUp()
        {
            return this.spedUp;
        }

        public GameLocation GetLocation()
        {
            return (this.automateMachine is not null) 
                ? this.automateMachine.Location 
                : null;
        }

        public Vector2? GetTile()
        {
            return (this.automateMachine is not null)
                ? new Vector2(this.automateMachine.TileArea.X, this.automateMachine.TileArea.Y)
                : null;
        }

        internal bool IsOnTile(Vector2 tile)
        {
            Vector2? this_tile = this.GetTile();
            return (this_tile is null)
                ? false
                : this_tile == tile;
        }

        public bool isSameMachine(SpedUpMachineWrapper other)
        {
            return isSameMachine(other.automateMachine);
        }

        public bool isSameMachine(IMachine machine)
        {
            return this.automateMachine.MachineTypeID == machine.MachineTypeID && this.automateMachine.TileArea == machine.TileArea;
        }

        public void OnTenMinutesTick()
        {
            if (this.currentMinutesUntilReady > 0)
            {
                this.currentMinutesUntilReady -= 10;
            }
            this.previousState = this.automateMachine.GetState();
        }

        public void UpdateState()
        { 
            // If previous state was not processing -> machine began processing and minutesuntilready should be reset
            if (ShouldDoSpeedup())
            {
                // Set initial values for minutesuntilready
                this.intialMinutesUntilReady = this.entity.MinutesUntilReady;
                this.currentMinutesUntilReady = this.intialMinutesUntilReady;

                // Calculate speedup for machine
                this.SpeedUp();

                this.previousState = MachineState.Processing;
            }
            else if (this.automateMachine.GetState() != MachineState.Processing)
            {
                this.spedUp = false;
            }
        }
        public void OnNStautesChange(int new_n_statues)
        {
            this.n_statues = new_n_statues;

            if (this.n_statues == 0)
            {
                // No statues -> restore speed
                this.RestoreSpeed();
                this.currentMinutesUntilReady = -1;
                this.intialMinutesUntilReady = -1;
            }
            else
            {
                // Force speedup with new n_statues
                this.SpeedUp();
            }
        }

        private bool ShouldDoSpeedup()
        {
            if (this.automateMachine.GetState() != MachineState.Processing || this.n_statues < 1)
                return false;

            if (this.previousState != MachineState.Processing || // wasn't processing before, but is processing now
                this.currentMinutesUntilReady == this.entity.MinutesUntilReady || // isn't sped up
                this.intialMinutesUntilReady == -1) // speed has been restored
                return true;

            return false;
        }
        public void SpeedUp()
        {
            // Don't speedup if there is nothing to speedup
            if (this.automateMachine.GetState() != MachineState.Processing || this.intialMinutesUntilReady <= 0)
                return;

            this.spedUp = true;
            this.entity.MinutesUntilReady = SpeedUpFunction(this.currentMinutesUntilReady, this.n_statues);
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{automateMachine.MachineTypeID} at {automateMachine.Location} ({automateMachine.TileArea.X}, {automateMachine.TileArea.Y}) was sped up: {this.currentMinutesUntilReady} -> {this.entity.MinutesUntilReady}");
        }


        public void RestoreSpeed()
        {
            this.spedUp = false;
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{automateMachine.MachineTypeID} at {automateMachine.Location} ({automateMachine.TileArea.X}, {automateMachine.TileArea.Y}) speed restored: {this.entity.MinutesUntilReady} -> {currentMinutesUntilReady}");
            this.entity.MinutesUntilReady = this.currentMinutesUntilReady;
        }

        /// <summary>
        /// Calculates new time left based on original time left 
        /// </summary>
        /// <param name="original_time"></param>
        public static int SpeedUpFunction(int minutes_until_ready, int n_statues)
        {
            if (n_statues == 0)
                return minutes_until_ready;
            
            // max of 10 statues -> divides time by 3
            double statues = Math.Min(ModEntry.Instance.Config.MaxStatues, n_statues);

            // Do factor^n_statues
            double factor = Math.Pow(ModEntry.Instance.Config.StatueSpeedupFactor, statues);
            double minutes_unrounded = minutes_until_ready * factor;

            return RoundToNearestTenth((int) minutes_unrounded);
        }

        /// <summary>
        /// Rounds <paramref name="x"/> to nearest tenth, with a minimum of 0
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static int RoundToNearestTenth(int x)
        {
            return Math.Max(x - (x % 10), 0);
        }

       
    }
}
