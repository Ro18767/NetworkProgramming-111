using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Http.Data
{
    public class DataContext : DbContext
    {
        public DbSet<NpUser> NpUsers { get; set; }
        //private readonly String connectionString;

        public DataContext(): base()
        {
            //this.connectionString = connectionString;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            // MessageBox.Show(System.IO.File.ReadAllText("emailconfig.json"));
            var emailConfig = JsonSerializer.Deserialize<dynamic>(
                System.IO.File.ReadAllText("emailconfig.json")
                );
            String connectionString = emailConfig.GetProperty("dbms").GetProperty("planetScale").GetString();
            optionBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)));
        }
    }
}
