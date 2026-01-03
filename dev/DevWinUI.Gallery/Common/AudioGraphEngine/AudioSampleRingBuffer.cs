namespace DevWinUIGallery.Common;

public sealed partial class AudioSampleRingBuffer
{
    private readonly float[] buffer;
    private int writeIndex;
    private int readIndex;

    public AudioSampleRingBuffer(int capacity)
    {
        buffer = new float[capacity];
    }

    public void Write(float value)
    {
        buffer[writeIndex] = value;
        writeIndex = (writeIndex + 1) % buffer.Length;
    }

    public bool TryRead(out float value)
    {
        if (readIndex == writeIndex)
        {
            value = 0;
            return false;
        }

        value = buffer[readIndex];
        readIndex = (readIndex + 1) % buffer.Length;
        return true;
    }
}
