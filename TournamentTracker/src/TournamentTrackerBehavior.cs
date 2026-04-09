using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;

namespace TournamentTracker
{
    /// <summary>
    /// Campaign behavior that provides safe access to active tournament data.
    /// Registered by <see cref="SubModule"/> so it is present for every campaign session.
    /// </summary>
    public sealed class TournamentTrackerBehavior : CampaignBehaviorBase
    {
        // No campaign events to subscribe to; data is read on demand.
        public override void RegisterEvents() { }

        // No persistent state to save or load.
        public override void SyncData(IDataStore dataStore) { }

        /// <summary>
        /// Returns a sorted (alphabetical by town name) list of all towns that currently
        /// have an active tournament, together with their prize details.
        /// Returns an empty list if the campaign is not active or no tournaments exist.
        /// Never throws — all exceptions are swallowed to keep the mod from crashing
        /// the host game.
        /// </summary>
        public static List<TournamentEntryData> GetActiveTournaments()
        {
            var result = new List<TournamentEntryData>();

            try
            {
                var manager = Campaign.Current?.TournamentManager;
                if (manager == null)
                    return result;

                foreach (Settlement settlement in Settlement.All)
                {
                    if (!settlement.IsTown)
                        continue;

                    Town? town = settlement.Town;
                    if (town == null)
                        continue;

                    TournamentGame? tournament = manager.GetTournamentGame(town);
                    if (tournament == null)
                        continue;

                    string townName  = town.Name?.ToString() ?? "Unknown Town";
                    string prizeName = tournament.Prize?.Name?.ToString() ?? "Unknown Prize";
                    int    prizeVal  = tournament.Prize?.Value ?? 0;

                    result.Add(new TournamentEntryData(townName, prizeName, prizeVal));
                }
            }
            catch (Exception)
            {
                // Intentionally silent — the mod must never crash the host game.
            }

            result.Sort(static (a, b) =>
                string.Compare(a.TownName, b.TownName, StringComparison.OrdinalIgnoreCase));

            return result;
        }
    }

    /// <summary>Plain-data record for a single active tournament.</summary>
    public sealed class TournamentEntryData
    {
        public string TownName  { get; }
        public string PrizeName { get; }
        public int    PrizeValue { get; }

        public TournamentEntryData(string townName, string prizeName, int prizeValue)
        {
            TownName   = townName;
            PrizeName  = prizeName;
            PrizeValue = prizeValue;
        }
    }
}
