using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace formEditor
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        List<string> BlockNames;
        List<Block> blocks;
        Block selectedBlock = null;
      //  EditorDb db;

        public Admin(List<Block> blocks)
        {
            InitializeComponent();
            this.blocks = blocks;
            Loaded += Admin_Loaded;
        }

        private void Admin_Loaded(object sender, RoutedEventArgs e)
        {
          //  db = new EditorDb();
            
            //    blocks = db.Blocks.ToList();
                BlockNames = new List<string>();
                blocks.ForEach(b => BlockNames.Add(b.Name));
                Blocks.ItemsSource = BlockNames;
                Blocks.SelectedIndex = 0;
                selectedBlock = blocks[0];
           
        }

        private void Blocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedBlock1 = BlockNames[Blocks.SelectedIndex];
            Block b = blocks.Where(b1 => b1.Name == selectedBlock1).SingleOrDefault();
            Timeout.Text = b.timer.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBlock == null)
            {
                MessageBox.Show("please select a block");
                return;
            }
            Int32 time;
            if (!Int32.TryParse(Timeout.Text, out time))
            {
                MessageBox.Show("time must be integer");
                return;
            }
            selectedBlock.timer = time;
         //   db.SaveChanges();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
