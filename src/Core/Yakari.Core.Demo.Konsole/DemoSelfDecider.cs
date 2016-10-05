using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Yakari.Core.Interfaces;

namespace Yakari.Core.Demo.Konsole
{
    public class DemoSelfDecider : IDisposable
    {
        readonly DemoDependencyContainer _demoDependencyContainer;
        readonly Timer _timer;

        public DemoSelfDecider(DemoDependencyContainer demoDependencyContainer)
        {
            _demoDependencyContainer = demoDependencyContainer;
            _demoHelper = _demoDependencyContainer.Resolve<IDemoHelper>();
            _logger = _demoDependencyContainer.Resolve<ILogger>();
            _localCache = _demoDependencyContainer.Resolve<ILocalCacheProvider>();
            _timer = new Timer(Cycle, null, int.MaxValue, 0);
        }

        Random rnd = new Random(1);
        ILocalCacheProvider _localCache;
        IDemoHelper _demoHelper;
        ILogger _logger;
        const int Max = 1000000;

        bool Decide()
        {
            // ReSharper disable once ArrangeRedundantParentheses : Parentheses left for readability
            return (rnd.Next(Max) % 10 == 0);
        }

        void Cycle(object sender)
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

        void DoSomethingExpensive()
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
            _timer.Change(100, 100);
        }
    }
}
