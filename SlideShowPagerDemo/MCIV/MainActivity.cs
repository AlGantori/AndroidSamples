﻿using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace SlideShowPager
{
    // https://github.com/Martynnw/AndroidDemos
    [Activity(Label = "@string/MainActivityLabel", Icon = "@drawable/icon", MainLauncher = true)] // ,MainLauncher = true
    public class MainActivity : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // uses a "canned" layout a "list selector"
            List<String> demos = new List<string> { "Basic", "Fade", "Scale", "Wheel", "Sink & Slide" };
            this.ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, demos);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var intent = new Intent(this, typeof(PagerActivity));
            intent.PutExtra(PagerActivity.PAGERTYPE, position);
            intent.PutExtra("MediaURL", "KIDS/JUMAL/BYTOPIC/20" );
            this.StartActivity(intent);
        }
    }
}

