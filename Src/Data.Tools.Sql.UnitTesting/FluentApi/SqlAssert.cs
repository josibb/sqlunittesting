using Data.Tools.UnitTesting.Equality;
using Data.Tools.UnitTesting.Result;
using System;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class SqlAssert
    {
        private static AssertionFailureDelegate assertionHandler = DefaultHandler;
        public static AssertionFailureDelegate AssertionHandler { get => assertionHandler; set => assertionHandler = value; }

        internal static void DefaultHandler(string assertionName, string message)
        {
            throw new Exception($"{assertionName} failed; {message}");
        }

        public static void Equals(ActionResult expected, ActionResult actual)
        {
            if (object.Equals(expected, actual))
                return;

            if (expected == null)
            {
                AssertionHandler("Equals", $"Expected: null; Actual: {actual.Serialize()}");
                return;
            }
            else if (actual == null)
            {
                AssertionHandler("Equals", $"Expected: {expected.Serialize()}; Actual: null");
                return;
            }

            if (!expected.EqualsActionResult(actual))
                AssertionHandler("Equals", $"Expected: {expected.Serialize()}; Actual: {actual.Serialize()}");
        }

        public static void MaxEllapsedSqlMilliseconds(ActionResult actionResult, long maxEllapsedSqlMilliseconds)
        {
            if (actionResult.EllapsedSqlMilliseconds > maxEllapsedSqlMilliseconds)
                AssertionHandler("MaxEllapsedSqlMilliseconds", $"Expected<={maxEllapsedSqlMilliseconds}ms; Actual={actionResult.EllapsedSqlMilliseconds}ms");
        }
    }

    public delegate void AssertionFailureDelegate(string assertionName, string message);
}
