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
    /// Interaction logic for FormDialog.xaml
    /// </summary>
    public partial class FormDialog : Window
    {
        List<FormEntry> lines;
        public int LineNumber;
        public Block block;
        public FormDialog(List<FormEntry> lines,int LineNumber,Block block)
        {
            InitializeComponent();
            this.LineNumber = LineNumber;
            this.lines = lines;
            this.block = block;
            Loaded += FormDialog_Loaded;;       
        }

        private void FormDialog_Loaded(object sender, RoutedEventArgs e)
        {
            /*
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
            */
            Next.IsEnabled = false;
            if (LineNumber != -1)
            {
                Linenum.Text = LineNumber.ToString();
                Next.IsEnabled = true;
            }
      //      SharedCode.setRoot(rootGrid);
      //     HandleType1(entry);
        }
           private RowDefinition CreateRowDefinition()
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
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
            this.Close();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            int newLineNum = 1;
            bool insertingAfterLine = false;
            bool bchecked = false;
            var selected = Types.SelectedItem as TabItem;
            using (var db = new EditorDb())
            {
              
                if (top.IsChecked == true)
                {
                    bchecked = true;
                    for (int line = 0; line < lines.Count; line++)
                    {
                        int oldLineNumber =  lines[line].linenum++;
                        db.Entry(lines[line]).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                if (Bottom.IsChecked == true)
                {
                    bchecked = true;
                    
                    newLineNum = lines[lines.Count - 1].linenum  + 1;
                    
                }
                if (bchecked == false)
                {
                   
                    if (Int32.TryParse(Linenum.Text, out newLineNum))
                    {
                       //note reverse order to avoid duplicate line numbers
                        for (int line = lines.Count; line > newLineNum; line--)
                        {
                            FormEntry item = block.questions.Where(i => i.linenum == line ).SingleOrDefault();
                      
                            item.linenum = item.linenum + 1;
                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;

                        }
                    }
                    newLineNum++;
                }
                FormEntry form = new FormEntry();
                if ((String)selected.Header == "Type 1")
                {
                    form = new FormEntry()
                    {
                        type = 1,
                        linenum = newLineNum,
                        label1 = Label1.Text,
                        label2 = Label2.Text,
                        checkbox1 = Check1.Text,
                        checkbox2 = Check2.Text
                    };
                    Label1.Text = "";
                    
                    Check1.Text = "Yes";
                    Check2.Text = "No";
                }
                if ((String)selected.Header == "Type 2")
                {
                    form = new FormEntry()
                    {
                        type = 2,
                        linenum = newLineNum,
                        label1 = T2Label1.Text,
                        label2 = T2Label2.Text,
                        label3 = T2Label3.Text,
                        label4 = T2Label4.Text

                    };
                    form.var1Type = choices.SelectedIndex;
                    form.var2Type = choices1.SelectedIndex;
                    form.var3Type = choices2.SelectedIndex;
                    T2Label1.Text = "";
                    T2Label2.Text = "";
                    T2Label3.Text = "";
                    T2Label4.Text = "";

                }
                if ((String)selected.Header == "Type 3")
                {
                    form = new FormEntry()
                    {
                        type = 3,
                        linenum = newLineNum,
                        label1 = T3Label1.Text
                       

                    };
                    if (MakeBold.IsChecked == true)
                        form.isBold = true;
                }
                


                lines.Add(form);
                block.questions.Add(form);
                db.Entry(form).State = System.Data.Entity.EntityState.Added;
                db.Entry(block).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            Linenum.Text = newLineNum.ToString();


        }

        private void top_Click(object sender, RoutedEventArgs e)
        {
            Next.IsEnabled = true;
        }

        private void Bottom_Click(object sender, RoutedEventArgs e)
        {
            Next.IsEnabled = true;
        }
    }

    
}
