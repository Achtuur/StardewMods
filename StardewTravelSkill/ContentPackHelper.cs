using ContentPatcher;
using SpaceCore;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewTravelSkill
{
    internal class ContentPackHelper
    {
        public ModEntry Instance;
        public static IContentPatcherAPI ContentPatcherAPI;

        public ContentPackHelper(ModEntry instance)
        {
            this.Instance = instance;
            ContentPackHelper.ContentPatcherAPI = this.Instance.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
        }

        public void CreateTokens()
        {
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasCheaperWarpTotems", hasCheaperWarpTotem);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "hasCheaperObelisks", hasCheaperObelisk);
            ContentPatcherAPI.RegisterToken(this.Instance.ModManifest, "canCraftScepter", canCraftScepter);
        }


        private IEnumerable<string> hasCheaperWarpTotem()
        {
            if (!Context.IsWorldReady)
                return null;
            return new[] { Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem).ToString() };
        }

        private IEnumerable<string> canCraftScepter()
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
}
