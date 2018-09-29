using Kooboo.Sites.Repository; 
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Executor
{
  public interface ITransferExecutor
    { 
        Task Execute();    
        
        SiteDb SiteDb { get; set; }

        TransferTask TransferTask { get; set; }
    }
}
