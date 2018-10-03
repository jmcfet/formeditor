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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace formEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FormEntry> lines;
        Grid rootGrid;
        int row = 0;
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new EditorDb())
            {
                lines = db.forms.ToList();
                lines = lines.OrderBy(l => l.linenum).ToList();
            }
            CreateControlsUsingObjects();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormDialog dialog = new FormDialog(lines);
            dialog.ShowDialog();
        }

        private void CreateControlsUsingObjects()
        {

            rootGrid = new Grid();
            rootGrid.Background = new SolidColorBrush(Colors.White); ;
            rootGrid.Margin = new Thickness(10.0);
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(.5, GridUnitType.Star) });
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
            foreach (FormEntry item in lines)
            {
                int displayRow = row;
                if (displayRow == 0)
                    displayRow++;

                Label lb = CreateLabel(row, 0, string.Format("{0}", row /2 + 1 ));
                rootGrid.Children.Add(lb);
                switch (item.type)
                {
                    case 1:
                        HandleType1(item);
                        break;
                    case 2:
                        HandleType2(item);
                        break;
                    case 3:
                        HandleType3(item);
                        break;


                }

            }
            rootGrid.Measure(new Size(600, 600));
            rootGrid.Arrange(new Rect(new Size(600, 600)));
            LayoutRoot.Children.Add(rootGrid);


        }
        void HandleType1(FormEntry item)
        {

            Label lb = CreateLabel(row, 1, item.label1);
            rootGrid.Children.Add(lb);
            CheckBox cb = CreateCheckBox(row, 2, item.checkbox1);
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            rootGrid.Children.Add(cb);
            cb = CreateCheckBox(row, 3, item.checkbox2);
            cb.HorizontalAlignment = HorizontalAlignment.Left;
            rootGrid.Children.Add(cb);
            row += 1;
            if (item.label2 == string.Empty)
                return;
            lb = CreateLabel(row, 1, item.label2);
            rootGrid.Children.Add(lb);
            rootGrid.Children.Add(CreateTextInputBlock(item.textbox1, row, 2));
            row += 1;
        }
        void HandleType2(FormEntry item)
        {

            CreateLine2(item.textbox1, item.textbox2);
            CreateLine2(item.textbox3, item.textbox4);
        }
        void HandleType3(FormEntry item)
        {
            TextBlock tb = new TextBlock() { Text = item.label1 };
            Grid.SetColumn(tb, 0);
            Grid.SetRow(tb, row);
            tb.TextWrapping = TextWrapping.WrapWithOverflow;

            rootGrid.Children.Add(tb);
            CheckBox cb = CreateCheckBox(row, 1, item.checkbox1);
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            rootGrid.Children.Add(cb);
            cb = CreateCheckBox(row, 2, item.checkbox2);
            cb.HorizontalAlignment = HorizontalAlignment.Left;
            rootGrid.Children.Add(cb);
            row += 1;
        }

        private TextBox CreateTextInputBlock(string text, int row, int column)
        {
            TextBox tb = new TextBox() { Text = text, Margin = new Thickness(5, 8, 0, 5) };


            var bc = new BrushConverter();
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        private Label CreateLabel(int row, int column, string text)
        {
            Label lb = new Label() { Content = text };

            lb.Margin = new Thickness(5);
            lb.Height = 30;
            //  lb.Width = 150;
            lb.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetColumn(lb, column);
            Grid.SetRow(lb, row);
            //   Grid.SetColumnSpan(lb, 2);

            return lb;

        }


        private CheckBox CreateCheckBox(int row, int column, string text)
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
        void CreateLine2(string textbox1, string textbox2)
        {
            Label lb = CreateLabel(row, 0, textbox1);
            rootGrid.Children.Add(lb);
            TextBox tb = CreateTextInputBlock("     ", row, 0);
            tb.HorizontalAlignment = HorizontalAlignment.Right;
            rootGrid.Children.Add(tb);
            lb = CreateLabel(row, 1, textbox2);
            rootGrid.Children.Add(lb);
            tb = CreateTextInputBlock("  ", row, 1);
            tb.HorizontalAlignment = HorizontalAlignment.Right;
            rootGrid.Children.Add(tb);
            row += 1;
        }
        private RowDefinition CreateRowDefinition()
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
        }
    }
}
