using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using System.Xml;
using System.Xml.Linq;

namespace XIScript
{
    public class CScriptDefinition
    {

        private Dictionary<string, CCodeLine> oNCodeLines = new Dictionary<string, CCodeLine>();

        private CCodeLine oTopNode = new CCodeLine();

        // Public Property Name() As String
        //     Get
        //         Return sName
        //     End Get
        //     Set(ByVal value As String)
        //         sName = value
        //     End Set
        // End Property
        public Dictionary<string, CCodeLine> NCodeLines
        {
            get
            {
                return oNCodeLines;
            }
            set
            {
                oNCodeLines = value;
            }
        }

        public CResult XILoad_Script(string sXMLFile)
        {
            CResult oMyResult = new CResult();
            //  Dim oCodeLine As New CCodeLine
            try
            {
                XmlDocument oXMLBase;
                string sProblem;
                if (System.IO.File.Exists(sXMLFile))
                {
                    // My.Computer.FileSystem.ReadAllText(sConfigXML)
                    try
                    {
                        oXMLBase = new XmlDocument();
                        oXMLBase.Load(sXMLFile);
                        foreach (XmlNode oXMLNode in oXMLBase.ChildNodes)
                        {
                            oTopNode.SerialiseNodeFromXML(oXMLNode, oTopNode);
                            //  oMyXI)
                        }

                        // xiXML2OM(oXMLBase, oMyXI)
                    }
                    catch (Exception ex)
                    {
                        sProblem = "The XML file could not be loaded due to " + "the following error:\r\n" + ex.Message;
                        oXMLBase = null;
                    }

                }
                else
                {
                    sProblem = "File " + sXMLFile + " does not exist";
                }

            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        public CResult Get_Lines_Recurse(List<XElement> qParent, CCodeLine oParentLine)
        {
            // System.Xml.Linq.XElement
            CResult oMyResult = new CResult();
            CCodeLine oCodeLine;
            CResult oCR;
            try
            {
                // TO DO - THIS IS RECURSIVE - USE XIGENERICS METHOD
                List<object> qLines = new List<object>();
                //qLines=qParent.AsQueryable().Descendants("line").SelectMany(qParent, qLines);
                //.Select(XmlNode).ToList();
                //Nodes;
                //qParent.Descendants("line");
                //switch (Nodes)
                //{
                //}
                foreach (var oResult in qLines)
                {
                    oCodeLine = new CCodeLine();
                    oCodeLine.sOperator = oResult.GetType().GetProperty("Operator").ToString();
                    oCodeLine.sResolvedValue = oResult.GetType().GetProperty("Value").ToString();
                    oCodeLine.sResult = oResult.GetType().GetProperty("Value").ToString();
                    oCR = oParentLine.AddCodeLine(oCodeLine);
                    if (oCR.xiStatus == (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiSuccess)
                    {
                        // oCR = Get_Lines_Recurse(oResult.tolist, oCodeLine)
                        // qParent = From nodes In oResult.Descendants("xiScript") Select nodes
                        // oCR = Get_Lines_Recurse(oResult.tolist, oCodeLine)
                    }

                    // oCodeLine.Name = oResult.Elements("name").Value
                    // sParamName = oCodeLine.Name
                    // oCodeLine.Type = oResult.Elements("type").Value
                    // oCodeLine.Value = oResult.Elements("value").Value
                    // oCodeLine.ParamOpt = oResult.Elements("optional").Value
                    // oNParameters.Add(sParamName, oCodeLine)
                }

            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiError;
                oMyResult.sMessage = "ERROR: [" + this.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - "
                            + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            }

            return oMyResult;
        }

        public string Get_Class()
        {
            System.Reflection.MethodBase mb;
            Type Type;
            string sFullReference = "";
            mb = System.Reflection.MethodBase.GetCurrentMethod();
            Type = mb.DeclaringType;
            sFullReference = Type.FullName;
            return sFullReference;
        }

        public CResult TemplateFunction()
        {
            CResult oMyResult = new CResult();
            try
            {
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiError;
                oMyResult.sMessage = "ERROR: [" + this.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - "
                    + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            }

            return oMyResult;
        }

        public string Serialise_FromOM()
        {
            string sResult = String.Empty;
            // Dim sConcat As String = ""
            // sConcat = sName & vbCrLf
            // For Each oParam In NParameter
            //     sConcat = sConcat & "Param  :  " & oParam.Value.Name & vbCrLf & vbTab & oParam.Value.Type & vbCrLf & vbTab & oParam.Value.Value & vbCrLf & vbTab & oParam.Value.ParamOpt & vbCrLf
            // Next
            // sConcat = sConcat & vbCrLf
            // For Each oSteps In NStep
            //     sConcat = sConcat & "Step  : " & oSteps.Value.Name & vbCrLf & vbTab & oSteps.Value.Type & vbCrLf & vbTab & oSteps.Value.QueryName & vbCrLf & vbTab & oSteps.Value.Resolve & vbCrLf
            //     For Each oStepNod In oSteps.Value.NStepNode
            //         sConcat = sConcat & vbTab & "StepNode : " & oStepNod.Value.Name & vbCrLf & vbTab & vbTab & oStepNod.Value.NodeName & vbCrLf & vbTab & vbTab & oStepNod.Value.UIDParam & vbCrLf
            //         For Each oStepGrp In oStepNod.Value.NStepGroup
            //             sConcat = sConcat & vbTab & vbTab & "StepGroup : " & oStepGrp.Value.Load & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.List & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.Edit & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.Lock & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.OrderBy & vbCrLf
            //         Next
            //     Next
            //     For Each oStepNRef In oSteps.Value.NStepNodeRef
            //         sConcat = sConcat & vbTab & "StepNodeRef : " & oStepNRef.Value.StepName & vbCrLf & vbTab & vbTab & vbTab & oStepNRef.Value.StepNode & vbCrLf
            //     Next
            //     For Each oStepBtn In oSteps.Value.NButton
            //         sConcat = sConcat & vbTab & "Buttons:" & oStepBtn.Value.Link & vbCrLf & vbTab & vbTab & vbTab & oStepBtn.Value.Text & vbCrLf
            //     Next
            //     sConcat = sConcat & vbTab & oSteps.Value.NextStep & vbCrLf
            //     sConcat = sConcat & vbTab & oSteps.Value.ListStep & vbCrLf
            //     sConcat = sConcat & vbCrLf
            // Next
            // AppFlowFormat = sConcat
            return sResult;
        }
    }
}