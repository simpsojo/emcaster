namespace Emcaster.Sockets
{
    public interface IByteParser
    {
        void OnBytes(byte[] data, int offset, int length);
    }
}