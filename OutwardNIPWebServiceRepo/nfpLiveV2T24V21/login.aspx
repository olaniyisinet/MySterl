<%@ Page Language="C#" MasterPageFile="~/login.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Title="NFP | Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  
       <div id="login">
            <div id="box">
            <wpt:infoBar ID="ibox" runat="server" msg="" />
      <table width="400" border="1" class="mytablelogon">
          <tr>
            <td style="width: 180px" class="mytablelogontd">Username</td>
            <td class="mytablelogontd"><asp:TextBox ID="txtusername" runat="server"></asp:TextBox></td>
          </tr>
          <tr>
            <td class="mytablelogontd">Password</td>
            <td class="mytablelogontd"><asp:TextBox ID="txtpassword" runat="server" TextMode="Password"></asp:TextBox></td>
          </tr>
          <tr>
            <td  class="mytablelogontd">&nbsp;</td>
            <td class="mytablelogontd"><asp:Button ID="btn_login" runat="server" Text="Logon" OnClick="btn_login_Click" /></td>
          </tr>
        </table>
       </div>
        </div>
</asp:Content>

