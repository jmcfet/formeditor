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
            for(int i =1;i < 5;i++)
            {
                string key = "Cell" + i.ToString();
                var user = ConfigurationManager.AppSettings[key];
                if (user == null)
                    break;
                string txtNumber = user.ToString();
               
                numbers.Add(txtNumber);
            }
            phonenumbers.ItemsSource = numbers;
           selectedBlock = blocks[0];
           
            Carriers.Add("ATT");
            Carriers.Add("Verizon");
            Carriers.Add("TMobile");
            Carrier.ItemsSource = Carriers;
            Carrier.SelectedIndex = 0;
            Remove.IsEnabled = false;

        }

        private void Blocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedBlock1 = BlockNames[Blocks.SelectedIndex];
            Block b = blocks.Where(b1 => b1.Name == selectedBlock1).SingleOrDefault();
            Timeout.Text = b.timer.ToString();
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
            Save.Content = "Done";
            db.SaveChanges();
        }
        
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //use regex to remove all the non-numerics
            string pattern = @"\d";
            StringBuilder sb = new StringBuilder();
            foreach (Match m in Regex.Matches(phonenumber.Text, pattern))
            {
                sb.Append(m);
            }
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //ge the carrier email from appsettings
            var carrierEmail = ConfigurationManager.AppSettings[carrier];
            if (carrierEmail == null)
            {
                MessageBox.Show("invalid carrier");
                return;
            }
           
            string num = sb.ToString() + "@" + carrierEmail.ToString();
            numbers.Add(num);
            phonenumbers.Items.Refresh();
            //add to end of appconfig file
            string key = "Cell" + numbers.Count.ToString();
            config.AppSettings.Settings.Add(key,num);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");


        }

        
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string id = numbers[phonenumbers.SelectedIndex];
            numbers.RemoveAt(phonenumbers.SelectedIndex);
            phonenumbers.Items.Refresh();
            //now find the key for this entry in app.config settings
            for (int i = 1; i < 5; i++)
            {
                string key = "Cell" + i.ToString();
                string val = ConfigurationManager.AppSettings[key];
                if (val == null)
                    break;
                if (val == id)
                {
                    config.AppSettings.Settings.Remove(key);
                    config.Save(ConfigurationSaveMode.Modified);

                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            
           
        }
        string carrier;
        private void Carrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             carrier = Carriers[Carrier.SelectedIndex];
        }

        private void phonenumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Remove.IsEnabled = true;
        }
    }
}
