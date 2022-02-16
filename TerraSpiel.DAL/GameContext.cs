using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TerraSpiel.DAL.Models;

namespace TerraSpiel.DAL
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Tile> Map { get; set; }
    }
}
