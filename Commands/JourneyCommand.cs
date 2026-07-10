using System.Linq;
using TShockAPI;
using Microsoft.Xna.Framework;
using JourneyUnlocker.Services;

namespace JourneyUnlocker.Commands
{
    public static class JourneyCommand
    {
        public static void Execute(CommandArgs args)
        {
            var parameters = args.Parameters.Any() 
                ? args.Parameters.Select(p => p.ToLower()).ToArray() 
                : ["help"];

            var subCommand = parameters.First();

            switch (subCommand)
            {
                case "help":
                    ShowHelp(args.Player);
                    break;

                case "all":
                    HandleUnlockAll(args.Player, parameters);
                    break;

                case "unlock":
                    HandleUnlock(args.Player, parameters);
                    break;

                case "disable":
                    HandleDisable(args.Player);
                    break;

                case "enable":
                    HandleEnable(args.Player);
                    break;

                default:
                    ShowHelp(args.Player);
                    break;
            }
        }

        private static void HandleUnlockAll(TSPlayer player, string[] parameters)
        {
            if (!player.HasPermission(Permissions.All))
            {
                player.SendErrorMessage("You do not have permission to unlock items for other players!");
                return;
            }

            if (!player.RealPlayer)
            {
                player.SendErrorMessage("This command is only available to players");
                return;
            }

            JourneyService.UnlockAllItems(player);
            player.SendInfoMessage("You have unlocked all items!");

        }

        private static void HandleUnlock(TSPlayer player, string[] parameters)
        {
            if (!player.HasPermission(Permissions.Unlock))
            {
                player.SendErrorMessage("You do not have sufficient permission to use this command!");
                return;
            }
            UnlockForSelf(player, parameters);
        }

        private static void UnlockForSelf(TSPlayer player, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                player.SendErrorMessage($"Invalid syntax. Correct usage: {TShock.Config.Settings.CommandSpecifier}journey unlock <item>");
                return;
            }

            var item = ItemService.FindItem(player, parameters[1]);
            if (item == null)
                return;

            JourneyService.UnlockItem(player, item.type);
            player.SendInfoMessage($"Item [i/s{item.stack}:{item.type}] unlocked!"); //[i/s9998:2019]
        }

        private static void HandleDisable(TSPlayer player)
        {
            if (!player.HasPermission(Permissions.Disable))
            {
                player.SendErrorMessage("You do not have permission to disable Journey Mode.");
                return;
            }

            JourneyService.DisableJourneyMode(player);
            player.SendMessage("[i:2019] Journey Mode disabled", 240, 240, 240);
        }

        private static void HandleEnable(TSPlayer player)
        {
            if (!player.HasPermission(Permissions.Enable))
            {
                player.SendErrorMessage("You do not have sufficient permission to enable Journey Mode!");
                return;
            }

            JourneyService.EnableJourneyMode(player);
            player.SendMessage("[i:2890] Journey Mode enabled", 203, 179, 73);
            player.SendMessage("To unlock all items in the Journey Mode menu, use the /journey unlock all command.\n", 240, 255, 255);
            player.SendMessage("NOTE:", 217, 36, 36);
            player.SendMessage("While Journey Mode is enabled, the items on your character will not be saved", 240, 255, 255);
            player.SendMessage("All changes to your character will be reverted to the state it was in before this mode was enabled.", 240, 255, 255);
            //player.SendMessage("If you want to take an item and keep it after leaving the server, use the storage.", 240, 255, 255);
        }

        private static void ShowHelp(TSPlayer player)
        {
            var prefix = TShock.Config.Settings.CommandSpecifier;

            player.SendInfoMessage("Journey Mode management");
            player.SendInfoMessage("Available commands");
            player.SendMessage($"{prefix}journey unlock <item> - unlock an item", Color.LightGray);
            player.SendMessage($"{prefix}journey all - unlock all items", Color.LightGray);
            player.SendMessage($"{prefix}journey enable - enable Journey Mode", Color.LightGray);
            player.SendMessage($"{prefix}journey disable - disable Journey Mode", Color.LightGray);
        }
    }
}
