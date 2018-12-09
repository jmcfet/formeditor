using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formEditor.Models
{
    public class EditorDb : DbContext
    {
        public EditorDb() : base("name=FormEditor")
        {
      //      AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }
        public DbSet<Block> Blocks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //  public DbSet<FormEntry> forms { get; set; }

    }
}
