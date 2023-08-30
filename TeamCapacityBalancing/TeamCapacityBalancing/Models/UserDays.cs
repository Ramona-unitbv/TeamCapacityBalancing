using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class UserDays
{
    public User User { get; set; }
    public float Days { get; set; }
    public UserDays(User user, float days)
    {
        User = user;
        Days = days;
    }
}
