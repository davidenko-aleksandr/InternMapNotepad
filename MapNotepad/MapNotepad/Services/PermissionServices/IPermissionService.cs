using Plugin.Permissions;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PermissionServices
{
    public interface IPermissionService
    {
        Task<bool> PermissionRequestAsync<T>(T item) where T : BasePermission, new();
    }
}
