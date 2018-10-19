<%@ Page Language="C#" AutoEventWireup="true" CodeFile="sess.aspx.cs" Inherits="sess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body { font-family: tahoma;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <table>
            <tr>
                <td>
                 <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            DataKeyNames="refID" DataSourceID="SqlDataSource1" 
                         Font-Size="10px">
            <Columns>
                <asp:BoundField DataField="refID" HeaderText="refID" InsertVisible="False" 
                    ReadOnly="True" SortExpression="refID" />
                <asp:BoundField DataField="sessionid" HeaderText="sessionid" 
                    SortExpression="sessionid" />
                <asp:BoundField DataField="msisdn" HeaderText="msisdn" 
                    SortExpression="msisdn" />
                <asp:BoundField DataField="op_id" HeaderText="op_id" SortExpression="op_id" />
                <asp:BoundField DataField="sub_op_id" HeaderText="sub_op_id" 
                    SortExpression="sub_op_id" />
                <asp:BoundField DataField="lastUpdated" HeaderText="lastUpdated" 
                    SortExpression="lastUpdated" />
                <asp:BoundField DataField="op_type" HeaderText="op_type" 
                    SortExpression="op_type" />
                <asp:BoundField DataField="statusflag" HeaderText="statusflag" 
                    SortExpression="statusflag" />
                <asp:BoundField DataField="params" HeaderText="params" 
                    SortExpression="params" />
                <asp:BoundField DataField="authkey" HeaderText="authkey" 
                    SortExpression="authkey" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:dbConn %>" 
            SelectCommand="SELECT * FROM [tbl_USSD_reqstate] WHERE ([sessionid] = @sessionid)">
            <SelectParameters>
                <asp:QueryStringParameter Name="sessionid" QueryStringField="id" 
                    Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>   
                </td>
            </tr>
            <tr>
                <td valign="top">
                    
    
      <div>
    
          <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
              DataKeyNames="reqID" DataSourceID="SqlDataSource2" 
              Font-Size="10px">
              <Columns> 
                  <asp:BoundField DataField="sessionid" HeaderText="sessionid" 
                      SortExpression="sessionid" />
                  <asp:BoundField DataField="req_msisdn" HeaderText="req_msisdn" 
                      SortExpression="req_msisdn" />
                  <asp:BoundField DataField="req_type" HeaderText="req_type" 
                      SortExpression="req_type" />
                  <asp:BoundField DataField="req_msg" HeaderText="req_msg" 
                      SortExpression="req_msg" />
                  <asp:BoundField DataField="resp_type" HeaderText="resp_type" 
                      SortExpression="resp_type" />
                  <asp:BoundField DataField="resp_msg" HeaderText="resp_msg" 
                      SortExpression="resp_msg" /> 
                  <asp:BoundField DataField="resp_ref" HeaderText="resp_ref" 
                      SortExpression="resp_ref" /> 
              </Columns>
          </asp:GridView>
          <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
              ConnectionString="<%$ ConnectionStrings:dbConn %>" 
              SelectCommand="SELECT * FROM [tbl_USSD_trnx] WHERE ([sessionid] = @sessionid)">
              <SelectParameters>
                  <asp:QueryStringParameter Name="sessionid" QueryStringField="id" 
                      Type="String" />
              </SelectParameters>
          </asp:SqlDataSource>
    
    </div>

                </td>
                 
            </tr>
        </table>
      <div>
    
    </div>
    </form>
</body>
</html>
