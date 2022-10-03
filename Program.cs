using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace CopyFiles
{
    class Program
    {
        const string inputFileName = "CopyFiles.json";
        private const string backupSuffix = "_backup";
        static void Main(string[] args)
        {
            var cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var inputFilePath = Path.Combine(cwd, inputFileName);
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Input file not found");
                Exit();
            }
            var input = JsonConvert.DeserializeObject<CopyFiles>(File.ReadAllText(inputFilePath));

            if (input?.Batch == null || input.Batch.Length == 0)
            {
                Console.WriteLine("Invalid input");
                Exit();
            }

            var restoreKeys = new string[] {"r", "-r", "--r", "/r", "restore", "-restore", "--restore", "/restore" };
            bool restoreFromBackup = args.Length > 0 &&
                                     restoreKeys.Any(x =>
                                         string.Equals(x, args[0], StringComparison.OrdinalIgnoreCase));

            if (!restoreFromBackup)
            {
                Console.WriteLine("  == Copy ==  ");
                CopyFilesFromSource(input);
            }
            else
            {
                Console.WriteLine("  == Restore ==  ");
                RestoreTargetFilesFromBackup(input);
            }

            Exit();
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void CopyFilesFromSource(CopyFiles input)
        {
            foreach (var batch in input.Batch)
            {
                Console.WriteLine($"Copying files from {batch.Source} to {batch.Target}");
                foreach (var file in batch.Files)
                {
                    try
                    {
                        if (input.CreateBackup)
                        {
                            File.Copy(Path.Combine(batch.Target, file), Path.Combine(batch.Target, file + backupSuffix), true);
                        }
                        File.Copy(Path.Combine(batch.Source, file), Path.Combine(batch.Target, file), true);
                        Console.WriteLine($"Copy over for: {file}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Failed to copy file: {file}");
                    }
                }
            }
        }

        private static void RestoreTargetFilesFromBackup(CopyFiles input)
        {
            foreach (var batch in input.Batch)
            {
                Console.WriteLine($"Restore files from {batch.Target}");
                foreach (var file in batch.Files)
                {
                    try
                    {
                        if (File.Exists(Path.Combine(batch.Target, file + backupSuffix)))
                        {
                            File.Copy(Path.Combine(batch.Target, file + backupSuffix), Path.Combine(batch.Target, file), true);
                            Console.WriteLine($"Restore over for: {file}");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Failed to restore file: {file}");
                    }
                }
            }
        }
    }
}
