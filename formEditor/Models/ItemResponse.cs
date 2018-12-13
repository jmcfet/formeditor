using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formEditor.Models
{
    public class itemResponse 
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public string userName { get; set; }
        public int type { get; set; }
        public string Name { get; set; }
        public int linenum { get; set; }
        public string var1 { get; set; }
        public string var2 { get; set; }
        public string var3 { get; set; }
    }
}
