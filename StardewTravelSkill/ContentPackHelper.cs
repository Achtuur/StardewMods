﻿using SpaceCore;
using StardewModdingAPI;
using StardewTravelSkill.Integrations;
using StardewValley;
using System.Collections.Generic;

namespace StardewTravelSkill;

internal class ContentPackHelper
{
    public ModEntry Instance;
    public static IContentPatcherAPI ContentPatcherAPI;

    public ContentPackHelper(ModEntry instance)
    {
        this.Instance = instance;
        ContentPackHelper.ContentPatcherAPI = this.Instance.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
    }

    public void CreateTokens()
    {
        if (ContentPatcherAPI == null)
            return;


        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionMovespeed", hasMovespeed);
        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionRestoreStamina", hasRestoreStamina);
        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionSprint", hasSprint);

        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionCheaperWarpTotems", hasCheaperWarpTotem);
        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionCheaperObelisks", hasCheaperObelisk);
        ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasProfessionTotemReuse", hasTotemReuse);
    }

    private IEnumerable<string> hasMovespeed()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionMovespeed).ToString() };
    }

    private IEnumerable<string> hasRestoreStamina()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina).ToString() };
    }

    private IEnumerable<string> hasSprint()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionSprint).ToString() };
    }

    private IEnumerable<string> hasCheaperWarpTotem()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem).ToString() };
    }

    private IEnumerable<string> hasTotemReuse()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk).ToString() };
    }

    private IEnumerable<string> hasCheaperObelisk()
    {
        if (!Context.IsWorldReady)
            return null;
        return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionTotemReuse).ToString() };
    }

}
