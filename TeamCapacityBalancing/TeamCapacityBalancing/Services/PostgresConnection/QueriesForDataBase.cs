using Newtonsoft.Json.Serialization;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCapacityBalancing.Models;
using TeamCapacityBalancing.Services.ServicesAbstractions;

namespace TeamCapacityBalancing.Services.Postgres_connection
{
    public class QueriesForDataBase : IDataProvider
    {
        private const string JiraissueTable = "jiraissue";
        private const string IssuelinkTable = "issuelink";
        private const string UserTable = "cwd_user";
        private const string StoryIssueType = "10001";
        private const string EpicStoryLinkType = "10201";
        private const string StoryTaskLinkType = "10100";

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
                        $"JOIN {IssuelinkTable} " +
                        $"ON {JiraissueTable}.id = {IssuelinkTable}.destination " +
                        $"WHERE {IssuelinkTable}.linktype = {StoryTaskLinkType} " +
                        $"AND {IssuelinkTable}.source = {storyId}", connection);
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
            return timeEstimate-timeSpent;
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

        public List<IssueData> GetStoriesByEpic(int epicId)
        {
            List<IssueData> stories = new List<IssueData>();

            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand($"SELECT {JiraissueTable}.id, {JiraissueTable}.assignee, {JiraissueTable}.issuenum, {JiraissueTable}.project, {JiraissueTable}.summary " +
                        $"FROM {JiraissueTable} " +
                        $"JOIN {IssuelinkTable} " +
                        $"ON {JiraissueTable}.id = {IssuelinkTable}.destination " +
                        $"WHERE {IssuelinkTable}.linktype = {EpicStoryLinkType} " +
                        $"AND {IssuelinkTable}.source = {epicId}", connection);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string name = reader.GetString(reader.GetOrdinal("summary"));
                        string assignee = reader.GetString(reader.GetOrdinal("assignee"));
                        int issueNumber = reader.GetInt32(reader.GetOrdinal("issuenum"));
                        int projectId = reader.GetInt32(reader.GetOrdinal("project"));

                        //calculate only the first to decimals for a float
                        float result = ((CalculateRemainingTimeForStory(id) / 60) / 60) / 24; //from seconds to days
                        float onlydecimals =(float)( result - (int)result);
                        int first2DecimalsInt = (int)(onlydecimals * 100);
                        float firstTwoDecimalsFloat = (float)first2DecimalsInt / 100;
                        float remaining = firstTwoDecimalsFloat + (int)result;
                        stories.Add(new IssueData(id, name, assignee,remaining));

                        //stories.Add(new IssueData(id, name, IssueData.IssueType.Story, assignee, issueNumber, projectId));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return stories;
        }

        public List<IssueData> GetAllStoriesByTeamLeader(string teamLeaderUsername)
        {
            List<IssueData> stories = new List<IssueData>();

            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand($"SELECT {JiraissueTable}.id, {JiraissueTable}.assignee, {JiraissueTable}.issuenum, {JiraissueTable}.project, {JiraissueTable}.summary " +
                        $"From {JiraissueTable} " +
                        $"where {JiraissueTable}.assignee = '{teamLeaderUsername}' " +
                        $"and {JiraissueTable}.issuetype = '{StoryIssueType}' " +
                        $"group by {JiraissueTable}.id", connection);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string name = reader.GetString(reader.GetOrdinal("summary"));
                        string assignee = reader.GetString(reader.GetOrdinal("assignee"));
                        int issueNumber = reader.GetInt32(reader.GetOrdinal("issuenum"));
                        int projectId = reader.GetInt32(reader.GetOrdinal("project"));
                        stories.Add(new IssueData(id, name, assignee));
                        //stories.Add(new IssueData(id, name, IssueData.IssueType.Story, assignee, issueNumber, projectId));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return stories;
        }

        public List<IssueData> GetAllEpicsByTeamLeader(User teamLeader)
        {
            List<IssueData> epics = new List<IssueData>();

            try
            {
                using (var connection = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString()))
                {
                    connection.Open();

                    var cmd = new NpgsqlCommand($@"
                        SELECT {JiraissueTable}.id, {JiraissueTable}.assignee, {JiraissueTable}.issuenum, {JiraissueTable}.project, {JiraissueTable}.summary
                        FROM {JiraissueTable}
                        WHERE {JiraissueTable}.id IN
                        (SELECT {IssuelinkTable}.source
                        FROM {IssuelinkTable}
                        WHERE {IssuelinkTable}.destination IN
                        (SELECT {JiraissueTable}.id
                        FROM {JiraissueTable}
                        WHERE {JiraissueTable}.assignee = 'JIRAUSER{teamLeader.Id}'
                        AND {JiraissueTable}.issuetype = '{StoryIssueType}'
                        AND {JiraissueTable}.summary LIKE '%#%'
)
                        
        )", connection);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string name = reader.GetString(reader.GetOrdinal("summary"));
                        string assignee = reader.GetString(reader.GetOrdinal("assignee"));
                        int issueNumber = reader.GetInt32(reader.GetOrdinal("issuenum"));
                        int projectId = reader.GetInt32(reader.GetOrdinal("project"));
                        epics.Add(new IssueData(id, name, assignee));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return epics;
        }
    }
}
