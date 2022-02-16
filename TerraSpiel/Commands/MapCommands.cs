using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using TerraSpiel.DAL.Models;

namespace TerraSpiel.Bot.Commands
{
    public class MapCommands : BaseCommandModule
    {
        private readonly IGameService _userService;
        public MapCommands(IGameService userService)
        {
            _userService = userService;
        }
        [Command("map")]
        public async Task Map(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Current Map View"
            };
            var render = await _userService.GetMapRender();

            var mapInfo = render.mapInfo;

            foreach (var player in mapInfo.amountOfTilesPerUser.Keys)
            {
                var discordUser = await ctx.Guild.GetMemberAsync((ulong)player.DiscordID);
                embed.Description += player.Flag + ": " + discordUser.Mention + " "+ mapInfo.amountOfTilesPerUser[player] + " tiles\n";
            }
            embed.AddField("Total tiles", mapInfo.totalTiles.ToString());
            embed.AddField("Amount of occupied tiles", mapInfo.amountOfOccupiedTiles.ToString());
            embed.AddField("Amount of empty tiles", mapInfo.amountOfEmptyTiles.ToString());

            embed.Description += render.render;

            await ctx.Channel.SendMessageAsync(embed);
        }
        [Command("buytile")]
        [Aliases("buy")]
        public async Task BuyTile(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Buy Tile"
            };

            var availableTiles = await _userService.GetPlayerAvailableBuyTiles((long)ctx.User.Id);

            if (availableTiles == null || availableTiles.Length == 0)
            {
                embed.Description = "You have no neighboring tiles available to buy.";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else
            {
                var mapRender = await _userService.GetMapRender(availableTiles, ":placard:");
                embed.Description = "These are the neighboring tiles available to buy for you:";
                embed.Description += "\n(Marked with " + ":placard:)";
                embed.Description += "\nUse \"buy x y\" to buy the tile you want.";
                embed.Description += "\nThe tile will cost you: $" + await _userService.GetNextTileCost((long)ctx.User.Id);
                embed.Description += "\n" + mapRender.render;

                foreach(var tile in availableTiles)
                {
                    embed.AddField("Tile " + tile.X + "," + tile.Y, "use \"buytile " + tile.X + " " + tile.Y + "\" to buy this tile");
                }

                await ctx.Channel.SendMessageAsync(embed);
            }
        }
        [Command("buytile")]
        public async Task BuyTile(CommandContext ctx, int x, int y)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var embed = new DiscordEmbedBuilder
            {
                Title = "Buy Tile Confirmation"
            };
            embed.Description = "Are you sure you want to buy this tile?";
            embed.Description += "\nTile marked with :white_check_mark: \n";

            var tilePrice = await _userService.GetNextTileCost((long)ctx.User.Id);
            embed.AddField("Price:", tilePrice.ToString());

            var tile = await _userService.GetTile(x, y);
            var (_, render) = await _userService.GetMapRender(new DAL.Models.Tile[] { tile }, ":white_check_mark:");

            embed.Description += render;

