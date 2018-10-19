using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formEditor.Models
{
    public class Block
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double timer { get; set; }
        public List<FormEntry> questions { get; set; }
    }
}
