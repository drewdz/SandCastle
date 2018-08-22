﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PE.Plugins.PubnubChat.Models
{
    public enum ChannelType : int
    {
        Individual = 0,
        Group= 1
    }

    public class Channel : IIndexable
    {
        /// <summary>
        /// Unique channel id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Name/Description of the channel
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Channel has had not activity for more than eg: 30 days
        /// </summary>
        [JsonProperty]
        public bool Archived { get; set; }

        /// <summary>
        /// The type of the channel
        /// </summary>
        [JsonProperty]
        public ChannelType ChannelType { get; set; }

        /// <summary>
        /// Users in this channel
        /// </summary>
        [JsonProperty]
        public string[] Users { get; set; }

        /// <summary>
        /// When last a message was posted
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset LastActivity { get; set; } = DateTimeOffset.MinValue;

        [JsonIgnore]
        public string UsersKey
        {
            get
            {
                if ((Users == null) || (Users.Length == 0)) return string.Empty;
                var list = new List<string>(Users).OrderBy(s => s).ToList();
                string users = string.Empty;
                foreach (var s in list)
                {
                    users += (string.IsNullOrEmpty(users)) ? s : "," + s;
                }
                return users;
            }
        }
    }
}
