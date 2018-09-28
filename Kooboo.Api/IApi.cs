namespace Kooboo.Api
{
   public interface IApi
    {  
        string ModelName { get; } 
        bool RequireSite { get; }
        bool RequireUser { get;  }
    }
}
