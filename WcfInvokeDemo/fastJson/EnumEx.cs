using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace fastJSON
{
    public static class EnumEx
    {
        private static IDictionary<Type, Hashtable> _enumBuffer = new Dictionary<Type, Hashtable>();

        public static IEnumerable<EnumItem<T>> GetItems<T>() where T : struct
        { 
            return GetFields<T>().Select(a => new EnumItem<T>(a.Key, a.Value));
        }

        public static IEnumerable<KeyValuePair<T, FieldInfo>> GetFields<T>() where T : struct
        {
            Hashtable hash = GetBuffer(typeof(T));
            if (hash == null) yield break;

            foreach (DictionaryEntry item in hash)
            {
                yield return new KeyValuePair<T, FieldInfo>((T)item.Key, (FieldInfo)item.Value);
            }
        }

        public static IEnumerable<FieldInfo> GetFields(Type enumType)
        {
            Hashtable hash = GetBuffer(enumType);
            if (hash == null) yield break;

            foreach (DictionaryEntry item in hash)
            {
                yield return (FieldInfo)item.Value;
            }
        }

        public static Hashtable GetBuffer(Type enumType)
        {
            if (!typeof(Enum).IsAssignableFrom(enumType))
                return null;

            Hashtable hash = null;

            if (!_enumBuffer.TryGetValue(enumType, out hash))
            {
                hash = new Hashtable();
                foreach (var item in enumType.GetFields().Where(a => a.IsStatic))
                {
                    hash[item.GetValue(null)] = item;
                }
                _enumBuffer[enumType] = hash;
            }

            return hash;
        }
    }


    /// <summary>
    /// 枚举项
    /// </summary>
    public class EnumItem
    {
        public EnumItem(FieldInfo field)
        {
            //this.Description = field.GetDescription();
            this.Name = field.Name;
            this.Value = field.GetValue(null); 
            this.IntValue = this.Value.Convert<int>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public object Value { get; set; }

        public int IntValue { get; set; }

        public override string ToString()
        {
            return this.Description.EmptyAsNull() ?? this.Name;
        }
    }


    public class EnumItem<T> : EnumItem where T : struct
    {
        public EnumItem(T t, FieldInfo field)
            : base(field)
        {
            _item = t;
        }

        private T _item;

        public T Item
        {
            get { return _item; }
            set { _item = value; }
        }

    }
}
