﻿using AchtuurCore.Events;
using AchtuurCore.Patches;
using AchtuurCore.Utility;
using PrismaticStatue.Patches;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;
using MailFrameworkMod.Api;
using StardewValley.Quests;
using MailFrameworkMod;

namespace PrismaticStatue
{
    public class ModEntry : Mod
    {
        internal readonly string ContentPackPath = Path.Combine("assets", "ContentPack");
        internal readonly string PFMPath = Path.Combine("assets", "PFM");
        internal readonly string StatueName = "Prismatic Statue";

        public static ModEntry Instance;
        internal ModConfig Config;

        internal static bool PFMEnabled;
        internal IAutomateAPI AutomateAPI;
        internal JsonAssets.IApi JsonAssetsAPI;
        internal IMailFrameworkModApi mailFrameworkModApi;

        internal int SpeedupStatueID;

        internal List<SpedUpMachineGroup> SpedupMachineGroups;

        internal Overlay UIOverlay;
        
        internal void RemoveMachineGroup(int i)
        {
            this.SpedupMachineGroups[i].RestoreAllMachines();
            this.SpedupMachineGroups.RemoveAt(i);
        }
        internal void RemoveMachineGroup(SpedUpMachineGroup group)
        {
            group.RestoreAllMachines();
            this.SpedupMachineGroups.Remove(group);
        }

        internal SpedUpMachineWrapper GetMachineWrapperOnTile(SpedUpMachineGroup group, Vector2 tile)
        {
            return (group is null)
                ? null
                : group.Machines.Find(machine => machine.IsOnTile(tile));
        }
        internal SpedUpMachineWrapper GetMachineWrapperOnTile(Vector2 tile)
        {
            SpedUpMachineGroup group = GetMachineGroupOnTile(tile);
            return (group is null)
                ? null
                : group.Machines.Find(machine => machine.IsOnTile(tile));
        }

        internal SpedUpMachineGroup GetMachineGroupOnTile(Vector2 tile)
        {
            return SpedupMachineGroups.Find(group => group.ContainsTile(tile) && Game1.player.currentLocation == group.Location);
        }

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            HarmonyPatcher.ApplyPatches(this,
                new MachineGroupAutomatePatch()
            );

            SpedupMachineGroups = new List<SpedUpMachineGroup>();
            this.UIOverlay = new PrismaticStatue.Overlay();

            this.Config = this.Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // Remove machines groups that are empty or have a mismatch
            // This is for when the statue is used to connect two groups of machines. 
            // example: [m][s][c] (m = machine, s = statue, c = chest)
            // The machine in the example would not be restored if the statue breaks, since Automate() is not called on the group.
            for (int i = this.SpedupMachineGroups.Count - 1; i >= 0; i--)
            {
                if (this.SpedupMachineGroups[i].n_statues < 1 || !this.SpedupMachineGroups[i].TilesMatchNStatues())
                    this.RemoveMachineGroup(i);
            }
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            // Draw overlay
            this.UIOverlay.DrawOverlay(e.SpriteBatch);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Check if overlay button is pressed
            if (e.Button == this.Config.OverlayButton)
            {
                this.UIOverlay.Enabled = !this.UIOverlay.Enabled;
            }
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Tick each machine group
            foreach(SpedUpMachineGroup machinegroup in SpedupMachineGroups)
            {
                machinegroup.OnTenMinutesTick();
            }
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            // Create config menu
            this.Config.createMenu();

            // Get necessary API's from other mods
            AutomateAPI = this.Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            JsonAssetsAPI = this.Helper.ModRegistry.GetApi<JsonAssets.IApi>("spacechase0.JsonAssets");

            // Check for pfm, needed to select proper machine entity
            ModEntry.PFMEnabled = 
                this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod") &&
                this.Helper.ModRegistry.IsLoaded("Digus.PFMAutomate");

            // Load assets
            CreateRecipeUnlockMail();
            JsonAssetsAPI.LoadAssets(Path.Combine(Helper.DirectoryPath, this.ContentPackPath));

            
            // Add statue to automate factory
            AutomateAPI.AddFactory(new StatueFactory());
        }

        private void OnSaveLoad(object sender, EventArgs e)
        {
            // Get id here, as id is not available before save loads
            SpeedupStatueID = JsonAssetsAPI.GetBigCraftableId(StatueName);
        }

        private void CreateRecipeUnlockMail()
        {
            MailDao.SaveLetter(
                new Letter(
                    id: "Achtuur.PrismaticStatue.StatueRecipeMail",
                    text: "mail_statuerecipe.text",
                    recipe: null,
                    (l) => {
                        return !Game1.player.mailReceived.Contains(l.Id) &&
                        Game1.player.getFriendshipHeartLevelForNPC("Robin") >= 6 &&
                        Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 6 &&
                        Game1.player.hasCompletedCommunityCenter();
                    },
                    (l) => Game1.player.mailReceived.Add(l.Id)
                )
                {
                    Title = "mail_statuerecipe.title",
                    I18N = Helper.Translation
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    id: "Achtuur.PrismaticStatue.StatueUseMail",
                    text: "mail_statueuse.text",
                    recipe: null,
                    (l) => {
                        return !Game1.player.mailReceived.Contains(l.Id) &&
                        Game1.player.mailReceived.Contains("Achtuur.PrismaticStatue.StatueRecipeMail") &&
                        Game1.player.knowsRecipe(StatueName);
                    },
                    (l) => Game1.player.mailReceived.Add(l.Id),
                    whichBG: 2 // Wizard background
                )
                {
                    Title = "mail_statueuse.title",
                    I18N = Helper.Translation
                }
            );
        }
    }
}