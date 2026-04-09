using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace TournamentTracker.UI
{
    /// <summary>
    /// A full-screen overlay that renders the Tournament Tracker UI on top of the
    /// campaign map. Pushed onto the screen stack by <see cref="SubModule"/> when
    /// the player presses Ctrl+T on the campaign map.
    /// </summary>
    public sealed class TournamentTrackerScreen : ScreenBase
    {
        private GauntletLayer?         _layer;
        private TournamentTrackerVM?   _viewModel;
        private IGauntletMovie?        _movie;

        // ──────────────────────────────────────────────────────────────────────
        // Screen lifecycle
        // ──────────────────────────────────────────────────────────────────────

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _viewModel = new TournamentTrackerVM(this);

            _layer = new GauntletLayer(500);
            _movie = _layer.LoadMovie("TournamentTracker", _viewModel);

            // Capture mouse and keyboard so clicks reach the overlay.
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);

            AddLayer(_layer);
        }

        protected override void OnFinalize()
        {
            if (_layer != null)
            {
                _layer.InputRestrictions.ResetInputRestrictions();

                if (_movie != null)
                {
                    _layer.ReleaseMovie(_movie);
                    _movie = null;
                }

                RemoveLayer(_layer);
                _layer = null;
            }

            _viewModel?.OnFinalize();
            _viewModel = null;

            base.OnFinalize();
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);

            // Allow closing with the Escape key in addition to the Close button.
            if (_layer != null && _layer.Input.IsKeyReleased(InputKey.Escape))
            {
                Close();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Public API (called by the ViewModel)
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Pops this screen from the stack, returning to the campaign map.</summary>
        public void Close()
        {
            ScreenManager.PopScreen();
        }
    }
}
