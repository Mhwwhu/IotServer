using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.DbStore.Entity
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string TopicName { get; set; }
        public byte[] Message { get; set; }
    }
}
