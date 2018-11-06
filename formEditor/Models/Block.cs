using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formEditor.Models
{
    public class Block : INotifyPropertyChanged
    {
        string name;
        double timeLefttoComplete;
        int currentItem;
        bool selected;
        public int Id { get; set; }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
       
        public string Description { get; set; }
        public double timer { get; set; }
        public double TimeLefttoComplete
        {
            get { return timeLefttoComplete; }
            set
            {
                timeLefttoComplete = value;
                NotifyPropertyChanged("TimeLefttoComplete");
            }
        }
        public int CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                NotifyPropertyChanged("CurrentItem");
            }
        }
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyPropertyChanged("Selected");
            }
        }

        public List<FormEntry> questions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
