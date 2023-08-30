using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class UserStoryAssociation
{
    public IssueData StoryData { get; set; }
    public bool ShortTerm { get; set; }
    public float Remaining { get; set; }
    public UserAssociation UserAndDays { get; set; }
    public float Coverage { get; set; }
    public UserStoryAssociation(IssueData storyData, bool shortTerm, float remaining, UserAssociation userAndDays, float coverage)
    {
        StoryData = storyData;
        ShortTerm = shortTerm;
        Remaining = remaining;
        UserAndDays = userAndDays;
        Coverage = coverage;
    }
}