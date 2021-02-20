using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace fastJSON
{
    /// <summary>
    /// 基础类型转换器
    /// </summary>
    public static class TPConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Convert<T>(this object value)
        {
            T result = default(T);
            try
            {
                if (value == null) return result;
                Type cType = value.GetType();
                Type tType = typeof(T);

                TypeConverter valueC = TypeDescriptor.GetConverter(value);
                TypeConverter valueT = TypeDescriptor.GetConverter(tType);

                if (valueC.CanConvertTo(tType))
                    return (T)valueC.ConvertTo(value, tType);
                if (tType == typeof(Boolean))
                {
                    if (value is string && ((string)value).IsNumeric())
                    {
                        double dbl = value.Convert<double>();
                        value = System.Convert.ToBoolean(dbl).ToString();
                    }
                    else
                        value = System.Convert.ToBoolean(value).ToString();
                }
                if (valueT.CanConvertFrom(cType))
                    return (T)valueT.ConvertFrom(value);

                MethodInfo mi = null;
                if (!tType.IsGenericType)
                    mi = typeof(System.Convert).GetMethod(string.Format("To{0}", tType.Name), new Type[] { typeof(object) });
                if (mi != null) return (T)mi.Invoke(null, new object[] { value });
            }
            catch (Exception)
            {
                
            }
            return result;
        }

        public static object Convert(this object value, Type tType)
        {
            object result = null;
            try
            {
                Type cType = value.GetType();

                TypeConverter valueC = TypeDescriptor.GetConverter(value);
                TypeConverter valueT = TypeDescriptor.GetConverter(tType);

                if (valueC.CanConvertTo(tType))
                    result = valueC.ConvertTo(value, tType);
                if (valueT.CanConvertFrom(cType))
                    result = valueT.ConvertFrom(value);

                MethodInfo mi = null;
                if (!tType.IsGenericType)
                    mi = typeof(System.Convert).GetMethod(string.Format("To{0}", tType.Name), new Type[] { tType });
                if (mi != null) result = mi.Invoke(null, new object[] { value });
            }
            catch (Exception)
            {
                
            }
            return result;
        }
        /// <summary>
        /// 根据属性类型，将读得的文本内容转换为对应类型
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static object ConvertByPropertyInfo(this string text, PropertyInfo pi)
        {
            object lastValue = null;

            try
            {
                var typeName = pi.PropertyType.Name;
                TypeCode typeCode = EnumEx.GetItems<TypeCode>().Where(a => a.Name == typeName).FirstOrDefault().Item;
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        lastValue = Convert<bool>(text);
                        break;
                    case TypeCode.DateTime:
                        lastValue = Convert<DateTime>(text);
                        break;
                    case TypeCode.Decimal:
                        lastValue = Convert<decimal>(text);
                        break;
                    case TypeCode.Double:
                        lastValue = Convert<double>(text);
                        break;
                    case TypeCode.Int32:
                        lastValue = Convert<int>(text);
                        break;
                    case TypeCode.String:
                        lastValue = text;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                
            }
            return lastValue;
        }

        #region 泛型数据类型转换

        /// <summary>
        /// 泛型数据类型转换
        /// </summary>
        /// <typeparam name="T">自定义数据类型</typeparam>
        /// <param name="value">传入需要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T ConvertType<T>(object value, T defaultValue)
        {
            return (T)ConvertToT<T>(value, defaultValue);
        }
        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="T">自定义数据类型</typeparam>
        /// <param name="myvalue">传入需要转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        private static object ConvertToT<T>(object myvalue, T defaultValue)
        {
            TypeCode typeCode = System.Type.GetTypeCode(typeof(T));
            if (myvalue == null)
                return defaultValue;
            string value = myvalue.ToString();
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    bool flag = false;
                    if (bool.TryParse(value, out flag))
                        return flag;
                    return defaultValue;
                case TypeCode.Char:
                    char c;
                    if (Char.TryParse(value, out c))
                        return c;
                    return defaultValue;
                case TypeCode.SByte:
                    sbyte s = 0;
                    if (SByte.TryParse(value, out s))
                        return s;
                    return defaultValue;
                case TypeCode.Byte:
                    byte b = 0;
                    if (Byte.TryParse(value, out b))
                        return b;
                    return defaultValue;
                case TypeCode.Int16:
                    Int16 i16 = 0;
                    if (Int16.TryParse(value, out i16))
                        return i16;
                    return defaultValue;
                case TypeCode.UInt16:
                    UInt16 ui16 = 0;
                    if (UInt16.TryParse(value, out ui16))
                        return ui16;
                    return defaultValue;
                case TypeCode.Int32:
                    int i = 0;
                    if (Int32.TryParse(value, out i))
                        return i;
                    return defaultValue;
                case TypeCode.UInt32:
                    UInt32 ui32 = 0;
                    if (UInt32.TryParse(value, out ui32))
                        return ui32;
                    return defaultValue;
                case TypeCode.Int64:
                    Int64 i64 = 0;
                    if (Int64.TryParse(value, out i64))
                        return i64;
                    return defaultValue;
                case TypeCode.UInt64:
                    UInt64 ui64 = 0;
                    if (UInt64.TryParse(value, out ui64))
                        return ui64;
                    return defaultValue;
                case TypeCode.Single:
                    Single single = 0;
                    if (Single.TryParse(value, out single))
                        return single;
                    return defaultValue;
                case TypeCode.Double:
                    double d = 0;
                    if (Double.TryParse(value, out d))
                        return d;
                    return defaultValue;
                case TypeCode.Decimal:
                    decimal de = 0;
                    if (Decimal.TryParse(value, out de))
                        return de;
                    return defaultValue;
                case TypeCode.DateTime:
                    DateTime dt;
                    if (DateTime.TryParse(value, out dt))
                        return dt;
                    return defaultValue;
                case TypeCode.String:
                    if (value.Length == 0)
                        return "";
                    return value.ToString();
            }
            throw new ArgumentNullException("defaultValue", "不能为Empty,Object,DBNull");
        }
        #endregion


        //public static Delegate Convert(Delegate eventHandler, Type eventHandlerType)
        //{
        //    Guard.ArgumentNotNull(eventHandler, "eventHandler");
        //    Guard.ArgumentNotNull(eventHandlerType, "eventHandlerType");
        //    return Delegate.CreateDelegate(eventHandlerType, eventHandler.Method);
        //}

        public static TDelegate Convert<TDelegate>(Delegate eventHandler)
        {
            return (TDelegate)(object)Convert(eventHandler, typeof(TDelegate));
        }

        public static bool IsValidEventHandler(Type eventHandlerType, out ParameterInfo[] parameters)
        {
            if (!typeof(Delegate).IsAssignableFrom(eventHandlerType))
            {
                parameters = new ParameterInfo[0];
                return false;
            }

            MethodInfo invokeMethod = eventHandlerType.GetMethod("Invoke");
            if (invokeMethod.ReturnType != typeof(void))
            {
                parameters = new ParameterInfo[0];
                return false;
            }
            parameters = invokeMethod.GetParameters();
            if (parameters.Length != 2 || parameters[0].ParameterType != typeof(object))
            {
                return false;
            }
            if (!typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
            {
                return false;
            }
            return true;
        }

        public static Delegate Convert(Delegate eventHandler, Type eventHandlerType)
        {
            ParameterInfo[] destinationParameters;
            if (!IsValidEventHandler(eventHandlerType, out destinationParameters))
            {
                throw new InvalidOperationException();
            }

            if (eventHandler.GetType() == eventHandlerType)
            {
                return eventHandler;
            }

            ParameterInfo[] sourceParameters;
            if (!IsValidEventHandler(eventHandler.GetType(), out sourceParameters))
            {
                throw new InvalidOperationException();
            }
            Type[] paramTypes = new Type[destinationParameters.Length + 1];
            paramTypes[0] = eventHandler.GetType();
            for (int i = 0; i < destinationParameters.Length; i++)
            {
                paramTypes[i + 1] = destinationParameters[i].ParameterType;
            }
            DynamicMethod method = new DynamicMethod("WrappedEventHandler", null, paramTypes);
            MethodInfo invoker = paramTypes[0].GetMethod("Invoke");
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            if (!sourceParameters[1].ParameterType.IsAssignableFrom(destinationParameters[1].ParameterType))
            {
                il.Emit(OpCodes.Castclass, sourceParameters[1].ParameterType);
            }
            il.Emit(OpCodes.Call, invoker);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(eventHandlerType, eventHandler);
        }
    }
}
