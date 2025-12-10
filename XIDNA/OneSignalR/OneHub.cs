using Microsoft.AspNet.SignalR;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Interop;
using XICore;
using XIDNA.Controllers;
using XIDNA.Models;
using XIDNA.Repository;
using Microsoft.Azure.NotificationHubs;
using XISystem;
using Newtonsoft.Json;
using Notification = XIDNA.Models.Notification;
using Microsoft.SqlServer.Management.Smo;
using System.EnterpriseServices;
using System.Web.Mvc;

namespace XIDNA.OneSignalR
{
    public class OneHub:Hub
    {
        CommonRepository Common = new CommonRepository();
        public void Send(string name, string message)
        {
            Clients.All.sendMessage(name, message);
        }
        public void Update(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public void LeaveGroup(string groupName)
        {
            Groups.Remove(Context.ConnectionId, groupName);
        }
        public void SendMessageToGroup(string groupName, string ProjectID, string message, string userID, string ChattypeID, string FilePath, string FileAliasName,string CategoryID, string chatID, string userName, string Assinginguserid,string BOSelection, bool bThumbsIcon)
        {
            //string userID = "0";
            //string ChattypeID = "";    
            //string FilePath= "";
            //string FileAliasName= "";
            try
            {
                XIComponentsController obj = new XIComponentsController();
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.UserID= userID;
                chatMessage.ReceiverID= Assinginguserid;
                chatMessage.Message = message;
                chatMessage.ProjectID= ProjectID;
                chatMessage.ChatTypeID= ChattypeID;
                chatMessage.FileAliasName= FileAliasName;
                chatMessage.FilePath= FilePath;
                chatMessage.CategoryID= CategoryID;
                chatMessage.ChatID = chatID;
                chatMessage.UserName= userName;
                chatMessage.BOSelection= BOSelection;
                chatMessage.bThumbsIcon = bThumbsIcon;
                // var result = obj.SaveChatMsg(chatMessage);
                // obj.Save_ChatMessage(chatMessage);
                var result = obj.SaveChatMsg(chatMessage) as JsonResult;
                CNV BOID = null;
                var BOName = "";
                if (result != null && result is JsonResult jsonResult && jsonResult.Data is List<CNV> dataList)
                {
                    if (!string.IsNullOrEmpty(BOSelection)) { 
                     BOID = dataList.FirstOrDefault(x => x.sName == "FKiBOIID");
                     BOName = dataList.FirstOrDefault(x => x.sName == "BOName").sValue;}
                    
                    if (bThumbsIcon) { chatID = dataList.FirstOrDefault(x => x.sName == "FKIChatID").sValue;}
                    else {chatID = dataList.FirstOrDefault(x => x.sName == "id").sValue; }
                    
                }
                    Clients.Group(groupName).sendMessage(userID, userName, message,ChattypeID, CategoryID, FilePath, FileAliasName, ProjectID, chatID, BOSelection, BOID?.sValue, BOName, bThumbsIcon);
                var group = obj.Get_ConProjectID(ProjectID);
                if (group != null) { 
                  //  SendPushNotification(string token, DevicePlatform platform, string msg) 
                        }                        
                // Clients.Group(groupName).sendMessage(ProjectID, message, userID, ChattypeID, FilePath, FileAliasName, CategoryID, chatID, userName);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void SendMessageAppToGroup(string groupName, string ProjectID, string message, string userID, string ChattypeID, string CategoryID, string chatID, string userName, bool bThumbsIcon)
        {
            string FilePath = "";
            string FileAliasName = "";
            try
            {
                XIComponentsController obj = new XIComponentsController();
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.UserID = userID;
                chatMessage.Message = message;
                chatMessage.ProjectID = ProjectID;
                chatMessage.ChatTypeID = ChattypeID;
                chatMessage.CategoryID = CategoryID;
                chatMessage.ChatID = chatID;
                chatMessage.UserName = userName;
                //obj.Save_ChatMessage(chatMessage);
                Clients.Group(groupName).sendMessage(userID, userName, message, ChattypeID, CategoryID, FilePath, FileAliasName, ProjectID, chatID, bThumbsIcon);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateChatInfo(string message, string sConnectionID)
        {
            try
            {
                Clients.Client(sConnectionID).UpdateChatInfo(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override Task OnConnected()
        {
            XIComponentsController obj = new XIComponentsController();
            ConnectionStatus constatus = new ConnectionStatus();
            constatus.sConnectionId = Context.ConnectionId;
            constatus.sUserID =Context.User.Identity.Name;
            constatus.iStatus = true;
            obj.Save_ConStatus(constatus);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool bISDisconnect)
        {
            XIComponentsController obj = new XIComponentsController();
            ConnectionStatus constatus = new ConnectionStatus();
            constatus.sConnectionId = "";
            constatus.sUserID = Context.User.Identity.Name;
            constatus.iStatus = false;
            obj.Save_ConStatus(constatus);
            return base.OnDisconnected(bISDisconnect);
        }
        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }
        private async Task SendPushNotification(string token, DevicePlatform platform, string payload)
        {
            switch (platform)
            {
                case DevicePlatform.Android:

                    var notfiObj = new NotificationAlert()
                    {
                        data = new Data()
                        {
                            notification = new Notification()
                            {
                                alert = "New Message",
                                backgroundImage = "demo.png",
                                backgroundImageTextColour = "#FFFFFF",
                                badge = 1,
                                body = "Login Request",
                                category = "Authentication",
                                channel = "BrokerStar",
                                colour = "White",
                                groupIcon = "test",
                                groupKey = "BrokerStar",
                                groupSummary = "BrokerStar",
                                groupTitle = "BrokerStar",
                                icon = "icon.png",
                                largeIcon = "icon.png",
                                priority = "high",
                                sound = "default",
                                TimeToLiveInSeconds = 120,
                                style = new Style()
                                {
                                    image = "icon.png",
                                    lines = new System.Collections.Generic.List<string>() { "New Message" },
                                    text = "BrokerStar",
                                    type = "text"
                                },
                                title = "New Message",
                                vibrate = "true"
                            },
                            payload = payload
                        },
                    };

                    FcmNotification fcmNotification = new FcmNotification(JsonConvert.SerializeObject(notfiObj));
                    await Notifications.Instance.Hub.SendDirectNotificationAsync(fcmNotification, token);

                    break;
                case DevicePlatform.iOS:
                    {
                        var notification = new ApnsNotification()
                        {
                            aps = new Aps()
                            {
                                alert = new Alert()
                                {
                                    body = "New Message",
                                    title = "new message"
                                },
                                badge = 1,
                                sound = "default",
                                vibrate = "true",
                                ContentAvialble = "1"
                            },
                            payload = payload
                        };
                        AppleNotification iosfcmNotification = new AppleNotification(JsonConvert.SerializeObject(notification));
                        iosfcmNotification.Priority = 10;
                        iosfcmNotification.ContentType = "application/json";
                        await Notifications.Instance.Hub.SendDirectNotificationAsync(iosfcmNotification, token);
                    }
                    break;
            }
        }
    }
}