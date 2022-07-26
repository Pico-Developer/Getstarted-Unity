/*******************************************************************************
Copyright © 2015-2022 Pico Technology Co., Ltd.All rights reserved.

NOTICE：All information contained herein is, and remains the property of
Pico Technology Co., Ltd. The intellectual and technical concepts
contained herein are proprietary to Pico Technology Co., Ltd. and may be
covered by patents, patents in process, and are protected by trade secret or
copyright law. Dissemination of this information or reproduction of this
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd.
*******************************************************************************/

namespace Pico.Platform
{
    public enum PlatformInitializeResult
    {
        Unknown = -999,
        NetError = -6,
        MissingImpl = -5,
        LoadImplFailed = -4,
        InternalError = -3,
        InvalidParams = -2,
        AlreadyInitialized = -1,
        Success = 0,
    }
    
    public enum GameConnectionEvent
    {
        Connected = 0,
        Closed = 1,
        Lost = 2,
        Resumed = 3,
        KickedByRelogin = 4,
        KickedByGameServer = 5,
        GameLogicError = 6,
        Unknown = 7,
    }
    
    public enum GameRequestFailedReason
    {
        None = 0,
        NotInitialized = 1,
        Uninitialized = 2,
        CurrentlyUnavailable = 3,
        CurrentlyUnknown = 4,
    }
    
    public enum GameInitializeResult
    {
        Success = 0,
        Uninitialized = 1,
        NetworkError = 2,
        InvalidCredentials = 3,
        ServiceNotAvailable = 4,
        Unknown = 5,
    }
    
    public enum KVPairType
    {
        String = 0,
        Int = 1,
        Double = 2,
        Unknown = 3,
    }
}
