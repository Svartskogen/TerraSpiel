using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerraSpiel.Bot.Services
{
    public class GameConfiguration
    {
        public string Enviroment { get; private set; }
        public int HarvestCd { get; private set; }
        public int TrainCd { get; private set; }
        public int BuildCd { get; private set; }
        public int StartingMoney { get; private set; }

        //TODO implementar en json
        public int BaseReinforceCost { get; private set; }
        public int ReinforceCounterModifier { get; private set; } 
        public int InitialHQReinforcementLevel { get; private set; }
        public int InitialTileBoughtReinforcementLevel { get; private set; }
        public int TileBuyBasePrice { get; private set; }
        public int MilitaryUnityCost { get; set; }

        //Building values
        public (int min, int max) HQMoneyProduction;
        public int HQUnitsProduction;
        public (int min, int max) FactoryMoneyProduction;
        public int FactoryPrice;
        public int BarracksUnitsProduction;
        public int BarracksPrice;
        public GameConfiguration(ConfigJson json)
        {
            Enviroment = json.Enviroment;
            HarvestCd = json.HarvestCd;
            TrainCd = json.TrainCd;
            BuildCd = json.BuildCd;

            StartingMoney = json.StartingMoney;

            BaseReinforceCost = 500;
            ReinforceCounterModifier = 100;
            InitialHQReinforcementLevel = 25;
            InitialTileBoughtReinforcementLevel = 5;
            TileBuyBasePrice = 750;
            MilitaryUnityCost = 30;

            //Buildings
            HQMoneyProduction = (20, 60); //average: 40, 80/hr
            HQUnitsProduction = 1;
            FactoryMoneyProduction = (25, 40); //average: 32.5, 65/hr
            FactoryPrice = 800;
            BarracksUnitsProduction = 1;
            BarracksPrice = 700;
        }
    }
}
