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
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PXR_SQPLoader : MonoBehaviour
{
    public const string RESOURCE_BUNDLE_NAME = "pxr_asset_resources";
    public const string SCENE_BUNDLE_NAME = "pxr_scene_";
    public const string SCENE_LOAD_DATA_NAME = "SQPData.txt";
    public const string ANDROID_DATA_PATH = "/sdcard/Android/data";
    private const string INDEX_NAME = "PxrSQPIndex";
    public const string SQP_INDEX_NAME = INDEX_NAME + ".unity";
    public const string CACHE_SCENES_PATH = "cache/sqp";

    private string sceneLoadDataPath = "";
    private long curVersion = 0;
    private List<string> curScenes = new List<string>();
    private List<AssetBundle> assetBundles = new List<AssetBundle>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        string scenePath = Path.Combine(Path.Combine(ANDROID_DATA_PATH, Application.identifier), CACHE_SCENES_PATH);
        sceneLoadDataPath = Path.Combine(scenePath, SCENE_LOAD_DATA_NAME);

        try
        {
            StreamReader reader = new StreamReader(sceneLoadDataPath);
            curVersion = Convert.ToInt64(reader.ReadLine());
            while (!reader.EndOfStream)
            {
                curScenes.Add(reader.ReadLine());
            }

            if (0 == curVersion || string.IsNullOrEmpty(curScenes[0]))
            {
                return;
            }

            string[] bundles = Directory.GetFiles(scenePath, "*_*");

            foreach (string bundle in bundles)
            {
                var assetBundle = AssetBundle.LoadFromFile(bundle);

                if (null != assetBundle)
                {
                    assetBundles.Add(assetBundle);
                }

                if ((SCENE_BUNDLE_NAME + curScenes[0].ToLower()) == assetBundle.name)
                {
                    string[] scenePaths = assetBundle.GetAllScenePaths();
                    SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(scenePaths[0]));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void Update()
    {
        long version = 0;
        try
        {
            StreamReader reader = new StreamReader(sceneLoadDataPath);
            version = Convert.ToInt64(reader.ReadLine());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        if (curVersion == version)
        {
            return;
        }

        foreach (var assetBundle in assetBundles)
        {
            if (null != assetBundle)
            {
                assetBundle.Unload(true);
            }
        }
        assetBundles.Clear();
        int activeScenes = SceneManager.sceneCount;

        for (int i = 0; i < activeScenes; i++)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            Destroy(go);
        }

        SceneManager.LoadSceneAsync(INDEX_NAME);
    }

}
