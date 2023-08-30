using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.Postgres_connection;
using TeamCapacityBalancing.ViewModels;

namespace TeamCapacityBalancing.Views
{
    public partial class BalancingPage : UserControl
    {
        public BalancingPage()
        {
            InitializeComponent();
        }
    }
}
