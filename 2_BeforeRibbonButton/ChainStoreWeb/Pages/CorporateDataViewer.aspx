<%-- Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file. --%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CorporateDataViewer.aspx.cs" Inherits="ChainStoreWeb.CorporateDataViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chain Store</title>
    <script 
        src="http://ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" 
        type="text/javascript">
    </script>
    <script 
        type="text/javascript" 
        src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.9.1.min.js">
    </script>      
    <script type="text/javascript">
    var hostweburl;

    // Load the SharePoint resources.
    $(document).ready(function () {

        // Get the URI decoded add-in web URL.
        hostweburl =
            decodeURIComponent(
                getQueryStringParameter("SPHostUrl")
        );

        // The SharePoint js files URL are in the form:
        // web_url/_layouts/15/resource.js
        var scriptbase = hostweburl + "/_layouts/15/";

        // Load the js file and continue to the 
        // success handler.
        $.getScript(scriptbase + "SP.UI.Controls.js")
    });

    // Function to retrieve a query string value.
    function getQueryStringParameter(paramToRetrieve) {
        var params =
            document.URL.split("?")[1].split("&");
        var strParams = "";
        for (var i = 0; i < params.length; i = i + 1) {
            var singleParam = params[i].split("=");
            if (singleParam[0] == paramToRetrieve)
                return singleParam[1];
        }
    }
</script>
</head>
<body style="margin:10px;">

    <!-- Chrome control placeholder. Options are declared inline.  -->
<div 
    id="chrome_ctrl_container"
    data-ms-control="SP.UI.Controls.Navigation"  
    data-ms-options=
        '{  
            "appHelpPageUrl" : "Help.aspx",
            "appIconUrl" : "/Images/AppIcon.png",
            "appTitle" : "Chain Store",
            "settingsLinks" : [
                {
                    "linkUrl" : "Account.aspx",
                    "displayName" : "Account settings"
                },
                {
                    "linkUrl" : "Contact.aspx",
                    "displayName" : "Contact us"
                }
            ]
         }'>
</div>
    <h1>Fabrikam Corporate Data</h1>

    <form id="frmStoreData" runat="server" visible="true">
       
        <br />
        <h2>Orders for This Store:</h2>
            <br />
            <asp:Button ID="btnShowOrders" runat="server" Text="Show Orders" OnClick="btnShowOrders_Click" />
            <br />
            <asp:GridView ID="ordersGridView" runat="server" CellPadding="5" GridLines="None" HeaderStyle-CssClass="ms-uppercase" />

        <br />
        <h2>Inventory for This Store:</h2>
           <br />
           <asp:Button ID="btnShowInventory" runat="server" Text="Show Inventory" OnClick="btnShowInventory_Click" />
           <br />
           <asp:GridView ID="inventoryGridView" runat="server" CellPadding="5" GridLines="None" HeaderStyle-CssClass="ms-uppercase" />
        <br />
        <h2>Store Employees Registered with Corporate HR</h2>
           <br />
           <asp:Button ID="btnCorpEmployees" runat="server" Text="Show Employees" OnClick="btnShowCorpEmployees_Click" />
           <br />
           <asp:GridView ID="corpEmployeesGridView" runat="server" CellPadding="5" GridLines="None" HeaderStyle-CssClass="ms-uppercase" />
    </form>
     <asp:HyperLink runat="server" Text="Order Form" NavigateUrl="/Pages/OrderForm.aspx" />
   
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