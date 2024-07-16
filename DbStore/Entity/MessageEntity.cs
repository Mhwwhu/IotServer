using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbStore.Entity
{
	public class MessageEntity
	{
		public DateTime Timestamp { get; init; }
		public TopicEntity Topic { get; init; }
		public byte[] Message { get; init; }
	}
}
