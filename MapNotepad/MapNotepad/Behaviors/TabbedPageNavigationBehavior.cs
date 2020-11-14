using Prism.Behaviors;
using Prism.Common;
using Prism.Navigation;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MapNotepad.Behaviors
{
    public class TabbedPageNavigationBehavior : BehaviorBase<TabbedPage>
    {
        private Page CurrentPage { get; set; }

        #region -- Overrides --

        protected override void OnAttachedTo(TabbedPage bindable)
        {
            bindable.CurrentPageChanged += OnCurrentPageChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(TabbedPage bindable)
        {
            bindable.CurrentPageChanged -= OnCurrentPageChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void OnCurrentPageChanged(object sender, EventArgs e)
        {
            Page newPage = AssociatedObject.CurrentPage;

            if (CurrentPage != null)
            {
                NavigationParameters parameters = new NavigationParameters();
                PageUtilities.OnNavigatedFrom(CurrentPage, parameters);
                PageUtilities.OnNavigatedTo(newPage, parameters);
            }
            else
            {
                Debug.WriteLine("CurrentPage is null");
            }

            CurrentPage = newPage;
        }

        #endregion
    }
}