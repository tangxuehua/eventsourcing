//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace CodeSharp.EventSourcing
{
    public class DynamicLifetimeManager<T> : AbstractDictStackLifetimeManager<T> where T : class, ILifetimeObject
    {
        protected override IDictionary GetDictionary()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Items[DictionaryName] as IDictionary;
            }
            else
            {
                return CallContext.GetData(DictionaryName) as IDictionary;
            }
        }

        protected override void StoreDictionary(IDictionary dictionary)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[DictionaryName] = dictionary;
            }
            else
            {
                CallContext.SetData(DictionaryName, dictionary);
            }
        }
    }
}
