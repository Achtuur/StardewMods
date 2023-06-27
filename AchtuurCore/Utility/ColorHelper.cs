using Microsoft.Xna.Framework;

namespace AchtuurCore.Utility;

public static class ColorHelper
{
    public static Color AddColor(this Color lhs, Color rhs)
    {
        return new Color(lhs.R + rhs.R, lhs.G + rhs.G, lhs.B + rhs.B, lhs.A + rhs.A);
    }

    public static Color ToGrayScale(this Color color)
    {
        int gs = (int)(0.299f * color.R) + (int)(0.587f * color.G) + (int)(0.114f * color.B);
        return new Color(gs, gs, gs, color.A);
    }
}
