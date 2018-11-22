using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kooboo.Data.Models
{
    public class Demand : IGolbalObject
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

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public DemandStatus Status { get; set; }

        public List<string> Skills { get; set; }

        public decimal Budget { get; set; }

        public decimal FinalPrice { get; set; }

        public int ProposalCount { get; set; }

        public string Currency { get; set; }

        [JsonIgnore]
        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }
        public List<ResouceAttachment> Attachments { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //demand objection when demandstaus set to Unfinished
        public bool HasObjection { get; set; }
        //to mark whether unresolved demand is confirmed
        public bool IsConfirmed { get; set; }
        //job auto confirm,manual confirm
        public string ConfirmRemark { get; set; }

    }

    public enum DemandStatus
    {
        Tendering = 0,
        EndOfTender = 1,
        Unfinished = 2,
        Finished = 3,
        Invalid = 4
    }
}
