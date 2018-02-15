namespace Binance.Client
{
    /// <summary>
    /// An <see cref="IUserDataClient"/> that can only be subscribed to a
    /// single user.
    /// </summary>
    public interface ISingleUserDataClient : IUserDataClient
    { }
}
