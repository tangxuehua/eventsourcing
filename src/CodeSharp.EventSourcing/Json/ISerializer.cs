//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    public interface ISerializer
    {
        /// <summary>
        /// 将一个给定对象序列化为字符串
        /// </summary>
        string Serialize(object obj);
        /// <summary>
        /// 将一个字符串反序列化为指定类型的对象
        /// </summary>
        object Deserialize(string value, Type objectType);
        /// <summary>
        /// 将一个字符串反序列化为指定类型的对象，并返回该对象的强类型
        /// </summary>
        T Deserialize<T>(string value) where T : class;
        /// <summary>
        /// 将一个字符串反序列化为匿名类型的对象
        /// </summary>
        T Deserialize<T>(string value, T anonymousTypeObject);
    }
}
