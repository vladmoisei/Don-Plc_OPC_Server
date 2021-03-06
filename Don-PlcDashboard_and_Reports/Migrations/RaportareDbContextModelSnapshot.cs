﻿// <auto-generated />
using Don_PlcDashboard_and_Reports.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Don_PlcDashboard_and_Reports.Migrations
{
    [DbContext(typeof(RaportareDbContext))]
    partial class RaportareDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Don_PlcDashboard_and_Reports.Models.PlcModel", b =>
                {
                    b.Property<int>("PlcModelID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CpuType")
                        .HasColumnType("int");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("nvarchar(15)")
                        .HasMaxLength(15);

                    b.Property<bool>("IsEnable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<short>("Rack")
                        .HasColumnType("smallint");

                    b.Property<short>("Slot")
                        .HasColumnType("smallint");

                    b.HasKey("PlcModelID");

                    b.ToTable("Plcs");
                });

            modelBuilder.Entity("Don_PlcDashboard_and_Reports.Models.TagModel", b =>
                {
                    b.Property<int>("TagID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("DataType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("PlcModelID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("TagID");

                    b.HasIndex("PlcModelID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Don_PlcDashboard_and_Reports.Models.TagModel", b =>
                {
                    b.HasOne("Don_PlcDashboard_and_Reports.Models.PlcModel", "PlcModel")
                        .WithMany("TagsList")
                        .HasForeignKey("PlcModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
