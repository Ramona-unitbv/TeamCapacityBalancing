﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using TeamCapacityBalancing.Services;

namespace TeamCapacityBalancing.Models;

public class Wrapper<T> : Utility
{
    public T? _value;
    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            NotifyPropertyChanged();
        }
    }
    public Wrapper()
    {
    }
}

public partial class UserStoryAssociation : ObservableObject
{
    public IssueData StoryData { get; set; }

    [ObservableProperty]
    public bool _shortTerm;
    public float Remaining { get; set; }

    private ObservableCollection<Wrapper<float>> _days;
    public ObservableCollection<Wrapper<float>> Days
    {
        get => _days;
        set
        {
            _days = value;
            OnPropertyChanged();
        }
    }


    public Wrapper<float> Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, List<float> days)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        _days = new(days.Select(x => new Wrapper<float>() { Value = x }));
        Coverage = new Wrapper<float>() { Value = 0 };
    }

    public void CalculateCoverage()
    {
        Coverage.Value = Days.Sum(x => x.Value);
    }
}
