using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class Team
{
    public List<User> Users { get; set; } 
    public Team(List<User> userList)
    {
        Users = userList;
    }
}
