﻿using System.ComponentModel;
using System.Drawing;
using MapNotepad.Controls;
using MapNotepad.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(iOSCustomEntryRenderer))]
namespace MapNotepad.iOS.Renderers
{
    public class iOSCustomEntryRenderer : EntryRenderer
    {
        public CustomEntry ElementV2 => Element as CustomEntry;
        public UITextFieldPadding ControlV2 => Control as UITextFieldPadding;

        protected override UITextField CreateNativeControl()
        {
            var control = new UITextFieldPadding(RectangleF.Empty)
            {
                Padding = ElementV2.Padding,
                BorderStyle = UITextBorderStyle.RoundedRect,
                ClipsToBounds = true
            };

            UpdateBackground(control);

            return control;
        }

        protected void UpdateBackground(UITextField control)
        {
            if (control != null)
            {
                control.Layer.CornerRadius = ElementV2.CornerRadius;
                control.Layer.BorderWidth = ElementV2.BorderThickness;
                control.Layer.BorderColor = ElementV2.BorderColor.ToCGColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CustomEntry.PaddingProperty.PropertyName)
            {
                UpdatePadding();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected void UpdatePadding()
        {
            if (Control != null)
            {
                ControlV2.Padding = ElementV2.Padding;
            }            
        }
    }   
}