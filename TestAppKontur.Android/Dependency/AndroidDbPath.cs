using System.IO;
using TestAppKontur.Dependency;
using TestAppKontur.Droid.Dependency;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDbPath))]
namespace TestAppKontur.Droid.Dependency
{
    public class AndroidDbPath : IPath
    {
        public string GetDatabasePath(string filename)
        {
            return Path.Combine(System.Environment.GetFolderPath
                (System.Environment.SpecialFolder.Personal), filename);
        }
    }
}