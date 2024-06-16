using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

/// <summary>
/// Empty label that can be used to fill space and not draw anything.
/// </summary>
public class EmptyLabel : Label
{
    public override float Width => width;
    public override float Height => height;

    private float width;
    private float height;
    public EmptyLabel() : this(0, 0)
    {
    }
    public EmptyLabel(Vector2 size) : this((int)size.X, (int)size.Y)
    {
    }

    public EmptyLabel(float width, float height) : base(string.Empty)
    {
        this.width = width;
        this.height = height;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
    }
}
