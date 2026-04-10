using TaleWorlds.Library;

namespace TournamentTracker.UI
{
    /// <summary>
    /// ViewModel for a single row in the Tournament Tracker list.
    /// Exposes the town name, prize name, and a formatted prize-value string
    /// for display in the GauntletUI prefab.
    /// </summary>
    public sealed class TournamentItemVM : ViewModel
    {
        private string _townName     = string.Empty;
        private string _prizeName    = string.Empty;
        private string _prizeValue   = string.Empty;
        private string _distanceText = string.Empty;

        /// <summary>Raw prize value in gold — used for sorting.</summary>
        public int   RawPrizeValue { get; }

        /// <summary>Straight-line distance from the player — used for sorting.</summary>
        public float RawDistance   { get; }

        public TournamentItemVM(string townName, string prizeName, int prizeValue, float distance)
        {
            RawPrizeValue = prizeValue;
            RawDistance    = distance;

            TownName     = townName;
            PrizeName    = prizeName;
            PrizeValue   = $"{prizeValue:N0}";
            DistanceText = $"{distance:F1}";
        }

        [DataSourceProperty]
        public string TownName
        {
            get => _townName;
            set
            {
                if (_townName != value)
                {
                    _townName = value;
                    OnPropertyChangedWithValue(value, nameof(TownName));
                }
            }
        }

        [DataSourceProperty]
        public string PrizeName
        {
            get => _prizeName;
            set
            {
                if (_prizeName != value)
                {
                    _prizeName = value;
                    OnPropertyChangedWithValue(value, nameof(PrizeName));
                }
            }
        }

        [DataSourceProperty]
        public string PrizeValue
        {
            get => _prizeValue;
            set
            {
                if (_prizeValue != value)
                {
                    _prizeValue = value;
                    OnPropertyChangedWithValue(value, nameof(PrizeValue));
                }
            }
        }

        [DataSourceProperty]
        public string DistanceText
        {
            get => _distanceText;
            set
            {
                if (_distanceText != value)
                {
                    _distanceText = value;
                    OnPropertyChangedWithValue(value, nameof(DistanceText));
                }
            }
        }
    }
}
