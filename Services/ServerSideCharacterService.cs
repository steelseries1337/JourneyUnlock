using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Social;
using TShockAPI;
using static Terraria.WorldGen;

namespace JourneyUnlocker.Services
{
    public static class ServerSideCharacterService
    {
        public static void EnableSSC(TSPlayer player)
        {
            if (Main.ServerSideCharacter)
                return;

            if (player == null)
                throw new NullReferenceException("Player is null");

            var packet = BuildWorldInfoPacket(true);
            player.SendRawData(packet);
        }

        private static byte[] BuildWorldInfoPacket(bool sscEnabled)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.BaseStream.Position = 2;
            writer.Write((byte)7);

            WriteTimeData(writer);
            WriteWorldDimensions(writer);
            WriteWorldInfo(writer);
            WriteBackgroundStyles(writer);
            WriteTreeData(writer);
            WriteCaveData(writer);
            WriteWeatherData(writer);
            WriteGameProgressFlags(writer, sscEnabled);
            WriteOreInfo(writer);
            WriteMiscData(writer);

            var length = (ushort)writer.BaseStream.Position;
            writer.BaseStream.Position = 0;
            writer.Write(length);

            return stream.ToArray();
        }

        private static void WriteTimeData(BinaryWriter writer)
        {
            writer.Write((int)Main.time);

            var dayFlags = new BitsByte
            {
                [0] = Main.dayTime,
                [1] = Main.bloodMoon,
                [2] = Main.eclipse
            };
            writer.Write(dayFlags);

            writer.Write((byte)Main.moonPhase);
        }

        private static void WriteWorldDimensions(BinaryWriter writer)
        {
            writer.Write((short)Main.maxTilesX);
            writer.Write((short)Main.maxTilesY);
            writer.Write((short)Main.spawnTileX);
            writer.Write((short)Main.spawnTileY);
            writer.Write((short)Main.worldSurface);
            writer.Write((short)Main.rockLayer);
        }

        private static void WriteWorldInfo(BinaryWriter writer)
        {
            writer.Write(Main.ActiveWorldFileData.WorldId);
            writer.Write(Main.worldName);
            writer.Write((byte)Main.GameMode);
            writer.Write(Main.ActiveWorldFileData.UniqueId.ToByteArray());
            writer.Write(Main.ActiveWorldFileData.WorldGeneratorVersion);
            writer.Write((byte)Main.moonType);
        }

        private static void WriteBackgroundStyles(BinaryWriter writer)
        {
            writer.Write((byte)WorldGen.treeBG1);
            writer.Write((byte)WorldGen.treeBG2);
            writer.Write((byte)WorldGen.treeBG3);
            writer.Write((byte)WorldGen.treeBG4);
            writer.Write((byte)WorldGen.corruptBG);
            writer.Write((byte)WorldGen.jungleBG);
            writer.Write((byte)WorldGen.snowBG);
            writer.Write((byte)WorldGen.hallowBG);
            writer.Write((byte)WorldGen.crimsonBG);
            writer.Write((byte)WorldGen.desertBG);
            writer.Write((byte)WorldGen.oceanBG);
            writer.Write((byte)WorldGen.mushroomBG);
            writer.Write((byte)WorldGen.underworldBG);
            writer.Write((byte)Main.iceBackStyle);
            writer.Write((byte)Main.jungleBackStyle);
            writer.Write((byte)Main.hellBackStyle);
        }

