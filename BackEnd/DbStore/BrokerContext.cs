using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BackEnd.DbStore.Relationship;
using BackEnd.DbStore.Entity;

namespace BackEnd.DbStore
{
    public class BrokerContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<TopicEntity> Topics { get; set; }
        public DbSet<UserTopic> UserTopics { get; set; }
        public BrokerContext(DbContextOptions<BrokerContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>().ToTable("Users");
            modelBuilder.Entity<UserEntity>().Property(u => u.UserName).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(u => u.Password).IsRequired();
            modelBuilder.Entity<UserEntity>().HasKey(u => u.Id);
            modelBuilder.Entity<UserEntity>().Property(u => u.Id).ValueGeneratedOnAdd();


            modelBuilder.Entity<MessageEntity>().ToTable("Messages");
            modelBuilder.Entity<MessageEntity>().Property(m => m.Message).IsRequired();
            modelBuilder.Entity<MessageEntity>().Property(m => m.Timestamp).IsRequired();
            modelBuilder.Entity<MessageEntity>().Property(m => m.TopicName).IsRequired();
            modelBuilder.Entity<MessageEntity>().HasKey(m => m.Id);
            modelBuilder.Entity<MessageEntity>().Property(m => m.Id).ValueGeneratedOnAdd();
			modelBuilder.Entity<MessageEntity>()
				.HasOne<TopicEntity>()
				.WithMany()
				.HasForeignKey(m => m.TopicName)
				.IsRequired()
				.OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<TopicEntity>().ToTable("Topics");
			modelBuilder.Entity<TopicEntity>().HasKey(t => t.Topic);
			modelBuilder.Entity<TopicEntity>().Property(t => t.Description);

            modelBuilder.Entity<UserTopic>().ToTable("UserTopics");
            modelBuilder.Entity<UserTopic>().HasKey(ut => new { ut.UserId, ut.TopicName });
            modelBuilder.Entity<UserTopic>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTopics)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserTopic>()
                .HasOne(ut => ut.Topic)
                .WithMany(t => t.UserTopics)
                .HasForeignKey(ut => ut.TopicName)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
