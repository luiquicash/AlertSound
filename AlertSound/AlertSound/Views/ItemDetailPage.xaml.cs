﻿using AlertSound.ViewModels;
using Xamarin.Forms;

namespace AlertSound.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}