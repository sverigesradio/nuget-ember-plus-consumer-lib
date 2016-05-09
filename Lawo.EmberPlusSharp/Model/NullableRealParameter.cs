﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2016 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.EmberPlusSharp.Model
{
    using System.Diagnostics.CodeAnalysis;

    using Ember;
    using Glow;

    /// <summary>Represents a nullable real parameter in the object tree accessible through
    /// <see cref="Consumer{T}.Root">Consumer&lt;TRoot&gt;.Root</see>.</summary>
    /// <threadsafety static="true" instance="false"/>
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Fewer levels of inheritance would lead to more code duplication.")]
    public sealed class NullableRealParameter : NullableNumericParameter<NullableRealParameter, double>
    {
        internal sealed override double? ReadValue(EmberReader reader, out ParameterType? parameterType)
        {
            parameterType = ParameterType.Real;
            return reader.AssertAndReadContentsAsDouble();
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Method is not public, CA bug?")]
        internal sealed override void WriteValue(EmberWriter writer, double? value) =>
            writer.WriteValue(GlowParameterContents.Value.OuterId, value.GetValueOrDefault());

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private NullableRealParameter()
        {
        }
    }
}
