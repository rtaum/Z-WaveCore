﻿namespace ZWaveCore.Enums
{
    public enum Function : byte
    {
        None = 0x00,
        DiscoveryNodes = 0x02,
        SerialApiApplNodeInformation = 0x03,
        ApplicationCommandHandler = 0x04,
        GetControllerCapabilities = 0x05,
        SerialApiSetTimeouts = 0x06,
        SerialGetCapabilities = 0x07,
        SerialApiSoftReset = 0x08,
        SetRFReceiveMode = 0x10,
        SetSleepMode = 0x11,
        SendNodeInformation = 0x12,
        SendData = 0x13,
        SendDataMulti = 0x14,
        GetVersion = 0x15,
        SendDataAbort = 0x16,
        RFPowerLevelSet = 0x17,
        SendDataMeta = 0x18,
        MemoryGetId = 0x20,
        MemoryGetByte = 0x21,
        MemoryPutByte = 0x22,
        MemoryGetBuffer = 0x23,
        MemoryPutBuffer = 0x24,
        ReadMemory = 0x23,
        ClockSet = 0x30,
        ClockGet = 0x31,
        ClockCompare = 0x32,
        RtcTimerCreate = 0x33,
        RtcTimerRead = 0x34,
        RtcTimerDelete = 0x35,
        RtcTimerCall = 0x36,
        GetNodeProtocolInfo = 0x41,
        SetDefault = 0x42,
        ReplicationCommandComplete = 0x44,
        ReplicationSendData = 0x45,
        AssignReturnRoute = 0x46,
        DeleteReturnRoute = 0x47,
        RequestNodeNeighborUpdate = 0x48,
        ApplicationUpdate = 0x49,
        AddNodeToNetwork = 0x4a,
        RemoveNodeFromNetwork = 0x4b,
        CreateNewPrimary = 0x4c,
        ControllerChange = 0x4d,
        SetLearnMode = 0x50,
        AssignSucReturnRoute = 0x51,
        EnableSuc = 0x52,
        RequestNetworkUpdate = 0x53,
        SetSucNodeId = 0x54,
        DeleteSucReturnRoute = 0x55,
        GetSucNodeId = 0x56,
        SendSucId = 0x57,
        RediscoveryNeeded = 0x59,
        RequestNodeInfo = 0x60,
        RemoveFailedNodeId = 0x61,
        IsFailedNode = 0x62,
        ReplaceFailedNode = 0x63,
        TimerStart = 0x70,
        TimerRestart = 0x71,
        TimerCancel = 0x72,
        TimerCall = 0x73,
        GetRoutingTableLine = 0x80,
        GetTXCounter = 0x81,
        ResetTXCounter = 0x82,
        StoreNodeInfo = 0x83,
        StoreHomeId = 0x84,
        LockRouteResponse = 0x90,
        SendDataRouteDemo = 0x91,
        SerialApiTest = 0x95,
        SerialApiSlaveNodeInfo = 0xa0,
        ApplicationSlaveCommandHandler = 0xa1,
        SendSlaveNodeInfo = 0xa2,
        SendSlaveData = 0xa3,
        SetSlaveLearnMode = 0xa4,
        GetVirtualNodes = 0xa5,
        IsVirtualNode = 0xa6,
        SetPromiscuousMode = 0xd0,
        SetConfiguration = 0xF2,
        GetConfiguration = 0xF3,
    }
}
