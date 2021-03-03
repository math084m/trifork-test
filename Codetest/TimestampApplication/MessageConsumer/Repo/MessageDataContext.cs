using System;
using MessageConsumer.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MessageConsumer.Repo
{
    public class MessageDataContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=mssql-server-db,1433;Database=MessageDB;User Id=sa;Password=yourStrong(!)Password;");
        }
    }
}