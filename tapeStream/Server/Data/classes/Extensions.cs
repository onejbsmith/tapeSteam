using Newtonsoft.Json;
using System;
using System.Diagnostics;

public static class DictionaryExtensions
{
    //public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic,
    //    Func<TKey, TValue, bool> predicate)
    //{
    //    var keys = dic.Keys.Where(k => predicate(k, dic[k])).ToList();
    //    foreach (var key in keys)
    //    {
    //        dic.Remove(key);
    //    }
    //}
}
public static class ObjectExtensions
{
    public static void Dump<T>(this T data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        Console.WriteLine(json);
        Debug.WriteLine(json);
    }

}