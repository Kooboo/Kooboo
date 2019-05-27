using System; 

namespace Kooboo.Sites.Models
{
 public class RolePermission
    {
        public Guid Id { get; set; }
       
      //each 4 bytes as a hash key.... and then to base64 as string, and then back. 
       public string MenuItem { get; set; }
        
       public Guid RoldId { get; set; }
    }
} 