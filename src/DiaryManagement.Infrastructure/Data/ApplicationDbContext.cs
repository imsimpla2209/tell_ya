using DiaryManagement.Core.Entities;
using DiaryManagement.Infrastructure.Extentions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DiaryManagement.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Diary> Diarys { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WriterDiary> WriterDiarys { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            EntityReference.DuplicateIndex(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("workstation id=diarymanagement.mssql.somee.com;packet size=4096;user id=hoangnguyensp_SQLLogin_1;pwd=qb3e3gej43;data source=diarymanagement.mssql.somee.com;persist security info=False;initial catalog=diarymanagement;TrustServerCertificate=True");
        }
    }
}
