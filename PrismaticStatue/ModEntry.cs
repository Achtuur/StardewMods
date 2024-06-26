﻿using AchtuurCore;
using AchtuurCore.Patches;
using MailFrameworkMod;
using MailFrameworkMod.Api;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using PrismaticStatue.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

namespace PrismaticStatue;

public class ModEntry : Mod
{
    internal int AnimationTickCooldown = 10;
    internal const int AnimationFrames = 8;

    internal readonly string ContentPackPath = Path.Combine("assets", "ContentPack");
    internal readonly string PFMPath = Path.Combine("assets", "PFM");
    internal readonly string StatueName = "Prismatic Statue";


    internal static ModEntry Instance;
    internal ModConfig Config;

    internal static bool PFMEnabled;
    internal IAutomateAPI AutomateAPI;
    internal JsonAssets.IApi JsonAssetsAPI;
    internal IMailFrameworkModApi mailFrameworkModApi;

    internal List<SpedUpMachineGroup> SpedupMachineGroups;
    internal int secondUpdateCounter;

    internal StatueOverlay UIOverlay;

    internal int animationTickCounter = 0;
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

    internal GenericSpedUpMachineWrapper GetMachineWrapperOnTile(SpedUpMachineGroup group, Vector2 tile)
    {
        return (group is null)
            ? null
            : group.Machines.Find(machine => machine.IsOnTile(tile));
    }
    internal GenericSpedUpMachineWrapper GetMachineWrapperOnTile(Vector2 tile)
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
            //new PerformToolActionPatch(),
            new MachineGroupAutomatePatch()
        );

        SpedupMachineGroups = new List<SpedUpMachineGroup>();
        this.UIOverlay = new PrismaticStatue.StatueOverlay();
        this.Config = this.Helper.ReadConfig<ModConfig>();

        this.secondUpdateCounter = 0;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        //helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;

        // Animation stuff
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        foreach(SpedUpMachineGroup group in this.SpedupMachineGroups)
        {
            group.RestoreAllMachines();
        }
    }

    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        AnimationTickCooldown = 10;

        if (this.animationTickCounter++ >= AnimationTickCooldown)
        {
            UpdateAnimationFrame();
            animationTickCounter = 0;
        }
    }


    // Animation currently broke due to JA changes
    private void UpdateAnimationFrame()
    {

        //if (!Context.IsWorldReady || SpeedupStatue.ID is null)
        //    return;

        //IEnumerable<SObject> prismatic_statues = Game1.currentLocation.objects.Values
        //    .Where(sobj => sobj.QualifiedItemId == SpeedupStatue.ID);

        //foreach (SObject sobj in prismatic_statues)
        //{
        //    sobj.ParentSheetIndex++;
        //    if (sobj.ParentSheetIndex >= ModEntry.AnimationFrames)
        //        sobj.ParentSheetIndex = 0;
        //}
    }

    private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
    {
        // Only check once every 2 seconds for slightly better performance
        if (this.secondUpdateCounter++ < 2)
            return;

        // Remove machines groups that are empty or have a mismatch
        // This is for when the statue is used to connect two groups of machines. 
        // example: [m][s][c] (m = machine, s = statue, c = chest)
        // The machine in the example would not be restored if the statue breaks, since that will break the IMachineGroup, leaving this mod's groups unupdated as well
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
        if (e.Button == this.Config.OverlayButton && Context.IsPlayerFree)
        {
            this.UIOverlay.Enabled = !this.UIOverlay.Enabled;
        }
    }

    private void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
        // Tick each machine group
        foreach (SpedUpMachineGroup machinegroup in SpedupMachineGroups)
        {
            machinegroup.OnTimeChanged();
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
        string unqualified_id = JsonAssetsAPI.GetBigCraftableId(StatueName);
        SpeedupStatue.ID = ItemRegistry.Create<SObject>(unqualified_id).QualifiedItemId;
        Logger.DebugLog(ModEntry.Instance.Monitor, $"{SpeedupStatue.ID}");
    }

    private void OnDayStarted(object sender, EventArgs e)
    {
        foreach(SpedUpMachineGroup group in SpedupMachineGroups)
        {
            group.OnDayStarted();
        }
    }

    private void CreateRecipeUnlockMail()
    {
        //MailDao.SaveLetter(
        //    new Letter(
        //        id: "Achtuur.PrismaticStatue.StatueRecipeMail",
        //        text: "mail_statuerecipe.text",
        //        recipe: null,
        //        (l) =>
        //        {
        //            return !Game1.player.mailReceived.Contains(l.Id) &&
        //            Game1.player.getFriendshipHeartLevelForNPC("Robin") >= 6 &&
        //            Game1.player.getFriendshipHeartLevelForNPC("Wizard") >= 6 &&
        //            (Game1.player.hasCompletedCommunityCenter() ||
        //            false/*completed jojamart */);
        //        },
        //        (l) => Game1.player.mailReceived.Add(l.Id)
        //    )
        //    {
        //        Title = "mail_statuerecipe.title",
        //        I18N = Helper.Translation
        //    }
        //);
        //MailDao.SaveLetter(
        //    new Letter(
        //        id: "Achtuur.PrismaticStatue.StatueUseMail",
        //        text: "mail_statueuse.text",
        //        recipe: null,
        //        (l) =>
        //        {
        //            return !Game1.player.mailReceived.Contains(l.Id) &&
        //            Game1.player.mailReceived.Contains("Achtuur.PrismaticStatue.StatueRecipeMail") &&
        //            Game1.player.knowsRecipe(StatueName);
        //        },
        //        (l) => Game1.player.mailReceived.Add(l.Id),
        //        whichBG: 2 // Wizard background
        //    )
        //    {
        //        Title = "mail_statueuse.title",
        //        I18N = Helper.Translation
        //    }
        //);
    }
}
