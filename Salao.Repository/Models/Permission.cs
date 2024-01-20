using System;
using System.Collections.Generic;

namespace Renavam.Repository.Models
{
    public partial class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
