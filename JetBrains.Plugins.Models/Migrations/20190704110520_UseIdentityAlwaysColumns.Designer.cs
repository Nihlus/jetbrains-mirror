﻿// <auto-generated />
#pragma warning disable CS1591
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using JetBrains.Plugins.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JetBrains.Plugins.Models.Migrations
{
    [DbContext(typeof(PluginsDatabaseContext))]
    [Migration("20190704110520_UseIdentityAlwaysColumns")]
    partial class UseIdentityAlwaysColumns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("JetBrains.Plugins.Models.Plugin", b =>
                {
                    b.Property<decimal>("ID")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("CategoryID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PluginID")
                        .IsRequired();

                    b.Property<string>("ProjectURL")
                        .IsRequired();

                    b.Property<double>("Rating");

                    b.Property<List<string>>("Tags")
                        .IsRequired();

                    b.Property<decimal>("VendorID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("VendorID");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginCategory", b =>
                {
                    b.Property<decimal>("ID")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginDependency", b =>
                {
                    b.Property<decimal>("ID")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("DependencyID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("DependerID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal?>("PluginReleaseID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.HasKey("ID");

                    b.HasIndex("DependencyID");

                    b.HasIndex("DependerID");

                    b.HasIndex("PluginReleaseID");

                    b.ToTable("PluginDependency");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginRelease", b =>
                {
                    b.Property<decimal>("ID")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("ChangeNotes")
                        .IsRequired();

                    b.Property<decimal>("Downloads")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Hash")
                        .IsRequired();

                    b.Property<decimal>("PluginID")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("Size")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<DateTime>("UploadedAt");

                    b.HasKey("ID");

                    b.HasIndex("PluginID");

                    b.ToTable("PluginRelease");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.Vendor", b =>
                {
                    b.Property<decimal>("ID")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("URL");

                    b.HasKey("ID");

                    b.ToTable("Vendor");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.Plugin", b =>
                {
                    b.HasOne("JetBrains.Plugins.Models.PluginCategory", "Category")
                        .WithMany("Plugins")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JetBrains.Plugins.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginDependency", b =>
                {
                    b.HasOne("JetBrains.Plugins.Models.Plugin", "Dependency")
                        .WithMany()
                        .HasForeignKey("DependencyID");

                    b.HasOne("JetBrains.Plugins.Models.Plugin", "Depender")
                        .WithMany()
                        .HasForeignKey("DependerID");

                    b.HasOne("JetBrains.Plugins.Models.PluginRelease")
                        .WithMany("Dependencies")
                        .HasForeignKey("PluginReleaseID");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginRelease", b =>
                {
                    b.HasOne("JetBrains.Plugins.Models.Plugin", "Plugin")
                        .WithMany("Releases")
                        .HasForeignKey("PluginID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("JetBrains.Plugins.Models.IDEVersionRange", "CompatibleWith", b1 =>
                        {
                            b1.Property<decimal>("PluginReleaseID")
                                .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                            b1.HasKey("PluginReleaseID");

                            b1.ToTable("PluginRelease");

                            b1.HasOne("JetBrains.Plugins.Models.PluginRelease")
                                .WithOne("CompatibleWith")
                                .HasForeignKey("JetBrains.Plugins.Models.IDEVersionRange", "PluginReleaseID")
                                .OnDelete(DeleteBehavior.Cascade);

                            b1.OwnsOne("JetBrains.Plugins.Models.IDEVersion", "SinceBuild", b2 =>
                                {
                                    b2.Property<decimal>("IDEVersionRangePluginReleaseID")
                                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                                    b2.Property<int>("Branch");

                                    b2.Property<int?>("Build");

                                    b2.Property<List<int>>("Extra");

                                    b2.Property<string>("ProductID")
                                        .IsRequired();

                                    b2.HasKey("IDEVersionRangePluginReleaseID");

                                    b2.ToTable("PluginRelease");

                                    b2.HasOne("JetBrains.Plugins.Models.IDEVersionRange")
                                        .WithOne("SinceBuild")
                                        .HasForeignKey("JetBrains.Plugins.Models.IDEVersion", "IDEVersionRangePluginReleaseID")
                                        .OnDelete(DeleteBehavior.Cascade);
                                });

                            b1.OwnsOne("JetBrains.Plugins.Models.IDEVersion", "UntilBuild", b2 =>
                                {
                                    b2.Property<decimal>("IDEVersionRangePluginReleaseID")
                                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                                    b2.Property<int>("Branch");

                                    b2.Property<int?>("Build");

                                    b2.Property<List<int>>("Extra");

                                    b2.Property<string>("ProductID")
                                        .IsRequired();

                                    b2.HasKey("IDEVersionRangePluginReleaseID");

                                    b2.ToTable("PluginRelease");

                                    b2.HasOne("JetBrains.Plugins.Models.IDEVersionRange")
                                        .WithOne("UntilBuild")
                                        .HasForeignKey("JetBrains.Plugins.Models.IDEVersion", "IDEVersionRangePluginReleaseID")
                                        .OnDelete(DeleteBehavior.Cascade);
                                });
                        });

                    b.OwnsOne("JetBrains.Plugins.Models.PluginVersion", "Version", b1 =>
                        {
                            b1.Property<decimal>("PluginReleaseID")
                                .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                            b1.Property<string>("Extra");

                            b1.Property<int>("Major");

                            b1.Property<int>("Minor");

                            b1.Property<int>("Patch");

                            b1.HasKey("PluginReleaseID");

                            b1.ToTable("PluginRelease");

                            b1.HasOne("JetBrains.Plugins.Models.PluginRelease")
                                .WithOne("Version")
                                .HasForeignKey("JetBrains.Plugins.Models.PluginVersion", "PluginReleaseID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
