﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2015 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.EmberPlus.Ember
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;

    using BerLib;
    using Lawo.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>Tests compatibility with EmberLib.net.</summary>
    [TestClass]
    public class CompatibilityTest : TestBase
    {
        /// <summary>Tests compatibility with Boolean contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void BooleanTest()
        {
            var value = this.Random.Next(0, 2) == 1;
            AssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetBoolean(), value);
            AssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsBoolean(), value);
        }

        /// <summary>Tests compatibility with Integer contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void IntegerTest()
        {
            var longBytes = new byte[Marshal.SizeOf(typeof(long))];
            this.Random.NextBytes(longBytes);
            var value = BitConverter.ToInt64(longBytes, 0);

            AssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetLong(), value);
            AssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsInt64(), value);
        }

        /// <summary>Tests compatibility with Octetstring contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void OctetstringTest()
        {
            var value = new byte[1024];
            this.Random.NextBytes(value);
            CollectionAssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetOctetString(), value);
            CollectionAssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsByteArray(), value);
        }

        /// <summary>Tests compatibility with Real contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void RealTest()
        {
            var value = (this.Random.NextDouble() - 0.5) * this.Random.Next(int.MaxValue);
            AssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetReal(), value);
            AssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsDouble(), value);
        }

        /// <summary>Exposes the real decoding bug in EmberLib.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void EmberLibBugTest()
        {
            var value = -83981925.8237834;
            AssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsDouble(), value);
            AssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetReal(), value);
        }

        /// <summary>Tests compatibility with UTF8String contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void Utf8StringTest()
        {
            var value = Guid.NewGuid().ToString();
            AssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetString(), value);
            AssertEqual((w, t, v) => w.Write(t, v), r => r.ReadContentsAsString(), value);
        }

        /// <summary>Tests compatibility with Relative object identifier contents.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        public void RelativeObjectIdentifierTest()
        {
            var value = new int[this.Random.Next(0, 16)];

            for (int index = 0; index < value.Length; ++index)
            {
                value[index] = this.Random.Next();
            }

            CollectionAssertEqual((w, i, v) => w.WriteValue(i, v), r => r.GetRelativeOid(), value);
            CollectionAssertEqual((w, t, v) => w.WriteRelativeOid(t, v), r => r.ReadContentsAsInt32Array(), value);
        }

        /// <summary>Tests compatibility with Sequence and Set.</summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test code.")]
        [TestCategory("Unattended")]
        [TestMethod]
        public void ContainerTest()
        {
            AssertEqual((w, i, d) => w.WriteStartSequence(i), r => r.IsContainer, true);
            AssertEqual((w, i, d) => w.WriteStartSet(i), r => r.IsContainer, true);
            AssertEqual(
                (w, i, d) => w.WriteStartApplicationDefinedType(i, InnerNumber.FirstApplication),
                r => r.IsContainer,
                true);

            AssertEqual((w, t, d) => w.WriteSequenceBegin(t), r => r.InnerNumber == InnerNumber.Sequence, true);
            AssertEqual((w, t, d) => w.WriteSetBegin(t), r => r.InnerNumber == InnerNumber.Set, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void AssertEqual<T>(Action<EmberWriter, EmberId, T> write, Func<EmberLib.EmberReader, T> read, T value)
        {
            AssertEqual(write, read, value, Assert.AreEqual);
        }

        private void CollectionAssertEqual<T>(
            Action<EmberWriter, EmberId, T> write, Func<EmberLib.EmberReader, T> read, T value) where T : ICollection
        {
            AssertEqual(write, read, value, (l, r) => CollectionAssert.AreEqual(l, r));
        }

        private void AssertEqual<T>(Action<EmberLib.EmberWriter, BerTag, T> write, Func<EmberReader, T> read, T value)
        {
            AssertEqual(write, read, value, Assert.AreEqual);
        }

        private void CollectionAssertEqual<T>(
            Action<EmberLib.EmberWriter, BerTag, T> write, Func<EmberReader, T> read, T value) where T : ICollection
        {
            AssertEqual(write, read, value, (l, r) => CollectionAssert.AreEqual(l, r));
        }

        private void AssertEqual<T>(
            Action<EmberWriter, EmberId, T> write, Func<EmberLib.EmberReader, T> read, T value, Action<T, T> assertEqual)
        {
            var number = this.Random.Next();
            var outer = EmberId.CreateApplication(number);
            var tag = new BerTag(BerClass.Application, (uint)number);

            MemoryStream output;

            using (output = new MemoryStream())
            using (var writer = new EmberWriter(output, 1))
            {
                write(writer, outer, value);
            }

            var input = new BerMemoryInput(output.ToArray());
            var reader = new EmberLib.EmberReader(input);
            Assert.IsTrue(reader.Read());
            Assert.AreEqual(tag, reader.Tag);
            assertEqual(value, read(reader));
        }

        private void AssertEqual<T>(
            Action<EmberLib.EmberWriter, BerTag, T> write, Func<EmberReader, T> read, T value, Action<T, T> assertEqual)
        {
            var number = this.Random.Next();
            var outerId = EmberId.CreateApplication(number);
            var tag = new BerTag(BerClass.Application, (uint)number);

            var output = new BerMemoryOutput();
            var writer = new EmberLib.EmberWriter(output);
            write(writer, tag, value);

            using (var input = new MemoryStream(output.ToArray()))
            using (var reader = new EmberReader(input, 1))
            {
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(outerId, reader.OuterId);
                assertEqual(value, read(reader));
            }
        }
    }
}
