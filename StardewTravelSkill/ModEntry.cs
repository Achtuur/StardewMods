using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceCore;
using System.Runtime.CompilerServices;
using HarmonyLib;
using StardewTravelSkill.Patches;
using StardewValley.Menus;
using ContentPatcher.Framework;
using ContentPatcher;
using Pathoschild.Stardew.Common.Integrations.GenericModConfigMenu;
using AchtuurCore.Patches;

namespace StardewTravelSkill
{
    internal sealed class ModEntry : Mod
    {
        public static Mod Instance;
        public static IContentPatcherAPI ContentAPI;
        public static TravelSkill TravelSkill;

        public ContentPackHelper contentPackHelper;
        public ModConfig Config;

        /// <summary>
        /// Amount of steps taken the previous time it was checked
        /// </summary>
        private uint m_previousSteps;

        /// <summary>
        /// Amount of steps taken since last moving
        /// </summary>
        private uint m_consecutiveSteps;

        public static bool SprintActive { get; set; }

        /// <summary>
        /// Returns movespeed multiplier farmer should receive
        /// </summary>
        /// <returns>
        /// <c>
        ///     <see cref="TravelSkill.LevelMovespeedBonus"/> * level + [<see cref="TravelSkill.MovespeedProfessionBonus"/>]
        /// </c>
        /// </returns>
        public static float GetMovespeedMultiplier()
        {
            float professionbonus = Game1.player.HasCustomProfession(TravelSkill.ProfessionMovespeed) ? ModConfig.MovespeedProfessionBonus : 0.0f;
            float sprintbonus = SprintActive ? ModConfig.SprintMovespeedBonus : 0.0f;

            float multiplier = Game1.player.GetCustomSkillLevel(TravelSkill) * ModConfig.LevelMovespeedBonus + professionbonus + sprintbonus;
            return 1 + multiplier;
        }

        public static double GetWarpTotemConsumeChance()
        {
            return Game1.player.HasCustomProfession(TravelSkill.ProfessionTotemReuse)
                ? ModConfig.TotemUseChance
                : 1.0;
        }

        /// <summary>
        /// Returns stamina that should be restored every 10 minutes
        /// </summary>
        /// <returns></returns>
        public static double GetStaminaRestoreAmount()
        {
            return Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina)
                ? ModConfig.RestoreStaminaPercentage
                : 0.0;
        }

        /// <summary>
        /// Mod entry point, called after mod is first loaded
        /// </summary>
        /// <param name="helper">Simplified API for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            HarmonyPatcher.ApplyPatches(this,
                new MoveSpeedPatch(),
                new ReduceActiveItemPatch()
            );

            // Init references to mod api
            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            this.Config = helper.ReadConfig<ModConfig>();

            Skills.RegisterSkill(ModEntry.TravelSkill = new TravelSkill());


            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.SaveCreated += this.OnSaveCreate;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            ConsoleCommands.Initialize(helper);
        }


        /*********
        ** Private methods
        *********/
        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            // Can only assign content api on game launch as it takes a few ticks before api is available
            this.contentPackHelper = new ContentPackHelper(this);
            this.contentPackHelper.CreateTokens();

            this.Config.createMenu(this);
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.player.HasCustomProfession(TravelSkill.ProfessionSprint))
                CheckSprintActive();
        }

        private void OnSaveCreate(object sender, EventArgs e)
        {
            // Should always true but is kept here just in case
            if (!Context.IsWorldReady)
                return;

            this.m_previousSteps = Game1.player.stats.stepsTaken;
        }

        private void OnSaveLoad(object sender, EventArgs e)
        {
            // Should always true but is kept here just in case
            if (!Context.IsWorldReady)
                return;

            this.m_previousSteps = Game1.player.stats.stepsTaken;
        }

        /// <summary>
        /// On button release, check the amount of steps taken and increase EXP based on that
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonReleased(object sender,ButtonReleasedEventArgs e)
        {
            // Exit early if no world loaded
            if (!Context.IsWorldReady)
                return;

            // Exit early if button pressed was not a movement command
            if (e.IsSuppressed() || !this.isMovementButton(e.Button))
                return;

            // Calcuate difference in steps, and if it exceeds 1 exp treshold, add it as exp. Hacky fix to get xp values between 0 and 1
            uint step_diff = Game1.player.stats.stepsTaken - this.m_previousSteps;
            if (step_diff > ModConfig.StepsPerExp)
            {
                Game1.player.AddCustomSkillExperience(TravelSkill, 1);
                // Set previous steps to current steps, with correction
                this.m_previousSteps = Game1.player.stats.stepsTaken + ((uint) ModConfig.StepsPerExp - step_diff);
            }
            
        }

        /// <summary>
        /// When time changes in game, restore a little bit of stamina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeChanged(object sender, EventArgs e)
        {
            if (Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina))
            {
                Game1.player.stamina += Game1.player.MaxStamina * ModConfig.RestoreStaminaPercentage;
            }
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> is a movement control button
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns></returns>
        private bool isMovementButton(SButton button)
        {
            InputButton b_equiv;
            button.TryGetStardewInput(out b_equiv);
            return Game1.options.moveUpButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveDownButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveLeftButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveRightButton.Any(b => b.Equals(b_equiv));
        }

        private bool MovementButtonHeld()
        {
            return ButtonHeld(SButton.W) || ButtonHeld(SButton.S) || ButtonHeld(SButton.A) || ButtonHeld(SButton.D);
        }

        private bool ButtonHeld(SButton button)
        {
            SButtonState state = this.Helper.Input.GetState(button);
            return state == SButtonState.Held || state == SButtonState.Pressed;
        }

        /// <summary>
        /// <para>Counts consecutive steps and activates sprint while walking and having at least 3 consecutive steps.</para>
        /// <para>Should be called every tick</para>
        /// </summary>
        private void CheckSprintActive()
        {
            if (!this.MovementButtonHeld())
            {
                // "Reset" counter by setting it to current step count
                this.m_consecutiveSteps = Game1.player.stats.stepsTaken;
                ModEntry.SprintActive = false;
                return;
            }

            uint step_diff = Game1.player.stats.stepsTaken - this.m_consecutiveSteps;
            
            if (step_diff > ModConfig.SprintSteps && !ModEntry.SprintActive)
            {
                this.Monitor.Log("Now sprinting", LogLevel.Debug);
                ModEntry.SprintActive = true;
            }
        }

    }
}
