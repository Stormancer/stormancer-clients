using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Net.WebSockets;

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

        internal void PushCycleStep(Type implementationType)
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
        internal void PopCycleStep()
        {
            if (_cycleDetectionStack != null)
            {
                _cycleDetectionStack.Pop();
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
        private readonly DependencyScope? _parent;


        private readonly Dictionary<Type, List<Dependency>> _dependencies = new Dictionary<Type, List<Dependency>>();
        internal DependencyScope(IEnumerable<Registration> registrations)
        {

            foreach (var registration in registrations)
            {
                bool registered = false;
                foreach (var serviceType in registration.ServicesTypes)
                {
                    registered = true;
                    if (!_dependencies.TryGetValue(serviceType, out var dependencies))
                    {
                        dependencies = new List<Dependency>();
                        _dependencies.Add(serviceType, dependencies);
                    }

                    dependencies.Add(new Dependency(registration));
                }
                if(!registered)
                {
                    if (!_dependencies.TryGetValue(registration.ImplementationType, out var dependencies))
                    {
                        dependencies = new List<Dependency>();
                        _dependencies.Add(registration.ImplementationType, dependencies);
                    }

                    dependencies.Add(new Dependency(registration));
                }

                
            }
        }

        internal DependencyScope(DependencyScope parent, IEnumerable<Registration> registrations) : this(registrations)
        {

            _parent = parent;

        }


        /// <summary>
        /// Resolves a mandatory dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>(DependencyResolutionContext? context = null) where T : class
        {

            if (TryResolve<T>(out var result, context))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException($"Fail to resolve '{typeof(T)}.'");
            }
        }

        /// <summary>
        /// Resolves an optional dependency.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? ResolveOptional<T>(DependencyResolutionContext? context = null) where T : class
        {
            if (TryResolve<T>(out var result, context))
            {
                return result;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Resolves all dependencies satisfying a given contract.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ResolveAll<T>(DependencyResolutionContext? context = null) where T : class
        {
            var type = typeof(T);


            if (_dependencies.TryGetValue(type, out var dependencies))
            {
                foreach (var dep in dependencies)
                {
                    var registration = dep.Registration;
                    if (context == null)
                    {
                        context = new DependencyResolutionContext(this, registration);
                    }

                    if (dep.Registration.LifecyclePolicy.SingleInstance)
                    {
                        if (dep.Instance == null)
                        {


                            dep.Instance = registration.GetInstance<T>(context);
                        }
                        if (dep.Instance is T instance)
                        {
                            yield return instance;
                        }

                    }
                    else if (registration.GetInstance<T>(context) is T instance)
                    {
                        yield return instance;
                    }
                }
            }
            else if (_parent != null)
            {
                foreach (var v in _parent.ResolveAll<T>(context))
                {
                    yield return v;
                }

            }

        }

        /// <summary>
        /// Tries resolving a dependency satisfying a given contract, and returns false if it couldn't be resolved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public bool TryResolve<T>([NotNullWhen(true)] out T? dependency, DependencyResolutionContext? context = null) where T : class
        {
            var type = typeof(T);

            if (_dependencies.TryGetValue(type, out var dependencies))
            {
                var dep = dependencies[dependencies.Count - 1];
                var registration = dep.Registration;
                if (context == null)
                {
                    context = new DependencyResolutionContext(this, registration);
                }

                if (dep.Registration.LifecyclePolicy.SingleInstance)
                {
                    if (dep.Instance == null)
                    {


                        dep.Instance = registration.GetInstance<T>(context);
                    }

                    dependency = (T?)dep.Instance;
                }
                else
                {
                    dependency = registration.GetInstance<T>(context);
                }
            }
            else if (_parent != null)
            {
                dependency = _parent.Resolve<T>(context);

            }
            else
            {
                dependency = default;
            }


            return dependency != null;
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

        /// <summary>
        /// Disposes the scope and all disposable dependencies in it.
        /// </summary>
        public void Dispose()
        {
            foreach (var dependency in _dependencies.SelectMany(kvp => kvp.Value))
            {
                if (dependency.Instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
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
        public Registration<T> Register<T>(Func<T> component) where T : class
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
    /// Describes a scope policy
    /// </summary>
    public class ScopePolicy
    {
        public bool SingleInstance { get; set; }
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
        public ScopePolicy LifecyclePolicy { get; protected set; } = new ScopePolicy();

        /// <summary>
        /// True if the registration was tested for cycles.
        /// </summary>
        internal bool CycleCheckDone { get; set; } = false;

        internal TService? GetInstance<TService>(DependencyResolutionContext context) where TService : class
        {
            if (!CycleCheckDone)
            {
                context.PushCycleStep(ImplementationType);
            }
            TService? instance;
            if (_instance != null)
            {
                CycleCheckDone = true;
                instance = (TService?)_instance;
            }
            else
            {

                instance = (TService?)_factory?.Invoke(context);
                CycleCheckDone = true;

            }
            if (!CycleCheckDone)
            {
                context.PopCycleStep();
            }
            return instance;
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
        public Registration<T> SingleInstance()
        {
            this.LifecyclePolicy.SingleInstance = true;
            return this;
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


}