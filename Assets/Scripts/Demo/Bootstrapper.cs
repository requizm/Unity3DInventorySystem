using System.Linq;
using Core;
using UnityEngine;

namespace Demo
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(new InventoryManager());
            ServiceLocator.Current.Register(Object.FindObjectOfType<Player>());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIInventoryManager>());

            foreach (var binder in Object.FindObjectsOfType<MonoBehaviour>().OfType<IBinder>())
            {
                binder.Initialize();
            }
        }
    }
}
