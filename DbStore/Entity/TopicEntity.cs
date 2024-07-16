using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbStore.Entity
{
	public class TopicEntity
	{
		public string Topic { get; set; }
		public string Description { get; set; }

		public List<UserEntity> Subscribers { get; set; }
	}
}
