using AchtuurCore;
using AchtuurCore.Events;
using AchtuurCore.Patches;
using SearchAround;
using SearchAround.Query;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace SearchAround
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal ModConfig Config;

        SearchOverlay m_SearchOverlay;

        SearchBar m_SearchBar;


        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            // HarmonyPatcher.ApplyPatches(this,
            
            // );

            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.m_SearchOverlay = new SearchOverlay();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;


            Logger.DebugLog(ModEntry.Instance.Monitor, $"{e.Button}");

            switch (e.Button)
            {
                case SButton.Space:
                    EnableSearchBar();
                    break;
                case SButton.Escape:
                    DisableSearchBar();
                    break;
                default:
                    break;
            }
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (m_SearchOverlay.Enabled)
                m_SearchOverlay.DrawOverlay(e.SpriteBatch);
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu();
        }

        private void EnableSearchBar()
        {
            if (m_SearchBar is not null)
                return;
            m_SearchBar = new SearchBar();
            m_SearchBar.SearchPressed += OnSearchPressed;
        }


        private void DisableSearchBar()
        {
            if (m_SearchBar is not null)
            {
                m_SearchBar.Disable();
                m_SearchBar.SearchPressed -= OnSearchPressed;
            }
            m_SearchBar = null;
        }
        private void OnSearchPressed(object sender, SearchQuery e)
        {
            DisableSearchBar();
            Logger.DebugLog(ModEntry.Instance.Monitor, $"searching for {e}...");
        }
    }
}
