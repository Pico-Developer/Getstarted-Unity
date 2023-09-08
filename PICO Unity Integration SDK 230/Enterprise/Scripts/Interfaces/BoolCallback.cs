﻿using System;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class BoolCallback : AndroidJavaProxy
    {
        public Action<bool> mCallback;
  
        public BoolCallback(Action<bool> callback) : base("com.picoxr.tobservice.interfaces.BoolCallback")
        {
            mCallback = callback;
        }

        public void CallBack(bool var1)
        {
            Debug.Log("ToBService IBoolCallBack 回调:" + var1);
            PXR_EnterpriseTools.Instance.QueueOnMainThread(() =>
            {
                if (mCallback!=null)
                {
                    mCallback(var1);
                }
            });
        }
    }
}