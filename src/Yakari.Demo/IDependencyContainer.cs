using System;

namespace Yakari.Demo
{
    public interface IDependencyContainer<TContainer>: IDisposable
    {
        void Replace(TContainer container);

        T Resolve<T>();

        T Resolve<T>(string name);

        object Resolve(Type type);

        IDisposable BeginScope();

    }
}