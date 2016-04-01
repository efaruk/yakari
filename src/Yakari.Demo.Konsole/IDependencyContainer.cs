using System;
using LightInject;

namespace Yakari.Demo.Konsole
{
    public interface IDependencyContainer<TContainer>: IDisposable
    {
        void Reset(TContainer container);

        T Resolve<T>();

        T Resolve<T>(string name);

        object Resolve(Type type);

        object Resolve(Type type, string name);

        IDisposable BeginScope();

    }
}