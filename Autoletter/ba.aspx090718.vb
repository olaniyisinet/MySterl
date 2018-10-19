Imports ICSharpCode.SharpZipLib.Core
Imports ICSharpCode.SharpZipLib.Zip
Imports System.IO
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.Configuration.ConfigurationManager


Partial Class ba
    Inherits System.Web.UI.Page
    Private auto As New autoletter
    Private bno As String = ""
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
            Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                Dim rd As New System.Random
                Dim dt As String = TextBox1.Text.Trim.Replace("-", "")


                bno = Year(Now) & Month(Now) & Day(Now) & Second(Now) & Minute(Now) & Hour(Now) & rd.Next(100, 9999)
                Session("bno") = bno
                Session.Timeout = 900
                Dim bk As New bank.banks


                Dim da As New Data.SqlClient.SqlDataAdapter
                'Dim cmd As New Data.SqlClient.SqlCommand

                'cmd.CommandText = "select * from BA_Discounted where AGREEMENT_DATE=@agreementdate and CO_CODE = @coycode"
                Dim cmd As New Data.SqlClient.SqlCommand("select * from BA_Discounted where AGREEMENT_DATE=@agreementdate and CO_CODE = @coycode", conn)
                cmd.Parameters.AddWithValue("@agreementdate", dt)
                cmd.Parameters.AddWithValue("@coycode", TextBox2.Text.Trim())

                da.SelectCommand = cmd
                Dim ds As New Data.DataSet
                da.Fill(ds)
                cmd.Parameters.Clear()


                ' ds = bk.ReportsPortalDDL(System.IO.File.ReadAllText(Server.MapPath("./scripts/BA.txt")).Replace(":Date01", TextBox1.Text.Trim.Replace("-", "")).Replace(":Branch", TextBox2.Text.Trim))

                If ds.Tables(0).Rows.Count > 0 Then
                    For Each dr As Data.DataRow In ds.Tables(0).Rows
                        'auto.CreateBAInfo(dr("BRA_CODE").ToString, dr("CUS_NUM").ToString, dr("CUR_CODE").ToString, dr("LED_CODE").ToString, dr("SUB_ACCT_CODE").ToString, dr("BUS_AMT").ToString, dr("DIS_AMT").ToString, dr("VAL_DATE").ToString, dr("MAT_DATE").ToString, dr("GRA_DAYS").ToString, dr("INT_RATE").ToString, dr("CUS_SHO_NAME").ToString, dr("ADD_LINE1").ToString, dr("ADD_LINE2").ToString, dr("EMAIL").ToString, dr("MOB_NUM").ToString, dr("ALT_CUR_CODE").ToString, dr("CURRENCY").ToString, "", bno)
                        auto.CreateBAInfo(dr("CO_CODE").ToString, dr("CUSTOMER_ID").ToString, dr("CURRENCY").ToString, dr("CATEGORY").ToString, "", dr("AMOUNT").ToString, Convert.ToString(Val(dr("AMOUNT").ToString) - Val(dr("TOT_INTEREST_AMT").ToString)), dr("VALUE_DATE").ToString, dr("FIN_MAT_DATE").ToString, "0", dr("INTEREST_RATE").ToString, dr("SHORT_NAME").ToString, dr("STREET").ToString, dr("ADDRESS").ToString, dr("EMAIL_1").ToString, 0, dr("PRODCCY").ToString, dr("CURRENCY").ToString, "", bno)

                    Next

                    'show temp data from DB -BA table to gridview
                    GridView1.DataSource = auto.getBA(bno)
                    GridView1.DataBind()
                    Label1.ForeColor = Drawing.Color.Green

                    Label1.Text = "Records generated. Edit each field to enter Issuer Name"
                    Button2.Visible = True



                Else
                    Label1.Text = "No letters found for this date"

                End If

            End Using

        Catch ex As Exception


            Label1.ForeColor = Drawing.Color.Red
            Label1.Text = Err.Description '"An Error occured."
            System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex.Message & vbCrLf)
        End Try

    End Sub

    Sub BankersAcceptance()

        Try

            Dim ds As New Data.DataSet
            Dim rd As New System.Random
            Dim fname As String = ""
            Dim oname As String = ""
            Dim rndseed As Integer
            Dim msgbody As String = ""


            Dim bk As New bank.banks
            Dim ref As String = ""

            Dim htmlstr As String = ""


            ' ds = bk.ReportsPortalDDL(System.IO.File.ReadAllText(Server.MapPath("./scripts/BA.txt")).Replace(":Date01", TextBox1.Text.Trim.Replace("-", "")).Replace(":Branch", TextBox2.Text.Trim))
            ds = auto.getBA(Session("bno"))
            If ds.Tables(0).Rows.Count > 0 Then
                System.IO.Directory.CreateDirectory(AppSettings("out").ToString() & Session("bno"))
                System.IO.Directory.CreateDirectory(AppSettings("out2").ToString() & Session("bno"))

                Dim htmlstr2 As String = ""

                For Each dr As Data.DataRow In ds.Tables(0).Rows

                    'get ref no
                    '  ref = rd.Next(1000, 9999) & Minute(Now) & Month(Now) & Day(Now) & Hour(Now) & Year(Now) & Second(Now)


                    ref = Convert.ToString(Val(dr("CUS_NUM").ToString) * Val(dr("bra_code").ToString) + Val(dr("bus_amt").ToString.Replace(",", "").Replace(".", ""))).PadRight(15, "0")



                    'get currency
                    Dim cur As String = ""
                    Select Case dr("CUR_CODE").ToString.Trim
                        Case "NGN"
                            cur = "NGN"
                        Case "USD"
                            cur = "USD"
                        Case "GBP"
                            cur = "GBP"
                        Case "EUR"
                            cur = "EUR"
                        Case Else
                            cur = "NGN"
                    End Select


                    'get template doc
                    htmlstr = System.IO.File.ReadAllText(Server.MapPath("./templates/Bankers Acceptance.txt"))

                    auto.CreateCountData(dr("CUS_NUM").ToString, 100)

                    Dim finmatdate As Date = Left(dr("MAT_DATE"), 4) & "-" & Mid(dr("MAT_DATE"), 5, 2) & "-" & Right(dr("MAT_DATE"), 2)
                    Dim valuedate As Date = Left(dr("VAL_DATE"), 4) & "-" & Mid(dr("VAL_DATE"), 5, 2) & "-" & Right(dr("VAL_DATE"), 2)
                    Dim thetenor As Int32 = finmatdate.Subtract(valuedate).Days


                    Dim matda As String = Left(dr("MAT_DATE"), 4) & "-" & Mid(dr("MAT_DATE"), 5, 2) & "-" & Right(dr("MAT_DATE"), 2)


                    Dim valda As String = Left(dr("VAL_DATE"), 4) & "-" & Mid(dr("VAL_DATE"), 5, 2) & "-" & Right(dr("VAL_DATE"), 2)


                    'Append parameters to template html file
                    htmlstr2 = htmlstr.Replace("%date%", valda) _
                    .Replace("%name%", dr("CUS_SHO_NAME").ToString).Replace("%issuer%", dr("ISSUER_NAME").ToString) _
                    .Replace("%refno%", ref) _
                    .Replace("%address1%", dr("ADD_LINE1").ToString) _
                    .Replace("%address2%", dr("ADD_LINE2").ToString) _
                    .Replace("%amount%", FormatNumber(dr("BUS_AMT").ToString, 2)).Replace("%currency%", cur) _
                    .Replace("%disvalue%", FormatNumber(dr("DIS_AMT").ToString, 2)) _
                    .Replace("%tenor%", thetenor.ToString) _
                    .Replace("%rate%", dr("INT_RATE").ToString) _
                    .Replace("%matdate%", matda) _
                    .Replace("%valdate%", valda) _
                    .Replace("%title%", "CONFIRMATION OF TERMS ON " & cur & FormatNumber(dr("BUS_AMT").ToString, 2) & " INVESTMENT").Replace("%tellerid%", Session("tid")).Replace("%branch%", Session("branch"))

                    '.Replace("%refno%", Session("branch").ToString.PadLeft(4, "0") & dr("CUS_NUM").ToString & Session("tid").ToString.PadLeft(4, "0") & auto.getCustomerCount(dr("CUS_NUM").ToString)) _
                    '20160616



                    rndseed = rd.Next(1, 99999)

                    fname = AppSettings("out").ToString() & Session("bno") & "\" & dr("CUS_SHO_NAME").ToString.Replace("'", "`").Replace("&", "-").Replace("\", "-").Replace("/", "-") & rndseed & ".pdf"
                    oname = AppSettings("out2").ToString() & Session("bno") & "\" & dr("CUS_SHO_NAME").ToString.Replace("'", "`").Replace("&", "-").Replace("\", "-").Replace("/", "-") & rndseed & ".pdf"

                    'create pdf for each record
                    createPDF(htmlstr2, fname)


                    'add watermark to pdf
                    AddWaterMarktoPdf(fname, oname, ref)



                    'send mail to customer
                    If CheckBox1.Checked Then
                        If Not String.IsNullOrEmpty(dr("EMAIL").ToString()) Then
                            Try
                                msgbody = "Dear " & dr("CUS_SHO_NAME").ToString & "<br/><br/>" & "Find attached a sample of your Bankers Acceptance Confirmation letter.<br/>Please note your reference number for future verification.<br/><br/><b>Letter Reference No:</b> " & ref & "<br/><br/>Sterling Bank Plc"
                                auto.sendmail(dr("EMAIL").ToString, "ebusiness@sterlingbankng.com", "TIME DEPOSIT LETTER FROM STERLING BANK", msgbody, oname)
                            Catch ex2 As Exception
                                System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex2.Message & vbCrLf)

                            End Try

                        End If
                    End If

                    'Send SMS to customer
                    'If CheckBox2.Checked Then
                    '    If Not String.IsNullOrEmpty(dr("PHONE_NUMBER").ToString()) Then
                    '        Try
                    '            msgbody = "Dear Customer, Your Bankers Acceptance Confirmation letter is now ready.%0APlease call customer care on 01-4484481-5 for more info.%0ASterling Bank Plc"
                    '            auto.sendSMS(dr("PHONE_NUMBER").ToString, msgbody)
                    '        Catch ex2 As Exception
                    '            System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex2.Message & vbCrLf)

                    '        End Try
                    '    End If
                    'End If

                    're-initialise htmlstr
                    htmlstr = ""
                    htmlstr2 = ""
                    Dim dt1 As String = dr("VAL_DATE").ToString()
                    
                    Dim dt As Date = Date.ParseExact(dt1, "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)

                    Dim dtx As String = dt.ToString("dd-MMM-yyyy")

                    'log letters based on Val date on the Letter as Filename
                    System.IO.File.AppendAllText(AppSettings("log").ToString() & dtx & ".txt", Now & ": " & Session("name") & " generated a letter with reference Number: " & ref & " from PC IP Number:" & Request.UserHostAddress.ToString & vbCrLf)



                Next

                'flush temp data
                auto.DeleteBA(Session("bno"))

                'create ZIP file
                CreateZip(AppSettings("out").ToString() & Session("bno") & ".zip", "", AppSettings("out2").ToString() & Session("bno"))

                Label1.ForeColor = Drawing.Color.Green
                Label1.Text = "Letters generated. Download ZIP folder for all letter(s) <a href='http://" & AppSettings("serverIP").ToString & "/autoletterZip/" & Session("bno") & ".zip'>HERE</a>"


            Else
                Label1.Text = "No letters found for this date"

            End If



        Catch ex As Exception


            Label1.ForeColor = Drawing.Color.Red
            Label1.Text = "An Error occured."
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

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        bind()
        Button3.Focus()

    End Sub

    Protected Sub GridView1_RowEditing(sender As Object, e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        GridView1.DataSource = auto.getBA(Session("bno"))

        GridView1.DataBind()
        Button3.Focus()



    End Sub

    Sub bind()
        GridView1.DataSource = auto.getBA(Session("bno"))

        GridView1.DataBind()

    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim tn As New TextBox
        Dim gr As GridViewRow
        gr = GridView1.Rows(e.RowIndex)
        tn = gr.FindControl("issuerNameTxt")
        auto.UpdateBA(GridView1.DataKeys(e.RowIndex).Value, tn.Text)

        GridView1.EditIndex = -1
        bind()
        Button3.Focus()

    End Sub

    Protected Sub Button2_Click(sender As Object, e As System.EventArgs) Handles Button2.Click

        If auto.CheckIssuerName(Session("bno")) = True Then

            BankersAcceptance()
        Else

            Label1.ForeColor = Drawing.Color.Red
            Label1.Text = "Please enter ISSUER NAME for all the records before you continue"

        End If


    End Sub

End Class
