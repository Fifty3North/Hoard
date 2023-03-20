using Maui.ViewModels;

namespace Maui;

public partial class MainPage : ContentPage
{
    private CounterViewModel _counterViewModel;

    public MainPage(CounterViewModel counterViewModel)
	{
        _counterViewModel = counterViewModel;
        BindingContext = counterViewModel;
        InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        await _counterViewModel.Initialise();
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        _counterViewModel.Dispose();
        base.OnDisappearing();
    }
}

