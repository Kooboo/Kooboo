//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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
