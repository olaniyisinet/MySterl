

Partial Class main
    Inherits System.Web.UI.Page
    Private bno As String
    Private extra As String = ""

    Private chk2 As New bulktra
    Private enc As New CyclopsDES

    Private chk1 As String = ""
    Private chk11 As String = ""
    Private chk12 As String = ""
    Private chk13 As String = ""
    Private chk14 As String = ""


    Private i As Integer = 0
    Private j As Integer = 0
    Private b As Integer = 0
    Private bnk As New bank.bank
    Private chk As New bulktra
    Private conn2 As New Data.SqlClient.SqlConnection(chk.sqlconn())

    Public Function sqlconn() As String
        'Return "user id=bulktra;password=(45*tra#;initial catalog=bulktraDBT24;data source=10.0.0.204,1490"
        Return "user id=sa;password=tylent;initial catalog=bulktraDB;data source=10.0.41.101"
    End Function
    Function getname(cusnum As String, bracode As String) As String

        Dim chck As New SBPSwitch.sbpswitch
        'Dim ac As New Data.DataSet
        Dim ac As New Integer

        Dim bc As String

        ac = chck.getInternalBankID(cusnum)

        If Not cusnum = "0" Then

            If ac <> 0 Then
                bc = Convert.ToString(ac)
                'Return bc

            Else

                Return ""

            End If
        End If

        If bc = "1" Then

            'Dim bankchk As New NewBanks.banks

            'Dim bankchk As New Ttwenfour.banks
            Dim bankchk As New NewBanks.banks

            Dim ds As New Data.DataSet

            'ds = bankchk.getCustomrInfo(cusnum)
            ds = bankchk.getAccountFullInfo(cusnum)

            Try
                getname = ds.Tables(0).Rows(0).Item("CUS_SHO_NAME").ToString
            Catch ex As Exception

            End Try

            Return getname

            'Return bnk.DATCONV_getCustomerName(cusnum, bracode)

        Else
            Dim bankchk As New NewImal.Service

            Dim ds As New Data.DataSet

            ds = bankchk.GetAccountByAccountNumber(cusnum)
            Try
                getname = ds.Tables(0).Rows(0).Item("CUSTOMERNAME").ToString

            Catch ex As Exception

            End Try
            Return getname

        End If

    End Function

    Public Function getAccountFromNUBAN(ByVal nuban As String) As String
        Dim ds As New Data.DataSet()

        'Using con As New System.Data.OracleClient.OracleConnection("Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.0.0.130)(PORT = 1590)))(CONNECT_DATA =(SERVICE_NAME = hobank)));Persist Security Info=True;User ID=transapp;Password=yemichigordayo140210;pooling=true;Max Pool Size=900")
        Using con As New System.Data.OracleClient.OracleConnection("Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.0.0.209)(PORT = 1390)))(CONNECT_DATA =(SERVICE_NAME = stdb)));Persist Security Info=True;User ID=transapp;Password=Transapp#123;pooling=true;Max Pool Size=900")

            con.Open()

            'connect to banks and get account information
            Dim sql As String = "select * from map_acct where map_acc_no = :nuban"
            Dim cmd As New Data.OracleClient.OracleCommand
            cmd.Parameters.AddWithValue(":nuban", nuban)
            cmd.Connection = con
            Dim da As New Data.OracleClient.OracleDataAdapter
            da.SelectCommand = cmd
            da.Fill(ds)
        End Using

        If ds.Tables(0).Rows.Count > 0 Then
            Dim bc As String = Convert.ToString(ds.Tables(0).Rows(0)("bra_code"))
            Dim cn As String = Convert.ToString(ds.Tables(0).Rows(0)("cus_num"))
            Dim cc As String = Convert.ToString(ds.Tables(0).Rows(0)("cur_code"))
            Dim lc As String = Convert.ToString(ds.Tables(0).Rows(0)("led_code"))
            Dim sc As String = Convert.ToString(ds.Tables(0).Rows(0)("sub_acct_code"))

            Return bc & cn & cc & lc & sc
        Else


            Return ""
        End If
    End Function

    Public Function getAccountFromIMAL(ByVal nuban As String) As String
        Dim ds As New Data.DataSet()


        Using con As New System.Data.OracleClient.OracleConnection("Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.0.41.95)(PORT = 1521)))(CONNECT_DATA =(SERVICE_NAME = imaluatd)));Persist Security Info=True;User ID=imal;Password=imal;pooling=true;Max Pool Size=900")

            con.Open()

            'connect to banks and get account information
            Dim sql As String = "select * from amf where ADDITIONAL_REFERENCE  = :nuban"
            Dim cmd As New Data.OracleClient.OracleCommand
            cmd.Parameters.AddWithValue(":nuban", nuban)
            cmd.Connection = con
            Dim da As New Data.OracleClient.OracleDataAdapter
            da.SelectCommand = cmd
            da.Fill(ds)
        End Using

        If ds.Tables(0).Rows.Count > 0 Then
            Dim bc As String = Convert.ToString(ds.Tables(0).Rows(0)("BRANCH_CODE"))
            Dim cn As String = Convert.ToString(ds.Tables(0).Rows(0)("CIF_SUB_NO"))
            Dim cc As String = Convert.ToString(ds.Tables(0).Rows(0)("CURRENCY_CODE"))
            Dim lc As String = Convert.ToString(ds.Tables(0).Rows(0)("GL_CODE"))
            Dim sc As String = Convert.ToString(ds.Tables(0).Rows(0)("SL_NO"))

            Return bc & cn & cc & lc & sc
        Else


            Return ""
        End If
    End Function

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Button2.Visible = False
        Button4.Visible = False

        Label4.Visible = False

        Try
            If Session("branch").ToString.Contains("900") Then
                Dim expl As Array
                expl = chk2.getExplCodes("HQ").Split("*")
                DropDownList2.DataSource = expl
                DropDownList2.DataBind()
                Label7.Text = "Select Unit"
                DropDownList3.DataSource = chk2.getUnits().Split("*")
                DropDownList3.DataBind()

            Else
                Label7.Text = ""
                DropDownList3.Visible = False

                Dim expl As Array
                expl = chk2.getExplCodes("BRANCH").Split("*")
                DropDownList2.DataSource = expl
                DropDownList2.DataBind()

            End If
        Catch ex As Exception
            If ex.Message.Contains("Object") Then
                Response.Redirect("default.aspx")

            End If


        End Try

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If (Session("name") = "" And Session("type") <> "Inputter") Then
            Response.Redirect("default.aspx")

        End If
        Label2.Text = "Welcome, " & Session("name") & "    |   <a href='main.aspx'>Home</a>    |    <a href='myuploads.aspx'>My Uploads</a>    |   <a href='Bulktraformats.zip'>Download Excel Format</a>     |     <a href='logout.aspx'>signout</a>"

    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click

        Button2.Visible = True
        Label3.Text = ""
        Label6.Text = ""

        Button4.Visible = False


        If TextBox1.Text = "" Then

            Button1.Enabled = True
            Button1.Text = "Upload"
            Label3.ForeColor = Drawing.Color.Red
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & "Please enter a Title for this schedule" & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"

            Exit Sub

        End If

        Try

            If DropDownList2.Text.Contains("INWARD") Or DropDownList2.Text.Contains("DAT") Then

                inward()
                Exit Sub

            End If


            If DropDownList1.Text = "NUBAN" Then
                nuban()
                'imal()
                'ElseIf DropDownList1.Text = "IMAL" Then
                '    imal()

            Else
                non_nuban()

            End If

        Catch ex As Exception
            chk2.createErrorLog(ex.ToString(), Now)

            Label3.ForeColor = Drawing.Color.Red

            If ex.Message.Contains("account_number") Or ex.Message.Contains("bra_code") Or ex.Message.Contains("docnum") Then

                Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'><b>Error: Please select the correct schedule</b></td></tr><tr><td valign='middle'></td></tr></table>"


                Exit Sub

            End If

            If ex.Message.Contains("Oracle") Then
                Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'><b>Error: Cannot connect to BANKS at this time.</b></td></tr><tr><td valign='middle'></td></tr></table>"
                'Label3.Text = ex.Message
                Exit Sub

            Else

                Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'><b>An error was encountered, Check your excel File.</b></td></tr><tr><td valign='middle'></td></tr></table>"
                'Label3.Text = ex.Message
                Exit Sub

            End If

            Exit Sub

        End Try


    End Sub

    Sub nuban_thread()
        Dim t As New Threading.Thread(AddressOf nuban)

        t.Priority = Threading.ThreadPriority.Lowest

        t.Start()
    End Sub
    Sub InsertBatchType(ByVal bno As String)
        Dim bu As New bulktra

        Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "insert into batchType(BATCH_ID, BATCH_TYPE, EXPL_CODE) values(@bid,@btype,@ecode)"
            cmd.Parameters.AddWithValue("@bid", bno)
            cmd.Parameters.AddWithValue("@btype", DropDownList2.Text.Split("-")(1).Trim)
            cmd.Parameters.AddWithValue("@ecode", bu.getExplCode(DropDownList2.Text.Split("-")(1).Trim))

            cmd.ExecuteNonQuery()

        End Using
    End Sub
    Sub InsertBatch(ByVal bno As String)

        Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "insert into batches(batch_id,uploadedBy, Date) values(@bno,@uploadby,@date)"
            cmd.Parameters.AddWithValue("@bno", bno)
            cmd.Parameters.AddWithValue("@uploadBy", Session("name"))
            cmd.Parameters.AddWithValue("@Date", Now.ToString("yyyy-MM-dd hh:ss tt"))

            cmd.ExecuteNonQuery()

        End Using

    End Sub
    Sub export()
        export2(Session("bno") & ".xls", DataGrid1)
        Button2.Text = "Export to Excel"
        Button2.Enabled = True

    End Sub

    Sub export2(ByVal fn As String, ByVal dg As DataGrid)
        Response.ClearContent()
        Response.AddHeader("content-disposition", "attachment; filename=" & Server.MapPath("./temp2/exported_" & fn))
        Response.ContentType = "application/excel"
        Dim sw As New System.IO.StringWriter()
        Dim htw As New HtmlTextWriter(sw)
        dg.RenderControl(htw)
        Button2.Text = "Export to Excel"
        Button2.Enabled = True
        Response.Write(sw.ToString())
        Response.[End]()

    End Sub

    Protected Sub Button2_Click(sender As Object, e As System.EventArgs) Handles Button2.Click
        export()


    End Sub
    Function check(ByVal bra_code As String, ByVal cus_num As String, ByVal cur_code As String, ByVal led_code As String, ByVal sub_acct_code As String, ByVal tra_amt As String) As String
        Dim acc As String = ""
        acc = bra_code & cus_num & cur_code & led_code ''& sub_acct_code

        Dim res As String = IO.File.ReadAllText(Server.MapPath("./AllowedLedgers.txt"), Encoding.Default)

        If (bra_code = String.Empty Or cus_num = String.Empty Or cur_code = String.Empty Or led_code = String.Empty Or sub_acct_code = String.Empty Or tra_amt = String.Empty Or Not IsNumeric(tra_amt)) Then
            j = j + 1

            Return "WRONG FORMAT"
        End If

        'Disable Ledger 303
        If led_code.Trim = "303" Then
            j = j + 1

            Return "DISALLOWED LEDGER"
        End If

        If Not Session("branch").ToString.Contains("900") Then
            If (Val(cus_num) < 20000 And res.Contains("[" & led_code.Trim & "]")) Then
                Return "<img src='images\good.png' title='No empty fields present.'   width='22'>"
            ElseIf (Val(cus_num) < 20000 And Not res.Contains("[" & led_code.Trim & "]")) Then
                i = i + 1
                Return "ACCOUNT NOT ALLOWED"

            Else
                Return "<img src='images\good.png' title='No empty fields present.'   width='22'>"
            End If

        Else
            Return "<img src='images\good.png' title='No empty fields present.'   width='22'>"

        End If

    End Function
    Function check2(ByVal bra_code As String, ByVal cus_num As String, ByVal cur_code As String, ByVal led_code As String, ByVal sub_acct_code As String, ByVal tra_amt As String) As String
        ''Dim bankchk As New bank.bank
        'Dim bankchk As New NewBanks.banks
        Dim chck As New SBPSwitch.sbpswitch
        Dim ac As New Integer

        Dim bc As String

        ac = chck.getInternalBankID(sub_acct_code)

        If Not sub_acct_code = "0" Then

            If ac <> 0 Then
                bc = Convert.ToString(ac)
            Else

                Return ""

            End If
        End If

        If bc = "1" Then

            Dim bankchk As New NewBanks.banks

            Dim ds As New Data.DataSet
            Dim acc As String = ""
            Dim status As String = ""
            If Not (bra_code = String.Empty Or cus_num = String.Empty Or cur_code = String.Empty Or led_code = String.Empty Or sub_acct_code = String.Empty Or tra_amt = String.Empty) Then

                acc = bra_code & cus_num & cur_code & led_code ''& sub_acct_code

                ds = bankchk.getAccountFullInfo(sub_acct_code)

                Try

                    status = ds.Tables(0).Rows(0).Item("STA_CODE").ToString

                Catch ex As Exception

                End Try

                ''status = bankchk.getSTACODE(bra_code, cus_num, cur_code, led_code, sub_acct_code)
            End If


            Dim st As String = ""


            status = "1"


            Select Case status
                Case 0
                    i = i + 1
                    st = "ACCOUNT INACTIVE"
                    Return st
                Case 1
                    Return "<img src='images\good.png' title='Account is active.'   width='22'>"

                Case 2
                    i = i + 1
                    st = "ACCOUNT CLOSED"
                    Return st

                Case 3
                    i = i + 1
                    st = "ACCOUNT DORMANT"
                    Return st

                Case 4
                    i = i + 1
                    st = "ACCOUNT SUSPENDED"
                    Return st

                Case 8
                    i = i + 1
                    st = "ACCOUNT BLACKLISTED"
                    Return st

                Case 9
                    i = i + 1
                    st = "ACCOUNT DELETED"
                    Return st
                Case Else

                    i = i + 1
                    st = "ACCOUNT NOT EXIST"
                    Return st
            End Select

        Else
            Dim bankchk As New NewImal.Service

            Dim ds As New Data.DataSet
            Dim acc As String = ""
            Dim status As String = ""
            If Not (bra_code = String.Empty Or cus_num = String.Empty Or cur_code = String.Empty Or led_code = String.Empty Or sub_acct_code = String.Empty Or tra_amt = String.Empty) Then

                acc = bra_code & cus_num & cur_code & led_code ''& sub_acct_code

                ds = bankchk.GetAccountByAccountNumber(sub_acct_code)

                Try
                    status = ds.Tables(0).Rows(0).Item("STA_CODE").ToString
                Catch ex As Exception

                End Try

                ''status = bankchk.getSTACODE(bra_code, cus_num, cur_code, led_code, sub_acct_code)
            End If


            Dim st As String = ""

            status = "1"

            Select Case status
                Case 0
                    i = i + 1
                    st = "I"
                    Return st

                Case 1
                    i = i + 1
                    st = "A"
                    Return "<img src='images\good.png' title='Account is active.'   width='22'>"

                Case 2
                    i = i + 1
                    st = "C"
                    Return st

                Case 3
                    i = i + 1
                    st = "T"
                    Return st

                Case 4
                    i = i + 1
                    st = "S"
                    Return st

                Case 5
                    i = i + 1
                    st = "P"
                    Return st

                Case 6
                    i = i + 1
                    st = "O"
                    Return st

                Case 7
                    i = i + 1
                    st = "R"
                    Return st

                Case 9
                    i = i + 1
                    st = "D"
                    Return st

                Case 10
                    i = i + 1
                    st = "F"
                    Return st

                Case 11
                    i = i + 1
                    st = "M"
                    Return st

                Case 12
                    i = i + 1
                    st = "Q"
                    Return st

                Case 13
                    i = i + 1
                    st = "Y"
                    Return st

                Case 14
                    i = i + 1
                    st = "X"
                    Return st

                Case Else

                    i = i + 1
                    st = "ACCOUNT NOT EXIST"
                    Return st
            End Select

        End If



    End Function
    Sub non_nuban()
        Dim rnd As New System.Random(Second(Now))
        bno = Day(Now) & Year(Now) & Month(Now) & Second(Now) & Minute(Now) & Hour(Now) & rnd.Next(10, 90) & Request.UserHostAddress.ToString.Split(".")(3)

        Session("bno") = bno.ToString.PadLeft(15, "0")


        Label4.Text = ""
        Label5.Text = ""


        If Not FileUpload1.FileName = String.Empty And (FileUpload1.FileName.Contains(".xlsx") Or FileUpload1.FileName.Contains(".xls")) Then

            If FileUpload1.FileName.Contains(".xlsx") Then

                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xlsx"))
            Else
                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xls"))


            End If

            FileUpload1.Dispose()
            Using conn1 As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                conn1.Open()
                Dim cmd1 As New Data.SqlClient.SqlCommand
                cmd1.Connection = conn1
                Dim constr As String = ""
                If FileUpload1.FileName.Contains(".xlsx") Then

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xlsx") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                Else

                    'constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" & Server.MapPath("./temp/" & Session("bno") & ".xls") & "';Extended Properties='Excel 8.0;IMEX=1;HDR=YES;'"

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xls") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                End If


                Using conn As New Data.OleDb.OleDbConnection(constr)
                    conn.Open()
                    Dim cmd As New Data.OleDb.OleDbCommand
                    cmd.Connection = conn
                    cmd.CommandText = "select * from [sheet1$]"
                    Dim rs As Data.OleDb.OleDbDataReader

                    rs = cmd.ExecuteReader

                    If rs.HasRows Then

                        While rs.Read

                            If Val(rs("tra_amt")) >= 0 Then

                                If Session("branch").ToString.Contains("900") Then

                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,docnum,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@dnum,@cb)"


                                    cmd1.Parameters.AddWithValue("@bc", rs("bra_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", rs("cus_num").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", rs("cur_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", rs("led_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", rs("sub_acct_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt").ToString), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@dnum", rs("docnum").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cb", "1")

                                    cmd1.ExecuteNonQuery()
                                    cmd1.Parameters.Clear()

                                Else

                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@cb)"

                                    cmd1.Parameters.AddWithValue("@bc", rs("bra_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", rs("cus_num").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", rs("cur_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", rs("led_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", rs("sub_acct_code").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt").ToString), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@cb", "1")
                                    cmd1.ExecuteNonQuery()

                                End If

                                cmd1.Parameters.Clear()

                            End If

                        End While

                        Dim da As New Data.SqlClient.SqlDataAdapter
                        Dim cm As New Data.SqlClient.SqlCommand("select * from transTemp where batch_id=@bid", conn1)
                        cm.Parameters.AddWithValue("@bid", Session("bno"))
                        da.SelectCommand = cm

                        Dim ds As New Data.DataSet
                        da.Fill(ds)
                        DataGrid1.DataSource = ds
                        'DataGrid1.DataBind()
                        Button1.Enabled = True

                        Button1.Text = "Upload"
                        Label3.ForeColor = Drawing.Color.Green
                        Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>"
                        Label3.Text &= "<img src='images\arrow.png' width='10'> " & chk2.getTotalDRCR(Session("bno")) & "<br>"
                        Label3.Text &= "<img src='images\arrow.png' width='10'> " & getTot(Session("bno")) & " Record(s)" & "</br>"

                        If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then

                            Label3.Text &= "<img src='images\arrow.png' width='10'> <b>Customer's Balance:NGN </b>" & chk2.getbalance(chk2.getAccNoofBatch(Session("bno"))) & "<br>"
                        Else

                        End If

                        If Not (chk2.getTotalCR(Session("bno")) = chk2.getTotalDR(Session("bno"))) Then
                            i = i + 1
                            extra = "<img src='images\arrow.png' width='10'> Mismatch in Total Debits and Credits.<br>Credits(<b>" & FormatNumber(chk2.getTotalCR(Session("bno")), 2) & "</b>) ----  Debits(<b>" & FormatNumber(chk2.getTotalDR(Session("bno")), 2) & "</b>)"

                        End If

                        Label3.Text &= "<img src='images\arrow.png' width='10'><b> No of Similar Schedule(s):</b><b>" & chk2.CheckTotalCusnum(Session("name"), Session("bno")) & " - Continue?" & "</b><br>"
                        Label3.Text &= "<br></td></tr></table>"

                        Label6.ForeColor = Drawing.Color.Green
                        Label6.Text = "<table width='400' bgcolor='#ffffcc'><tr><td valign='middle'><img src='images\good.png' width='15'> " & "Upload & Validation completed. See results below.<br><b><a href='main.aspx'>New Transaction</a></b>" & "</td></tr></table>"

                        InsertBatchType(Session("bno"))

                        DataGrid1.Focus()

                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded for validation by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                        If i > 0 Or j > 0 Then
                            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & extra & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"
                            Button2.Visible = True

                            flushtemp(Session("bno"))

                        Else
                            Button2.Visible = True
                            Button4.Visible = True
                            Label4.Visible = True
                        End If
                    Else
                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded Empty by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                    End If

                End Using
            End Using

        Else
            Label3.ForeColor = Drawing.Color.Red
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & "Please select a valid Excel .xls, .xlsx file" & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"

        End If

    End Sub
    Sub nuban()
        Dim rnd As New System.Random(Second(Now))
        bno = Day(Now) & Year(Now) & Month(Now) & Second(Now) & Minute(Now) & Hour(Now) & rnd.Next(10, 90)
        '& Request.UserHostAddress.ToString.Split(".")(3)

        Session("bno") = bno.ToString.PadLeft(15, "0")

        Label4.Text = ""
        Label5.Text = ""


        If Not FileUpload1.FileName = String.Empty And (FileUpload1.FileName.Contains(".xlsx") Or FileUpload1.FileName.Contains(".xls")) Then
            If FileUpload1.FileName.Contains(".xlsx") Then

                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xlsx"))

            Else

                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xls"))

            End If

            FileUpload1.Dispose()
            Using conn1 As New Data.SqlClient.SqlConnection(chk2.sqlconn())


                conn1.Open()
                Dim cmd1 As New Data.SqlClient.SqlCommand
                cmd1.Connection = conn1


                'load excel file into temp
                Dim constr As String = ""
                If FileUpload1.FileName.Contains(".xlsx") Then

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xlsx") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                Else

                    'constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" & Server.MapPath("./temp/" & Session("bno") & ".xls") & "';Extended Properties='Excel 8.0;IMEX=1;HDR=YES;'"

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xls") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                End If

                Using conn As New Data.OleDb.OleDbConnection(constr)

                    Dim branch As String = ""
                    Dim cmd2 As New Data.SqlClient.SqlCommand
                    cmd2.Connection = conn2
                    conn2.Open()
                    Dim rs2 As Data.SqlClient.SqlDataReader
                    Dim splitbrcode As Array = Session("branch").ToString.Split("*")
                    cmd2.CommandText = "select T24BRANCHID from BranchInfo  where BANKSBRACODE = @bankcode"
                    cmd2.Parameters.AddWithValue("@bankcode", splitbrcode(0))
                    rs2 = cmd2.ExecuteReader
                    rs2.Read()
                    branch = rs2("T24BRANCHID")
                    conn2.Close()

                    conn.Open()
                    Dim cmd As New Data.OleDb.OleDbCommand
                    cmd.Connection = conn
                    cmd.CommandText = "select * from [sheet1$]"
                    Dim rs As Data.OleDb.OleDbDataReader

                    rs = cmd.ExecuteReader
                    Dim bra_code As String = ""
                    Dim cus_num As String = ""
                    Dim cur_code As String = ""
                    Dim led_code As String = ""
                    Dim sub_acct_code As String = ""
                    Dim tra_amt As String = ""
                    Dim deb_cre_ind As String = ""
                    Dim nub As String = ""
                    Dim bnk2 As New bank.bank
                    Dim ds2 As New Data.DataSet
                    Dim act As String = ""
                    Dim cbs = ""
                    If rs.HasRows Then


                        While rs.Read

                            If Val(rs("tra_amt").ToString) >= 0 Then
                                nub = Convert.ToString(rs("account_number")).Trim.PadLeft(10, "0")

                                Dim check As New SBPSwitch.sbpswitch
                                Dim alc As New Integer

                                Dim blc As String

                                alc = check.getInternalBankID(nub)

                                If Not nub = "0" Then

                                    If alc <> 0 Then
                                        blc = Convert.ToString(alc)
                                        'Return bc

                                    Else

                                    End If
                                End If

                                Try

                                    If blc = "1" Then
                                        cbs = 1
                                        act = getOldAcc(nub)
                                    Else
                                        act = getOldImalAcc(nub)
                                        cbs = 2
                                    End If

                                Catch ex As Exception
                                    chk2.createErrorLog(ex.ToString(), Now)

                                    Label3.ForeColor = Drawing.Color.Red

                                    If ex.Message.Contains("position") Then

                                        Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'><b>Error:" & nub & "  Account Number does not exist:</b></td></tr><tr><td valign='middle'></td></tr></table>"

                                        Exit Sub

                                    End If

                                End Try
                                ' act = getAccountFromNUBAN(nub)
                                If Not (act = "" Or act = String.Empty) Then

                                    bra_code = act.Split("*")(0).ToString
                                    cus_num = act.Split("*")(1).ToString
                                    cur_code = act.Split("*")(2).ToString
                                    led_code = act.Split("*")(3).ToString
                                    sub_acct_code = nub '' act.Split("*")(4).ToString
                                    tra_amt = rs("tra_amt")
                                    deb_cre_ind = rs("deb_cre_ind")

                                Else
                                    bra_code = ""
                                    cus_num = ""
                                    cur_code = ""
                                    led_code = ""
                                    sub_acct_code = ""
                                    tra_amt = ""
                                    deb_cre_ind = ""

                                End If

                                If Session("branch").ToString.Contains("900") Then

                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,docnum,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@dnum,@cb)"

                                    cmd1.Parameters.AddWithValue("@bc", branch.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", cus_num.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", cur_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", led_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", sub_acct_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt")), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@dnum", rs("docnum"))
                                    cmd1.Parameters.AddWithValue("@cb", cbs)
                                    cmd1.ExecuteNonQuery()
                                    cmd1.Parameters.Clear()

                                Else
                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@cb)"

                                    cmd1.Parameters.AddWithValue("@bc", branch.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", cus_num.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", cur_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", led_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", sub_acct_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt")), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@cb", cbs)
                                    cmd1.ExecuteNonQuery()
                                    cmd1.Parameters.Clear()

                                End If

                            End If


                        End While

                        Dim da As New Data.SqlClient.SqlDataAdapter
                        Dim cm As New Data.SqlClient.SqlCommand("select * from transTemp where batch_id=@bid", conn1)
                        cm.Parameters.AddWithValue("@bid", Session("bno"))
                        da.SelectCommand = cm
                        Dim ds As New Data.DataSet
                        da.Fill(ds)
                        DataGrid1.DataSource = ds
                        DataGrid1.DataBind()

                        Button1.Enabled = True

                        Button1.Text = "Upload"
                        Label3.ForeColor = Drawing.Color.Green
                        Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>"
                        Label3.Text &= chk2.getTotalDRCR(Session("bno")) & "<br>"
                        Label3.Text &= getTot(Session("bno")) & " Record(s)" & "</br>"

                        'Dim chck As New SBPSwitch.sbpswitch
                        'Dim ac As New Integer

                        'Dim bc As String

                        'ac = chck.getInternalBankID(nub)

                        'If Not nub = "0" Then

                        'If ac <> 0 Then
                        '    bc = Convert.ToString(ac)

                        'Else

                        'End If

                        'If bc = 1 Then
                        If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then
                            Label3.Text &= "<b>Customer's Balance:</b>" & chk2.getbalance(chk2.getAccNoofBatch(Session("bno"))) & "<br>"
                        Else
                            'Label3.Text &= "<b>Customer's Balance:</b>" & chk2.getbalanceimal(chk2.getAccNoofBatch(Session("bno"))) & "<br>"
                        End If

                        'Else
                        '    If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then
                        '        Label3.Text &= "<b>Customer's Balance:</b>" & chk2.getbalanceimal(chk2.getAccNoofBatchImal(Session("bno"))) & "<br>"
                        '    Else

                        '    End If

                        'End If



                        Label3.Text &= "<img src='images\arrow.png' width='10'><b> No of Similar Schedule(s):</b><b>" & chk2.CheckTotalCusnum(Session("name"), Session("bno")) & " - Continue?" & "</b><br>"
                        Label3.Text &= "<br></td></tr></table>"

                        Label6.ForeColor = Drawing.Color.Green
                        Label6.Text = "<table width='400' bgcolor='#ffffcc'><tr><td valign='middle'><img src='images\good.png' width='15'> " & "Upload & Validation completed. See results below.<br><b><a href='main.aspx'>New Transaction</a></b>" & "</td></tr></table>"

                        InsertBatchType(Session("bno"))

                        DataGrid1.Focus()

                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded for validation by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                        If i < 0 Or j < 0 Then
                            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & chk2.getTotalDRCR(Session("bno")) & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"
                            Button2.Visible = True
                            flushtemp(Session("bno"))

                        Else
                            Button2.Visible = True
                            Button4.Visible = True
                            Label4.Visible = True
                        End If

                        If check3(bra_code, cus_num, cur_code, led_code, sub_acct_code, tra_amt, deb_cre_ind) = "INSUFFICIENT BALANCE" Then
                            Button4.Visible = False
                        Else
                            Button4.Visible = True

                            If compareTotalDRCR(Session("bno")) = True Then
                                Button4.Visible = True
                            Else

                                Button4.Visible = False

                            End If
                        End If

                    Else
                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded Empty by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                    End If
                    '    End If
                End Using
            End Using


        Else
            Label3.ForeColor = Drawing.Color.Red
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & "Please select a valid Excel .xls, .xlsx file" & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"


        End If

    End Sub

    Sub imal()
        Dim rnd As New System.Random(Second(Now))
        bno = Day(Now) & Year(Now) & Month(Now) & Second(Now) & Minute(Now) & Hour(Now) & rnd.Next(10, 90)


        Session("bno") = bno.ToString.PadLeft(15, "0")

        Label4.Text = ""
        Label5.Text = ""


        If Not FileUpload1.FileName = String.Empty And (FileUpload1.FileName.Contains(".xlsx") Or FileUpload1.FileName.Contains(".xls")) Then
            If FileUpload1.FileName.Contains(".xlsx") Then

                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xlsx"))

            Else

                FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".xls"))

            End If

            FileUpload1.Dispose()
            Using conn1 As New Data.SqlClient.SqlConnection(chk2.sqlconn())

                conn1.Open()
                Dim cmd1 As New Data.SqlClient.SqlCommand
                cmd1.Connection = conn1


                Dim constr As String = ""
                If FileUpload1.FileName.Contains(".xlsx") Then

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xlsx") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                Else

                    constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Server.MapPath("./temp/" & Session("bno") & ".xls") & ";Extended Properties='Excel 12.0 Xml;IMEX=1;HDR=YES;'"

                End If

                Using conn As New Data.OleDb.OleDbConnection(constr)

                    Dim branch As String = ""
                    Dim cmd2 As New Data.SqlClient.SqlCommand
                    cmd2.Connection = conn2
                    conn2.Open()
                    Dim rs2 As Data.SqlClient.SqlDataReader
                    Dim splitbrcode As Array = Session("branch").ToString.Split("*")
                    cmd2.CommandText = "select T24BRANCHID from BranchInfo  where BANKSBRACODE = @bankcode"
                    cmd2.Parameters.AddWithValue("@bankcode", splitbrcode(0))
                    rs2 = cmd2.ExecuteReader
                    rs2.Read()
                    branch = rs2("T24BRANCHID")
                    conn2.Close()

                    conn.Open()
                    Dim cmd As New Data.OleDb.OleDbCommand
                    cmd.Connection = conn
                    cmd.CommandText = "select * from [sheet1$]"
                    Dim rs As Data.OleDb.OleDbDataReader

                    rs = cmd.ExecuteReader
                    Dim bra_code As String = ""
                    Dim cus_num As String = ""
                    Dim cur_code As String = ""
                    Dim led_code As String = ""
                    Dim sub_acct_code As String = ""
                    Dim nub As String = ""
                    Dim cbs As String = 2
                    Dim ds2 As New Data.DataSet
                    Dim act As String = ""
                    If rs.HasRows Then

                        While rs.Read

                            If Val(rs("tra_amt").ToString) >= 0 Then
                                nub = Convert.ToString(rs("account_number")).Trim.PadLeft(10, "0")
                                Try
                                    act = getOldImalAcc(nub)

                                Catch ex As Exception
                                    chk2.createErrorLog(ex.ToString(), Now)

                                    Label3.ForeColor = Drawing.Color.Red

                                    If ex.Message.Contains("position") Then

                                        Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'><b>Error:" & nub & "  Account Number does not exist:</b></td></tr><tr><td valign='middle'></td></tr></table>"

                                        Exit Sub

                                    End If

                                End Try
                                If Not (act = "" Or act = String.Empty) Then

                                    bra_code = act.Split("*")(0).ToString
                                    cus_num = act.Split("*")(1).ToString
                                    cur_code = act.Split("*")(2).ToString
                                    led_code = act.Split("*")(3).ToString
                                    sub_acct_code = nub

                                Else
                                    bra_code = ""
                                    cus_num = ""
                                    cur_code = ""
                                    led_code = ""
                                    sub_acct_code = ""

                                End If

                                If Session("branch").ToString.Contains("900") Then

                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,docnum,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@dnum,@cb)"

                                    cmd1.Parameters.AddWithValue("@bc", branch.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", cus_num.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", cur_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", led_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", sub_acct_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt")), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@dnum", rs("docnum"))
                                    cmd1.Parameters.AddWithValue("@cb", cbs)
                                    cmd1.ExecuteNonQuery()
                                    cmd1.Parameters.Clear()

                                Else
                                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@cb)"

                                    cmd1.Parameters.AddWithValue("@bc", branch.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cn", cus_num.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@cc", cur_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@lc", led_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@sc", sub_acct_code.ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@dci", rs("deb_cre_ind").ToString.Trim())
                                    cmd1.Parameters.AddWithValue("@ta", FormatNumber(Val(rs("tra_amt")), 2))
                                    cmd1.Parameters.AddWithValue("@rmks", rs("remarks").ToString.Replace("&", "-") & "-" & Session("bno"))
                                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                                    cmd1.Parameters.AddWithValue("@cb", cbs)

                                    cmd1.ExecuteNonQuery()
                                    cmd1.Parameters.Clear()

                                End If

                            End If
                        End While

                        Dim da As New Data.SqlClient.SqlDataAdapter
                        Dim cm As New Data.SqlClient.SqlCommand("select * from transTemp where batch_id=@bid", conn1)
                        cm.Parameters.AddWithValue("@bid", Session("bno"))
                        da.SelectCommand = cm
                        Dim ds As New Data.DataSet
                        da.Fill(ds)
                        DataGrid1.DataSource = ds
                        DataGrid1.DataBind()

                        Button1.Enabled = True

                        Button1.Text = "Upload"
                        Label3.ForeColor = Drawing.Color.Green
                        Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>"
                        Label3.Text &= chk2.getTotalDRCR(Session("bno")) & "<br>"
                        Label3.Text &= getTot(Session("bno")) & " Record(s)" & "</br>"

                        If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then
                            Label3.Text &= "<b>Customer's Balance:</b>" & chk2.getbalanceimal(chk2.getAccNoofBatchImal(Session("bno"))) & "<br>"
                        Else

                        End If

                        Label3.Text &= "<img src='images\arrow.png' width='10'><b> No of Similar Schedule(s):</b><b>" & chk2.CheckTotalCusnum(Session("name"), Session("bno")) & " - Continue?" & "</b><br>"
                        Label3.Text &= "<br></td></tr></table>"

                        Label6.ForeColor = Drawing.Color.Green
                        Label6.Text = "<table width='400' bgcolor='#ffffcc'><tr><td valign='middle'><img src='images\good.png' width='15'> " & "Upload & Validation completed. See results below.<br><b><a href='main.aspx'>New Transaction</a></b>" & "</td></tr></table>"

                        InsertBatchType(Session("bno"))

                        DataGrid1.Focus()

                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded for validation by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                        If i < 0 Or j < 0 Then
                            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & chk2.getTotalDRCR(Session("bno")) & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"
                            Button2.Visible = True
                            flushtemp(Session("bno"))

                        Else
                            Button2.Visible = True
                            Button4.Visible = True
                            Label4.Visible = True
                        End If
                    Else
                        Dim CrAudit As New bulktra
                        CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded Empty by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)

                    End If

                End Using
            End Using

        Else
            Label3.ForeColor = Drawing.Color.Red
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & "Please select a valid Excel .xls, .xlsx file" & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"

        End If

    End Sub
    Function check3(ByVal bra_code As String, ByVal cus_num As String, ByVal cur_code As String, ByVal led_code As String, ByVal sub_acct_code As String, ByVal tra_amt As String, ByVal deb_cre_ind As String) As String

        Dim chck As New SBPSwitch.sbpswitch

        Dim ac As New Integer

        Dim bc As String

        ac = chck.getInternalBankID(sub_acct_code)

        If Not sub_acct_code = "0" Then

            If ac <> 0 Then
                bc = Convert.ToString(ac)
                'Return bc

            Else

                Return ""

            End If
        End If


        If bc = "1" Then

            Dim s As String = ""
            If deb_cre_ind = "1" Then

                Dim acc As String = ""
                Dim clebal As String = ""
                ''Dim bank As New bank.bank
                'Dim bank As New NewBanks.banks
                Dim bank As New NewBanks.banks

                Dim ds As New Data.DataSet

                ds = bank.getAccountFullInfo(sub_acct_code)

                Try
                    clebal = ds.Tables(0).Rows(0).Item("UsableBal").ToString
                Catch ex As Exception

                End Try

                If Not cus_num = "0" Then
                    If ds.Tables(0).Rows.Count <> 0 Then
                        If Val(tra_amt) > Val(clebal) Then
                            s = "INSUFFICIENT BALANCE"
                            i = i + 1
                        End If
                    End If
                End If

            End If

            Return s

        Else

            Dim s As String = ""
            If deb_cre_ind = "1" Then

                Dim acc As String = ""
                Dim clebal As String = ""
                ''Dim bank As New bank.bank
                Dim bank As New NewImal.Service

                Dim ds As New Data.DataSet

                ds = bank.GetAccountByAccountNumber(sub_acct_code)

                Try
                    clebal = ds.Tables(0).Rows(0).Item("AVAIL_BAL").ToString
                Catch ex As Exception

                End Try

                If Not cus_num = "0" Then
                    If ds.Tables(0).Rows.Count <> 0 Then
                        If Val(tra_amt) > Val(clebal) Then
                            s = "INSUFFICIENT BALANCE"
                            i = i + 1
                        End If
                    End If
                End If

            End If


            Return s

        End If

    End Function
    Function check4(ByVal bra_code As String, ByVal cus_num As String, ByVal cur_code As String, ByVal led_code As String, ByVal sub_acct_code As String, ByVal tra_amt As String, ByVal deb_cre_ind As String) As String

        Dim chck As New SBPSwitch.sbpswitch

        Dim ac As New Integer

        Dim bc As String

        ac = chck.getInternalBankID(sub_acct_code)

        If Not sub_acct_code = "0" Then

            If ac <> 0 Then
                bc = Convert.ToString(ac)
                'Return bc

            Else

                Return ""

            End If
        End If

        If bc = "1" Then

            Dim s As String = ""

            'Dim bank As New NewBanks.banks
            Dim bank As New NewBanks.banks

            Dim ds As New Data.DataSet
            Dim strRest As String

            ds = bank.getAccountFullInfo(sub_acct_code)
            ''REST_FLAG
            strRest = ds.Tables(0).Rows(0).Item("REST_FLAG").ToString
            If strRest = "1" Then
                s = "TOTAL RESTRICTION"
                i = i + 1

            Else
                s = "<img src='images\good.png' title=''   width='22'>"

            End If

            Return s


        Else

            '' Dim bank As New bank.bank
            Dim s As String = ""

            Dim bank As New NewImal.Service
            Dim ds As New Data.DataSet
            Dim strRest As String

            ds = bank.GetAccountByAccountNumber(sub_acct_code)
            ''REST_FLAG
            strRest = ds.Tables(0).Rows(0).Item("REST_FLAG").ToString
            If strRest = "1" Then
                s = "TOTAL RESTRICTION"
                i = i + 1

            Else
                s = "<img src='images\good.png' title=''   width='22'>"

            End If

            Return s

        End If

    End Function
    Function getOldAcc(ByVal nuban As String) As String
        Dim ds As New Data.DataSet
        Dim bnk As New NewBanks.banks

        Dim oldacc As String = ""

        ds = bnk.getAccountFullInfo(nuban)
        '' oldacc = ds.Tables(0).Rows(0).Item("T24_BRA_CODE").ToString & "*" & ds.Tables(0).Rows(0).Item("T24_CUS_NUM").ToString & "*" & ds.Tables(0).Rows(0).Item("T24_CUR_CODE").ToString & "*" & ds.Tables(0).Rows(0).Item("T24_LED_CODE").ToString
        oldacc = ds.Tables(0).Rows(0).Item("T24_BRA_CODE").ToString & "*" & ds.Tables(0).Rows(0).Item("T24_CUS_NUM").ToString & "*" & ds.Tables(0).Rows(0).Item("Currency_code").ToString & "*" & ds.Tables(0).Rows(0).Item("T24_LED_CODE").ToString

        Return oldacc

    End Function

    Function getOldImalAcc(ByVal nuban As String) As String
        Dim ds As New Data.DataSet

        Dim bnk As New NewImal.Service

        Dim oldacc As String = ""

        ds = bnk.GetAccountByAccountNumber(nuban)

        oldacc = ds.Tables(0).Rows(0).Item("BRANCHCODE").ToString & "*" & ds.Tables(0).Rows(0).Item("CUS_NUM").ToString & "*" & ds.Tables(0).Rows(0).Item("CURRENCYCODE").ToString & "*" & ds.Tables(0).Rows(0).Item("LED_CODE").ToString

        Return oldacc

    End Function

    Function getChkDig(cusnum As String) As String
        Dim bnk As New bank.bank
        Return bnk.DATCONV_getCHE_DIG(cusnum, "")
    End Function
    Protected Sub DropDownList2_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList2.SelectedIndexChanged
        If DropDownList2.Text.Contains("CHARGES") Then
            Label5.Text = "5% VAT Applicable"

        ElseIf DropDownList2.Text.Contains("INWARD") Or DropDownList2.Text.Contains("DAT") Then
            Label5.Text = "DAT File Required"

        Else

            Label5.Text = ""

        End If
    End Sub
    Sub transmit()

        If Page.IsValid = True Then

            'Transmit details to authorizer
            Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                conn.Open()
                Dim cmd As New Data.SqlClient.SqlCommand
                Dim cba As String
                Dim chck As New SBPSwitch.sbpswitch
                Dim ac As New Integer
                Dim bc As String
                Dim acc As String
                cmd.Connection = conn

                Using connX As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                    connX.Open()
                    Dim cmdX As New Data.SqlClient.SqlCommand
                    cmdX.Connection = connX
                    cmdX.CommandText = "select * from transTemp where batch_id=@bid"
                    cmdX.Parameters.AddWithValue("@bid", Session("bno"))

                    Dim rsX As Data.SqlClient.SqlDataReader
                    rsX = cmdX.ExecuteReader
                    While rsX.Read
                        acc = rsX("sub_acct_code").ToString.Trim()

                        ac = chck.getInternalBankID(acc)

                        If Not acc = "0" Then
                            If ac <> 0 Then
                                bc = Convert.ToString(ac)
                            Else

                            End If
                        End If
                        cmd.CommandText = "INSERT INTO trans(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,docnum,cb) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@dnum,@cba)"


                        cmd.Parameters.AddWithValue("@bc", rsX("bra_code").ToString.Trim())
                        cmd.Parameters.AddWithValue("@cn", rsX("cus_num").ToString.Trim())
                        cmd.Parameters.AddWithValue("@cc", rsX("cur_code").ToString.Trim())
                        cmd.Parameters.AddWithValue("@lc", rsX("led_code").ToString.Trim())
                        cmd.Parameters.AddWithValue("@sc", rsX("sub_acct_code").ToString.Trim())
                        cmd.Parameters.AddWithValue("@dci", rsX("deb_cre_ind").ToString.Trim())
                        cmd.Parameters.AddWithValue("@ta", FormatNumber(Val(rsX("tra_amt")), 2))
                        cmd.Parameters.AddWithValue("@rmks", rsX("remarks").ToString)
                        cmd.Parameters.AddWithValue("@bid", Session("bno"))
                        cmd.Parameters.AddWithValue("@dnum", rsX("docnum"))

                        If bc = "1" Then
                            cmd.Parameters.AddWithValue("@cba", "1")
                        Else
                            cmd.Parameters.AddWithValue("@cba", "2")
                        End If

                        cmd.ExecuteNonQuery()
                        cmd.Parameters.Clear()


                    End While

                End Using



                'update batch unit
                If Session("branch").ToString.Contains("900") Then
                    chk2.createBatchUnit(Session("bno"), DropDownList3.Text)
                Else

                    chk2.createBatchUnit(Session("bno"), Session("braname"))

                End If



                'get total amount of batch
                Using connX As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                    connX.Open()

                    Dim total As String

                    Dim cmdX2 As New Data.SqlClient.SqlCommand
                    cmdX2.Connection = connX
                    cmdX2.CommandText = "select sum(tra_amt) from TransTemp where deb_cre_ind=@dci and batch_id=@bno"
                    cmdX2.Parameters.AddWithValue("@dci", "1")
                    cmdX2.Parameters.AddWithValue("@bno", Session("bno"))

                    total = cmdX2.ExecuteScalar

                    Dim CrBatch As New bulktra
                    CrBatch.createBatch(Session("bno"), TextBox1.Text.ToString.Replace("&", " and ").Replace("'", "`").Replace(",", "-"), Now.ToString("yyyy-MM-dd hh:mm tt"), total, "uploaded", Session("name"), "", "")

                    CrBatch.createBatchSuspense(Session("bno"), CrBatch.getBatchSuspense(Session("branch")))

                    'create audit trail
                    Dim CrAudit As New bulktra
                    CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was transmitted by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)


                    'notify authorizer
                    Dim body As String = ""
                    body = "Sir/Madam" & "<br>" & "<br>" & "This mail is a request for you to authorize a Bulk Transaction which I have uploaded. Details of this transaction are as follows;" & "<br>" &
                    "Batch Number: " & Session("bno") & "<br>" & "Total Amount: " & FormatNumber(Val(total), 2) & "<br>" & "Title: " & TextBox1.Text.Trim() & "<br><br>" & "Click on the link below to approve this transaction <br><br><a href='http://" & System.Configuration.ConfigurationManager.AppSettings("serverip") & "/" & System.Configuration.ConfigurationManager.AppSettings("AppVirDirAuth") & "/default.aspx?action=schedule&bid=" & enc.TripleDESEncode(Session("bno"), "bulktra!@#") & "'>Authorize this Transaction</a> <br><br>Thank You"

                    Try
                        Dim mail As New bulktra
                        mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmail(Session("branch")), Session("email"), "BULK TRANSACTION AUTHORIZATION REQUEST (BATCH NO: " & Session("bno") & ")", body)

                    Catch ex As Exception
                        chk2.createErrorLog(ex.Message, Now)

                    End Try

                    'flush temp table
                    Dim cmd2 As New Data.SqlClient.SqlCommand
                    cmd2.Connection = conn
                    cmd2.CommandText = "DELETE FROM TransTemp WHERE batch_id=@bid"
                    cmd2.Parameters.AddWithValue("@bid", Session("bno"))

                    cmd2.ExecuteNonQuery()

                End Using
            End Using


            Response.Redirect("uploadResp.aspx?bid=" & Session("bno"))




        End If


    End Sub

    Function getTot(ByVal bno As String) As Long
        Dim c As ULong = 0
        Using conn1 As New Data.SqlClient.SqlConnection(chk2.sqlconn())
            conn1.Open()
            Dim cmd1 As New Data.SqlClient.SqlCommand
            cmd1.Connection = conn1
            cmd1.CommandText = "select count(*) from transTemp where batch_id=@bid"
            cmd1.Parameters.AddWithValue("@bid", bno)

            c = cmd1.ExecuteScalar
        End Using

        Return c


    End Function

    Protected Sub Button4_Click(sender As Object, e As System.EventArgs) Handles Button4.Click
        transmit()

    End Sub
    Sub flushtemp(ByVal bno1 As String)
        Using conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
            conn.Open()

            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "DELETE FROM TransTemp WHERE batch_id=@bid"
            cmd2.Parameters.AddWithValue("@bid", bno1)

            cmd2.ExecuteNonQuery()

        End Using

    End Sub
    Sub inward()
        Dim rnd As New System.Random(Second(Now))
        bno = Day(Now) & Year(Now) & Month(Now) & Second(Now) & Minute(Now) & Hour(Now) & rnd.Next(10, 90)

        Session("bno") = bno.ToString.PadLeft(15, "0")


        Label4.Text = ""
        Label5.Text = ""


        If Not FileUpload1.FileName = String.Empty And (FileUpload1.FileName.Contains(".DAT") Or FileUpload1.FileName.Contains(".dat")) Then

            FileUpload1.SaveAs(Server.MapPath("./temp/" & Session("bno") & ".DAT"))
            FileUpload1.Dispose()

            Using conn1 As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                conn1.Open()
                Dim cmd1 As New Data.SqlClient.SqlCommand
                cmd1.Connection = conn1
                Dim sr As New IO.StreamReader(Server.MapPath("./temp/" & Session("bno") & ".DAT"))
                Dim rec As String = ""
                Do

                    rec = sr.ReadLine()

                    cmd1.CommandText = "INSERT INTO transTemp(bra_code,cus_num,cur_code,led_code,sub_acct_code,deb_cre_ind,tra_amt,remarks,batch_id,cba) VALUES(@bc,@cn,@cc,@lc,@sc,@dci,@ta,@rmks,@bid,@cb)"
                    Dim b As String = Convert.ToDecimal(Mid(rec, 21, 15)).ToString
                    cmd1.Parameters.AddWithValue("@bc", Val(Mid(rec, 1, 4)).ToString)
                    cmd1.Parameters.AddWithValue("@cn", Val(Mid(rec, 5, 7)).ToString)
                    cmd1.Parameters.AddWithValue("@cc", "1")
                    cmd1.Parameters.AddWithValue("@lc", Val(Mid(rec, 13, 4)).ToString)
                    cmd1.Parameters.AddWithValue("@sc", Val(Mid(rec, 17, 3)).ToString)
                    cmd1.Parameters.AddWithValue("@dci", Mid(rec, 20, 1).ToString)
                    cmd1.Parameters.AddWithValue("@ta", Convert.ToDecimal(Mid(rec, 21, 15)).ToString)
                    cmd1.Parameters.AddWithValue("@rmks", Mid(rec, 49, 250).ToString.Replace("&", "-") & "-" & Session("bno"))

                    cmd1.Parameters.AddWithValue("@bid", Session("bno"))
                    cmd1.Parameters.AddWithValue("@cb", Val(Mid(rec, 242).ToString))

                    cmd1.ExecuteNonQuery()

                    cmd1.Parameters.Clear()






                Loop Until sr.EndOfStream

                sr.Close()


                Dim da As New Data.SqlClient.SqlDataAdapter
                Dim cm As New Data.SqlClient.SqlCommand("select * from transTemp where batch_id=@bid", conn1)
                cm.Parameters.AddWithValue("@bid", Session("bno"))
                da.SelectCommand = cm

                Dim ds As New Data.DataSet
                da.Fill(ds)
                DataGrid1.DataSource = ds
                DataGrid1.DataBind()
            End Using

            Button1.Enabled = True

            Button1.Text = "Upload"
            Label3.ForeColor = Drawing.Color.Green
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>"
            Label3.Text &= "<img src='images\arrow.png' width='10'> " & chk2.getTotalDRCR(Session("bno")) & "<br>"
            Label3.Text &= "<img src='images\arrow.png' width='10'> " & getTot(Session("bno")) & " Record(s)" & "</br>"

            If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then
                Label3.Text &= "<img src='images\arrow.png' width='10'> <b>Customer's Balance:NGN </b>" & chk2.getbalance(chk2.getAccNoofBatch(Session("bno"))) & "<br>"
            Else

            End If

            If Not (chk2.getTotalCR(Session("bno")) = chk2.getTotalDR(Session("bno"))) Then
                i = i + 1
                extra = "<img src='images\arrow.png' width='10'> Mismatch in Total DR and CR.<br>"

            End If

            If DropDownList2.Text.Contains("SALARY") Or DropDownList2.Text.Contains("BULK") Then
                Label3.Text &= "<img src='images\arrow.png' width='10'> <b>Customer's Balance:NGN </b>" & chk2.getbalanceimal(chk2.getAccNoofBatch(Session("bno"))) & "<br>"
            Else

            End If

            If Not (chk2.getTotalCR(Session("bno")) = chk2.getTotalDR(Session("bno"))) Then
                i = i + 1
                extra = "<img src='images\arrow.png' width='10'> Mismatch in Total DR and CR.<br>"

            End If

            Label3.Text &= "<img src='images\arrow.png' width='10'><b> No of Similar Schedule(s):</b><b>" & chk2.CheckTotalCusnum(Session("name"), Session("bno")) & " - Continue?" & "</b><br>"
            Label3.Text &= "<br></td></tr></table>"

            Label6.ForeColor = Drawing.Color.Green
            Label6.Text = "<table width='400' bgcolor='#ffffcc'><tr><td valign='middle'><img src='images\good.png' width='15'> " & "Upload & Validation completed. See results below.<br><b><a href='main.aspx'>New Transaction</a></b>" & "</td></tr></table>"

            InsertBatchType(Session("bno"))

            DataGrid1.Focus()

            Dim CrAudit As New bulktra
            CrAudit.createAudit("Bulk Transaction with Batch Number " & Session("bno") & " was uploaded for validation by " & Session("name") & "(" & Session("uname") & ")" & " IP: " & Request.UserHostAddress, Now)



            If i > 0 Or j > 0 Then
                Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & extra & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"
                Button2.Visible = True


            Else
                Button2.Visible = True
                Button4.Visible = True
                Label4.Visible = True
            End If



        Else
            Label3.ForeColor = Drawing.Color.Red
            Label3.Text = "<table width='500' bgcolor='#ffffcc'><tr><td valign='middle'>" & "Please select a valid .DAT file" & "</td></tr><tr><td valign='middle'><p style='color:red;'>Errors are present in this schedule.</p></td></tr></table>"


        End If


    End Sub

    Public Function compareTotalDRCR(ByVal batch_id As String) As String

        Dim batchAcc As String
        Dim b As Boolean

        Dim transAmt As String
        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd3 As New Data.SqlClient.SqlCommand
            cmd3.Connection = conn
            cmd3.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd3.Parameters.AddWithValue("@bid", batch_id)
            cmd3.Parameters.AddWithValue("@dci", "1")

            batchAcc = Val(cmd3.ExecuteScalar)

            Dim cmd4 As New Data.SqlClient.SqlCommand
            cmd4.Connection = conn
            cmd4.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd4.Parameters.AddWithValue("@bid", batch_id)
            cmd4.Parameters.AddWithValue("@dci", "2")

            transAmt = Val(cmd4.ExecuteScalar)

        End Using

        If transAmt = batchAcc Then

            b = True
        Else

            'Label4.Visible = True
            'Button4.Visible = False
            b = False
        End If

        Return b

    End Function

End Class
