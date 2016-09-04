using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UCCTaskSolver;

namespace CCluster.Common.Solver
{
    public class TaskSolverTypesLoader
    {
        public ICollection<Type> LoadPluginTypes(string path)
        {
            string[] dllFileNames = null;
            ICollection<Type> pluginTypes = new List<Type>();

            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.dll");

                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (var dllFile in dllFileNames)
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(assemblyName);
                    assemblies.Add(assembly);
                }

                Type baseType = typeof (TaskSolver);

                foreach (var assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (!type.IsAbstract && !type.IsInterface && type.BaseType == baseType)
                        {
                            pluginTypes.Add(type);
                        }
                    }
                }
            }

            return pluginTypes;
        }
    }
}