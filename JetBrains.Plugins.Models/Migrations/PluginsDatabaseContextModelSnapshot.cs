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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JetBrains.Plugins.Models.Migrations
{
    [DbContext(typeof(PluginsDatabaseContext))]
    partial class PluginsDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("JetBrains.Plugins.Models.Plugin", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CategoryID");

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

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginCategory", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginRelease", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ChangeNotes")
                        .IsRequired();

                    b.Property<List<string>>("Dependencies")
                        .IsRequired();

                    b.Property<long>("Downloads");

                    b.Property<string>("Hash")
                        .IsRequired();

                    b.Property<long>("PluginID");

                    b.Property<long>("Size");

                    b.Property<DateTime>("UploadedAt");

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("PluginID");

                    b.ToTable("PluginRelease");
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.Plugin", b =>
                {
                    b.HasOne("JetBrains.Plugins.Models.PluginCategory", "Category")
                        .WithMany("Plugins")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("JetBrains.Plugins.Models.Vendor", "Vendor", b1 =>
                        {
                            b1.Property<long>("PluginID");

                            b1.Property<string>("Email");

                            b1.Property<string>("Name");

                            b1.Property<string>("URL");

                            b1.HasKey("PluginID");

                            b1.ToTable("Plugins");

                            b1.HasOne("JetBrains.Plugins.Models.Plugin")
                                .WithOne("Vendor")
                                .HasForeignKey("JetBrains.Plugins.Models.Vendor", "PluginID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("JetBrains.Plugins.Models.PluginRelease", b =>
                {
                    b.HasOne("JetBrains.Plugins.Models.Plugin", "Plugin")
                        .WithMany("Releases")
                        .HasForeignKey("PluginID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("JetBrains.Plugins.Models.IDEVersionRange", "CompatibleWith", b1 =>
                        {
                            b1.Property<long>("PluginReleaseID");

                            b1.HasKey("PluginReleaseID");

                            b1.ToTable("PluginRelease");

                            b1.HasOne("JetBrains.Plugins.Models.PluginRelease")
                                .WithOne("CompatibleWith")
                                .HasForeignKey("JetBrains.Plugins.Models.IDEVersionRange", "PluginReleaseID")
                                .OnDelete(DeleteBehavior.Cascade);

                            b1.OwnsOne("JetBrains.Plugins.Models.IDEVersion", "SinceBuild", b2 =>
                                {
                                    b2.Property<long>("IDEVersionRangePluginReleaseID");

                                    b2.Property<int>("Branch");

                                    b2.Property<int?>("Build");

                                    b2.Property<List<int>>("Extra");

                                    b2.Property<string>("ProductID");

                                    b2.HasKey("IDEVersionRangePluginReleaseID");

                                    b2.ToTable("PluginRelease");

                                    b2.HasOne("JetBrains.Plugins.Models.IDEVersionRange")
                                        .WithOne("SinceBuild")
                                        .HasForeignKey("JetBrains.Plugins.Models.IDEVersion", "IDEVersionRangePluginReleaseID")
                                        .OnDelete(DeleteBehavior.Cascade);
                                });

                            b1.OwnsOne("JetBrains.Plugins.Models.IDEVersion", "UntilBuild", b2 =>
                                {
                                    b2.Property<long>("IDEVersionRangePluginReleaseID");

                                    b2.Property<int>("Branch");

                                    b2.Property<int?>("Build");

                                    b2.Property<List<int>>("Extra");

                                    b2.Property<string>("ProductID");

                                    b2.HasKey("IDEVersionRangePluginReleaseID");

                                    b2.ToTable("PluginRelease");

                                    b2.HasOne("JetBrains.Plugins.Models.IDEVersionRange")
                                        .WithOne("UntilBuild")
                                        .HasForeignKey("JetBrains.Plugins.Models.IDEVersion", "IDEVersionRangePluginReleaseID")
                                        .OnDelete(DeleteBehavior.Cascade);
                                });
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
