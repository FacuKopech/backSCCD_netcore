﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231008203720_deletedPadreDocenteAndDirectivoEntities")]
    partial class deletedPadreDocenteAndDirectivoEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AccionGrupo", b =>
                {
                    b.Property<int>("AccionesId")
                        .HasColumnType("int");

                    b.Property<int>("GruposId")
                        .HasColumnType("int");

                    b.HasKey("AccionesId", "GruposId");

                    b.HasIndex("GruposId");

                    b.ToTable("AccionGrupo");
                });

            modelBuilder.Entity("AlumnoAusencia", b =>
                {
                    b.Property<int>("AusenciasId")
                        .HasColumnType("int");

                    b.Property<int>("HijosConAusenciaId")
                        .HasColumnType("int");

                    b.HasKey("AusenciasId", "HijosConAusenciaId");

                    b.HasIndex("HijosConAusenciaId");

                    b.ToTable("AlumnoAusencia");
                });

            modelBuilder.Entity("AulaNota", b =>
                {
                    b.Property<int>("AulasDestinadasId")
                        .HasColumnType("int");

                    b.Property<int>("NotasParaAulaId")
                        .HasColumnType("int");

                    b.HasKey("AulasDestinadasId", "NotasParaAulaId");

                    b.HasIndex("NotasParaAulaId");

                    b.ToTable("AulaNota");
                });

            modelBuilder.Entity("GrupoUsuario", b =>
                {
                    b.Property<int>("GruposId")
                        .HasColumnType("int");

                    b.Property<int>("UsuariosId")
                        .HasColumnType("int");

                    b.HasKey("GruposId", "UsuariosId");

                    b.HasIndex("UsuariosId");

                    b.ToTable("GrupoUsuario");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Model.Entities.Accion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Acciones");
                });

            modelBuilder.Entity("Model.Entities.Asistencia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AulaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaAsistenciaTomada")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AulaId");

                    b.ToTable("AsistenciasTomadas");
                });

            modelBuilder.Entity("Model.Entities.AsistenciaAlumno", b =>
                {
                    b.Property<int>("AsistenciaId")
                        .HasColumnType("int");

                    b.Property<int>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AsistenciaId", "AlumnoId");

                    b.HasIndex("AlumnoId");

                    b.ToTable("AsistenciaAlumno");
                });

            modelBuilder.Entity("Model.Entities.Aula", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("CantidadAlumnos")
                        .HasColumnType("int");

                    b.Property<int?>("DocenteId")
                        .HasColumnType("int");

                    b.Property<int>("InstitucionId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DocenteId");

                    b.HasIndex("InstitucionId");

                    b.ToTable("Aulas");
                });

            modelBuilder.Entity("Model.Entities.Ausencia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("FechaComienzo")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaEmision")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("datetime2");

                    b.Property<string>("Justificada")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Motivo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ausencias");
                });

            modelBuilder.Entity("Model.Entities.Grupo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Grupos");
                });

            modelBuilder.Entity("Model.Entities.Historial", b =>
                {
                    b.Property<int>("IdHistorial")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdHistorial"), 1L, 1);

                    b.Property<int?>("AlumnoId")
                        .HasColumnType("int");

                    b.Property<decimal?>("Calificacion")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Firmado")
                        .HasColumnType("bit");

                    b.HasKey("IdHistorial");

                    b.HasIndex("AlumnoId");

                    b.ToTable("Historiales");
                });

            modelBuilder.Entity("Model.Entities.Institucion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Ciudad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Instituciones");
                });

            modelBuilder.Entity("Model.Entities.Nota", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Cuerpo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmisorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("Leida")
                        .HasColumnType("bit");

                    b.Property<int?>("ReferidoId")
                        .HasColumnType("int");

                    b.Property<int>("Tipo")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EmisorId");

                    b.HasIndex("ReferidoId");

                    b.ToTable("Notas");
                });

            modelBuilder.Entity("Model.Entities.Persona", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DNI")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Domicilio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FechaIngreso")
                        .HasColumnType("datetime2");

                    b.Property<int?>("InstitucionId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefono")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InstitucionId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Personas");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Persona");
                });

            modelBuilder.Entity("Model.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("NotaPersona", b =>
                {
                    b.Property<int>("DestinatariosId")
                        .HasColumnType("int");

                    b.Property<int>("NotasRecibidasId")
                        .HasColumnType("int");

                    b.HasKey("DestinatariosId", "NotasRecibidasId");

                    b.HasIndex("NotasRecibidasId");

                    b.ToTable("NotaPersona");
                });

            modelBuilder.Entity("NotaPersona1", b =>
                {
                    b.Property<int>("LeidaPorId")
                        .HasColumnType("int");

                    b.Property<int>("NotasLeidasId")
                        .HasColumnType("int");

                    b.HasKey("LeidaPorId", "NotasLeidasId");

                    b.HasIndex("NotasLeidasId");

                    b.ToTable("NotaPersona1");
                });

            modelBuilder.Entity("NotaPersona2", b =>
                {
                    b.Property<int>("FirmadaPorId")
                        .HasColumnType("int");

                    b.Property<int>("NotasFirmadasId")
                        .HasColumnType("int");

                    b.HasKey("FirmadaPorId", "NotasFirmadasId");

                    b.HasIndex("NotasFirmadasId");

                    b.ToTable("NotaPersona2");
                });

            modelBuilder.Entity("Model.Entities.Alumno", b =>
                {
                    b.HasBaseType("Model.Entities.Persona");

                    b.Property<decimal>("Asistencia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("AulaId")
                        .HasColumnType("int");

                    b.Property<int>("AñoActual")
                        .HasColumnType("int");

                    b.Property<string>("Division")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<string>("Grado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PersonaId")
                        .HasColumnType("int");

                    b.HasIndex("AulaId");

                    b.HasIndex("PersonaId");

                    b.HasDiscriminator().HasValue("Alumno");
                });

            modelBuilder.Entity("AccionGrupo", b =>
                {
                    b.HasOne("Model.Entities.Accion", null)
                        .WithMany()
                        .HasForeignKey("AccionesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Grupo", null)
                        .WithMany()
                        .HasForeignKey("GruposId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlumnoAusencia", b =>
                {
                    b.HasOne("Model.Entities.Ausencia", null)
                        .WithMany()
                        .HasForeignKey("AusenciasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Alumno", null)
                        .WithMany()
                        .HasForeignKey("HijosConAusenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AulaNota", b =>
                {
                    b.HasOne("Model.Entities.Aula", null)
                        .WithMany()
                        .HasForeignKey("AulasDestinadasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Nota", null)
                        .WithMany()
                        .HasForeignKey("NotasParaAulaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GrupoUsuario", b =>
                {
                    b.HasOne("Model.Entities.Grupo", null)
                        .WithMany()
                        .HasForeignKey("GruposId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuariosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Model.Entities.Asistencia", b =>
                {
                    b.HasOne("Model.Entities.Aula", null)
                        .WithMany("Asistencias")
                        .HasForeignKey("AulaId");
                });

            modelBuilder.Entity("Model.Entities.AsistenciaAlumno", b =>
                {
                    b.HasOne("Model.Entities.Alumno", null)
                        .WithMany("Asistencias")
                        .HasForeignKey("AlumnoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Asistencia", null)
                        .WithMany("AsistenciaAlumno")
                        .HasForeignKey("AsistenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Model.Entities.Aula", b =>
                {
                    b.HasOne("Model.Entities.Persona", "Docente")
                        .WithMany()
                        .HasForeignKey("DocenteId");

                    b.HasOne("Model.Entities.Institucion", "Institucion")
                        .WithMany()
                        .HasForeignKey("InstitucionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Docente");

                    b.Navigation("Institucion");
                });

            modelBuilder.Entity("Model.Entities.Historial", b =>
                {
                    b.HasOne("Model.Entities.Alumno", null)
                        .WithMany("Historiales")
                        .HasForeignKey("AlumnoId");
                });

            modelBuilder.Entity("Model.Entities.Nota", b =>
                {
                    b.HasOne("Model.Entities.Persona", "Emisor")
                        .WithMany()
                        .HasForeignKey("EmisorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Alumno", "Referido")
                        .WithMany()
                        .HasForeignKey("ReferidoId");

                    b.Navigation("Emisor");

                    b.Navigation("Referido");
                });

            modelBuilder.Entity("Model.Entities.Persona", b =>
                {
                    b.HasOne("Model.Entities.Institucion", "Institucion")
                        .WithMany()
                        .HasForeignKey("InstitucionId");

                    b.HasOne("Model.Entities.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Institucion");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("NotaPersona", b =>
                {
                    b.HasOne("Model.Entities.Persona", null)
                        .WithMany()
                        .HasForeignKey("DestinatariosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Nota", null)
                        .WithMany()
                        .HasForeignKey("NotasRecibidasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NotaPersona1", b =>
                {
                    b.HasOne("Model.Entities.Persona", null)
                        .WithMany()
                        .HasForeignKey("LeidaPorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Nota", null)
                        .WithMany()
                        .HasForeignKey("NotasLeidasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NotaPersona2", b =>
                {
                    b.HasOne("Model.Entities.Persona", null)
                        .WithMany()
                        .HasForeignKey("FirmadaPorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Entities.Nota", null)
                        .WithMany()
                        .HasForeignKey("NotasFirmadasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Model.Entities.Alumno", b =>
                {
                    b.HasOne("Model.Entities.Aula", null)
                        .WithMany("Alumnos")
                        .HasForeignKey("AulaId");

                    b.HasOne("Model.Entities.Persona", null)
                        .WithMany("Hijos")
                        .HasForeignKey("PersonaId");
                });

            modelBuilder.Entity("Model.Entities.Asistencia", b =>
                {
                    b.Navigation("AsistenciaAlumno");
                });

            modelBuilder.Entity("Model.Entities.Aula", b =>
                {
                    b.Navigation("Alumnos");

                    b.Navigation("Asistencias");
                });

            modelBuilder.Entity("Model.Entities.Persona", b =>
                {
                    b.Navigation("Hijos");
                });

            modelBuilder.Entity("Model.Entities.Alumno", b =>
                {
                    b.Navigation("Asistencias");

                    b.Navigation("Historiales");
                });
#pragma warning restore 612, 618
        }
    }
}
