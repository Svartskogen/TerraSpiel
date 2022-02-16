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
    public class AdminCommands : BaseCommandModule
    {
        private readonly IGameService _userService;
        public AdminCommands(IGameService userService)
        {
            _userService = userService;
        }
        [Command("initialize")]
        [RequireOwner]
        public async Task InitalizeMap(CommandContext ctx, int size)
        {
            if(size <= 2)
            {
                return;
            }
            await _userService.InitializeGame(size);
            await ctx.Channel.SendMessageAsync("New game sucessfuly created, map size: " + size + "x" + size);
        }
    }
}
