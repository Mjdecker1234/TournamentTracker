using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;
using TournamentTracker.UI;

namespace TournamentTracker
{
    /// <summary>
    /// Mod entry point. Registers the campaign behavior and handles the Ctrl+T hotkey
    /// to open the Tournament Tracker screen while on the campaign map.
    /// </summary>
    public sealed class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (game.GameType is Campaign && gameStarter is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new TournamentTrackerBehavior());
            }
        }

        /// <summary>
        /// Called every application frame. Checks for the Ctrl+T hotkey while the campaign
        /// map is the active screen and opens the Tournament Tracker if pressed.
        /// </summary>
        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            if (Campaign.Current == null)
                return;

            // Only activate on the campaign map screen.
            // Use Contains to handle both "MapScreen" and derived types.
            string? topScreenName = ScreenManager.TopScreen?.GetType().Name;
            if (topScreenName == null || !topScreenName.Contains("MapScreen"))
                return;

            bool ctrlHeld = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
            if (ctrlHeld && Input.IsKeyPressed(InputKey.T))
            {
                ScreenManager.PushScreen(new TournamentTrackerScreen());
            }
        }
    }
}
