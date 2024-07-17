
using BackEnd.DbStore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace BackEnd.DbStore
{
    public class DbController
    {
        private readonly BrokerContext _context;
        private readonly ILoggerService _logger;
        private readonly string _tag = "DbController";
		private ReaderWriterLockSlim _lock = new();
        public DbController(IConfiguration configuration, ILoggerService logger)
        {
            DbContextOptionsBuilder<BrokerContext> optionsBuilder = new();
            optionsBuilder.UseSqlite(configuration.GetSection("Database")["ConnectionString"]);
			optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
			_context = new BrokerContext(optionsBuilder.Options);
			//_context.Database.Migrate();
			_logger = logger;
            _context.Database.EnsureCreated();
			var topics = _context.Topics.ToList();
			foreach (var topic in topics)
			{
				_context.Entry(topic).State = EntityState.Unchanged;
			}
		}
        public async Task<int> RegisterUserAsync(string userName, string password)
        {
            var user = new UserEntity()
            {
                UserName = userName,
                Password = password
            };
            try
            {
				_lock.EnterWriteLock();
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
				int id = user.Id;
                _logger.Info(_tag, $"User {user.UserName} added successfully.");
				return id;
            }
            catch (Exception ex)
            {
                _logger.Error(_tag, $"Error adding user {user.UserName}: {ex.Message}");
				throw;
            }
			finally
			{
				_lock.ExitWriteLock(); 
			}
        }
        public async Task UnregisterUserAsync(int userId)
        {
            try
            {
				_lock.EnterWriteLock();
                var rmUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (rmUser != null)
                {
                    _context.Users.Remove(rmUser);
                    await _context.SaveChangesAsync();
                    _logger.Info(_tag, $"User {rmUser.UserName} added successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_tag, $"Error removing user {userId}: {ex.Message}");
				throw;
            }
			finally
			{
				_lock.ExitWriteLock();
			}
        }
        public async Task<int> PutMessageAsync(DateTime timeStamp, string topicName, byte[] payload)
        {
            var message = new MessageEntity()
            {
                Timestamp = timeStamp,
                TopicName = topicName,
                Message = payload
            };
            try
            {
				_lock.EnterWriteLock();
				await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
				int id = message.Id;
                _logger.Info(_tag, $"Message added: {message.Timestamp} - {message.TopicName}");
				return id;
            }
            catch (Exception ex)
            {
                _logger.Error(_tag, $"Error adding message: {message.Timestamp} - {message.TopicName}");
                _logger.Error(_tag, ex.Message);
				throw;
            }
			finally
			{
				_lock.ExitWriteLock();
			}
        }
        public async Task<List<MessageEntity>?> GetMessagesByTimeAsync(string topic, DateTime startTime, DateTime endTime)
        {
            try
            {
				_lock.EnterReadLock();
                return await _context.Messages.Where(m => m.TopicName == topic && m.Timestamp >= startTime && m.Timestamp <= endTime).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(_tag, $"Error getting message by time: {topic} : {startTime} - {endTime}");
                _logger.Error(_tag, ex.Message);
				throw;
            }
			finally { _lock.ExitReadLock(); }
        }
		public async Task<MessageEntity?> GetMessagesByIdAsync(int id)
		{
			try
			{
				_lock.EnterReadLock();
				return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
			}
			catch (Exception ex)
			{
				_logger.Error(_tag, $"Error getting message by id: {id}");
				_logger.Error(_tag, ex.Message);
				throw;
			}
		}
        public async Task CreateTopicAsync(string topic, string? description = null)
        {
            try
            {
				_lock.EnterWriteLock();
                _context.Topics.Add(new TopicEntity()
                {
                    Topic = topic,
                    Description = description
                });
                await _context.SaveChangesAsync();
                _logger.Info(_tag, $"Created topic: {topic} with description {description}");
            }
            catch (Exception ex)
            {
                _logger.Error(_tag, $"Error creating topic: {topic}");
                _logger.Error(_tag, ex.Message);
				throw;
            }
			finally { _lock.ExitWriteLock(); }
        }
		public async Task<TopicEntity?> GetTopicAsync(string topicName)
		{
			try
			{
				_lock.EnterReadLock();
				var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Topic == topicName);
				return topic;
			}
			catch (Exception ex)
			{
				_logger.Error(_tag, $"Error getting topic: {topicName}");
				_logger.Error(_tag, ex.Message);
				throw;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
    }
}
