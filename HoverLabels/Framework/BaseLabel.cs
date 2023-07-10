using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace HoverLabels.Framework;
internal abstract class BaseLabel
{
    protected readonly string NewLineDelimiter = "\n\n";

    protected Vector2 CursorTile { get; set; }
    protected string Name { get; set; }
    protected List<string> Description { get; set; }

    public BaseLabel(Vector2 cursorTile)
    {
        this.CursorTile = cursorTile;
        this.Name = string.Empty;
        this.Description = new List<string>();
        this.GenerateLabel();
    }

    /// <summary>
    /// Generates objectname and description, is called in the constructor
    /// </summary>
    protected abstract void GenerateLabel();

    public bool HasDescription()
    {
        return Description.Count > 0;
    }

    public string GetObjectName()
    {
        return this.Name;
    }

    public string GetDescriptionAsString()
    {
        return String.Join(NewLineDelimiter, Description.ToArray());
    }

    public string GetLabelString()
    {
        return GetObjectName() + (this.HasDescription() ? this.NewLineDelimiter + this.GetDescriptionAsString() : "");
    }

    public Vector2 GetLabelSize(SpriteFont font)
    {
        return font.MeasureString(this.GetLabelString());
    }

    public Vector2 GetNameSize(SpriteFont font)
    {
        return font.MeasureString(this.Name);
    }

    public Vector2 GetDescriptionSize(SpriteFont font)
    {
        if (this.HasDescription())
            return font.MeasureString(this.GetDescriptionAsString());
        return Vector2.Zero;
    }

}
