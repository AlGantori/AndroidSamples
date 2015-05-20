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


        //----------------------------------------------------------------------
        #region Public Events

        public delegate void SlidesDataReady_Delegate(object sender);
        /// <summary>
        /// Raised when a media/player's data has been loaded and ready for play
        /// </summary>
        public event SlidesDataReady_Delegate DataReady;

        #endregion


        //----------------------------------------------------------------------
        String MediaURL_;
        /// <summary>
        /// Loads specified Media
        /// Fires event DataReady on completion.
        /// </summary>
        /// <param name="MediaURL"></param>
        public void Load(String MediaURL)
        {
            MediaURL_ = MediaURL;
            // In reality this may take time (potentially async)
            // notify listeners
            if (DataReady != null)
            {
                //Log.EventFire("SlidesDataReady");
                DataReady(this);
            }
        }

        int Index = 0;
        /// <summary>
        /// Play current slide content (Graphic, Animations, Sounds, ... all what applies)
        /// </summary>
        public void Play()
        {
            Index++;
            // -) Invoke the View  (display graphic, animation)
            if (SlideShowView_I != null)
            {
                //
                SlideShowView_I.DisplaySlide(Slide(Index));
            }
        }
        public void Play(int Index)
        {
            this.Index = Index;
            Play();
        }
        public void Pause()
        {
            Play();
        }
        public void Stop()
        {

        }
        // 20150520
        /// <summary>
        /// Returns the slide model at the specified index.
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Slide Slide(int Index)
        {
            return new Slide()
                {
                    Title = "Some title " + Index,
                    // quick sample path construction...
                    ImageFileName = String.Format("{0}/{1}/{2:D2}.JPG", AppPaths.LOCAL_DATA_ROOT, MediaURL_, Index),
                    Index = Index
                };
        }
        // 20150520
        /// <summary>
        /// To be used by the Adapter?
        /// </summary>
        /// <returns></returns>
        public List<Slide> Slides()
        {
            var ss = new List<Slide>();
            for (int Index = 1; Index <= 20; Index++)  // Playlist.Count
            {
                //
                ss.Add(Slide(Index));
            }
            //
            return ss;
        }
    }
}