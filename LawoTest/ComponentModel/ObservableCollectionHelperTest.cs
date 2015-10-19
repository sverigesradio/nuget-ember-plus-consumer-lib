﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2015 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Lawo.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>Tests <see cref="ObservableCollectionHelper"/>.</summary>
    [TestClass]
    public sealed class ObservableCollectionHelperTest : TestBase
    {
        /// <summary>Tests the main <see cref="ObservableCollectionHelper"/> use cases.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void MainTest()
        {
            AssertChange(c => { });
            AssertChange(c => c.Add(42));
            AssertChange(c => c.Insert(1, 42));
            AssertChange(c => c.Remove(2));
            AssertChange(c => c.RemoveAt(3));
            AssertChange(c => c.Move(1, 3));
            AssertChange(c => c[2] = 42);
            AssertChange(c => c.Clear());
        }

        /// <summary>Tests the exceptions thrown by <see cref="ObservableCollectionHelper"/>.</summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test code.")]
        [TestCategory("Unattended")]
        [TestMethod]
        public void ExceptionTest()
        {
            var collection = new ObservableCollection<int>();
            AssertThrow<ArgumentNullException>(
                () => ((ObservableCollection<int>)null).AddChangeHandlers((int i, int j) => { }, (i, j) => { }, () => { }),
                () => collection.AddChangeHandlers(null, (int i, int j) => { }, () => { }),
                () => collection.AddChangeHandlers((int i, int j) => { }, null, () => { }),
                () => collection.AddChangeHandlers((int i, int j) => { }, (i, j) => { }, null),
                () => ((ObservableCollection<int>)null).Project((int i) => i),
                () => collection.Project((Func<int, int>)null));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AssertChange(Action<ObservableCollection<int>> change)
        {
            var o = new ObservableCollection<int>(Enumerable.Range(0, this.Random.Next(4, 10)));
            var c = new List<int>();
            var handler = o.AddChangeHandlers(
                (int index, int item) => c.Insert(index, item), (index, item) => c.RemoveAt(index), c.Clear);
            AssertChange(o, change, c, handler);

            using (var projection = o.Project((int i) => i))
            {
                AssertChange(o, change, projection);
            }
        }

        private static void AssertChange(
            ObservableCollection<int> original,
            Action<ObservableCollection<int>> change,
            ICollection copy,
            NotifyCollectionChangedEventHandler handler)
        {
            try
            {
                change(original);
                CollectionAssert.AreEqual(original, copy);
            }
            finally
            {
                original.CollectionChanged -= handler;
            }

            original.Add(42);
            CollectionAssert.AreNotEqual(original, copy);
        }

        private static void AssertChange(
            ObservableCollection<int> original, Action<ObservableCollection<int>> change, ICollection copy)
        {
            change(original);
            CollectionAssert.AreEqual(original, copy);
        }
    }
}
