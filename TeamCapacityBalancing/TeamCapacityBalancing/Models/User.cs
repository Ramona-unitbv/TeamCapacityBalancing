namespace TeamCapacityBalancing.Models
{
    public class User
    {
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public bool HasTeam { get; set; }

        public int Id { get; set; }

        public User()
        {

        }

        public User(string username, string displayName, int id)
        {
            Username = username;
            DisplayName = displayName;
            Id = id;
        }

        public User(string username, string displayName, bool hasTeam, int id)
        {
            Username = username;
            DisplayName = displayName;
            HasTeam = hasTeam;
            Id = id;
        }

        public User(string username)
        {
            DisplayName = username;
        }
    }
}
