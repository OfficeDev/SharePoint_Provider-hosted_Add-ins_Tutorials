

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderForm.aspx.cs" Inherits="ChainStoreWeb.Pages.OrderForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Form</title>
    <link type="text/css" rel="stylesheet" href="<%= spContext.SPHostUrl.ToString() + "_layouts/15/defaultcss.ashx" %>" />
</head>
<body style="margin:10px;">
   <!-- 
        A form in which the user enters a data for an order. 
        This order is then added to the SQL Azure database.
    -->
    <form id="frmOrder" runat="server" visible="true">
        <asp:Label ID="lblOrderPrompt" runat="server"
         Text="Enter a supplier, product, and quantity; and then press <span class='ms-accentText'>Place Order</span>.">
</asp:Label>
        <asp:Literal ID="Literal11" runat="server" Text="<br /><br />"></asp:Literal>

        <asp:Label ID="lblSupplier" runat="server" Text="Supplier:"></asp:Label>
        <asp:Literal ID="Literal12" runat="server" Text="<br />"></asp:Literal>
        <asp:TextBox ID="txtBoxSupplier" runat="server"></asp:TextBox>
        <asp:Literal ID="Literal15" runat="server" Text="<br /><br />"></asp:Literal>

        <asp:Label ID="lblProduct" runat="server" Text="Product:"></asp:Label>
        <asp:Literal ID="Literal20" runat="server" Text="<br />"></asp:Literal>
        <asp:TextBox ID="txtBoxItemName" runat="server"></asp:TextBox>
        <asp:Literal ID="Literal13" runat="server" Text="<br /><br />"></asp:Literal>

        <asp:Label ID="lblQuantity" runat="server" Text="Quantity:"></asp:Label>
        <asp:Literal ID="Literal21" runat="server" Text="<br />"></asp:Literal>
        <asp:TextBox ID="txtBoxQuantity" runat="server"></asp:TextBox>
        <asp:Literal ID="Literal19" runat="server" Text="<br /><br />"></asp:Literal>

        <asp:Button ID="btnCreateOrder" runat="server" Text="Place Order" OnClick="btnCreateOrder_Click" />
    </form>
</body>
</html>

<%--

OfficeDev/SharePoint_Provider-hosted_Add-ins_Tutorials, https://github.com/OfficeDev/SharePoint_Provider-hosted_Add-ins_Tutorials
 
Copyright (c) Microsoft Corporation
All rights reserved. 
 
MIT License:
Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:
 
The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.    
  
--%>
