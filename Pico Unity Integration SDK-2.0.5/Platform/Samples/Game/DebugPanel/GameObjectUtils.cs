using System.Collections.Generic;
using Pico.Platform.Samples.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Stark
{
    public class GameObjectUtils
    {
        private static Dictionary<string, GameObject> gameObjectCache = new Dictionary<string, GameObject>();

        private static GameObject FindGameObject(GameObject parent, string path)
        {
            GameObject goTarget = null;
            Transform tranTarget = FindTransform(parent, path);
            if (null != tranTarget)
            {
                goTarget = tranTarget.gameObject;
            }

            return goTarget;
        }

        // Component
        public static T AddComponent<T>(GameObject parent, string path) where T : Component
        {
            T scriptComponent = null;
            GameObject goTarget = FindGameObject(parent, path);
            if (null != goTarget)
            {
                scriptComponent = goTarget.GetComponent<T>();
                if (null == scriptComponent)
                {
                    scriptComponent = goTarget.AddComponent<T>();
                }
            }

            return scriptComponent;
        }

        public static GameObject CreateGameObject(string path)
        {
            GameObject prefab;
            if (!TryGetPrefabFromCache(path, out prefab))
            {
                prefab = LoadPrefab(path);
                if (prefab != null)
                    gameObjectCache.Add(path, prefab);
            }

            if (prefab != null)
            {
                GameObject objClone = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                return objClone;
            }

            return null;
        }

        private static bool TryGetPrefabFromCache(string key, out GameObject obj)
        {
            return gameObjectCache.TryGetValue(key, out obj);
        }

        public static GameObject LoadPrefab(string path)
        {
            GameObject localGameObj = Resources.Load<GameObject>(path);
            if (localGameObj == null)
            {
                LogHelper.LogError("GameObjectUtils.LoadPrefab - failed to load prefab from path '{0}'", path);
            }

            return localGameObj;
        }

        public static T GetComponent<T>(GameObject obj, bool addComponentIfNoExist = true) where T : Component
        {
            if (obj == null)
                return null;
            T script = obj.GetComponent<T>();
            if (script == null && addComponentIfNoExist)
                script = obj.AddComponent<T>();
            return script;
        }

        public static T GetComponent<T>(Transform transform, bool addComponentIfNoExist = true) where T : Component
        {
            if (transform == null)
                return null;
            return GetComponent<T>(transform.gameObject, addComponentIfNoExist);
        }

        public static T GetComponent<T>(Transform transform, string path) where T : Component
        {
            if (transform == null)
                return null;
            if (string.IsNullOrEmpty(path))
                return transform.GetComponent<T>();
            Transform childTransform = transform.Find(path);
            if (childTransform == null)
                return null;
            T script = childTransform.GetComponent<T>();
            return script;
        }

        public static GameObject FindGameObject(Transform transform, string path)
        {
            if (transform == null)
                return null;
            Transform findTransform = transform.Find(path);
            if (findTransform == null)
            {
                LogHelper.LogError("GameObjectUtils.FindGameObject - Failed to find GameObject from path '{0}'",
                    path);
                return null;
            }

            return findTransform.gameObject;
        }

        public static void AddButtonClick(GameObject parent, string path, UnityAction listener)
        {
            Button scriptBtn = null;
            GameObject goTarget = FindGameObject(parent, path);
            if (null != goTarget)
            {
                scriptBtn = goTarget.GetComponent<Button>();
            }

            if (null != scriptBtn)
            {
                scriptBtn.onClick.RemoveAllListeners();
                scriptBtn.onClick.AddListener(delegate
                {
                    if (listener != null)
                    {
                        listener();
                    }
                });
            }
        }

        // find node
        public static Transform FindTransform(GameObject parent, string path)
        {
            Transform transTarget = null;
            if (parent)
            {
                transTarget = parent.GetComponent<Transform>().Find(path);
            }
            else
            {
                GameObject goTarget = GameObject.Find(path);
                if (goTarget)
                {
                    transTarget = goTarget.transform;
                }
                else
                {
                    string rootPath = path;
                    int splitIndex = path.IndexOf("/");
                    if (0 <= splitIndex)
                    {
                        rootPath = path.Substring(0, splitIndex);
                    }

                    GameObject[] allSceneGo = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));
                    for (int i = 0; i < allSceneGo.Length; ++i)
                    {
                        if (allSceneGo[i].name == rootPath)
                        {
                            if (rootPath == path)
                            {
                                transTarget = allSceneGo[i].transform;
                            }
                            else
                            {
                                string remainPath = path.Substring(path.IndexOf("/") + 1,
                                    path.Length - path.IndexOf("/") - 1);
                                transTarget = allSceneGo[i].transform.Find(remainPath);
                            }

                            break;
                        }
                    }
                }
            }

            return transTarget;
        }
    }
}