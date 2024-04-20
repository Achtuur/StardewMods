using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

public class TitleLabelText : Label
{
    public TitleLabelText(string text) : base(text)
    {
        this.Font = Game1.dialogueFont;
    }
}
