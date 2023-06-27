﻿using System;

namespace AchtuurCore.Utility;

public class SliderRange
{
    public readonly float min;
    public readonly float max;
    public readonly float interval;

    public SliderRange(float min, float max, float interval)
    {
        this.min = min;
        this.max = max;
        this.interval = interval;
    }

    /// <summary>
    /// Get value in the middle of the range, equal to <c>(max + min) / 2</c>
    /// </summary>
    /// <param name="digits">Round to this many digits, default = 2</param>
    /// <returns></returns>
    public float GetMiddle(int digits = 2)
    {
        return (float)Math.Round((this.max + this.min) / 2f, digits);
    }
}
