using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

﻿namespace TeamCapacityBalancing.Models
{
    public class User
    {
        public string Username { get; set; }
      
        public string DisplayName { get; set; }
      
        public bool HasTeam { get; set; }
      
        public int Id { get; set; }

        public bool IsMember { get; set; }
      
        public User(string username, string displayName, int id, bool isMember)
        {
            Username = username;
            DisplayName = displayName;
            Id = id;
            IsMember = isMember;
        }

        public User(string username)
        {
            DisplayName = username;
        }
    }
}
