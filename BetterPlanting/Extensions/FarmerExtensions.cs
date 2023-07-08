using StardewValley;

namespace BetterPlanting.Extensions;

internal static class FarmerExtensions
{
    public static bool IsHoldingCategory(this Farmer farmer, int category)
    {
        return farmer.CurrentItem is not null && farmer.CurrentItem.Category == category;
    }
}
