// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.Web.Configuration;


namespace ChainStoreWeb.Utilities
{
    public static class SharePointComponentDeployer
    {
        internal static SharePointContext sPContext;
        internal static Version localVersion;

        internal static Version RemoteTenantVersion
        {
            get
            {
                return GetTenantVersion();
            }
            set
            {
                SetTenantVersion(value);
            }
        }

        public static bool IsDeployed
        {
            get
            {
                if (RemoteTenantVersion < localVersion)
                    return false;
                else
                    return true;
            }
        }
        internal static void DeployChainStoreComponentsToHostWeb(HttpRequest request)
        {
            CreateLocalEmployeesList();
            ChangeCustomActionRegistration();
            CreateExpectedShipmentsList();
            RegisterExpectedShipmentsEventHandler(request);
            RemoteTenantVersion = localVersion;
        }

        private static void CreateLocalEmployeesList()
        {
            using (var clientContext = sPContext.CreateUserClientContextForSPHost())
            {
                var query = from list in clientContext.Web.Lists
                            where list.Title == "Local Employees"
                            select list;
                IEnumerable<List> matchingLists = clientContext.LoadQuery(query);
                clientContext.ExecuteQuery();

                if (matchingLists.Count() == 0)
                {
                    ListCreationInformation listInfo = new ListCreationInformation();
                    listInfo.Title = "Local Employees";
                    listInfo.TemplateType = (int)ListTemplateType.GenericList;
                    listInfo.Url = "Lists/Local Employees";
                    List localEmployeesList = clientContext.Web.Lists.Add(listInfo);

                    Field field = localEmployeesList.Fields.GetByInternalNameOrTitle("Title");
                    field.Title = "Name";
                    field.Update();

                    localEmployeesList.Fields.AddFieldAsXml("<Field DisplayName='Added to Corporate DB'"
                                         + " Type='Boolean'"
                                         + " ShowInEditForm='FALSE' "
                                         + " ShowInNewForm='FALSE'>"
                                         + "<Default>FALSE</Default></Field>",
                                         true,
                                         AddFieldOptions.DefaultValue);

                    clientContext.ExecuteQuery();
                }
            }
        }

        private static void ChangeCustomActionRegistration()
        {
            using (var clientContext = sPContext.CreateUserClientContextForSPHost())
            {
                var query = from action in clientContext.Web.UserCustomActions
                            where action.Name == "6601a902-f458-4757-9000-09f23eaa5386.AddEmployeeToCorpDB"
                            select action;
                IEnumerable<UserCustomAction> matchingActions = clientContext.LoadQuery(query);
                clientContext.ExecuteQuery();

                UserCustomAction webScopedEmployeeAction = matchingActions.Single();

                var queryForList = from list in clientContext.Web.Lists
                                   where list.Title == "Local Employees"
                                   select list;
                IEnumerable<List> matchingLists = clientContext.LoadQuery(queryForList);
                clientContext.ExecuteQuery();

                List employeeList = matchingLists.First();
                var listActions = employeeList.UserCustomActions;
                clientContext.Load(listActions);
                listActions.Clear();

                var listScopedEmployeeAction = listActions.Add();

                listScopedEmployeeAction.Title = webScopedEmployeeAction.Title;
                listScopedEmployeeAction.Location = webScopedEmployeeAction.Location;
                listScopedEmployeeAction.Sequence = webScopedEmployeeAction.Sequence;
                listScopedEmployeeAction.CommandUIExtension = webScopedEmployeeAction.CommandUIExtension;
                listScopedEmployeeAction.Update();

                webScopedEmployeeAction.DeleteObject();

                clientContext.ExecuteQuery();
            }
        }

        private static void CreateExpectedShipmentsList()
        {
            using (var clientContext = sPContext.CreateUserClientContextForSPHost())
            {
                var query = from list in clientContext.Web.Lists
                            where list.Title == "Expected Shipments"
                            select list;
                IEnumerable<List> matchingLists = clientContext.LoadQuery(query);
                clientContext.ExecuteQuery();

                if (matchingLists.Count() == 0)
                {
                    ListCreationInformation listInfo = new ListCreationInformation();
                    listInfo.Title = "Expected Shipments";
                    listInfo.TemplateType = (int)ListTemplateType.GenericList;
                    listInfo.Url = "Lists/ExpectedShipments";
                    List expectedShipmentsList = clientContext.Web.Lists.Add(listInfo);

                    Field field = expectedShipmentsList.Fields.GetByInternalNameOrTitle("Title");
                    field.Title = "Product";
                    field.Update();

                    expectedShipmentsList.Fields.AddFieldAsXml("<Field DisplayName='Supplier'"
                                                                + " Type='Text' />",
                                                                true,
                                                                AddFieldOptions.DefaultValue);
                    expectedShipmentsList.Fields.AddFieldAsXml("<Field DisplayName='Quantity'"
                                                                + " Type='Number'"
                                                                + " Required='TRUE' >"
                                                                + "<Default>1</Default></Field>",
                                                                true,
                                                                AddFieldOptions.DefaultValue);
                    expectedShipmentsList.Fields.AddFieldAsXml("<Field DisplayName='Arrived'"
                                                               + " Type='Boolean'"
                                                               + " ShowInNewForm='FALSE'>"
                                                               + "<Default>FALSE</Default></Field>",
                                                                true,
                                                                AddFieldOptions.DefaultValue);
                    expectedShipmentsList.Fields.AddFieldAsXml("<Field DisplayName='Added to Inventory'"
                                                                + " Type='Boolean'"
                                                                + " ShowInNewForm='FALSE'>"
                                                                + "<Default>FALSE</Default></Field>",
                                                                true,
                                                                AddFieldOptions.DefaultValue);

                    clientContext.ExecuteQuery();
                }
            }
        }

        private static void RegisterExpectedShipmentsEventHandler(HttpRequest request)
        {
            using (var clientContext = sPContext.CreateUserClientContextForSPHost())
            {
                var query = from list in clientContext.Web.Lists
                            where list.Title == "Expected Shipments"
                            select list;
                IEnumerable<List> matchingLists = clientContext.LoadQuery(query);
                clientContext.ExecuteQuery();

                List expectedShipmentsList = matchingLists.Single();

                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.ReceiverName = "ExpectedShipmentsItemUpdated";
                receiver.EventType = EventReceiverType.ItemUpdated;

#if DEBUG
                receiver.ReceiverUrl = WebConfigurationManager.AppSettings["RERdebuggingServiceBusUrl"].ToString();
#else
                receiver.ReceiverUrl = "https://" + request.Headers["Host"] + "/Services/RemoteEventReceiver1.svc"; 
#endif

                expectedShipmentsList.EventReceivers.Add(receiver);

                clientContext.ExecuteQuery();
            }
        }

        private static Version GetTenantVersion()
        {
            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "GetTenantVersion";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter name = cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                name.Value = sPContext.SPHostUrl.ToString();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return new Version(reader["Version"].ToString());
                    }
                    else
                        throw new Exception("Unknown tenant: " + sPContext.SPHostUrl.ToString());
                }
            }//dispose conn and cmd
        }

        private static void SetTenantVersion(Version newVersion)
        {
            using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "UpdateTenantVersion";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter name = cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                name.Value = sPContext.SPHostUrl.ToString();
                SqlParameter version = cmd.Parameters.Add("@Version", SqlDbType.NVarChar);
                version.Value = newVersion.ToString();
                cmd.ExecuteNonQuery();
            }//dispose conn and cmd
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
