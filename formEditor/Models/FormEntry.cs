using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace formEditor.Models
{
    
    public class FormEntry :  INotifyPropertyChanged,IDataErrorInfo
    {
        private string var1;
        private string var2;
        private string var3;

        

        public int Id { get; set; }
        public int linenum { get; set; }
        public int type { get; set; }
        public string label1 { get; set; }
        public string label2 { get; set; }
        public string label3 { get; set; }
        public string label4 { get; set; }
        public bool checkbox1State { get; set; }
        public bool checkbox2State { get; set; }
        public string checkbox1 { get; set; }
        public string checkbox2 { get; set; }
        public string textbox1 { get; set; }
        public string textbox2 { get; set; }
        public string textbox3 { get; set; }
        public string textbox4 { get; set; }
        public String Var1
        {
            get { return var1; }
            set
            {
               // throw new ArgumentException("Invalid phone format");
                var1 = value;
                OnPropertyChanged("MyDataProperty");
            }
        }
        public String Var2
        {
            get { return var2; }
            set
            {
                var2 = value;
                OnPropertyChanged("MyDataProperty");
            }
        }
        public String Var3
        {
            get { return var3; }
            set
            {
                var3 = value;
                OnPropertyChanged("MyDataProperty");
            }
        }
        // public String var1 { get; set; }
        //  public String var2 { get; set; }
   //    public String var3 { get; set; }
        public String var4 { get; set; }
        public bool isBold { get; set; }
        public int var1Type { get; set; }
        public int var2Type { get; set; }
        public int var3Type { get; set; }
        public string Error => throw new NotImplementedException();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public string this[string columnName]
        {
            get
            {
                string errorMsg = String.Empty;
                if (columnName.Equals("Var1"))
                {
                    if (String.IsNullOrEmpty(this.var1))
                        errorMsg = "mandatory field";
                    else if (!verifyField(this.var1Type, this.var1))
                        errorMsg = "mandatory field";
                }
                else if (columnName.Equals("Var2"))
                {
                    if (String.IsNullOrEmpty(this.var2))
                        errorMsg = "mandatory field";
                    else if (!verifyField(this.var2Type, this.var2))
                        errorMsg = "mandatory field";
                }
                else if (columnName.Equals("Var3"))
                {
                    if (String.IsNullOrEmpty(this.var3))
                        errorMsg = "mandatory field";

                }
                else if (columnName.Equals("Var4"))
                {
                    if (String.IsNullOrEmpty(this.var4))
                        errorMsg = "mandatory field";

                }

                else
                    errorMsg = String.Empty;

                return errorMsg;
            }
        }
        bool verifyField(int type, string tb)
        {
            DateTime dateTime2;
            if (type == 0)
            {
                if (DateTime.TryParse(tb, out dateTime2))
                {
                    Console.WriteLine(dateTime2);
                }
                else
                {
                    MessageBox.Show("invalid date");
                    return false;
                }
            }
            if (type == 1)
            {
                int weight;
                bool bFailed = false;
                if (!Int32.TryParse(tb, out weight))
                    bFailed = true;

                //if (weight < 10 || weight > 85)
                //    bFailed = true;
                if (bFailed)
                {
                    MessageBox.Show("invalid weight");
                    return false;
                }
            }
            return true;
        }
    }
}
