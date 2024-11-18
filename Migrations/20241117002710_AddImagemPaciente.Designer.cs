﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SQLApp.Data;

#nullable disable

namespace ehr_csharp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241117002710_AddImagemPaciente")]
    partial class AddImagemPaciente
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

            modelBuilder.Entity("ehr_csharp.Models.Anexo", b =>
                {
                    b.Property<int>("IdAnexo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAnexo"));

                    b.Property<byte[]>("ArquivoData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("Ativo")
                        .HasColumnType("bit");

                    b.Property<string>("IdTabelaReferencia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NmTabelaReferencia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomeArquivo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TipoArquivo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdAnexo");

                    b.ToTable("Anexo");
                });

            modelBuilder.Entity("ehr_csharp.Models.Antecedente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PacienteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PacienteId");

                    b.ToTable("Antecedente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Consulta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Anamnese")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExameFisico")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExamesSolicitados")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HipoteseDiagnostica")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HistoricoDoencaAtual")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdMedico")
                        .HasColumnType("int");

                    b.Property<int>("IdPaciente")
                        .HasColumnType("int");

                    b.Property<string>("Motivo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MotivoCancelamento")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Observacoes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Orientacoes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prescricao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QueixaPrincipal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RetornoConsulta")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusConsulta")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdMedico");

                    b.HasIndex("IdPaciente");

                    b.ToTable("Consulta");
                });

            modelBuilder.Entity("ehr_csharp.Models.Diagnostico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CID10")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Especialidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdMedico")
                        .HasColumnType("int");

                    b.Property<int>("IdPaciente")
                        .HasColumnType("int");

                    b.Property<int>("MedicoId")
                        .HasColumnType("int");

                    b.Property<int>("PacienteId")
                        .HasColumnType("int");

                    b.Property<string>("Patologia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MedicoId");

                    b.HasIndex("PacienteId");

                    b.ToTable("Diagnostico");
                });

            modelBuilder.Entity("ehr_csharp.Models.Especialidade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Especialidade");
                });

            modelBuilder.Entity("ehr_csharp.Models.Exame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HemogramaId")
                        .HasColumnType("int");

                    b.Property<int>("IdHemograma")
                        .HasColumnType("int");

                    b.Property<int>("IdMedico")
                        .HasColumnType("int");

                    b.Property<int>("IdPaciente")
                        .HasColumnType("int");

                    b.Property<int>("MedicoId")
                        .HasColumnType("int");

                    b.Property<int>("PacienteId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Validade")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("HemogramaId");

                    b.HasIndex("MedicoId");

                    b.HasIndex("PacienteId");

                    b.ToTable("Exame");
                });

            modelBuilder.Entity("ehr_csharp.Models.Hemograma", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AcidoUrico")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Basofilos_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Basofilos_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bastonetes_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bastonetes_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CHCM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColesterolTotal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Creatinina")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Eosinofilos_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Eosinofilos_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Eritrocitos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Glicemia")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HCM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HDL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hematocrito")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hemoglobina")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdConsulta")
                        .HasColumnType("int");

                    b.Property<string>("LDL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Leucocitos_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Leucocitos_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Linfocitos_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Linfocitos_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Monocitos_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Monocitos_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NaoHDL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Plaquetas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prolactina")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RDW")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Segmentados_Absoluto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Segmentados_Relativo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Testosterona")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Triglicerides")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VCM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VPM")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IdConsulta");

                    b.ToTable("Hemograma");
                });

            modelBuilder.Entity("ehr_csharp.Models.Medicamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Dosagem")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Duracao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdPrescricao")
                        .HasColumnType("int");

                    b.Property<string>("Intervalo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Medicacao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PrescricaoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PrescricaoId");

                    b.ToTable("Medicamento");
                });

            modelBuilder.Entity("ehr_csharp.Models.Medico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CRM")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdEspecialidade")
                        .HasColumnType("int");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("IdEspecialidade");

                    b.HasIndex("IdUsuario")
                        .IsUnique();

                    b.ToTable("Medico");
                });

            modelBuilder.Entity("ehr_csharp.Models.Paciente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float?>("Altura")
                        .HasColumnType("real");

                    b.Property<bool>("Ativo")
                        .HasColumnType("bit");

                    b.Property<string>("Carteirinha")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Celular")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("DataCadastro")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DataNascimento")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EstadoCivil")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HistoricoFamiliar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagemBase64")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Peso")
                        .HasColumnType("real");

                    b.Property<string>("Profissao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sexo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelefoneFixo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Prescricao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("IdMedico")
                        .HasColumnType("int");

                    b.Property<int>("IdPaciente")
                        .HasColumnType("int");

                    b.Property<int>("MedicoId")
                        .HasColumnType("int");

                    b.Property<int>("PacienteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MedicoId");

                    b.HasIndex("PacienteId");

                    b.ToTable("Prescricao");
                });

            modelBuilder.Entity("ehr_csharp.Models.Usuario", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ImageByteStr")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<DateTime>("UpdateAt")
                        .HasColumnType("datetime2");

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
                    b.HasOne("ehr_csharp.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ehr_csharp.Models.Usuario", null)
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

                    b.HasOne("ehr_csharp.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ehr_csharp.Models.Usuario", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ehr_csharp.Models.Antecedente", b =>
                {
                    b.HasOne("ehr_csharp.Models.Paciente", "Paciente")
                        .WithMany("Antecedentes")
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Consulta", b =>
                {
                    b.HasOne("ehr_csharp.Models.Medico", "Medico")
                        .WithMany("Consultas")
                        .HasForeignKey("IdMedico")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Paciente", "Paciente")
                        .WithMany("Consultas")
                        .HasForeignKey("IdPaciente")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medico");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Diagnostico", b =>
                {
                    b.HasOne("ehr_csharp.Models.Medico", "Medico")
                        .WithMany("Diagnosticos")
                        .HasForeignKey("MedicoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Paciente", "Paciente")
                        .WithMany("Diagnosticos")
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medico");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Exame", b =>
                {
                    b.HasOne("ehr_csharp.Models.Hemograma", "Hemograma")
                        .WithMany()
                        .HasForeignKey("HemogramaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Medico", "Medico")
                        .WithMany()
                        .HasForeignKey("MedicoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Paciente", "Paciente")
                        .WithMany()
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hemograma");

                    b.Navigation("Medico");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Hemograma", b =>
                {
                    b.HasOne("ehr_csharp.Models.Consulta", "Consulta")
                        .WithMany()
                        .HasForeignKey("IdConsulta")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Consulta");
                });

            modelBuilder.Entity("ehr_csharp.Models.Medicamento", b =>
                {
                    b.HasOne("ehr_csharp.Models.Prescricao", "Prescricao")
                        .WithMany("Medicamentos")
                        .HasForeignKey("PrescricaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Prescricao");
                });

            modelBuilder.Entity("ehr_csharp.Models.Medico", b =>
                {
                    b.HasOne("ehr_csharp.Models.Especialidade", "Especialidade")
                        .WithMany("Medicos")
                        .HasForeignKey("IdEspecialidade")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Usuario", "Usuario")
                        .WithOne("Medico")
                        .HasForeignKey("ehr_csharp.Models.Medico", "IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Especialidade");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ehr_csharp.Models.Prescricao", b =>
                {
                    b.HasOne("ehr_csharp.Models.Medico", "Medico")
                        .WithMany("Prescricoes")
                        .HasForeignKey("MedicoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ehr_csharp.Models.Paciente", "Paciente")
                        .WithMany("Prescricoes")
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Medico");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("ehr_csharp.Models.Especialidade", b =>
                {
                    b.Navigation("Medicos");
                });

            modelBuilder.Entity("ehr_csharp.Models.Medico", b =>
                {
                    b.Navigation("Consultas");

                    b.Navigation("Diagnosticos");

                    b.Navigation("Prescricoes");
                });

            modelBuilder.Entity("ehr_csharp.Models.Paciente", b =>
                {
                    b.Navigation("Antecedentes");

                    b.Navigation("Consultas");

                    b.Navigation("Diagnosticos");

                    b.Navigation("Prescricoes");
                });

            modelBuilder.Entity("ehr_csharp.Models.Prescricao", b =>
                {
                    b.Navigation("Medicamentos");
                });

            modelBuilder.Entity("ehr_csharp.Models.Usuario", b =>
                {
                    b.Navigation("Medico");
                });
#pragma warning restore 612, 618
        }
    }
}
