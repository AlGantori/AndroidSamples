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

namespace PagerDemo
{
    // https://github.com/Martynnw/AndroidDemos
    internal class DemoPagerAdapter : Android.Support.V13.App.FragmentStatePagerAdapter
    {
        private const int PageCount = 20;

        public DemoPagerAdapter(FragmentManager fm) : base(fm)
        {
        }

        public override Fragment GetItem(int position)
        {
            //return new PagerFragment();
            // return PagerFragment.newInstance( position, "Some title" );
            return new PagerFragment(position, "Some title");
        }

        public override int Count
        {
            get { return PageCount; }
        }
    }
}