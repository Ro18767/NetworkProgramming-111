using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ChatMessage
    {
        public String Autor { get; set; } = null!;
        public String Text { get; set; } = null!;

        public DateTime Moment { get; set; }
    }
}
