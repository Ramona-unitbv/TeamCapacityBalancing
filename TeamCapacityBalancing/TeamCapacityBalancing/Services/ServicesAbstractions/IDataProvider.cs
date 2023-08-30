using System.Collections.Generic;
using TeamCapacityBalancing.Models;

namespace TeamCapacityBalancing.Services.ServicesAbstractions
{
    public interface IDataProvider
    {
        public List<IssueData> GetAllEpicsByTeamLeader(User teamLeader);
        public List<IssueData> GetAllStoriesByTeamLeader(string teamLeaderUsername);
        public List<User> GetAllUsers();
        public List<IssueData> GetStoriesByEpic(int epicId);
    }
}
