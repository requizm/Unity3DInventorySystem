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
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName != "SampleScene")
            {
                Debug.Log($"Bootstrapper not initialized for scene {sceneName}");
                return;
            }

            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(new InventoryManager());
            ServiceLocator.Current.Register(Object.FindObjectOfType<Player>());
            ServiceLocator.Current.Register(new UIInventoryManager());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIInventory>());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIManager>());
            ServiceLocator.Current.Register(new UIInventorySearcher());

            ServiceLocator.Current.InitializeServices();

            Debug.Log("Bootstrapper initialized");
        }
    }
}
