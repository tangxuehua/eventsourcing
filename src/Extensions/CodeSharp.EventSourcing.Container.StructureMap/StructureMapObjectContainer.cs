using System;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace CodeSharp.EventSourcing.Container.StructureMap
{
    public class StructureMapObjectContainer : IObjectContainer
    {
        public void RegisterType(Type type)
        {
            var life = ParseLife(type);

            if (!IsRegistered(type))
            {
                if (life == LifeStyle.Singleton)
                {
                    ObjectFactory.Configure(x => x.For(type).Singleton().Use(type));
                }
                else
                {
                    ObjectFactory.Configure(x => x.For(type).Use(type));
                }
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!IsRegistered(interfaceType))
                {
                    if (life == LifeStyle.Singleton)
                    {
                        ObjectFactory.Configure(x => x.For(interfaceType).Singleton().Use(type));
                    }
                    else
                    {
                        ObjectFactory.Configure(x => x.For(interfaceType).Use(type));
                    }
                }
            }
        }
        public void RegisterType(Type type, string key)
        {
            var life = ParseLife(type);

            if (!IsRegistered(type, key))
            {
                if (life == LifeStyle.Singleton)
                {
                    ObjectFactory.Configure(x => x.For(type).Singleton().Use(type).Named(key));
                }
                else
                {
                    ObjectFactory.Configure(x => x.For(type).Use(type).Named(key));
                }
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!IsRegistered(interfaceType, key))
                {
                    if (life == LifeStyle.Singleton)
                    {
                        ObjectFactory.Configure(x => x.For(interfaceType).Singleton().Use(type).Named(key));
                    }
                    else
                    {
                        ObjectFactory.Configure(x => x.For(interfaceType).Use(type).Named(key));
                    }
                }
            }
        }
        public void RegisterTypes(Func<Type, bool> typePredicate, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetExportedTypes().Where(x => typePredicate(x)))
                {
                    RegisterType(type);
                }
            }
        }
        public void Register<TService, TImpl>(LifeStyle life) where TService : class where TImpl : class, TService
        {
            if (life == LifeStyle.Singleton)
            {
                ObjectFactory.Configure(x => x.For<TService>().Singleton().Use<TImpl>());
            }
            else
            {
                ObjectFactory.Configure(x => x.For<TService>().Transient().Use<TImpl>());
            }
        }
        public void Register<TService, TImpl>(string key, LifeStyle life = LifeStyle.Singleton) where TService : class where TImpl : class, TService
        {
            if (life == LifeStyle.Singleton)
            {
                ObjectFactory.Configure(x => x.For<TService>().Singleton().Use<TImpl>().Named(key));
            }
            else
            {
                ObjectFactory.Configure(x => x.For<TService>().Transient().Use<TImpl>().Named(key));
            }
        }
        public void RegisterDefault<TService, TImpl>(LifeStyle life) where TService : class where TImpl : class, TService
        {
            if (life == LifeStyle.Singleton)
            {
                ObjectFactory.Configure(x => x.For<TService>().Singleton().Use<TImpl>());
            }
            else
            {
                ObjectFactory.Configure(x => x.For<TService>().Transient().Use<TImpl>());
            }
        }
        public void Register<T>(T instance, LifeStyle life) where T : class
        {
            if (life == LifeStyle.Singleton)
            {
                ObjectFactory.Configure(x => x.For<T>().Singleton().Use(instance));
            }
            else
            {
                ObjectFactory.Configure(x => x.For<T>().Transient().Use(instance));
            }
        }
        public void Register<T>(T instance, string key, LifeStyle life) where T : class
        {
            if (life == LifeStyle.Singleton)
            {
                ObjectFactory.Configure(x => x.For<T>().Singleton().Use(instance).Named(key));
            }
            else
            {
                ObjectFactory.Configure(x => x.For<T>().Transient().Use(instance).Named(key));
            }
        }
        public bool IsRegistered(Type type)
        {
            return ObjectFactory.TryGetInstance(type) != null;
        }
        public bool IsRegistered(Type type, string key)
        {
            return ObjectFactory.TryGetInstance(type, key) != null;
        }
        public T Resolve<T>() where T : class
        {
            return ObjectFactory.GetInstance<T>();
        }
        public T Resolve<T>(string key) where T : class
        {
            return ObjectFactory.TryGetInstance<T>(key);
        }
        public object Resolve(Type type)
        {
            return ObjectFactory.GetInstance(type);
        }
        public object Resolve(string key, Type type)
        {
            return ObjectFactory.TryGetInstance(type, key);
        }

        private static LifeStyle ParseLife(Type type)
        {
            var componentAttributes = type.GetCustomAttributes(typeof(ComponentAttribute), false);
            return componentAttributes.Count() <= 0 ? LifeStyle.Transient : (componentAttributes[0] as ComponentAttribute).LifeStyle;
        }
    }
}
