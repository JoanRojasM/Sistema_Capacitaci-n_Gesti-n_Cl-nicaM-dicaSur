﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using scg_clinicasur.Data;

#nullable disable

namespace scg_clinicasur.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("scg_clinicasur.Models.Capacitacion", b =>
                {
                    b.Property<int>("id_capacitacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id_capacitacion"));

                    b.Property<string>("archivo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<TimeSpan>("duracion")
                        .HasColumnType("time");

                    b.Property<string>("estado")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("fecha_creacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<int>("id_usuario")
                        .HasColumnType("int");

                    b.Property<string>("titulo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("id_capacitacion");

                    b.HasIndex("id_usuario");

                    b.ToTable("Capacitaciones");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Evaluacion", b =>
                {
                    b.Property<int>("id_evaluacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id_evaluacion"));

                    b.Property<string>("archivo")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("fecha_creacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<int>("id_usuario")
                        .HasColumnType("int");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<TimeSpan>("tiempo_prueba")
                        .HasColumnType("time");

                    b.HasKey("id_evaluacion");

                    b.HasIndex("id_usuario");

                    b.ToTable("Evaluaciones");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Expediente", b =>
                {
                    b.Property<int>("idExpediente")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_expediente");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idExpediente"));

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)")
                        .HasColumnName("descripcion");

                    b.Property<string>("diagnostico")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("diagnostico");

                    b.Property<DateTime>("fechaCreacion")
                        .HasColumnType("datetime2")
                        .HasColumnName("fecha_creacion");

                    b.Property<DateTime>("fechaNacimiento")
                        .HasColumnType("datetime2")
                        .HasColumnName("fecha_nacimiento");

                    b.Property<int>("idPaciente")
                        .HasColumnType("int")
                        .HasColumnName("id_paciente");

                    b.Property<string>("nombrePaciente")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("nombre_paciente");

                    b.Property<string>("tratamientosPrevios")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)")
                        .HasColumnName("tratamientos_previos");

                    b.Property<DateTime>("ultimaConsulta")
                        .HasColumnType("datetime2")
                        .HasColumnName("ultima_consulta");

                    b.HasKey("idExpediente");

                    b.ToTable("Expedientes");
                });

            modelBuilder.Entity("scg_clinicasur.Models.ResultadosLaboratorio", b =>
                {
                    b.Property<int>("IdResultado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("IdResultado");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdResultado"));

                    b.Property<DateTime>("FechaPrueba")
                        .HasColumnType("datetime2")
                        .HasColumnName("FechaPrueba");

                    b.Property<int>("IdExpediente")
                        .HasColumnType("int")
                        .HasColumnName("IdExpediente");

                    b.Property<int>("IdPaciente")
                        .HasColumnType("int")
                        .HasColumnName("IdPaciente");

                    b.Property<string>("Resultado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Resultado");

                    b.Property<string>("TipoPrueba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TipoPrueba");

                    b.HasKey("IdResultado");

                    b.HasIndex("IdExpediente");

                    b.HasIndex("IdPaciente");

                    b.ToTable("resultados_laboratorio");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Roles", b =>
                {
                    b.Property<int>("id_rol")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id_rol"));

                    b.Property<string>("nombre_rol")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("id_rol");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Usuario", b =>
                {
                    b.Property<int>("id_usuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id_usuario"));

                    b.Property<string>("apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("contraseña")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("correo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("estado")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("fecha_registro")
                        .HasColumnType("datetime2");

                    b.Property<int>("id_rol")
                        .HasColumnType("int");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("telefono")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("id_usuario");

                    b.HasIndex("id_rol");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Capacitacion", b =>
                {
                    b.HasOne("scg_clinicasur.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("id_usuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Evaluacion", b =>
                {
                    b.HasOne("scg_clinicasur.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("id_usuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("scg_clinicasur.Models.ResultadosLaboratorio", b =>
                {
                    b.HasOne("scg_clinicasur.Models.Expediente", "Expediente")
                        .WithMany()
                        .HasForeignKey("IdExpediente")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("scg_clinicasur.Models.Usuario", "Paciente")
                        .WithMany()
                        .HasForeignKey("IdPaciente")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Expediente");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("scg_clinicasur.Models.Usuario", b =>
                {
                    b.HasOne("scg_clinicasur.Models.Roles", "roles")
                        .WithMany()
                        .HasForeignKey("id_rol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("roles");
                });
#pragma warning restore 612, 618
        }
    }
}
