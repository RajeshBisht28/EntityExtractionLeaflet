using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ExtractEntity_TEXT
{
    public class Program
    {
        static void Main(string[] args)
        {
            var infile = @"E:\DELETES_9000\amy40\amy40.pdf"; // paulaData\paulaData\bbb.pdf";
#if DEBUG
            //LoggerWrite.ReadAndFilterFile(@"C:\Users\hp\Downloads\txtfile\100.txt");

#else
            infile = args[0];
#endif
            var dirPath = Path.GetDirectoryName(infile);
            var newDir = Path.Combine(dirPath, "entity_ext");
            var exeDir = GetExeDirectoryPath();
            //var infoPath = Path.Combine(Path.GetDirectoryName(infile), "page.txt");
            var logpath = Path.Combine(Path.GetDirectoryName(infile), "log.txt");
            var obj = new TextExtractionPDF();
            try
            {
                obj.TextExtractionProcess(exeDir, infile, logpath);
            }
            catch { }

            var resp = SetupFolders(infile);
            try
            {
                obj.ExtractEntity(resp.pythonFilepath, resp.pythonExe, resp.textFileDir, resp.entityjsonPath, resp.outputResultFilePath);
            }
            catch { }
        }
        private static (string textFileDir, string outputResultFilePath, string entityjsonPath, string pythonFilepath, string pythonExe) SetupFolders(string filepath)
        {
            var dirPath = Path.GetDirectoryName(filepath);
            var infojson = Path.Combine(dirPath, "entity_info.json");
            var extDirectory = Path.Combine(dirPath, "entity_ext");
            var exeDir = GetExeDirectoryPath();
            var pythonFile = Path.Combine(exeDir, @"EntityExt\source\startup.py");
            var pythonExePath = Path.Combine(exeDir, @"EntityExt\leaflet_ent\Scripts\python.exe");
            if (!Directory.Exists(extDirectory))
            {
                Directory.CreateDirectory(extDirectory);
            }
            var resultFilePath = Path.Combine(extDirectory, "result.json");
            var entityjsonPath = Path.Combine(extDirectory, "entity_info.json");
           if (File.Exists(infojson))
            {
                File.Copy(infojson, entityjsonPath, true);
            }
            return (extDirectory, resultFilePath, entityjsonPath, pythonFile, pythonExePath);
        }
        public static string GetExeDirectoryPath()
        {
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string executableDirectory = Path.GetDirectoryName(executablePath);
            return executableDirectory;
        }
    }
}
