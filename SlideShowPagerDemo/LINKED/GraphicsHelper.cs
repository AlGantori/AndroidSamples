using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.IO;
//
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//
using Android.Graphics;
using Android.Graphics.Drawables;  // 20150306
//
using VR.Utils;  // 20130503

//
namespace VR.Utils
{
    // https://github.com/xamarin/monodroid-samples/blob/master/CameraAppDemo/BitmapHelpers.cs
    public static class GraphicsHelper
    {

        // http://stackoverflow.com/questions/16503064/how-to-get-screen-size-in-android-including-virtual-buttons
        // 20150312
        /// <summary>
        /// Needed to define Display size
        /// </summary>
        /// <param name="Activity"></param>
        public static void Init(Activity Activity)
        {
            var displayMetrics = new Android.Util.DisplayMetrics();
            Display display = Activity.WindowManager.DefaultDisplay;
            display.GetMetrics(displayMetrics);
            // Define default Image size for 4:3
            DefaultImageWidth = displayMetrics.WidthPixels / 2;
            DefaultImageHeight = 3 * DefaultImageWidth / 4;
            //
            DisplayHeigth = displayMetrics.HeightPixels;
#if DEBUG
            //Toast.MakeText(Activity, "Display.Width=" + displayMetrics.WidthPixels.ToString(), ToastLength.Short).Show();
#endif
        }

        public static int DisplayHeigth { get; set; } 
        static int DefaultImageWidth = 400;
        static int DefaultImageHeight = 300;

        public static void SetImageSize( int Width, int Height )
        {
            DefaultImageWidth = Width ;
            DefaultImageHeight = Height ;
        }

        /// <summary>
        /// default resource id for "Media Not Found" bitmap
        /// </summary>
        public static int DefaultResID;


        // 20140322
        //----------------------------------------------------------------------
        /// <summary>
        /// Load an ImageView via its resource-id with the specified file from local storage
        /// </summary>
        /// <param name="Activity"></param>
        /// <param name="ResID"></param>
        /// <param name="FileName"></param>
        public static void Load(Activity Activity, int ResID, String FileName)
        {
            //
            using (var View = Activity.FindViewById<ImageView>(ResID))
            {
                //
                Load(Activity, View, FileName);
            }
        }

        //----------------------------------------------------------------------
        // 20140402
        /// <summary>
        /// Load an ImageView with the specified file from local storage
        /// </summary>
        /// <param name="Activity"></param>
        /// <param name="ImageView"></param>
        /// <param name="FileName"></param>
        public static void Load(Activity Activity, ImageView ImageView, String FileName)
        {
            Log.MethodCalled("FileName= " + FileName);
            // -) GRA
            if (File.Exists(FileName))
            {
                try
                {
                    // -)  
                    using (Bitmap Bitmap = ImageSampler.DecodeSampledBitmapFromFileName(FileName, DefaultImageWidth, DefaultImageHeight))
                    {
                        if (Bitmap == null)
                        {
                            //then there wasn't enough memory
                            Toast.MakeText(Activity, "GH: Ran out of memory", ToastLength.Short).Show();
                            System.Console.WriteLine("GH: Ran out of memory");
                            return;
                        }
                        ImageView.SetImageBitmap(Bitmap);
                        //Bitmap.Recycle();  // causes app to close
                        Bitmap.Dispose();                       
                    }

                    //ImageSampler2.DecodeSampledBitmapFromFileName(FileName, DefaultImageWidth, DefaultImageHeight, ImageView);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                // Approach 1) Resource.Drawable.MediaNotAvailable  // not reacheable when this file is used in different project/namespace
                // Approach 2) the following approach will not get compiler caught if Identifier changes :(
                // int ResId = Activity.Resources.GetIdentifier("MediaNotAvailable", "drawable", Activity.PackageName);
                // Approach 3) expect MediaNotAvailable set via a public member.
                //
                // -) Load the MEDIA_NOT_AVAILABLE bitmap
                try
                {
                    using (Bitmap Bitmap = ImageSampler.DecodeSampledBitmapFromResource(Activity.Resources, DefaultResID, DefaultImageWidth, DefaultImageHeight)) // 800, 600
                    {
                        if (Bitmap == null)
                        {
                            //then there wasn't enough memory
                            Toast.MakeText(Activity, "GH: Ran out of memory", ToastLength.Short).Show();
                            System.Console.WriteLine("GH: Ran out of memory");
                            return;
                        }
                        ImageView.SetImageBitmap(Bitmap);
                        //Bitmap.Recycle();
                        Bitmap.Dispose();
                        // https://forums.xamarin.com/discussion/7288/complex-image-gallery-viewpager-fragments-and-dinamic-layouts-out-of-memory
                        // You are not disposing your Bitmaps after you have passed them to the ImageView instances, hence C# will hold a reference and the underlying Java instance will never get recycled.
                    }

                    //ImageSampler2.DecodeSampledBitmapFromResource(Activity.Resources, DefaultResID, DefaultImageWidth, DefaultImageHeight, ImageView);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            System.GC.Collect();
        }

        // https://developer.android.com/training/displaying-bitmaps/manage-memory.html says its OS version dependant :( yuck !
        // borrowed from here // https://github.com/xamarin/monodroid-samples/blob/master/CameraAppDemo/BitmapHelpers.cs 
        // 20150306
        /// <summary>
        /// This method will recyle the memory help by a bitmap in an ImageView
        /// </summary>
        /// <param name="imageView">Image view.</param>
        public static void RecycleBitmap(this ImageView imageView)
        {
            if (imageView == null)
            {
                return;
            }
            // Crash with: System.ArgumentException: 'jobject' must not be IntPtr.Zero.
            // Sol? found here https://github.com/toggl/mobile/blob/master/Joey/UI/Adapters/BaseDataViewAdapter.cs
            // Need to access the Handle property, else mono optimises/loses the context and we get a weird
            // low-level exception about "'jobject' must not be IntPtr.Zero".
            if (imageView.Handle == IntPtr.Zero)
            {
                return;
            }
             
            Drawable toRecycle = imageView.Drawable;
            if (toRecycle != null)
            {
                ((BitmapDrawable)toRecycle).Bitmap.Recycle(); // .Recycle() .Dispose()?
            }
        }


    }
}