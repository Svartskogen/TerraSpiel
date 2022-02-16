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
    public class CooldownCommands : BaseCommandModule
    {
        private readonly IGameService _userService;
        public CooldownCommands(IGameService userService)
        {
            _userService = userService;
        }

        [Command("harvest")]
        [Aliases("h", "farm", "collect")]
        public async Task Harvest(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Harvest"
            };
            var exists = await _userService.PlayerExists((long)ctx.Member.Id);
            if (!exists.exists)
            {
                embed.Description = "You need to join the game first, use the join command";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else if (!exists.player.Alive)
            {
                embed.Description = "You have died and can not participate anymore in the current game";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }


            var (result, remainingTime, amountHarvested) = await _userService.HarvestService((long)ctx.Member.Id);
            if (exists.exists && result)
            {
                embed.Description = ctx.Member.Mention + " successfuly harvested $" + amountHarvested + ":moneybag:";
                await ctx.Channel.SendMessageAsync(embed);
            }
            else
            {
                embed.Description = ":x: " + ctx.Member.Mention + " you must wait " + remainingTime / 60 + " minutes to harvest again.";
                await ctx.Channel.SendMessageAsync(embed);
            }
        }
        [Command("recruit")]
        [Aliases("train", "r")]
        public async Task Recruit(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Recruit"
            };
            var exists = await _userService.PlayerExists((long)ctx.Member.Id);
            if (!exists.exists)
            {
                embed.Description = "You need to join the game first, use the join command";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }
            else if (!exists.player.Alive)
            {
                embed.Description = "You have died and can not participate anymore in the current game";
                await ctx.Channel.SendMessageAsync(embed);
                return;
            }


            var (result, remainingTime, amountHarvested, cost) = await _userService.RecruitService((long)ctx.Member.Id);
            if (exists.exists && result)
            {
                embed.Description = ctx.Member.Mention + " successfuly trained " + amountHarvested + " units :military_helmet:";
                embed.AddField("Training cost:", "$" + cost);
                await ctx.Channel.SendMessageAsync(embed);
            }
            else
            {
                if(cost != 0)
                {
                    embed.Description = ":x: " + ctx.Member.Mention + " you cannot afford to train units, you need $" + cost;
                }
                else
                {
                    embed.Description = ":x: " + ctx.Member.Mention + " you must wait " + remainingTime / 60 + " minutes to train units again.";
                }
                
                await ctx.Channel.SendMessageAsync(embed);
            }
        }
        [Command("cooldown")]
        [Aliases("cd")]
        public async Task Cooldowns(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Cooldowns"
            };
            var cooldowns = await _userService.GetCooldowns((long)ctx.Member.Id);
            if(cooldowns.harvest > 0)
            {
                embed.AddField("Harvest", cooldowns.harvest / 60 + " minutes to harvest");
            }
            else
            {
                embed.AddField("Harvest", "Ready");
            }

            if (cooldowns.recruit > 0)
            {
                embed.AddField("Recruit", cooldowns.recruit / 60 + " minutes to recruit units");
            }
            else
            {
                embed.AddField("Recruit", "Ready");
            }

            if (cooldowns.build > 0)
            {
                embed.AddField("Build", cooldowns.build / 60 + " minutes to build");
            }
            else
            {
                embed.AddField("Build", "Ready");
            }
            await ctx.Channel.SendMessageAsync(embed);
        }
    }
}
