//Copyright (c) CodeSharp.  All rights reserved.

using System;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 标记某个类是一个组件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public LifeStyle LifeStyle { get; set; }
        public ComponentAttribute() : this(LifeStyle.Transient) { }
        public ComponentAttribute(LifeStyle lifeStyle)
        {
            this.LifeStyle = lifeStyle;
        }
    }
    /// <summary>
    /// 组件生命生命周期枚举
    /// </summary>
    public enum LifeStyle
    {
        /// <summary>
        /// 瞬态
        /// </summary>
        Transient = 0,
        /// <summary>
        /// 单例
        /// </summary>
        Singleton
    }
}
