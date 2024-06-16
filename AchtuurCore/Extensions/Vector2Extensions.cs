using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Extensions;
public static class Vector2Extensions
{
    public static float Angle(this Vector2 v)
    {
        return (float)Math.Atan2(v.Y, v.X);
    }
}
