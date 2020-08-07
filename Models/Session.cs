using System;
using System.Collections.Generic;

namespace SterlingCommunityAPI.Models
{
    public partial class Session
    {
        public int SessionId { get; set; }
        public string SessionKey { get; set; }
        public string SessionKeyInsertedByUser { get; set; }
        public string UserName { get; set; }
        public string Bvn { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? DateInserted { get; set; }
        public DateTime? DateSessionKeyInsertedByUser { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateIsActiveChanged { get; set; }
    }
}
