﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// <copyright>Copyright 2012-2015 Lawo AG (http://www.lawo.com).</copyright>
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Lawo.EmberPlus.Model.Test
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Instantiated through reflection.")]
    internal sealed class PropertiesRoot : Root<PropertiesRoot>
    {
        internal IntegerParameter FactorIntegerParameter { get; private set; }

        internal IntegerParameter FormulaIntegerParameter { get; private set; }

        internal IntegerParameter EnumerationParameter { get; private set; }

        internal IntegerParameter EnumMapParameter { get; private set; }

        internal RecursiveFieldNode FieldNode { get; private set; }
    }
}
