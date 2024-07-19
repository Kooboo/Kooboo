using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Data.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class DepartmentApi : IApi
    {
        public string ModelName => "Department";
        public bool RequireSite => false;
        public bool RequireUser => true;

        public List<OrganizationDepartmentViewModel> List(ApiCall call)
        {
            var user = call.Context.User;
            return GlobalDb.Department.List(user.CurrentOrgId);
        }

        public List<DepartmentUser> Users(Guid id, ApiCall call)
        {
            return GlobalDb.Department.GetUsersByDepartment(id);
        }

        public record UserInfo(string UserName, Guid DepartmentId, bool IsManager, string Function);
        public void AddUser(UserInfo userInfo, ApiCall call)
        {
            GlobalDb.Department.AddUser(
               userInfo.UserName,
               userInfo.DepartmentId,
               userInfo.IsManager,
               userInfo.Function,
               call.Context);
        }

        public record DepartmentCreate(string Name);
        public void Post(DepartmentCreate departmentCreate, ApiCall call)
        {
            GlobalDb.Department.AddOrUpdate(new OrganizationDepartment
            {
                Name = departmentCreate.Name,
                DisplayName = departmentCreate.Name,
                OrganizationId = call.Context.User.CurrentOrgId
            }, call.Context);
        }

        public void Deletes(Guid[] ids, ApiCall call)
        {
            foreach (var item in ids)
            {
                GlobalDb.Department.Delete(item, call.Context);
            }
        }

        public void DeleteUsers(string[] userNames, Guid departmentId, ApiCall call)
        {
            foreach (var item in userNames)
            {
                GlobalDb.Department.DeleteUser(item, departmentId, call.Context);
            }
        }
    }
}
