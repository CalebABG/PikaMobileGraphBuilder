using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using CS253ShortestPath.Helpers;
using SQLite;

namespace CS253ShortestPath.Services
{
    public class LocalDatabase<T>
    {
        private readonly Lazy<SQLiteAsyncConnection> _lazyInitializer =
            new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(Globals.DatabasePath, Constants.Flags));


        private bool _initialized;

        protected LocalDatabase()
        {
            InitializeAsync().SafeFireAndForget(continueOnCapturedContext: false);
        }

        protected SQLiteAsyncConnection Database => _lazyInitializer.Value;

        private async Task InitializeAsync()
        {
            if (!_initialized)
            {
                if (Database.TableMappings.All(m => m.MappedType.Name != typeof(T).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(T))
                        .ConfigureAwait(false);

                    _initialized = true;
                }
            }
        }
    }
}