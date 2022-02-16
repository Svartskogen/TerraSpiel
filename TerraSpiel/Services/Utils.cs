using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.GameObjects;
using TerraSpiel.DAL.Models;

namespace TerraSpiel.Bot.Services
{
    public static class Utils
    {
        public static readonly string emptyTileFlag = ":white_large_square:";
        public static readonly string[] availableFlags = new string[] { ":blue_square:", ":red_square:", ":orange_square:", ":brown_square:", ":purple_square:", ":green_square:", ":yellow_square:", ":black_large_square:", ":red_circle:", ":blue_circle:", ":brown_circle:", ":purple_circle:", ":green_circle:", ":yellow_circle:", ":orange_circle:", ":black_circle:" };
        public static readonly string[] xAxisNumeration = new string[] { ":regional_indicator_a:",":regional_indicator_b:",":regional_indicator_c:",":regional_indicator_d:",":regional_indicator_e:",":regional_indicator_f:",":regional_indicator_g:",":regional_indicator_h:",":regional_indicator_i:",":regional_indicator_j:",":regional_indicator_k:",":regional_indicator_l:",":regional_indicator_m:",":regional_indicator_n:",":regional_indicator_o:",":regional_indicator_p:" };
        public static readonly string[] yAxisNumeration = new string[] { ":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:", ":keycap_ten:" };
        public static readonly string mapProp = "🗺";
        public static string GetNextFlag(int playersAmount)
        {
            return availableFlags[playersAmount];
        }
        public static long GetTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public static int GetReinforcementCost(Tile tile, GameConfiguration config)
        {
            return config.BaseReinforceCost + (tile.ReinforcementCounter * config.ReinforceCounterModifier);
        }
        public static int GetNextTileCost(Player player, GameConfiguration config)
        {
            return (player.TilesBoughtCounter + 1) * config.TileBuyBasePrice;
        }
        public static bool IsNeighbor(Tile first, Tile second)
        {
            if (first.X == second.X && first.Y == second.Y) return false;
            return (first.X >= second.X - 1 && first.X <= second.X + 1) && (first.Y >= second.Y - 1 && first.Y <= second.Y + 1);
        }
        public static Building GetBuildingOfType(BuildingType type, MapInfo mapState, GameConfiguration gameConfig, long ownerId, Random random)
        {
            switch (type)
            {
                case BuildingType.None:
                    return new Building(mapState, gameConfig, ownerId, random);
                case BuildingType.HQ:
                    return new HQ(mapState, gameConfig, ownerId, random);
                case BuildingType.Barracks:
                    return new Barracks(mapState, gameConfig, ownerId, random);
                case BuildingType.Factory:
                    return new Factory(mapState, gameConfig, ownerId, random);
            }
            return null;
        }
        public class MapInfo
        {
            public int amountOfOccupiedTiles;
            public int amountOfEmptyTiles;
            public Dictionary<Player, int> amountOfTilesPerUser;
            public int mapSize;
            public int totalTiles;
            public MapInfo()
            {
                amountOfOccupiedTiles = 0;
                amountOfEmptyTiles = 0;
                amountOfTilesPerUser = new Dictionary<Player, int>();
                mapSize = 0;
                totalTiles = 0;
            }
        }
        public class BuildOption
        {
            public BuildingType type;
            public string symbol;
            public string parsedEmoji;
            public string description;
            public int cost;
            public bool availableToBuy;
            public BuildOption(BuildingType type, string symbol, string parsedEmoji, string description, int cost, bool available)
            {
                this.type = type;
                this.symbol = symbol;
                this.parsedEmoji = parsedEmoji;
                this.description = description;
                this.cost = cost;
                this.availableToBuy = available;
            }
        }
    }
}
