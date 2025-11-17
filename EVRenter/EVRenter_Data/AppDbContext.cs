using EVRenter_Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVRenter_Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<RentalPrice> RentalPrices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<ExtraFee> ExtraFee { get; set; }
        public DbSet<FeeType> FeeType { get; set; }
        public DbSet<HandoverAndReturn> HandoverAndReturn { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ModelImage> ModelImages { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Amenities> Amenities { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<CarItem> CarItems { get; set; }
        public DbSet<StaffProfile> StaffProfiles { get; set; }
        public DbSet<RenterProfile> RenterProfiles { get; set; }
        public DbSet<IDImage> IDImages { get; set; }
        public DbSet<DriverLicenseImage> DriverLicenseImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RentalPrice>()
                .HasOne(rp => rp.Model)
                .WithOne(m => m.RentalPrice)
                .HasForeignKey<RentalPrice>(rp => rp.ModelID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.Id);
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany(m => m.Vehicles)
                .HasForeignKey(v => v.ModelID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Station)
                .WithMany(s => s.Vehicles)
                .HasForeignKey(v => v.StationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Vehicle)
                .WithMany(m => m.Bookings)
                .HasForeignKey(v => v.VehicleID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.RenterID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Voucher)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.VoucherID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HandoverAndReturn>()
                .HasKey(hr => hr.Id);
            modelBuilder.Entity<HandoverAndReturn>()
                .HasOne(hr => hr.Booking)
                .WithMany(b => b.HandoverAndReturns)
                .HasForeignKey(hr => hr.BookingID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HandoverAndReturn>()
                .HasOne(hr => hr.User)
                .WithMany(b => b.HandoverAndReturns)
                .HasForeignKey(hr => hr.StaffID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExtraFee>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<ExtraFee>()
                .HasOne(p => p.User)
                .WithMany(b => b.ExtraFees)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ExtraFee>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.ExtraFees)
                .HasForeignKey(p => p.BookingID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FeeType>()
                .HasKey(ft => ft.Id);
            modelBuilder.Entity<FeeType>()
                .HasOne(p => p.ExtraFee)
                .WithMany(b => b.FeeTypes)
                .HasForeignKey(p => p.ExtraFeeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<Feedback>()
                .HasOne(p => p.User)
                .WithMany(b => b.Feedbacks)
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Feedback>()
                .HasOne(p => p.Model)
                .WithMany(b => b.Feedbacks)
                .HasForeignKey(p => p.ModelID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ModelImage>()
                .HasKey(mi => new { mi.ModelID, mi.ImageID });
            modelBuilder.Entity<ModelImage>()
                .HasOne(mi => mi.Model)
                .WithMany(m => m.ModelImages)
                .HasForeignKey(mi => mi.ModelID);
            modelBuilder.Entity<ModelImage>()
                .HasOne(mi => mi.Image)
                .WithMany(m => m.ModelImages)
                .HasForeignKey(mi => mi.ImageID);

            modelBuilder.Entity<User>()
                .Property(u => u.RoleID)
                .HasConversion<int>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentType)
                .HasConversion<int>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Amenities>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Amenities>()
                .HasOne(a => a.Model)
                .WithMany(m => m.Amenities)
                .HasForeignKey(a => a.ModelID);

            modelBuilder.Entity<CarItem>()
                .HasKey(ci => ci.Id);
            modelBuilder.Entity<CarItem>()
                .HasOne(ci => ci.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CategoryID);
            modelBuilder.Entity<CarItem>()
                .HasOne(ci => ci.Vehicle)
                .WithMany(v => v.CarItems)
                .HasForeignKey(ci => ci.VehicleID);

            modelBuilder.Entity<StaffProfile>()
                .HasKey(sp => new { sp.UserID, sp.StationID });
            modelBuilder.Entity<StaffProfile>()
                .HasOne(sp => sp.User)
                .WithOne(u => u.StaffProfile)
                .HasForeignKey<StaffProfile>(sp => sp.UserID);
            modelBuilder.Entity<StaffProfile>()
                .HasOne(sp => sp.Station)
                .WithMany(s => s.StaffProfiles)
                .HasForeignKey(sp => sp.StationID);

            modelBuilder.Entity<RenterProfile>()
                .HasKey(rp => rp.Id);
            modelBuilder.Entity<RenterProfile>()
                .HasOne(rp => rp.User)
                .WithOne(u => u.RenterProfile)
                .HasForeignKey<RenterProfile>(rp => rp.UserID);

            modelBuilder.Entity<IDImage>()
                .HasKey(ii => new { ii.RenterID, ii.ImageID });
            modelBuilder.Entity<IDImage>()
                .HasOne(ii => ii.Profile)
                .WithMany(p => p.IDImages)
                .HasForeignKey(ii => ii.RenterID);
            modelBuilder.Entity<IDImage>()
                .HasOne(ii => ii.Image)
                .WithMany(m => m.IDImages)
                .HasForeignKey(ii => ii.ImageID);

            modelBuilder.Entity<DriverLicenseImage>()
                .HasKey(ii => new { ii.RenterID, ii.ImageID });
            modelBuilder.Entity<DriverLicenseImage>()
                .HasOne(ii => ii.Profile)
                .WithMany(p => p.DriverLicenseImages)
                .HasForeignKey(ii => ii.RenterID);
            modelBuilder.Entity<DriverLicenseImage>()
                .HasOne(ii => ii.Image)
                .WithMany(m => m.DriverLicenseImages)
                .HasForeignKey(ii => ii.ImageID);

            modelBuilder.Entity<ItemCategory>().HasData(
                new ItemCategory { Id = 1, Name = "Ngoại thất" },
                new ItemCategory { Id = 2, Name = "Nội thất" },
                new ItemCategory { Id = 3, Name = "Pin & Kỹ thuật" },
                new ItemCategory { Id = 4, Name = "Phụ kiện" }
                );

            modelBuilder.Entity<IDImage>(entity =>
            {
                entity.HasKey(e => new { e.ImageID, e.ProfileID, e.Type });
                entity.HasOne(e => e.Image)
                      .WithMany(i => i.IDImages)   
                      .HasForeignKey(e => e.ImageID)
                      .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasOne(e => e.Renter)
                      .WithMany(u => u.IDImages)   
                      .HasForeignKey(e => e.RenterID)
                      .OnDelete(DeleteBehavior.Restrict);

               
                entity.HasOne(e => e.Profile)
                      .WithMany(p => p.IDImages)   
                      .HasForeignKey(e => e.ProfileID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DriverLicenseImage>(entity =>
            {
                
                entity.HasKey(e => new { e.ImageID, e.ProfileID, e.Type });

                
                entity.HasOne(e => e.Image)
                      .WithMany(i => i.DriverLicenseImages) 
                      .HasForeignKey(e => e.ImageID)
                      .OnDelete(DeleteBehavior.Cascade);

                
                entity.HasOne(e => e.Renter)
                      .WithMany(u => u.DriverLicenseImages)  
                      .HasForeignKey(e => e.RenterID)
                      .OnDelete(DeleteBehavior.Restrict);

                
                entity.HasOne(e => e.Profile)
                      .WithMany(p => p.DriverLicenseImages)   
                      .HasForeignKey(e => e.ProfileID)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<VehicleImage>()
                .ToTable("VehicleImages");

            modelBuilder.Entity<ModelImage>()
                .ToTable("ModelImages");

            modelBuilder.Entity<IDImage>()
                .ToTable("IDImages");

            modelBuilder.Entity<DriverLicenseImage>()
                .ToTable("DriverLicenseImages");

            modelBuilder.Entity<VehicleImage>()
                .HasKey(vi => new { vi.VehicleID, vi.ImageID });

            modelBuilder.Entity<VehicleImage>()
                .HasOne(vi => vi.Vehicle)
                .WithMany(v => v.VehicleImages)
                .HasForeignKey(vi => vi.VehicleID);

            modelBuilder.Entity<VehicleImage>()
                .HasOne(vi => vi.Image)
                .WithMany(i => i.VehicleImages)
                .HasForeignKey(vi => vi.ImageID);

        }
    }
}
