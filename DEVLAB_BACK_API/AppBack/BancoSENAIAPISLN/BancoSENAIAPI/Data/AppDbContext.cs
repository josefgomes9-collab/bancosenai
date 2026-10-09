using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;
<<<<<<< Updated upstream

namespace BancoSENAIAPI.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Agencia> Agencia => Set<Agencia>();


    }
}
=======
using System.Collections.Generic;

namespace BancoSENAIAPI.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Agencia> Agencia => Set<Agencia>();
        public DbSet<Carteira> Carteira => Set<Carteira>();
        public DbSet<Cliente> Cliente => Set<Cliente>();
        public DbSet<DocumentoMetadados> Documento => Set<DocumentoMetadados>();
        public DbSet<Usuario> Usuario => Set<Usuario>();
    }
}
>>>>>>> Stashed changes
