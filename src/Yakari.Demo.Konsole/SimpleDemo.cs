using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Bogus;

namespace Yakari.Demo.Konsole
{
    public class SimpleDemo : IDisposable
    {
        private readonly DependencyContainer _dependencyContainer;
        private Timer _timer = new Timer(100);

        public SimpleDemo(DependencyContainer dependencyContainer)
        {
            _dependencyContainer = dependencyContainer;
            _demoHelper = _dependencyContainer.Resolve<IDemoHelper>();
            _localCache = _dependencyContainer.Resolve<ICacheProvider>(DependencyContainer.LocalCacheProviderName);
            _timer.Elapsed += Cycle;
        }

        Random rnd = new Random(1);
        private ICacheProvider _localCache;
        private IDemoHelper _demoHelper;
        const int Max = 1000000;

        private bool Decide()
        {
            return (rnd.Next(Max) % 10 == 0);
        }

        private void Cycle(object sender, ElapsedEventArgs e)
        {
            if (Decide())
            {
                DoSomethingExpensive();
            }
        }

        private void DoSomethingExpensive()
        {
            var list = _localCache.Get<List<Person>>("personlist", TimeSpan.FromSeconds(3), () =>
            {
                var result = new List<Person>(_demoHelper.GeneratePersons(1000));
                return result;
            }, TimeSpan.FromMinutes(15));
            list.OrderBy(person => person.FirstName);
        }

        public void Dispose()
        {
            _dependencyContainer.Dispose();
        }

        public void StartDemo()
        {
            _timer.Start();
        }

        public void StopDemo()
        {
            _timer.Stop();
        }

    }
}
