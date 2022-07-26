/*******************************************************************************
Copyright © 2015-2022 Pico Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
Pico Technology Co., Ltd. The intellectual and technical concepts 
contained hererin are proprietary to Pico Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
Pico Technology Co., Ltd. 
*******************************************************************************/

using System;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_PassThroughDescriptor : SubsystemDescriptor<PXR_PassThroughSystem>
    {
        public struct Cinfo : IEquatable<Cinfo>
        {
            public string id { get; set; }
            public Type ImplementaionType { get; set; }

            public bool Equals(Cinfo other)
            {
                return id == other.id ||ImplementaionType == other.ImplementaionType;
            }
        }
        internal static PXR_PassThroughDescriptor Create(Cinfo info)
        {
            return new PXR_PassThroughDescriptor(info);
        }

        public PXR_PassThroughDescriptor(Cinfo info)
        {
            id = info.id;
            subsystemImplementationType = info.ImplementaionType;
        }
    }
}
