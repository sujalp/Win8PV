using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Win8PV
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PhotoView : Win8PV.Common.LayoutAwarePage
    {
        public PhotoView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Tuple<OneAlbumViewModel, Image> param = (Tuple<OneAlbumViewModel, Image>)navigationParameter;
            DataContext = param.Item1;
            xFlipView.SelectedItem = param.Item2;
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            int index = xFlipView.SelectedIndex;
            var pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
            if (pp.Properties.MouseWheelDelta < 0)
            {
                index++;
            }
            else
            {
                index--;
            }
            if ((index >= 0) && 
                (index < ((OneAlbumViewModel)DataContext).AllImages.Count)
               )
            {
                xFlipView.SelectedIndex = index;
            }
            base.OnPointerWheelChanged(e);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
