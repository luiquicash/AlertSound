using AlertSound.Models;
using AlertSound.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AlertSound.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            Item.From = e.NewDate;
        }

        private void DatePicker_DateSelected_1(object sender, DateChangedEventArgs e)
        {
            Item.To = e.NewDate;
        }
    }
}