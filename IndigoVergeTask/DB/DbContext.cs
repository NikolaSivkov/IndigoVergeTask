using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndigoVergeTask.DB
{
    public class SensorDataDB : DbContext
    {
        public DbSet<SensorRecord> SensorsData { get; set; }
      
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SensorsData.db");
    }

}
