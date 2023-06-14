using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace Wrench
{
    internal class WrenchTool : Tool
    {
        /// <summary>
        /// Just to the right of the Iridium trash can
        /// </summary>
        public const int MaxUses = 5;

        private int usesLeft;

        public WrenchTool() : base("Wrench", -1, 0, 0, false)
        {
            this.ResetUsesLeft();
        }

        public void ResetUsesLeft()
        {
            this.usesLeft = MaxUses;
        }

        /// <summary>
        /// Copied straight from Shears implementation
        /// </summary>
        /// <returns></returns>
        public override Item getOne()
        {
            WrenchTool destination = new WrenchTool();
            this.CopyEnchantments((Tool)this, (Tool)destination);
            destination._GetOneFrom((Item)this);
            return (Item)destination;
        }
        protected override string loadDisplayName()
        {
            return I18n.Wrenchtool_Name();
        }

        protected override string loadDescription()
        {
            return I18n.Wrenchtool_Desc();
        }


        //// dont need this method??
        //public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        //{
        //    x = (int)who.GetToolLocation().X;
        //    y = (int)who.GetToolLocation().Y;
        //    Rectangle toolRect = new Rectangle(x - 32, y - 32, 64, 64); //target?


        //    // if location has valid machine

        //    switch (location)
        //    {
        //        case Farm _:
        //            this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>)(location as Farm).animals.Values, (Tool)this, toolRect);
        //            break;
        //        case AnimalHouse _:
        //            this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>)(location as AnimalHouse).animals.Values, (Tool)this, toolRect);
        //            break;
        //    }
        //    who.Halt();
        //    int currentFrame = who.FarmerSprite.CurrentFrame;
        //    who.FarmerSprite.animateOnce(283 + who.FacingDirection, 50f, 4);
        //    who.FarmerSprite.oldFrame = currentFrame;
        //    who.UsingTool = true;
        //    who.CanMove = false;
        //    return true;
        //}

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            base.DoFunction(location, x, y, power, who);

            who.stamina -= 5;
            // play sound

            // make affected machine tick faster for the rest of the day
            // reduce tool usage
            // display mesasge with amount of uses left this day
        }
    }
}
