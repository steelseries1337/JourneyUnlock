using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace JourneyUnlocker.Services
{
    public static class PlayerService
    {
        public static TSPlayer FindPlayer(TSPlayer sender, string nameOrId)
        {
            var players = TSPlayer.FindByNameOrID(nameOrId);

            if (players.Count == 0)
            {
                sender.SendErrorMessage("Player not found.");
                return null;
            }

            if (players.Count > 1)
            {
                var playerNames = GetPlayerNames(players);
                sender.SendErrorMessage($"Multiple players found: {playerNames}");
                return null;
            }

            return players.First();
        }

        private static string GetPlayerNames(List<TSPlayer> players)
        {
            return string.Join(", ", players.Select(p => p.Name));
        }
    }
}
