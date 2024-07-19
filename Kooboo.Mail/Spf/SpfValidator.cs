using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Spf
{
    public class SpfValidator : BaseValidator<SpfRecord>
    {
        private static readonly ISpfDnsRequest _koobooDnsRequest = new SpfDnsRequest();

        protected override async Task<LoadRecordResult> LoadRecordsAsync(DomainName domainName, CancellationToken cancellationToken)
        {
            var txtResult = await _koobooDnsRequest.GetTxtResolveResultAsync(domainName.ToString(), cancellationToken);
            if (txtResult == null || txtResult!.ReturnCode != ReturnCode.NoError && txtResult.ReturnCode != ReturnCode.NxDomain)
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Temperror };
            }

            var spfTextRecords = new List<string>();
            foreach (var records in txtResult.Records!)
            {
                foreach (var record in records)
                {
                    if (SpfRecord.IsSpfRecord(record))
                    {
                        spfTextRecords.Add(record);
                    }
                }
            }

            SpfRecord resultRecord;
            if (spfTextRecords?.Count == 0)
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.None };
            }
            else if (spfTextRecords?.Count == 1 && SpfRecord.TryParse(spfTextRecords[0], out resultRecord))
            {
                return new LoadRecordResult() { CouldBeLoaded = true, Record = resultRecord! };
            }
            else
            {
                return new LoadRecordResult() { CouldBeLoaded = false, ErrorResult = ResultsOfEvaluation.Permerror };
            }
        }
    }
}

