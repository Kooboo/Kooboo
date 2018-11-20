using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kooboo.Data.Models
{
    public class DemandProposal:IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Description { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Guid DemandId { get; set; }

        public string DemandTitle { get; set; }

        public List<string> Skills { get; set; }

        public decimal Budget { get; set; }

        public bool WinTheBidding { get; set; }

        public ProposalStatus ProposalStatus { get; set; }

        public string Currency { get; set; }
        
        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }

        public DateTime CreateTime { get; set; }

        public DateTime LastModify { get; set; }

        /// <summary>
        /// days
        /// </summary>
        public int Duration { get; set; }

        public string AttachmentName { get; set; }

        public byte[] Attachment { get; set; }

    }


    public enum ProposalStatus
    {
        InTheBid = 0,
        WinTheBidding = 1,
        LoseTheBid=2,
        UnFinished = 3,
        Finished = 4,
        Invalid = 5
    }
}
