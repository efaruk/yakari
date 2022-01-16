using System;

namespace Yakari.Demo
{
    public interface IDependencyContainer
    {

        void Setup(IServiceProvider serviceProvider);

        T Resolve<T>();

        object Resolve(Type type);

        //IDisposable BeginScope();

    }
}