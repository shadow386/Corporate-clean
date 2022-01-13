using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FallenMoon.Library
{
    public class SocialMedia : IDisposable
    {
        #region IDisposable implementation with finalizer
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion

        public void PostMessage(string message)
        {
            Facebook facebook = new Facebook();
            Twitter twitter = new Twitter();

            facebook.Post(message);
        }

        public List<Tweet> GetTweets(string userName, int count)
        {
            Twitter twitter = new Twitter();
            twitter.LoadTweets(userName, count);

            return twitter.Tweets;
        }
    }
}
