using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Yakari.Demo.Konsole
{
    public class DemoSelfDecider : IDisposable
    {
        private readonly DemoDependencyContainer _demoDependencyContainer;
        private readonly Timer _timer = new Timer(100);

        public DemoSelfDecider(DemoDependencyContainer demoDependencyContainer)
        {
            _demoDependencyContainer = demoDependencyContainer;
            _demoHelper = _demoDependencyContainer.Resolve<IDemoHelper>();
            _logger = _demoDependencyContainer.Resolve<ILogger>();
            _localCache = _demoDependencyContainer.Resolve<ILocalCacheProvider>();
            _timer.Elapsed += Cycle;
        }

        Random rnd = new Random(1);
        private ILocalCacheProvider _localCache;
        private IDemoHelper _demoHelper;
        private ILogger _logger;
        const int Max = 1000000;

        private bool Decide()
        {
            // ReSharper disable once ArrangeRedundantParentheses : Parentheses left for readability
            return (rnd.Next(Max) % 10 == 0);
        }

        private void Cycle(object sender, ElapsedEventArgs e)
        {
            if (Decide())
            {
                try
                {
                    DoSomethingExpensive();
                }
                catch (Exception ex)
                {
                    _logger.Log("SimpleDemo", ex);
                }
            }
        }

        private void DoSomethingExpensive()
        {
            var list = _localCache.Get("personlist", TimeSpan.FromSeconds(3), () =>
            {
                var result = new List<DemoObject>(_demoHelper.GenerateDemoObjects(1000));
                return result;
            }, TimeSpan.FromMinutes(15));
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed : Sake of Demo
            list.OrderBy(o => o.CreatedAt);
        }

        public void Dispose()
        {
            _demoDependencyContainer.Dispose();
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
