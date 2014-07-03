using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationAccess.Model
{
    public class Secret
    {
        public int aid { get; set; }
        public int uid { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string content { get; set; }
        public int time { get; set; }
        public int distance { get; set; }
        public int comments_count { get; set; }
        public int favourite_count { get; set; }
    }
}
