﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2015 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.EmberPlus.Glow
{
    using System.Diagnostics.CodeAnalysis;

    using Ember;

    internal static class GlowStreamEntry
    {
        internal const int InnerNumber = Ember.InnerNumber.FirstApplication + 5;
        internal const string Name = "StreamEntry";

        internal static class StreamIdentifier
        {
            internal const int OuterNumber = 0;
            internal const string Name = "streamIdentifier";

            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used through reflection.")]
            internal static readonly EmberId OuterId = EmberId.CreateContextSpecific(OuterNumber);
        }

        internal static class StreamValue
        {
            internal const int OuterNumber = 1;
            internal const string Name = "streamValue";

            [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used through reflection.")]
            internal static readonly EmberId OuterId = EmberId.CreateContextSpecific(OuterNumber);
        }
    }
}
