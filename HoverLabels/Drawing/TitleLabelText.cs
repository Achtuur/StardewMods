using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Drawing;
internal class TitleLabelText : LabelText
{
    public TitleLabelText(string text) : base(text)
    {
        this.Font = Game1.dialogueFont;
    }
}
