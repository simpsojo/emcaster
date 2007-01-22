namespace Emcaster.Topics
{
    public interface ITopicMessage
    {
        string Topic { get; }

        object ParseObject();
        byte[] ParseBytes();
    }
}