        private static void WriteTreeData(BinaryWriter writer)
        {
            writer.Write(Main.windSpeedTarget);
            writer.Write((byte)Main.numClouds);

            for (int i = 0; i < 3; i++)
            {
                writer.Write(Main.treeX[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write((byte)Main.treeStyle[i]);
            }
        }

        private static void WriteCaveData(BinaryWriter writer)
        {
            for (int i = 0; i < 3; i++)
            {
                writer.Write(Main.caveBackX[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write((byte)Main.caveBackStyle[i]);
            }

            WorldGen.TreeTops.SyncSend(writer);
        }

        private static void WriteWeatherData(BinaryWriter writer)
        {
            writer.Write(Main.raining ? Main.maxRaining : 0f);
        }

        private static void WriteGameProgressFlags(BinaryWriter writer, bool sscEnabled)
        {
            var flags1 = new BitsByte
            {
                [0] = WorldGen.shadowOrbSmashed,
                [1] = NPC.downedBoss1,
                [2] = NPC.downedBoss2,
                [3] = NPC.downedBoss3,
                [4] = Main.hardMode,
                [5] = NPC.downedClown,
                [6] = sscEnabled,
                [7] = NPC.downedPlantBoss
            };
            writer.Write(flags1);

            var flags2 = new BitsByte
            {
                [0] = NPC.downedMechBoss1,
                [1] = NPC.downedMechBoss2,
                [2] = NPC.downedMechBoss3,
                [3] = NPC.downedMechBossAny,
                [4] = Main.cloudBGActive >= 1f,
                [5] = WorldGen.crimson,
                [6] = Main.pumpkinMoon,
                [7] = Main.snowMoon
            };
            writer.Write(flags2);

            var flags3 = new BitsByte
            {
                [1] = Main.fastForwardTimeToDawn,
                [2] = Main.slimeRain,
                [3] = NPC.downedSlimeKing,
                [4] = NPC.downedQueenBee,
                [5] = NPC.downedFishron,
                [6] = NPC.downedMartians,
                [7] = NPC.downedAncientCultist
            };
            writer.Write(flags3);

            var flags4 = new BitsByte
            {
                [0] = NPC.downedMoonlord,
                [1] = NPC.downedHalloweenKing,
                [2] = NPC.downedHalloweenTree,
                [3] = NPC.downedChristmasIceQueen,
                [4] = NPC.downedChristmasSantank,
                [5] = NPC.downedChristmasTree,
                [6] = NPC.downedGolemBoss,
                [7] = BirthdayParty.PartyIsUp
            };
            writer.Write(flags4);

            var flags5 = new BitsByte
            {
                [0] = NPC.downedPirates,
                [1] = NPC.downedFrost,
                [2] = NPC.downedGoblins,
                [3] = Sandstorm.Happening,
                [4] = DD2Event.Ongoing,
                [5] = DD2Event.DownedInvasionT1,
                [6] = DD2Event.DownedInvasionT2,
                [7] = DD2Event.DownedInvasionT3
            };
            writer.Write(flags5);

            var flags6 = new BitsByte
            {
                [0] = NPC.combatBookWasUsed,
                [1] = LanternNight.LanternsUp,
                [2] = NPC.downedTowerSolar,
                [3] = NPC.downedTowerVortex,
                [4] = NPC.downedTowerNebula,
                [5] = NPC.downedTowerStardust,
                [6] = Main.forceHalloweenForToday,
                [7] = Main.forceXMasForToday
            };
            writer.Write(flags6);

            var flags7 = new BitsByte
            {
                [0] = NPC.boughtCat,
                [1] = NPC.boughtDog,
                [2] = NPC.boughtBunny,
                [3] = NPC.freeCake,
                [4] = Main.drunkWorld,
                [5] = NPC.downedEmpressOfLight,
                [6] = NPC.downedQueenSlime,
                [7] = Main.getGoodWorld
            };
            writer.Write(flags7);

            var flags8 = new BitsByte
            {
                [0] = Main.tenthAnniversaryWorld,
                [1] = Main.dontStarveWorld,
                [2] = NPC.downedDeerclops,
                [3] = Main.notTheBeesWorld,
                [4] = Main.remixWorld,
                [5] = NPC.unlockedSlimeBlueSpawn,
                [6] = NPC.combatBookVolumeTwoWasUsed,
                [7] = NPC.peddlersSatchelWasUsed
            };
            writer.Write(flags8);

            var flags9 = new BitsByte
            {
                [0] = NPC.unlockedSlimeGreenSpawn,
                [1] = NPC.unlockedSlimeOldSpawn,
                [2] = NPC.unlockedSlimePurpleSpawn,
                [3] = NPC.unlockedSlimeRainbowSpawn,
                [4] = NPC.unlockedSlimeRedSpawn,
                [5] = NPC.unlockedSlimeYellowSpawn,
                [6] = NPC.unlockedSlimeCopperSpawn,
                [7] = Main.fastForwardTimeToDusk
            };
            writer.Write(flags9);

            var flags10 = new BitsByte
            {
                [0] = Main.noTrapsWorld,
                [1] = Main.zenithWorld,
                [2] = NPC.unlockedTruffleSpawn,
                [3] = Main.vampireSeed,
                [4] = Main.infectedSeed,
                [5] = Main.teamBasedSpawnsSeed,
                [6] = Main.skyblockWorld,
                [7] = Main.dualDungeonsSeed
            };
            writer.Write(flags10);

            var flags11 = new BitsByte
            {
                [0] = WorldGen.Skyblock.lowTiles
            };
            writer.Write(flags11);

            writer.Write((byte)Main.sundialCooldown);
            writer.Write((byte)Main.moondialCooldown);
        }

        private static void WriteOreInfo(BinaryWriter writer)
        {
            writer.Write((short)SavedOreTiers.Copper);
            writer.Write((short)SavedOreTiers.Iron);
            writer.Write((short)SavedOreTiers.Silver);
            writer.Write((short)SavedOreTiers.Gold);
            writer.Write((short)SavedOreTiers.Cobalt);
            writer.Write((short)SavedOreTiers.Mythril);
            writer.Write((short)SavedOreTiers.Adamantite);
        }

        private static void WriteMiscData(BinaryWriter writer)
        {
            writer.Write((sbyte)Main.invasionType);

            if (SocialAPI.Network != null)
            {
                writer.Write(SocialAPI.Network.GetLobbyId());
            }
            else
            {
                writer.Write(0uL);
            }

            writer.Write(Sandstorm.IntendedSeverity);
            ExtraSpawnPointManager.Write(writer, networking: true);
        }
    }
}
