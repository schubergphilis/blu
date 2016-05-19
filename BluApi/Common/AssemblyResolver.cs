using System;
using System.Reflection;

namespace BluApi.Common
{
    /// <summary>
    /// For portability, all referenced libraries in this project are loaded as "Embedded Assmebly".
    /// The path to embedded assembly is resolved by this utility class.
    /// </summary>
    public static class AssemblyResolver
    {
        /// <summary>
        /// Return assembly as byte array
        /// </summary>
        /// <param name="args">Event args (Event is raised when we try to resole assembly path.)</param>
        /// <returns>Embedded assembly as byte[]</returns>
        public static Byte[] ResolveAssembly(ResolveEventArgs args)
        {
            if (args.Name.Contains("System.Management.Automation")) return null;
            String resourceName = "BluStation.Embedded." + new AssemblyName(args.Name).Name + ".dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return assemblyData;
            }
        }
    }
}
