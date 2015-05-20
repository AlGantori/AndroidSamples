using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;

namespace SlideShowPager
{
    /*
     */
    // https://github.com/Martynnw/AndroidDemos
    /// <summary>
    /// Trying to fit my Model-Controller-Interface-View to Android activities.
    /// I have found it particularly challenging with the ViewPager / Adapter / Fragment an Android thingy.
    /// The Views (Activities) in Android apps are first and invasive I think. 
    /// So I am hosting one and only one reference to the Controller inside the View
    /// which then talks back to the View via an interface.  
    /// The Controller only passes needed Models to the view to carry its task.
    /// </summary>
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon")]  // , MainLauncher = true
    public class PagerActivity : Activity, SlideShowView_I
    // Android.Support.V4.App.FragmentActivity // Activity
    {
        public const string PAGERTYPE = "PagerType";

        private ViewPager ViewPager;
        private PagerAdapter PagerAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.SetContentView(Resource.Layout.PagerActivity);
            //
            View__ControlsSetup();

        }
        // 20150515
        /// <summary>
        /// OnResume() called when accessing Activity from Task Manager (aka "User returns to Activity")
        /// after a FORCE STOP?
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            //
            ViewPager__Setup();
            //View__ControlsSetup();
            //
            SlideShowController__StartResume();
        }
        // 20150519
        protected override void OnPause()
        {
            //
            SlideShowController__StopSave();
            base.OnPause();
        }

        // 20150519
        protected override void OnDestroy()
        {
            base.OnDestroy();
            SlideShowController__Disconnect();
        }

        // 20140305
        public override void OnBackPressed()
        {
            //
            base.OnBackPressed();
            //
            // Force stop
            //SlideShowController.Stop();
            //
            SlideShowController__StopSave();
            //
            Finish();  // trying this to kill sound
        }


        // 20140305
        /// <summary>
        /// Perform Android specific needed cleanups (View, ...)
        /// </summary>
        void Cleanup()
        {
            ViewPager__UnSetup();
            View__ControlsUnSetup();
            //
            System.GC.Collect();
        }





        //----------------------------------------------------------------------
        void ViewPager__Setup()
        {
            // ViewPager Setup
            PagerAdapter = new PagerAdapter(this.FragmentManager);
            //
            ViewPager = this.FindViewById<ViewPager>(Resource.Id.pager);
            ViewPager.Adapter = PagerAdapter;
            this.SetPageTransformer();
            // https://github.com/codepath/android_guides/wiki/ViewPager-with-FragmentPagerAdapter
            ViewPager.OffscreenPageLimit = 5;
        }
        void ViewPager__UnSetup()
        {
            if (ViewPager != null)
            {
                ViewPager.Adapter = null;
                //ViewPager.Click -= ViewPager_Click;
                //ViewPager.PageSelected -= ViewPager_PageSelected;
                ViewPager = null;
            }
        }
        private void SetPageTransformer()
        {
            switch (this.Intent.GetIntExtra(PAGERTYPE, 0))
            {
                case 1:
                    ViewPager.SetPageTransformer(true, new FadeTransformer());
                    break;

                case 2:
                    ViewPager.SetPageTransformer(true, new ScaleTransformer());
                    break;

                case 3:
                    ViewPager.SetPageTransformer(true, new WheelPageTransformer());
                    break;

                case 4:
                    ViewPager.SetPageTransformer(false, new SinkAndSlideTransformer());
                    break;
            }
        }




        //----------------------------------------------------------------------
        #region View  Handlers

        // 20150519
        void View__ControlsSetup()
        {
            //
            Button PC_Pause = FindViewById<Button>(Resource.Id.PC_PausePlay);
            PC_Pause.Click += delegate
            {
                //TimerAcitivityTimeout.Stop();
                SlideShowController.Pause();
            };
        }
        // 20150519
        void View__ControlsUnSetup()
        {
            // TODO: how to unregister delegate ?? :)
        }
        #endregion





        //----------------------------------------------------------------------
        #region SlideShowController  Handlers
        //
        static SlideShowController SlideShowController;
        // INOTE: static or so I thought, no matter how many time Android OS
        // wants to kill and re-create the View, I should only need one Controller instance ...
        // Some of the posts/docs seem to warn about keeping references to the View, not sure I am doing that, I am
        // trying to Cleanup (unlinking, unregistering listeners, etc...)
        //
        String MediaURL = "";
        static String LastMediaURL = "";
        static int LastIndex = 1;  // last index played by this View // base 1
        //
        //LinearLayout TargetsLayout;
        //LinearLayout VisualFeedbackLayout;



        // 20150515
        void SlideShowController__StartResume()
        {
            //
            SlideShowController__RestoreParameters();
            // Player Setup
            if (SlideShowController == null)  // || LastMediaURL != MediaURL
            {
                // Case1: this the very 1st creation of this activity 
#if DEBUG
                Toast.MakeText(this, "Case1: 1st-Created...", ToastLength.Short).Show();
#endif
                // -) Make AMSPlayer
                SlideShowController = new SlideShowController();

                // -) Re-connect to AMSPlayer instance
                SlideShowController__Reconnect();
                // -) Load media info
                // SlideShowController.Load(MediaURL);
                SlideShowController.Play(LastIndex);
            }
            else
            {
                if (LastMediaURL != MediaURL)
                {
                    // Case2: this is another creation of this actitivity but with a new MEDIA 
#if DEBUG
                    Toast.MakeText(this, "Case2: Re-Created...NEW Media", ToastLength.Short).Show();
#endif
                    // -) Re-connect to AMSPlayer instance
                    SlideShowController__Reconnect();
                    // -) Load media info
                    // SlideShowController.Load(MediaURL);
                    SlideShowController.Play(LastIndex);
                }
                else
                {
                    // Case3: this is another creation of this activity with same last MEDIA
                    // caused by ConfigurationChange (eg. rotation, ...)
#if DEBUG
                    Toast.MakeText(this, "Case3: Re-Created...SAME Media", ToastLength.Short).Show();
#endif
                    // -) Re-connect to AMSPlayer instance
                    SlideShowController__Reconnect();

                    // -) Since we are NOT reloading the Controller we won't be notified of Data_Ready we have to setup View explicitly
                    //PlayerControlsSetup();
                    //
                    //ViewPagerItems__Setup();
                    ViewPager__Setup();
                    SlideShowController.Play(LastIndex);
                }
            }
        }


        // 20150515
        void SlideShowController__StopSave()
        {
            //
            //TimerAcitivityTimeout.Stop();
            // -) Stop Controller (Player)
            // The function isFinishing() returns a boolean. 
            // True if your App is actually closing, 
            // False if your app is still running but for example the screen turns off.
            if (IsFinishing)
                SlideShowController.Stop();
            //
            SlideShowController__SaveParameters();
            //
            SlideShowController__Disconnect();
            //
            Cleanup();
        }
        // 20150515
        void SlideShowController__SaveParameters()
        {
            // Save
            // http://developer.android.com/guide/topics/data/data-storage.html#pref
            Android.Content.ISharedPreferences Settings = GetPreferences(FileCreationMode.Private);  // GetSharedPreferences("MumtiPref", 0);
            var Editor = Settings.Edit();
            // Remember last slide/media we are on.
            Editor.PutString("LastMediaURL", MediaURL);
            Editor.PutInt("LastIndex", LastIndex);
            // Commit the edits!
            Editor.Commit();
        }
        // 20150515
        /// <summary>
        /// Retrieve caller parameters - cross-activity parameters
        /// Retrieve session persisted parameters
        /// </summary>
        void SlideShowController__RestoreParameters()
        {

            //=========================================
            // -) Retrieve caller parameters - cross-activity parameters
            // What MEDIA?
            // eg. "http://arabicrescue.com/AR/KIDS/DU3AA/32/"
            MediaURL = Intent.GetStringExtra("MediaURL") ?? "UNKNOWN";
            if (MediaURL == "UNKNOWN")
            {
                Toast.MakeText(this, "Specified media is invalid at this time", ToastLength.Short).Show();
                //return;
            }
#if DEBUG
            Toast.MakeText(this, "MediaURL=" + MediaURL, ToastLength.Short).Show();
#endif

            //=========================================
            // -) Retrieve session persisted parameters
            // Do we have a previous session/life MEDIA / Index ?
            // http://developer.android.com/guide/topics/data/data-storage.html#pref
            Android.Content.ISharedPreferences Settings = GetPreferences(FileCreationMode.Private);  // GetSharedPreferences("MumtiPref", 0);
            // ??
            var ss = Settings.GetString("LastMediaURL", "UNKNOWN");
            if (ss != "UNKNOWN")
                LastMediaURL = ss;
            //
            LastIndex = Settings.GetInt("LastIndex", 1);
#if DEBUG
            Toast.MakeText(this, "LastIndex " + LastIndex.ToString(), ToastLength.Short).Show();
#endif

            // if starting a new Media, start from 1st, we only keep track of last while playing the same Media
            if (MediaURL != LastMediaURL)
                LastIndex = 1;
            //
        }



        // 20150228
        /// <summary>
        /// Reconnect with View, listeners, ...
        /// </summary>
        void SlideShowController__Reconnect()
        {
            // -) Reconnect the View
            // View Controller (player) was connected to an Activity instance which has been destroyed, re-connect to new one.
            SlideShowController.SlideShowView_I = this;
            // -) Re-connect all listeners (re-Subscribe to events)
            // SlideShowController.Done += SlideShowController_Done;

            // -) Subviews of this View instance
            // the following are accessed often so cache them per Activity.
            //TargetsLayout = FindViewById<LinearLayout>(Resource.Id.TargetsLayout);
            //VisualFeedbackLayout = FindViewById<LinearLayout>(Resource.Id.VisualFeedbackLayout);

        }

        // 20150515
        /// <summary>
        /// Disconnect with View, listeners, ...
        /// </summary>
        void SlideShowController__Disconnect()
        {
            // AMSPlayer was connected to an Activity instance which is being destroyed disconnect to avoid referring to it.
            // -) Disconnect the View
            SlideShowController.SlideShowView_I = null;

            // -) Disconnect all listeners (Unsubscribe to events)
            //SlideShowController.Done -= SlideShowController_Done;
        }



        #endregion




        //----------------------------------------------------------------------
        #region SlideShowView_I Members

        public void DisplaySlide(Slide Slide)
        {
            // TODO: how to pass to current fragment?
            // TODO: How to retrieve reference to current fragment?
            // https://github.com/codepath/android_guides/wiki/ViewPager-with-FragmentPagerAdapter
            //throw new NotImplementedException();
            ViewPager.SetCurrentItem(Slide.Index, true);
            //
            LastIndex = Slide.Index;
        }

        public Activity GetActivity()
        {
            return this;
        }

        #endregion
    }
}

