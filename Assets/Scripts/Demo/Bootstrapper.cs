using System.Linq;
using Core;
using UnityEngine;

namespace Demo
{
    /// <summary>
    /// Bootstrapper is a class that initializes all services.
    /// </summary>
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
            ServiceLocator.Current.Register(new UIInventoryManager());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIInventory>());

            ServiceLocator.Current.InitializeServices();
        }
    }
}
