using AchtuurCore.Events;
using StardewModdingAPI;
using StardewValley;
using System;

namespace MultiplayerExpShare
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;

        public static Farmer[] GetNearbyPlayers() {
            // return all players that are close to the main player
        }

        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            I18n.Init(helper.Translation);

        }

    }
}
