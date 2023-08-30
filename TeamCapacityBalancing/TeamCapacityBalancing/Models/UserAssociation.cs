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
    public List<float> Days { get; set; }
    public bool IsShow { get; set; }

    public UserAssociation(User user, List<float> days, bool isShow)
    {
        User = user;
        Days = days;
        IsShow = isShow;
    }
}
