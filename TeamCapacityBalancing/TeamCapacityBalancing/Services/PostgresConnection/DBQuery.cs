using Avalonia.Animation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
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
        public const string CustomFieldValueTable = databasePrefix + "Customfieldvalue";
        public const string CustomFieldOptionTable = databasePrefix + "Customfieldoption";
        public const string UserTable = databasePrefix + "cwd_user";
        public const string AppUserTable = databasePrefix + "app_user";

        public const string StoryIssueType = "7";
        public const string SubTaskIssueType = "8";
        public const string EpicStoryLinkType = "10200";
        public const string StoryTaskLinkType = "10100";
        public const string CustomFieldBusinessCase = "10105";
        public const string OpenStatus = "1";
        public const string Project = "12200";
        public DBQuery() { Query = ""; QuerySchema = null; }

        public string Query { get; set; }
        public DBQuerySchema? QuerySchema { get; set; }
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

    class EpicsForPLQuery : DBQuery
    {
        public EpicsForPLQuery(string PLID)
        {
            Query = $@"
                    SELECT i.id, i.issuenum, i.summary, cfo.customvalue
                    FROM {JiraissueTable} AS i
                    JOIN {CustomFieldValueTable} AS cfv ON cfv.issue = i.id
                    JOIN {CustomFieldOptionTable} AS cfo ON cfv.stringvalue=cfo.id
                    WHERE cfv.customfield = {CustomFieldBusinessCase} AND i.id IN
                    (SELECT i2.id
                    FROM {IssuelinkTable} AS il, {JiraissueTable} AS i2
                    WHERE i2.id = il.source AND  il.linktype = 10200 AND il.destination IN
                    (SELECT i3.id
                    FROM {JiraissueTable} AS i3
                    WHERE i3.assignee = '{PLID}'
                    AND i3.issuetype = '{StoryIssueType}'
                    AND i3.summary LIKE '%#%'
                    AND i.PROJECT = {Project}))";

            QuerySchema = new DBQuerySchema()
            {
                {"id", "integer" },
                {"summary", "string" },
                {"issuenum", "integer" },
                {"customvalue", "string"}
        };
    }
    }

    class StoriesForPLQuery : DBQuery
    {
        public StoriesForPLQuery(string PLID)
        {
            Query = $@"
                    SELECT istory.id, istory.issuenum, istory.summary, istory.assignee, iepic.id AS parentId, istory.timeestimate
                    FROM {JiraissueTable} AS istory, {JiraissueTable} AS iepic, {IssuelinkTable} AS il
                    WHERE istory.assignee = '{PLID}'  AND il.source = iepic.id AND il.linktype = {EpicStoryLinkType} AND il.destination = istory.id 
                    AND istory.issuetype = '{StoryIssueType}' AND istory.PROJECT = {Project}
                    AND (istory.timeestimate > 0 OR istory.id in ( 
                        SELECT Distinct istory2.id
                        FROM {JiraissueTable} AS istory2, {JiraissueTable} AS itask2, {IssuelinkTable} AS il2
                        WHERE il2.source = istory2.id AND il2.linktype = {StoryTaskLinkType} AND il2.destination = itask2.id 
                        AND itask2.timeestimate > 0 AND istory2.PROJECT = {Project}))";

            QuerySchema = new DBQuerySchema()
            {
                {"id", "integer" },
                {"summary", "string" },
                {"assignee", "string" },
                {"issuenum", "integer" },
                {"parentId", "integer" },
                {"timeestimate", "integer" }
        };
        }
    }

    class OpenTasksForMembersByPLQuery : DBQuery
    {
        public OpenTasksForMembersByPLQuery(string PLID)
        {
            Query = $@"
                    SELECT itask.id, istory.id as parentID, itask.summary, itask.timeestimate, itask.assignee
                    FROM {JiraissueTable} AS istory, {JiraissueTable} AS itask, {IssuelinkTable} AS il
                    WHERE istory.assignee = '{PLID}'  AND il.source = istory.id AND il.linktype = {StoryTaskLinkType} AND il.destination = itask.id 
                    AND itask.issuetype = '{SubTaskIssueType}' AND istory.PROJECT = {Project}
                    AND itask.timeestimate > 0";

            QuerySchema = new DBQuerySchema()
            {
                {"id", "integer" },
                {"summary", "string" },
                {"assignee", "string" },
                {"parentId", "integer" },
                {"timeestimate", "integer" }
        };
        }
    }

    class UsersQuery : DBQuery
    {
        public UsersQuery()
        {
            Query = $@"
                    SELECT Distinct u.user_name, u.display_name, u.id 
                    FROM {UserTable} AS u, {JiraissueTable} AS i
                    WHERE i.assignee = u.user_name AND i.PROJECT = {Project}
                    ORDER By u.display_name";

            QuerySchema = new DBQuerySchema()
            {
                {"id", "integer" },
                {"user_name", "string" },
                {"display_name", "string" }
        };
        }
    }

    class RemainingTimeUserQuery : DBQuery
    {
        public RemainingTimeUserQuery()
        {
            Query = $@"
                    SELECT cu.id, cu.user_name, cu.display_name,
                    SUM(ji.timeestimate / 60 / 60 / 8) AS totalremaining 
                    FROM {JiraissueTable} AS ji
                    JOIN {AppUserTable} AS au ON au.user_key = ji.assignee
                    JOIN {UserTable} AS cu ON cu.user_name = au.user_key
                    WHERE ji.issuetype = {SubTaskIssueType} 
                    AND ji.assignee IS NOT NULL AND ji.timeestimate > 0 AND ji.project = {Project}
                    GROUP BY cu.user_name, cu.id, cu.display_name";

            QuerySchema = new DBQuerySchema()
            {
                {"id", "integer" },
                {"user_name", "string" },
                {"display_name", "string" },
                {"totalremaining", "double" },
        };
        }
    }
}
