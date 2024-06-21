using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAround.Utility;
/// <summary>
/// A wrapper class for <see cref="IKeyboardSubscriber"/>, taken from MindMeltMax's SAML repository.
/// <see href="https://github.com/MindMeltMax/SAML/blob/main/SAML/Utilities/DummyKeyboardSubscriber.cs"/>
/// </summary>
/// 
/// 
/// <remarks>
/// Use this for textbox or chatbox will open and force focus
/// </remarks>
internal class DummyKeyboardSubscriber : IKeyboardSubscriber
{
    public bool Selected
    {
        get => true;
        set { }
    }

    public void RecieveCommandInput(char command)
    {
    }

    public void RecieveSpecialInput(Keys key)
    {
    }

    public void RecieveTextInput(char inputChar)
    {
    }

    public void RecieveTextInput(string text)
    {
    }
}
