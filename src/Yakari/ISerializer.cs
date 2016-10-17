namespace Yakari
{
    /// <summary>
    ///     Generic Serializer Interface
    /// </summary>
    public interface ISerializer
    {
        object Serialize<TInput>(TInput instance);

        TOutput Deserialize<TOutput>(object data);
    }

}