using F3N.Hoard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace F3N.Hoard.State
{
    public class Dispatcher<TStore> where TStore : IStore
    {
        private static readonly Type _storeType = typeof(TStore);

        private static readonly string[] conventions = { "On", "Handle" };

        private static readonly Dictionary<Type, Func<IStore, DomainCommand<TStore>, object>> _commandHandlers = new Dictionary<Type, Func<IStore, DomainCommand<TStore>, object>>();
        private static readonly Dictionary<Type, Action<IStore, Event>> _eventHandlers = new Dictionary<Type, Action<IStore, Event>>();

        static Dispatcher()
        {
            foreach (MethodInfo method in GetMethods(_storeType))
            {
                Register(method);
            }
        }

        public static async Task<IEnumerable<Event>> Dispatch(IStore target, DomainCommand<TStore> command)
        {
            var commandType = command.GetType();
            Func<IStore, DomainCommand<TStore>, object> handler = _commandHandlers.Find(commandType);

            if (handler != null)
            {
                System.Diagnostics.Debug.WriteLine("About to dispatch command: {0}", commandType);
                object result = handler(target, command);
                IEnumerable<Event> output;

                if (result is Task<IEnumerable<Event>>)
                {
                    output = await (Task<IEnumerable<Event>>)result;
                }
                else
                {
                    output = (IEnumerable<Event>)result;
                }

                System.Diagnostics.Debug.WriteLine("Finished dispatching command: {0}", commandType);

                return output;
            }

            throw new InvalidOperationException($"Handler for {command} is not registered for {_storeType}");
        }

        public static void Dispatch(IStore target, Event evt)
        {
            var eventType = evt.GetType();
            Action<IStore, Event> handler = _eventHandlers.Find(eventType);

            if (handler != null)
            {
                System.Diagnostics.Debug.WriteLine("About to dispatch event: {0}", eventType);
                handler(target, evt);
                System.Diagnostics.Debug.WriteLine("Finished dispatching event: {0}", eventType);
            }
            else
            {
                throw new InvalidOperationException($"Handler for {evt} is not registered for {_storeType}");
            }
        }

        private static void Register(MethodInfo method)
        {
            Type message = method.GetParameters()[0].ParameterType;

            IEnumerable<Type> interfaces = message.GetTypeInfo().ImplementedInterfaces;

            if (interfaces.Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(DomainCommand<>) && x.GenericTypeArguments.First() == _storeType))
            {
                _commandHandlers.Add(message, MethodToCommmandHandler(method));
            }
            else if (typeof(Event).GetTypeInfo().IsAssignableFrom(message.GetTypeInfo()))
            {
                _eventHandlers.Add(message, MethodToEventHandler(method));
            }
            else
            {
                throw new InvalidOperationException(
                    String.Format("Method {0} with param {1} in {2}: Supported public methods are Handle, On with supported parameters of Command, Query, Event", method.Name, message.Name, _storeType));
            }
        }

        private static Func<IStore, DomainCommand<TStore>, object> MethodToCommmandHandler(MethodInfo method)
        {
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression request = Expression.Parameter(typeof(object));

            UnaryExpression targetConversion = Expression.Convert(target, method.DeclaringType);
            UnaryExpression requestConversion = Expression.Convert(request, method.GetParameters()[0].ParameterType);

            MethodCallExpression call = Expression.Call(targetConversion, method, requestConversion);
            Func<IStore, DomainCommand<TStore>, object> func = Expression.Lambda<Func<IStore, DomainCommand<TStore>, object>>(call, target, request).Compile();

            return (t, r) => func(t, r);
        }

        private static Action<IStore, Event> MethodToEventHandler(MethodInfo method)
        {
            //Debug.Assert(method.DeclaringType != null);
            ParameterExpression target = Expression.Parameter(typeof(object));
            ParameterExpression request = Expression.Parameter(typeof(object));

            UnaryExpression targetConversion = Expression.Convert(target, method.DeclaringType);
            UnaryExpression requestConversion = Expression.Convert(request, method.GetParameters()[0].ParameterType);

            MethodCallExpression call = Expression.Call(targetConversion, method, requestConversion);
            Action<IStore, Event> func = Expression.Lambda<Action<IStore, Event>>(call, target, request).Compile();

            return (t, r) => func(t, r);
        }

        private static IEnumerable<MethodInfo> GetMethods(Type actor)
        {
            while (true)
            {
                if (actor == typeof(Store<>) || actor == null)
                    yield break;

                IEnumerable<MethodInfo> methods = actor
                    .GetRuntimeMethods()
                    .Where(m =>
                            m.GetParameters().Length == 1 &&
                            !m.GetParameters()[0].IsOut &&
                            !m.GetParameters()[0].IsRetval &&
                            !m.IsGenericMethod && !m.ContainsGenericParameters &&
                            conventions.Contains(m.Name));

                foreach (MethodInfo method in methods)
                    yield return method;

                actor = actor.GetTypeInfo().BaseType;
            }
        }
    }
}

