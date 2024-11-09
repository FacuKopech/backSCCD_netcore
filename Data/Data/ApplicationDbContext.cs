using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Model.Entities;
using System.Diagnostics.Metrics;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private ApplicationDbContext() { }

        private static ApplicationDbContext _instance;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public static ApplicationDbContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ApplicationDbContext();
            }
            return _instance;
        }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Institucion> Instituciones { get; set; }
        public virtual DbSet<Aula> Aulas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Nota> Notas { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<Historial> Historiales { get; set; }
        public virtual DbSet<Ausencia> Ausencias { get; set; }
        public virtual DbSet<Asistencia> AsistenciasTomadas { get; set; }
        public virtual DbSet<AsistenciaAlumno> AsistenciaAlumno { get; set; }
        public virtual DbSet<LoginAudit> LoginAudit { get; set; }
        public virtual DbSet<HistorialAudit> HistorialAudit { get; set; }

        public virtual DbSet<Evento> Eventos { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS,1433; Initial Catalog=SCC;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Persona>().HasOne(persona => persona.Usuario);

            modelBuilder.Entity<Usuario>().HasMany(grupos => grupos.Grupos)
            .WithMany(usuarios => usuarios.Usuarios);

            modelBuilder.Entity<LoginAudit>().HasOne(usuario => usuario.UsuarioLogueado);            

            modelBuilder.Entity<Admin>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Admin>("Admin");

            modelBuilder.Entity<Nota>().HasOne(nota => nota.Emisor);

            modelBuilder.Entity<Nota>()
                .HasMany(nota => nota.Destinatarios)
                .WithMany(p => p.NotasRecibidas);

            modelBuilder.Entity<Nota>()
               .HasMany(nota => nota.LeidaPor)
               .WithMany(p => p.NotasLeidas);

            modelBuilder.Entity<Nota>()
               .HasMany(nota => nota.FirmadaPor)
               .WithMany(p => p.NotasFirmadas);

            modelBuilder.Entity<Nota>().HasMany(nota => nota.AulasDestinadas).WithMany(aulas => aulas.NotasParaAula);

            modelBuilder.Entity<Evento>()
                .HasMany(asistira => asistira.Asistiran)
                .WithMany(eventosAsistire => eventosAsistire.EventosAsistire);

            modelBuilder.Entity<Evento>()
                .HasMany(noAsistira => noAsistira.NoAsistiran)
                .WithMany(eventosNoAsistire => eventosNoAsistire.EventosNoAsistire);

            modelBuilder.Entity<Evento>()
                .HasMany(talVezAsistan => talVezAsistan.TalVezAsistan)
                .WithMany(eventosTalVezAsista => eventosTalVezAsista.EventosTalVezAsista);

            modelBuilder.Entity<Evento>()
                .HasOne(aula => aula.AulaDestinada)
                .WithMany(eventos => eventos.Eventos);

            modelBuilder.Entity<Evento>()
                .HasOne(creador => creador.Creador);

            modelBuilder.Entity<Padre>().HasMany(padre => padre.Hijos);
            modelBuilder.Entity<Docente>().HasMany(docente => docente.Hijos);
            modelBuilder.Entity<Directivo>().HasMany(directivo => directivo.Hijos);            
            
            modelBuilder.Entity<Alumno>().HasMany(alumno => alumno.Historiales);
            modelBuilder.Entity<Alumno>().HasMany(asistenciaAlumno => asistenciaAlumno.Asistencias);

            modelBuilder.Entity<Ausencia>().HasMany(hijos => hijos.HijosConAusencia).WithMany(ausenciasDeAlumno => ausenciasDeAlumno.Ausencias);

            modelBuilder.Entity<Aula>().HasMany(alumnos => alumnos.Alumnos);
            modelBuilder.Entity<Aula>().HasMany(asistencia => asistencia.Asistencias);
            modelBuilder.Entity<Aula>().HasOne(docente => docente.Docente);

            modelBuilder.Entity<AsistenciaAlumno>().HasKey(aa => new { aa.AsistenciaId, aa.AlumnoId });
            modelBuilder.Entity<Asistencia>().HasMany(asistenciaAlumno => asistenciaAlumno.AsistenciaAlumno);                        
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS,1433; Initial Catalog=SCC;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}