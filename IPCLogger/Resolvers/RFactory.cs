using IPCLogger.Resolvers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IPCLogger.Resolvers
{
    internal static class RFactory
    {

#region Static fields

        private static readonly Dictionary<string, IResolver> _namedResolvers =
            new Dictionary<string, IResolver>();

        private static readonly Dictionary<Enum, ResolverList> _typedResolvers =
            new Dictionary<Enum, ResolverList>();

#endregion

#region Static ctor

        static RFactory()
        {
            InitializeResolversList();
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoadEventHandler;
        }

#endregion

#region Private methods

        private static IEnumerable<Type> GetDeclaredResolvers(Assembly assembly)
        {
            return assembly.GetTypes().Where
            (
                t => typeof(IResolver).IsAssignableFrom(t) && !t.IsAbstract && t.Name != nameof(ResolverList)
            );
        }

        private static void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                IEnumerable<Type> resolverTypes = GetDeclaredResolvers(args.LoadedAssembly);
                AppendResolvers(resolverTypes);
            }
            catch { }
        }

        private static void InitializeResolversList()
        {
            List<Type> resolverTypes = new List<Type>();
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.
                GetAssemblies().
                Where(a => !a.FullName.StartsWith("System."));
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    resolverTypes.AddRange(GetDeclaredResolvers(assembly));
                }
                catch { }
            }

            AppendResolvers(resolverTypes);
        }

        private static void AppendResolvers(IEnumerable<Type> resolverTypes)
        {
            lock (_namedResolvers)
            {
                foreach (Type resolverType in resolverTypes)
                {
                    if (Activator.CreateInstance(resolverType) is IResolver resolver)
                    {
                        if (!_typedResolvers.TryGetValue(resolver.Type, out var resolvers))
                        {
                            resolvers = new ResolverList(resolver.Type);
                            _typedResolvers.Add(resolver.Type, resolvers);
                        }

                        string resolverName = resolver.GetType().FullName;
                        _namedResolvers.Add(resolverName, resolver);

                        resolvers.Add(resolver);
                    }
                }
            }
        }

#endregion

#region Public methods

        public static IResolver Get(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return null;
            }
            _namedResolvers.TryGetValue(className.Trim(), out IResolver resolver);
            return resolver;
        }

        public static IResolver Get(Enum e)
        {
            _typedResolvers.TryGetValue(e, out ResolverList resolvers);
            return resolvers;
        }

        public static IResolver Get(Enum e, object tag)
        {
            _typedResolvers.TryGetValue(e, out ResolverList resolvers);
            return resolvers?.GetByTag(tag);
        }

#endregion

    }
}