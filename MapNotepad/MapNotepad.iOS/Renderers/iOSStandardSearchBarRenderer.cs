using Foundation;
using MapNotepad.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBar), typeof(iOSStandardSearchBarRenderer))]
namespace MapNotepad.iOS.Renderers
{
    public sealed class iOSStandardSearchBarRenderer : SearchBarRenderer
    {
        protected override void SetBackgroundColor(Color color)
        {
            base.SetBackgroundColor(color);

            if (Control == null)
                return;

            Control.BarTintColor = UIColor.Clear;
            Control.BackgroundColor = UIColor.Clear;
            Control.Translucent = true;

            if (Control.ValueForKey(new NSString("searchField")) is UITextField textField)
            {
                textField.BackgroundColor = color.ToUIColor();
                textField.Layer.BorderWidth = 1.0f;
               // textField.Layer.BorderColor = UIColor.LightGray.CGColor;
                textField.Layer.CornerRadius = 8.0f;
            }

            UpdateCancelButton();
        }
    }
}