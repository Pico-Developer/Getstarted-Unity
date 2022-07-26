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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.XR.PXR;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using Object = UnityEngine.Object;

namespace Unity.XR.PXR.Editor
{
    public class PXR_BuildProcessor : XRBuildHelper<PXR_Settings>
    {
        public override string BuildSettingsKey { get { return "Unity.XR.PXR.Settings"; } }

        private bool IsCurrentBuildTargetValid(BuildReport report)
        {
            return report.summary.platformGroup == BuildTargetGroup.Android;
        }

        private bool HasLoaderEnabledForTarget(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup != BuildTargetGroup.Android)
                return false;

            XRGeneralSettings settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (settings == null)
                return false;

            bool loaderFound = false;
#if UNITY_2021_1_OR_NEWER
            for (int i = 0; i < settings.Manager.activeLoaders.Count; ++i)
            {
                if (settings.Manager.activeLoaders[i] as PXR_Loader != null)
                {
                    loaderFound = true;
                    break;
                }
            }
#else
            for (int i = 0; i < settings.Manager.loaders.Count; ++i)
            {
                if (settings.Manager.loaders[i] as PXR_Loader != null)
                {
                    loaderFound = true;
                    break;
                }
            }
#endif

            return loaderFound;
        }

        private readonly string[] runtimePluginNames = new string[]
        {
            "achievenment.jar",
            "Pico_PaymentSDK_Android_V1.0.34.aar",
            "PxrPlatform.aar",
            "pxr_api-release.aar"
        };

        private bool ShouldIncludeRuntimePluginsInBuild(string path, BuildTargetGroup platformGroup)
        {
            return HasLoaderEnabledForTarget(platformGroup);
        }
        
        public override void OnPreprocessBuild(BuildReport report)
        {
            if (IsCurrentBuildTargetValid(report) && HasLoaderEnabledForTarget(report.summary.platformGroup))
                base.OnPreprocessBuild(report);

            var allPlugins = PluginImporter.GetAllImporters();
            foreach (var plugin in allPlugins)
            {
                if (plugin.isNativePlugin)
                {
                    foreach (var pluginName in runtimePluginNames)
                    {
                        if (plugin.assetPath.Contains(pluginName))
                        {
                            plugin.SetIncludeInBuildDelegate((path) => { return ShouldIncludeRuntimePluginsInBuild(path, report.summary.platformGroup); });
                            break;
                        }
                    }
                }
            }
        }
    }

    public static class PXR_BuildTools
    {
        public static bool LoaderPresentInSettingsForBuildTarget(BuildTargetGroup btg)
        {
            var generalSettingsForBuildTarget = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(btg);
            if (!generalSettingsForBuildTarget)
                return false;
            var settings = generalSettingsForBuildTarget.AssignedSettings;
            if (!settings)
                return false;
            List<XRLoader> loaders = settings.loaders;
            return loaders.Exists(loader => loader is PXR_Loader);
        }

        public static PXR_Settings GetSettings()
        {
            PXR_Settings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<PXR_Settings>("Unity.XR.PXR.Settings", out settings);
#else
            settings = PXR_Settings.settings;
#endif
            return settings;
        }
    }

    internal class PXR_PrebuildSettings : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)

        {
            if (!PXR_BuildTools.LoaderPresentInSettingsForBuildTarget(report.summary.platformGroup))
                return;
            if (report.summary.platformGroup == BuildTargetGroup.Android)
            {
                GraphicsDeviceType firstGfxType = PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0];
                if (firstGfxType != GraphicsDeviceType.OpenGLES3 && firstGfxType != GraphicsDeviceType.Vulkan && firstGfxType != GraphicsDeviceType.OpenGLES2)
                {
                    throw new BuildFailedException("OpenGLES2, OpenGLES3, and Vulkan are currently the only graphics APIs compatible with the Pico XR Plugin on mobile platforms.");
                }
                if (PXR_BuildTools.GetSettings().stereoRenderingModeAndroid == PXR_Settings.StereoRenderingModeAndroid.Multiview && firstGfxType == GraphicsDeviceType.OpenGLES2)
                {
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
                }
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel26)
                {
                    throw new BuildFailedException("Minimum API must be set to 26 or higher for Pico XR Plugin.");
                }
                PlayerSettings.Android.forceSDCardPermission = true;
            }
        }
    }

