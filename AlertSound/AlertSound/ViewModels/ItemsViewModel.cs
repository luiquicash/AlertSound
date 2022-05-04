using AlertSound.Models;
using AlertSound.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AlertSound.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Events _selectedItem;
        private Events _updateItem;
        private Events _deletedItem;
        public ObservableCollection<Events> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Events> ItemTapped { get; }
        public Command<Events> ItemDelete { get; }
        public Command<Events> ItemUpdate { get; }
        public ItemsViewModel()
        {
            Title = "Lista de Alarmas";
            Items = new ObservableCollection<Events>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Events>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            ItemDelete = new Command<Events>(OnDeleteItem);

            ItemUpdate = new Command<Events>(OnEditItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.Data.GetEventListsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
            DeletedItem = null;
        }

        public Events SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }
        public Events UpdateItem
        {
            get => _updateItem;
            set
            {
                SetProperty(ref _updateItem, value);
                OnEditItem(value);
            }
        }
        public Events DeletedItem
        {
            get => _deletedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnDeleteItem(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        private async void OnDeleteItem(Events item)
        {
            if (item == null)
                return;

            await App.Data.DeleteEventAsync(item.Id);
            await ExecuteLoadItemsCommand();
        }

        private async void OnEditItem(Events item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(EditItemPage)}?{nameof(EditItemViewModel.ItemId)}={item.Id}");
        }

        async void OnItemSelected(Events item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}