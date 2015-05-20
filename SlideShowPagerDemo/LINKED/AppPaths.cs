using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SlideShowPager
{
     public static class AppPaths
    {
        /// <summary>
        /// Where MUMTI data is mapped on Local storage
        /// AKA DataRootPathLocal
        /// </summary>
        public static string LOCAL_DATA_ROOT { get { return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Android/data/mumti/DATA"; } }
    }
}