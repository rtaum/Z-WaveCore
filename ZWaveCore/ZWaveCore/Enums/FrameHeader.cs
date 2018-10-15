namespace ZWaveCore.Enums
{
    public enum FrameHeader: byte
    {
        StartOfFrame = 0x01,
        Acknowledge = 0x06,
        NegativeAcknowledge = 0x15,
        Cancel = 0x18
    }
}
