﻿using Xamarin.Forms;

namespace MapNotepad.Controls
{    
    public class CustomEntry : Entry
    {
        #region -- Bindable properties --

        public static BindableProperty CornerRadiusProperty = BindableProperty.Create(
                                                                                nameof(CornerRadius), 
                                                                                typeof(int), 
                                                                                typeof(CustomEntry), 
                                                                                0);

        public static BindableProperty BorderThicknessProperty = BindableProperty.Create(
                                                                                   nameof(BorderThickness), 
                                                                                   typeof(int), 
                                                                                   typeof(CustomEntry), 
                                                                                   0);

        public static BindableProperty PaddingProperty = BindableProperty.Create(
                                                                           nameof(Padding), 
                                                                           typeof(Thickness), 
                                                                           typeof(CustomEntry), 
                                                                           new Thickness(5));

        public static BindableProperty BorderColorProperty = BindableProperty.Create(
                                                                               nameof(BorderColor), 
                                                                               typeof(Color), 
                                                                               typeof(CustomEntry), 
                                                                               Color.Transparent);
        #endregion

        #region -- Public properties --

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
        #endregion
    }
}
