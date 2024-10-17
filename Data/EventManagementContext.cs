
using DebraSheru.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace DebraSheru.Data;

public class EventManagementContext : DbContext
{
    public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<EventCommission> EventCommissions { get; set; }
    public DbSet<TicketBatch> TicketBatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
    }

}



