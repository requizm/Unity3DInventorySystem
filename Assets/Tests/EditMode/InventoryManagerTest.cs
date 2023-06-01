using System.Collections.Generic;
using Core;
using Demo;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditMode
{
    public class InventoryManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(new InventoryManager());
        }
        
        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current.Cleanup();
        }

        /// <summary>
        /// Checks if AddItem and RemoveItem works correctly
        /// </summary>
        [Test]
        public void AddRemoveItemTest()
        {
            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            Assert.AreEqual(inventoryManager.Items.Count, inventoryManager.Limit);

            var item1 = new TestItem(375, new TestItemAsset(0, "Asset1", new List<string>(), 2, null, null));
            var item2 = new TestItem(777, new TestItemAsset(1, "Asset2", new List<string>(), 2, null, null));
            inventoryManager.AddItem(item1);
            inventoryManager.AddItem(item2);
            Assert.AreEqual(2, inventoryManager.Count);
            Assert.AreEqual(375, inventoryManager.Items[0][0].Id);
            Assert.AreEqual(777, inventoryManager.Items[1][0].Id);

            inventoryManager.RemoveItem(item1);
            Assert.AreEqual(1, inventoryManager.Count);

            inventoryManager.RemoveItem(item2);
            Assert.AreEqual(0, inventoryManager.Count);
        }

        /// <summary>
        /// Adds items to inventory until it is full and checks if it is full.
        /// </summary>
        [Test]
        public void FullInventoryFailTest()
        {
            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var items = new List<IItem>();
            for (var i = 0; i < inventoryManager.Limit; i++)
            {
                var item = new TestItem(i, new TestItemAsset(i, $"Asset{i}", new List<string>(), 2, null, null));
                items.Add(item);
                inventoryManager.AddItem(item);
            }

            Assert.AreEqual(inventoryManager.Limit, inventoryManager.Count);
            Assert.AreEqual(inventoryManager.Limit, inventoryManager.Items.Count);

            for (var i = 0; i < inventoryManager.Limit; i++)
            {
                Assert.AreEqual(items[i].Id, inventoryManager.Items[i][0].Id);
            }

            var newItem = new TestItem(999, new TestItemAsset(999, $"Asset999", new List<string>(), 2, null, null));
            inventoryManager.AddItem(newItem);
            LogAssert.Expect(LogType.Error, $"Inventory is full");
            Assert.AreEqual(inventoryManager.Limit, inventoryManager.Count);
            Assert.AreEqual(inventoryManager.Limit, inventoryManager.Items.Count);
        }

        /// <summary>
        /// Checks if RemoveItem works correctly when inventory is empty
        /// </summary>
        [Test]
        public void EmptyInventoryFailTest()
        {
            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            Assert.AreEqual(0, inventoryManager.Count);

            var item = new TestItem(999, new TestItemAsset(999, $"Asset999", new List<string>(), 2, null, null));
            inventoryManager.RemoveItem(item);
            LogAssert.Expect(LogType.Error, $"Item {item.ItemAsset.AssetName}:{item.Id} not found");
            Assert.AreEqual(0, inventoryManager.Count);
        }

        /// <summary>
        /// Tries to add items with same type and checks stack count.
        /// </summary>
        [Test]
        public void StackTest()
        {
            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            var testItemAsset = new TestItemAsset(999, $"Asset999", new List<string>(), 2, null, null);
            var item1 = new TestItem(999, testItemAsset);
            var item2 = new TestItem(888, testItemAsset);
            inventoryManager.AddItem(item1);
            inventoryManager.AddItem(item2);
            Assert.AreEqual(1, inventoryManager.Count);
            Assert.AreEqual(2, inventoryManager.Items[0].Count);
            Assert.AreEqual(999, inventoryManager.Items[0][0].Id);
            Assert.AreEqual(888, inventoryManager.Items[0][1].Id);

            inventoryManager.RemoveItem(item1);
            Assert.AreEqual(1, inventoryManager.Count);
            Assert.AreEqual(1, inventoryManager.Items[0].Count);
            Assert.AreEqual(888, inventoryManager.Items[0][0].Id);
        }
    }
}
