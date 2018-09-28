// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Kooboo.HttpServer
{
    public interface ITimeoutControl
    {
        void SetTimeout(long ticks, TimeoutAction timeoutAction);
        void ResetTimeout(long ticks, TimeoutAction timeoutAction);
        void CancelTimeout();

        void StartTimingReads();
        void PauseTimingReads();
        void ResumeTimingReads();
        void StopTimingReads();
        void BytesRead(long count);

        void StartTimingWrite(long size);
        void StopTimingWrite();
    }
}
