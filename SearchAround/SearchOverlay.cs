using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAround;
internal class SearchOverlay : AchtuurCore.Framework.Overlay
{
    public SearchOverlay()
    {
    }


    public override void Enable()
    {
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        //SearchBar.Draw(spriteBatch);   
    }
}
