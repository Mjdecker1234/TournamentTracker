using TaleWorlds.Library;

// The data layer lives in the parent namespace.
using TournamentTracker;

namespace TournamentTracker.UI
{
    /// <summary>
    /// Root ViewModel for the Tournament Tracker screen.
    /// Holds the list of active tournament rows and controls which panel is visible.
    /// </summary>
    public sealed class TournamentTrackerVM : ViewModel
    {
        private readonly TournamentTrackerScreen _screen;

        private string _title             = "Tournament Tracker";
        private string _noTournamentsText = "No active tournaments found.";
        private bool   _hasTournaments;
        private bool   _hasNoTournaments = true;
        private MBBindingList<TournamentItemVM> _tournaments = new MBBindingList<TournamentItemVM>();

        public TournamentTrackerVM(TournamentTrackerScreen screen)
        {
            _screen = screen;
            Refresh();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Data-bound properties
        // ──────────────────────────────────────────────────────────────────────

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChangedWithValue(value, nameof(Title));
                }
            }
        }

        [DataSourceProperty]
        public string NoTournamentsText
        {
            get => _noTournamentsText;
            set
            {
                if (_noTournamentsText != value)
                {
                    _noTournamentsText = value;
                    OnPropertyChangedWithValue(value, nameof(NoTournamentsText));
                }
            }
        }

        /// <summary>True when at least one active tournament is found.</summary>
        [DataSourceProperty]
        public bool HasTournaments
        {
            get => _hasTournaments;
            set
            {
                if (_hasTournaments != value)
                {
                    _hasTournaments = value;
                    OnPropertyChangedWithValue(value, nameof(HasTournaments));
                }
            }
        }

        /// <summary>
        /// Inverse of <see cref="HasTournaments"/> — used to drive the empty-state
        /// message visibility without requiring negation syntax in the UI XML.
        /// </summary>
        [DataSourceProperty]
        public bool HasNoTournaments
        {
            get => _hasNoTournaments;
            set
            {
                if (_hasNoTournaments != value)
                {
                    _hasNoTournaments = value;
                    OnPropertyChangedWithValue(value, nameof(HasNoTournaments));
                }
            }
        }

        /// <summary>The list of active tournament rows, sorted alphabetically by town.</summary>
        [DataSourceProperty]
        public MBBindingList<TournamentItemVM> Tournaments
        {
            get => _tournaments;
            set
            {
                if (_tournaments != value)
                {
                    _tournaments = value;
                    OnPropertyChangedWithValue(value, nameof(Tournaments));
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Commands bound to UI buttons
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Re-fetches tournament data and rebuilds the list.</summary>
        public void ExecuteRefresh()
        {
            Refresh();
        }

        /// <summary>Closes the tracker screen and returns to the campaign map.</summary>
        public void ExecuteClose()
        {
            _screen.Close();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ──────────────────────────────────────────────────────────────────────

        private void Refresh()
        {
            _tournaments.Clear();

            var activeTournaments = TournamentTrackerBehavior.GetActiveTournaments();
            foreach (var entry in activeTournaments)
            {
                _tournaments.Add(new TournamentItemVM(entry.TownName, entry.PrizeName, entry.PrizeValue));
            }

            HasTournaments   = _tournaments.Count > 0;
            HasNoTournaments = !HasTournaments;
        }
    }
}
