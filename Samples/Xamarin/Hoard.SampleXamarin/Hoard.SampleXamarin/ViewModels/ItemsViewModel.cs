using F3N.Hoard.Extensions;
using F3N.YaMVVM.ViewModel;
using Hoard.SampleXamarin.Models;
using Hoard.SampleXamarin.Store;
using Hoard.SampleXamarin.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace Hoard.SampleXamarin.ViewModels
{
    public class ItemsViewModel : PageViewModel, INotifyPropertyChanged
    {
        private ItemStore _itemStore;
        private IDisposable _subscription;

        public ObservableCollection<Item> Items { get; set; }
        public Command RefreshItemsCommand { get; set; }

        public Command AddItem { get; set; }

        public Command SelectItem { get; set; }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<Item>();
            Title = "Browse";
            RefreshItemsCommand = new Command(async () => await LoadItems());
            AddItem = new Command(async () => await this.PushModalPage<NewItemPage>(new NewItemViewModel(), toolbar: true));
            SelectItem = new Command(async (param) => {
                if (param is Item item)
                {
                    await this.PushPage<ItemDetailPage>(new ItemDetailViewModel(item.Id));
                }
            });
        }

        public override async Task Initialise()
        {
            _itemStore = await ItemStore.Instance;
            _subscription = _itemStore.ObserveWithEvents().Subscribe(async payload => {
                switch(payload.@event)
                {
                    case Store.Events.ItemAdded ia:
                        Items.Add(payload.State);
                        break;
                    case Store.Events.ItemRemoved ir:
                        var lookup = Items.FirstOrDefault(i => i.Id == payload.State.Id);
                        if (lookup != null)
                            Items.Remove(lookup);
                        break;
                    case Store.Events.TitleUpdated tu:
                        var titleLookup = Items.FirstOrDefault(item => item.Id == payload.State.Id);
                        if(titleLookup != null)
                            titleLookup.Text = payload.State.Text;
                        break;
                    case Store.Events.DescriptionUpdated du:
                        var descLookup = Items.FirstOrDefault(item => item.Id == payload.State.Id);
                        if(descLookup != null)
                            descLookup.Description = payload.State.Description;
                        break;
                    case Store.Events.ItemNotFoundOccured inf:
                        await DisplayAlert("Error", "Could not find item", "OK");
                        break;
                }
            });

            await LoadItems();
            await base.Initialise();
        }

        async Task LoadItems()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await ItemStore.State;

                if(items.Count == 0)
                {
                    await BootstrapData();
                }

                foreach (var item in items)
                {
                    if(!Items.Contains(item)) Items.Add(item);
                }

                Items.Add(new Item() { Id = Guid.NewGuid(), Text = "Non-existing item", Description = "Non-existing item description" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task BootstrapData()
        {
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 1", "Item 1 description"));
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 2", "Item 2 description"));
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 3", "Item 3 description"));
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 4", "Item 4 description"));
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 5", "Item 5 description"));
            await _itemStore.Dispatch(new Store.Commands.AddItem(Guid.NewGuid(), "Item 6", "Item 6 description"));
        }

        public override Task Destroy()
        {
            _subscription.Dispose();
            return base.Destroy();
        }
    }
}