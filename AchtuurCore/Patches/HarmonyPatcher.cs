﻿using HarmonyLib;
using StardewModdingAPI;
using System;

namespace AchtuurCore.Patches;

/// <summary>
/// Can take in a number of GenericPatches and apply them using Harmony
/// </summary>
public static class HarmonyPatcher
{
    public static void ApplyPatches(Mod instance, params GenericPatcher[] patches)
    {
        Harmony harmony = new Harmony(instance.ModManifest.UniqueID);
        foreach (GenericPatcher patch in patches)
        {
            try
            {
                patch.Patch(harmony);
            }
            catch (Exception e)
            {
                Logger.ErrorLog(ModEntry.Instance.Monitor, $"Applying patch {patch} failed:\n{e}");
            }
        }
    }
}
