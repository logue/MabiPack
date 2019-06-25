// MabiPacker
// Copyright (c) 2019 by Logue <http://logue.be/>
// Distributed under the MIT license

using System.Reflection;
using WPFLocalizeExtension.Extensions;

namespace MabiPacker.Library
{
    public static class LocalizationProvider
    {
        public static T GetLocalizedValue<T>(string key)
        {
            return LocExtension.GetLocalizedValue<T>(Assembly.GetCallingAssembly().GetName().Name + ":Resources:" + key);
        }
    }
}
