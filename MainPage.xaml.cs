using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Win8PV.Common;
using Windows.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Win8PV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        UniqueNumberGenerator ung = new UniqueNumberGenerator();
        Task<Metadata> m_metadataloader;
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            DataContext = new DefaultViewModel();
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            base.SaveState(pageState);
        }

        private void HandleClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            Year y = (Year)e.ClickedItem;
            this.Frame.Navigate(typeof(MonthPage), y.Y);
        }

        private void Grid_PointerReleased_1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            DefaultViewModel dvm = (DefaultViewModel)DataContext;
            this.Frame.Navigate(typeof(MonthPage), dvm.FirstYear.Y);
        }
    }
}
