using System.Linq;
using Core;
using UnityEngine;

namespace Demo
{
    /// <summary>
    /// Initializes all services.
    /// </summary>
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            // Skip if not in SampleScene. This is for PlayMode tests.
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
            ServiceLocator.Current.Register(new UIInventoryInteractor());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIInventoryManager>());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIManager>());
            ServiceLocator.Current.Register(new UIInventorySearcher());

            ServiceLocator.Current.InitializeServices();

            Debug.Log("Bootstrapper initialized");
        }
    }
}
