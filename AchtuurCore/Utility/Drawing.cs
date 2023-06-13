using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Utility
{
    public static class Drawing
    {
        /// <summary>
        /// <para>A blank pixel which can be colorized and stretched to draw geometric shapes.</para>
        /// 
        /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/CommonHelper.cs#L27"
        /// </summary>
        private static readonly Lazy<Texture2D> LazyPixel = new(() =>
        {
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            return pixel;
        });
        public static Texture2D Pixel => Drawing.LazyPixel.Value;
        /****
        ** Drawing 
        ****/
        /// <summary>Draw a sprite to the screen. (<see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/CommonHelper.cs#L370"/>)</summary>
        /// <param name="batch">The sprite batch.</param>
        /// <param name="x">The X-position at which to start the line.</param>
        /// <param name="y">The X-position at which to start the line.</param>
        /// <param name="size">The line dimensions.</param>
        /// <param name="color">The color to tint the sprite.</param>
        public static void DrawLine(this SpriteBatch batch, float x, float y, in Vector2 size, in Color? color = null)
        {
            batch.Draw(Drawing.Pixel, new Rectangle((int)x, (int)y, (int)size.X, (int)size.Y), color ?? Color.White);
        }
    }
}
