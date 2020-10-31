using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Clustering;

namespace MapNotepad.Controls
{
    public class CustomMap : ClusteredMap
    {
        public ObservableCollection<Pin> PinList
        {
            get { return (ObservableCollection<Pin>)GetValue(PinListProperty); }
            set { SetValue(PinListProperty, value); }
        }
        public CustomMap()
        {
            UiSettings.MyLocationButtonEnabled = true;
        }

        public static readonly BindableProperty PinListProperty = BindableProperty.Create(nameof(PinList),
                                                                                         typeof(ObservableCollection<Pin>),
                                                                                         typeof(CustomMap),
                                                                                         propertyChanged: OnPinListPropertyChanged);
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
                Pins.Clear();

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
    }
}

