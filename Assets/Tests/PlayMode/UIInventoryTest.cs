using System;
using System.Collections;
using Core;
using Demo;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
            var uiInventoryManager = managerGameObject.GetComponent<UIInventoryManager>();

            uiInventoryManager.pagesPanel = pagesPanel;
            uiInventoryManager.pageButtonsPanel = pageButtonsPanel;
            uiInventoryManager.searchInput = searchInput;

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
            ServiceLocator.Current.Register(new UIInventoryInteractor());
            ServiceLocator.Current.Register(Object.FindObjectOfType<UIInventoryManager>());
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

        /// <summary>
        ///  Test if all slots are empty.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator EmptySlotTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

            var items = inventoryManager.Items;
            var itemSlots = uiInventoryManager.ItemSlots;
            Assert.AreEqual((inventoryManager.Limit / inventoryManager.PageLimit) * inventoryManager.PageLimit,
                itemSlots.Count);

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(0, itemSlots[i].Items.Count);
                Assert.AreEqual(0, items[i].Count);
            }
        }

        /// <summary>
        /// Pick all items and check if they are in the inventory and UI.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator PickTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

            var pickables = Object.FindObjectsOfType<Pickable>();
            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            var items = inventoryManager.Items;
            var itemSlots = uiInventoryManager.ItemSlots;

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(items[i].Count, itemSlots[i].Items.Count);
                for (var j = 0; j < items[i].Count; j++)
                {
                    Assert.AreEqual(items[i][j].Id, itemSlots[i].Items[j].Id);
                }
            }
        }

        /// <summary>
        /// Pick all items. Then drop all items and check if all slots are empty.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator DropTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();

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
            var itemSlots = uiInventoryManager.ItemSlots;

            for (var i = 0; i < itemSlots.Count; i++)
            {
                Assert.AreEqual(0, itemSlots[i].Items.Count);
                Assert.AreEqual(0, items[i].Count);
            }
        }

        /// <summary>
        /// Tries to swap two items in the inventory.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator SwapNotNullTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            var uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();

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

            var item1Slot = uiInventoryManager.GetItemSlot(item1);
            var item2Slot = uiInventoryManager.GetItemSlot(item2);
            var oldSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var oldSlotOrder2 = item2Slot.transform.GetSiblingIndex();


            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
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

        /// <summary>
        /// Tries to swap two slots in the inventory. But second slot is empty.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator SwapNullTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            var uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();

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

            var item1Slot = uiInventoryManager.GetItemSlot(item1);
            var item2Slot = uiInventoryManager.ItemSlots[randomIndex2];
            var oldSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var oldSlotOrder2 = item2Slot.transform.GetSiblingIndex();


            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
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

        /// <summary>
        /// Tries to swap-merge two items in the inventory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [UnityTest]
        public IEnumerator SwapWithMergeTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            var uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();

            // Pick items
            var pickables = Object.FindObjectsOfType<Pickable>();
            var stackLimit = pickables[0].ItemAsset.StackLimit;
            if (stackLimit == 1)
            {
                throw new Exception("Stack limit is 1. It should be more than 1");
            }

            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            // Select random two slots and drop one item from the second slot.
            var randomIndex1 = Random.Range(0, 15);
            var randomIndex2 = Random.Range(0, 15);
            while (randomIndex1 == randomIndex2)
            {
                randomIndex2 = Random.Range(0, 15);
            }

            var item1 = inventoryManager.Items[randomIndex1][0];
            var oldItemOrder1 = randomIndex1;
            var item1Slot = uiInventoryManager.GetItemSlot(item1);


            var item2 = inventoryManager.Items[randomIndex2][0];
            var oldItemOrder2 = randomIndex2;
            var item2Slot = uiInventoryManager.GetItemSlot(item2);
            ((Pickable)item2).Drop();

            // Swap two slots.
            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            // Sibling index should be the same because items are merged. Not swapped.
            var newSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var newSlotOrder2 = item2Slot.transform.GetSiblingIndex();
            Assert.AreEqual(oldItemOrder1, newSlotOrder1);
            Assert.AreEqual(oldItemOrder2, newSlotOrder2);

            // Item order should be the same because items are merged. Not swapped.
            var newItemOrder1 = inventoryManager.GetItemIndex(item1);
            var newItemOrder2 = randomIndex2;
            Assert.AreEqual(oldItemOrder1, newItemOrder1.Item1);
            Assert.AreEqual(oldItemOrder2, newItemOrder2);

            // Count of first item should be decreased by 1.
            Assert.AreEqual(stackLimit - 1, item1Slot.Items.Count);
            Assert.AreEqual(stackLimit - 1, inventoryManager.Items[newItemOrder1.Item1].Count);

            // Count of second item should be increased by 1.
            Assert.AreEqual(stackLimit, item2Slot.Items.Count);
            Assert.AreEqual(stackLimit, inventoryManager.Items[newItemOrder2].Count);
        }

        /// <summary>
        /// Tries to swap-merge two items in the inventory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [UnityTest]
        public IEnumerator SwapWithFullMergeTest()
        {
            yield return null;

            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var uiInventoryManager = ServiceLocator.Current.Get<UIInventoryManager>();
            var uiInventoryInteractor = ServiceLocator.Current.Get<UIInventoryInteractor>();

            // Pick items
            var pickables = Object.FindObjectsOfType<Pickable>();
            var stackLimit = pickables[0].ItemAsset.StackLimit;
            if (stackLimit == 1)
            {
                throw new Exception("Stack limit is 1. It should be more than 1");
            }

            foreach (var pickable in pickables)
            {
                pickable.Pick();
            }

            // Select random two slots and drop one item from the second slot.
            var randomIndex1 = Random.Range(0, 15);
            var randomIndex2 = Random.Range(0, 15);
            while (randomIndex1 == randomIndex2)
            {
                randomIndex2 = Random.Range(0, 15);
            }

            var item1 = inventoryManager.Items[randomIndex1][0];
            var oldItemOrder1 = randomIndex1;
            var item1Slot = uiInventoryManager.GetItemSlot(item1);
            ((Pickable)item1).Drop();


            var item2 = inventoryManager.Items[randomIndex2][0];
            var oldItemOrder2 = randomIndex2;
            var item2Slot = uiInventoryManager.GetItemSlot(item2);
            for (int i = 0; i < stackLimit - 1; i++)
            {
                ((Pickable)inventoryManager.Items[randomIndex2][i]).Drop();
            }

            // Swap two slots.
            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            // Sibling index should be the same because items are merged. But first item is empty.
            var newSlotOrder1 = item1Slot.transform.GetSiblingIndex();
            var newSlotOrder2 = item2Slot.transform.GetSiblingIndex();
            Assert.AreEqual(oldItemOrder1, newSlotOrder1);
            Assert.AreEqual(oldItemOrder2, newSlotOrder2);

            // Item order should be the same because items are merged.  But first item is empty.
            var newItemOrder1 = randomIndex1;
            var newItemOrder2 = randomIndex2;
            Assert.AreEqual(oldItemOrder1, newItemOrder1);
            Assert.AreEqual(oldItemOrder2, newItemOrder2);

            // Count of first item should be decreased to 0.
            Assert.AreEqual(0, item1Slot.Items.Count);
            Assert.AreEqual(0, inventoryManager.Items[newItemOrder1].Count);

            // Count of second item should be increased to stack limit.
            Assert.AreEqual(stackLimit, item2Slot.Items.Count);
            Assert.AreEqual(stackLimit, inventoryManager.Items[newItemOrder2].Count);
        }
    }
}
