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
        public int StoryId { get; set; }
   
        public bool ShortTerm { get; set; }
       
        public List<Tuple<User,float>> UsersCapacity { get; set; } = new List<Tuple<User, float>>();

    }

    
}
