using F3N.Hoard.State;
using System;
using System.ComponentModel;

namespace Hoard.SampleXamarin.Models
{
    public class Item : IStatefulCollectionItem, INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}