using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSpiel.Bot.GameObjects;
using TerraSpiel.DAL;
using TerraSpiel.DAL.Models;
using static TerraSpiel.Bot.Services.Utils;

namespace TerraSpiel.Bot.Services
{
    public interface IGameService
    {
        Task<(bool exists, Player player)> PlayerExists(long discordId);
        Task<bool> CreateProfile(long discordId);
        Task<(bool result, int remainingTime, int amountHarvested)> HarvestService(long discordId);
        Task<(bool result, int remainingTime, int amountHarvested, int cost)> RecruitService(long discordId);
        Task<(bool result, int remainingTime, bool notEnoughMoney)> BuildService(long discordId, Tile tile, BuildingType building);
        Task InitializeGame(int mapSize);
        Task<(MapInfo mapInfo, string render)> GetMapRender();
        Task<(MapInfo mapInfo, string render)> GetMapRender(Tile[] tilesToMark, string mark);
        Task<Tile[]> GetPlayerTiles(long discordId);
        Task<Tile[]> GetPlayerAvailableBuyTiles(long discordId);
        Task<(bool success, bool notEnoughMoney, bool notNeighbor, bool occupied)> BuyTile(long buyer, Tile tileToBuy);
        Task<Tile> GetTile(int x, int y);
        Task<int> GetNextTileCost(long discordId);
        Task<(long harvest, long recruit, long build)> GetCooldowns(long discordId);
        BuildOption[] GetBuildings();
    }
    public class GameService : IGameService
    {
        private readonly GameContext gameContext;
        private readonly GameConfiguration gameConfig;
        private readonly Random random;

        public GameService(GameContext gameContext, GameConfiguration gameConfiguration)
        {
            this.gameContext = gameContext;
            this.gameConfig = gameConfiguration;
            this.random = new Random();
            

        }
        public async Task<(bool exists,Player player)> PlayerExists(long discordId)
        {
            var outProfile = await gameContext.Players.FirstOrDefaultAsync(x => x.DiscordID == discordId);
            return (outProfile != null, outProfile);
        }
        public async Task<bool> CreateProfile(long discordId)
        {
            //Creo el player
            Player profile = new Player(discordId, gameConfig.StartingMoney, Utils.GetNextFlag(gameContext.Players.Count()));
            await gameContext.Players.AddAsync(profile);
            //Creo su primer tile con HQ
            var tiles = gameContext.Map.Where(o => o.OwnerId == 0).ToArray();
            if(tiles.Length == 0)
            {
                return false;
            }

            tiles[random.Next(0, tiles.Length)].InitializeForUser(discordId, gameConfig.InitialHQReinforcementLevel);
            await gameContext.SaveChangesAsync();
            return true;
        }
        public async Task<(bool result, int remainingTime, int amountHarvested)> HarvestService(long discordId)
        {
            var (exists, player) = await PlayerExists(discordId);

            if (exists)
            {
                if (Utils.GetTime() > player.HarvestTimestamp + gameConfig.HarvestCd)
                {
                    var mapInfo = await GetMapRender();
                    var playerTiles = await GetPlayerTiles(discordId);
                    var playerBuildings = GetBuildingsFromTiles(playerTiles, mapInfo.mapInfo, discordId);

                    //Harvest
                    int amountToHarvest = 0;

                    foreach(var building in playerBuildings)
                    {
                        amountToHarvest += building.GetMoneyProduction();
                    }

                    player.Money += amountToHarvest;
                    player.HarvestTimestamp = Utils.GetTime();

                    await gameContext.SaveChangesAsync();
                    return (true, gameConfig.HarvestCd, amountToHarvest);
                }
                else
                {
                    //In cooldown
                    return (false, (int)(player.HarvestTimestamp + gameConfig.HarvestCd - Utils.GetTime()), 0);
                }
            }
            else
            {
                return (false, 0, 0);
            }
        }
        public async Task<(bool result, int remainingTime, int amountHarvested, int cost)> RecruitService(long discordId)
        {
            var (exists, player) = await PlayerExists(discordId);

            if (exists)
            {
                if (Utils.GetTime() > player.TrainTimestamp + gameConfig.TrainCd)
                {
                    var mapInfo = await GetMapRender();
                    var playerTiles = await GetPlayerTiles(discordId);
                    var playerBuildings = GetBuildingsFromTiles(playerTiles, mapInfo.mapInfo, discordId);

                    //Harvest
                    int amountToTrain = 0;
                    int cost;
                    foreach (var building in playerBuildings)
                    {
                        amountToTrain += building.GetUnitsProduction();
                    }

                    cost = amountToTrain * gameConfig.MilitaryUnityCost;
                    if(cost <= player.Money)
                    {
                        player.Military += amountToTrain;
                        player.Money -= cost;
                        player.TrainTimestamp = Utils.GetTime();

                        await gameContext.SaveChangesAsync();
                        return (true, gameConfig.TrainCd, amountToTrain, cost);
                    }
                    else
                    {
                        return (false, 0, 0, cost);
                    }
                }
                else
                {
                    //In cooldown
                    return (false, (int)(player.TrainTimestamp + gameConfig.TrainCd - Utils.GetTime()), 0, 0);
                }
            }
            else
            {
                return (false, 0, 0, 0);
            }
        }
        public async Task<(bool result, int remainingTime, bool notEnoughMoney)> BuildService(long discordId, Tile tile, BuildingType building)
        {
            var (exists, player) = await PlayerExists(discordId);

            if (exists && player.DiscordID == tile.OwnerId && tile.BuildingId != (int)BuildingType.HQ)
            {
                if (Utils.GetTime() > player.BuildTimestamp + gameConfig.BuildCd)
                {
                    var mapInfo = await GetMapRender();
                    var playerTiles = await GetPlayerTiles(discordId);
                    var playerBuildings = GetBuildingsFromTiles(playerTiles, mapInfo.mapInfo, discordId);

                    int cost;

                    cost = GetBuildings().Where(o => o.type == building).FirstOrDefault().cost;
                    if (cost <= player.Money)
                    {
                        tile.Build(building);
                        player.Money -= cost;
                        player.BuildTimestamp = Utils.GetTime();

                        await gameContext.SaveChangesAsync();
                        return (true, gameConfig.TrainCd, false);
                    }
                    else
                    {
                        return (false, 0, true);
                    }
                }
                else
                {
                    //In cooldown
                    return (false, (int)(player.BuildTimestamp + gameConfig.BuildCd - Utils.GetTime()), false);
                }
            }
            else
            {
                return (false, 0, false);
            }
        }
        public async Task InitializeGame(int mapSize)
        {
            //Limpio la db:
            foreach (var entity in gameContext.Map)
                gameContext.Map.Remove(entity);

            foreach (var entity in gameContext.Players)
                gameContext.Players.Remove(entity);

            Tile[,] newMapTiles = new Tile[mapSize, mapSize];
            for(int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    newMapTiles[x, y] = new Tile(x, y);
                    gameContext.Map.Add(newMapTiles[x, y]);
                }
            }

