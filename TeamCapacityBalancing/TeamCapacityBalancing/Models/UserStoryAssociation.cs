using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services;

namespace TeamCapacityBalancing.Models;

public class Wrapper<T>
{
    public T Value { get; set; } = default;
    public Wrapper()
    {
    }
}

public class UserStoryAssociation : Utility
{
    public IssueData StoryData { get; set; }
    public bool ShortTerm { get; set; }
    public float Remaining { get; set; }

    private ObservableCollection<Wrapper<float>> _days;
    public ObservableCollection<Wrapper<float>> Days
    {
        get => _days;
        set
        {
            _days = value;
            NotifyPropertyChanged();
        }
    }


    public float Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, List<float> days)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        Days = new(days.Select(x => new Wrapper<float>() { Value = x}));
        Coverage = 0;
    }
}
