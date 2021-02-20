//using UnityEngine;
//using System.Collections.Generic;

//public static class JsonHelper
//{
//    public static T[] FromArrayJson<T>(string json)
//    {
//        ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(json);
//        return wrapper.Items;
//    }

//    public static List<T> FromListJson<T>(string json)
//    {
//        ListWrapper<T> wrapper = JsonUtility.FromJson<ListWrapper<T>>(json);
//        return wrapper.Items;
//    }

//    public static string ToArrayJson<T>(T[] array)
//    {
//        ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
//        wrapper.Items = array;
//        return JsonUtility.ToJson(wrapper);
//    }

//    public static string ToArrayJson<T>(T[] array, bool prettyPrint)
//    {
//        ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
//        wrapper.Items = array;
//        return JsonUtility.ToJson(wrapper, prettyPrint);
//    }

//    public static string ToListJson<T>(List<T> list)
//    {
//        ListWrapper<T> wrapper = new ListWrapper<T>();
//        wrapper.Items = list;
//        return JsonUtility.ToJson(wrapper);
//    }

//    public static string FixJson(string value)
//    {
//        value = "{\"Items\":" + value + "}";
//        return value;
//    }

//    [System.Serializable]
//    private class ArrayWrapper<T>
//    {
//        public T[] Items;
//    }

//    [System.Serializable]
//    private class ListWrapper<T>
//    {
//        public List<T> Items;
//    }
//}