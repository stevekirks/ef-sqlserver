using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EfSqlServer.Ef
{
    [Table("Food")]
    public partial class Food
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
