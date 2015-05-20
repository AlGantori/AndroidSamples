//
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
    //----------------------------------------------------------------------
    // 20130426
    /// <summary>
    /// Interface to instruct the "View" to update to the correct slide.
    /// </summary>
    public interface SlideShowView_I
    {
        // 20130426
        /// <summary>
        /// Play current slide graphics eg. rendering 
        ///     1) slide's image(s), (animation)
        ///     2) slide's text (animation)
        /// NOTE: The Slide's target are shuffled by UQSPlayer (controller)
        /// </summary>
        void DisplaySlide(Slide Slide);


        // 20150422
        /// <summary>
        /// Controller need the current View Activity reference, an Android much needed thingy :)
        /// </summary>
        /// <returns></returns>
        Activity GetActivity();
    }
}