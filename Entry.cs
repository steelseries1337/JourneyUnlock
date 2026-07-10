using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using JourneyUnlocker.Commands;

namespace JourneyUnlocker
{
    [ApiVersion(2, 1)]
    public class Entry : TerrariaPlugin
    {
        public override string Author => "SteelSeries";

        public override string Description => "Enables Journey mode";

        public override string Name => "JourneyUnlocker";

        public override Version Version => new("2.2.0.0");

        public Entry(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(
                Permissions.Unlock,
                JourneyCommand.Execute,
                "journey"
            ));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TShockAPI.Commands.ChatCommands.RemoveAll(cmd => 
                    cmd.CommandDelegate == new CommandDelegate(JourneyCommand.Execute)
                );
            }
        }
    }
}
