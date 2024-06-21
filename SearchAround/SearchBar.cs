using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SearchAround.Extensions;
using SearchAround.Query;
using SearchAround.Utility;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAround;
internal class SearchBar : IClickableMenu
{
    public static int SearchBarPosX = 300;
    public static int SearchBarPosY = 400;

    public static int SearchBarWidth = 800;
    public static int SearchBarHeight = 200;
    public static int MaxSearchResults = 10;

    string m_SearchQuery = "";
    Border m_QueryBorder;
    BorderDrawer borderDrawer= new BorderDrawer();

    IClickableMenu m_PreviousMenu;
    DummyKeyboardSubscriber m_KeyboardSubscriber = new();

    public EventHandler<SearchQuery> SearchPressed;

    public SearchBar()
    {
        m_PreviousMenu = Game1.activeClickableMenu;
        Game1.activeClickableMenu = this;
        Game1.keyboardDispatcher.Subscriber = m_KeyboardSubscriber;
        Update();
    }

    public void Disable() {
        Game1.keyboardDispatcher.Subscriber = null;
        Game1.activeClickableMenu = m_PreviousMenu;
        m_SearchQuery = "";
    }

    public override void receiveKeyPress(Keys key)
    {
        //base.receiveKeyPress(key);
        SButton button = key.ToSButton();
        if (button.IsValidTextInput())
            m_SearchQuery += button.Format();
        else if (button == SButton.Back)
            m_SearchQuery = m_SearchQuery[..^1];
        else if (button == SButton.Enter)
            Search();

        Update();
    }

    private void Search()
    {
        SearchQuery query = new SearchQuery(m_SearchQuery);
        SearchPressed?.Invoke(this, query);
        this.Disable();
    }

    private void Update()
    {
        UpdateDimensions();
        UpdateBorders();
    }

    private void UpdateBorders()
    {
        string query_with_cursor = m_SearchQuery + "|";
        m_QueryBorder = new Border(new TitleLabel(query_with_cursor));
        borderDrawer.SetBorder(m_QueryBorder);
    }

    private void UpdateDimensions()
    {
        this.xPositionOnScreen = SearchBarPosX;
        this.yPositionOnScreen = SearchBarPosY;
        this.width = SearchBarWidth;
        this.height = SearchBarHeight;
    }

    public override void draw(SpriteBatch b)
    {
        base.draw(b);
        Vector2 offset = new Vector2(SearchBarPosX, SearchBarPosY);
        borderDrawer.Draw(b, offset, fixed_width: SearchBarWidth, fixed_height: SearchBarHeight);
    }
}
