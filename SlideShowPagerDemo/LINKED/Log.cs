

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
// http://developer.xamarin.com/guides/cross-platform/application_fundamentals/building_cross_platform_applications/part_4_-_platform_divergence_abstraction_divergent_implementation/
#if __ANDROID__
using Android.Views;
using Android.Widget;
#endif
//
//
using System.Diagnostics;  // 20120401 for StackFrame, StackTrace
using System.Reflection;  // 20120401 for MethodBase

//
namespace VR.Utils
{
    // The following is about writing to the Android Debug Log
    // http://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/android_debug_log/
    // This log is a monster, don't know if can filter the view to just my own lines...??

    // DebuggerStepThrough
    // http://abhijitjana.net/2010/09/22/tips-on-debugging-using-debuggerstepthrough-attribute/
    // http://www.codeproject.com/Articles/111965/Tips-on-Debugging-Using-DebuggerStepThrough-attrib
    // http://stackoverflow.com/questions/10696304/adding-debuggerstepthrough-attribute-to-class
    /// <summary>
    /// 20100228
    /// </summary>
    [DebuggerStepThrough]
    public static class Log
    {

        public static Boolean verbose = false;
        public static string LogText = "";
        static int LogLineCounter = 0;
        /// <summary>
        /// 20091102
        /// </summary>
        /// <param name="msg"></param>
        public static void Write(string msg)
        {
            LogLineCounter++;
            LogText = LogText + LogLineCounter.ToString() + " " + msg + "\n";
            // Console.WriteLine(msg); // won't work
            // System.Diagnostics.Debugger.Log(0, "Debug", msg); // won't work
            //System.Diagnostics.Debugger.Log(0, null, msg + "\n");

            System.Diagnostics.Debug.WriteLine(msg + "\n\n", "Log.Write");  // monodroid: goes to Output window

            //Console.WriteLine(msg);
        }
        /// <summary>
        /// 20100301
        /// </summary>
        /// <param name="msg"></param>
        public static void EventReceived(string msg)
        {
            Write("    <<<<EventReceived------------- " + msg + " ---- from " + WhoCalledMe());
        }
        // 20120401
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="CurrentInstance"></param>
        public static void EventReceived(string msg, Object CurrentInstance)
        {
            Write("    <<<<EventReceived------------T2- " + msg + " ---<<<from<<< " + CurrentInstance.ToString());
        }
        // 20120402
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="CurrentInstance"></param>
        public static void EventReceivedSys(string msg, Object CurrentInstance)
        {
            Write("    <<<<EventReceived------------T3- " + msg + " ---<<<from-Sys<<< " + CurrentInstance.ToString());
        }

        /// <summary>
        /// 20100301
        /// </summary>
        /// <param name="msg"></param>
        public static void EventFire(string msg)
        {
            Write("    !!!!!EventFire------------- " + msg);
        }
        // 20120401
        /// <summary>
        /// Pass Current Instance
        /// </summary>
        /// <param name="msg"></param>
        public static void EventFire(string msg, Object Caller)
        {
            Write("    !!!!!EventFire------------- " + msg + " ---- from " + Caller.GetType().ToString());
        }



        /// <summary>
        /// 20100507
        /// </summary>
        /// <param name="msg"></param>
        public static void MethodCalled(string msg)
        {
            Write("  --Called->> " + msg + " ---- from " + WhoCalledMe()); //  + WhoCalledMe());
        }
        // 20120402
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void MethodCalled(string msg, Object CurrentInstance)
        {
            Write("  --Called->> " + msg + " ---- from " + WhoCalledMe() + " -----<<<from<<< " + CurrentInstance.ToString());
        }
        /// <summary>
        /// 20100507
        /// </summary>
        /// <param name="msg"></param>
        public static void MethodCall(string msg)
        {
            Write("  --Calling............ " + msg);
        }



        /// <summary>
        /// 20100507
        /// </summary>
        /// <param name="msg"></param>
        public static void Constructor(string msg)
        {
            Write("!!--Make-------------------------- " + msg + " ......Constructor called");
        }
        // 20120401
        public static void Constructor(string msg, Object CurrentInstance)
        {
            Constructor(msg + " -----<<<from<<< " + CurrentInstance.ToString());
        }




        public static String Context(Object CurrentInstance)
        {
            return " <<<from<<< " + CurrentInstance.ToString();
        }
        /// <summary>
        /// 20100507
        /// </summary>
        /// <param name="msg"></param>
        public static void RawData(string msg)
        {
            if (verbose)
                Write("\n=======RawData========\n" + msg + "\n=======RawData========\n");
        }




        /// <summary>
        /// 20111213
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void Exception(string msg, Exception ex)
        {
            Write(String.Format("    * * *Exception* * *-------------Info={0}\n ex.msg={1}\n ex.Inner={2}\n",
                msg, ex.Message, ex.InnerException));
        }

        ///// <summary>
        ///// 20111222
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="ex"></param>
        //public static void Exception(string msg, ExceptionRoutedEventArgs ex)
        //{
        //    Write(String.Format("    *****Exception-------------Info={0}\n ErrorException={1} ", msg,
        //         ex.ErrorException.ToString()));
        //}
        ///// <summary>
        ///// 20111222
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="ex"></param>
        //public static void Exception(string msg, MediaFailedRoutedEventArgs ex)
        //{
        //    Write(String.Format("    *****Exception-------------Info={0}/n ErrorException={1} ", msg,
        //         ex.ErrorException.ToString()));
        //}



        //===================================================================
        // Function to display parent function 
        public static String WhoCalledMe()
        {
            try
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(2);
                MethodBase methodBase = stackFrame.GetMethod();


                //// Displays “WhatsmyName” 
                ////Console.WriteLine(" Parent Method Name {0} ", methodBase.Name);
                return String.Format("{0}.{1}()", methodBase.DeclaringType.Name, methodBase.Name);
            }
            catch (Exception)
            {

                return "UNKNOWN: WhoCalledMe() failed";
            }


            //return "TODO:";
        }


        //===================================================================
        #region // Handle GUI buttons...
        // 20100228
        // Requires the following to be set prior to calling method...
#if __ANDROID__
        public static TextView LOG;  // TextBox
#endif
        //
        private static int Log_cc = 0;
        public static void ShowHide()
        {
#if __ANDROID__
            Log_cc++;
            if (Log_cc >= 3)
            {

                if (LOG.Visibility == ViewStates.Invisible)
                {
                    LOG.Visibility = ViewStates.Visible;
                    LOG.SetHeight(400);
                    LOG.Text = Log.LogText;
                    // make the page bg visible

                }
                else
                {
                    LOG.Visibility = ViewStates.Invisible;
                    Log_cc = 0;
                }
            }
#endif
        }

        #endregion

    }
}
