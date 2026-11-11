namespace Elite580Link.Core.Hl7;

public class MllpFramer
{
    private const byte StartBlock = 0x0B;
    private const byte EndBlock = 0x0D;
    private const byte CarriageReturn = 0x0D;

    private readonly List<byte> _buffer = [];

    public IReadOnlyList<string> Feed(byte[] data)
    {
        throw new NotImplementedException();
    }

    private int IndexOfEnd()
    {
        for (int i = 0; i < _buffer.Count; i++)
        {
            if (_buffer[i] == EndBlock && _buffer[i + 1] == CarriageReturn)
            {
                return i;
            }
        }
        return -1;
    }
}
