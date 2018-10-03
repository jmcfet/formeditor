using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formEditor.Models
{
    
    public class FormEntry
    {
        public int Id { get; set; }
        public int linenum { get; set; }
        public int type { get; set; }
        public string label1 { get; set; }
        public string label2 { get; set; }
        public string checkbox1 { get; set; }
        public string checkbox2 { get; set; }
        public string textbox1 { get; set; }
        public string textbox2 { get; set; }
        public string textbox3 { get; set; }
        public string textbox4 { get; set; }
    }
}
