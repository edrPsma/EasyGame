using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtra
{
    public static T2 TryGet<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
    {
        T2 result = default(T2);
        dic.TryGetValue(key, out result);
        return result;
    }
}
