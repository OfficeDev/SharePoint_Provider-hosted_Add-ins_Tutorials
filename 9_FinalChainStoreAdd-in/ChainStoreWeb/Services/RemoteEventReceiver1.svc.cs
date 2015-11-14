using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Data.SqlClient;
using System.Data;
using ChainStoreWeb.Utilities;

namespace ChainStoreWeb.Services
{
    public class RemoteEventReceiver1 : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, 
        /// such as when a user is adding or deleting a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles events that occur after an action occurs, 
        /// such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            switch (properties.EventType)
            {
                case SPRemoteEventType.ItemUpdated:

                    switch (properties.ItemEventProperties.ListTitle)
                    {
                        case "Expected Shipments":

                            bool updateComplete = TryUpdateInventory(properties);
                            if (updateComplete)
                            {
                                RecordInventoryUpdateLocally(properties);
                            }
                            break;
                    }
                    break;
            }
        }

        private bool TryUpdateInventory(SPRemoteEventProperties properties)
        {
            bool successFlag = false;

            try
            { 
                var arrived = Convert.ToBoolean(properties.ItemEventProperties.AfterProperties["Arrived"]);
                var addedToInventory = Convert.ToBoolean(properties.ItemEventProperties.AfterProperties["Added_x0020_to_x0020_Inventory"]);

                if (arrived && !addedToInventory)
                {

                    using (SqlConnection conn = SQLAzureUtilities.GetActiveSqlConnection())
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "UpdateInventory";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter tenant = cmd.Parameters.Add("@Tenant", SqlDbType.NVarChar);
                        tenant.Value = properties.ItemEventProperties.WebUrl + "/";
                        SqlParameter product = cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 50);
                        product.Value = properties.ItemEventProperties.AfterProperties["Title"]; // not "Product"
                        SqlParameter quantity = cmd.Parameters.Add("@Quantity", SqlDbType.SmallInt);
                        quantity.Value = Convert.ToUInt16(properties.ItemEventProperties.AfterProperties["Quantity"]);
                        cmd.ExecuteNonQuery();
                    }
                    successFlag = true;
                }
            }
            catch (KeyNotFoundException)
            {
                successFlag = false;
            }
            return successFlag;
        }

        private void RecordInventoryUpdateLocally(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                List expectedShipmentslist = clientContext.Web.Lists.GetByTitle(properties.ItemEventProperties.ListTitle);
                ListItem arrivedItem = expectedShipmentslist.GetItemById(properties.ItemEventProperties.ListItemId);
                arrivedItem["Added_x0020_to_x0020_Inventory"] = true;
                arrivedItem.Update();
                clientContext.ExecuteQuery();
            }
        }
    }
}