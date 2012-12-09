//Copyright (c) CodeSharp.  All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace CodeSharp.EventSourcing.Container.Windsor
{
    public class WindsorObjectContainer : IObjectContainer
    {
        private IWindsorContainer _windsorContainer;

        public WindsorObjectContainer(IWindsorContainer windsorContainer)
        {
            _windsorContainer = windsorContainer;
        }

        public IWindsorContainer WindsorContainer
        {
            get { return _windsorContainer; }
        }

        public void RegisterType(Type type)
        {
            var life = ParseLife(type);

            if (!IsRegistered(type))
            {
                _windsorContainer.Register(Component.For(type).ImplementedBy(type).Named(type.FullName).Life(life));
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!IsRegistered(interfaceType))
                {
                    _windsorContainer.Register(Component.For(interfaceType).ImplementedBy(type).Named(interfaceType.FullName).Life(life));
                }
            }
        }
        public void RegisterType(Type type, string key)
        {
            var life = ParseLife(type);

            if (!IsRegistered(type, key))
            {
                _windsorContainer.Register(Component.For(type).ImplementedBy(type).Named(key).Life(life));
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!IsRegistered(interfaceType, key))
                {
                    _windsorContainer.Register(Component.For(interfaceType).ImplementedBy(type).Named(key).Life(life));
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
            _windsorContainer.Register(Component.For<TService>().ImplementedBy<TImpl>().Named(typeof(TService).FullName).Life(life));
        }
        public void Register<TService, TImpl>(string key, LifeStyle life = LifeStyle.Singleton) where TService : class where TImpl : class, TService
        {
            _windsorContainer.Register(Component.For<TService>().ImplementedBy<TImpl>().Named(key).Life(life));
        }
        public void RegisterDefault<TService, TImpl>(LifeStyle life) where TService : class where TImpl : class, TService
        {
            _windsorContainer.Register(Component.For<TService>().ImplementedBy<TImpl>().Named(typeof(TService).FullName).IsDefault().Life(life));
        }
        public void Register<T>(T instance, LifeStyle life) where T : class
        {
            _windsorContainer.Register(Component.For<T>().Instance(instance).Named(typeof(T).FullName).Life(life));
        }
        public void Register<T>(T instance, string key, LifeStyle life) where T : class
        {
            _windsorContainer.Register(Component.For<T>().Instance(instance).Named(key).Life(life));
        }
        public bool IsRegistered(Type type)
        {
            return _windsorContainer.Kernel.HasComponent(type);
        }
        public bool IsRegistered(Type type, string key)
        {
            try
            {
                return _windsorContainer.Kernel.Resolve(key, type) != null;
            }
            catch
            {
                return false;
            }
        }
        public T Resolve<T>() where T : class
        {
            return _windsorContainer.Resolve<T>(typeof(T).FullName);
        }
        public T Resolve<T>(string key) where T : class
        {
            return _windsorContainer.Resolve<T>(key);
        }
        public object Resolve(Type type)
        {
            return _windsorContainer.Resolve(type.FullName, type);
        }
        public object Resolve(string key, Type type)
        {
            return _windsorContainer.Resolve(key, type);
        }

        private static LifeStyle ParseLife(Type type)
        {
            var componentAttributes = type.GetCustomAttributes(typeof(ComponentAttribute), false);
            return componentAttributes.Count() <= 0 ? LifeStyle.Transient : (componentAttributes[0] as ComponentAttribute).LifeStyle;
        }
    }
    public static class Extensions
    {
        public static ComponentRegistration<T> Life<T>(this ComponentRegistration<T> registration, LifeStyle life) where T : class
        {
            if (life == LifeStyle.Singleton)
            {
                return registration.LifeStyle.Singleton;
            }
            return registration.LifeStyle.Transient;
        }
    }
}