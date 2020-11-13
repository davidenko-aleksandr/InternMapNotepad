using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
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
            else
            {
                Debug.WriteLine("Permission status is Unknown");
            }

			return status == PermissionStatus.Granted;
		}
    }
}
