using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TeamCapacityBalancing.Models
{
    public class UserStoryDataSerialization
    {
        public IssueData Story { get; set; }
   
        public bool ShortTerm { get; set; }

        public float Remaining { get; set; }  
       
        public List<Tuple<User,float>> UsersCapacity { get; set; } = new List<Tuple<User, float>>();

        public UserStoryDataSerialization(IssueData story, bool shortTerm, float remaining, List<Tuple<User, float>> usersCapacity)
        {
            Story = story;
            ShortTerm = shortTerm;
            Remaining = remaining;
            UsersCapacity = usersCapacity;
        }

        public UserStoryDataSerialization()
        {
        }
    }

    
}
