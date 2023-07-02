using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterPlanting.Extensions
{
    internal static class FarmerExtensions
    {
        public static bool IsHoldingCategory(this Farmer farmer, int category)
        {
            return farmer.CurrentItem is not null && farmer.CurrentItem.Category == category;
        }
    }
}
