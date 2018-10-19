<%@ Page Language="C#" MasterPageFile="~/login.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Title="NFP | Roles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table width="100%" border="0" class="mytable">
      <tr>
          <td class="mytabletd" colspan="2">
             <div id="#qa2">
               <p><asp:Label ID="lblwelcome" runat="server" Font-Bold="True" Font-Size="14px"></asp:Label></p>
             </div>
              </td>
      </tr>
      <tr>
          <td class="mytabletd" colspan="2" style="background-color:#eee;">
          <h2>
              User's Roles</h2>
          </td>
      </tr>
             <tr>
               <td colspan="2" class="mytabletd" rowspan="4">
               
                   <asp:Label ID="Label1" runat="server"></asp:Label>
               </td>
             </tr>
           </table>
</asp:Content>

