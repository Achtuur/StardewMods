using AchtuurCore.Utility;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtuurCore.Framework;
public class DecayingText
{
    public string Text { get; set; }
    private int tickTimer;
    public int TickLifeSpan { get; set; }

    public bool LifeSpanOver { get; set; }

    public Color TextColor { get; set; }

    public DecayingText(string text, int tickLifeSpan, Color? color = null)
    {
        this.Text = text;
        this.TickLifeSpan = tickLifeSpan;
        this.tickTimer = 0;
        TextColor = color ?? Color.White;

        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        this.Tick();

        // destroy object when it should die
        if (this.tickTimer > this.TickLifeSpan)
        {
            ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
            LifeSpanOver = true;
        }
    }

    private void Tick()
    {
        this.tickTimer++;
        int a = Math.Max(0, 255 * (this.TickLifeSpan - this.tickTimer) / this.TickLifeSpan);
        TextColor = new Color(TextColor, a);
    }

    public void Destroy()
    {
        this.tickTimer = TickLifeSpan;
        TextColor = Color.Transparent;
    }

    public void DrawToScreen(SpriteBatch spriteBatch, Vector2 position, Color? color = null)
    {
        Color clr = new Color(color ?? this.TextColor, TextColor.A);
        spriteBatch.DrawString(Game1.dialogueFont, this.Text, position, clr);
    }
}
