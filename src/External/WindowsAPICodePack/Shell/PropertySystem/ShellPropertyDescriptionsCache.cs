//Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell;

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    internal class ShellPropertyDescriptionsCache
    {
        private ShellPropertyDescriptionsCache() { propsDictionary = new Dictionary<PropertyKey, ShellPropertyDescription>(); }
        private IDictionary<PropertyKey, ShellPropertyDescription> propsDictionary = null;
        
        private static ShellPropertyDescriptionsCache cacheInstance = null;

        public static ShellPropertyDescriptionsCache Cache
        {
            get
            {
                if (cacheInstance == null)
                {
                    cacheInstance = new ShellPropertyDescriptionsCache();
                }
                return cacheInstance;
            }        
        }

        public ShellPropertyDescription GetPropertyDescription(PropertyKey key)
        {
            if (!propsDictionary.ContainsKey(key))
            {
                propsDictionary.Add(key, new ShellPropertyDescription(key));
            }
            return propsDictionary[key];
        }
    }
}
