﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NearbyMessagesApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(this, "Publish");
        }
        void Buttona_Clicked(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(this, "Subscribe");
        }
    }
}
