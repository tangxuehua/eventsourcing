//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 内部辅助类，用于调用某个对象的某个方法
    /// </summary>
    internal class MethodInvoker
    {
        public static object InvokeMethod(Type objType, string methodName, params object[] parameters)
        {
            object obj = null;

            if (ObjectContainer.IsRegistered(objType))
            {
                obj = ObjectContainer.Resolve(objType);
            }
            else
            {
                obj = Activator.CreateInstance(objType);
            }

            return InvokeMethod(obj, methodName, parameters);
        }
        public static object InvokeMethod(object obj, string methodName, params object[] parameters)
        {
            return obj.GetType().GetMethod(methodName).Invoke(obj, parameters);
        }
    }
}
