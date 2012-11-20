//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.ComponentModel;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 提供一些实用的方法
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 将一个给定的对象转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">转换后的类型</typeparam>
        /// <param name="value">源对象</param>
        /// <returns>转换后的对象</returns>
        public static T ConvertType<T>(object value)
        {
            if (value == null)
            {
                return default(T);
            }
            TypeConverter typeConverter1 = TypeDescriptor.GetConverter(typeof(T));
            TypeConverter typeConverter2 = TypeDescriptor.GetConverter(value.GetType());
            if (typeConverter1.CanConvertFrom(value.GetType()))
            {
                return (T)typeConverter1.ConvertFrom(value);
            }
            else if (typeConverter2.CanConvertTo(typeof(T)))
            {
                return (T)typeConverter2.ConvertTo(value, typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
        /// <summary>
        /// 判断某个类型是否继承自某个父类
        /// </summary>
        /// <param name="type">要判断的类型</param>
        /// <param name="requiredBaseType">基类类型</param>
        public static void AssertTypeInheritance(Type type, Type requiredBaseType)
        {
            if (!requiredBaseType.IsAssignableFrom(type))
            {
                throw new EventSourcingException(string.Format("类型{0}不是一个有效的类型，因为没有继承类型{1}", type.FullName, requiredBaseType.FullName));
            }
        }
    }
}
