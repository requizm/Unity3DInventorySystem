using System.Collections;
using Core;
using Demo;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class PickableTest
    {
        [SetUp]
        public void SetUp()
        {
            var playerGameObject = new GameObject("Player");
            var player = playerGameObject.AddComponent<Player>();
            player.transform.position = new Vector3(0, 0, 0);
            var canvas = new GameObject("Canvas");
            AssemblyUtils.SetField(player, "canvas", canvas);


            var pickableGameObject = new GameObject("Pickable");
            var pickable = pickableGameObject.AddComponent<Pickable>();
            pickable.transform.position = new Vector3(0, 0, 10);
            SOItemAsset soItemAsset = Resources.Load<SOItemAsset>("Test/Items/TestItem");
            AssemblyUtils.SetField(pickable, "soItemAsset", soItemAsset);
            var visual = new GameObject("Visual");
            AssemblyUtils.SetField(pickable, "visual", visual);


            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(new InventoryManager());
            ServiceLocator.Current.Register(player);

            ServiceLocator.Current.InitializeServices();
        }
        
        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current.Cleanup();
            
            var pickables = Object.FindObjectsOfType<Pickable>();
            foreach (var pickable in pickables)
            {
                Object.DestroyImmediate(pickable.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator PickAndDropTest()
        {
            var pickable = Object.FindObjectOfType<Pickable>();
            Assert.IsFalse(pickable.IsPicked);
            yield return null;

            pickable.Pick();
            Assert.IsTrue(pickable.IsPicked);

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            Assert.AreEqual(1, inventoryManager.Count);
            Assert.AreEqual(pickable, (Pickable)inventoryManager.Items[0][0]);
            yield return null;

            pickable.Drop();
            Assert.IsFalse(pickable.IsPicked);
            Assert.AreEqual(0, inventoryManager.Count);
        }

        [UnityTest]
        public IEnumerator PickFailTest()
        {
            var pickable = Object.FindObjectOfType<Pickable>();
            pickable.Pick();
            Assert.IsTrue(pickable.IsPicked);

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            Assert.AreEqual(1, inventoryManager.Count);
            Assert.AreEqual(pickable, (Pickable)inventoryManager.Items[0][0]);
            yield return null;

            pickable.Pick();
            LogAssert.Expect(LogType.Error, $"{pickable.ItemAsset.AssetName}:{pickable.Id} already picked");
            yield return null;
        }

        [UnityTest]
        public IEnumerator DropFailTest()
        {
            var pickable = Object.FindObjectOfType<Pickable>();

            pickable.Drop();
            LogAssert.Expect(LogType.Error, $"{pickable.ItemAsset.AssetName}:{pickable.Id} is not picked");
            yield return null;
        }
    }
}
