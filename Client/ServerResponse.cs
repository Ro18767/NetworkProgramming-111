using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    internal class ServerResponse
    {
        public String Status { get; set; } = null!;
        public List<ChatMessage> Messages { get; set; } = null!;

    }
}
