// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Collections;

namespace MS.Internal
{
    /// <summary>
    ///     Helper WeakDictionary class implemented using WeakHashTable
    /// </summary>
    internal class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : class
    {
        #region KeyCollection

        private class KeyCollection<KeyType, ValueType> : ICollection<KeyType> where KeyType : class
        {
            #region Constructor And Properties

            public KeyCollection(WeakDictionary<KeyType, ValueType> dict)
            {
                Dict = dict;
            }

            public WeakDictionary<KeyType, ValueType> Dict
            {
                get;
                private set;
            }

            #endregion

            #region ICollection<KeyType> Members

            public void Add(KeyType item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyType item)
            {
                return Dict.ContainsKey(item);
            }

            public void CopyTo(KeyType[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return Dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(KeyType item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<KeyType> Members

            public IEnumerator<KeyType> GetEnumerator()
            {
                IWeakHashtable hashTable = Dict._hashTable;
                foreach (object obj in hashTable.Keys)
                {
                    KeyType key = hashTable.UnwrapKey(obj) as KeyType;
                    if (key != null)
                    {
                        yield return key;
                    }
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ValueCollection

        private class ValueCollection<KeyType, ValueType> : ICollection<ValueType> where KeyType : class
        {
            #region Constructor And Properties

            public ValueCollection(WeakDictionary<KeyType, ValueType> dict)
            {
                Dict = dict;
            }

            public WeakDictionary<KeyType, ValueType> Dict
            {
                get;
                private set;
            }

            #endregion

            #region ICollection<TValue> Members

            public void Add(ValueType item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ValueType item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ValueType[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return Dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(ValueType item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<TValue> Members

            public IEnumerator<ValueType> GetEnumerator()
            {
                IWeakHashtable hashTable = Dict._hashTable;
                foreach (object obj in hashTable.Keys)
                {
                    KeyType key = hashTable.UnwrapKey(obj) as KeyType;
                    if (key != null)
                    {
                        yield return (ValueType)hashTable[obj];
                    }
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            _hashTable.SetWeak(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _hashTable.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_keys == null)
                {
                    _keys = new WeakDictionary<TKey, TValue>.KeyCollection<TKey, TValue>(this);
                }
                return _keys;
            }
        }

        public bool Remove(TKey key)
        {
            if (_hashTable.ContainsKey(key))
            {
                _hashTable.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_hashTable.ContainsKey(key))
            {
                value = (TValue)_hashTable[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new WeakDictionary<TKey, TValue>.ValueCollection<TKey, TValue>(this);
                }
                return _values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!_hashTable.ContainsKey(key))
                {
                    throw new KeyNotFoundException();
                }
                return (TValue)_hashTable[key];
            }
            set
            {
                _hashTable.SetWeak(key, value);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _hashTable.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!_hashTable.ContainsKey(item.Key))
            {
                return false;
            }

            if (object.Equals(_hashTable[item.Key], item.Value))
            {
                return true;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
            ArgumentNullException.ThrowIfNull(array);

            int count = 0;
            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                count++;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length - count);

            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public int Count
        {
            get { return _hashTable.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return this.Remove(item.Key);
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (object obj in _hashTable.Keys)
            {
                TKey key = _hashTable.UnwrapKey(obj) as TKey;
                if (key != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(key, (TValue)_hashTable[obj]);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Private Data

        private IWeakHashtable _hashTable = WeakHashtable.FromKeyType(typeof(TKey));
        private KeyCollection<TKey, TValue> _keys = null;
        private ValueCollection<TKey, TValue> _values = null;

        #endregion
    }
}
