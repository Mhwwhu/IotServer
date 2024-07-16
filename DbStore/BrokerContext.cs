using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using DbStore.Entity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DbStore.Relationship;

namespace DbStore
{
	public class BrokerContext : DbContext
	{
		public DbSet<UserEntity> Users { get; set; }
		public DbSet<MessageEntity> Messages { get; set; }
		public DbSet<TopicEntity> Topic { get; set; }
		public BrokerContext(DbContextOptions<BrokerContext> options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserEntity>().ToTable("Users");
			modelBuilder.Entity<UserEntity>().Property(u =>  u.UserName).IsRequired();
			modelBuilder.Entity<UserEntity>().Property(u => u.Password).IsRequired();
			modelBuilder.Entity<UserEntity>().HasKey(u => u.UserName);
			modelBuilder.Entity<UserEntity>()
				.HasMany(u => u.Subscribes)
				.WithMany(t => t.Subscribers)
				.UsingEntity(ut => ut.ToTable("UserTopic"));

			modelBuilder.Entity<MessageEntity>().ToTable("Messages");
			modelBuilder.Entity<MessageEntity>().Property(m => m.Message).IsRequired();
			modelBuilder.Entity<MessageEntity>().HasKey(m => new { m.Timestamp, m.Topic });

			modelBuilder.Entity<TopicEntity>().ToTable("Topics");
			modelBuilder.Entity<TopicEntity>().HasKey(t => t.Topic);
		}
	}
}
