using System;
using System.Net.Http;
using Punchclock;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Disposables;
using Splat;

namespace Akavache.Http
{
    /// <summary>
    /// 
    /// </summary>
    public enum Priorities {
        Speculative = 10,
        UserInitiated = 100,
        Background = 20,
        //BackgroundGuaranteed = 30,
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHttpScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="priority"></param>
        /// <param name="shouldFetchContent"></param>
        /// <returns></returns>
        IObservable<Tuple<HttpResponseMessage, byte[]>> Schedule(HttpRequestMessage request, int priority, Func<HttpResponseMessage, bool> shouldFetchContent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxBytesToRead"></param>
        void ResetLimit(long? maxBytesToRead = null);

        /// <summary>
        ///
        /// </summary>
        void CancelAll();

        /// <summary>
        /// 
        /// </summary>
        HttpClient Client { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class NetCache 
    {
        static IHttpScheduler speculative;
        [ThreadStatic] static IHttpScheduler unitTestSpeculative;

        /// <summary>
        /// 
        /// </summary>
        public static IHttpScheduler Speculative
        {
            get { return unitTestSpeculative ?? speculative ?? Locator.Current.GetService<IHttpScheduler>("Speculative"); }
            set 
            {
                if (ModeDetector.InUnitTestRunner())
                {
                    unitTestSpeculative = value;
                    speculative = speculative ?? value;
                }
                else
                {
                    speculative = value;
                }
            }
        }
                
        static IHttpScheduler userInitiated;
        [ThreadStatic] static IHttpScheduler unitTestUserInitiated;

        /// <summary>
        /// 
        /// </summary>
        public static IHttpScheduler UserInitiated
        {
            get { return unitTestUserInitiated ?? userInitiated ?? Locator.Current.GetService<IHttpScheduler>("UserInitiated"); }
            set 
            {
                if (ModeDetector.InUnitTestRunner())
                {
                    unitTestUserInitiated = value;
                    userInitiated = userInitiated ?? value;
                }
                else
                {
                    userInitiated = value;
                }
            }
        }

        static IHttpScheduler background;
        [ThreadStatic] static IHttpScheduler unitTestBackground;

        /// <summary>
        /// 
        /// </summary>
        public static IHttpScheduler Background
        {
            get { return unitTestBackground ?? background ?? Locator.Current.GetService<IHttpScheduler>("Background"); }
            set 
            {
                if (ModeDetector.InUnitTestRunner())
                {
                    unitTestBackground = value;
                    background = background ?? value;
                }
                else
                {
                    background = value;
                }
            }
        }

        /*
        static IHttpScheduler backgroundGuaranteed;
        [ThreadStatic] static IHttpScheduler unitTestBackgroundGuaranteed;
        public static IHttpScheduler BackgroundGuaranteed
        {
            get { return unitTestBackgroundGuaranteed ?? backgroundGuaranteed ?? Locator.Current.GetService<IHttpScheduler>("BackgroundGuaranteed"); }
            set 
            {
                if (RxApp.InUnitTestRunner())
                {
                    unitTestBackgroundGuaranteed = value;
                    backgroundGuaranteed = backgroundGuaranteed ?? value;
                }
                else
                {
                    backgroundGuaranteed = value;
                }
            }
        }
        */
    }
}