using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TeamCapacityBalancing.Services.Postgres_connection
{
    public class DBQuerySchema : Dictionary<string, string>
    {
    }

    public class DBQuery
    {
        private const string databasePrefix = "jiradb.dbo.";

        public const string JiraissueTable = databasePrefix + "jiraissue";
        public const string IssuelinkTable = databasePrefix + "issuelink";
        public const string CustomFieldTable = databasePrefix + "Customfieldvalue";
        public const string UserTable = databasePrefix + "cwd_user";
        public const string AppUserTable = databasePrefix + "app_user";
        public const string StoryIssueType = "7";
        public const string EpicStoryLinkType = "10201";
        public const string StoryTaskLinkType = "10100";
        public const string SubTaskIssueType = "10003";
        public const string OpenStatus = "1";
        public const string Project = "12200";
        public DBQuery() {}

        public string Query { get; set; }
        public DBQuerySchema QuerySchema { get; set; }
    }

    class PLQuery : DBQuery
    {
        public PLQuery()
        {
            Query = $@"SELECT Distinct cu.id, cu.user_name, cu.display_name
                    FROM {JiraissueTable} AS i
                    JOIN {AppUserTable} AS au ON i.assignee = au.user_key 
                    JOIN {UserTable} AS cu ON au.user_key = cu.user_name 
                    WHERE i.issuetype = '{StoryIssueType}' AND i.summary LIKE '%#%' AND i.PROJECT = {Project}";

            QuerySchema = new DBQuerySchema()
            {
                { "id", "integer" },
                { "user_name" , "string" },
                { "display_name" , "string" }
            };
        }
    }
}
