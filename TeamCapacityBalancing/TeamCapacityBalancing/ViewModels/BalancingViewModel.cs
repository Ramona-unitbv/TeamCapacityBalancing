using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Navigation;
using TeamCapacityBalancing.Views;

namespace TeamCapacityBalancing.ViewModels;

public sealed partial class BalancingViewModel : ObservableObject
{
    private readonly PageService _pageService;
    private readonly NavigationService _navigationService;
    public List<PageData> Pages { get; }

    [ObservableProperty]
    public List<User> _team;

    [ObservableProperty]
    public List<float> _number=new() { 0,0,0,0,0,0,0,0,0,0};

    public BalancingViewModel()
    {

    }

    public BalancingViewModel(PageService pageService, NavigationService navigationService)
    {
        _pageService = pageService;
        _navigationService = navigationService;
        Pages = _pageService.Pages.Select(x => x.Value).Where(x => x.ViewModelType != this.GetType()).ToList();
        ShowShortTermStoryes();
    }

    [ObservableProperty]
    private bool _isShortTermVisible = true;

    [ObservableProperty]
    private bool _isBalancing = true;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty]
    private bool _sumsOpen = true;

    [ObservableProperty]
    private SplitViewDisplayMode _mode = SplitViewDisplayMode.CompactInline;

    public ObservableCollection<UserStoryAssociation> MyUserAssociation { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
           new UserStoryAssociation(
                new IssueData("Sample Story 1", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                80.0f
            ),
            new UserStoryAssociation(
                new IssueData("Sample Story 2", 8.0f, "Release 2", "Sprint 2", true, IssueData.IssueType.Story),
                false,
                5.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                60.0f
            ),

             new UserStoryAssociation(
                new IssueData("Sample Story 3", 8.0f, "Release 2", "Sprint 2", true, IssueData.IssueType.Story),
                true,
                5.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                60.0f
            ),

              new UserStoryAssociation(
                new IssueData("Sample Story 4", 8.0f, "Release 2", "Sprint 2", true, IssueData.IssueType.Story),
                true,
                5.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                60.0f
            ),
    };

    public ObservableCollection<UserStoryAssociation> Balancing { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
           new UserStoryAssociation(
                new IssueData("Balancing", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                80.0f
            ),
    };


    public ObservableCollection<UserStoryAssociation> Totals { get; set; } = new ObservableCollection<UserStoryAssociation>
    {
       new UserStoryAssociation(
                new IssueData("Total work open story", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                80.0f
            ),
       new UserStoryAssociation(
                new IssueData("Total work", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                80.0f
            ),
       new UserStoryAssociation(
                new IssueData("Total capacity", 5.0f, "Release 1", "Sprint 1", true, IssueData.IssueType.Story),
                true,
                3.0f,
                new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                80.0f
            ),
    };

    public ObservableCollection<IssueData> Epics { get; set; } = new() { new("Epic 1"), new("Epic 2"), new("Epic 3") };

    public Team TeamMembers { get; set; } = new Team(new List<User>
            {
                new User("user1", "User One", 1, true),
                new User("user2", "User Two", 2, true),
                new User("user3", "User Three", 3, true),
                new User("user4", "User Four", 3, false),
                new User("user5", "User Five", 3, false),
                new User("user6", "User Six", 3, false),
                new User("user7", "User Seven", 3,false),
                new User("user8", "User Eight", 3,false),
                new User("user9", "User Nine", 3,false),
                new User("user10", "User Ten", 3,false),
            });

    [ObservableProperty]
    public ObservableCollection<UserStoryAssociation> _shortTermStoryes;

    [RelayCommand]
    public void OpenTeamPage()
    {
        _navigationService.CurrentPageType = typeof(TeamPage);
    }

    [RelayCommand]
    public void OpenSprintSelection() 
    {
        _navigationService.CurrentPageType = typeof(SprintSelectionPage);
    }

    [RelayCommand]
    public void ShowShortTermStoryes() 
    {

       
        ShortTermStoryes = new();

        for (int i = 0; i < MyUserAssociation.Count; i++) 
        {
            if (MyUserAssociation[i].ShortTerm) 
            {
                ShortTermStoryes.Add(MyUserAssociation[i]);
            }
        }

   
    }

    [RelayCommand]
    public void UncheckShortTermStory(string id) 
    {
        for (int i = 0; i < MyUserAssociation.Count; i++) 
        {
            if (MyUserAssociation[i].StoryData.Name == id) 
            {
                MyUserAssociation[i].ShortTerm = false;
                ShortTermStoryes.Remove(MyUserAssociation[i]);
            }
        }
    }
}
