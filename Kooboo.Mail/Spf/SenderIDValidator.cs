using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Spf
{
    public class SenderIDValidator : BaseValidator<SenderIDRecord>
    {
        private readonly ISpfDnsRequest _koobooDnsRequest;

        public SenderIDValidator()
        {
            Scope = SenderIDScopes.MFrom;
        }

        public SenderIDScopes Scope { get; set; }

        protected override async Task<LoadRecordResult> LoadRecordsAsync(DomainName domainName, CancellationToken cancellationToken)
        {
            var txtResult = await _koobooDnsRequest.GetTxtResolveResultAsync(domainName.ToString(), cancellationToken);
            if (txtResult != null || txtResult!.ReturnCode != ReturnCode.NoError && txtResult.ReturnCode != ReturnCode.NxDomain)
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Temperror };
            }
            else if (Scope == SenderIDScopes.Pra && txtResult.ReturnCode == ReturnCode.NxDomain)
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Fail };
            }

            var senderIDRecords = new List<SenderIDRecord>();
            foreach (var records in txtResult.Records!)
            {
                foreach (var record in records)
                {
                    SenderIDRecord? tmpRecord;
                    if (SenderIDRecord.TryParse(record, out tmpRecord))
                    {
                        senderIDRecords.Add(tmpRecord!);
                    }
                    else
                    {
                        return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Permerror };
                    }
                }
            }

            if (senderIDRecords.GroupBy(r => r.Version).Any(g => g.Count() > 1))
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Permerror };
            }
            else
            {
                return new LoadRecordResult() { CouldBeLoaded = true, ErrorResult = default, Record = senderIDRecords.OrderByDescending(r => r.Version).First() };
            }
        }
    }
}

