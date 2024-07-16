using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbStore.Entity
{
	public class UserEntity
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public List<TopicEntity> Subscribes {  get; set; }
	}
}
