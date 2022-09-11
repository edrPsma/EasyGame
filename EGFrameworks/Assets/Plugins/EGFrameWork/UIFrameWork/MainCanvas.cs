using UnityEngine;
using UnityEngine.UI;

namespace EG.UI.Core
{
    public class MainCanvas : ViewController
    {
        private static MainCanvas mInstance;
        public static MainCanvas Instance => mInstance;
        public CanvasScaler Scaler { get; private set; }

        private void Awake()
        {
            mInstance = this;
            Scaler = GetComponent<CanvasScaler>();
        }
    }
}
