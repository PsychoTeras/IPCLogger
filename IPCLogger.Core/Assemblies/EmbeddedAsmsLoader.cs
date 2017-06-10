using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IPCLogger.Core.Assemblies
{
    static class EmbeddedAsmsLoader
    {

#region Static fields

        private static Assembly _assembly;
        private static string[] _resourceNames;

#endregion

#region Static ctor

        static EmbeddedAsmsLoader()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _resourceNames = _assembly.GetManifestResourceNames();
            AppDomain.CurrentDomain.AssemblyResolve += EventHandler;
        }

#endregion

#region Static methods

        private static Assembly EventHandler(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(',')).ToLower();
            string resourceName = _resourceNames.FirstOrDefault
                (
                    r =>
                        {
                            string name = r.ToLower();
                            return name.EndsWith(assemblyName + ".dll") ||
                                   name.EndsWith(assemblyName + ".exe");
                        }
                );
            if (resourceName == null)
            {
                return null;
            }

            byte[] bytes = null;
            using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    long length = stream.Length;
                    bytes = new byte[length];
                    stream.Read(bytes, 0, (int) length);
                }
            }

            return bytes != null ? Assembly.Load(bytes) : null;
        }

        public static void Init() { }

#endregion

    }
}
