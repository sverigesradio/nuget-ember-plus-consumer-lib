#region copyright
/*
 * NuGet EmBER+ Consumer Lib
 *
 * Copyright (c) 2023 Roger Sandholm, Stockholm, Sweden
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion copyright

using EmberPlusConsumerClassLib.Model;
using Lawo.EmberPlusSharp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmberPlusConsumerClassLib.EmberHelpers
{
    //public static class NodeIdentityExtension
    //{

    //    public class SupportedProducts
    //    {
    //        private enum ProductFamily
    //        {
    //            Sapphire,
    //            SapphireCompact,
    //            Ruby,
    //            R3LAYVirtualPatchBay,
    //            Dhd
    //        }

    //        public SupportedProducts(string[] products)
    //        {
    //            if (products == null || products.Length == 0)
    //            {
    //                Supported = Enum.GetNames(typeof(ProductFamily));
    //            }
    //            else
    //            {
    //                Supported = products;
    //            }
    //        }

    //        public string[] Supported { get; private set; }
    //    }

    //    public class ProductIdentity
    //    {
    //        public SupportedProducts ProductFamily { get; set; }
    //        public string Company { get; set; }
    //        public string Product { get; set; }
    //    }

    //    private static ProductIdentity ParseProductIdentity<TRoot>(this INode root, string[] products = null)
    //    {
    //        var productFamilies = new SupportedProducts(products);
    //        var familyNode = (INode)root.Children.FirstOrDefault(c => productFamilies.Supported.Contains(c.Identifier));

    //        if (familyNode == null || !Enum.TryParse(familyNode.Identifier, out ProductFamily productFamily))
    //        {
    //            return null;
    //        }

    //        var identity = familyNode.GetChildNode("identity");
    //        if (identity == null)
    //        {
    //            identity = familyNode.GetChildNode("Identity");
    //        }

    //        if (identity == null)
    //        {
    //            return null;
    //        }

    //        var company = identity.GetParameter("company", false);
    //        if (company == null)
    //        {
    //            return null;
    //        }

    //        var product = identity.GetParameter("product", false);
    //        if (product == null)
    //        {
    //            return null;
    //        }

    //        return product != null
    //            ? new ProductIdentity
    //            {
    //                ProductFamily = productFamily,
    //                Company = company.Value.ToString(),
    //                Product = product.Value.ToString()
    //            }
    //            : null;
    //    }
    //}
}
