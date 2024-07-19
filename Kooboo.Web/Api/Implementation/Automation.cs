using Kooboo.Api;
using Kooboo.Sites.Automation;

namespace Kooboo.Web.Api.Implementation
{
    public class AutomationApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "automation";
            }
        }
        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser => true;


        public Dictionary<string, int> RunningTask = new Dictionary<string, int>();

        public List<AutomationTaskModel> List(ApiCall call)
        {
            return Kooboo.Sites.Automation.AutomaionService.Instance.AvailableTask(call.Context);

        }

        public Guid ExecuteTask(int TaskId, ApiCall call)
        {
            var sessionid = AutomaionService.Instance.Run(TaskId, call.Context);

            return sessionid;
        }

        public AutomationResponse GetStatus(Guid sessionId, ApiCall call)
        {
            var session = AutomaionService.Instance.GetSession(sessionId);

            AutomationResponse response = new AutomationResponse();

            if (session != null)
            {
                response.Results = session.FetchResponse(50);
                if (session.IsEnd && response.Results.Count < 50)
                {
                    response.IsEnd = true;
                }
                return response;
            }
            else
            {
                response.IsEnd = true;
            }
            return response;
        }

    }

    public class AutomationResponse
    {
        public bool IsEnd { get; set; }

        public List<Sites.Automation.Response.ResponseLine> Results { get; set; }

    }




    public class AutomationTask
    {
        public Guid Id { get; set; } = System.Guid.NewGuid();

        public string Name { get; set; }

        public string Description { get; set; }
    }

}
