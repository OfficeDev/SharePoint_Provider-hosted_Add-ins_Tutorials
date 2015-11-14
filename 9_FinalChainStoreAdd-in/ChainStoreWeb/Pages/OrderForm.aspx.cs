// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using System;
using System.Data.SqlClient;
using System.Data;
using ChainStoreWeb.Utilities;
using Microsoft.SharePoint.Client;

namespace ChainStoreWeb.Pages
{
    public partial class OrderForm : System.Web.UI.Page
    {
        protected SharePointContext spContext;

        protected void Page_Load(object sender, EventArgs e)
        {
            spContext = Session["SPContext"] as SharePointContext;
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            UInt16 quantity;
            UInt16.TryParse(txtBoxQuantity.Text, out quantity);

            // Handle case where user presses the button without first entering rquired data.
            if (String.IsNullOrEmpty(txtBoxSupplier.Text) || String.IsNullOrEmpty(txtBoxItemName.Text))
            {
                lblOrderPrompt.Text = "Please enter a supplier and item.";
                lblOrderPrompt.ForeColor = System.Drawing.Color.Red;
                return;
            }
            else
            {
                if (quantity == 0)
                {
                    lblOrderPrompt.Text = "Quantity must be a positive number below 32,768.";
                    lblOrderPrompt.ForeColor = System.Drawing.Color.Red;
                    return;
                }
            }

            CreateOrder(txtBoxSupplier.Text, txtBoxItemName.Text, quantity);
            CreateExpectedShipment(txtBoxSupplier.Text, txtBoxItemName.Text, quantity);
        }
        private void CreateOrder(String supplierName, String productName, UInt16 quantityOrdered)
        {
            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "AddOrder";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter tenant = cmd.Parameters.Add("@Tenant", SqlDbType.NVarChar);
                tenant.Value = spContext.SPHostUrl.ToString();
                SqlParameter supplier = cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 50);
                supplier.Value = supplierName;
                SqlParameter itemName = cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 50);
                itemName.Value = productName;
                SqlParameter quantity = cmd.Parameters.Add("@Quantity", SqlDbType.SmallInt);
                quantity.Value = quantityOrdered;
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateExpectedShipment(string supplier, string product, UInt16 quantity)
        {
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List expectedShipmentsList = clientContext.Web.Lists.GetByTitle("Expected Shipments");
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = expectedShipmentsList.AddItem(itemCreateInfo);
                newItem["Title"] = product;
                newItem["Supplier"] = supplier;
                newItem["Quantity"] = quantity;
                newItem.Update();
                clientContext.ExecuteQuery();
            }
        }
    }
}

/*

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
  
*/