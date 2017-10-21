using System;
using System.IO;
using Fortuna.Extensions;
using System.Threading.Tasks;

namespace Fortuna.CLI
{
    class Program
    {
        const string SeedFileFolder = "/var/lib/fortuna/";
        const string SeedFilePath = "/var/lib/fortuna/.seed";

        public static async Task Main(string[] args)
        {
            if (!Directory.Exists(SeedFileFolder))
            {
                Directory.CreateDirectory(SeedFileFolder);
            }

            var seedStream = File.Open(SeedFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using (var crng = await PRNGFortunaProviderFactory.CreateWithSeedFileAsync(seedStream))
            {
                await crng.InitializePRNGAsync();

                string input = args.Length > 0 ? args[0] : "100";
                do
                {
                    if (int.TryParse(input, out int range))
                    {
                        Console.WriteLine(crng.RandomNumber(range));
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid integer.");
                    }

                    input = Console.ReadLine();
                } while (input != "exit" && args.Length <= 0);
            }
        }
    }
}
