# JourneyUnlock

JourneyUnlock is a TShock plugin that lets players temporarily use Journey Mode and unlock items in the Journey duplication menu on a non-Journey server.

## Features

- Enables or disables Journey Mode for an individual player.
- Unlocks a specific item by name or numeric ID.
- Unlocks every item available in Terraria.

## Installation

1. Build the project or download a compiled `JourneyUnlock.dll`.
2. Copy `JourneyUnlock.dll` into the server's `ServerPlugins` directory.
3. Restart the server.
4. Grant the required permissions to the appropriate TShock groups.

## Commands

| Command | Description | Additional permission |
| --- | --- | --- |
| `/journey help` | Shows the available commands. | None |
| `/journey unlock <item>` | Unlocks one item by name or ID. | None |
| `/journey all` | Unlocks every item in the Journey duplication menu. | `journeymode.all` |
| `/journey enable` | Enables Journey Mode for your character. | `journeymode.enable` |
| `/journey disable` | Disables Journey Mode for your character. | `journeymode.disable` |

The base `journeymode.unlock` permission is required to access the `/journey` command and all of its subcommands.

Example permission setup:

```text
/group addperm <group> journeymode.unlock
/group addperm <group> journeymode.all
/group addperm <group> journeymode.enable
/group addperm <group> journeymode.disable
```

Replace `<group>` with the name of your TShock group.

## Important behavior

Journey Mode is enabled for the player's current session by sending the required server-side-character WorldInfo state and changing that player's difficulty. Items and other character changes made while this mode is active should not be treated as permanently saved character data.
