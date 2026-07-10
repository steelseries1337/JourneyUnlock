using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;

namespace JourneyUnlocker.Services
{
    public static class ItemService
    {
        public static Item FindItem(TSPlayer sender, string nameOrId)
        {
            var items = TShock.Utils.GetItemByIdOrName(nameOrId);

            if (items.Count == 0)
            {
                sender.SendErrorMessage("Item not found.");
                sender.SendErrorMessage("Try specifying the item by name or ID.");
                return null;
            }

            if (items.Count > 1)
            {
                var itemNames = GetItemNames(items);
                sender.SendErrorMessage($"Multiple items found: {itemNames}");
                return null;
            }

            return items.First();
        }

        private static string GetItemNames(List<Item> items)
        {
            return string.Join(", ", items.Select(i => i.Name));
        }
    }
}
