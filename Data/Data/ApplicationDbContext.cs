using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Model.Entities;
using System.Diagnostics.Metrics;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public Guid ContextId { get; } = Guid.NewGuid();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Institucion> Instituciones { get; set; }
        public virtual DbSet<Aula> Aulas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Nota> Notas { get; set; }
        public virtual DbSet<NotaPersona> NotaPersona { get; set; }
        public virtual DbSet<Grupo> Grupos { get; set; }
        public virtual DbSet<Historial> Historiales { get; set; }
        public virtual DbSet<Ausencia> Ausencias { get; set; }
        public virtual DbSet<Asistencia> AsistenciasTomadas { get; set; }
        public virtual DbSet<AsistenciaAlumno> AsistenciaAlumno { get; set; }
        public virtual DbSet<LoginAudit> LoginAudit { get; set; }
        public virtual DbSet<HistorialAudit> HistorialAudit { get; set; }
        public virtual DbSet<Evento> Eventos { get; set; }
        public virtual DbSet<EventoPersona> EventoPersona { get; set; }

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
            modelBuilder.Entity<Persona>().HasKey(persona => persona.Id);

            modelBuilder.Entity<Usuario>().HasMany(grupos => grupos.Grupos)
            .WithMany(usuarios => usuarios.Usuarios);
            modelBuilder.Entity<Usuario>().HasKey(usuario => usuario.Id);

            modelBuilder.Entity<LoginAudit>().HasOne(usuario => usuario.UsuarioLogueado);
            modelBuilder.Entity<LoginAudit>().HasKey(loginAud => loginAud.Id);

            modelBuilder.Entity<Admin>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Admin>("Admin");

            modelBuilder.Entity<Nota>().HasOne(nota => nota.Emisor);
            modelBuilder.Entity<Nota>().HasKey(nota => nota.Id);

            modelBuilder.Entity<NotaPersona>()
                .HasKey(np => np.Id);

            modelBuilder.Entity<NotaPersona>()
                .HasOne(np => np.Nota)
                .WithMany(n => n.NotaPersonas)
                .HasForeignKey(np => np.NotaId);

            modelBuilder.Entity<NotaPersona>()
                .HasOne(np => np.Persona)
                .WithMany(p => p.NotaPersonas)
                .HasForeignKey(np => np.PersonaId);


            modelBuilder.Entity<Nota>().HasMany(nota => nota.AulasDestinadas).WithMany(aulas => aulas.NotasParaAula);

            modelBuilder.Entity<EventoPersona>()
                .HasKey(ep => ep.Id);

            modelBuilder.Entity<EventoPersona>()
                .HasOne(evento => evento.Evento)
                .WithMany(persona => persona.EventoPersonas)
                .HasForeignKey(evento => evento.EventoId);

            modelBuilder.Entity<EventoPersona>()
                .HasOne(persona => persona.Persona)
                .WithMany(evento => evento.EventosPersona)
                .HasForeignKey(persona => persona.PersonaId);

            modelBuilder.Entity<Evento>().HasKey(evento => evento.Id);

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
            modelBuilder.Entity<Ausencia>().HasKey(ausencia => ausencia.Id);

            modelBuilder.Entity<Aula>().HasMany(alumnos => alumnos.Alumnos);
            modelBuilder.Entity<Aula>().HasMany(asistencia => asistencia.Asistencias);
            modelBuilder.Entity<Aula>().HasOne(docente => docente.Docente);
            modelBuilder.Entity<Aula>().HasKey(aula => aula.Id);

            modelBuilder.Entity<AsistenciaAlumno>().HasKey(aa => new { aa.AsistenciaId, aa.AlumnoId });
            modelBuilder.Entity<Asistencia>().HasMany(asistenciaAlumno => asistenciaAlumno.AsistenciaAlumno);
            modelBuilder.Entity<Asistencia>().HasKey(asistenciaAlumno => asistenciaAlumno.Id);
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