using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;

namespace TerraSpiel.Bot.GameObjects
{
    public class Barracks : Building
    {
        public Barracks(Utils.MapInfo mapState, GameConfiguration gameConfig, long ownerId, Random random) : base(mapState, gameConfig, ownerId, random)
        {
        }

        public override int GetMoneyProduction()
        {
            return 0;
        }

        public override int GetUnitsProduction()
        {
            return gameConfig.BarracksUnitsProduction;
        }
    }
}
