Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Partial Class pdf
    Inherits System.Web.UI.Page

    Sub AddWaterMarktoPdf(Inputpath As String, OutputPath As String, watermarkText As String)
        Dim pdfReader As New iTextSharp.text.pdf.PdfReader(Inputpath)
        'create stream of filestream or memorystream etc. to create output file

        Dim stream As New FileStream(OutputPath, FileMode.OpenOrCreate)
        'create pdfstamper object which is used to add addtional content to source pdf file
        Dim pdfStamper As New iTextSharp.text.pdf.PdfStamper(pdfReader, stream)
        'iterate through all pages in source pdf
        For pageIndex As Integer = 1 To pdfReader.NumberOfPages
            'Rectangle class in iText represent geomatric representation... in this case, rectanle object would contain page geomatry
            Dim pageRectangle As Rectangle = pdfReader.GetPageSizeWithRotation(pageIndex)
            'pdfcontentbyte object contains graphics and text content of page returned by pdfstamper
            Dim pdfData As PdfContentByte = pdfStamper.GetUnderContent(pageIndex)
            'create fontsize for watermark
            pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 40)
            'create new graphics state and assign opacity
            Dim graphicsState As New PdfGState()
            graphicsState.FillOpacity = 0.4F
            'set graphics state to pdfcontentbyte
            pdfData.SetGState(graphicsState)
            'set color of watermark
            pdfData.SetColorFill(BaseColor.LIGHT_GRAY)


            'indicates start of writing of text
            pdfData.BeginText()
            'show text as per position and rotation
            pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, pageRectangle.Width / 2, pageRectangle.Height / 2, 45)
            'call endText to invalid font set
            pdfData.EndText()
        Next
        'close stamper and output filestream
        pdfStamper.Close()
        stream.Close()

        Response.Write("mumu")


        
    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        AddWaterMarktoPdf("d:\liveapps\autoletter\input.pdf", "d:\liveapps\autoletter\output.pdf", "NDEWO")

    End Sub
End Class
