using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.Bot.Services;
using TerraSpiel.DAL.Models;
using static TerraSpiel.Bot.Services.Utils;

namespace TerraSpiel.Bot.GameObjects
{
    public class Building
    {
        public readonly BuildingType BuildingType = BuildingType.None;

        protected MapInfo mapInfo;
        protected GameConfiguration gameConfig;
        protected long ownerId;
        protected Random random;
        public Building(MapInfo mapState, GameConfiguration gameConfig, long ownerId, Random random)
        {
            this.mapInfo = mapState;
            this.gameConfig = gameConfig;
            this.ownerId = ownerId;
            this.random = random;
        }
        public virtual int GetMoneyProduction()
        {
            return 0;
        }
        public virtual int GetUnitsProduction()
        {
            return 0;
        }
    }
}
