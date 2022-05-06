using AlertSound.ViewModels;
using Xamarin.Forms;

namespace AlertSound.Views
{
    public partial class AlertPage : ContentPage
    {
        public AlertPage()
        {
            InitializeComponent();
            BindingContext = new AlertPageViewModel();
        }
    }
}