            await gameContext.SaveChangesAsync();
        }
        public async Task<(MapInfo mapInfo, string render)> GetMapRender()
        {
            return await GetMapRender(null, null);
        }
        public async Task<(MapInfo mapInfo, string render)> GetMapRender(Tile[] tilesToMark, string mark)
        {
            string render = "\n";
            var tiles = await gameContext.Map.ToArrayAsync();
            var players = await gameContext.Players.ToArrayAsync();
            var mapSize = (int)Math.Sqrt(tiles.Length);

            MapInfo mapInfo = new MapInfo();

            bool isMarked = false;

            render += Utils.mapProp;
            for(int i = 0; i < mapSize; i++) //y axis legend
            {
                render += Utils.yAxisNumeration[i];
            }
            render += "\n";

            for (int x = 0; x < mapSize; x++)
            {
                render += Utils.xAxisNumeration[x];
                for (int y = 0; y < mapSize; y++)
                {
                    var tile = tiles.Where(o => o.X == x && o.Y == y).FirstOrDefault();

                    if(tilesToMark != null)
                    {
                        isMarked = tilesToMark.Any(o => o == tile);
                    }

                    if (tile.OwnerId == 0)
                    {
                        if (isMarked)
                        {
                            render += mark;
                        }
                        else
                        {
                            render += Utils.emptyTileFlag;
                        }
                        mapInfo.amountOfEmptyTiles++;
                    }
                    else
                    {
                        var player = players.Where(o => o.DiscordID == tile.OwnerId).FirstOrDefault();
                        if (isMarked)
                        {
                            render += mark;
                        }
                        else
                        {
                            render += player.Flag;
                        }
                        mapInfo.amountOfOccupiedTiles++;
                        if (mapInfo.amountOfTilesPerUser.ContainsKey(player))
                        {
                            mapInfo.amountOfTilesPerUser[player]++;
                        }
                        else
                        {
                            mapInfo.amountOfTilesPerUser.Add(player, 1);
                        }
                    }

                }
                render += "\n";
            }
            mapInfo.amountOfTilesPerUser = mapInfo.amountOfTilesPerUser.OrderByDescending(o => o.Value).ToDictionary(x => x.Key, x=> x.Value);
            mapInfo.mapSize = mapSize;
            mapInfo.totalTiles = tiles.Length;

            return (mapInfo, render);
        }
        public async Task<Tile[]> GetPlayerTiles(long discordId)
        {
            return await gameContext.Map.Where(o => o.OwnerId == discordId).ToArrayAsync();
        }
        public async Task<Tile[]> GetPlayerAvailableBuyTiles(long discordId)
        {
            var freeTiles = await gameContext.Map.Where(o => o.OwnerId == 0).ToArrayAsync();
            var playerTiles = await GetPlayerTiles(discordId);

            List<Tile> neighbourTiles = new List<Tile>();

            for(int i = 0; i < playerTiles.Length; i++)
            {
                for(int j = 0; j < freeTiles.Length; j++)
                {
                    if(IsNeighbor(playerTiles[i], freeTiles[j]))
                    {
                        neighbourTiles.Add(freeTiles[j]);
                    }
                }
            }

            return neighbourTiles.GroupBy(o => o.Id).Select(y => y.First()).ToArray();
        }
        public async Task<(bool success, bool notEnoughMoney, bool notNeighbor, bool occupied)> BuyTile(long discordId, Tile tileToBuy)
        {
            var player = await gameContext.Players.FirstOrDefaultAsync(x => x.DiscordID == discordId);
            if(player != null)
            {
                var availableTiles = await GetPlayerAvailableBuyTiles(discordId);
                if (tileToBuy.OwnerId != 0)
                {
                    return (false, false, false, true);
                }
                else if(player.Money < Utils.GetNextTileCost(player, gameConfig))
                {
                    return (false, true, false, false);
                }
                else if(availableTiles.Any(o => o == tileToBuy))
                {
                    player.Money -= Utils.GetNextTileCost(player, gameConfig);
                    player.TilesBoughtCounter += 1;
                    tileToBuy.Buy(discordId, gameConfig.InitialTileBoughtReinforcementLevel);

                    await gameContext.SaveChangesAsync();
                    return (true, false, false, false);
                }
                else
                {
                    return (false, false, true, false);
                }
            }
            else
            {
                return (false, false, false, false);
            }
        }
        public async Task<Tile> GetTile(int x, int y)
        {
            return await gameContext.Map.FirstOrDefaultAsync(o => o.X == x && o.Y == y);
        }
        public async Task<int> GetNextTileCost(long discordId)
        {
            var user = await gameContext.Players.FirstOrDefaultAsync(x => x.DiscordID == discordId);
            if(user != null)
            {
                return Utils.GetNextTileCost(user, gameConfig);
            }
            else
            {
                return -1;
            }
        }
        public async Task<(long harvest, long recruit, long build)> GetCooldowns(long discordId)
        {
            var outProfile = await gameContext.Players.FirstOrDefaultAsync(x => x.DiscordID == discordId);
            if(outProfile != null)
            {
                return (outProfile.HarvestTimestamp + gameConfig.HarvestCd - Utils.GetTime(),
                    outProfile.TrainTimestamp + gameConfig.TrainCd - Utils.GetTime(),
                    outProfile.BuildTimestamp + gameConfig.BuildCd - Utils.GetTime());
            }
            else
            {
                return (0, 0, 0);
            }
        }
        public Building[] GetBuildingsFromTiles(Tile[] tiles, MapInfo latestMapState, long owner)
        {
            /*var buildingTiles = new BuildingTile[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                buildingTiles[i] = new BuildingTile(tiles[i]);
            }
            return buildingTiles;*/
            var buildings = new Building[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                buildings[i] = GetBuildingOfType((BuildingType)tiles[i].BuildingId, latestMapState, gameConfig, owner, random);
            }
            return buildings;
        }
        public BuildOption[] GetBuildings()
        {
            //:bank::camping::ear_of_rice::military_helmet::european_castle::factory::classical_building::white_large_square:
            List<BuildOption> list = new List<BuildOption>();
            list.Add(new BuildOption(BuildingType.None, ":white_large_square:", "", "Empty tile", 0, false));
            list.Add(new BuildOption(BuildingType.HQ, ":european_castle:", "", "Main HQ", 0, false));
            list.Add(new BuildOption(BuildingType.Barracks, ":military_helmet:", "", "Produces military units at a fixed rate", gameConfig.BarracksPrice, true));
            list.Add(new BuildOption(BuildingType.Factory, ":factory:", "🏭", "Produces money at a fixed rate", gameConfig.FactoryPrice, true));

            return list.ToArray();
        }
    }
}
