using System;
using System.Collections;
using System.Linq;
using Core;
using Demo;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

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

            var item1Slot = uiInventoryManager.ItemSlots[0];
            var item2Slot = uiInventoryManager.ItemSlots[1];


            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            Assert.AreEqual(item1Slot.Items.Count, item2Slot.Items.Count);

            for (var i = 0; i < item1Slot.Items.Count; i++)
            {
                Assert.AreEqual(item1Slot.Items[i].Id, inventoryManager.Items[0][i].Id);
            }

            for (var i = 0; i < item2Slot.Items.Count; i++)
            {
                Assert.AreEqual(item2Slot.Items[i].Id, inventoryManager.Items[1][i].Id);
            }
        }

        [UnityTest]
        public IEnumerator DropAndSwapTest()
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

            // Drop item of the third slot.
            var dropableItem = (Pickable)uiInventoryManager.ItemSlots[2].Items[0];
            dropableItem.Drop();

            Assert.AreEqual(stackLimit - 1, uiInventoryManager.ItemSlots[2].Items.Count);
            Assert.AreEqual(stackLimit - 1, inventoryManager.Items[2].Count);

            uiInventoryInteractor.SwapTwoItems(uiInventoryManager.ItemSlots[2], uiInventoryManager.ItemSlots[1]);

            Assert.AreEqual(stackLimit - 1, uiInventoryManager.ItemSlots[1].Items.Count);
            Assert.AreEqual(stackLimit - 1, inventoryManager.Items[1].Count);
            Assert.AreEqual(stackLimit, uiInventoryManager.ItemSlots[2].Items.Count);
            Assert.AreEqual(stackLimit, inventoryManager.Items[2].Count);
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

            var emptySlotIndex = inventoryManager.Items.Where(x => x.Count == 0).Select(x => inventoryManager.Items.IndexOf(x)).First();

            var item1Slot = uiInventoryManager.ItemSlots[0];
            var item2Slot = uiInventoryManager.ItemSlots[emptySlotIndex];

            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            Assert.AreEqual(0, item1Slot.Items.Count);
            Assert.True(item2Slot.Items.Count > 0);

            for (var i = 0; i < item2Slot.Items.Count; i++)
            {
                Assert.AreEqual(item2Slot.Items[i].Id, inventoryManager.Items[emptySlotIndex][i].Id);
            }
        }

        /// <summary>
        /// Tries to swap-merge two items in the inventory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [UnityTest]
        public IEnumerator SwapWithPartialMergeTest()
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

            var item1Slot = uiInventoryManager.ItemSlots[0];
            var item2Slot = uiInventoryManager.ItemSlots[1];

            var item2 = item2Slot.Items[0];
            ((Pickable)item2).Drop();

            // Swap two slots.
            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            Assert.AreEqual(2, item1Slot.Items.Count);
            Assert.AreEqual(stackLimit, item2Slot.Items.Count);

            for (var i = 0; i < item1Slot.Items.Count; i++)
            {
                Assert.AreEqual(item1Slot.Items[i].Id, inventoryManager.Items[0][i].Id);
            }

            for (var i = 0; i < item2Slot.Items.Count; i++)
            {
                Assert.AreEqual(item2Slot.Items[i].Id, inventoryManager.Items[1][i].Id);
            }
        }

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

            var item1Slot = uiInventoryManager.ItemSlots[0];
            var item1 = item1Slot.Items[0];
            ((Pickable)item1).Drop();

            var item2Slot = uiInventoryManager.ItemSlots[1];
            for (int i = 0; i < stackLimit - 1; i++)
            {
                ((Pickable)inventoryManager.Items[1][i]).Drop();
            }

            // Swap two slots.
            uiInventoryInteractor.SwapTwoItems(item1Slot, item2Slot);
            yield return null;

            Assert.AreEqual(0, item1Slot.Items.Count);
            Assert.AreEqual(stackLimit, item2Slot.Items.Count);

            for (var i = 0; i < item2Slot.Items.Count; i++)
            {
                Assert.AreEqual(item2Slot.Items[i].Id, inventoryManager.Items[1][i].Id);
            }
        }
    }
}
