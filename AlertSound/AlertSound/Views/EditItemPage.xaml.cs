using AlertSound.ViewModels;
using Xamarin.Forms;

namespace AlertSound.Views
{
    public partial class EditItemPage : ContentPage
    {
        public EditItemPage()
        {
            InitializeComponent();
            BindingContext = new EditItemViewModel();
        }
    }
}