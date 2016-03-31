namespace Yakari
{
    /// <summary>
    ///     Generic Serializer Interface
    /// </summary>
    public interface ISerializer<T>
    {
        T Serialize<TInput>(TInput instance);

        TOutput Deserialize<TOutput>(T data);
    }
}