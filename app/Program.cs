using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app
{
    class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddDbContext<AppDbContext>(db => db
                    .UseChangeTrackingProxies()
                    .UseSqlite("Data Source=db.db"));
            });

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();

                if (!await context.A.AnyAsync())
                {
                    var b = context.B.CreateProxy(b => context.B.Add(b));
                    var a = context.A.CreateProxy(a =>
                    {
                        context.A.Add(a);
                        a.B = b;
                    });
                }

                await context.SaveChangesAsync();
            }

            using (var scope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                A a = await context.A.FirstAsync();
                Console.WriteLine(a.B?.Id);
                if (a is INotifyPropertyChanging changing)
                {
                    changing.PropertyChanging += (s, e) => Console.WriteLine($"Changing: {e.PropertyName}");
                }
                if (a is INotifyPropertyChanged changed)
                {
                    changed.PropertyChanged += (s, e) => Console.WriteLine($"Changed: {e.PropertyName}");
                }
                await context.B.LoadAsync();
                Console.WriteLine(a.B?.Id);
                Console.WriteLine("Before Reload");
                await context.Entry(a).ReloadAsync();
                Console.WriteLine("After Reload Reload");
                Console.WriteLine(a.B?.Id);
            }
        }
    }
}
