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
        private List<IssueData> _epics = new List<IssueData>();
        private List<IssueData> _stories = new List<IssueData>();
        public TeamLeaderInfo(string name) { _name = name; }

        public string Name { get { return _name; } }
        public List<IssueData> Epics { get => _epics; set { _epics = value; } }
        public List<IssueData> Stories { get => _stories; set { _stories = value; } }

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

        private float CalculateRemainingTimeForStory(int storyId)
        {
            float timeEstimate = 0;
            float timeSpent = 0;
            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand($"SELECT {JiraissueTable}.timeestimate, {JiraissueTable}.timespent " +
                        $"FROM {JiraissueTable} " +
                        $"WHERE {JiraissueTable}.id = {storyId} ", connection);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        timeEstimate += reader.GetInt32(reader.GetOrdinal("timeestimate"));
                        timeSpent += reader.GetInt32(reader.GetOrdinal("timespent"));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return (timeEstimate +timeSpent)-timeSpent;
        }

        //private int GetEpicIdFromStory(int storyId)
        //{
        //    int epicId = 0;
        //    try
        //    {
        //        using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
        //        {
        //            connection.Open();

        //            var cmd = new NpgsqlCommand($"SELECT {IssuelinkTable}.source " +
        //                $"FROM {IssuelinkTable} " +
        //                $"WHERE {IssuelinkTable}.linktype = {EpicStoryLinkType} " +
        //                $"AND {IssuelinkTable}.destination = {storyId}", connection);
        //            var reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                epicId = reader.GetInt32(reader.GetOrdinal("source"));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }

        //    return epicId;
        //}

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
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand("SELECT user_name, display_name, id " +
                        "FROM " + UserTable, connection);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string username = reader.GetString(reader.GetOrdinal("user_name"));
                        string displayName = reader.GetString(reader.GetOrdinal("display_name"));
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        users.Add(new User(username, displayName, id));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return users;
        }


        public List<IssueData> GetAllStoriesByTeamLeader(User teamLeader)
        {
            TeamLeaderInfo teamLeaderInfo = GetTeamLeaderInfo(teamLeader.Username);

            if (teamLeaderInfo.Stories.Count > 0)
            {
                return teamLeaderInfo.Stories;
            }

            List<IssueData> stories = new List<IssueData>();

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
                    int epicId = item.GetInt("epicId");

                    float remaining = 100; // ((CalculateRemainingTimeForStory(id) / 60) / 60) / 8; //from seconds to days
                    Math.Round(remaining, 2);

                    if (remaining > 0)
                    {
                        stories.Add(new IssueData(id, name, assignee, remaining, epicId));
                    }
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

        public List<IssueData> GetAllEpicsByTeamLeader(User teamLeader)
        {
            TeamLeaderInfo teamLeaderInfo = GetTeamLeaderInfo(teamLeader.Username);

            if (teamLeaderInfo.Epics.Count > 0)
            {
                return teamLeaderInfo.Epics;
            }

            List<IssueData> epics = new List<IssueData>();

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
                    epics.Add(new IssueData(id, name, businesscase));
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
                //using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                //{
                //    connection.Open();
                //    var cmd = new NpgsqlCommand($@"SELECT ji.assignee AS User, au.id, cu.user_name, cu.display_name,
                //        SUM(((ji.timeestimate + ji.timespent) - ji.timespent) / 60 / 60 / 8) AS TotalRemaining 
                //        FROM {JiraissueTable} AS ji
                //        JOIN app_user AS au ON au.user_key = ji.assignee
                //        JOIN {UserTable} AS cu ON cu.id = au.id 
                //        WHERE ji.issuetype = '{SubTaskIssueType}' 
                //        AND ji.assignee IS NOT NULL 
                //        AND ji.issuestatus = '{OpenStatus}' 
                //        GROUP BY ji.assignee, cu.user_name, au.id, cu.display_name", connection);

                //    var reader = cmd.ExecuteReader();
                //    while (reader.Read())
                //    {
                //        int id=  reader.GetInt32(reader.GetOrdinal("id"));
                //        string username = reader.GetString(reader.GetOrdinal("user_name"));
                //        string displayName = reader.GetString(reader.GetOrdinal("display_name"));
                //        User user= new User(username, displayName, id);
                //        float remaining = reader.GetFloat(reader.GetOrdinal("totalremaining"));
                //        openTasks.Add(new OpenTasksUserAssociation(user, (float)Math.Round(remaining,2)));
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return openTasks;
        }
    }
}
