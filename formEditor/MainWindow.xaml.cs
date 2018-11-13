using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        Dictionary<string,info> propsnotinDB = new Dictionary<string, info>();
        // Grid rootGrid;
        int row = 0;
        int formRowSelected = -1;
        FormEntry selectedItem = null;
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
        List<Block> blocks;
        List<string> BlockNames;
        int currentBlockIndex = -1;
        string selectedBlock;
        public MainWindow()
        {
            InitializeComponent();
           
            KeyDown += MainWindow_KeyDown;
            using (var db = new EditorDb())
            {

                blocks = db.Blocks
                    .Include(b => b.questions).ToList();
            }
            // blocks = db.Blocks.ToList();
            blockinfo.ItemsSource = blocks;
            blockinfo.DataContext = blocks;
            
            BlockNames = new List<string>();
            foreach(Block blok in blocks)
            {
                blok.TimeLefttoComplete = blok.timer * 60;
                info info = new info() { lastLineNumber = 0, TimeLeft = 0 , bActive = false};
                propsnotinDB.Add(blok.Name,info);
                BlockNames.Add(blok.Name);
            }
            selectedBlock = BlockNames[0];
            Start.Visibility = Visibility.Visible;
            itemNumber = -1;
            Refresh();
            rootGrid.IsEnabled = false;
                   
            Start.Visibility = Visibility.Visible;
            
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (bEntryMode)
                return;
            Button but = sender as Button;
            if (selectedItem == null)
            {
                MessageBox.Show("please select a line first", "Form error");
                return;
            }
            using (var db = new EditorDb())
            {
                FormEntry item = block.questions.Where(i => i.linenum == formRowSelected).SingleOrDefault();
               block.questions.Remove(selectedItem);
                db.Entry(selectedItem).State = System.Data.Entity.EntityState.Deleted;
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
  
            block = blocks.Where(b => b.Name == selectedBlock)
                
                     .SingleOrDefault();
       
            info info = propsnotinDB[block.Name];
            lines = block.questions.Where(q=>q.linenum > block.CurrentItem).OrderBy(l => l.linenum).ToList();
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
                if (item.type != 3)
                {
                    Button but = CreateButton( item, displayRow, 0);
                    rootGrid.Children.Add(but);
                }
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

            
             //      rootGrid.Measure(new Size(600, 600));
             //      rootGrid.Arrange(new Rect(new Size(600, 600)));
             //       LayoutRoot.Children.Add(rootGrid);


        }
       
        void HandleType1(FormEntry item)
        {
            StackPanel sp = CreateStackPanel(row, 1);
            sp.Tag = item;
            TextBlock tb1 = new TextBlock();
            tb1.Width = 400;
            tb1.Height = 30;
            tb1.Text = item.label1;
            if (item.label1.Length > 80)
            {
                tb1.Height = 60;
                tb1.TextWrapping = TextWrapping.Wrap;
            }
            sp.Children.Add(tb1);
            CheckBox cb1 = CreateCheckBox1(item);
            if (item.checkbox1State)
                cb1.IsChecked = true;
            sp.Children.Add(cb1);
            CheckBox cb = CreateCheckBox2(item);
            if (item.checkbox2State)
                cb.IsChecked = true;
            sp.Children.Add(cb);

            row += 1;
            if (item.label2 == null || item.label2 == string.Empty)
            {
                
                return;
            }
                
            StackPanel sp2 = CreateStackPanel(row, 1);
            sp2.Tag = item;
            sp2.Children.Add(CreateLabel(item.label2, item.isBold));
            TextBox tb = new TextBox() { Width = 60, Height = 30 };
            tb.Tag = cb1;
            tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
            tb.LostFocus += Tb_LostFocus;
            tb.KeyDown += Tb_KeyDown;
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
            {
                removeline(cb.Tag as FormEntry);
                
            }



        }

        void HandleType2(FormEntry item)
        {
            StackPanel sp  = CreateStackPanel(row, 1);
            sp.Tag = item;
            sp.Children.Add(CreateLabel(item.label1, item.isBold));
            TextBox tb = new TextBox() { Width = 60, Height = 30 };
            tb.Tag = item;
            tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
            tb.KeyDown += Tb_KeyDown;
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
                tb.Tag = item;
                tb.KeyDown += Tb_KeyDown;
                tb.LostFocus += Tb_LostFocus2;
                myBinding = setBinding();
                myBinding.Source = item;
                myBinding.Path = new PropertyPath("Var2");
                BindingOperations.SetBinding(tb, TextBox.TextProperty, myBinding);
                sp.Children.Add(tb);
            }
            if (item.label3 != null && item.label3.Count() > 0 )
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

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
           
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    if (e.Key == Key.Return)
                    {
                        if (e.Key == Key.Enter)
                        {
                        //  textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                        // Creating a FocusNavigationDirection object and setting it to a
                        // local field that contains the direction selected.
                        FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

                        // MoveFocus takes a TraveralReqest as its argument.
                        TraversalRequest request = new TraversalRequest(focusDirection);

                        // Gets the element with keyboard focus.
                        UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

                        // Change keyboard focus.
                        if (elementWithFocus != null)
                        {
                            
                            elementWithFocus.MoveFocus(request);
                            
                                e.Handled = true;
                               
                            
                        }
                        
                    }
                    }
                }
        }

        private void Tb_LostFocus1(object sender, RoutedEventArgs e)
        {
            //if textbox is the last in the stackpanel then action is done so remove
            TextBox tb = sender as TextBox;
            FormEntry entry = tb.Tag as FormEntry;
            if (entry.type == 1)
            {
                int weight;
                bool bFailed = false;
                if (!Int32.TryParse(tb.Text, out weight))
                    bFailed = true;

                if (weight < 10 || weight > 85)
                    bFailed = true;
                if (bFailed)
                {
                    MessageBox.Show("invalid weight");
                    return;
                }
            }

            Debug.WriteLine(string.Format("Tb_LostFocus1 {0} {1} ", tb.Tag, tb.Text));
            var sp = VisualTreeHelperExtensions.FindAncestor<StackPanel>(tb);
            var grid = VisualTreeHelperExtensions.FindAncestor<Grid>(sp);
            int index = sp.Children.IndexOf(tb);

            if (index + 1 == sp.Children.Count)
            {

                removeline(entry);
                              
                CheckForFinish();
               
            }

        }
        private void Tb_LostFocus2(object sender, RoutedEventArgs e)
        {
            //if textbox is the last in the stackpanel then action is done so remove
            TextBox tb = sender as TextBox;
            FormEntry entry = tb.Tag as FormEntry;
            //if (entry.var2Type == 1)
            //{
            //    int weight;
            //    bool bFailed = false;
            //    if (!Int32.TryParse(tb.Text, out weight))
            //        bFailed = true;

            //    if (weight < 10 || weight > 85)
            //        bFailed = true;
            //    if (bFailed)
            //    {
            //        MessageBox.Show("invalid weight");
            //        return ;
            //    }
            //}
            Debug.WriteLine(string.Format("Tb_LostFocus2 {0} {1} ", tb.Tag, tb.Text));
            var sp = VisualTreeHelperExtensions.FindAncestor<StackPanel>(tb);
            var grid = VisualTreeHelperExtensions.FindAncestor<Grid>(sp);
            int index = sp.Children.IndexOf(tb);

            if (index + 1 == sp.Children.Count)
            {
                removeline(entry);

                CheckForFinish();
               
            }
              //  rootGrid.Children.Remove(sptest);
        }
        void HandleType3(FormEntry item)
        {

            StackPanel sp = CreateStackPanel(row, 1);
            TextBlock tb = new TextBlock();
            tb.Width = 600;
            tb.Height = 30;
            tb.Text = item.label1;
            if (item.label1.Length > 80)
            {
                tb.Height = 60;
                tb.TextWrapping = TextWrapping.Wrap;
            }
            if (item.isBold == true)
            {
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 16;
            }
            sp.Children.Add(tb);
            row += 1;
        }
        
        Binding setBinding()
        {
            Binding myBinding = new Binding();
            myBinding.ValidatesOnExceptions = true;
            myBinding.ValidatesOnDataErrors = true;
            myBinding.NotifyOnValidationError = true;
          
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
            sp.Focusable = false;
            sp.Orientation = Orientation.Horizontal;
            Grid.SetColumn(sp, column);
            Grid.SetRow(sp, row);
            rootGrid.Children.Add(sp);
            return sp;
        }
        private CheckBox CreateCheckBox1(FormEntry item)
        {
            CheckBox cb = new CheckBox();
            cb.Tag = item;
            cb.Click += Cb_Click1;
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;


            cb.Content = item.checkbox1;

            return cb;
        }
        private CheckBox CreateCheckBox2(FormEntry item)
        {
            CheckBox cb = new CheckBox();
            cb.Tag = item;
            cb.Click += Cb_Click2;
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;


            cb.Content = item.checkbox2;

            return cb;
        }
        //check 1 can only be YES or Done
        private void Cb_Click1(object sender, RoutedEventArgs e)
        {
           
            CheckBox cb = sender as CheckBox;
            bool bDelete = false;
           
            FormEntry entry = cb.Tag as FormEntry;
            if ( cb.Content as String == "Done")
            { 
                bDelete = true;
                
                entry.checkbox1State = true;
             }
            else if (cb.Content as String == "Yes")    //no second input field
            {
                //if the next sibling in visual tree of checkbox parent (stackpanel) is a button then the
                //checkbox does not have an associated input that must be filled in. in this case delete also
                entry.checkbox1State = true;
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
            {
                removeline(entry);
                CheckForFinish();
                //buts.ForEach(b => rootGrid.Children.Remove(b));
                //stacks.ForEach(s => rootGrid.Children.Remove(s));
                //buts.Clear();
                //stacks.Clear();
            }


            }
        private void Cb_Click2(object sender, RoutedEventArgs e)
        {
            
            CheckBox cb = sender as CheckBox;
            bool bDelete = false;
            
            FormEntry entry = cb.Tag as FormEntry;
            if (cb.Content as String == "No")
            {
                bDelete = true;

                entry.checkbox2State = true;
            }
            else if (cb.Content as String == "Yes")    //no second input field
            {
                //if the next sibling in visual tree of checkbox parent (stackpanel) is a button then the
                //checkbox does not have an associated input that must be filled in. in this case delete also
                entry.checkbox1State = true;
                var sp = VisualTreeHelperExtensions.FindAncestor<StackPanel>(cb);
                var grid = VisualTreeHelperExtensions.FindAncestor<Grid>(sp);
                int index = grid.Children.IndexOf(sp);
                //check if this is last entry
                if (index == grid.Children.Count - 1)
                    bDelete = true;
                else
                {
                    Button but = grid.Children[index + 1] as Button;
                    if (but != null)
                        bDelete = true;
                }
            }

            if (bDelete)
            {
                removeline(entry);
               CheckForFinish();
            }

        }

       
        private Button CreateButton(FormEntry item, int row, int column)
        {
            Button but = new Button() { Content = item.linenum, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(5, 8, 0, 5) };
            but.Height = 25;
            but.Focusable = false;
            but.Width = 35;
            but.Click += but_Click;
            but.Tag = item;
            //  tb.Visibility = Visibility.Hidden;
            Grid.SetColumn(but, column);
            Grid.SetRow(but, row);
            Selectors.Add(but);
            return but;
        }

        private void but_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;

            selectedItem   = but.Tag as FormEntry;
            
            FindType(selectedItem, typeof(Button));
            FindType(selectedItem, typeof(StackPanel));
         //   StackPanel lb = GetGridElement(rootGrid, (int)but.Tag, 1) as StackPanel;
            stacks[0].Background = new SolidColorBrush(Colors.LightGray);
            if (oldSelectedRow != null)
            {
                oldSelectedRow.Background = oldSelectedBrush;

            }
            oldSelectedRow = stacks[0];
        }
        List<Button> buts = new List<Button>();
        List<StackPanel> stacks = new List<StackPanel>();
        void removeline(FormEntry item)
        {
            
            
            FindType(item, typeof(Button));
            FindType(item, typeof(StackPanel));
            buts.ForEach(b => rootGrid.Children.Remove(b));
            stacks.ForEach(s => rootGrid.Children.Remove(s));
            block.CurrentItem = item.linenum;


            //using (var db = new EditorDb())
            //{
            //    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //}
            lines.Remove(item);
            itemNumber = -1;
         
        }
       void FindType(FormEntry item,Type type)
       {
            if (type.Name == "Button")
                buts.Clear();
            else
                stacks.Clear();
            int childrenCount = VisualTreeHelper.GetChildrenCount(rootGrid);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(rootGrid, i);
                // If the child is not of the request child type child  
                if (type.Name == "Button")
                {
                    if (child as Button != null)
                    {
                        Button but = child as Button;
                        if (but.Tag != null)
                        {
                            FormEntry entry = but.Tag as FormEntry;
                            if (entry == item)
                                buts.Add(but);
                        }
                        continue;
                    }
                }

                    if (child as StackPanel != null)
                    {
                        StackPanel st = child as StackPanel;
                        if (st.Tag != null)
                        {
                            FormEntry entry = (st.Tag) as FormEntry;
                            if (entry == item)
                                stacks.Add(st);
                        }
                    
                    }
            }
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
                Configure.Visibility = Visibility.Visible;
                logout.Visibility = Visibility.Visible;
                rootGrid.IsEnabled = true;
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
        //user has loaded a form in a disabled state, enable it and start timer
        private void Start_Click(object sender, RoutedEventArgs e)
        {
          
            rootGrid.IsEnabled = true;
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
          //  if (propsnotinDB[block.Name].bCompleted)
            propsnotinDB[block.Name].bActive = true;
            passwordEntered.IsEnabled = true;
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // MessageBox.Show("work was not copleted in time, manager will be notified", "Severe Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            //  timer.Stop();
           
            foreach (Block blok in blocks)
            {
                if (propsnotinDB[blok.Name].bActive)
                {
                    
                    if (blok.TimeLefttoComplete == 0 && !propsnotinDB[blok.Name].bMessageShown)
                    {
                        blok.Warning = false;
                        blok.TimedOut = true;
                        MessageBox.Show("work was not completed in time, manager notified", "Severe Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                       
                        propsnotinDB[blok.Name].bMessageShown = true;
                        TextManager(blok);
                        return;
                    }
                    if (blok.TimeLefttoComplete == 0)   //message was shown but still allow the user to finish block
                        return;
                    blok.TimeLefttoComplete -= 1;
                    if (blok.TimeLefttoComplete > 0 && blok.TimeLefttoComplete < 120)   //2 minute warning
                        blok.Warning = true;
                   
                }

            }
        //    CheckForFinish();


        }
        void CheckForFinish()
        { 
            double numLeft = lines.Where(l => l.type != 3).Count();
            if (numLeft == 0)
            {
                
                timer.Stop();
                currentBlockIndex += 1;
                if (currentBlockIndex == BlockNames.Count())
                {
                    MessageBox.Show("all work done work ", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                   
                }
                selectedBlock = BlockNames[currentBlockIndex];
                timeElapsed = 0;
                Start.Visibility = Visibility.Visible;
                itemNumber = -1;
          //      Progress.Value = 0;
                Refresh();
                rootGrid.IsEnabled = false;
                passwordEntered.IsEnabled = true;
                return;

            }
            
        }

      
        private void BlockInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentBlockIndex = blockinfo.SelectedIndex;
            if (BlockNames == null)
                return;
            selectedBlock = BlockNames[currentBlockIndex];
            Start.Content = "Start";
            if (propsnotinDB[selectedBlock].bActive)
                Start.Content = "Resume";
            Start.Visibility = Visibility.Visible;
            itemNumber = -1;
            Refresh();
            rootGrid.IsEnabled = false;
        }


        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            Admin dlg = new Admin(blocks);
            dlg.ShowDialog();
            Refresh();
        }

        public  T FindChild<T>(DependencyObject parent, string childName)
          where T : DependencyObject
        {
            // Confirm parent and childName are valid.   
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child  
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree  
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child.   
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search  
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name  
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.  
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            Add.Visibility = Visibility.Collapsed;
            Configure.Visibility = Visibility.Collapsed;
            logout.Visibility = Visibility.Collapsed;
            bEntryMode = true;
        }

        private void chooseBlock_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;
            StackPanel sp = but.Content as StackPanel;
            string blockName = string.Empty;
            for (int i = 0; i < sp.Children.Count; i++)
            {
                UIElement element = sp.Children[i];
                TextBlock tb = sp.Children[i] as TextBlock;
                if (tb != null && tb.Name == "button")
                {
                    blockName = tb.Text;
                    break;
                }
            }
                if (blockName == string.Empty ||blockName == selectedBlock)
                    return;
                selectedBlock = blockName;
           
          
                Start.Visibility = Visibility.Visible;
                itemNumber = -1;
                Start.Content = "Start";
                if (propsnotinDB[blockName].bActive)
                    Start.Content = "Resume";
                Refresh();
          
        }
        void TextManager(Block blok)
        {
            var smtpServerName = ConfigurationManager.AppSettings["SmtpServer"];
            var port = ConfigurationManager.AppSettings["Port"];
            var senderEmailId = ConfigurationManager.AppSettings["SenderEmailId"];
            var senderPassword = ConfigurationManager.AppSettings["SenderPassword"];
            var smptClient = new SmtpClient(smtpServerName, Convert.ToInt32(port))
            {
                Credentials = new NetworkCredential(senderEmailId, senderPassword),
                EnableSsl = true
            };
            string[]  unfinished = blok.questions.Where(q => q.linenum > blok.CurrentItem).OrderBy(l => l.linenum).Select(l=>l.label1).ToArray();
            string blockName = string.Format("Block{0} ", blok.Name);
            
            var someString = String.Join(
                Environment.NewLine, unfinished );
            smptClient.Send(senderEmailId, ConfigurationManager.AppSettings["TextReceiver1"], blockName, someString);
        }

        private void SetTime_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    class info
    {
        public int lastLineNumber { get; set; }
        public double TimeLeft  { get; set; }
        public bool bActive { get; set; }
        public bool bMessageShown { get; set; }
    }
   
    
    public class TimeDisplay : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            TimeSpan t = TimeSpan.FromSeconds((double)value);
            string str = "Time left " + t.ToString(@"h\:mm\:ss");
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DisplayItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = string.Format("current item {0} " ,value);
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   

}
