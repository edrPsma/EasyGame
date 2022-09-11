using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EG.Core
{
    public class ContainableScanner
    {
        #region 创建父节点
        static Transform GetParentNode<T>() where T : ContainerAttribute
        {
            if (typeof(T) == typeof(SystemAttribute))
            {
                return ContainerManager.Instance.SystemNode;
            }
            else if (typeof(T) == typeof(ModelAttribute))
            {
                return ContainerManager.Instance.ModelNode;
            }
            else
            {
                return ContainerManager.Instance.UtilityNode;
            }
        }
        #endregion

        static Dictionary<object, IContainable> Scanning<T>(Dictionary<Type, Assembly> typesDic) where T : ContainerAttribute
        {
            var targetAttribute = typeof(T);
            Dictionary<object, IContainable> result = new Dictionary<object, IContainable>();

            foreach (var item in typesDic)
            {
                var attribute = item.Key.GetCustomAttribute(targetAttribute);
                if (attribute?.GetType() == targetAttribute)
                {
                    var tempAttribute = attribute as T;

                    GameObject obj = null;
                    Transform parent = GetParentNode<T>();

                    if (tempAttribute.Key != null)
                    {
                        Component instance = null;
                        obj = new GameObject(tempAttribute.Key.ToString());
                        instance = obj.AddComponent(item.Key);

                        result.Add(tempAttribute.Key, instance as IContainable);
                    }
                    else
                    {
                        Component instance = null;
                        obj = new GameObject(item.Key.ToString());
                        instance = obj.AddComponent(item.Key);

                        result.Add(item.Key, instance as IContainable);
                    }
                    obj.transform.SetParent(parent);
                }
            }
            return result;
        }

        public static Dictionary<object, IContainable> ScanningSystem(Dictionary<Type, Assembly> typesDic)
        {
            return Scanning<SystemAttribute>(typesDic);
        }

        public static Dictionary<object, IContainable> ScanningModel(Dictionary<Type, Assembly> typesDic)
        {
            return Scanning<ModelAttribute>(typesDic);
        }

        public static Dictionary<object, IContainable> ScanningUtility(Dictionary<Type, Assembly> typesDic)
        {
            return Scanning<UtilityAttribute>(typesDic);
        }
    }
}
