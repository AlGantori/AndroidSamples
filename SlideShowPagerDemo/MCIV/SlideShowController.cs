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
    public class SlideShowController
    {

        //----------------------------------------------------------------------
        SlideShowView_I SlideShowView_I_;
        public SlideShowView_I SlideShowView_I
        {
            get
            {
                return SlideShowView_I_;
            }
            set
            {
                SlideShowView_I_ = value;
                // ...

            }
        }

        int Index = 0;
        public void Play()
        {
            Index++;
            // -) Invoke the View  (display graphic, animation)
            if (SlideShowView_I != null)
            {
                // quick sample path construction...
                var FileName = String.Format("{0}/KIDS/JUMAL/BYTOPIC/20/{1:D2}.JPG", AppPaths.LOCAL_DATA_ROOT, Index);
                //
                var Slide = new Slide() { ImageFileName = FileName, Index = Index, Title = "Some title" };
                SlideShowView_I.DisplaySlide(Slide);
            }
        }
        public void Pause()
        {
            Play();
        }
        public void Stop()
        {

        }
    }
}