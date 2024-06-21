using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAround.Extensions;
public static class SbuttonExtensions
{
    public static bool IsValidTextInput(this SButton button)
    {
        switch (button)
        {
            case SButton.A:
            case SButton.B:
            case SButton.C:
            case SButton.D:
            case SButton.E:
            case SButton.F:
            case SButton.G:
            case SButton.H:
            case SButton.I:
            case SButton.J:
            case SButton.K:
            case SButton.L:
            case SButton.M:
            case SButton.N:
            case SButton.O:
            case SButton.P:
            case SButton.Q:
            case SButton.R:
            case SButton.S:
            case SButton.T:
            case SButton.U:
            case SButton.V:
            case SButton.W:
            case SButton.X:
            case SButton.Y:
            case SButton.Z:
            case SButton.D0:
            case SButton.D1:
            case SButton.D2:
            case SButton.D3:
            case SButton.D4:
            case SButton.D5:
            case SButton.D6:
            case SButton.D7:
            case SButton.D8:
            case SButton.D9:
            case SButton.NumPad0:
            case SButton.NumPad1:
            case SButton.NumPad2:
            case SButton.NumPad3:
            case SButton.NumPad4:
            case SButton.NumPad5:
            case SButton.NumPad6:
            case SButton.NumPad7:
            case SButton.NumPad8:
            case SButton.NumPad9:
            case SButton.Space:
                return true;
            default: return false;
        }
    }

    public static string Format(this SButton button)
    {
        switch (button) 
        {
            case SButton.D0:
            case SButton.D1:
            case SButton.D2:
            case SButton.D3:
            case SButton.D4:
            case SButton.D5:
            case SButton.D6:
            case SButton.D7:
            case SButton.D8:
            case SButton.D9: return button.ToString().Substring(1);
            case SButton.NumPad0:
            case SButton.NumPad1:
            case SButton.NumPad2:
            case SButton.NumPad3:
            case SButton.NumPad4:
            case SButton.NumPad5:
            case SButton.NumPad6:
            case SButton.NumPad7:
            case SButton.NumPad8:
            case SButton.NumPad9: return button.ToString().Substring(6);
            case SButton.Space: return " ";
            default: return button.ToString().ToLowerInvariant();
        }
    }
}
