//using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;
using iTextSharp.tool.xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.Services.Description;
using iTextSharp.text.pdf.events;
using System.Globalization;
using System.Configuration;
using IronPdf;
using System.Web.Configuration;

namespace XIInfrastructure
{
    public class XIInfraEmail : XIInstanceBase
    {
        public string sOrgDatabase { get; set; }
        public string sCoreDataBase { get; set; }
        public string EmailID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string SMTP { get; set; }
        public string UserName { get; set; }
        public string sSubject { get; set; }

        public string sSurName { get; set; }
        public string sDOB { get; set; }
        public string sMobile { get; set; }
        public string sDocumentName { get; set; }
        int Port = 0;
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        XIInfraScript oIScript = new XIInfraScript();
        XIIXI oIXI = new XIIXI();

        public CResult Sendmail(int iOrgID, string sContent, List<Attachment> oAttachments)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            string Messagebody = string.Empty;
            try
            {
                XIInfraEncryption xiinfra = new XIInfraEncryption();
                XIInfraEmail oXIMail = new XIInfraEmail();
                string Mailbody = "";
                //Getting Server Details
                XIDXI oXID = new XIDXI();
                var ServerDetails = oXID.Get_IOSServerDetails(iOrgID);
                if (ServerDetails.xiStatus == 0 && ServerDetails.oResult != null)
                {
                    XIIOServerDetails oServerDetail = (XIIOServerDetails)ServerDetails.oResult;
                    MailMessage mail = new MailMessage();
                    mail.To.Add(EmailID);
                    mail.From = new MailAddress(oServerDetail.FromAddress);
                    if (sContent == null)
                    {
                        mail.Subject = "Zeeinsurance Password reset link";

                        //Encrypting Password with Email and Current Date Time.
                        string EncryptedHash = xiinfra.EncryptData(EmailID + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), true, "URP");
                        Messagebody = "http://localhost:53996/Account/ResetPasswordLink?key=" + EncryptedHash;
                        string html = @"<html><body>" + Messagebody + "</body></html>";
                        Mailbody = @"<html><body><p>";
                        Mailbody += " Hi <b>" + UserName + "</b>" + "," + "<p>We have receieved request from you to reset your login password.</p> ";
                        Mailbody += "<p> Please <a href = " + Messagebody + " style = 'text - decoration:underline; ' > Click here </a> Here to reset your password or use the link below:</p>";
                        Mailbody += "<br/ ><a>Temporary link: <a href = " + Messagebody + " > " + Messagebody + " </a></a>";
                        Mailbody += "<p> This link was expired in 24 hours. </p><span><br />If you have any queries, please contact: <a href = 'https://Zeeinsurance.co.uk/' > SystemsDNA@zeeinsurance.co.uk </a></span><br /><br /><span> Kind regards,<br />Admin <br /> Zeeinsurance </span></p></body></html >";
                    }
                    else
                    {
                        //mail.Subject = "Certificate of your policy";
                        mail.Subject = sSubject;
                        Mailbody = @sContent;
                    }
                    AlternateView altView = AlternateView.CreateAlternateViewFromString(Mailbody, null, MediaTypeNames.Text.Html);
                    mail.AlternateViews.Add(altView);
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = oServerDetail.ServerName;
                    smtp.Port = oServerDetail.Port;
                    smtp.EnableSsl = false;
                    //Chcecking Server Credentials
                    smtp.Credentials = new System.Net.NetworkCredential(oServerDetail.UserName, oServerDetail.Password);
                    if (oAttachments != null && oAttachments.Count() > 0)
                    {
                        foreach (var attachment in oAttachments)
                        {
                            mail.Attachments.Add(attachment);
                        }
                    }
                    smtp.Send(mail); //Mail Sending
                    oCResult.sMessage = "Mail Sent Successfully";
                    SaveErrortoDB(oCResult);
                }
                oCResult.oResult = oCResult;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
        public CResult GeneratePDFFile(MemoryStream MStream)
        {
            CResult oCResult = new CResult();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                MemoryStream file = MStream;
                file = new MemoryStream(file.ToArray());
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                file.Seek(0, SeekOrigin.Begin);
                Attachment data = new Attachment(file, sDocumentName + ".pdf", "application/pdf");
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.DateTime.Now;
                disposition.ModificationDate = System.DateTime.Now;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                oCResult.oResult = data;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public CResult IronPdf(string message, string sCSSFileName, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        {
            CResult oCResult = new CResult();
            try
            {
                MemoryStream output = new MemoryStream();
                HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = string.Empty;
                if (!string.IsNullOrEmpty(sCSSFileName))
                {
                    sPath = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\css\\DocumentCSS\\" + sCSSFileName + ".css";
                    Renderer.PrintOptions.CustomCssUrl = new Uri(sPath);
                }
                Renderer.PrintOptions.CssMediaType =
PdfPrintOptions.PdfCssMediaType.Print;
                //Renderer.RenderHtmlAsPdf(message).SaveAs("D:\\Test1.pdf");
                //PdfResource myPdf = Renderer.RenderHtmlAsPdf(message);
                var myPdf = Renderer.RenderHtmlAsPdf(message);
                MemoryStream memoryStream = myPdf.Stream;
                byte[] bytes = memoryStream.ToArray();
                string sSurNamePW = string.Empty; string sDOBPW = string.Empty;
                if (bIsPaswordProtected)
                {
                    if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                    {
                        if (!string.IsNullOrEmpty(sSurName))
                        {
                            int iStartPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[0]);
                            int iEndPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[1]);
                            if (sSurName.Length >= iEndPos)
                            {
                                sSurNamePW = sSurName.Substring(iStartPos, iEndPos);
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(sDOBPasswordRange))
                    {
                        if (!string.IsNullOrEmpty(sDOB))
                        {
                            if (sDOBPasswordRange == "yyyy")
                            {
                                XIIBO oBOI = new XIIBO();
                                DateTime dDate = oBOI.ConvertToDtTime(sDOB);
                                sDOBPW = dDate.Year.ToString();
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(sSurNamePW) && !string.IsNullOrEmpty(sDOBPW))
                    {
                        using (MemoryStream input = new MemoryStream(bytes))
                        {
                            string password = sSurNamePW + sDOBPW;
                            PdfReader reader = new PdfReader(input);
                            PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                            bytes = output.ToArray();
                        }
                    }
                    else
                    {
                        output = memoryStream;
                    }
                }
                else
                {
                    output = memoryStream;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = output;
                //System.IO.MemoryStream stream = HtmlToPdf.RenderHtmlAsPdf(message).Stream;
                //byte[] data = HtmlToPdf.RenderHtmlAsPdf(message).BinaryData;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public CResult PDFGenerate(string message, bool bIsPaswordProtected, string sSurNamePasswordRange, string sDOBPasswordRange)
        {
            CResult oCResult = new CResult();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                MemoryStream output = new MemoryStream();
                StringReader sr = new StringReader(message);
                Document pdfDoc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //string watermark = "Test";

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    //htmlparser.Parse(sr);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    string sSurNamePW = string.Empty; string sDOBPW = string.Empty;
                    if (bIsPaswordProtected)
                    {
                        if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                        {
                            if (!string.IsNullOrEmpty(sSurName))
                            {
                                int iStartPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[0]);
                                int iEndPos = Convert.ToInt32(sSurNamePasswordRange.Split('-')[1]);
                                if (sSurName.Length >= iEndPos)
                                {
                                    sSurNamePW = sSurName.Substring(iStartPos, iEndPos);
                                }

                            }
                        }
                        if (!string.IsNullOrEmpty(sDOBPasswordRange))
                        {
                            if (!string.IsNullOrEmpty(sDOB))
                            {
                                if (sDOBPasswordRange == "yyyy")
                                {
                                    XIIBO oBOI = new XIIBO();
                                    DateTime dDate = oBOI.ConvertToDtTime(sDOB);
                                    sDOBPW = dDate.Year.ToString();
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(sSurNamePW) && !string.IsNullOrEmpty(sDOBPW))
                        {
                            using (MemoryStream input = new MemoryStream(bytes))
                            {
                                //using (MemoryStream soutput = new MemoryStream())
                                //{
                                string password = sSurNamePW + sDOBPW;
                                PdfReader reader = new PdfReader(input);
                                PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.ALLOW_SCREENREADERS);
                                bytes = output.ToArray();
                                //}
                            }
                        }
                        else
                        {
                            output = memoryStream;
                        }
                    }
                    else
                    {
                        output = memoryStream;
                    }
                    //Response.Clear();
                    //output = memoryStream;
                    oCResult.oResult = output;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }
        #region SendSMSAlert
        public CResult SendSMSAlert(int iOrgID, string sContent, List<Attachment> oAttachments)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                if (Content == null)
                {
                    Content = "Hi";
                }
                //Getting Server Details
                XIDXI oXID = new XIDXI();
                var ServerDetails = oXID.Get_IOSServerDetails(iOrgID);
                if (ServerDetails.xiStatus == 0 && ServerDetails.oResult != null)
                {
                    XIIOServerDetails oServerDetail = (XIIOServerDetails)ServerDetails.oResult;
                    string BulkSMSPath = oServerDetail.SMSPath;
                    string SMSAPIKey = oServerDetail.SMSAPIKey;
                    string strResult = BulkSMSPath + "authkey=" + SMSAPIKey + "&mobiles=" + sMobile + "&message=" + Content;
                    WebClient WebClient = new WebClient();
                    System.IO.StreamReader reader = new System.IO.StreamReader(WebClient.OpenRead(strResult));
                    dynamic ResultHTML = reader.ReadToEnd();
                    oCResult.sMessage = "SMS Sent Successfully";
                    SaveErrortoDB(oCResult);
                }
                oCResult.oResult = oCResult;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        #endregion
    }
}