            var messageInstance = await ctx.Channel.SendMessageAsync(embed);
            await messageInstance.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
            await messageInstance.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));

            var response = await interactivity.WaitForReactionAsync(o => o.Message == messageInstance && o.User == ctx.User).ConfigureAwait(false);

            if(response.Result.Emoji.Name == "✅")
            {
                var buyRequest = await _userService.BuyTile((long)ctx.User.Id, tile);

                embed = new DiscordEmbedBuilder
                {
                    Title = "Buy Tile Result"
                };
                if (buyRequest.success)
                {
                    embed.Description = "Tile bought successfuly";
                }
                else
                {
                    if (buyRequest.notEnoughMoney)
                    {
                        embed.Description = "You dont have enough money to buy this tile";
                    }
                    else if (buyRequest.notNeighbor)
                    {
                        embed.Description = "This tile is not a neighbour of any of your tiles";
                    }
                    else if (buyRequest.occupied)
                    {
                        embed.Description = "This tile is currently occupied by another player";
                    }
                }
                await ctx.Channel.SendMessageAsync(embed);
            }
            else if(response.Result.Emoji.Name == ":x:")
            {
                return;
            }
        }
        [Command("build")]
        public async Task Build(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Build"
            };

            var userTiles = await _userService.GetPlayerTiles((long)ctx.User.Id);
            var availableToBuild = userTiles.Where(o => o.BuildingId == 0).ToArray();
            var mapRender = await _userService.GetMapRender(availableToBuild, ":hammer:");
            embed.Description = "These are your tiles with space available to build:";
            embed.Description += "\n(Marked with " + ":hammer:)";
            embed.Description += "\nUse \"build x y\" to select a tile to build";
            //embed.Description += "\nUse \"demolish x y\"if you want to clear a building from an already used tile";
            embed.Description += "\n" + mapRender.render;

            foreach (var tile in userTiles)
            {
                string text = "";
                if(tile.BuildingId != 0)
                {
                    text = ((BuildingType)tile.BuildingId).ToString();
                    if(tile.BuildingId == (int)BuildingType.HQ)
                    {
                        text += "\nyou cannot change your initial HQ building";
                    }
                    else
                    {
                        text += "\nif you build something here, it will replace its existing building";
                    }
                }
                else
                {
                    text = "Clear tile, Available to build here";
                }
                embed.AddField("Tile " + tile.X + "," + tile.Y, text);
            }
            await ctx.Channel.SendMessageAsync(embed);
        }
        [Command("build")]
        public async Task Build(CommandContext ctx, int x, int y)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var embed = new DiscordEmbedBuilder
            {
                Title = "Buy in Tile"
            };
            embed.Description = "Choose which building you want to build in this tile";
            embed.Description += "\nTile marked with :hammer: \n";

            var tile = await _userService.GetTile(x, y);
            var (_, render) = await _userService.GetMapRender(new DAL.Models.Tile[] { tile }, ":hammer:");

            embed.Description += render;

            //List available buildings
            var buildings = _userService.GetBuildings();

            foreach(var building in buildings)
            {
                if (building.availableToBuy)
                {
                    embed.AddField(building.symbol + " " + building.type.ToString(), building.description + "\n Price to build: $" + building.cost); 
                }
            }

            var messageInstance = await ctx.Channel.SendMessageAsync(embed);

            foreach (var building in buildings)
            {
                if (building.availableToBuy)
                {
                    var emoji = DiscordEmoji.FromName(ctx.Client, building.symbol);
                    building.parsedEmoji = emoji.Name;
                    await messageInstance.CreateReactionAsync(emoji);
                }
            }
            
            var response = await interactivity.WaitForReactionAsync(o => o.Message == messageInstance && o.User == ctx.User).ConfigureAwait(false);

            BuildingType type = BuildingType.None;
            foreach (var building in buildings)
            {
                if(building.availableToBuy && response.Result.Emoji.Name == building.parsedEmoji)
                {
                    type = building.type;
                    break;
                }
            }

            if(type == BuildingType.None)
            {
                await ctx.Channel.SendMessageAsync("Error.");
                return;
            }
            var service = await _userService.BuildService((long)ctx.User.Id, tile, type);

            embed = new DiscordEmbedBuilder
            {
                Title = "Buy Tile Result"
            };

            if (service.result)
            {
                embed.Description = "Sucessfuly built " + type + " at tile " + x + " " + y;
            }
            else
            {
                if (service.notEnoughMoney)
                {
                    embed.Description = "You dont have enough money to build " + type;
                }
                else if (tile.BuildingId == (int)BuildingType.HQ)
                {
                    embed.Description = "You cannot replace your HQ";
                }
                else
                {
                    embed.Description = ":x: " + ctx.Member.Mention + " you must wait " + service.remainingTime / 60 + " minutes to build again.";
                    
                }
            }
            await ctx.Channel.SendMessageAsync(embed);
        }
    }
}
