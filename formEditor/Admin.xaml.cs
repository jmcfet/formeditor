using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        EditorDb db;
        List<string> numbers = new List<string>();
        List<string> Carriers = new List<string>();
        public Admin(List<Block> blocks)
        {
            InitializeComponent();
            this.blocks = blocks;
           
            Loaded += Admin_Loaded;
        }

        private void Admin_Loaded(object sender, RoutedEventArgs e)
        {
            db = new EditorDb();
           
            BlockNames = new List<string>();
            blocks.ForEach(b => BlockNames.Add(b.Name));
            Blocks.ItemsSource = BlockNames;
            Blocks.SelectedIndex = 0;
            Blocks2.ItemsSource = BlockNames;
            Blocks2.SelectedIndex = 0;
           
           selectedBlock = blocks[0];
                  
            startTimeBlock1.Value = Convert.ToDateTime(selectedBlock.ExpectedStart.ToString());
            
        }

        private void Blocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedBlock = blocks[Blocks.SelectedIndex];
            Timeout.Text = selectedBlock.timer.ToString();
            Save.Content = "Save";
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
            selectedBlock.TimeLefttoComplete = 60 * time;
            db.Entry(selectedBlock).State = System.Data.Entity.EntityState.Modified;
            Save.Content = "Done";
            db.SaveChanges();
        }
       
        private void Blocks2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Block block = blocks[Blocks2.SelectedIndex];
            startTimeBlock1.Value = Convert.ToDateTime(block.ExpectedStart.ToString());
        }
        private void set_Click(object sender, RoutedEventArgs e)
        {
            Block block = blocks[Blocks2.SelectedIndex];
           DateTime temp = (DateTime)startTimeBlock1.Value;
            block.ExpectedStart = temp.TimeOfDay;
            db.Entry(block).State = System.Data.Entity.EntityState.Modified;
            set.Content = "Done";
            db.SaveChanges();
        }
    }
}
