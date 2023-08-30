using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models;

public class Team
{
    public List<User> Users { get; set; } = new List<User>() { new("user1", "User One", 1), new("user2", "User Two", 2), new("user3", "User Three", 3) };
    public Team(List<User> userList)
    {
        Users = userList;
    }
}
