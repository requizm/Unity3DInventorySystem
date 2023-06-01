using System.Collections;
using Core;
using Demo;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class UIInventoryTest
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

        [UnityTest]
        public IEnumerator SwapNotNullTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventory = ServiceLocator.Current.Get<UIInventory>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

            var pickables = Object.FindObjectsOfType<Pickable>();
            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            var randomIndex1 = Random.Range(0, 15);
            var randomIndex2 = Random.Range(0, 15);
            while (randomIndex1 == randomIndex2)
            {
                randomIndex2 = Random.Range(0, 15);
            }

            var item1 = inventoryManager.Items[randomIndex1][0];
            var item2 = inventoryManager.Items[randomIndex2][0];
            var oldItemOrder1 = randomIndex1;
            var oldItemOrder2 = randomIndex2;

            var item1Slot = uiInventory.GetItemSlot(item1);
            var item2Slot = uiInventory.GetItemSlot(item2);
            var oldSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var oldSlotOrder2 = item2Slot.transform.GetSiblingIndex();


            uiInventoryManager.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            var newSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var newSlotOrder2 = item2Slot.transform.GetSiblingIndex();
            Assert.AreEqual(oldSlotOrder1, newSlotOrder2);
            Assert.AreEqual(oldSlotOrder2, newSlotOrder1);

            var newItemOrder1 = inventoryManager.GetItemIndex(item1);
            var newItemOrder2 = inventoryManager.GetItemIndex(item2);

            Assert.AreEqual(oldItemOrder1, newItemOrder2.Item1);
            Assert.AreEqual(oldItemOrder2, newItemOrder1.Item1);
        }

        [UnityTest]
        public IEnumerator SwapNullTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventory = ServiceLocator.Current.Get<UIInventory>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

            var pickables = Object.FindObjectsOfType<Pickable>();
            var stackLimit = pickables[0].ItemAsset.StackLimit;
            var pageLimit = inventoryManager.PageLimit - 3;
            var itemLimit = pageLimit * stackLimit;
            for (var i = 0; i < itemLimit; i++)
            {
                pickables[i].Pick();
            }

            var randomIndex1 = Random.Range(0, pageLimit);
            var randomIndex2 = inventoryManager.GetEmptySlot();
            while (randomIndex1 == randomIndex2)
            {
                randomIndex1 = Random.Range(0, pageLimit);
            }

            var item1 = inventoryManager.Items[randomIndex1][0];
            IItem item2 = null;
            var oldItemOrder1 = randomIndex1;
            var oldItemOrder2 = randomIndex2;

            var item1Slot = uiInventory.GetItemSlot(item1);
            var item2Slot = uiInventory.ItemSlots[randomIndex2];
            var oldSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var oldSlotOrder2 = item2Slot.transform.GetSiblingIndex();


            uiInventoryManager.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            var newSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var newSlotOrder2 = item2Slot.transform.GetSiblingIndex();
            Assert.AreEqual(oldSlotOrder1, newSlotOrder2);
            Assert.AreEqual(oldSlotOrder2, newSlotOrder1);

            var newItemOrder1 = inventoryManager.GetItemIndex(item1);
            var newItemOrder2 = randomIndex1;

            Assert.AreEqual(oldItemOrder1, newItemOrder2);
            Assert.AreEqual(oldItemOrder2, newItemOrder1.Item1);
        }
    }
}