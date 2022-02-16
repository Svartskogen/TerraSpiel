using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TerraSpiel.DAL.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
