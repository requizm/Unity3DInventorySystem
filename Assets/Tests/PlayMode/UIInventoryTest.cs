using System.Collections;
using Core;
using Demo;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class SwapItemTest
    {
        [SetUp]
        public void SetUp()
        {
            var canvasGameObject = Resources.Load<GameObject>("Test/Prefabs/Canvas");
            var canvas = Object.Instantiate(canvasGameObject);
            var pagesPanel = canvas.transform.Find("Inventory").Find("Pages").gameObject;
            var pageButtonsPanel = canvas.transform.Find("Inventory").Find("PageButtons").gameObject;
            var searchInput = canvas.transform.Find("Inventory").Find("InputField (TMP)")
                .GetComponent<TMP_InputField>();

            var managerPrefab = Resources.Load<GameObject>("Test/Prefabs/Manager");
            var managerGameObject = Object.Instantiate(managerPrefab);
            var uiManager = managerGameObject.GetComponent<UIManager>();
            var uiInventory = managerGameObject.GetComponent<UIInventory>();

            uiInventory.pagesPanel = pagesPanel;
            uiInventory.pageButtonsPanel = pageButtonsPanel;
            uiInventory.searchInput = searchInput;

            var playerPrefab = Resources.Load<GameObject>("Test/Prefabs/Player");
            var playerGameObject = Object.Instantiate(playerPrefab);
            var player = playerGameObject.GetComponent<Player>();

            AssemblyUtils.SetField(player, "canvas", canvas);
            AssemblyUtils.SetField(uiManager, "canvas", canvas);


            var testObjectsPrefab = Resources.Load<GameObject>("Test/Prefabs/TestObjects");
            Object.Instantiate(testObjectsPrefab);


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
        public IEnumerator EmptySlotTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventory = ServiceLocator.Current.Get<UIInventory>();

            var items = inventoryManager.Items;
            var itemSlots = uiInventory.ItemSlots;
            Assert.AreEqual((inventoryManager.Limit / inventoryManager.PageLimit) * inventoryManager.PageLimit,
                itemSlots.Count);

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(0, itemSlots[i].Items.Count);
                Assert.AreEqual(0, items[i].Count);
            }
        }

        [UnityTest]
        public IEnumerator PickTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventory = ServiceLocator.Current.Get<UIInventory>();

            var pickables = Object.FindObjectsOfType<Pickable>();
            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            var items = inventoryManager.Items;
            var itemSlots = uiInventory.ItemSlots;

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(items[i].Count, itemSlots[i].Items.Count);
                for (var j = 0; j < items[i].Count; j++)
                {
                    Assert.AreEqual(items[i][j].Id, itemSlots[i].Items[j].Id);
                }
            }
        }

        [UnityTest]
        public IEnumerator DropTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventory = ServiceLocator.Current.Get<UIInventory>();

            var pickables = Object.FindObjectsOfType<Pickable>();
            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            foreach (var pickable in pickables)
            {
                pickable.Drop();
            }

            var items = inventoryManager.Items;
            var itemSlots = uiInventory.ItemSlots;

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(0, itemSlots[i].Items.Count);
                Assert.AreEqual(0, items[i].Count);
            }
        }
    }
}
