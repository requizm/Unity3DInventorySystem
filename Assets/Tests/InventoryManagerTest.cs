using System.Collections;
using System.Collections.Generic;
using Core;
using Demo;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InventoryManagerTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Initialize default service locator.
            ServiceLocator.Initialize();

            // Register all your services next.
            ServiceLocator.Current.Register(new InventoryManager());
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

        [Test]
        public void FullInventoryTest()
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

        [Test]
        public void EmptyInventoryTest()
        {
            var inventoryManager = ServiceLocator.Current.Get<InventoryManager>();
            Assert.AreEqual(0, inventoryManager.Count);

            var item = new TestItem(999, new TestItemAsset(999, $"Asset999", new List<string>(), 2, null, null));
            inventoryManager.RemoveItem(item);
            LogAssert.Expect(LogType.Error, $"Item {item.ItemAsset.AssetName}:{item.Id} not found");
            Assert.AreEqual(0, inventoryManager.Count);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        /*[UnityTest]
        public IEnumerator InventoryManagerTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }*/
    }
}
