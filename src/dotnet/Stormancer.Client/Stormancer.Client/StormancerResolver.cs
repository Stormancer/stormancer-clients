using System;
using System.Collections.Generic;
using System.Linq;

namespace Stormancer
{
    /// <summary>
    /// Dependency scope
    /// </summary>
    public interface IDependencyScope
    {
        T Resolve<T>() where T : class;
        T? ResolveOptional<T>() where T : class;
        IEnumerable<T> ResolveAll<T>() where T : class;
        bool TryResolve<T>(out T? dependency) where T : class;
       
      
    }

    public class DependencyBuilder
    {
        public Registration Register<T>(Func<T> component) where T : class
        {
            return Register(_ => component());
        }
        public Registration<T> Register<T>(Func<IDependencyScope, T> component) where T : class
        {

        }
        public Registration<T> RegisterInstance<T>(T component) where T : class
        {

        }
    }
    public class Registration
    {

    }
    public class Registration<T> : Registration where T : class
    {

        public Func<IDependencyScope, object>? factory;
        public bool singleInstance;
        public object? instance;

        public Registration(Func<IDependencyResolver, object>? factory, bool singleInstance, object? instance)
        {
            this.factory = factory;
            this.singleInstance = singleInstance;
            this.instance = instance;
        }

        public Registration As<TService>()
    }

    public class StormancerResolver : IDependencyResolver, IDisposable
    {
        private readonly Dictionary<Type, List<Registration>> _registrations = new Dictionary<Type, List<Registration>>();
        private readonly StormancerResolver? _parent = null;


        public StormancerResolver(StormancerResolver? parent = null)
        {
            _parent = parent;
        }

       public  T? ResolveOptional<T>() where T : notnull
        {
            if (TryResolve<T>(out var result))
            {
                return result;
            }
            else if (_parent != null && _parent.TryResolve(out result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }
        public T Resolve<T>() where T : notnull
        {

            var result = ResolveOptional<T>();
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("The requested component of type {0} was not registered.", typeof(T)));
            }
        }

        public bool TryResolve<T>(out T? dependency) where T : notnull
        {

            if (_registrations.TryGetValue(typeof(T), out var registrations) && registrations.Any())
            {
                var registration = registrations.Last();
                var factory = registration.factory;
                if (registration.singleInstance)
                {

                    if (registration.instance == null && factory != null)
                    {
                        registration.instance = factory(this);
                    }

                    dependency = (T?)registration.instance;

                    return dependency != null;
                }
                else
                {
                    if (factory != null)
                    {
                        dependency = (T)factory(this);
                        return dependency != null;
                    }
                }
            }

            dependency = default(T);
            return false;
        }

        private Func<IDependencyResolver, T> ResolveFactory<T>()
        {

            if (_registrations.TryGetValue(typeof(T), out var registrations) && registrations.Any())
            {
                var registration = registrations.Last();
                return resolver => (T)(registration.factory(resolver));
            }
            else if (_parent != null)
            {
                return _parent.ResolveFactory<T>();
            }
            else
            {
                return null;
            }
        }

        public void Register<T>(Func<T> component) where T : notnull
        {
            Register(c => component(), true);
        }

        public void Register<T>(Func<IDependencyResolver, T> factory, bool singleInstance = false) where T : notnull
        {
            Registration registration = new Registration((dependencyResolver) => factory(dependencyResolver), singleInstance, null);


            if (!_registrations.TryGetValue(typeof(T), out var registrations))
            {
                registrations = new List<Registration>();
                _registrations.Add(typeof(T), registrations);
            }
            registrations.Add(registration);
        }

        public void RegisterDependency<T>(T component) where T : notnull
        {
            Registration registration = new Registration(null, true, component);
            registration.singleInstance = true;
            registration.instance = component;
            if (!_registrations.TryGetValue(typeof(T), out var registrations))
            {
                registrations = new List<Registration>();
                _registrations.Add(typeof(T), registrations);
            }
            registrations.Add(registration);
        }

        public void Dispose()
        {
            foreach (var registrations in _registrations)
            {
                foreach (var registration in registrations.Value)
                {
                    var disposable = registration.instance as IDisposable;
                    disposable?.Dispose();
                }
            }
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            if (_registrations.TryGetValue(typeof(T), out var registrations))
            {
                foreach (var registration in registrations)
                {

                    var factory = registration.factory;
                    if (registration.singleInstance)
                    {
                        if (registration.instance == null && factory != null)
                        {
                            registration.instance = factory(this);
                        }

                        yield return (T)registration.instance;

                    }
                    else
                    {
                        yield return (T)registration.factory(this);
                    }



                }
            }


        }
    }
}