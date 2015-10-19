﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2015 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.EmberPlus.Ember
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Lawo.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>Tests <see cref="EmberWriter"/>.</summary>
    [TestClass]
    public class EmberWriterTest : TestBase
    {
        /// <summary>Tests the main use cases of <see cref="EmberWriter"/>.</summary>
        [TestCategory("Unattended")]
        [TestMethod]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test code.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "In this case readability is improved.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "In this case readability is improved.")]
        public void MainTest()
        {
            AssertEncode(
                writer => writer.WriteValue(EmberId.CreateApplication(0), true),
                0x60, 0x03, 0x01, 0x01, 0xFF);

            AssertEncode(
                writer => writer.WriteValue(EmberId.CreateContextSpecific(1), 0x80),
                0xA1, 0x04, 0x02, 0x02, 0x00, 0x80);

            AssertEncode(
                writer => writer.WriteValue(EmberId.CreateApplication(2), new byte[] { 0x42 }),
                0x62, 0x03, 0x04, 0x01, 0x42);

            AssertEncode(writer => writer.WriteValue(EmberId.CreateContextSpecific(3), 0.0), 0xA3, 0x02, 0x09, 0x00);
            AssertEncode(writer => writer.WriteValue(EmberId.CreateContextSpecific(3), double.PositiveInfinity), 0xA3, 0x03, 0x09, 0x01, 0x40);
            AssertEncode(writer => writer.WriteValue(EmberId.CreateContextSpecific(3), double.NegativeInfinity), 0xA3, 0x03, 0x09, 0x01, 0x41);
            AssertEncode(writer => writer.WriteValue(EmberId.CreateContextSpecific(3), -1.0), 0xA3, 0x05, 0x09, 0x03, 0xC0, 0x00, 0x01);

            AssertEncode(
                writer => writer.WriteValue(EmberId.CreateApplication(4), "A"),
                0x64, 0x03, 0x0C, 0x01, 0x41);

            AssertEncode(
                writer => writer.WriteValue(EmberId.CreateContextSpecific(5), new[] { 15 }),
                0xA5, 0x03, 0x0D, 0x01, 0x0F);

            AssertEncode(
                writer => writer.WriteStartSequence(EmberId.CreateApplication(6)),
                0x66, 0x80, 0x30, 0x80);

            AssertEncode(
                writer => writer.WriteStartSet(EmberId.CreateContextSpecific(7)),
                0xA7, 0x80, 0x31, 0x80);

            AssertEncode(
                writer => writer.WriteStartApplicationDefinedType(
                    EmberId.CreateContextSpecific(7), InnerNumber.FirstApplication),
                0xA7, 0x80, 0x60, 0x80);

            AssertEncode(
                writer => writer.WriteEndContainer(),
                0x00, 0x00, 0x00, 0x00);
        }

        /// <summary>Tests <see cref="EmberWriter"/> exceptions.</summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The stream is disposed through the writer.")]
        [TestCategory("Unattended")]
        [TestMethod]
        public void ExceptionTest()
        {
            AssertThrow<ArgumentNullException>(() => new EmberWriter(null, 1).Dispose());
            AssertThrow<ArgumentException>(() => new EmberWriter(new MemoryStream(), 0).Dispose());

            using (var writer = new EmberWriter(new MemoryStream(), 1))
            {
                var outer = EmberId.CreateApplication(0);

                AssertThrow<ArgumentNullException>(
                    () => writer.WriteValue(outer, (byte[])null),
                    () => writer.WriteValue(outer, (int[])null),
                    () => writer.WriteValue(outer, (string)null));

                AssertThrow<ArgumentOutOfRangeException>(
                    () => writer.WriteStartApplicationDefinedType(outer, InnerNumber.FirstApplication - 1));

                writer.Dispose();
                AssertThrow<ObjectDisposedException>(
                    () => writer.WriteValue(outer, true),
                    () => writer.WriteValue(outer, 0),
                    () => writer.WriteValue(outer, new byte[] { }),
                    () => writer.WriteValue(outer, 0.0),
                    () => writer.WriteValue(outer, string.Empty),
                    () => writer.WriteValue(outer, new int[] { }),
                    () => writer.WriteStartSequence(outer),
                    () => writer.WriteStartSet(outer),
                    () => writer.WriteStartApplicationDefinedType(outer, InnerNumber.FirstApplication),
                    () => writer.WriteEndContainer());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void AssertEncode(Action<EmberWriter> write, params byte[] expected)
        {
            MemoryStream stream;

            using (stream = new MemoryStream())
            using (var writer = new EmberWriter(stream, 1))
            {
                write(writer);
            }

            CollectionAssert.AreEqual(expected, stream.ToArray());
        }
    }
}
