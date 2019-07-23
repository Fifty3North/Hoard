using F3N.Hoard.Extensions;
using F3N.YaMVVM.ViewModel;
using Hoard.SampleXamarin.Models;
using Hoard.SampleXamarin.Store;
using Splat;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hoard.SampleXamarin.ViewModels
{
    public class NewItemViewModel : PageViewModel
    {
        private ItemStore _itemStore;
        private IDisposable _subscription;
        private Guid _id;
        private bool _editing;
        private Item _item;

        public Command Save => new Command(async () => {
            if (!_editing)
            {
                await _itemStore.Dispatch(new Store.Commands.AddItem(_id, Text, Description));
            }
            else
            {
                if (Text != _item.Text)
                {
                    await _itemStore.Dispatch(new Store.Commands.UpdateTitle(_id, Text));
                }
                if(Description != _item.Description)
                {
                    await _itemStore.Dispatch(new Store.Commands.UpdateDescription(_id, Description));
                }
            }
        });

        public Command Cancel => new Command(async () => {
            await PopPage();
        });

        public string Text { get; set; }
        public string Description { get; set; }

        public NewItemViewModel(Item item = null)
        {
            _itemStore = Locator.Current.GetService<ItemStore>();

            if (item == null)
            {
                _id = Guid.NewGuid();
            }
            else
            {
                _id = item.Id;
                _editing = true;
                _item = item;
            }

            Title = item?.Text;
            Text = item?.Text;
            Description = item?.Description;
        }

        public override async Task Initialise()
        {
            await _itemStore.Initialise();

            _subscription = _itemStore.ObserveWhereWithEvents(ev => ev.Id == _id).Subscribe(async payload => {
                switch(payload.@event)
                {
                    case Store.Events.ItemAdded success:
                    case Store.Events.TitleUpdated tu:
                    case Store.Events.DescriptionUpdated du:
                        await this.PopPage();
                        break;
                    case Store.Events.ItemAddFailed failed:
                        await this.ModelPage.DisplayAlert("Error adding item", failed.Reason, "OK");
                        break;
                    
                }
            });

            await base.Initialise();
        }

        public override Task Destroy()
        {
            _subscription.Dispose();
            return base.Destroy();
        }
    }
}
