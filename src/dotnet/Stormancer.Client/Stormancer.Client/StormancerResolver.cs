using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Stormancer
{
    /// <summary>
    /// An exception that occurs when a cycle is detected while resolving a dependency.
    /// </summary>
    public class DependencyCycleException : Exception
    {
        internal DependencyCycleException(Stack<Type> stack) : base("Failed to resolve dependency because a cycle was detected in the dependency graph.")
        {
            Stack = stack;


        }

        /// <summary>
        /// Dependency stack leading to a cycle.
        /// </summary>
        public Stack<Type> Stack { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DependencyResolutionContext
    {
        private readonly DependencyScope _scope;

        private readonly Stack<Type>? _cycleDetectionStack;

        internal DependencyResolutionContext(DependencyScope scope, Registration registration)
        {
            _scope = scope;
            if (!registration.CycleCheckDone)
            {
                _cycleDetectionStack = new Stack<Type>();
            }
        }


        /// <summary>
        /// Resolves a mandatory dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return _scope.Resolve<T>(this);
        }

        /// <summary>
        /// Resolves an optional dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? ResolveOptional<T>() where T : class
        {
            return _scope.ResolveOptional<T>(this);
        }

        /// <summary>
        /// Resolves all dependencies satisfying a given contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return _scope.ResolveAll<T>(this);
        }

        /// <summary>
        /// Tries resolving a dependency satisfying a given contract, and returns false if it couldn't be resolved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public bool TryResolve<T>([NotNullWhen(true)] out T? dependency) where T : class
        {
            return _scope.TryResolve<T>(out dependency, this);
        }

        internal void PushCycleTest(Type implementationType)
        {
            if (_cycleDetectionStack != null)
            {
                if (_cycleDetectionStack.Contains(implementationType))
                {
                    _cycleDetectionStack.Push(implementationType);
                    throw new DependencyCycleException(_cycleDetectionStack);
                }
                else
                {
                    _cycleDetectionStack.Push(implementationType);
                }
            }

        }
    }

    /// <summary>
    /// A Scope for dependencies 
    /// </summary>
    public class DependencyScope : IDisposable
    {
        private class Dependency : IDisposable
        {
            public Dependency(Registration registration)
            {
                Registration = registration;
            }
            public object? Instance { get; set; }
            public Registration Registration { get; }

            public void Dispose()
            {
                if (Instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        private readonly IEnumerable<Registration> _registrations;
        private readonly DependencyScope? _parent;

        object _syncRoot = new object();

        private readonly Dictionary<Type, List<Dependency>> _dependencies = new Dictionary<Type, List<Dependency>>();
        internal DependencyScope(IEnumerable<Registration> registrations)
        {
            _registrations = registrations;
        }

        internal DependencyScope(DependencyScope parent, IEnumerable<Registration> registrations)
        {
            _registrations = registrations;
            _parent = parent;
        }
        /// <summary>
        /// Resolves a mandatory dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>(DependencyResolutionContext? context = null) where T : class
        {
            var type = typeof(T);
            T? instance = null;
            lock (_syncRoot)
            {
                if (_dependencies.TryGetValue(type, out var dependencies))
                {
                    var dep = dependencies[dependencies.Count - 1];


                    if (dep.Registration.SingleInstance)
                    {
                        if (dep.Instance == null)
                        {
                            var registration = dep.Registration;
                            if (context == null)
                            {
                                context = new DependencyResolutionContext(this, registration);
                            }
                            dep.Instance = registration.GetInstance<T>(context);
                        }

                        instance = (T?)dep.Instance;
                    }
                    else
                    {
                        instance = dep.Registration.GetInstance<T>(context);
                    }
                }
                else if (_parent != null)
                {
                    return _parent.Resolve<T>(context);
                }
            }

            if (instance == null)
            {
                throw new InvalidOperationException($"Fail to resolve '{typeof(T)}.'");
            }
            return instance;
        }

        /// <summary>
        /// Resolves an optional dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? ResolveOptional<T>(DependencyResolutionContext? context = null) where T : class
        {

        }

        /// <summary>
        /// Resolves all dependencies satisfying a given contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ResolveAll<T>(DependencyResolutionContext? context = null) where T : class
        {

        }

        /// <summary>
        /// Tries resolving a dependency satisfying a given contract, and returns false if it couldn't be resolved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public bool TryResolve<T>([NotNullWhen(true)] out T? dependency, DependencyResolutionContext? context = null) where T : class
        {

        }

        /// <summary>
        /// Creates a child scope of the current scope, that inherits registrations and 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public DependencyScope CreateChild(string name, Action<DependencyBuilder>? configurator = null)
        {
            if (configurator == null)
            {
                return new DependencyScope(this, Enumerable.Empty<Registration>());
            }
            else
            {
                var builder = new DependencyBuilder();
                configurator(builder);
                return new DependencyScope(this, builder.Registrations);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides methods to configure dependencies in a scope.
    /// </summary>
    public class DependencyBuilder
    {
        /// <summary>
        /// Registers a new dependency type by providing a factory method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public Registration Register<T>(Func<T> component) where T : class
        {
            return Register(_ => component());
        }

        /// <summary>
        /// Registers a new dependency type by providing a factory method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public Registration<T> Register<T>(Func<DependencyResolutionContext, T> component) where T : class
        {
            var registration = new Registration<T>(component);
            Registrations.Add(registration);
            return registration;
        }

        /// <summary>
        /// Registers a dependency as an already created instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public Registration<T> Register<T>(T component) where T : class
        {
            var registration = new Registration<T>(component);
            Registrations.Add(registration);
            return registration;
        }

        internal List<Registration> Registrations { get; } = new List<Registration>();
    }

    /// <summary>
    /// Base class for registrations in the dependency injection system.
    /// </summary>
    public class Registration
    {
        internal Registration(Type implementationType)
        {
            ImplementationType = implementationType;
        }

        /// <summary>
        /// Instance provided when creating the registration if it exists.
        /// </summary>
        protected object? _instance;

        /// <summary>
        /// Factory used to create new instances of the registration if it exists.
        /// </summary>
        /// <remarks>
        /// <see cref="_instance"/> and <see cref="_factory"/> are mutually exclusive.
        /// </remarks>
        protected Func<DependencyResolutionContext, object>? _factory;

        /// <summary>
        /// Gets the type of the objects created by the registration.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// Gets the list of services provided by the registration.
        /// </summary>
        public List<Type> ServicesTypes { get; } = new List<Type>();

        /// <summary>
        /// Gets a value indicating if the registration should be a single instance per scope.
        /// </summary>
        public bool SingleInstance { get; protected set; }

        /// <summary>
        /// True if the registration was tested for cycles.
        /// </summary>
        internal bool CycleCheckDone { get; set; } = false;

        internal TService? GetInstance<TService>(DependencyResolutionContext context) where TService : class
        {
            if (!CycleCheckDone)
            {
                context.EnsureNotCycle(ImplementationType);
            }
            if (_instance != null)
            {
                CycleCheckDone = true;
                return (TService)_instance;
            }
            else
            {

                var result = (TService?)_factory?.Invoke(context);
                CycleCheckDone = true;
                return result;
            }
        }
    }

    /// <summary>
    /// A registration in the Dependency injection system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Registration<T> : Registration where T : class
    {



        internal Registration(Func<DependencyResolutionContext, T>? factory) : base(typeof(T))
        {
            _factory = factory;

        }

        internal Registration(T instance) : base(typeof(T))
        {
            _instance = instance;

        }

        /// <summary>
        /// Adds a <typeparamref name="TService"/> as a contract for <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public Registration<T> As<TService>() where TService : class
        {
            if (typeof(TService).IsAssignableFrom(typeof(T)))
            {
                ServicesTypes.Add(typeof(TService));
                return this;
            }
            else
            {
                throw new InvalidCastException($"'{typeof(T)}' does not implement '{typeof(TService)}'");
            }
        }
    }


    internal class Container : IDisposable
    {
        private DependencyScope? _rootScope;
        public IEnumerable<Registration> Registrations { get; }
        public Container(Action<DependencyBuilder> configurator)
        {
            var builder = new DependencyBuilder();
            configurator(builder);
            Registrations = builder.Registrations;
        }

        public DependencyScope CreateRootScope()
        {
            _rootScope = new DependencyScope(Registrations);
            return _rootScope;
        }

        public void Dispose()
        {
            _rootScope?.Dispose();
        }
    }


    public class StormancerResolver : IDependencyResolver, IDisposable
    {
        private readonly Dictionary<Type, List<Registration>> _registrations = new Dictionary<Type, List<Registration>>();
        private readonly StormancerResolver? _parent = null;


        public StormancerResolver(StormancerResolver? parent = null)
        {
            _parent = parent;
        }

        public T? ResolveOptional<T>() where T : notnull
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