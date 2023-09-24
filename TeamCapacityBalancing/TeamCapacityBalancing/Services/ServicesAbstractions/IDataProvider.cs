using System.Collections.Generic;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    public interface IDataProvider
    {
        public void RefreshTeamLeader(User teamLeader);
        public Dictionary<int, IssueData> GetAllEpicsByTeamLeader(User teamLeader);
        public Dictionary<int, IssueData> GetAllStoriesByTeamLeader(User teamLeader);
        public Dictionary<int, IssueData> GetOpenTasksForMembersByTeamLeader(User teamLeader);
        public List<User> GetAllUsers();
        public List<OpenTasksUserAssociation> GetRemainingForUser();
        public List<User> GetAllTeamLeaders();
    }
}
