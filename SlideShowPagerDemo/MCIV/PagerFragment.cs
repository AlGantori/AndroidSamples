//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
//
using VR.Utils;
//
namespace SlideShowPager
{
    // 
    // https://github.com/codepath/android_guides/wiki/ViewPager-with-FragmentPagerAdapter
    // https://github.com/Martynnw/AndroidDemos
    public class PagerFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.DemoFragment, container, false);
            // 
            var Info = view.FindViewById<TextView>(Resource.Id.Info);
            // TODO: 20150519 How display memory left to this activity?
            Info.Text = String.Format( "Slide Index={0} Title={1}\n Available Memory={2}", Index, Title, 
                MemoryHelper.MemoryAvailableToActivity(this.Activity));
            // Load this slide's bitmap
            var ImageView = view.FindViewById<ImageView>(Resource.Id.imageView1);
            // quick sample path construction...
            // var FileName = String.Format("{0}/KIDS/JUMAL/BYTOPIC/20/{1:D2}.JPG", AppPaths.LOCAL_DATA_ROOT, Index);
            GraphicsHelper.Load(this.Activity, ImageView, ImageFileName);
            //
            return view;
        }


        // 20150518
        /// <summary>
        /// newInstance constructor for creating fragment with arguments
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static PagerFragment newInstance(Slide Slide)  // 
        {
            var PagerFragment = new PagerFragment();
            var Bundle = new Bundle();
            Bundle.PutInt("Index", Slide.Index);
            Bundle.PutString("Title", Slide.Title);
            Bundle.PutString("ImageFileName", Slide.ImageFileName);            
            PagerFragment.Arguments = Bundle;
            return PagerFragment;
        }
        // default ctr needed by static above, where there is other ctrs :(
        public PagerFragment()
        { }
        // cheat for now, 
        // TODO: I could not get the static newInstance above to pass in the instance arguments correctly to the fragments?? 
        // Got PagerFragment.newInstance to work correctly.  I was trying to use savedInstanceState to retrieve the args :(
        // 20150519
        public PagerFragment(int Index, String Title)
        {
            this.Index = Index;
            this.Title = Title;
        }
        // 20150518
        int Index;
        String Title;
        String ImageFileName;
        // 20150518
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //
            Index = Arguments.GetInt("Index", 0);
            Title = Arguments.GetString("Title", "Unknown");
            ImageFileName = Arguments.GetString("ImageFileName", "Unknown");
            // for now..
            GraphicsHelper.Init(this.Activity);
            GraphicsHelper.DefaultResID = Resource.Drawable.MediaNotAvailable;           
        }

       
    }
}