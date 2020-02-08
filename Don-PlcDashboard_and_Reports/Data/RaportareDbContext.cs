using Don_PlcDashboard_and_Reports.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Don_PlcDashboard_and_Reports.Data
{
    public class RaportareDbContext : DbContext
    {
        public RaportareDbContext(DbContextOptions<RaportareDbContext> options)
            : base(options)
        { }

        public DbSet<PlcModel> Plcs { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        //public DbSet<ConsumGazModel> ConsumGazModels{ get; set; }
    }
}
