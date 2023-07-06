#region copyright
/*
 * NuGet EmBER+ Consumer Lib
 *
 * Copyright (c) 2023 Roger Sandholm & Fredrik Bergholtz, Stockholm, Sweden
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmberPlusConsumerClassLib.Model;
using Lawo.EmberPlusSharp.Model;

namespace EmberPlusConsumerClassLib.EmberHelpers
{
    public static class NodeExtensions
    {
        /// <summary>
        /// Returns the child parameter <see cref="IParameter"/> of parent <paramref name="node"/> with <paramref name="identifier"/>.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <returns>IParameter or default if not found</returns>
        public static IParameter GetParameter(this INode node, string identifier, bool compareCaseSensitive = true)
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
            }
            if (compareCaseSensitive)
            {
                return node.Children.FirstOrDefault(c => c.Identifier == identifier) as IParameter;
            }
            return node.Children.FirstOrDefault(c => c.Identifier?.ToLower() == identifier?.ToLower()) as IParameter;
        }

        /// <summary>
        /// Returns the child parameter <see cref="IParameter"/> of parent <paramref name="node"/> with <paramref name="identifier"/>.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <param name="consumer"></param>
        /// <returns>IParameter or default if not found</returns>
        public static async Task<IParameter> GetParameter<TRoot>(this INode node, string identifier, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                await consumer.SendAsync();
            }
            return node.Children.FirstOrDefault(c => c.Identifier == identifier) as IParameter;
        }

        /// <summary>
        /// Returns the child function <see cref="IFunction"/> of parent <paramref name="node"/> with <paramref name="identifier"/>.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <returns></returns>
        public static IFunction GetFunction(this INode node, string identifier)
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
            }
            return node.Children.FirstOrDefault(c => c.Identifier == identifier) as IFunction;
        }

        /// <summary>
        /// Returns the child function <see cref="IFunction"/> of parent <paramref name="node"/> with <paramref name="identifier"/>.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public static async Task<IFunction> GetFunction<TRoot>(this INode node, string identifier, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                await consumer.SendAsync();
            }
            return node.Children.FirstOrDefault(c => c.Identifier == identifier) as IFunction;
        }

        /// <summary>
        /// Returns all child nodes <see cref="IEnumerable{INode}"/> of parent <paramref name="node"/>.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <returns>list of nodes <see cref="INode"/></returns>
        public static IEnumerable<INode> ChildNodes(this INode node)
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
            }
            return node.Children.OfType<INode>();
        }

        /// <summary>
        /// Returns all child nodes <see cref="IEnumerable{INode}"/> of parent <paramref name="node"/>
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="consumer"></param>
        /// <returns>list of nodes <see cref="INode"/></returns>
        public static async Task<IEnumerable<INode>> ChildNodes<TRoot>(this INode node, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                await consumer.SendAsync();
            }
            return node.Children.OfType<INode>();
        }

        /// <summary>
        /// Returns child parameter <see cref="IParameter"/> nodes of parent <paramref name="node"/>.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <returns>list of parameter nodes <see cref="IParameter"/></returns>
        public static IEnumerable<IParameter> ChildParameterNodes(this INode node)
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
            }
            return node.Children.OfType<IParameter>();
        }

        /// <summary>
        /// Returns child parameter <see cref="IParameter"/> nodes of parent <paramref name="node"/>.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="consumer"></param>
        /// <returns>list of parameter nodes <see cref="IParameter"/></returns>
        public static async Task<IEnumerable<IParameter>> ChildParameterNodes<TRoot>(this INode node, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                await consumer.SendAsync();
            }
            return node.Children.OfType<IParameter>();
        }

        /// <summary>
        /// Get child node <see cref="INode"/> of <paramref name="node"/> with path idenfifier <paramref name="string"/>.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <returns>node <see cref="INode"/></returns>
        public static INode GetChildNode(this INode node, string identifier)
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
            }
            return node.Children.OfType<INode>().FirstOrDefault(c => c.Identifier == identifier);
        }

        /// <summary>
        /// Get child node <see cref="INode"/> of <paramref name="node"/> with path idenfifier <paramref name="string"/>.
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="identifier">String path identifier</param>
        /// <param name="consumer"></param>
        /// <returns>node <see cref="INode"/></returns>
        public static async Task<INode> GetChildNode<TRoot>(this INode node, string identifier, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            if (node.Children.Count == 0)
            {
                node.ChildrenRetrievalPolicy = ChildrenRetrievalPolicy.DirectOnly;
                await consumer.SendAsync();
            }
            return node.Children.OfType<INode>().FirstOrDefault(c => c.Identifier == identifier);
        }

        /// <summary>
        /// Navigate from <paramref name="root"/> node to the <paramref name="path"/> node <see cref="INode"/> , the path is split by '/'.
        /// TODO: This one doesn't use the consumer.SendAsync() to trigger changes on no children, needs to be verified.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="path">Path as string</param>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public static INode NavigateToNode(this INode root, string path)
        {
            string[] steps = path.Split('/');

            INode node = root;
            foreach (string identifier in steps)
            {
                node = node.GetChildNode(identifier);
                if (node == null)
                {
                    break;
                }
            }
            return node;
        }

        /// <summary>
        /// Navigate from <paramref name="root"/> node to the <paramref name="path"/> node <see cref="INode"/> , the path is split by '/'.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="path">Path as string</param>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public static async Task<INode> NavigateToNode<TRoot>(this INode root, string path, Consumer<TRoot> consumer) where TRoot : Root<TRoot>
        {
            string[] steps = path.Split('/');

            INode node = root;
            foreach (string identifier in steps)
            {
                node = await node.GetChildNode(identifier, consumer);
                if (node == null)
                {
                    break;
                }
            }
            return node;
        }
    }
}