using System;

namespace SandCastle.Core.Models
{
    public class MenuItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Action Action { get; set; }
    }
}
