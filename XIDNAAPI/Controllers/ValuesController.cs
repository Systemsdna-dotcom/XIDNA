using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XICore;
using XISystem;

namespace XIDNAAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
        XIInfraCache oCache = new XIInfraCache();
        CResult oCResult = new CResult();
        XIDBO oBOD = new XIDBO();
        XIDefinitionBase oXID = new XIDefinitionBase();
        string sReceiver = "slgm";

        public XIContentEditors LoadTemplate(string sTemplateID)
        {
            XIContentEditors oContent = new XIContentEditors();
            List<XIContentEditors> oContentDef = new List<XIContentEditors>();
            if (!string.IsNullOrEmpty(sTemplateID))
            {
                oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sTemplateID, null);
                if (oContentDef != null && oContentDef.Count() > 0)
                {
                    oContent = oContentDef.FirstOrDefault();
                }
            }
            return oContent;
        }
        public string GetMails(string sKey)
        {
            string sFunction = "xi.s|{xi.a|'XIConfig_T','" + sKey + "','sValue','','sName'}";
            CResult oCRSLMG = new CResult();
            XIDScript oXIScript = new XIDScript();
            oXIScript.sScript = sFunction.ToString();
            oCRSLMG = oXIScript.Execute_Script("", "");
            return oCRSLMG.oResult.ToString();
        }
        public CResult SendLeadMail(XIContentEditors oContent, XIBOInstance oInstance, XIIBO oBOI, string sEmail, string o1ClickID)
        {
            XIInfraEmail oEmail = new XIInfraEmail();
            XIContentEditors oContentEditor = new XIContentEditors();
            CResult Result = oContentEditor.MergeContentTemplate(oContent, oInstance);
            string sContext = XIConstant.Lead_Transfer;
            oEmail.EmailID = sEmail;
            oEmail.From = oContent.sFrom;
            oEmail.Bcc = oContent.sBCC;
            oEmail.cc = oContent.sCC;
            oEmail.sSubject = oContent.sSubject;// + oInstance.AttributeI("id").sValue;

            oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + oEmail.EmailID + "" });
            //var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, oBOI.AttributeI("id").iValue, "", 0, oContent.bIsBCCOnly);//send mail with attachments
            var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, 44541, null, 0, oContent.bIsBCCOnly);

            oCResult.oTraceStack.Add(new CNV { sName = "Lead Transfer", sValue = "Lead Transferred" });
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            oBOI.BOD = oBOD;
            if (oMailResult.bOK && oMailResult.oResult != null)
            {
                //insert  Lead life cycle event
                XIIBO oBOILifeCycle = new XIIBO();
                XIDBO oBODLifeCycle = new XIDBO();
                oBODLifeCycle = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "LifeCycle", null);
                oBOILifeCycle.BOD = oBODLifeCycle;
                oBOILifeCycle.SetAttribute("FKiLeadID", oBOI.AttributeI("id").sValue);
                oBOILifeCycle.SetAttribute("sFrom", oBOI.AttributeI("iStatus").sResolvedValue);
                oBOILifeCycle.SetAttribute("sTo", "Junk");
                var LifeCycle = oBOILifeCycle.Save(oBOILifeCycle);
                // update lead
                oBOI.SetAttribute("iTransferStatus", "20"); //Transfer Completed
                oBOI.SetAttribute("iTransferRestrict", "10"); //Disabled
                oBOI.SetAttribute("iJunkResaonID", "90");
                oBOI.SetAttribute("iStatus", "50");  //junk status
                var res = oBOI.Save(oBOI);
                if (res.bOK && res.oResult != null)
                {
                    XIIBO oBOITransferTransaction = new XIIBO();
                    XIDBO oBODTransferTransaction = new XIDBO();
                    oBODTransferTransaction = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "TransferTransaction", null);
                    oBOITransferTransaction.BOD = oBODTransferTransaction;
                    oBOITransferTransaction.SetAttribute("FKiLeadID", oBOI.AttributeI("id").sValue);
                    oBOITransferTransaction.SetAttribute("FKi1ClickID", o1ClickID);
                    oBOITransferTransaction.SetAttribute("sContent", Result.oResult.ToString());
                    oBOITransferTransaction.SetAttribute("sReceiver", sReceiver);
                    oBOITransferTransaction.SetAttribute("sEmail", sEmail);
                    var Transaction = oBOITransferTransaction.Save(oBOITransferTransaction);
                }
                oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + oEmail.EmailID + "" });
                oXID.SaveErrortoDB(oCResult);
            }
            else
            {
                oBOI.SetAttribute("iTransferStatus", "30"); //Transfer Error
                oBOI.SetAttribute("iTransferRestrict", "0"); //None
                oBOI.Save(oBOI);
                oCResult.oTraceStack.Add(new CNV { sName = "Mail not sended ", sValue = "Mail not sended to email:" + oEmail.EmailID + "" });
                oXID.SaveErrortoDB(oCResult);
            }
            return oMailResult;
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("api/values/LeadTransfer/")]
        public string LeadTransfer()
        {
            oCResult = new CResult();
            XIIXI oIXI = new XIIXI();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                string sNewGUID = Guid.NewGuid().ToString();

                //Load Templates
                string sTemplateID = "Quote searcher";
                XIContentEditors oQuoteSearcher = LoadTemplate(sTemplateID);
                sTemplateID = "Lead Transfer";
                XIContentEditors oKompare = LoadTemplate(sTemplateID);
                XIContentEditors oSLGM = LoadTemplate(sTemplateID);

                //Get Mails
                string SLGM = GetMails("LeadTransferMail");
                string QuoteSearcher = GetMails("LeadTransferMail");
                string Kompare = GetMails("LeadTransferMail");

                XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Transfer transaction", null);
                XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                //Get BO Definition
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                o1ClickC.BOD = oBOD;
                //Get Headings of 1-Click
                //o1ClickC.Get_1ClickHeadings();
                Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                sReceiver = oRes.Values.Select(x => x.AttributeI("sreceiver").sValue).FirstOrDefault();
                string sOneClickName = "Lead Transfer";
                 o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                 o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                //Get BO Definition
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                o1ClickC.BOD = oBOD;
                //Get Headings of 1-Click
                //o1ClickC.Get_1ClickHeadings();
               oRes = o1ClickC.OneClick_Execute();
                XIIBO oBOI = new XIIBO();
                for (int i = 1; i <= oRes.Values.Count(); i++)
                {
                    oBOI = oRes.ElementAt(i).Value;
                    XIBOInstance oBOIns = new XIBOInstance();
                    List<XIIBO> nBOI = new List<XIIBO>();
                    if (oBOI.iBODID == 0)
                    {
                        oBOI.iBODID = o1ClickC.BOID;
                    }
                    nBOI.Add(oBOI);
                    oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                    oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                    CResult oMailResult = new CResult();
                    if (sReceiver == "slgm")
                    {
                        var oLIst = oIXI.BOI("QS Instance", oBOI.AttributeI("FKiQSInstanceID").sValue);  //Quote searcher
                        oBOIns = oLIst.Structure("NotationStructure").XILoad();
                        oMailResult = SendLeadMail(oQuoteSearcher, oBOIns, oBOI, QuoteSearcher, o1ClickC.ID.ToString());
                        sReceiver = "quote searcher";
                    }
                    else
                    {
                        oMailResult = SendLeadMail(oSLGM, oBOIns, oBOI, SLGM, o1ClickC.ID.ToString()); //SLGM
                        sReceiver = "slgm";
                    }
                    oMailResult = SendLeadMail(oKompare, oBOIns, oBOI, Kompare, o1ClickC.ID.ToString());  //Kompare
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.oTraceStack.Add(new CNV { sName = "Lead Transfer", sValue = "Error: In Lead Transfer Method" + oCResult.sMessage });
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oXID.SaveErrortoDB(oCResult);
            }
            return "";
        }
        private bool MergeTemplate()
        {
            return true;
        }
    }
}