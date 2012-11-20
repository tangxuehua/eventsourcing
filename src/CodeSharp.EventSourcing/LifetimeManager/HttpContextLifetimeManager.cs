//Copyright (c) CodeSharp.  All rights reserved.

using System.Collections;
using System.Web;

namespace CodeSharp.EventSourcing
{
    public class HttpContextLifetimeManager<T> : AbstractDictStackLifetimeManager<T> where T : class, ILifetimeObject
    {
        protected override IDictionary GetDictionary()
        {
            return HttpContext.Current.Items[DictionaryName] as IDictionary;
        }
        protected override void StoreDictionary(IDictionary dictionary)
        {
            HttpContext.Current.Items[DictionaryName] = dictionary;
        }
    }
}