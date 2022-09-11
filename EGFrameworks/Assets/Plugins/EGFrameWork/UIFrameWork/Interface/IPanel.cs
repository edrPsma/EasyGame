using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG.UI.Core
{
    public interface IPanel
    {
        void Init();
        void Enter();
        void Pause();
        void Resume();
        void Exit();
    }
}
