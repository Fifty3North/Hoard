using F3N.Hoard.Extensions;
using F3N.YaMVVM.ViewModel;
using Hoard.SampleXamarin.Models;
using Hoard.SampleXamarin.Store;
using Hoard.SampleXamarin.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hoard.SampleXamarin.ViewModels
{
    public class ItemDetailViewModel : PageViewModel, INotifyPropertyChanged
    {
        private Item _item;
        private ItemStore _itemStore;
        private IReadOnlyCollection<Item> _itemState;
        private IDisposable _subscription;
        private Guid _id;

        public string Text { get; set; }
        public string Description { get; set; }

        public Command EditCommand => new Command(async () => await this.PushPage<NewItemPage>(new NewItemViewModel(_item)));

        public ItemDetailViewModel(Guid id)
        {
            _id = id;
        }

        private async Task LoadItem()
        {
            _item = _itemState.FirstOrDefault(i => i.Id == _id);

            if (_item == null)
            {
                await Task.Delay(100);

                await DisplayAlert("Error", "Item not found", "OK");
                
                await PopPage();
                return;
            }

            Title = _item.Text;
            Text = _item.Text;
            Description = _item.Description;
        }

        public override async Task Initialise()
        {
            _itemStore = await ItemStore.Instance;
            _itemState = await ItemStore.State;

            _subscription = _itemStore.ObserveWhere(item => item.Id == _item.Id).Subscribe(async (item) => {
                _item = item;
                await LoadItem();
            });

            await LoadItem();

            await base.Initialise();
        }

        public override Task Destroy()
        {
            _subscription.Dispose();
            return base.Destroy();
        }
    }
}
