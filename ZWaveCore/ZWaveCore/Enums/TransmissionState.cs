namespace ZWaveCore.Enums
{
    public enum TransmissionState
    {
        CompleteOk = 0x00,
        CompleteNoAcknowledge = 0x01,
        CompleteFail = 0x02,
        CompleteNoRoute = 0x04,
        NoAcknowledge = 0x05,
        ResMissing = 0x06,
    }
}
