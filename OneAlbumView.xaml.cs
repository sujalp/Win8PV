﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Win8PV
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class OneAlbumView : Win8PV.Common.LayoutAwarePage
    {
        public OneAlbumView()
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
            Tuple<string, string, string> param = (Tuple<string, string, string>)navigationParameter;
            DataContext = new OneAlbumViewModel(param.Item1, param.Item2, param.Item3);
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

        private void Grid_PointerReleased_1(object sender, PointerRoutedEventArgs e)
        {
            OneAlbumViewModel oavm = (OneAlbumViewModel)DataContext;
            NavigateTo(oavm.MainImage);
        }

        private void itemGridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            NavigateTo((Image)e.ClickedItem);
        }

        private void NavigateTo(Image im)
        {
            OneAlbumViewModel oavm = (OneAlbumViewModel)DataContext;
            this.Frame.Navigate(typeof(PhotoView), new Tuple<OneAlbumViewModel, Image>(oavm, im));
        }
    }
}
