
using fastJSON.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace fastJSON
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicJson : DynamicObject, IEnumerable, IEntityWithKey
    {
        public static DynamicJson Default()
        {
            return "{}".AsDynamicJson();
        }

        private IDictionary<string, object> _dictionary { get; set; }
        public IDictionary<string, object> Dictionary { get; set; }
        private IDictionary<string, object> _upperKeyDictionary
        {
            get
            {
                if (_dictionary == null) return null;
                return _dictionary.Select(a => new { Key = a.Key.ToUpper(), a.Value }).ToDictionary(a => a.Key, a => a.Value);
            }
        }
        public IDictionary<string, object> GetKeyValues()
        {
            return _dictionary;
        }

        public bool IsEmpty
        {
            get { return !(_list != null || _dictionary != null); }
        }

        private List<object> _list { get; set; }
        public List<object> List { get { return _list; } }

        public DynamicJson(string json)
        {
            var parse = fastJSON.JSON.Parse(json);

            if (parse is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)parse;
            else
                _list = (List<object>)parse;
        }

        private DynamicJson(object dictionary)
        {
            if (dictionary is IDictionary<string, object>)
                _dictionary = (IDictionary<string, object>)dictionary;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys.ToList();
        }

        public override bool TryGetIndex(GetIndexBinder binder, Object[] indexes, out Object result)
        {
            var index = indexes[0];
            if (index is int)
            {
                result = _list[(int)index];
            }
            else
            {
                result = _dictionary[(string)index];
            }
            if (result is IDictionary<string, object>)
                result = new DynamicJson(result as IDictionary<string, object>);
            return true;
        }

        public object this[string propertyName]
        {
            get
            {
                object r = Get(propertyName);
                if (r is IDictionary<string, object>)
                    r = new DynamicJson(r as IDictionary<string, object>);
                return r;
            }
            set
            {
                Set(propertyName, value);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                if (_dictionary.TryGetValue(binder.Name, out result) == false)
                    if (_dictionary.TryGetValue(binder.Name.ToLower(), out result) == false)
                        if (_upperKeyDictionary.TryGetValue(binder.Name.ToUpper(), out result) == false)
                            throw new Exception("找不到属性: " + binder.Name);

                if (result is IDictionary<string, object>)
                {
                    result = new DynamicJson(result as IDictionary<string, object>);
                }
                else if (result is List<object>)
                {
                    List<object> list = new List<object>();
                    foreach (object item in (List<object>)result)
                    {
                        if (item is IDictionary<string, object>)
                            list.Add(new DynamicJson(item as IDictionary<string, object>));
                        else
                            list.Add(item);
                    }
                    result = list;
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetChild<T>(string propertyName)
            where T : class
        {
            object result = default(T);
            try
            {
                if (_dictionary.TryGetValue(propertyName, out result) == false)
                    if (_dictionary.TryGetValue(propertyName.ToLower(), out result) == false)
                        if (_upperKeyDictionary.TryGetValue(propertyName.ToUpper(), out result) == false)
                            throw new Exception("找不到属性: " + propertyName);

                if (result is IDictionary<string, object>)
                {
                    result = new DynamicJson(result as IDictionary<string, object>);
                }
                else if (result is List<object>)
                {
                    List<object> list = new List<object>();
                    foreach (object item in (List<object>)result)
                    {
                        if (item is IDictionary<string, object>)
                            list.Add(new DynamicJson(item as IDictionary<string, object>));
                        else
                            list.Add(item);
                    }
                    result = list;
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result as T;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_list == null) yield return null;
            else
            {
                foreach (var o in _list)
                {
                    yield return new DynamicJson(o as IDictionary<string, object>);
                }
            }
        }


        public ICollection<string> Keys
        {
            get
            {
                if (_dictionary == null) return new List<string>();
                return _dictionary.Keys;
            }
        }

        public bool ContainsKey(string propertyName)
        {
            if (_dictionary != null && _dictionary.ContainsKey(propertyName))
                return true;
            else if (_upperKeyDictionary != null && _upperKeyDictionary.ContainsKey(propertyName.ToUpper()))
                return true;
            return false;
        }

        public T Get<T>(string propertyName)
        {
            var r = Get(propertyName);
            if ((r == null || r.ToString() == string.Empty) && typeof(T) == typeof(int))
                r = "-1";
            return r.Convert<T>();
        }

        public string GetStr(string propertyName)
        {
            return Get(propertyName).ToStringEx();
        }

        public object Get(string propertyName)
        {
            if (_dictionary != null)
            {
                if (_dictionary.ContainsKey(propertyName))
                    return _dictionary[propertyName];
                if (_upperKeyDictionary.ContainsKey(propertyName.ToUpper()))
                    return _upperKeyDictionary[propertyName.ToUpper()];
            }
            return string.Empty;
        }

        public DynamicJson Set(string propertyName, object value)
        {
            try
            {
                if (value is DynamicJson)
                {
                    var dj = value as DynamicJson;
                    if (dj.IsList)
                        value = dj.List;
                    else
                        value = dj.GetKeyValues();
                }
                if (_dictionary != null)
                {
                    propertyName = _dictionary.Keys.FirstOrDefault(a => a.ToUpper() == propertyName.ToUpper()) ?? propertyName;
                    _dictionary[propertyName] = value;
                }
            }
            catch (Exception)
            {
                return this;
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsList { get { return _list != null && _list.Count > 0; } }

        /// <summary>
        /// 
        /// </summary>
        public int PropertyCount { get { if (_dictionary != null) return this._dictionary.Count; else return -1; } }

        public EntityKey EntityKey { get; set; }

        public override string ToString()
        {
            if (_list != null && _list.Count() > 0) return _list.ToJson();
            if (_dictionary != null && _dictionary.Count() > 0) return _dictionary.ToJson();
            return base.ToString();
        }
    }
}