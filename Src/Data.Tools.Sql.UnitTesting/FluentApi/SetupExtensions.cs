using Data.Tools.UnitTesting.TestSetup;

namespace Data.Tools.UnitTesting.FluentApi
{
    public static class SetupExtensions
    {
        public static Test CreateTest(this Setup setup)
        {
            var test = new Test();
            test.Setup = setup;

            return test;
        }
    }


}
