using IPCLogger.Core.Resolvers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IPCLogger.Core.Resolvers
{
    public static class RFactory
    {

#region Static fields

        private static readonly Dictionary<string, IBaseResolver> _resolvers =
            new Dictionary<string, IBaseResolver>();

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
            return assembly.GetTypes().Where(t => typeof(IBaseResolver).IsAssignableFrom(t) && !t.IsInterface);
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
            lock (_resolvers)
            {
                foreach (Type resolverType in resolverTypes)
                {
                    if (Activator.CreateInstance(resolverType) is IBaseResolver resolver)
                    {
                        _resolvers.Add(resolverType.FullName, resolver);
                    }
                }
            }
        }

#endregion

#region Public methods

        public static IBaseResolver Get(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return null;
            }
            _resolvers.TryGetValue(className.Trim(), out IBaseResolver resolver);
            return resolver;
        }

#endregion

    }
}