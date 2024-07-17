using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DbStore.Relationship;

namespace BackEnd.DbStore.Entity
{
    public class TopicEntity
    {
        public string Topic { get; set; }
        public string? Description { get; set; }

        public List<UserTopic> UserTopics { get; set; } = new();
		//public List<MessageEntity> Messages {  get; set; } = new();
    }
}
