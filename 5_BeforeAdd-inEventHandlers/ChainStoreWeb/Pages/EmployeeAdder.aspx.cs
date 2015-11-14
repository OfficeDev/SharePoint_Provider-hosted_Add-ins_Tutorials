// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using ChainStoreWeb.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SharePoint.Client;

namespace ChainStoreWeb.Pages
{
    public partial class EmployeeAdder : System.Web.UI.Page
    {
        private SharePointContext spContext;
        private int listItemID;

        protected void Page_Load(object sender, EventArgs e)
        {
            spContext = Session["SPContext"] as SharePointContext;
            listItemID = GetListItemIDFromQueryParameter();

            // Read from SharePoint
            string employeeName = GetLocalEmployeeName();

            // Write to remote database
            AddLocalEmployeeToCorpDB(employeeName);

            // Write to SharePoint
            SetLocalEmployeeSyncStatus();

            // Go back to the Local Employees page
            Response.Redirect(spContext.SPHostUrl.ToString() + "Lists/Local%20Employees/AllItems.aspx", true);
        }

        private void SetLocalEmployeeSyncStatus()
        {
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List localEmployeesList = clientContext.Web.Lists.GetByTitle("Local Employees");
                ListItem selectedLocalEmployee = localEmployeesList.GetItemById(listItemID);
                selectedLocalEmployee["Added_x0020_to_x0020_Corporate_x"] = true;
                selectedLocalEmployee.Update();
                clientContext.ExecuteQuery();
            }
        }
        private string GetLocalEmployeeName()
        {
            ListItem localEmployee;

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                List localEmployeesList = clientContext.Web.Lists.GetByTitle("Local Employees");
                localEmployee = localEmployeesList.GetItemById(listItemID);
                clientContext.Load(localEmployee);
                clientContext.ExecuteQuery();
            }
            return localEmployee["Title"].ToString();
        }

        private int GetListItemIDFromQueryParameter()
        {
            int result;
            Int32.TryParse(Request.QueryString["SPListItemId"], out result);
            return result;
        }

        private void AddLocalEmployeeToCorpDB(string employeeName)
        {
            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "AddEmployee";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter tenant = cmd.Parameters.Add("@Tenant", SqlDbType.NVarChar);
                tenant.Value = spContext.SPHostUrl.ToString();
                SqlParameter name = cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
                name.Value = employeeName;
                cmd.ExecuteNonQuery();
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