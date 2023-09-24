using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.PostgresConnection;
using TeamCapacityBalancing.Services.ServicesAbstractions;

namespace TeamCapacityBalancing.Services.Postgres_connection
{
    public class TeamLeaderInfo
    {
        private readonly string _name;
        private Dictionary<int, IssueData> _epics = new Dictionary<int, IssueData>();
        private Dictionary<int, IssueData> _stories = new Dictionary<int, IssueData>();
        private Dictionary<int, IssueData> _tasks = new Dictionary<int, IssueData>();

        public TeamLeaderInfo(string name) { _name = name; }

        public string Name { get { return _name; } }
        public Dictionary<int, IssueData> Epics { get => _epics; set { _epics = value; } }
        public Dictionary<int, IssueData> Stories { get => _stories; set { _stories = value; } }
        public Dictionary<int, IssueData> Tasks { get => _tasks; set { _tasks = value; } }

    }
    public class QueriesForDataBase : IDataProvider
    {
        private const string JiraissueTable = "jiraissue";
        private const string IssuelinkTable = "issuelink";
        private const string CustomFieldTable = "Customfieldvalue";
        private const string UserTable = "cwd_user";
        private const string StoryIssueType = "10001";
        private const string EpicStoryLinkType = "10201";
        private const string StoryTaskLinkType = "10100";
        private const string SubTaskIssueType = "10003";
        private const string OpenStatus = "1";

        private List<User> teamLeaders = new List<User>();
        private Dictionary<string, TeamLeaderInfo> teamLeadersInfos = new Dictionary<string, TeamLeaderInfo>();

        public void RefreshTeamLeader(User teamLeader)
        {
            teamLeadersInfos.Remove(teamLeader.Username);
        }

        private TeamLeaderInfo GetTeamLeaderInfo(string teamLeader)
        {
            TeamLeaderInfo? teamLeaderInfo;
            teamLeadersInfos.TryGetValue(teamLeader, out teamLeaderInfo);

            if (teamLeaderInfo == null)
            {
                teamLeaderInfo = new TeamLeaderInfo(teamLeader);
                teamLeadersInfos.Add(teamLeader, teamLeaderInfo);
            }

            return teamLeaderInfo;
        }
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {

                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new UsersQuery()))
                    return users;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    string username = item.GetString("user_name");
                    string displayName = item.GetString("display_name");
                    int id = item.GetInt("id");
                    users.Add(new User(username, displayName, id));

                    item = dBConnection.NextRow();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return users;
        }


        public Dictionary<int, IssueData> GetAllStoriesByTeamLeader(User teamLeader)
        {
            TeamLeaderInfo teamLeaderInfo = GetTeamLeaderInfo(teamLeader.Username);

            if (teamLeaderInfo.Stories.Count > 0)
            {
                return teamLeaderInfo.Stories;
            }

            Dictionary<int, IssueData> stories = new Dictionary<int, IssueData>();

            try
            {
                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new StoriesForPLQuery(teamLeader.Username)))
                    return stories;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    int id = item.GetInt("id");
                    string name = item.GetString("summary");
                    string assignee = item.GetString("assignee");
                    int issueNumber = item.GetInt("issuenum");
                    int parentId = item.GetInt("parentId");
                    float remaining = item.GetInt("timeestimate") / 60 / 60 / 8; //from seconds to days
                    Math.Round(remaining, 2);

                    //if (remaining > 0)
                    //{
                        stories.Add(id, new IssueData(id, name, assignee, remaining, parentId));
                    //}
                    item = dBConnection.NextRow();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            teamLeaderInfo.Stories = stories;

            return stories;
        }
        public Dictionary<int, IssueData> GetAllEpicsByTeamLeader(User teamLeader)
        {
            TeamLeaderInfo teamLeaderInfo = GetTeamLeaderInfo(teamLeader.Username);

            if (teamLeaderInfo.Epics.Count > 0)
            {
                return teamLeaderInfo.Epics;
            }

            Dictionary<int, IssueData> epics = new Dictionary<int, IssueData>();

            try
            {
                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new EpicsForPLQuery(teamLeader.Username)))
                    return epics;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    int id = item.GetInt("id");
                    string name = item.GetString("summary");
                    int issueNumber = item.GetInt("issuenum");
                    string businesscase = item.GetString("customvalue");
                    epics.Add(id, new IssueData(id, name, businesscase));
                    
                    item = dBConnection.NextRow();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            teamLeaderInfo.Epics = epics;

            return epics;
        }

        public Dictionary<int, IssueData> GetOpenTasksForMembersByTeamLeader(User teamLeader)
        {
            TeamLeaderInfo teamLeaderInfo = GetTeamLeaderInfo(teamLeader.Username);

            if (teamLeaderInfo.Tasks.Count > 0)
            {
                return teamLeaderInfo.Tasks;
            }

            Dictionary<int, IssueData> tasks = new Dictionary<int, IssueData>();

            try
            {
                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new OpenTasksForMembersByPLQuery(teamLeader.Username)))
                    return tasks;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    int id = item.GetInt("id");
                    string name = item.GetString("summary");
                    string assignee = item.GetString("assignee");
                    int parentId = item.GetInt("parentId");
                    float remaining = item.GetInt("timeestimate") / 60 / 60 / 8;

                    Math.Round(remaining, 2);

                    //if (remaining > 0)
                    //{
                    tasks.Add(id, new IssueData(id, name, assignee, remaining, parentId));
                    //}
                    item = dBConnection.NextRow();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            teamLeaderInfo.Tasks = tasks;

            return tasks;
        }

        
        public List<User> GetAllTeamLeaders()
        {
            if (teamLeaders.Count > 0)
                return teamLeaders;

            try
            {
                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new PLQuery()))
                    return teamLeaders;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    int id = item.GetInt("id");
                    string username = item.GetString("user_name");
                    string displayName = item.GetString("display_name");
                    teamLeaders.Add(new User(username, displayName, id));
                    item = dBConnection.NextRow();
                }

                dBConnection.DisconnectFromJira();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return teamLeaders;
        }
        public List<OpenTasksUserAssociation> GetRemainingForUser()
        {
            List<OpenTasksUserAssociation> openTasks= new List<OpenTasksUserAssociation>();
            try
            {
                DataBaseConnectionBase dBConnection = DataBaseConnectionBaseFactory.GetMeTheRightConnection();
                dBConnection.ConnectToJira();

                if (!dBConnection.RunQuery(new RemainingTimeUserQuery()))
                    return openTasks;

                var item = dBConnection.NextRow();
                while (item != null)
                {
                    int id = item.GetInt("id");
                    string username = item.GetString("user_name");
                    string displayName = item.GetString("display_name");
                    User user = new User(username, displayName, id);
                    double remaining = item.GetDouble("totalremaining");
                    openTasks.Add(new OpenTasksUserAssociation(user, (float)Math.Round(remaining, 2)));

                    item = dBConnection.NextRow();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return openTasks;
        }
    }
}
