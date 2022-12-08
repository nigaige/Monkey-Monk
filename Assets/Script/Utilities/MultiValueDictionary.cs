using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Utilities
{
    public class MultiValueDictionary<Key, Value> : Dictionary<Key, List<Value>>
    {

        public void Add(Key key, Value value)
        {
            if (!base.TryGetValue(key, out List<Value> list)) base.Add(key, new List<Value>());

            base[key].Add(value);
        }

    }
}