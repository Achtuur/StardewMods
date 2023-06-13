using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Utility
{
    public static class ColorHelper
    {
        public static Color AddColor(this Color lhs, Color rhs)
        {
            return new Color(lhs.R + rhs.R, lhs.G + rhs.G, lhs.B + rhs.B, lhs.A + rhs.A);
        }
    }
}
