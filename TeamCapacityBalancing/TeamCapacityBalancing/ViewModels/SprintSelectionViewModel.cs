using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class SprintSelectionViewModel : ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    public List<PageData> Pages { get; }

    [ObservableProperty]
    private int _isShortTermVisible = 0;

    [ObservableProperty]
    private int _numberOfSprints = 0;

    public ObservableCollection<Sprint> Sprints { get; set; } = new ObservableCollection<Sprint>()
    {};

    [ObservableProperty]
    private DateTime _finishDate;

    public SprintSelectionViewModel() 
    {
    
    }

    public SprintSelectionViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        
    }

    [RelayCommand]
    public void GenerateSprints()
    {
        for(int i=0;i<NumberOfSprints;i++)
        {
            Sprints.Add(new Sprint($"Sprint {i+1}", 0, false));
        }
    }

    [RelayCommand]
    public void OpenBalancingPage() 
    {
        _navigationService.CurrentPageType = typeof(BalancingPage);
    }
}
