
using DbStore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DbStore
{
	public class DbController
	{
		private readonly BrokerContext _context;
		private readonly ILogger<DbController> _logger;
		public DbController(BrokerContext context, ILogger<DbController> logger)
		{
			_context = context;
			_logger = logger;
			_context.Database.EnsureCreated();
		}
		public async Task RegisterUserAsync(UserEntity user)
		{
			try
			{
				await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();
				_logger.LogInformation($"User {user.UserName} added successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error adding user {user.UserName}: {ex.Message}");
				throw;
			}
		}
		public async Task UnRegisterUserAsync(string username)
		{
			try
			{
				var rmUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
				if (rmUser != null) {
					_context.Users.Remove(rmUser);
					await _context.SaveChangesAsync();
					_logger.LogInformation($"User {rmUser.UserName} added successfully.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error removing user {username}: {ex.Message}");
				throw;
			}
		}
		public async Task PutMessageAsync(MessageEntity message)
		{
			try
			{
				await _context.Messages.AddAsync(message);
				await _context.SaveChangesAsync();
				_logger.LogInformation($"Message added: {message.Timestamp} - {message.Topic}");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error adding message: {message.Timestamp} - {message.Topic}");
			}
		}
	}
}
