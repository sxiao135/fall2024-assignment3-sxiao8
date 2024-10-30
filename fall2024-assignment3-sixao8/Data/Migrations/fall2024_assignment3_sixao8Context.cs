using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fall2024_assignment3_sixao8.Models;

namespace fall2024_assignment3_sixao8.Data
{
    public class fall2024_assignment3_sixao8Context : DbContext
    {
        public fall2024_assignment3_sixao8Context (DbContextOptions<fall2024_assignment3_sixao8Context> options)
            : base(options)
        {
        }

        public DbSet<fall2024_assignment3_sixao8.Models.Actor> Actor { get; set; } = default!;
        public DbSet<fall2024_assignment3_sixao8.Models.Movie> Movie { get; set; } = default!;
        public DbSet<fall2024_assignment3_sixao8.Models.ActorMovie> ActorMovie { get; set; } = default!;
    }
}
