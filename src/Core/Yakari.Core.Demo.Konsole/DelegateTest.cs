using System;
using System.Threading;

namespace Yakari.Core.Demo.Konsole
{
    public class DelegateTest
    {
        public void TestDelegate1()
        {
            Console.WriteLine("TestDelegate1: {0}", Thread.CurrentThread.ManagedThreadId);
            if (OnDelegate1 != null)
                OnDelegate1("Delegate1");
        }

        public void TestDelegate2()
        {
            Console.WriteLine("TestDelegate2: {0}", Thread.CurrentThread.ManagedThreadId);
            if (OnDelegate2 != null)
                OnDelegate2("Delegate2");
        }

        public event Delegate1 OnDelegate1;

        public event Delegate2 OnDelegate2;
    }

    public delegate void Delegate1(string data);

    public delegate int Delegate2(string data);
}