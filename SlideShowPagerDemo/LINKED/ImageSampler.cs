

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
using Android.Graphics;
//

namespace VR.Utils
{
    //  inPurgeable was deprecated in API level 21
    // http://developer.android.com/reference/android/graphics/BitmapFactory.Options.html#inPurgeable
    // While inPurgeable can help avoid big Dalvik heap allocations (from API level 11 onward), it sacrifices performance predictability since any image that the view system tries to draw may incur a decode delay which can lead to dropped frames. Therefore, most apps should avoid using inPurgeable to allow for a fast and fluid UI. To minimize Dalvik heap allocations use the inBitmap flag instead.
    // 
    // 20130421
    public static class ImageSampler
    {


        //----------------------------------------------------------------------
        #region Load Bitmaps effeciently to avoid VM memory exception from bitmaps loading

        //----------------------------------------------------------------------

        // http://developer.android.com/training/displaying-bitmaps/load-bitmap.html
        // http://docs.xamarin.com/recipes/android/resources/general/load_large_bitmaps_efficiently
        //BitmapFactory.Options BFOptions = new BitmapFactory.Options();
        //BFOptions.InJustDecodeBounds = true;

        // 20130421
        public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            var height = (float)options.OutHeight;
            var width = (float)options.OutWidth;
            var inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                inSampleSize = width > height
                                    ? height / reqHeight
                                    : width / reqWidth;
            }

            return (int)inSampleSize;
        }

        // http://stackoverflow.com/questions/477572/strange-out-of-memory-issue-while-loading-an-image-to-a-bitmap-object/823966#823966
        // 20130421
        /// <summary>
        /// Decode bitmap from resource
        /// </summary>
        /// <param name="res">eg. this.context.Resources</param>
        /// <param name="resId"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static Bitmap DecodeSampledBitmapFromResource(Android.Content.Res.Resources res, int resId, int reqWidth, int reqHeight)
        {
            try
            {
                // First decode with inJustDecodeBounds=true to check dimensions
                var options = new BitmapFactory.Options { InPurgeable = true, InJustDecodeBounds = true }; 
                BitmapFactory.DecodeResource(res, resId, options);  // returns BM = null 

                // Calculate inSampleSize
                options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

                // Decode bitmap with inSampleSize set
                options.InJustDecodeBounds = false;
                return BitmapFactory.DecodeResource(res, resId, options);
            }
            catch (Exception e)
            {
                Android.Util.Log.Error("DecodeSampledBitmapFromResource", "Exception: " + e.Message);
                return null;
            }

        }


        // https://github.com/xamarin/monodroid-samples/blob/master/CameraAppDemo/BitmapHelpers.cs
        // 20130421
        /// <summary>
        /// Decode bitmap from local file
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static Bitmap DecodeSampledBitmapFromFileName(String FileName, int reqWidth, int reqHeight)
        {
            // http://stackoverflow.com/questions/7535503/mono-for-android-exit-code-255-on-bitmapfactory-decodestream
            try
            {
                // First decode with inJustDecodeBounds=true to check dimensions
                var options = new BitmapFactory.Options { InPurgeable = true, InJustDecodeBounds = true };
                BitmapFactory.DecodeFile(FileName, options);  // returns BM = null 

                var options2 = new BitmapFactory.Options { }; 
                // Calculate inSampleSize
                options2.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

                // Decode bitmap with inSampleSize set
                options.InJustDecodeBounds = false;
                return BitmapFactory.DecodeFile(FileName, options2);
            }
            catch (Exception e)
            {
                Android.Util.Log.Error("DecodeSampledBitmapFromFileName", "Exception: " + e.Message);
                return null;
            }

        }


        // 20130421
        /// <summary>
        /// Decode bitmap from ByteArray as returned by webClient.DownloadDataAsync(URL)
        /// </summary>
        /// <param name="Bytes"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static Bitmap DecodeSampledBitmapFromByteArray(byte[] Bytes, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            var options = new BitmapFactory.Options { InPurgeable = true, InJustDecodeBounds = true };
            BitmapFactory.DecodeByteArray(Bytes, 0, Bytes.Length, options);  // returns BM = null 

            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            // http://stackoverflow.com/questions/15193463/monodroid-loading-images-efficiently
            // http://stackoverflow.com/questions/7068132/why-would-i-ever-not-use-bitmapfactorys-inpurgeable-option
            options.InPurgeable = true;  // suggested in above posts, apparently used with .DecodeByteArray
            return BitmapFactory.DecodeByteArray(Bytes, 0, Bytes.Length, options);
        }


        #endregion
    }
}