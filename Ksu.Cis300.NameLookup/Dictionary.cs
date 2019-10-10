/* Dictionary.cs
 * Author: Rod Howell
 * Modified by: Lauren Grieb
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;

        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// returns the given tree with the node at the smallest key removed
        /// </summary>
        /// <param name="t">the given tree</param>
        /// <param name="min">the pair removed</param>
        /// <returns>the given tree at the smallest key</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMininumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t,
            out KeyValuePair<TKey, TValue> min) {
            if(t.LeftChild == null) {
                min = t.Data;
                return t.RightChild;
            }
            else {
                BinaryTreeNode<KeyValuePair<TKey, TValue>> recursiveTree = RemoveMininumKey(t.LeftChild, out min);
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, recursiveTree, t.RightChild);
            }
        }

        /// <summary>
        /// removes the node for the given tree at the given key and the out parameter
        /// is if the key is found or not
        /// </summary>
        /// <param name="key">the given key</param>
        /// <param name="t">the given tree</param>
        /// <param name="removed">if the key is found or not</param>
        /// <returns>the tree without the node at the given key</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t,
            out bool removed) {
            
            if (t == null) {
                removed = false;
                return null;
            }
            else if (t.Data.Key.CompareTo(key) > 0) {
                BinaryTreeNode<KeyValuePair<TKey, TValue>> recursiveTree = Remove(key, t.LeftChild, out removed);
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, recursiveTree, t.RightChild);
            }
            else if (t.Data.Key.CompareTo(key) < 0) {
                BinaryTreeNode<KeyValuePair<TKey, TValue>> recursiveTree = Remove(key, t.RightChild, out removed);
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, recursiveTree);
            } //left and right are just swapped from above
            else {
                removed = true;
                if (t.LeftChild == null && t.RightChild == null) {
                    return null;
                }
                else if (t.RightChild == null && t.LeftChild != null) {
                    return t.LeftChild;
                }
                else if (t.LeftChild == null && t.RightChild != null) {
                    return t.RightChild;
                }
                else {
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> withoutMinKey = RemoveMininumKey(t.RightChild, out KeyValuePair<TKey, TValue> min);
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, t.LeftChild, withoutMinKey);
                }
            }
            
        }

        /// <summary>
        /// removes the given key from _elements
        /// </summary>
        /// <param name="k">the key to be removed</param>
        /// <returns>if the key was found or not</returns>
        public bool Remove(TKey k) {
            CheckKey(k);
            _elements = Remove(k, _elements, out bool answer);
            return answer;
        }

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }
    }
}
