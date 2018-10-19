<%@ Page Title="" Language="C#" MasterPageFile="~/all.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>USSD State Engine Tool</h1>


    <div class="flex-grid">
        <div class="row">
            <div class="cell colspan12">
                <asp:TreeView ID="TreeView1" runat="server"></asp:TreeView>

                <%--<asp:GridView ID="GridView1" runat="server" Font-Size="11px">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" op_id="<%# Eval("op_id") %>" op_resp="<%# Eval("op_resp") %>">Edit</a>

                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>--%>
            </div>
        </div>
    </div>

</asp:Content>

