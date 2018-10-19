Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.Zip
Imports System.IO
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.Configuration.ConfigurationManager



Partial Class main
    Inherits System.Web.UI.Page
    Private auto As New autoletter
    Private chk2 As New Accounts

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Session("name")) Then
            Response.Redirect("default.aspx")

        End If
        Label2.Text = Session("name") & " <b>[" & Session("job") & "]</b>"


        'TextBox2.Text = Session("branch")

    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click

        Try

            Dim rd As New System.Random

            Dim bno As String = Year(Now) & Month(Now) & Day(Now) & Second(Now) & Minute(Now) & Hour(Now) & rd.Next(100, 9999)
            Dim msgbody As String = ""
            Dim fname As String = ""
            Dim oname As String = ""
            Dim rndseed As Integer

            Dim bk As New bank.banks

            'Dim ds As New Data.DataSet
            Dim htmlstr As String = ""

            Dim ref As String = ""


            Dim selected As String = ""

            If RadioButton1.Checked Then
                selected = RadioButton1.Text
            ElseIf RadioButton2.Checked Then
                selected = RadioButton2.Text
            ElseIf RadioButton3.Checked Then
                selected = RadioButton3.Text
            ElseIf RadioButton4.Checked Then
                selected = RadioButton4.Text
            ElseIf RadioButton5.Checked Then

                Exit Sub

            End If






            If selected.Contains("FI") And TextBox2.Text.Trim <> "900" Then

                Label1.ForeColor = Drawing.Color.Red
                Label1.Text = "Selected Template can only be used for branch 900"
                Exit Sub

            End If

            'ds = bk.ReportsPortalDDL(System.IO.File.ReadAllText(Server.MapPath("./scripts/FixedDeposit.txt")).Replace(":Date01", TextBox1.Text.Trim.Replace("-", "")).Replace(":Branch", TextBox2.Text.Trim))
            '  Session("dsEmail") = ds
            ' Dim dt2 As Date = TextBox1.Text
            'Dim dt3 As String = dt2.DayOfWeek
            Dim dt As String = TextBox1.Text.Trim.Replace("-", "")
            Session.Timeout = 1234

            Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())

                'Dim ds As New Data.DataSet
                Dim getCustnum As String = TextBox3.Text.Trim
                Dim da As New Data.SqlClient.SqlDataAdapter

                If String.IsNullOrEmpty(getCustnum) Then

                    'Dim cm As New Data.SqlClient.SqlCommand("select * from T24FDD WHERE CONVERT(varchar, CONVERT(datetime, START_DATE), 100)=@bid", conn)
                    Dim cm As New Data.SqlClient.SqlCommand("select DISTINCT T24FDD2.* from T24FDD2,BranchInfo WHERE T24BRANCHID=CO_CODE AND MATURITY_DATE <> ''  AND (BANKSBRACODE=@BR OR T24BRANCHID=@BR)  AND START_DATE=@bid", conn)
                    cm.Parameters.AddWithValue("@bid", dt)
                    cm.Parameters.AddWithValue("@BR", TextBox2.Text.Trim)
                    da.SelectCommand = cm

                Else
                    Dim cm As New Data.SqlClient.SqlCommand("select DISTINCT T24FDD2.* from T24FDD2,BranchInfo WHERE T24BRANCHID=CO_CODE AND MATURITY_DATE <> ''  AND (BANKSBRACODE=@BR OR T24BRANCHID=@BR)  AND START_DATE=@bid AND CUSTOMER=@cust", conn)
                    cm.Parameters.AddWithValue("@bid", dt)
                    cm.Parameters.AddWithValue("@BR", TextBox2.Text.Trim)
                    cm.Parameters.AddWithValue("@cust", TextBox3.Text.Trim)
                    da.SelectCommand = cm

                End If

               


                Dim ds As New Data.DataSet
                da.Fill(ds)


                If ds.Tables(0).Rows.Count > 0 Then
                    System.IO.Directory.CreateDirectory(AppSettings("out").ToString() & bno)
                    System.IO.Directory.CreateDirectory(AppSettings("out2").ToString() & bno)

                    Dim htmlstr2 As String = ""

                    For Each dr As Data.DataRow In ds.Tables(0).Rows

                        'get ref no
                        '  ref = rd.Next(1000, 9999) & Minute(Now) & Month(Now) & Day(Now) & Hour(Now) & Year(Now) & Second(Now)

                        ref = Convert.ToString(Val(dr("CUSTOMER").ToString) * Val(dr("CO_CODE").ToString) + Val(dr("AMOUNT").ToString.Replace(",", "").Replace(".", ""))).PadRight(15, "0")

                        'get currency
                        Dim cur As String = dr("CURRENCY").ToString.Trim
                        'Select Case dr("CURRENCY").ToString.Trim
                        '    Case "1"
                        '        cur = "NGN"
                        '    Case "2"
                        '        cur = "USD"
                        '    Case "3"
                        '        cur = "GBP"
                        '    Case "46"
                        '        cur = "EUR"
                        '    Case Else
                        '        cur = "NGN"
                        'End Select
                        Dim basis As String = dr("DAY_BASIS").ToString().Trim

                        'get template doc
                        htmlstr = System.IO.File.ReadAllText(Server.MapPath("./templates/" & selected & ".txt"))

                        auto.CreateCountData(dr("CUSTOMER").ToString, 100)

                        Dim t4 As String = Date.ParseExact(dr("MATURITY_DATE").ToString().Trim, "yyyyMMdd", Nothing).DayOfWeek

                        'get account number
                        Dim account As String = dr("LINKED_APPL_ID").ToString
                        Dim getday As String = dr("TERM").ToString.Replace("D", "")
                        Dim getday2 As Integer
                        If t4 = 0 Then
                            getday2 = getday
                        Else
                            getday2 = getday
                        End If
                        Dim getdayval As Integer = CInt(getday2)
                        Dim valueamt As Double = CDbl(dr("AMOUNT"))
                        Dim getrate As Double = (CDbl(dr("EFFECTIVE_RATE")) / 100.0)
                        Dim Intt As Double

                        If basis = "A" Then
                            Intt = (getdayval / 360) * getrate * valueamt
                        Else
                            Intt = (getdayval / 366) * getrate * valueamt
                        End If

                        Dim wht As Double = (10 / 100) * Intt
                        Dim dtt = Date.ParseExact(dr("START_DATE").ToString().Trim, "yyyyMMdd", Nothing)

                        Dim matdtt = Date.ParseExact(dr("MATURITY_DATE").ToString().Trim, "yyyyMMdd", Nothing)
                        Dim valdtt = Date.ParseExact(dr("START_DATE").ToString().Trim, "yyyyMMdd", Nothing)







                        'Append parameters to template html file
                        htmlstr2 = htmlstr.Replace("%date%", CDate(dtt.ToString).ToString("dd-MMM-yyyy")) _
                        .Replace("%name%", dr("SHORT_NAME").ToString & " (" & dr("CUSTOMER") & ") ") _
                         .Replace("%refno%", ref) _
                        .Replace("%address1%", dr("STREET").ToString) _
                                            .Replace("%address2%", dr("TOWN_COUNTRY").ToString) _
                                            .Replace("%amount%", FormatNumber(dr("AMOUNT").ToString, 2)).Replace("%currency%", cur) _
                                            .Replace("%interest%", dr("EFFECTIVE_RATE").ToString & " %") _
                                            .Replace("%days%", getday.ToString & " DAYS") _
                                            .Replace("%effectiveDate%", CDate(dtt.ToString).ToString("dd-MMM-yyyy")) _
                                            .Replace("%MaturityDate%", CDate(matdtt.ToString).ToString("dd-MMM-yyyy")) _
                                            .Replace("%title%", "CONFIRMATION OF TERMS ON " & cur & FormatNumber(dr("AMOUNT").ToString, 2) & " INVESTMENT") _
                                            .Replace("%gross%", FormatNumber(Intt, 2)) _
                                           .Replace("%wht%", FormatNumber(wht, 2)) _
                                           .Replace("%netInterest%", FormatNumber((Intt - wht), 2)) _
                                           .Replace("%matvalue%", FormatNumber((valueamt + (Intt - wht)), 2)) _
                                           .Replace("%account%", dr("CO_CODE").ToString & "/" & account & "/").Replace("%tellerid%", Session("tid")).Replace("%branch%", dr("CO_CODE").ToString)
                        ' .Replace("%refno%", Session("branch").ToString.PadLeft(4, "0") & dr("CUS_NUM").ToString & Session("tid").ToString.PadLeft(4, "0") & auto.getCustomerCount(dr("CUS_NUM").ToString))

                        rndseed = rd.Next(1, 99999)
                        fname = AppSettings("out").ToString() & bno & "\" & dr("SHORT_NAME").ToString.Replace("'", "`").Replace("&", "-").Replace("\", "-").Replace("/", "-").Replace("""", "") & rndseed & ".pdf"
                        oname = AppSettings("out2").ToString() & bno & "\" & dr("SHORT_NAME").ToString.Replace("'", "`").Replace("&", "-").Replace("\", "-").Replace("/", "-").Replace("""", "") & rndseed & ".pdf"

                        'create pdf for each record
                        createPDF(htmlstr2, fname)

                        'add watermark to pdf
                        AddWaterMarktoPdf(fname, oname, ref)



                        'send mail to customer
                        If CheckBox1.Checked Then

                            If Not String.IsNullOrEmpty(dr("EMAIL_1").ToString()) Then
                                Try
                                    msgbody = "Dear " & dr("SHORT_NAME").ToString & "<br/><br/>" & "Find attached a sample of your Fixed Deposit Confirmation letter.<br/>Please note your reference number for future verification.<br/><br/><b>Letter Reference No:</b> " & ref & "<br/><br/>Sterling Bank Plc"


                                    auto.sendmail(dr("EMAIL_1").ToString, "ebusiness@sterlingbankng.com", "TIME DEPOSIT LETTER FROM STERLING BANK", msgbody, oname)


                                Catch ex2 As Exception
                                    System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex2.Message & vbCrLf)

                                End Try
                            End If
                        End If

                        'Send SMS to customer
                        If CheckBox2.Checked Then
                            If Not String.IsNullOrEmpty(dr("PHONE_1").ToString()) Then
                                Try
                                    msgbody = "Dear Customer, Your Fixed Deposit letter is now ready.%0APlease call customer care on 01-4484481-5 for more info.%0ASterling Bank Plc"
                                    auto.sendSMS(dr("PHONE_1").ToString, msgbody)
                                Catch ex2 As Exception
                                    System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex2.Message & vbCrLf)

                                End Try
                            End If
                        End If

                        're-initialise htmlstr
                        htmlstr = ""
                        htmlstr2 = ""

                        'log letters based on Val date on the Letter as Filename
                        System.IO.File.AppendAllText(AppSettings("log").ToString() & CDate(valdtt.ToString).ToString("dd-MMM-yyyy") & ".txt", Now & ": " & Session("name") & " generated a letter with reference Number: " & ref & " from PC IP Number:" & Request.UserHostAddress.ToString & vbCrLf)

                    Next


                    'create ZIP file
                    CreateZip(AppSettings("out").ToString() & bno & ".zip", "", AppSettings("out2").ToString() & bno)

                    Label1.ForeColor = Drawing.Color.Green
                    Label1.Text = "Letters generated. Download ZIP folder for all letter(s) <a href='http://" & AppSettings("serverIP").ToString & "/autoletterZip/" & bno & ".zip'>HERE</a>"


                Else
                    Label1.Text = "No letters found for this date"

                End If

            End Using


        Catch ex As Exception


            Label1.ForeColor = Drawing.Color.Red
            Label1.Text = "An Error occured." & ex.Message
            If ex.InnerException IsNot Nothing Then
                Label1.Text &= " Addtional Error Messages: " & ex.InnerException.Message
            End If
            System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex.Message & vbCrLf)
        End Try




    End Sub

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



    End Sub

    ' Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
    '
    Public Sub CreateZip(outPathname As String, password As String, folderName As String)

        Dim fsOut As FileStream = File.Create(outPathname)
        Dim zipStream As New ZipOutputStream(fsOut)

        zipStream.SetLevel(3)       '0-9, 9 being the highest level of compression
        zipStream.Password = password   ' optional. Null is the same as not setting.

        ' This setting will strip the leading part of the folder path in the entries, to
        ' make the entries relative to the starting folder.
        ' To include the full path for each entry up to the drive root, assign folderOffset = 0.
        Dim folderOffset As Integer = folderName.Length + (If(folderName.EndsWith("\"), 0, 1))

        CompressFolder(folderName, zipStream, folderOffset)

        zipStream.IsStreamOwner = True
        ' Makes the Close also Close the underlying stream
        zipStream.Close()
    End Sub

    ' Recurses down the folder structure
    '
    Private Sub CompressFolder(path As String, zipStream As ZipOutputStream, folderOffset As Integer)

        Dim files As String() = Directory.GetFiles(path)

        For Each filename As String In files

            Dim fi As New FileInfo(filename)

            Dim entryName As String = filename.Substring(folderOffset)  ' Makes the name in zip based on the folder
            entryName = ZipEntry.CleanName(entryName)       ' Removes drive from name and fixes slash direction
            Dim newEntry As New ZipEntry(entryName)
            newEntry.DateTime = fi.LastWriteTime            ' Note the zip format stores 2 second granularity

            ' Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
            '   newEntry.AESKeySize = 256;

            ' To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
            ' you need to do one of the following: Specify UseZip64.Off, or set the Size.
            ' If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
            ' but the zip will be in Zip64 format which not all utilities can understand.
            '   zipStream.UseZip64 = UseZip64.Off;
            newEntry.Size = fi.Length

            zipStream.PutNextEntry(newEntry)

            ' Zip the file in buffered chunks
            ' the "using" will close the stream even if an exception occurs
            Dim buffer As Byte() = New Byte(4095) {}
            Using streamReader As FileStream = File.OpenRead(filename)
                StreamUtils.Copy(streamReader, zipStream, buffer)
            End Using
            zipStream.CloseEntry()
        Next
        Dim folders As String() = Directory.GetDirectories(path)
        For Each folder As String In folders
            CompressFolder(folder, zipStream, folderOffset)
        Next
    End Sub

    Sub createPDF(htmlstr As String, pdfname As String)
        ' Read html file to a string
        'Dim sr As StreamReader = New StreamReader("YOUR HTML FILE PATH WIIH NAME")
        Dim line As String = htmlstr


        'line = sr.ReadToEnd
        'sr.Close()

        ' Code to convert to pdf
        Dim doc As New Document(PageSize.LETTER, 80, 50, 30, 65)
        Dim fsNew As New StringReader(line)
        Dim document As New Document(PageSize.A4, 80, 50, 30, 65)


        Using fs As New FileStream(pdfname, FileMode.Create)
            PdfWriter.GetInstance(document, fs)
            Using stringReader As New StringReader(line)
                Dim parsedList As System.Collections.Generic.List(Of IElement) = html.simpleparser.HTMLWorker.ParseToList(stringReader, Nothing)
                document.Open()
                ' parse each html object and add it to the pdf document


                For Each item As Object In parsedList
                    document.Add(DirectCast(item, IElement))
                Next

                document.Close()

            End Using

        End Using


    End Sub



End Class
