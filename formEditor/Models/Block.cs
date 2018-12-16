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
        string description;
        double timeLefttoComplete;
        int currentItem;
        bool selected;
        bool warning;
        bool timedOut;
        int state { get; set; }
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
       
        public string Description {
            get { return description; }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }
        public double timer { get; set; }
        public TimeSpan ExpectedStart { get; set; }
        public bool bExpectedMessageSent { get; set; }
        public bool bWorkNotFinishedinTime { get; set; }
        public bool bnMinuteWarningSent { get; set; }
        public bool bActive { get; set; }
        public int  State {
            get { return state; }
            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }
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
        public bool Warning
        {
            get { return warning; }
            set
            {
                warning = value;
                NotifyPropertyChanged("Warning");
            }
        }
        public bool TimedOut
        {
            get { return timedOut; }
            set
            {
                timedOut = value;
                NotifyPropertyChanged("TimedOut");
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
