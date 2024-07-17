using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DbStore.Relationship;

namespace BackEnd.DbStore.Entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<UserTopic> UserTopics { get; set; } = new();
    }
}
