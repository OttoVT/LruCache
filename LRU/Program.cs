using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LRU
{
    class Program
    {
        static void Main(string[] args)
        {
            var lruCache = new LruCache<string, string>(3, x => x);
            lruCache.AddItem("1");
            lruCache.AddItem("2");
            lruCache.AddItem("3");
            PrintLruCache(lruCache);
            lruCache.AddItem("1");
            PrintLruCache(lruCache);
            lruCache.AddItem("4");
            PrintLruCache(lruCache);

            Console.ReadLine();
        }

        static void PrintLruCache(LruCache<string, string> cache)
        {
            Console.WriteLine();
            foreach (var cacheItem in cache)
            {
                Console.WriteLine(cacheItem);
            }
            Console.WriteLine();
        }
    }

    public class LruCache<TKey, TNodeValue> : IEnumerable<TNodeValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<TNodeValue>> _dictionary = new Dictionary<TKey, LinkedListNode<TNodeValue>>();
        private readonly LinkedList<TNodeValue> _linkedList = new LinkedList<TNodeValue>();
        private readonly int _capacity;
        private readonly Func<TNodeValue, TKey> _keyExtractor;

        public LruCache(int capacity, Func<TNodeValue, TKey> keyExtractor)
        {
            _keyExtractor = keyExtractor;
            _capacity = capacity;
        }

        public bool TryGetItemByKey(TKey key, out TNodeValue value)
        {
            if (_dictionary.TryGetValue(key, out var result))
            {
                value = result.Value;

                return true;
            }

            value = default(TNodeValue);

            return false;
        }

        public void AddItem(TNodeValue item)
        {
            var key = _keyExtractor(item);

            if (!_dictionary.ContainsKey(key))
            {
                if (_linkedList.Count == _capacity)
                {
                    var lastDictKey = _keyExtractor(_linkedList.Last.Value);
                    _linkedList.RemoveLast();
                    _dictionary.Remove(lastDictKey);
                }

                var newNode = _linkedList.AddFirst(item);
                _dictionary.Add(key, newNode);
            }
            else
            {
                var node = _dictionary[key];
                node.Value = item;

                _linkedList.Remove(node);
                _linkedList.AddFirst(node);
            }
        }

        public IEnumerator<TNodeValue> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
