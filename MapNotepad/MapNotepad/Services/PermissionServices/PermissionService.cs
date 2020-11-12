using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PermissionServices
{
    public class PermissionService : IPermissionService
    {
        public async Task<bool> PermissionRequestAsync()
        {
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

			if (status != PermissionStatus.Granted)
			{
				status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
			}

			return status == PermissionStatus.Granted;
		}
    }
}
