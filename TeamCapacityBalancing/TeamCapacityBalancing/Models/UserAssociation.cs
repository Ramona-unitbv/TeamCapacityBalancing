using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;


public sealed partial class UserAssociation : ObservableObject
{
    public User User { get; set; }
    public float Days { get; set; }
    public bool IsShow { get; set; }

    public UserAssociation(User user, float days, bool isShow)
    {
        User = user;
        Days = days;
        IsShow = isShow;
    }
}
