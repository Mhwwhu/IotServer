using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DbStore.Entity;

namespace BackEnd.DbStore.Relationship
{
    public class UserTopic
    {
        public int UserId { get; init; }
        public string TopicName { get; init; }
        public UserEntity User { get; init; }
        public TopicEntity Topic { get; init; }
    }
}
