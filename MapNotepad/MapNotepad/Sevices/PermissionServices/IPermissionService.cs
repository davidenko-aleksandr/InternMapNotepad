using System.Threading.Tasks;

namespace MapNotepad.Sevices.PermissionServices
{
    public interface IPermissionService
    {
        Task<bool> PermissionRequestAsync();
    }
}
