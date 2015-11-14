// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Data.SqlClient;
using System.Data;
using ChainStoreWeb.Utilities;

namespace ChainStoreWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            string tenantName = properties.AppEventProperties.HostWebFullUrl.ToString();
            if (!tenantName.EndsWith("/"))
            {
                tenantName += "/";
            }

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:

                    try
                    {
                        CreateTenant(tenantName);
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel and roll back the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;
                    }

                    break;
                case SPRemoteEventType.AppUpgraded:
                    // This sample does not implement an add-in upgrade handler.
                    break;
                case SPRemoteEventType.AppUninstalling:

                    try
                    {
                        DeleteTenant(tenantName);
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel and roll back the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;
                    }

                    break;
            }

            return result;
        }

        private void CreateTenant(string tenantName)
        {
            // Do not catch exceptions. Allow them to bubble up and trigger roll back
            // of installation.

            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "AddTenant";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter name = cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                name.Value = tenantName;
                cmd.ExecuteNonQuery();
            }//dispose conn and cmd
        }

        private void DeleteTenant(string tenantName)
        {
            // Do not catch exceptions. Allow them to bubble up and trigger roll back
            // of un-installation (removal from 2nd level Recycle Bin).

            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "RemoveTenant";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter name = cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                name.Value = tenantName;
                cmd.ExecuteNonQuery();
            }//dispose conn and cmd
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
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
