using System;
using System.Threading;

namespace Yakari.Demo.Konsole
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            var test = new DelegateTest();
            test.OnDelegate1 += Test_OnDelegate1;
            test.OnDelegate2 += Test_OnDelegate2;

            test.TestDelegate1();
            test.TestDelegate2();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            var simpleDemo0 = new DemoSelfDecider(new DependencyContainer(null));
            //var simpleDemo1 = new SimpleDemo(new DependencyContainer(null));
            //var simpleDemo2 = new SimpleDemo(new DependencyContainer(null));
            //var simpleDemo3 = new SimpleDemo(new DependencyContainer(null));
            //var simpleDemo4 = new SimpleDemo(new DependencyContainer(null));
            simpleDemo0.StartDemo();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            simpleDemo0.StartDemo();
        }

        private static int Test_OnDelegate2(string data)
        {
            Console.WriteLine("Delegate2: {0}", Thread.CurrentThread.ManagedThreadId);
            return 0;
        }

        private static void Test_OnDelegate1(string data)
        {
            Console.WriteLine("Delegate1: {0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