#if UNITY_ANDROID
    internal class PXR_Manifest : IPostGenerateGradleAndroidProject
    {
        static readonly string androidURI = "http://schemas.android.com/apk/res/android";

        static readonly string androidManifestPath = "/src/main/AndroidManifest.xml";

        void UpdateOrCreateAttributeInTag(XmlDocument doc, string parentPath, string tag, string name, string value)
        {
            var xmlNode = doc.SelectSingleNode(parentPath + "/" + tag);

            if (xmlNode != null)
            {
                ((XmlElement)xmlNode).SetAttribute(name,androidURI, value);
            }
        }

        void UpdateOrCreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName, string secondValue)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            foreach (XmlNode node in xmlNodeList)
            {
                var attributeList = ((XmlElement)node).Attributes;

                foreach (XmlAttribute attrib in attributeList)
                {
                    if (attrib.Value == firstValue)
                    {
                        XmlAttribute valueAttrib = attributeList[secondName, androidURI];
                        if (valueAttrib != null)
                        {
                            valueAttrib.Value = secondValue;
                        }
                        else
                        {
                            ((XmlElement)node).SetAttribute(secondName, androidURI, secondValue);
                        }
                        return;
                    }
                }
            }
            
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, androidURI, firstValue);
            childElement.SetAttribute(secondName, androidURI, secondValue);

            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        void CreateNameValueElementsInTag(XmlDocument doc, string parentPath, string tag,
            string firstName, string firstValue, string secondName = null, string secondValue = null, string thirdName=null, string thirdValue=null)
        {
            var xmlNodeList = doc.SelectNodes(parentPath + "/" + tag);

            // don't create if the firstValue matches
            foreach (XmlNode node in xmlNodeList)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Value == firstValue)
                    {
                        return;
                    }
                }
            }
            
            XmlElement childElement = doc.CreateElement(tag);
            childElement.SetAttribute(firstName, androidURI, firstValue);

            if (secondValue != null)
            {
                childElement.SetAttribute(secondName, androidURI, secondValue);
            }
            if (thirdValue != null)
            {
                childElement.SetAttribute(thirdName, androidURI, thirdValue);
            }
            var xmlParentNode = doc.SelectSingleNode(parentPath);

            if (xmlParentNode != null)
            {
                xmlParentNode.AppendChild(childElement);
            }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if(!PXR_BuildTools.LoaderPresentInSettingsForBuildTarget(BuildTargetGroup.Android))
               return;

            var manifestPath = path + androidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);
            var sdkVersion = (int)PlayerSettings.Android.minSdkVersion;
            var nodePath = "/manifest/application";
			UpdateOrCreateAttributeInTag(manifestDoc, "/manifest","application", "requestLegacyExternalStorage","true");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "pvr.app.type", "value", "vr");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "pvr.sdk.version", "value", "XR Platform_2.0.5.4");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "enable_cpt", "value", PXR_ProjectSetting.GetProjectConfig().useContentProtect ? "1" : "0");
            UpdateOrCreateNameValueElementsInTag(manifestDoc, nodePath, "meta-data", "name", "enable_entitlementcheck", "value", PXR_PlatformSetting.Instance.startTimeEntitlementCheck ? "1" : "0");
            CreateNameValueElementsInTag(manifestDoc, "/manifest", "uses-permission","name", "android.permission.WRITE_SETTINGS");		

			nodePath = "/manifest";
            manifestDoc.Save(manifestPath);
        }

        public int callbackOrder { get { return 10000; } }
    }
#endif

}
