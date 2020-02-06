using System;

namespace Kooboo.Sites.Ecommerce.Models
{
    public class SavingPoints
    {
        public long Points { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Guid CustomerId { get; set; }
    }
}