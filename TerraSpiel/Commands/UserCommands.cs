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

namespace TerraSpiel.Bot.Commands
{
    public class UserCommands : BaseCommandModule
    {
        private readonly IGameService _userService;
        public UserCommands(IGameService userService)
        {
            _userService = userService;
        }
        
        [Command("info")]
        public async Task UserInfo(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Player Info"
            };
            embed.Description = ctx.User.Mention;

            var (exists, user) = await _userService.PlayerExists((long)ctx.Member.Id);
            if (exists)
            {
                embed.AddField(":moneybag:" + " Money", "$" + user.Money.ToString());
                embed.AddField(":military_helmet:" + " Military", user.Military.ToString() + " Units");
                //TODO tiles info
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else
            {
                embed.Description = "You need to join the game first, use the join command";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
        }
        [Command("join")]
        public async Task JoinGame(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Join Game"
            };
            var exists = await _userService.PlayerExists((long)ctx.Member.Id);
            if (!exists.exists)
            {
                var creationResult = await _userService.CreateProfile((long)ctx.Member.Id);
                if (creationResult)
                {
                    embed.Description = "You sucessfuly joined the game!";
                }
                else
                {
                    embed.Description = "There has been a problem creating your new user in the current game, possibly due to no more room left in the game.";
                }
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else if (!exists.player.Alive)
            {
                embed.Description = "You have died and can not participate anymore in the current game";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else
            {
                embed.Description = "You have already joined the game.";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
        }
        [Command("DasBoot")]
        [Aliases("vonglomberton")]
        public async Task DasBoot(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("https://tenor.com/view/nikkalords-bosego-krieg-war-bosog-gif-22180760");
        }
    }
}
