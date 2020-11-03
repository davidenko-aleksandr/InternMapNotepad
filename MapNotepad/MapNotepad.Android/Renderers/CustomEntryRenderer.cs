using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Text;
using Android.Widget;
using MapNotepad.Droid.Renderers;
using MapNotepad.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace MapNotepad.Droid.Renderers
{
    class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var cornerOut = new float[] { 40, 40, 40, 40, 40, 40, 40, 40 };
                var cornerIn = new float[] { 40, 40, 40, 40, 40, 40, 40, 40 };

                if (Control is EditText editText)
                {
                    var shape = new ShapeDrawable(new RoundRectShape(cornerOut, new RectF(10, 10, 10, 10), cornerIn));

                    shape.Paint.Color = Xamarin.Forms.Color.FromHex("#c0c8de").ToAndroid();
                    
                    //shape.Paint.SetStyle(Paint.Style.FillAndStroke);

                    editText.Background = shape;
                }
            }
        }
    }
}