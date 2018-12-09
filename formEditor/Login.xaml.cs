
using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
       
        List<User> users = null;
        public Login()
        {
            using (var db = new EditorDb())
            { 
                users = db.Users.ToList();
            }
            InitializeComponent();
            Loaded += Login_Loaded;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            Name.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string userName = Name.Text;
            User user = users.Where(u => u.Name == userName && u.Password == Password.Password).SingleOrDefault();
            if (user == null)
            {
                MessageBox.Show("Invalid userName or password");
            }
            ((App)Application.Current).activeUser = user;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).activeUser = null;
            this.Close();
        }
    }
}
