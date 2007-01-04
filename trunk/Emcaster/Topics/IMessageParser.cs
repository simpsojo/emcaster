namespace Emcaster.Topics
{
    public interface IMessageParser
    {
        string Topic { get; }

        object ParseObject();
    }
}