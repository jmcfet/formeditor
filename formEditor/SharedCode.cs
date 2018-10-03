using formEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace formEditor
{
    public static class SharedCode
    {
        static Grid rootGrid;
        public static void setRoot(Grid root)
        {
            rootGrid = root;
        }
        public static void HandleType1(FormEntry item, bool bEditMode)
        {
            int row = 0;
            Label lb;
            if (bEditMode)
                rootGrid.Children.Add(CreateTextInputBlock(item.label1, row, 0));
            else
            {
                lb = CreateLabel(row, 0, "enter text here");
                rootGrid.Children.Add(lb);
            }
            CheckBox cb = CreateCheckBox(row, 1, item.checkbox1);
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            rootGrid.Children.Add(cb);
            cb = CreateCheckBox(row, 2, item.checkbox2);
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
        private  static TextBox CreateTextInputBlock(string text, int row, int column)
        {
            TextBox tb = new TextBox() { Text = text, Margin = new Thickness(5, 8, 0, 5) };


            var bc = new BrushConverter();
            tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

    }
}
