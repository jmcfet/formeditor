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
    /// Interaction logic for FormDialog.xaml
    /// </summary>
    public partial class FormDialog : Window
    {
        List<FormEntry> lines;
        public FormDialog(List<FormEntry> lines)
        {
            InitializeComponent();
            this.lines = lines;
         //   Loaded += FormDialog_Loaded;       
        }

        private void FormDialog_Loaded(object sender, RoutedEventArgs e)
        {
            
            rootGrid.Background = new SolidColorBrush(Colors.White); ;
            rootGrid.Margin = new Thickness(10.0);
            rootGrid.ColumnDefinitions.Add(
                  new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < 20; i++)
            {
                rootGrid.RowDefinitions.Add(CreateRowDefinition());
            }
            FormEntry entry = new FormEntry()
            {
                label1 = "enter text",
                label2 = "enter text",
                checkbox1 = "check1 text",
                checkbox2 = "check2 text"
            };
                 
            SharedCode.setRoot(rootGrid);
           HandleType1(entry);
        }
           private RowDefinition CreateRowDefinition()
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
        }
        public  void HandleType1(FormEntry item)
        {
            int row = 0;
            Label lb;
            rootGrid.Children.Add(CreateTextInputBlock(item.label1, row, 0));
            
            CheckBox cb = CreateCheckBox(row, 1, " ");
            cb.HorizontalAlignment = HorizontalAlignment.Right;
          //  rootGrid.Children.Add(cb);

            TextBox tb  = CreateTextInputBlock(item.checkbox1, row, 1);
         //   tb. = "Check 1";
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            rootGrid.Children.Add(tb);
            cb = CreateCheckBox(row, 3, item.checkbox2);
            cb.HorizontalAlignment = HorizontalAlignment.Left;
            rootGrid.Children.Add(cb);
            row += 1;
           
            lb = CreateLabel(row, 0, item.label2);
            rootGrid.Children.Add(lb);
            rootGrid.Children.Add(CreateTextInputBlock(item.textbox1, row, 0));
            row += 1;
        }
        private static Label CreateLabel(int row, int column, string text)
        {
            Label lb = new Label() { Content = text };

            lb.Margin = new Thickness(5);
            lb.Height = 30;

            lb.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetColumn(lb, column);
            Grid.SetRow(lb, row);
            //   Grid.SetColumnSpan(lb, 2);

            return lb;

        }


        private static CheckBox CreateCheckBox(int row, int column, string text)
        {
            CheckBox cb = new CheckBox();
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;

            Grid.SetColumn(cb, column);
            Grid.SetRow(cb, row);

            cb.Content = text;

            return cb;
        }
        private static TextBox CreateTextInputBlock(string text, int row, int column)
        {
            TextBox tb = new TextBox() { Text = text, Margin = new Thickness(5, 8, 0, 5) };


            var bc = new BrushConverter();
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Type1Done_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int afterline;
            bool insertingAfterLine = false;
            if (Int32.TryParse(Linenum.Text, out afterline))
            {
                insertingAfterLine = true;
                int newLineNum = afterline + 1;
                for (int line = afterline - 1; line < lines.Count-1; line++)
                {
                    lines[line].linenum++;
                }
            }
           
            FormEntry form = new FormEntry()
            {
                type=1,
                linenum = afterline,
                label1 = Label1.Text,
                label2 = Label2.Text,
                checkbox1 = Check1.Text,
                checkbox2 = Check2.Text
            };
            using (var db = new EditorDb())
            {
                db.forms.Add(form);
                db.SaveChanges();
            }
            lines.Add(form);
            Label1.Text = "";
            Label2.Text = "";
            Check1.Text = "Yes";
            Check2.Text = "No";
        }
    }

    
}
