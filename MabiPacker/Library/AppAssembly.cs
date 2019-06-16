using System.IO;
using System.Reflection;

namespace MabiPacker.Library
{
    /// <summary>
    /// バージョン情報
    /// </summary>
    public class AppAssembly
    {
        private readonly Assembly _assambly;
        public AppAssembly()
        {
            _assambly = Assembly.GetExecutingAssembly();
        }
        public string Title
        {
            get
            {
                object[] attributes = _assambly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(_assambly.CodeBase);
            }
        }
        public string Version => _assambly.GetName().Version.ToString();
        public string Description
        {
            get
            {
                object[] attributes = _assambly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        public string Product
        {
            get
            {
                object[] attributes = _assambly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        public string Copyright
        {
            get
            {
                object[] attributes = _assambly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        public string Company
        {
            get
            {
                object[] attributes = _assambly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
    }
}