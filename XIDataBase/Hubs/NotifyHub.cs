using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
//using Microsoft.AspNet.SignalR;
using XIDNA.Models;

namespace XIDataBase.Hubs
{
    //public class NotifyHub : Hub
    //{
    //    public void Send(string Message)
    //    {
    //        var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
    //        // Call the addNewMessageToPage method to update clients.
    //        context.Clients.All.addNewMessageToPage(Message);
    //    }

    //    public void UpdateFeed(string sMessage, string Time, string sMesID, int iLayoutID, string BOInfo, string sOrgs, int Org, string sMode, string sType, bool bUpdate)
    //    {
    //        var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
    //        // Call the addNewMessageToPage method to update clients.
    //        context.Clients.All.UpdateFeed(sMessage, Time, sMesID, iLayoutID, BOInfo, sOrgs, Org, sMode, sType, bUpdate);
    //    }

        //public void Query_Debug(string sQuery, string sIdentity)
        //{
        //    var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
        //    // Call the addNewMessageToPage method to update clients.
        //    context.Clients.All.QueryDebug(sQuery, sIdentity);
        //}


    //    public override Task OnConnected()
    //   {
    //        // Add your own code here.
    //        // For example: in a chat application, record the association between
    //        // the current connection ID and user name, and mark the user as online.
    //        // After the code in this method completes, the client is informed that
    //        // the connection is established; for example, in a JavaScript client,
    //        // the start().done callback is executed.
    //        return base.OnConnected();
    //    }
    //    public override Task OnDisconnected( bool bISDisconnect)
    //    {
    //        // Add your own code here.
    //        // For example: in a chat application, mark the user as offline, 
    //        // delete the association between the current connection id and user name.
    //        return base.OnDisconnected(bISDisconnect);
    //    }
    //    public override Task OnReconnected()
    //    {
    //        // Add your own code here.
    //        // For example: in a chat application, you might have marked the
    //        // user as offline after a period of inactivity; in that case 
    //        // mark the user as online again.
    //        return base.OnReconnected();
    //    }
    //    public void BCOneCommand(BCOneCommand Message)
    //    {
    //        var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
    //        // Call the addNewMessageToPage method to update clients.
    //        context.Clients.All.BCOneCommand(Message);
    //    }
    //}
}