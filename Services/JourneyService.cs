using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;
using TShockAPI;

namespace JourneyUnlocker.Services
{
    public static class JourneyService
    {
        private const int SpoofedReportingPlayerIndex = Main.maxPlayers;
        private const int MaxSacrificeCount = 9999;

        public static void UnlockAllItems(TSPlayer player)
        {
            int originalTeam = player.TPlayer.team;
            int originalReportingPlayerTeam = Main.player[SpoofedReportingPlayerIndex].team;

            try
            {
                PrepareUnlockReports(player);

                for (int itemId = 1; itemId < ItemID.Count; itemId++)
                {
                    SendUnlockReport(player, itemId);
                }
            }
            finally
            {
                RestoreTeams(player, originalTeam, originalReportingPlayerTeam);
            }
        }

        public static void UnlockItem(TSPlayer player, int itemId)
        {
            int originalTeam = player.TPlayer.team;
            int originalReportingPlayerTeam = Main.player[SpoofedReportingPlayerIndex].team;

            try
            {
                PrepareUnlockReports(player);
                SendUnlockReport(player, itemId);
            }
            finally
            {
                RestoreTeams(player, originalTeam, originalReportingPlayerTeam);
            }
        }

        private static void PrepareUnlockReports(TSPlayer player)
        {
            player.TPlayer.team = 1;
            Main.player[SpoofedReportingPlayerIndex].team = 1;

            player.SendData(PacketTypes.PlayerTeam, "", player.Index);
            player.SendData(PacketTypes.PlayerTeam, "", SpoofedReportingPlayerIndex);
        }

        private static void SendUnlockReport(TSPlayer player, int itemId)
        {
            NetManager.Instance.SendToClient(
                NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(
                    SpoofedReportingPlayerIndex,
                    itemId,
                    MaxSacrificeCount
                ),
                player.Index
            );
        }

        private static void RestoreTeams(TSPlayer player, int originalTeam, int originalReportingPlayerTeam)
        {
            player.TPlayer.team = originalTeam;
            Main.player[SpoofedReportingPlayerIndex].team = originalReportingPlayerTeam;

            player.SendData(PacketTypes.PlayerTeam, "", player.Index);
            player.SendData(PacketTypes.PlayerTeam, "", SpoofedReportingPlayerIndex);
        }

        public static void EnableJourneyMode(TSPlayer player)
        {
            ServerSideCharacterService.EnableSSC(player);
            player.TPlayer.difficulty = 3;
            player.SendData(PacketTypes.PlayerInfo, "", player.Index);
        }

        public static void DisableJourneyMode(TSPlayer player)
        {
            ServerSideCharacterService.EnableSSC(player);
            player.TPlayer.difficulty = 0;
            player.SendData(PacketTypes.PlayerInfo, "", player.Index);
        }
    }
}
