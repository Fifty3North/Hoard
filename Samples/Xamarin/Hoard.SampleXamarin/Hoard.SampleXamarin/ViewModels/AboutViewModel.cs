using F3N.YaMVVM.ViewModel;
using Hoard.SampleXamarin.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hoard.SampleXamarin.ViewModels
{
    public class AboutViewModel : PageViewModel
    {
        public int RabbitHoleDepth { get; set; }

        public AboutViewModel() : this(0)
        {
            
        }

        public AboutViewModel(int rabbitHoleDepth = 0)
        {
            RabbitHoleDepth = rabbitHoleDepth;

            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));

            PushPageCommand = new Command(async () => await this.PushPage<AboutPage>(new AboutViewModel(++RabbitHoleDepth)));
            PushModalPageCommand = new Command(async() => await this.PushModalPage<AboutPage>(new AboutViewModel(++RabbitHoleDepth), true));
        }

        public ICommand OpenWebCommand { get; }
        public Command PushPageCommand { get; private set; }
        public Command PushModalPageCommand { get; private set; }
    }
}