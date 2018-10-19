using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace formEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FormEntry> lines;
        // Grid rootGrid;
        int row = 0;
        int formRowSelected = -1;
        Brush oldSelectedBrush = null;
        StackPanel oldSelectedRow = null;
        List<Button> Selectors = new List<Button>();
        int itemNumber = -1;
        //  int numdeleted = 0;
        Block block = null;
        bool bEntryMode = true;
        int initialQuestions = -1;
        double timeElapsed = 0;
        DispatcherTimer timer;
        List<FormEntry> removed = new List<FormEntry>();
        List<string> blocks = new List<string>() { "Block1", "Block2", "Block3", "Block4", "Block5", "Block5" };
        string selectedBlock;
        public MainWindow()
        {
            InitializeComponent();
           
            KeyDown += MainWindow_KeyDown;
            BlockSelector.ItemsSource = blocks;
            BlockSelector.SelectedIndex = 0;
            selectedBlock =  blocks[0];
            Start.Visibility = Visibility.Visible;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (bEntryMode)
                return;
            if (formRowSelected == -1)
            {
                MessageBox.Show("please select a line first", "Form error");
                return;
            }
            using (var db = new EditorDb())
            {
                FormEntry item = block.questions.Where(i => i.linenum == formRowSelected).SingleOrDefault();
               block.questions.Remove(item);
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                for (int line = formRowSelected + 1; line <= lines.Count; line++)
                {
                    item = block.questions.Where(i => i.linenum == line).SingleOrDefault();
                    item.linenum = item.linenum - 1;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;   //EF change tracking not working

                }
                db.SaveChanges();
                MessageBox.Show(string.Format("Line {0} removed", formRowSelected));
                Refresh();
            }
        }

        void Refresh()
        {
            using (var db = new EditorDb())
            {
                block = db.Blocks.Where(b => b.Name == selectedBlock)
                     .Include(b => b.questions).SingleOrDefault();
                lines = block.questions.OrderBy(l => l.linenum).ToList();
            }

            row = 0;
            initialQuestions = lines.Count() - lines.Where(l => l.type == 3).Count();
            rootGrid.Children.Clear();
            CreateControlsUsingObjects();
        }

        private void CreateControlsUsingObjects()
        {
            FormEntry olditem = null;
            //    rootGrid = LayoutRoot;

            rootGrid.Background = new SolidColorBrush(Colors.White); ;
            rootGrid.Margin = new Thickness(10.0);

            for (int i = 0; i < lines.Count * 2; i++)
            {
                rootGrid.RowDefinitions.Add(CreateRowDefinition());
            }
            foreach (FormEntry item in lines)
            {
                itemNumber++;
                int displayRow = row;
                //if (olditem != null &&    olditem.type != 3)
                //    displayRow++;
                //     if (item.type != 3)
                //    { 
                Button but = CreateButton(string.Format("{0}", item.linenum), displayRow, 0);
                rootGrid.Children.Add(but);
                //   }
                //   Label lb = CreateLabel(row, 0, string.Format("{0}", item.linenum ));

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
            //       LayoutRoot.Children.Add(rootGrid);


        }
        void HandleType1(FormEntry item)
        {
            StackPanel sp = CreateStackPanel(row, 1);

            sp.Children.Add(CreateLabel(item.label1, item.isBold));
            CheckBox cb = CreateCheckBox(item.checkbox1);
            sp.Children.Add(cb);
            sp.Children.Add(CreateCheckBox(item.checkbox2));

            row += 1;
            if (item.label2 == null)
            {
                
                return;
            }
                
            StackPanel sp2 = CreateStackPanel(row, 1);
            sp2.Children.Add(CreateLabel(item.label2, item.isBold));
            TextBox tb = new TextBox() { Width = 60, Height = 30 };
            tb.Tag = cb;
            tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
            tb.LostFocus += Tb_LostFocus;
            Binding myBinding = setBinding();
            myBinding.Source = item;
            myBinding.Path = new PropertyPath("Var1");
            BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
            sp2.Children.Add(tb);
            row += 1;
        }

        private void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
           TextBox tb = sender as TextBox;
            tb.Text = DateTime.Now.ToShortTimeString();
        }

        private void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            CheckBox cb = tb.Tag as CheckBox;
            if (cb.IsChecked == true)
           
                removeline((int)cb.Tag);
           


        }

        void HandleType2(FormEntry item)
        {
            StackPanel sp = CreateStackPanel(row, 1);
            sp.Children.Add(CreateLabel(item.label1, item.isBold));
            TextBox tb = new TextBox() { Width = 60, Height = 30 };
            tb.Tag = itemNumber;
            tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
            tb.LostFocus += Tb_LostFocus1;
            Binding myBinding = setBinding();
            myBinding.Source = item;
            myBinding.Path = new PropertyPath("Var1");
            BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
            sp.Children.Add(tb);
            //   sp.Children.Add(CreateTextInputBlock("    "));
            if (item.label2.Count() > 0)
            {
                sp.Children.Add(CreateLabel(item.label2, item.isBold));
                //  sp.Children.Add(CreateTextInputBlock("    "));
                tb = new TextBox() { Width = 40, Height = 30 };
                tb.Tag = itemNumber;
                tb.LostFocus += Tb_LostFocus1;
                myBinding = setBinding();
                myBinding.Source = item;
                myBinding.Path = new PropertyPath("Var2");
                BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
                sp.Children.Add(tb);
            }
            if (item.label3 != null)
            {
                tb = new TextBox() { Width = 30 };
                myBinding = setBinding();
                myBinding.Source = item;
                myBinding.Path = new PropertyPath("Var3");
                BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
            }
            if (item.label4 != null)
            {
                //sp.Children.Add(CreateLabel(item.label4, item.isBold));
                //sp.Children.Add(CreateTextInputBlock("    "));
            }
            row += 1;
        }

        private void Tb_LostFocus1(object sender, RoutedEventArgs e)
        {
            //if the next sibling in visual tree of checkbox parent (stackpanel) is a button then the
            //checkbox does not have an associated input that must be filled in. in this case delete also
            TextBox tb = sender as TextBox;
            var sp = VisualTreeHelperExtensions.FindAncestor<StackPanel>(tb);
            var grid = VisualTreeHelperExtensions.FindAncestor<Grid>(sp);
            int index = sp.Children.IndexOf(tb);
           
            if (index + 1  == sp.Children.Count)
                removeline((int)tb.Tag);
        }

        void HandleType3(FormEntry item)
        {

            StackPanel sp = CreateStackPanel(row, 1);
            Label lb = CreateLabel(item.label1, item.isBold);
            sp.Children.Add(lb);
            row += 1;
        }
        Binding setBinding()
        {
            Binding myBinding = new Binding();
            myBinding.ValidatesOnExceptions = true;
            myBinding.ValidatesOnDataErrors = true;
            myBinding.NotifyOnValidationError = true;

            myBinding.Path = new PropertyPath("Var1");
            myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            return myBinding;
        }
        private TextBox CreateTextInputBlock(string text)
        {
            TextBox tb = new TextBox() { Text = text, Margin = new Thickness(5, 8, 0, 5) };
            var bc = new BrushConverter();
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");

            return tb;
        }
        private TextBox CreateTextInputBlock2(FormEntry item)
        {
            TextBox tb = new TextBox() { Margin = new Thickness(5, 8, 0, 5) };
            var bc = new BrushConverter();
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");
            //  MyData myDataObject = new MyData(DateTime.Now);
            if (item.type == 1)
            {

            }
            if (item.type == 2)
            {
                Binding myBinding = new Binding();
                myBinding.Source = item;
                myBinding.Path = new PropertyPath("Var1");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
                myBinding.Path = new PropertyPath("Var2");
            }
            return tb;
        }

        private Label CreateLabel(string text, bool isBold)
        {
            Label lb = new Label() { Content = text };
            lb.Name = "abcd";
            lb.Margin = new Thickness(5);
            lb.Height = 30;
            if (isBold == true)
            {
                lb.FontWeight = FontWeights.Bold;
                lb.FontSize = 16;
            }
            //  lb.Width = 150;
            lb.HorizontalAlignment = HorizontalAlignment.Left;

            //   Grid.SetColumnSpan(lb, 2);

            return lb;

        }

        StackPanel CreateStackPanel(int row, int column)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            Grid.SetColumn(sp, column);
            Grid.SetRow(sp, row);
            rootGrid.Children.Add(sp);
            return sp;
        }
        private CheckBox CreateCheckBox(string text)
        {
            CheckBox cb = new CheckBox();
            cb.Tag = itemNumber;
            cb.Click += Cb_Click;
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;


            cb.Content = text;

            return cb;
        }

        private void Cb_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool bDelete = false;
            int line = (int)cb.Tag;
            //    line -= numdeleted; 
            if (cb.Content as String == "No" || cb.Content as String == "Done" )  
                bDelete = true;
            else if (cb.Content as String == "Yes")    //no second input field
            {
                //if the next sibling in visual tree of checkbox parent (stackpanel) is a button then the
                //checkbox does not have an associated input that must be filled in. in this case delete also
                
                var sp = VisualTreeHelperExtensions.FindAncestor<StackPanel>(cb);
                var grid = VisualTreeHelperExtensions.FindAncestor<Grid>(sp);
                int index = grid.Children.IndexOf(sp);
                //check if this is last entry
                if (index == grid.Children.Count -1)
                    bDelete = true;
                else
                {
                    Button but = grid.Children[index + 1] as Button;
                    if (but != null)
                        bDelete = true;
                }
            }
                
            if (bDelete)
                  
                removeline(line);
            
        }

        void CreateLine2(FormEntry item)
        {
            //      Label lb = CreateLabel(row, 1, item.label1);
            //    rootGrid.Children.Add(lb);
            //     TextBox tb = CreateTextInputBlock("     ", row, 2);
            //   tb.HorizontalAlignment = HorizontalAlignment.Left;
            //    rootGrid.Children.Add(tb);
            //    lb = CreateLabel(row, 3, item.label2);
            //    rootGrid.Children.Add(lb);
            //     tb = CreateTextInputBlock("  ", row, 4);
            //     tb.HorizontalAlignment = HorizontalAlignment.Right;
            //     rootGrid.Children.Add(tb);
            row += 1;
        }
        private Button CreateButton(string text, int row, int column)
        {
            Button tb = new Button() { Content = text, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 8, 0, 5) };
            tb.Height = 25;
            tb.Width = 35;
            tb.Click += Tb_Click;
            tb.Tag = row;
            //  tb.Visibility = Visibility.Hidden;
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            Selectors.Add(tb);
            return tb;
        }

        private void Tb_Click(object sender, RoutedEventArgs e)
        {
            Button tb = sender as Button;
            formRowSelected = int.Parse(tb.Content as String);
            StackPanel lb = GetGridElement(rootGrid, (int)tb.Tag, 1) as StackPanel;
            lb.Background = new SolidColorBrush(Colors.LightGray);
            if (oldSelectedRow != null)
            {
                oldSelectedRow.Background = oldSelectedBrush;

            }
            oldSelectedRow = lb;
        }
        void removeline(int line)
        {
           
            lines.RemoveAt(line);
            row = 0;
            //    numdeleted++;
            itemNumber = -1;
            rootGrid.Children.Clear();
            CreateControlsUsingObjects();
        }
        UIElement GetGridElement(Grid g, int r, int c)
        {
            for (int i = 0; i < g.Children.Count; i++)
            {
                UIElement e = g.Children[i];
                if (Grid.GetRow(e) == r && Grid.GetColumn(e) == c)
                    return e;
            }
            return null;
        }
        private RowDefinition CreateRowDefinition()
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            return RowDefinition;
        }

        private void passwordEntered_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password == "tennis")
            {
                Add.Visibility = Visibility.Visible;
                bEntryMode = false;
                if (timer != null)
                    timer.Stop();
                Refresh();
            }
           
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            bEntryMode = false;
            Add.Visibility = Visibility.Visible;
            Refresh();
        }

        private void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //  bEntryMode = true;
            Refresh();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FormDialog dialog = new FormDialog(lines, formRowSelected,block);
            dialog.Closing += Dialog_Closing;
            dialog.LineNumber = formRowSelected;
            bEntryMode = false;
            dialog.ShowDialog();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            bool bHasErrors = false;
            int count = 0;
            foreach (FormEntry item in lines)
            {
               
                if (item.type == 1)
                {
                    
                    if (item.Var1 == null)
                    {
                        bHasErrors = true;
                        break;
                    }
                }
                if (item.type == 2)
                {
                    if (item.label1 != null)
                    {
                        if (item.Var1 == null)
                        {
                            bHasErrors = true;
                            break;
                        }
                    }
                    if (item.label2 != null)
                    {
                        if (item.Var2 == null)
                        {
                            bHasErrors = true;
                            break;
                        }
                    }
                    if (item.label3 != null)
                    {
                        if (item.Var3 == null)
                        {
                            bHasErrors = true;
                            break;
                        }
                    }
                }
            }
            if (bHasErrors)
            {
                MessageBox.Show("Must complete all steps");
            }
            timer.Stop();
        }

        public static class VisualTreeHelperExtensions
        {
            public static T FindAncestor<T>(DependencyObject dependencyObject)
                where T : class
            {
                DependencyObject target = dependencyObject;
                do
                {
                    target = VisualTreeHelper.GetParent(target);
                }
                while (target != null && !(target is T));
                return target as T;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // MessageBox.Show("work was not copleted in time, manager will be notified", "Severe Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            //  timer.Stop();
            double numDone = initialQuestions - (double)(lines.Count - lines.Where(l => l.type == 3).Count());
            if (numDone == initialQuestions)
            {
                MessageBox.Show("work was  completed in time", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                timer.Stop();

            }
            Progress.Value = numDone / initialQuestions * 100;
            timeElapsed += 10;
            if  (timeElapsed > block.timer * 60 )
            {
                MessageBox.Show("work was not completed in time, manager will be notified", "Severe Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                timer.Stop();
            }
        }

        private void BlockSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedBlock = blocks[BlockSelector.SelectedIndex];
            Start.Visibility = Visibility.Visible;
          //  Edit.Visibility = Visibility.Visible;
        }
    }
    class info
    {
        int linenum { get; set; }
        ComboBox cb { get; set; }
    }
}
