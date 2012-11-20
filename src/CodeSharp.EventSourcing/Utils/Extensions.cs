//Copyright (c) CodeSharp.  All rights reserved.

using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 提供一些实用的方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 判断当前的程序集是否是在Debug模式下编译
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsDebugBuild(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(x => x.IsJITTrackingEnabled);
        }
    }
}
