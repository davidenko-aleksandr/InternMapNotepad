using System.Windows.Input;
using Xamarin.Forms;

namespace MapNotepad.Controls
{
    public class CustomListView : ListView
    {
        public static BindableProperty ItemClickCommandProperty = BindableProperty.Create(
                                                                  nameof(ItemClickCommand),
                                                                  typeof(ICommand),
                                                                  typeof(CustomListView), 
                                                                  null);

        public ICommand ItemClickCommand
        {
            get
            {
                return (ICommand)this.GetValue(ItemClickCommandProperty);
            }
            set
            {
                this.SetValue(ItemClickCommandProperty, value);
            }
        }

        public CustomListView()
        {
            this.ItemTapped += OnItemTapped;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                ItemClickCommand?.Execute(e.Item);
                SelectedItem = null;
            }
        }
    }
}