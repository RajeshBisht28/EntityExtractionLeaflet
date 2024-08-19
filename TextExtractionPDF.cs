using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Graphics;

namespace ExtractEntity_TEXT
{
    public class TextExtractionPDF
    {
        public List<string> TextExtractionProcess(string exeDir,string pdfPath, string logpath)
        {
           
            LoggerWrite.LogPath = logpath;
            LoggerWrite.Logger("=======  Process Start ALL PAGES 17 AUG 2024 -2.2.14- Fixed header-footer count> ========");

            var objLFpdf = new LF_Pdfprocesing();
            var totpg = objLFpdf.GetTotalPageCount(pdfPath);
            var pglist = new List<string>();
            for (int i = 1; i <= totpg; i++)
            {
                pglist.Add(i.ToString());
            }
            var txt = string.Join(",", pglist.ToArray());
            var dirPath = Path.GetDirectoryName(pdfPath);
            var pagNo = txt.Trim().Split(',').ToList();
            var pyPdfWordFile = @"pythonfile\fitz_word2.py";
            PdfToWordTextExtraction(pdfPath, pagNo, exeDir, pyPdfWordFile);
            LoggerWrite.Logger("============Text Extraction has been completed====================");
            return null;
        }

        public void ExtractEntity(string pyFileName, string pythonExePath, string textfileDir, string entityInfoJsonPath, string outputFilePath)
        {
            LoggerWrite.Logger("========== Extraction Started ================");
            LoggerWrite.Logger($"pythonExe: {pythonExePath}");
            LoggerWrite.Logger($"python File Name: {pyFileName}");
            LoggerWrite.Logger($"Directory input files: {textfileDir}");
            LoggerWrite.Logger($"Entity Json path: {entityInfoJsonPath}");
            LoggerWrite.Logger($"output file: {outputFilePath}");

            var args = $"{pythonExePath} {pyFileName} \u0022{textfileDir}\u0022 \u0022{entityInfoJsonPath}\u0022 \"{outputFilePath}\"";
            LoggerWrite.Logger($"Command: {args}");
            var eCode = RunCommandSynch(args);
            LoggerWrite.Logger("========== Extraction Completed =================");
        }
        private string SplitPdf(string dirPath, string fileName, string pageNo)
        {
            var outfile = Path.Combine(dirPath, $"split_{pageNo}.pdf"); //"pdftk document.pdf cat 10 output page_10.pdf"
            //var args = $"pdftk {fileName} burst output {outfile}";
            var args = $"pdftk {fileName} cat {pageNo} output {outfile}";
            var eCode = RunCommandSynch(args);
            return eCode;
        }
        public void PdfToWordTextExtraction(string pdfPath, List<string> pageNumList, string exeDir, string pyfile)
        {
            LoggerWrite.Logger("===============PDF Word Extraction:Start =========");
            var dirPath = Path.GetDirectoryName(pdfPath);
            var newDir = Path.Combine(dirPath, "entity_ext");
            if(!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            var pyPath = Path.Combine(exeDir, pyfile);
            // var fileList = new List<string>();
            var txtFileList = new List<string>();
            Console.WriteLine($"Inside... PDFF WOrd EXtraction..");
            Console.WriteLine($"Python File path: {pyfile}");
            var stb = new StringBuilder();
            try
            {
                foreach (var p in pageNumList)
                {
                    var txtFile = Path.Combine(newDir, $"org_{p}.txt");
                    try
                    {
                        SplitPdf(dirPath, pdfPath, p.ToString());
                        var splitPdfPath = Path.Combine(dirPath, $"split_{p}.pdf");
                        if (File.Exists(txtFile))
                        {
                            File.Delete(txtFile);
                        }
                        txtFileList.Add(txtFile);
                        stb.Append($" org_{p}.txt");
                        var args = $"python {pyPath} \u0022{splitPdfPath}\u0022 \u0022{txtFile}\u0022";
                        var eCode = RunCommandSynch(args);
                    }
                    catch (Exception exp)
                    {
                        LoggerWrite.Logger($"Exception: PdffToWordTextExtraction- page no: {p} : {exp}");
                        if (File.Exists(txtFile))
                        {
                            File.Delete(txtFile);
                        }
                        File.WriteAllText(txtFile, " ");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrite.Logger($"EXP:PdfToWordTextExtraction {ex.Message}");
            }
            finally
            {
                LoggerWrite.Logger($"Finally:PdfToWordTextExtraction- {stb}");
            }
        }
        private string RunCommandSynch(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            // command = $"/c {command}";
            // Start the process
            using (Process process = Process.Start(psi))
            {
                // Write the command to the process input
                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                return process.ExitCode.ToString();
            }

        }
    }
}
