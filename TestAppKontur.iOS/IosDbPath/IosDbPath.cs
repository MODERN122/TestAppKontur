using System;
using System.IO;
using TestAppKontur.Dependency;
using TestAppKontur.iOS.IosDbPath;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosDbPath))]
namespace TestAppKontur.iOS.IosDbPath
{
    public class IosDbPath : IPath
    {
        public string GetDatabasePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "..", "Library", filename);
        }
    }
}