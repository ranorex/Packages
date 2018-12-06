using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Applitools.Images;
using PdfiumViewer;

namespace Applitools.ImageTester.TestObjects
{
    public class PDFTest : Test
    {
        private float dpi;
        private string pdfPassword;
        private List<int> pagesList;
        private string pages;

        public static string[] SupportedExtensionsWithSeparator { get; } = new[] { ".pdf" };

        public virtual void SetPages(string pages)
        {
            this.pages = pages;
            this.pagesList = SetPagesList(pages);
        }

        protected PDFTest(FileStream file, string appname) : this(file, appname, 300F)
        {
        }

        public PDFTest(FileStream file, string appname, float dpi) : base(file, appname)
        {
            this.dpi = dpi;
        }

        public override void Run(Eyes eyes)
        {
            var pdfDocument = PdfDocument.Load(this.file, pdfPassword);

            for (int i = 0; i < pagesList.Count; i++)
            {
                var bim = pdfDocument.Render(pagesList[i] - 1, this.dpi, this.dpi, false);
                eyes.CheckImage(new Bitmap(bim), string.Format("Page-{0}", pagesList[i]));
            }

        }

        public static bool Supports(FileStream file)
        {
            return SupportedExtensionsWithSeparator
                .AsParallel()
                .Any(e => file.Name.EndsWith(e));
        }

        protected virtual void SetDpi(float dpi)
        {
            this.dpi = dpi;
        }

        public virtual string GetPdfPassword()
        {
            return pdfPassword;
        }

        public virtual void SetPdfPassword(string pdfPassword)
        {
            this.pdfPassword = pdfPassword;
        }

        public virtual List<int> SetPagesList(string pages)
        {
            if (pages != null)
            {
                return ParsePagesToList(pages);
            }
            else
            {
                var pdfDocument = PdfDocument.Load(this.file, pdfPassword);
                var list = new List<int>();
                for (int page = 0; page < pdfDocument.PageCount; ++page)
                {
                    list.Add(page + 1);
                }

                return list;
            }
        }

        public override string Name()
        {
            string pagesText = "";
            if (pages != null)
            {
                pagesText = " pages [" + pages + "]";
            }

            return file == null ? name + pagesText : file.Name + pagesText;
        }
    }
}