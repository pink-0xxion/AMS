using Azure.Core;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AMS.Services
{
    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        //public byte[] GeneratePdf(string pageUrl)
        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            /*  var doc = new HtmlToPdfDocument //  is a model class that lets you define the settings for your PDF (like size, margins, URL to convert, etc).
             {
                 GlobalSettings = {
                 PaperSize = PaperKind.A4,
                 Orientation = Orientation.Portrait,
                 Margins = new MarginSettings { Top = 10, Bottom = 20 }
                 },
                 Objects = {
                 new ObjectSettings {
                 Page = pageUrl,
                 FooterSettings = {
                     FontSize = 12,
                     Center = "Signature: ____________________________",
                     Line = true
                 }
             }
         }
             };

             return _converter.Convert(doc); // Convert() takes the document with all its settings and creates a PDF as byte[].
          */


            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10, Bottom = 20 }
                },
                Objects = {
                new ObjectSettings {
                HtmlContent = htmlContent,
                    FooterSettings = {
                        FontSize = 12,
                        Center = "Signature: ____________________________",
                        Line = true
                    }
                }
                }
            };

            return _converter.Convert(doc);

        }


    }
}
