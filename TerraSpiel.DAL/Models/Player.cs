using System;
using System.Collections.Generic;
using System.Text;

namespace TerraSpiel.DAL.Models
{
    public class Player : Entity
    {
        public long DiscordID { get; set; }
        public int Money { get; set; }
        public int Military { get; set; }
        public bool Alive { get; set; }
        //CDS
        public long HarvestTimestamp { get; set; }
        public long TrainTimestamp { get; set; }
        public long BuildTimestamp { get; set; }

        //Owned cells
        public string Flag { get; set; }
        public int TilesBoughtCounter { get; set; }

        public Player()
        {

        }
        public Player(long discordId, int startingMoney, string flag)
        {
            DiscordID = discordId;
            Money = startingMoney;
            Military = 0;
            Alive = true;
            HarvestTimestamp = 0;
            TrainTimestamp = 0;
            BuildTimestamp = 0;
            Flag = flag;
            TilesBoughtCounter = 0;
        }

    }
}
