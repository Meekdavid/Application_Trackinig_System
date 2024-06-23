using ATS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ATS.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<JobPostRecruiter> JobPostRecruiters { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<R2Response> R2Details { get; set; }
        public DbSet<MainR2Questions> JobPostQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the relationship between JobPost and ApplicationUser (CreatedBy)
            builder.Entity<MainR2Questions>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Question)
                      .HasMaxLength(500);

                entity.HasOne(e => e.JobPost)
                      .WithMany(j => j.MainR2Questions)
                      .HasForeignKey(e => e.JobPostId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<JobPost>()
                .HasOne(j => j.CreatedBy)
                .WithMany()
                .HasForeignKey(j => j.CreatedById);

            // Configure the one-to-many relationship between JobPost and R2Response
            builder.Entity<JobPost>()
                .HasMany(j => j.R2Questions)
                .WithOne(r => r.JobPost)
                .HasForeignKey(r => r.JobPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationships and constraints for R2Response
            builder.Entity<R2Response>()
                .HasOne(r => r.JobPost)
                .WithMany(j => j.R2Questions)
                .HasForeignKey(r => r.JobPostId);

            builder.Entity<R2Response>()
                .HasOne(r => r.Application)
                .WithMany(a => a.R2Responses)
                .HasForeignKey(r => r.ApplicationId);

            // Configure the relationship between Application and JobPost
            builder.Entity<Application>()
                .HasOne(a => a.JobPost)
                .WithMany()
                .HasForeignKey(a => a.JobPostId);

            // Configure the relationship between Application and ApplicationUser (Candidate)
            builder.Entity<Application>()
                .HasOne(a => a.Candidate)
                .WithMany()
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

        //    builder.Entity<Application>()
        //.Property(a => a.CandidateId)
        //    .IsRequired();

        //    builder.Entity<Application>()
        //        .Property(a => a.ResumeBase64)
        //        .IsRequired(false);

            // Configure the relationship between JobPostRecruiter and ApplicationUser (Recruiter)
            builder.Entity<JobPostRecruiter>()
                .HasOne(jr => jr.Recruiter)
                .WithMany()
                .HasForeignKey(jr => jr.RecruiterId);

            // Configure the relationship between Education and ApplicationUser
            builder.Entity<Education>()
                .HasOne(e => e.User)
                .WithMany(u => u.Educations)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            // Configure the relationship between Experience and ApplicationUser
            builder.Entity<Experience>()
                .HasOne(e => e.User)
                .WithMany(u => u.Experiences)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            // Seed roles
            const string cordinatorRoleId = "1";
            const string employerRoleId = "2";
            const string recruiterRoleId = "3";
            const string candidateRoleId = "4";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = cordinatorRoleId, Name = "Coordinator", NormalizedName = "COORDINATOR" },
                new IdentityRole { Id = employerRoleId, Name = "Employer", NormalizedName = "EMPLOYER" },
                new IdentityRole { Id = recruiterRoleId, Name = "Recruiter", NormalizedName = "RECRUITER" },
                new IdentityRole { Id = candidateRoleId, Name = "Candidate", NormalizedName = "CANDIDATE" }
            );
        }
    }
}
