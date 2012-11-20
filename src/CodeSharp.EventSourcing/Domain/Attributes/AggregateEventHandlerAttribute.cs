//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSharp.EventSourcing
{
    /// <summary>
    /// 用于标记聚合根内的某个方法是某个事件的响应函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class AggregateEventHandlerAttribute : Attribute
    {
        private IList<Path> _paths = new List<Path>();
        private const string Seperator = ":";

        /// <summary>
        /// 一个经过解析后的Path对象集合。
        /// <remarks>
        /// 一个解析前的Path字符串由一个类型名称和一个聚合根ID所对应的属性名称组成。
        /// 类型是聚合根的类型，聚合根ID所对应的属性名称是指该类型的聚合根在其前一个Path的类型中的对象中的属性名。
        /// 第一个Path的前一个对象为事件对象本身；
        /// 另外，如果Path只有一级，那类型名称可以不指定，只需指定属性名称即可，此时聚合根的类型自动用当前响应该事件的聚合根类型。
        /// 比如下面这个例子，我在Forum聚合根内有如下方法：
        /// [AggregateEventHandler("ForumId")]
        /// private void ChangeState(ThreadCreated threadCreated)
        /// 该例子中因为Path只有一级，所以只需要指定属性名即可，聚合根的类型自动用当前响应该事件的聚合根类型。
        /// 该属性名表示我们可以从ThreadCreated事件中根据"ForumId"属性找到类型为Forum的聚合根，该聚合根就是ThreadCreated事件的订阅者。
        /// 另一个例子，Path有两级或更多级的情况：
        /// [AggregateEventHandler("Thread:ThreadId", "Forum:ForumId")]
        /// private void ChangeState(PostCreated postCreated)
        /// 如果Path有两级或以上，则需要像上面那样逐个指定每一级Path。
        /// Thread:ThreadId是第一级Path，ThreadId表示在PostCreated事件中有一个名称为ThreadId的属性，该属性对应的对象的类型是一个Thread类；
        /// 同理，Forum:ForumId是第二级Path，ForumId表示在Thread类中有一个名称为ForumId的属性，该属性对应的对象的类型是一个Forum类；
        /// 最后，对于类型名称，识别规则是：用户只需提供名称能让框架唯一识别其是某个唯一的类型即可。
        /// 比如，如果当前Model层中所有的聚合根的类型名称都不重复，那只需要指定类型的名称即可，即Type.Name即可；
        /// 如果当前Model层有类名重复的聚合根，那需要适当补充类名的前一级命名空间，直到确定从该命名空间开始到该类型名称所表示的字符串可以唯一确定这个类型即可。
        /// 比如像下面这样有两个命名空间中都有一个聚合根类型名称为Account。
        /// EventSourcing.Sample.Model.Orders.Account，EventSourcing.Sample.Model.MoneyTransfer.Account
        /// 那在Path中就应该这样写："Orders.Account:AccountId"，或"MoneyTransfer.Account:AccountId"
        /// </remarks>
        /// </summary>
        public IEnumerable<Path> Paths { get { return _paths; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paths">
        /// 一个Path集合。一个Path由一个类型名称和一个聚合根ID所对应的属性名称组成。
        /// 类型是聚合根的类型，聚合根ID所对应的属性名称是指该类型的聚合根在其前一个Path的类型中的对象中的属性名。
        /// 第一个Path的前一个对象为事件对象本身；
        /// 另外，如果Path只有一级，那类型名称可以不指定，只需指定属性名称即可，此时聚合根的类型自动用当前响应该事件的聚合根类型。
        /// 比如下面这个例子，我在Forum聚合根内有如下方法：
        /// [AggregateEventHandler("ForumId")]
        /// private void ChangeState(ThreadCreated threadCreated)
        /// 该例子中因为Path只有一级，所以只需要指定属性名即可，聚合根的类型自动用当前响应该事件的聚合根类型。
        /// 该属性名表示我们可以从ThreadCreated事件中根据"ForumId"属性找到类型为Forum的聚合根，该聚合根就是ThreadCreated事件的订阅者。
        /// 另一个例子，Path有两级或更多级的情况：
        /// [AggregateEventHandler("Thread:ThreadId", "Forum:ForumId")]
        /// private void ChangeState(PostCreated postCreated)
        /// 如果Path有两级或以上，则需要像上面那样逐个指定每一级Path。
        /// Thread:ThreadId是第一级Path，ThreadId表示在PostCreated事件中有一个名称为ThreadId的属性，该属性对应的对象的类型是一个Thread类；
        /// 同理，Forum:ForumId是第二级Path，ForumId表示在Thread类中有一个名称为ForumId的属性，该属性对应的对象的类型是一个Forum类；
        /// 最后，对于类型名称，识别规则是：用户只需提供名称能让框架唯一识别其是某个唯一的类型即可。
        /// 比如，如果当前Model层中所有的聚合根的类型名称都不重复，那只需要指定类型的名称即可，即Type.Name即可；
        /// 如果当前Model层有类名重复的聚合根，那需要适当补充类名的前一级命名空间，直到确定从该命名空间开始到该类型名称所表示的字符串可以唯一确定这个类型即可。
        /// 比如像下面这样有两个命名空间中都有一个聚合根类型名称为Account。
        /// EventSourcing.Sample.Model.Orders.Account，EventSourcing.Sample.Model.MoneyTransfer.Account
        /// 那在Path中就应该这样写："Orders.Account:AccountId"，或"MoneyTransfer.Account:AccountId"
        /// </param>
        public AggregateEventHandlerAttribute(params string[] paths)
        {
            ParsePaths(paths);
        }

        private void ParsePaths(IEnumerable<string> paths)
        {
            if (paths == null || paths.Count() == 0)
            {
                throw new EventSourcingException("Path count can not be null or zero.");
            }

            if (paths.Count() == 1)
            {
                var path = paths.Single();
                if (string.IsNullOrEmpty(path))
                {
                    throw new EventSourcingException("Path can not be null or empty string.");
                }

                var items = path.Split(new string[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 1)
                {
                    _paths.Add(new Path(null, path));
                }
                else if (items.Length == 2)
                {
                    AddNewPath(path, items[0], items[1]);
                }
                else
                {
                    throw new EventSourcingException(string.Format("Invalid path format, path:{0}", path));
                }
            }
            else
            {
                foreach (var path in paths)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        throw new EventSourcingException("Path can not be null or empty string.");
                    }
                    var items = path.Split(new string[] { Seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length != 2)
                    {
                        throw new EventSourcingException(string.Format("Invalid path format, path:{0}", path));
                    }
                    else
                    {
                        AddNewPath(path, items[0], items[1]);
                    }
                }
            }
        }
        private void AddNewPath(string path, string className, string propertyName)
        {
            var aggregateRootTypeProvider = ObjectContainer.Resolve<IAggregateRootTypeProvider>();
            var matchedTypes = aggregateRootTypeProvider.GetAllAggregateRootTypes().Where(x => x.FullName.EndsWith(className));

            if (matchedTypes.Count() == 0)
            {
                throw new EventSourcingException(string.Format("Not find matched aggregate root type which endwith class name '{0}' in path '{1}'", className, path));
            }
            else if (matchedTypes.Count() > 1)
            {
                throw new EventSourcingException(string.Format("Find more than one matched aggregate root types which endwith class name '{0}' in path '{1}'", className, path));
            }
            _paths.Add(new Path(matchedTypes.Single(), propertyName));
        }
    }

    /// <summary>
    /// 表示一个路径，该路径用于在事件发生时找到响应的聚合根事件订阅者。
    /// </summary>
    public class Path
    {
        /// <summary>
        /// 聚合根类型
        /// </summary>
        public Type AggregateRootType { get; private set; }
        /// <summary>
        /// 聚合根的ID在源对象中的属性名
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aggregateRootType">聚合根的类型</param>
        /// <param name="propertyName">聚合根的ID在源对象中的属性名</param>
        public Path(Type aggregateRootType, string propertyName)
        {
            AggregateRootType = aggregateRootType;
            PropertyName = propertyName;
        }
    }
    /// <summary>
    /// Path类的泛型定义
    /// </summary>
    /// <typeparam name="TAggregateRoot">聚合根类型</typeparam>
    public class Path<TAggregateRoot> : Path where TAggregateRoot : AggregateRoot
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName">聚合根的ID在源对象中的属性名</param>
        public Path(string propertyName) : base(typeof(TAggregateRoot), propertyName)
        {
        }
    }
}