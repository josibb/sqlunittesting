using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Tools.UnitTesting.Utils
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string paramName) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
            else
            {
                return obj;
            }
        }

        public static void ThrowIfNull<TException>(this object obj, string message) where TException : Exception
        {
            message.ThrowIfNull("message");
            if (obj == null)
            {
                throw (Exception)Activator.CreateInstance(typeof(TException), message);
            }
        }

        public static string ThrowIfNullOrEmpty<TException>(this string s, string message)
            where TException : Exception
        {
            return s.ThrowIfNullOrEmpty<TException, TException>(message);
        }

        public static string ThrowIfNullOrEmpty<TNullException, TEmptyException>(this string s, string message)
            where TNullException : Exception
            where TEmptyException : Exception
        {
            message.ThrowIfNull("message");

            if (s == null)
            {
                throw (Exception)Activator.CreateInstance(typeof(TNullException), message);
            }
            else if (string.IsNullOrEmpty(s))
            {
                throw (Exception)Activator.CreateInstance(typeof(TEmptyException), message);
            }
            else
            {
                return s;
            }
        }
    }
}
