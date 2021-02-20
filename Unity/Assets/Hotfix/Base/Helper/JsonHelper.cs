using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace ETHotfix
{
	public static class JsonHelper
	{
		public static string ToJson(object obj)
		{
			return JsonMapper.ToJson(obj);
		}

		public static T FromJson<T>(string str)
		{
			T t = JsonMapper.ToObject<T>(str);
			ISupportInitialize iSupportInitialize = t as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return t;
			}
			iSupportInitialize.EndInit();
			return t;
		}

		public static object FromJson(Type type, string str)
		{
			object t = JsonMapper.ToObject(type, str);
			ISupportInitialize iSupportInitialize = t as ISupportInitialize;
			if (iSupportInitialize == null)
			{
				return t;
			}
			iSupportInitialize.EndInit();
			return t;
		}

		public static T Clone<T>(T t)
		{
			return FromJson<T>(ToJson(t));
		}

        /*-----------------------------------------------------------------------*/
        public static T[] FromArrayJson<T>(string json)
        {
            ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(json);
            return wrapper.Items;
        }

        public static List<T> FromListJson<T>(string json)
        {
            ListWrapper<T> wrapper = JsonUtility.FromJson<ListWrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToArrayJson<T>(T[] array)
        {
            ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToArrayJson<T>(T[] array, bool prettyPrint)
        {
            ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        public static string ToListJson<T>(List<T> list)
        {
            ListWrapper<T> wrapper = new ListWrapper<T>();
            wrapper.Items = list;
            return JsonUtility.ToJson(wrapper);
        }

        public static string FixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        [Serializable]
        private class ArrayWrapper<T>
        {
            public T[] Items;
        }

        [Serializable]
        private class ListWrapper<T>
        {
            public List<T> Items;
        }
    }
}