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

using System;

namespace Pico.Platform.Models
{
    public class Error
    {
        public Error(int code, string msg)
        {
            this.Code = code;
            this.Message = msg;
        }

        public Error(IntPtr handle)
        {
            this.Code = CLIB.ppf_Error_GetCode(handle);
            this.Message = CLIB.ppf_Error_GetMessage(handle);
        }

        public readonly int Code;
        public readonly string Message;
    }
}