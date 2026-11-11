namespace Elite580Link.Core.Hl7;

public class MllpFramer
{
    private const byte StartByte = 0x0B;
    private const byte EndByte = 0x1C;
    private const byte CarriageReturn = 0x0D;

    private readonly List<byte> _buffer = [];

    // Message structure looks like this:
    // <SB> Message <EB> <CR>
    // And since TCP segments arrive in stream of bytes and can chop messages into pieces.
    // We keep waiting until we see 0x1C followed by 0x0D
    public IReadOnlyList<byte[]> Feed(ReadOnlySpan<byte> data)
    {
        _buffer.AddRange(data);
        var messages = new List<byte[]>();

        while (true)
        {
            int start = _buffer.IndexOf(StartByte);
            if (start < 0) { _buffer.Clear(); break; }
            _buffer.RemoveRange(0, start);

            int end = IndexOfEnd();

            // wait until we found a message end. 
            if (end < 0) break;

            // Copy the current message.
            var message = _buffer.GetRange(0, end + 2);

            // Remove that message from the buffer end + bytes.
            // position:  0    1    2    3    4
            // bytes:     0b   41   42   1c   0d
            // Since IndexOf returns 3, RemoveRange(0, 3) removes positions: 0, 1, 2
            // (0b, 41, 42) we remove what's left in that buffer.
            _buffer.RemoveRange(0, end + 2);

            messages.Add([.. message]);
        }
        return messages;
    }

    private int IndexOfEnd()
    {
        for (int i = 0; i < _buffer.Count - 1; i++)
        {
            if (_buffer[i] == EndByte && _buffer[i + 1] == CarriageReturn)
            {
                return i;
            }
        }
        return -1;
    }
}
