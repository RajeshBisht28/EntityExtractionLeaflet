using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace ExtractEntity_TEXT
{
    internal class LF_Pdfprocesing
    {
        public int GetTotalPageCount(string pdfPath)
        {
            var totalpages = 1;
            try
            {
                using (PdfDocument document = PdfDocument.Open(pdfPath))
                {
                    totalpages = document.NumberOfPages;
                }
            }
            catch { }
            return totalpages;
        }
    }
}
