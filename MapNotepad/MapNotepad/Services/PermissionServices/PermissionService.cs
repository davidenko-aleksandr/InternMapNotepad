using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PermissionServices
{
    public class PermissionService : IPermissionService
    {
        public async Task<bool> PermissionRequestAsync<T>(T item) where T : BasePermission, new()
        {
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync<T>();

			if (status != PermissionStatus.Granted)
			{
				status = await CrossPermissions.Current.RequestPermissionAsync<T>();
			}

			return status == PermissionStatus.Granted;
		}
    }
}
