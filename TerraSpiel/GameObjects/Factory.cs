using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;

namespace TerraSpiel.Bot.GameObjects
{
    public class Factory : Building
    {
        public Factory(Utils.MapInfo mapState, GameConfiguration gameConfig, long ownerId, Random random) : base(mapState, gameConfig, ownerId, random)
        {
        }

        public override int GetMoneyProduction()
        {
            return random.Next(gameConfig.FactoryMoneyProduction.min, gameConfig.FactoryMoneyProduction.max); ;
        }

        public override int GetUnitsProduction()
        {
            return 0;
        }
    }
}
