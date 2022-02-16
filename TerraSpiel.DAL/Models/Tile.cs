using System;
using System.Collections.Generic;
using System.Text;

namespace TerraSpiel.DAL.Models
{
    public class Tile : Entity
    {
        const int REINFORCEMENT_AMOUNT_PER_UPGRADE = 5; //cantidad de niveles de reinforcement que se le da por cada reinforce
        public int X { get; set; }
        public int Y { get; set; }
        public long OwnerId { get; set; }
        public int BuildingId { get; set; }
        public int ReinforcementLevel { get; set; }
        public int ReinforcementCounter { get; set; }
        public Tile()
        {

        }
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            OwnerId = 0;
            BuildingId = (int)BuildingType.None;
            ReinforcementLevel = 0;
            ReinforcementCounter = 0;
        }
        public void InitializeForUser(long playerId, int initialHQReinforcement)
        {
            if(OwnerId != 0)
            {
                Console.WriteLine("- ERROR: se inicializo una tile para jugador ya ocupada");
            }
            OwnerId = playerId;
            BuildingId = (int)BuildingType.HQ;
            ReinforcementLevel = initialHQReinforcement;
        }
        public void Buy(long playerId, int initialTileBuyReinforcement)
        {
            OwnerId = playerId;
            ReinforcementLevel = initialTileBuyReinforcement;
        }
        public void Reinforce()
        {
            ReinforcementLevel += REINFORCEMENT_AMOUNT_PER_UPGRADE;
            ReinforcementCounter++;
        }
        public void Build(BuildingType buildingType)
        {
            BuildingId = (int)buildingType;
        }
    }
    public enum BuildingType { None, HQ, Barracks, Factory, Farm, WeaponFactory, Bank, ResistanceCamp, GuardTower, MilitaryBase }
}
