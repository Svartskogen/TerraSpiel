using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerraSpiel.DAL.Models;

namespace TerraSpiel.Bot.GameObjects
{
    [Obsolete]
    public class BuildingTile : Tile
    {
        public Building building;
        public BuildingTile(Tile fromTile)
        {

        }
    }
}
