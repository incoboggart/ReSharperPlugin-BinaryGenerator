using System;
using System.Collections.Generic;

namespace ReSharperPlugins.BinaryMethodsGenerator.CodeGeneration.TypesHandling
{
    internal sealed class VariablesTracker
    {
        private readonly Dictionary<String, Int32> _variables = new Dictionary<String, Int32>();

        public String Declare(String key)
        {
            Int32 count = 0;

            if (_variables.ContainsKey(key))
            {
                count = _variables[key] + 1;
            }

            _variables[key] = count;
            return key + count;
        }

        public String Use(String key)
        {
            Int32 count;
            _variables.TryGetValue(key, out count);
            return key + count;
        }

        public void Dispose(String key)
        {
            if (!_variables.ContainsKey(key))
            {
                return;
            }

            int count = _variables[key] - 1;

            if (count > 0)
            {
                _variables[key] = count;
            }
            else
            {
                _variables.Remove(key);
            }
        }
    }
}