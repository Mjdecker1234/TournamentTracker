using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

// The data layer lives in the parent namespace.
using TournamentTracker;

namespace TournamentTracker.UI
{
    public enum SortMode
    {
        Name,
        Distance,
        PrizeValue
    }

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
        private string _sortLabel         = "Sort: Name";
        private SortMode _currentSort     = SortMode.Name;
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

        /// <summary>The list of active tournament rows.</summary>
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

        [DataSourceProperty]
        public string SortLabel
        {
            get => _sortLabel;
            set
            {
                if (_sortLabel != value)
                {
                    _sortLabel = value;
                    OnPropertyChangedWithValue(value, nameof(SortLabel));
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

        /// <summary>Cycles through sort modes: Name → Distance → Prize Value → Name …</summary>
        public void ExecuteCycleSort()
        {
            switch (_currentSort)
            {
                case SortMode.Name:
                    _currentSort = SortMode.Distance;
                    SortLabel = "Sort: Distance";
                    break;
                case SortMode.Distance:
                    _currentSort = SortMode.PrizeValue;
                    SortLabel = "Sort: Value";
                    break;
                default:
                    _currentSort = SortMode.Name;
                    SortLabel = "Sort: Name";
                    break;
            }
            Refresh();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ──────────────────────────────────────────────────────────────────────

        private void Refresh()
        {
            _tournaments.Clear();

            var activeTournaments = TournamentTrackerBehavior.GetActiveTournaments();

            switch (_currentSort)
            {
                case SortMode.Distance:
                    activeTournaments.Sort((a, b) => a.Distance.CompareTo(b.Distance));
                    break;
                case SortMode.PrizeValue:
                    activeTournaments.Sort((a, b) => b.PrizeValue.CompareTo(a.PrizeValue));
                    break;
                default:
                    activeTournaments.Sort((a, b) => string.Compare(a.TownName, b.TownName, StringComparison.OrdinalIgnoreCase));
                    break;
            }

            foreach (var entry in activeTournaments)
            {
                _tournaments.Add(new TournamentItemVM(entry.TownName, entry.PrizeName, entry.PrizeValue, entry.Distance));
            }

            HasTournaments   = _tournaments.Count > 0;
            HasNoTournaments = !HasTournaments;
        }
    }
}
