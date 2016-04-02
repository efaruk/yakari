using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yakari.Demo.Konsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
    }
}
