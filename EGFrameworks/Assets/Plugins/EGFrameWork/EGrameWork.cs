using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG.Core;
using EG.Resource;
using EG.Scene;
using EG.UI;

namespace EG
{
    public static class EGrameWork
    {
        public static StartModel Model { get; set; }
        public static void Start(StartModel model)
        {
            Model = model;
            ContainerManager.Instance.Init(RegisterDefault);
        }

        static void RegisterDefault()
        {
            ContainerManager.Instance.RegisterUtility<ResourcesManager>();
            ContainerManager.Instance.RegisterUtility<ObjectPoolManager>();
            ContainerManager.Instance.RegisterUtility<UIManager>();
            ContainerManager.Instance.RegisterUtility<SceneManager>();
        }
    }

    public enum StartModel
    {
        Editor,
        Application
    }
}
