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
			bool isPermissinConfirm = false;
			
			var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();
			if (status != PermissionStatus.Granted)
			{
				if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
				{
					await UserDialogs.Instance.AlertAsync("Gonna need that location", "Need location", "OK");
				}

				status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
			}

			if (status == PermissionStatus.Granted)
			{
				isPermissinConfirm = true;
			}
			else if (status != PermissionStatus.Unknown)
			{
				isPermissinConfirm = false;
			}
			return isPermissinConfirm;
		}
    }
}
