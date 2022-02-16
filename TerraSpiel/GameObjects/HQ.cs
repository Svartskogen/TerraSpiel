using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;

namespace TerraSpiel.Bot.GameObjects
{
    public class HQ : Building
    {
        public HQ(Utils.MapInfo mapState, GameConfiguration gameConfig, long ownerId, Random random) : base(mapState, gameConfig, ownerId, random)
        {
        }

        public override int GetMoneyProduction()
        {
            return random.Next(gameConfig.HQMoneyProduction.min, gameConfig.HQMoneyProduction.max);
        }

        public override int GetUnitsProduction()
        {
            return gameConfig.HQUnitsProduction;
        }
    }
}
