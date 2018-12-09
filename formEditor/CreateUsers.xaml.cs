
using formEditor.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace formEditor
{
    /// <summary>
    /// Interaction logic for CreateUsers.xaml
    /// </summary>
    public partial class CreateUsers : Window
    {
        List<User> users = null;
        List<string> CarrierNames = new List<string>();
        public CreateUsers()
        {
            
            InitializeComponent();
            using (var db = new EditorDb())
            {
                users = db.Users.ToList();
            }
           
            users = users.Where(u => u.Level == 2).ToList();
            CarrierNames.Add("ATT");
            CarrierNames.Add("Verizon");
            CarrierNames.Add("TMobile");
            Carriers.ItemsSource = CarrierNames;
            Carriers.SelectedIndex = 0;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            User user = new User()
            {
                Name = Name.Text,
                Password = Password.Password
            };
            if (Name.Text.Count() == 0)
            {
                MessageBox.Show("user Name cannot be blank");
                return;
            }
            if (Password.Password.Count() == 0)
            {
                MessageBox.Show("password cannot be blank");
                return;
            }
            User user1 = users.Where(u => u.Name == Name.Text ).SingleOrDefault();
            if (user1 != null)
            {
                MessageBox.Show("User already exists");
            }
            else
            {
                user.Level = 2;
            //    vm.Saveuser(user);
            }
            users.Add(user);
            UserInfo.Visibility = Visibility.Hidden;
            PassInfo.Visibility = Visibility.Hidden;
            Create.Visibility = Visibility.Hidden;
            Name.Text = "";
           // Password.Text = "";
            CreateUser.Focus();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            Userstuff.Visibility = Visibility.Visible;
            Save.IsEnabled = false;

            listUsers.Visibility = Visibility.Hidden;
            Delete.Visibility = Visibility.Hidden;
            Name.Focus();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            Userstuff.Visibility = Visibility.Collapsed;
            listUsers.Visibility = Visibility.Visible;
            Delete.Visibility = Visibility.Visible;
            listUsers.DataContext = users;
        }
        //delete a user
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            User user1 = listUsers.SelectedItem as User;

      //      User user1 = users.Where(u => u.Name == Name.Text).SingleOrDefault();
            if (user1 == null)
            {
                MessageBox.Show("User does not exist");
                return;
            }
         //   vm.Deleteuser(user1);
            users.Remove(user1);
            listUsers.ItemsSource = null;
            listUsers.ItemsSource = users;
        }
        //test
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Carriers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        string pattern = @"\d";
        private void test_Click(object sender, RoutedEventArgs e)
        {
            
            string carrierServer = CarrierNames[Carriers.SelectedIndex];
            var carrierEmail = ConfigurationManager.AppSettings[carrierServer];
            
            //use regex to remove all the non-numerics
            StringBuilder sb = new StringBuilder();
            foreach (Match m in Regex.Matches(Phonenum.Text, pattern))
            {
                sb.Append(m);
            }
            MainWindow.sendText(sb + "@" + carrierEmail, "test");
        }

       

        private void textreceived_Checked(object sender, RoutedEventArgs e)
        {
            Save.IsEnabled = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text.Count() == 0)
            {
                MessageBox.Show("Name must be filled in");
                return;
            }
            if (Password.Password.Count() == 0)
            {
                MessageBox.Show("Password must be filled in");
                return;
            }
            //use regex to remove all the non-numerics
            StringBuilder sb = new StringBuilder();
            foreach (Match m in Regex.Matches(Phonenum.Text, pattern))
            {
                sb.Append(m);
            }
            User user = new User()
            {
                carrier = CarrierNames[Carriers.SelectedIndex],
                Level = 2,
                Name = Name.Text,
                phoneNumber = sb.ToString(),
                Password = Password.Password
            };
       
            using (var db = new EditorDb())
            {
                User test = db.Users.Where(u => u.Name == Name.Text).SingleOrDefault();
                if (test != null)
                {
                    MessageBox.Show("user already exists");
                    return;
                }
                db.Users.Add(user);
                db.SaveChanges();
                
            }
            Userstuff.Visibility = Visibility.Collapsed;
        }
    }
}
