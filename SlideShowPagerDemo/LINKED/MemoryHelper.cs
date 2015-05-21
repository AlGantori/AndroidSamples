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
//
//using Android.App.ActivityManager;

namespace SlideShowPager
{
    // TODO: 20150519 Helpers are taboo, how can I do better and effeciently :) this should have been a Boogle API
    public static class MemoryHelper
    {
        const long BytesPerMB = 1048576L ;

        // http://stackoverflow.com/questions/3170691/how-to-get-current-memory-usage-in-android        
        // 20150519
        /// <summary>
        /// In naming this, I am assuming there is difference between app and activity allocated memory :)
        /// TODO: 20150519 is there a way not have to pass Context?
        /// </summary>
        /// <returns></returns>
        public static long MemoryAvailableToActivity(Context Context)  
        {
            var MemoryInfo = new ActivityManager.MemoryInfo();
            var ActivityManager = (ActivityManager)Context.GetSystemService(Context.ActivityService);
            ActivityManager.GetMemoryInfo(MemoryInfo);
            long availableMegs = MemoryInfo.AvailMem / BytesPerMB ;
            //
            return availableMegs;
        }
    }
}