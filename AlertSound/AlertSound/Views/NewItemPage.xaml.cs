using AlertSound.Models;
using AlertSound.ViewModels;
using Xamarin.Forms;

namespace AlertSound.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Events Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}