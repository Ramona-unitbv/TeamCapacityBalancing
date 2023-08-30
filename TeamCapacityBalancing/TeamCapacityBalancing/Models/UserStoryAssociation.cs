using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.ComponentModel;

namespace TeamCapacityBalancing.Models;

public partial class UserStoryAssociation : ObservableObject, INotifyPropertyChanged
{
    public IssueData StoryData { get; set; }
    [ObservableProperty]
    public bool _shortTerm;
    public float Remaining { get; set; }

    private List<float> _days;
    public List<float> Days
    {
        get => _days;
        set
        {
            if (_days != value)
            {
                _days = value;
                OnPropertyChanged();
            }
        }
    }
    public float Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, List<float> days, float coverage)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        Days = days;
        Coverage = coverage;
    }
}