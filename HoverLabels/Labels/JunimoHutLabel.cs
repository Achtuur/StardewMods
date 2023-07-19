using HoverLabels.Framework;
using StardewValley.Buildings;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AchtuurCore.Extensions;
using Microsoft.Xna.Framework.Graphics;
using AchtuurCore.Utility;
using AchtuurCore.Framework;
using StardewValley.Objects;

namespace HoverLabels.Labels;
internal class JunimoHutLabel : BaseLabel
{
    JunimoHut hoverHut;
    public JunimoHutLabel(int? priority=null) : base(priority)
    {
    }

    protected override void ResetLabel()
    {
        base.ResetLabel();
        hoverHut = null;
    }
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        // Assume that greenhouse can only be on farm and outdoors
        if (!Game1.currentLocation.IsFarm || !Game1.currentLocation.IsOutdoors)
            return false;

        IEnumerable<JunimoHut> farmHuts = Game1.getFarm().buildings.Where(b => b is JunimoHut).Cast<JunimoHut>();
        return farmHuts.Any(hut => hut.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        this.CursorTile = cursorTile;
        this.hoverHut = Game1.getFarm().buildings
            .Where(b => b is JunimoHut && b.GetRect().Contains(cursorTile)).First() as JunimoHut;

    }

    public override void GenerateLabel()
    {
        this.Name = "Junimo Hut";

        Chest hutInventory = hoverHut.output.Value;
        IEnumerable<string> inventoryContents = ChestLabel.ListInventoryContents(hutInventory.items, ModEntry.IsShowDetailButtonPressed());
        this.Description = inventoryContents.ToList();

        string showAllMsg = ChestLabel.GetShowAllMessage(hutInventory.items);
        if (showAllMsg is not null)
            this.Description.Add(showAllMsg);

        if (!ModEntry.IsAlternativeSortButtonPressed() && inventoryContents.Count() > 1)
            this.Description.Add(I18n.LabelChestAltsort(ModEntry.Instance.Config.AlternativeSortButton.ToString()));

        this.Description.Add(I18n.LabelShowrange(ModEntry.Instance.Config.ShowDetailsButton.ToString()));
    }

    public override void DrawOnOverlay(SpriteBatch spriteBatch)
    {
        if (!ModEntry.IsShowDetailButtonPressed())
            return;
        
        IEnumerable<Vector2> hutRange = GetHutRangeRect(this.hoverHut).GetTiles();
        Overlay.DrawTiles(spriteBatch, hutRange, tileTexture: Overlay.GreenTilePlacementTexture);
        
    }

    private static Rectangle GetHutRangeRect(JunimoHut hut)
    {
        return new Rectangle(hut.tileX.Value - 7, hut.tileY.Value - 7, 17, 17);
    }
}
