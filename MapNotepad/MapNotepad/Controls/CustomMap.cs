using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Clustering;

namespace MapNotepad.Controls
{
    public class CustomMap : ClusteredMap
    {
        public static readonly BindableProperty MapCameraPositionProperty = BindableProperty.Create(
                                                                                              propertyName: nameof(MapCameraPosition),
                                                                                              returnType: typeof(Position),
                                                                                              declaringType: typeof(CustomMap),
                                                                                              defaultBindingMode: BindingMode.TwoWay,
                                                                                              propertyChanged: CameraPositionPropertyChanged);

        public static readonly BindableProperty PinListProperty = BindableProperty.Create(
                                                                                    nameof(PinList),
                                                                                    typeof(ObservableCollection<Pin>),
                                                                                    typeof(CustomMap),
                                                                                    propertyChanged: OnPinListPropertyChanged);

        public static readonly BindableProperty OpenMapPositionProperty = BindableProperty.Create(
                                                                                            propertyName: nameof(OpenMapPosition),
                                                                                            returnType: typeof(CameraPosition),
                                                                                            declaringType: typeof(CustomMap),
                                                                                            defaultBindingMode: BindingMode.TwoWay,
                                                                                            propertyChanged: OpenMapCameraPosition);
        #region -- Public properties --
        public CameraPosition OpenMapPosition
        {
            get => (CameraPosition)GetValue(OpenMapPositionProperty);
            set => SetValue(OpenMapPositionProperty, value);
        }

        public Position MapCameraPosition
        {
            get => (Position)GetValue(MapCameraPositionProperty);
            set => SetValue(PinListProperty, value);
        }

        public ObservableCollection<Pin> PinList
        {
            get { return (ObservableCollection<Pin>)GetValue(PinListProperty); }
            set { SetValue(PinListProperty, value); }
        }
        #endregion

        #region -- Private helpers --
        private static void OnPinListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomMap castedMap)
            {
                if (oldValue is INotifyCollectionChanged castedOldCollection)
                {
                    castedOldCollection.CollectionChanged -= castedMap.OnCollectionChanged;
                }

                if (newValue is INotifyCollectionChanged castedNewCollection)
                {
                    castedNewCollection.CollectionChanged += castedMap.OnCollectionChanged;
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Pins.Clear();
            }

            if (e.OldItems != null)
            {
                foreach (Pin pin in e.OldItems)
                {
                    Pins.Remove(pin);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Pin pin in e.NewItems)
                {
                    Pins.Add(pin);
                }
            }
        }

        private static void CameraPositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue && bindable is CustomMap && newValue != null)
            {
                UpdateCameraPosition(bindable as CustomMap, (Position)newValue);
            }
        }

        private static void OpenMapCameraPosition(BindableObject bindable, object oldValue, object newValue)
        {
            Map map = bindable as Map;
            var cameraPosition = newValue as CameraPosition;

            var position = new Position(cameraPosition.Target.Latitude, cameraPosition.Target.Longitude);

            var cameraUpdate = CameraUpdateFactory.NewPositionZoom(position, cameraPosition.Zoom);

            map.InitialCameraUpdate = cameraUpdate;

            map.MoveCamera(cameraUpdate);
        }

        private static void UpdateCameraPosition(CustomMap map, Position cameraPosition)
        {            
            map.MoveToRegion(MapSpan.FromCenterAndRadius(cameraPosition, Distance.FromMiles(5)));
        }

        #endregion

        public CustomMap()
        {
            UiSettings.MyLocationButtonEnabled = true;
        }
    }
}

