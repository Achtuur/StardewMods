using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace AchtuurCore.Utility;
public static class VectorHelper
{
    public static Vector2 Normalize(Vector2 vec)
    {
        vec.Normalize();
        return vec;
    }

    /// <summary>
    /// Returns vector that points either North, South, East or West depending on direction of <paramref name="vec"/>
    /// <para>
    /// Example:
    /// <c>new Vector2(0.4, -0.7).toCardinal() // returns Vector2(0, -1)</c>
    /// </para>
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 toCardinal(this Vector2 vec)
    {
        if (vec.LengthSquared() < 0.000001f)
            return Vector2.Zero;

        vec.Normalize();
        return new Vector2((float)Math.Round(vec.X), (float)Math.Round(vec.Y));
    }

    public static Vector2 GetFaceDirectionUnitVector(int FaceDirection)
    {
        switch (FaceDirection)
        {
            case 0: // North
                return new Vector2(0, -1);
            case 1: // East
                return new Vector2(1, 0);
            case 2: // South
                return new Vector2(0, 1);
            case 3: // West
                return new Vector2(-1, 0);
            default: //default to south
                return new Vector2(0, 1); 
        }
    }
}
