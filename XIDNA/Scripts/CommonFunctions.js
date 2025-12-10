var ContentURL;
var DataURL;
var PopupURL;
var PopupORDialogURL;
var DialogURL;
var EditBOURL;
var SemanticURL;
var ComponentURL;
var StepURL;
var LabelDataURL;
var AutoCompleteLarge;
var AutoCompleteURL;
var AutoCreateLayoutURL;
var editpopups = new Array();
var Poppopups = new Array();
var AddEditLayoutURL;
var LayoutMappingsURL;
var AddEditXiLinkURL;
var XiLinkListByOrgURL;
var ViewXiLinkURL;
var LayoutDetailsURL;
var SaveEditBOURL;
var SaveBOGridURL;
var SaveBOFormURL;
var AddEditXiParameterURL;
var DiaOpacity;
var UpdateXIParamsURL;
var StepContentURL;
var StepDataURL;
var sOnLoadGUID;
var ComponentParmsByStepURL;
var UpdateInstanceIDURL;
var CallQSEventURL;
var ParentGUIDURL;
var XILinkLoadURL;
var XILinkLoadJsonURL;
var XIContentURL;
var XIContentLoadURL;
var DeleteBOURL;
var LoadLayoutURL;
var PCAPredicate_Key = "JT94-CJ69-WR48-NE81";//"NZ19-YM61-WZ73-XW9"; //"NZ19-YM61-WZ73-XW94";
var MakeaChangeURL;
var LoadStepURL;
var LoadStepContentURL;
var CurrentStepURL;
var ExecuteFollowUpAlgURl;
var ActRec = '';
var EnumReconciliation = '';
var Type = '';
var AutoCompleteDict = [];
var sRowId = "";
var sRowNo = "";
var SetParamUrl;
var XILinkDefinitionURL;
var PopupDefinitionURL;
var OpenInPopupURL;
var UpdateLockURL;
var SaveGridURL;
var RemoveSectionURL;
var sImagePath;
var ExecuteXIScriptURL;
var ExecuteXIAlgorithmURL;
var Execute1ClickDeleteAlgURL;
var ExecuteProcessControllerURL;
var SecureKeyURL;
var ChangeFields;
var QSInsDict = {};
var bUIDRef = "";
var PaymentCalls = [];
var GetCacheParameterValueURL;
var GetQSDefinitionURL;
var GetTemplateDefinitionURL;
var TriggerXILinkURL;
var AddTriggerXILinkURL;
var RemoveStepXIIValuesURL;
var XIPreviewURL;
var UpdateQuoteRankURL;
var GetGridDataURL;
var OverRideStatusDict = {};
var SaveMultiRowURL;
var GetNextHTMLRowsURL;
var GetParentDDLURL;
var GetXIScriptEditorURL;
var GetXIScriptValidatorURL;
var LoadScriptToEditorURL;
var AddNewScriptURL;
var CodeEditor;
var GetDependentFieldValuesURL;
var FilterInstanceTreeURL;
var DispalyTemplate;
var TriggerBOActionURL;
var iBODID;
var LoadSectionContentURL;
var GetInstanceTreeURL;
var GetBOInstanceIDURL;
var GetBOAttributeValueURL;
var HTMLMergeURL;
var GetCacheDataURL;
var DownloadDocURL;
var GetDependencyDropDownSearch;
var GetBoAttributeDependencyDropDown;
var PriceCalculationURL;
var GetBOIURL;
var GetBONameURL;
var LoadFeedURL;
var PrintPDFURL;
var LoginAccountComponentURL;
var SaveAccountComponentURL;
var ForgotPasswordURL;
var ResetPasswordURL;
var GetApplicationLayoutURL;
var LandingPageUrl;
var sReference = "";
var CheckLinkAccessURL;
var GetDefaultPopupURL;
var SchemaCompareforDB;
var DBDataCompare;
var TableSelectionCompare;
var TempDBCreation;
var TestConnectionString;
var DBNonZeroDataCompare;
var NonZeroTargetInsertUpdate;
var CssUrl;
var CodeEditorURL;
var CodeEditorDialogURL;
var CodeEditorContentURL;
var SnoozeLimit;
var GetDynamicDefaultPopupURL;
function CustomMessage(Message, Status) {
    $('.StatusMessages').remove();
    $('.innerborder ').prepend($('<div class="StatusMessages"></div>'));
    if (Status == true) {
        $('.StatusMessages').html('<div class="alert alert-success">' + Message + '</div>');
    }
    else if (Status == false) {
        $('.StatusMessages').html('<div class="alert alert-danger">' + Message + '</div>');
    }
    else {
        $('.StatusMessages').html('');
    }
}

function AddDialogToTaskbar(DialogName) {
    var Dname = 'button[data-name="' + DialogName + '"]';
    var ButtonExists = $("#DialogTaskbar").find(Dname);
    if (ButtonExists.length > 0) {

    }
    else
        $('#DialogTaskbar').append('<li class="list-group-item"><button type="button" class="btn btn-primary btn-xs dialogtaskbarbtn" data-status="open" data-name="' + DialogName + '">Dialog</button></li>').addClass('btn-visible');
}

var sTreeGUID = 0;
var sCurrentGUID = 0;
var sLayoutType = "";

function fncGetGUIDFromHTMLTree(parentName, childObj) {
    var HTMLObj = childObj.parentNode;
    var count = 1;
    if (HTMLObj) {
        while (HTMLObj.getAttribute('data-name') != parentName) {
            if (HTMLObj.parentNode.tagName != "HTML") {
                HTMLObj = HTMLObj.parentNode;
                count++;
            }
            else {
                return sTreeGUID
            }
        }
        sTreeGUID = HTMLObj.getAttribute('data-guid');
        sLayoutType = HTMLObj.getAttribute('data-type');
        if (sLayoutType && sLayoutType.length > 0 && sLayoutType == "independent") {
            return sTreeGUID;
        }
        if (sCurrentGUID == 0) {
            sCurrentGUID = sTreeGUID;
        }
        var sParentGUID = fncGetGUIDFromHTMLTree(parentName, HTMLObj);
        if (sParentGUID && sParentGUID.length > 0 && sParentGUID != 0) {
            sTreeGUID = sParentGUID;
            return sTreeGUID;
        }
        else {
            return sTreeGUID;
        }
    }
}


function fncGetContextFromHTMLTree(parentName, childObj) {
    var Context = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-name') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return Context
        }
    }
    Context = testObj.getAttribute('data-context');
    return Context;
}

function fncGetComponentNameFromHTMLTree(parentName, childObj) {
    var ActiveInstanceID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-type') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return ActiveInstanceID;
        }
    }
    ActiveInstanceID = testObj.getAttribute('data-name');
    return ActiveInstanceID;
}

function fncIsDivAlreadyExists(parentName, childObj) {
    var AllDivs = $("#" + parentName).children('div.' + childObj);
    if (AllDivs.length > 0) {
        return true;
    }
    return false;
}

function fncGetQSIDFromHTMLTree(parentName, childObj) {
    var Context = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-info') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return Context
        }
    }
    Context = testObj.getAttribute('data-value');
    return Context;
}


function XIRun($this, XiLinkID, ID, sGUID, BO, IsMerge, BODID, iQSDID, MenuName, oParams, bISActivity) {
    if ($this) {
        $($this).attr('disabled', 'true');
    }
    fncResloveLinkSecureKey(XiLinkID).then(function (iLinkID) {
        fncCheckLinkAccess(iLinkID).then(function (bAllowed) {
            XiLinkID = iLinkID;
            if (!bAllowed) {
                return false;
            }
            if (BO != null && (BO.toLowerCase() == "XIAlgorithmLines_T".toLowerCase() || BO.toLowerCase() == "EnvironmentDetails_T".toLowerCase() || BO.toLowerCase() == "Lead_T".toLowerCase())) {
                var formid = $($this).closest('form').attr('id');
                var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
                formData = processFormData(formData, formid);
                var Param = {};
                if (oParams == undefined) {
                    oParams = [];
                    var CList = $($this).closest('tr');
                    if (CList && CList.length > 0) {
                        var ClassList = $($this).closest('tr')[0].classList;
                        if (ClassList != undefined && ClassList.length > 1 && ClassList[1].startsWith('simpleclick_')) {
                            Param = {};
                            Param["sName"] = "id";
                            Param["sValue"] = ClassList[1].split('_')[1];
                            oParams.push(Param);
                            Param = {};
                            Param["sName"] = "{XIP|iInstanceID}";
                            Param["sValue"] = ClassList[1].split('_')[1];
                            oParams.push(Param);
                        }
                    }
                }
                for (var i = 0; i < formData.length; i++) {
                    Param = {};
                    Param["sName"] = formData[i].name;
                    Param["sValue"] = formData[i].value;
                    Param["sContext"] = formData[i].sMapField;
                    oParams.push(Param);
                }
            }
            iBODID = BODID;
            //if (XiLinkID) {
            //    if (typeof XiLinkID != "string") {

            //    }
            //    else {
            //        $.ajax({
            //            type: 'POST',
            //            url: SecureKeyURL,
            //            data: { sSecureKey: XiLinkID },
            //            cache: false,
            //            async: true,
            //            dataType: 'json',
            //            success: function (oResponse) {
            //                debugger
            //                XiLinkID = oResponse;
            //            },
            //            error: function (err) {

            //            }
            //        });
            //    }
            //}
            if (MenuName) {
                if (MenuName.toLowerCase() == 'Charges Drawdown'.toLowerCase()) {
                    ActRec = 'chargesdrawdown';
                    EnumReconciliation = '40'; Type = "10";
                } else if (MenuName.toLowerCase() == 'Commission Drawdown'.toLowerCase()) {
                    ActRec = 'commissiondrawdown';
                    EnumReconciliation = '30'; Type = "20";
                } else if (MenuName.toLowerCase() == 'Premium Finance'.toLowerCase()) {
                    ActRec = 'reconcilepf';
                    EnumReconciliation = '20'; Type = "30";
                } else if (MenuName.toLowerCase() == 'Supplier Payments'.toLowerCase()) {
                    ActRec = 'insurerpay';
                    EnumReconciliation = '10'; Type = "40";
                } else if (MenuName.toLowerCase() == 'Pre-Banking'.toLowerCase()) {
                    ActRec = 'prebankrec';
                    EnumReconciliation = '45'; Type = "45";
                } else if (MenuName.toLowerCase() == 'Bank Reconcillation'.toLowerCase()) {
                    ActRec = 'bankrec';
                    EnumReconciliation = '50'; Type = "50";
                } else if (MenuName.toLowerCase() == 'Policy Payment Charge'.toLowerCase()) {
                    ActRec = 'pfpayrec';
                    EnumReconciliation = '70'; Type = "70";
                } else if (MenuName.toLowerCase() == '50/50 Payment Charge'.toLowerCase()) {
                    ActRec = 'ffpayrec';
                    EnumReconciliation = '80'; Type = "80";
                }
            }
            //if(ActiveInstanceID >0)
            //ID = sGUID;
            if (XiLinkID) {
                if (XiLinkID.toString().length > 0) {
                    $.ajax({
                        type: 'GET',
                        //url: '@Url.Action("GetXiLinkData", "XiLink")',
                        url: DataURL,
                        contentType: "application/json; charset=utf-8",
                        data: { XiLinkID: XiLinkID, sGUID: sGUID, IsMerge: IsMerge },
                        async: true,
                        cache: false,
                        dataType: 'json',
                        //beforeSend: function (request) {
                        //    $.blockUI({ message: '<h3 class="nh3"><img src="/Scripts/ckfinder/plugins/gallery/colorbox/images/loading.gif" width="50px" /> Please wait while loading...</h3>' });
                        //},
                        success: function (data) {
                            var sXILinkType = data.sType;
                            var Output = "";
                            var SuccessMsg = "";
                            var FailedMsg = "";
                            var Popup = "";
                            var Dialog = "";
                            var DialogID = "";
                            var PopupID = "";
                            var my = "";
                            var at = "";
                            var My1 = "Center";
                            var My2 = "Center";
                            var At1 = "Center";
                            var At2 = "Center";
                            var TransValue = 0;
                            var IsScrollToOutput = "";
                            for (var i = 0; i < data.XiLinkNVs.length; i++) {
                                if (data.XiLinkNVs[i].Name == "Output") {
                                    Output = data.XiLinkNVs[i].Value;
                                }
                                if (data.XiLinkNVs[i].Name == "SuccessMessage") {
                                    SuccessMsg = data.XiLinkNVs[i].Value;
                                }
                                if (data.XiLinkNVs[i].Name == "FailedMessage") {
                                    FailedMsg = data.XiLinkNVs[i].Value;
                                }
                                if (data.XiLinkNVs[i].Name == "IsScrollToOutput") {
                                    IsScrollToOutput = data.XiLinkNVs[i].Value;
                                }
                                //if (data.XiLinkNVs[i].Name.toLowerCase() == "startaction".toLowerCase())
                                if (data.XiLinkNVs[i].Name == "StartAction" || data.XiLinkNVs[i].Name == "DialogID" || data.XiLinkNVs[i].Name == "PopupID" || data.XiLinkNVs[i].Name == "My1" || data.XiLinkNVs[i].Name == "My2" || data.XiLinkNVs[i].Name == "At1" || data.XiLinkNVs[i].Name == "At2" || data.XiLinkNVs[i].Name == "my" || data.XiLinkNVs[i].Name == "at") {
                                    if (data.XiLinkNVs[i].Value == "Popup") {
                                        Popup = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "PopupID") {
                                        PopupID = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Value == "Dialog") {
                                        Dialog = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "DialogID") {
                                        DialogID = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "My1") {
                                        var My1 = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "My2") {
                                        var My2 = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "At1") {
                                        var At1 = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "At2") {
                                        var At2 = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "my") {
                                        var my = data.XiLinkNVs[i].Value;
                                    }
                                    else if (data.XiLinkNVs[i].Name == "at") {
                                        var at = data.XiLinkNVs[i].Value;
                                    }
                                }
                            }
                            if (Popup.length > 0) {
                                //$.unblockUI();
                                $.ajax({
                                    type: 'POST',
                                    url: PopupURL,
                                    data: { ID: PopupID },
                                    cache: false,
                                    async: true,
                                    dataType: 'json',
                                    success: function (Popupdata) {
                                        var PopGUID = CreateGuid();

                                        window.open(PopupORDialogURL + '?XiLinkID=' + XiLinkID + '&BOID=' + BOID + '&sGUID=' + PopGUID + '&ID=' + ID, '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                                        if (bISActivity != undefined && bISActivity == "yes") {
                                            $.ajax({
                                                type: 'POST',
                                                url: SaveAuditBO,
                                                data: { iBODID: 17, sAuditBOName: "Audit_T", sBOName: "ACPolicy_T", sInstanceID: "", sAuditContent: MenuName + " opened", sGUID: sGUID },
                                                cache: false,
                                                async: true,
                                                dataType: 'json',
                                                success: function (data) {
                                                }
                                            });
                                        }
                                        //$.ajax({
                                        //    type: 'POST',
                                        //    url: PopupORDialogURL,
                                        //    data: { XiLinkID: XiLinkID, BOID: BOID, sGUID: PopGUID, ID: ID },
                                        //    cache: false,
                                        //    async: false,
                                        //    dataType: 'html',
                                        //    success: function (result) {
                                        //        
                                        //    }
                                        //});
                                    }
                                });
                            }
                            else if (Dialog.length > 0) {
                                if (bISActivity != undefined && bISActivity == "yes") {
                                    $.ajax({
                                        type: 'POST',
                                        url: SaveAuditBO,
                                        data: { iBODID: 17, sAuditBOName: "Audit_T", sBOName: "ACPolicy_T", sInstanceID: "", sAuditContent: MenuName + " opened", sGUID: sGUID },
                                        cache: false,
                                        async: true,
                                        dataType: 'json',
                                        success: function (data) {
                                        }
                                    });
                                }
                                if (my.length > 0 && at.length > 0) {
                                    var my = my;
                                    var at = at;
                                }
                                else {
                                    var my = My1 + " " + My2;
                                    var at = At1 + " " + At2;
                                }
                                $.ajax({
                                    type: 'POST',
                                    url: DialogURL,
                                    data: { ID: DialogID },
                                    cache: false,
                                    async: true,
                                    dataType: 'json',
                                    success: function (Dialogdata) {
                                        var TValue = Dialogdata.iTransparency;
                                        TransValue = TValue / 100;
                                        Transparent(TransValue);
                                        var DialogOpacityValue = DiaOpacity;
                                        sOnLoadGUID = CreateGuid();
                                        var onvParams;
                                        if (oParams != undefined && oParams != null) {
                                            onvParams = oParams;
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            url: PopupORDialogURL,
                                            data: { XiLinkID: XiLinkID, sGUID: sGUID, sNewGuid: sOnLoadGUID, BO: BO, sID: ID, PRDID: DialogID, BODID: BODID, ActRec: ActRec, EnumReconciliation: EnumReconciliation, Type: Type, oNVParams: onvParams },
                                            cache: false,
                                            async: true,
                                            dataType: 'html',
                                            success: function (result) {
                                                //$.unblockUI();
                                                if (Dialogdata.PopupSize == "Default") {
                                                    //Dialogdata.DialogHeight = screen.height - 190;
                                                    //Dialogdata.DialogWidth = screen.width - 50;
                                                    Dialogdata.DialogHeight = $(window).height() * 0.9;//
                                                    Dialogdata.DialogWidth = $(window).width() * 0.9;//
                                                }
                                                if (Dialogdata.IsGrouping == false) {
                                                    var DialogDivID = "ResultDialog-" + sOnLoadGUID;
                                                    $('.' + DialogDivID).dialog('destroy').remove();
                                                    var Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a></div>';
                                                    $('#Dialogs').append(Div);
                                                    var dlgBg = $('#dialog-background');
                                                    if (dlgBg && dlgBg.length == 0) {
                                                        $('body').append('<div id="dialog-background"></div>');
                                                    }
                                                    var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h"></i>';
                                                    var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v"></i>';
                                                    var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt"></i>';
                                                    var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt"></i>';
                                                    var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
                                                    if (Dialogdata.IsMinimiseIcon == true) {
                                                        var MinDia = '<i class="Minimize fa fa-window-minimize"></i>';
                                                    }
                                                    else {
                                                        var MinDia = "";
                                                    }
                                                    if (Dialogdata.IsMaximiseIcon == true) {
                                                        var MaxDia = '<i class="Maximize far fa-window-maximize"></i>';
                                                    }
                                                    else {
                                                        var MaxDia = "";
                                                    }
                                                    var RestoreDia = '<i class="RestoreDown far fa-window-restore"></i>';
                                                    $("." + DialogDivID).html(result);
                                                    $("." + DialogDivID).dialog({
                                                        title: ' ',
                                                        //modal: true,
                                                        height: Dialogdata.DialogHeight,
                                                        width: Dialogdata.DialogWidth,
                                                        resizable: Dialogdata.IsResizable,
                                                        IsCloseIcon: Dialogdata.IsCloseIcon,
                                                        BarPosition: Dialogdata.BarPosition,
                                                        dialogClass: 'titlebardata one-custom-dialog ' + DialogOpacityValue,
                                                        buttons: [
                                                        ],
                                                        open: function () {
                                                            $(this).parent().promise().done(function () {
                                                                $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                                                                $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + MinDia + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                                                                $(this).children('.ui-dialog-title').html('<div class="dialogtitleholder"></div>');
                                                                //<span class="fc-red">Alert message !!!</span> <span class="fc-green">Success message !!!</span>
                                                                $(this).children('.ui-dialog-titlebar').find(".RestoreDown").hide();
                                                                $(this).children('.ui-dialog-titlebar').find(".windowminWidth").hide();
                                                                $(this).children('.ui-dialog-titlebar').find(".windowminHeight").hide();
                                                                if ($(this).hasClass("sideSpaceDialog")) {
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 395,
                                                                        top: 70,
                                                                        left: 275,
                                                                    }, 0);
                                                                    //$(window).resize(function(){
                                                                    $(".dialog-box").animate({ height: screen.height });
                                                                    //}).trigger('resize');

                                                                }                                                                    
                                                                if ($('body').hasClass("inbox_kpi_show")) {
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 685,
                                                                        top: 70,
                                                                        left: 275
                                                                    }, 0);
                                                                }else{
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 395,
                                                                        top: 70,
                                                                        left: 275
                                                                    }, 0);
                                                                };

                                                                if (data.sType == "QS") {
                                                                    fncShowDialogTitle(MenuName, sOnLoadGUID, DialogDivID);
                                                                }
                                                                else {
                                                                    if (MenuName) {
                                                                        $(this).find('.ui-dialog-title').html('<span class="fc-head">' + MenuName + '</span>');
                                                                    }
                                                                }
                                                            })
                                                        },
                                                        close: function (event, ui) {
                                                            //$('#Dialogs').parent().hide();
                                                            $("." + DialogDivID).dialog('close');
                                                            $("." + DialogDivID).dialog('destroy').remove();
                                                        }
                                                    }).dialog("widget")
                                                        .draggable({
                                                            //containment: [left, top, right, bottom]
                                                            containment: [-screen.width + 600, 0, screen.width - 200, screen.height - 200],
                                                            start: function () {
                                                                $(this).data("startingScrollTop", $(this).parent().scrollTop());
                                                                $(this).parent().scrollTop();
                                                            },
                                                            drag: function (event, ui) {
                                                                var st = parseInt($(this).data("startingScrollTop"));
                                                                ui.position.top -= $(this).parent().scrollTop() - st;
                                                            }
                                                        }).css({ position: "fixed" })
                                                        .dblclick(function () {
                                                            //if ($(this).hasClass("actualScreenToggle")) {
                                                            //    $(this).removeClass("actualScreenToggle");
                                                            //    $(this).addClass("fullScreenToggle");
                                                            //    var maxIcon = $(this).find('i.Maximize');
                                                            //    fncdialogchange(maxIcon, "maximize");
                                                            //}
                                                            //else {
                                                            //    $(this).removeClass("fullScreenToggle");
                                                            //    $(this).addClass("actualScreenToggle");
                                                            //    var restoreIcon = $(this).find('i.RestoreDown');
                                                            //    fncdialogchange(restoreIcon, "restore");
                                                            //}
                                                        });
                                                    //$("." + DialogDivID).position({my: "center",at: "center",of: window});
                                                    $("." + DialogDivID).dialog("widget").resizable({ handles: "n, e, s, w" });
                                                    $("." + DialogDivID).dialog('option', 'position', 'center');
                                                    //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 150 });
                                                    //$(".PopupTabContentArea, #TreeScrollbar").height(Dialogdata.DialogHeight - 150);
                                                    $("." + DialogDivID).parent().find(".Maximize").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            //width: screen.width - 30,
                                                            //height: screen.height - 150,
                                                            width: $(window).width(),
                                                            height: $(window).height(),
                                                            top: 0,
                                                            bottom: 0,
                                                            left: 0,
                                                            right: 0,
                                                        });
                                                        $("." + DialogDivID).dialog({ dialogClass: 'classAbsolute', });
                                                        $("." + DialogDivID).closest(".ui-dialog").removeClass("classFixed");
                                                        $("." + DialogDivID).parent().find(".RestoreDown").show();
                                                        $("." + DialogDivID).parent().find(".windowWidth,.windowHeight,.Maximize").hide();
                                                        $("." + DialogDivID).parent().find(" .dialog-box, .Minimize,.windowminWidth, .windowminHeight").show();
                                                        //$(".PopupTabContentArea, #TreeScrollbar").height(screen.height - 150);
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(screen.height - 170)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(screen.height - 170 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: screen.height - 170 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: screen.height - 250 });
                                                        //$(".dialog-box").height($(window).height() - 100);
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: screen.height });											
                                                        //$(".ui-dialog, .dialog-box").animate({ height: screen.height, });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate(({ height: Dialogdata.DialogHeight - 150 }));
                                                        //$(".ui-dialog").animate({ height: screen.height - 10, });
                                                        //$(".dialog-box").animate({ height: screen.height - 50, });
                                                        $(".dialog-box").animate({ height: screen.height });
                                                    });
                                                    $("." + DialogDivID).parent().find(".RestoreDown").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            Maximize: Dialogdata.IsMaximiseIcon,
                                                            Minimize: Dialogdata.IsMinimizeIcon,
                                                            height: Dialogdata.DialogHeight,
                                                            width: Dialogdata.DialogWidth,
                                                            //top: '39.5px',
                                                            //left: '25px',
                                                            top: '5%',
                                                            left: '5%',
                                                        })
                                                        $("." + DialogDivID).parent().find(".RestoreDown,.windowminWidth, .windowminHeight").hide();
                                                        $("." + DialogDivID).parent().find(".Maximize,.windowWidth, .windowHeight").show();
                                                        //$("." + DialogDivID).dialog('option', 'position', 'center');
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        $(".dialog-box").animate({ height: Dialogdata.DialogHeight - 36 });
                                                    });
                                                    $("." + DialogDivID).parent().find(".Minimize").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            height: '35px',
                                                            top: screen.height - 155,
                                                            bottom: 0,
                                                            left: 0,
                                                            width: 800,
                                                        }, 500);
                                                        $("." + DialogDivID).parent().find(".RestoreDown").hide();
                                                        $("." + DialogDivID).parent().find(".Maximize").show();
                                                        $("." + DialogDivID).dialog({ dialogClass: 'classFixed', });
                                                        $("." + DialogDivID).closest(".ui-dialog").removeClass("classAbsolute");
                                                        $("." + DialogDivID).parent().find(".windowWidth, .windowHeight, .dialog-box, .Minimize").hide();
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        $(".dialog-box").animate({ height: Dialogdata.DialogHeight - 36 });
                                                    });
                                                    $("." + DialogDivID).parent().find(".windowWidth").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            width: screen.width,
                                                            left: 0,
                                                            right: 0,
                                                        });
                                                        $("." + DialogDivID).parent().find(".windowminWidth").show();
                                                        $("." + DialogDivID).parent().find(".windowWidth").hide();
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        //$(".dialog-box").animate({ height: Dialogdata.DialogHeight - 36 });
                                                        $(".dialog-box").animate({ height: screen.height });
                                                    });
                                                    $("." + DialogDivID).parent().find(".windowHeight").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            //height: screen.height,
                                                            height: $(window).height(),
                                                            top: 0,
                                                            bottom: 0,
                                                        });
                                                        $("." + DialogDivID).parent().find(".windowminHeight").show();
                                                        $("." + DialogDivID).parent().find(".windowHeight").hide();
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        //$(".dialog-box").animate({ height: Dialogdata.DialogHeight });
                                                        $(".dialog-box").animate({ height: screen.height });
                                                    });
                                                    $("." + DialogDivID).parent().find(".windowminWidth").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            //height: 'auto',
                                                            //width: '800px',
                                                            width: Dialogdata.DialogWidth,
                                                            //top: '39.5px',
                                                            left: '5%',
                                                        });
                                                        $("." + DialogDivID).parent().find(".windowWidth").show();
                                                        $("." + DialogDivID).parent().find(".windowminWidth").hide();
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        //$(".dialog-box").animate({ height: Dialogdata.DialogHeight - 36 });
                                                        $(".dialog-box").animate({ height: screen.height });

                                                    });
                                                    $("." + DialogDivID).parent().find(".windowminHeight").click(function () {
                                                        $(this).parents('.ui-dialog').animate({
                                                            //height: '590px',
                                                            height: Dialogdata.DialogHeight,
                                                            //width: '800px',
                                                            //width: Dialogdata.DialogWidth,
                                                            top: '5%',
                                                        });
                                                        $("." + DialogDivID).parent().find(".windowminHeight").hide();
                                                        $("." + DialogDivID).parent().find(".windowHeight").show();
                                                        var TabHeight = $('#TabsArea').height();
                                                        //$(".PopupTabContentArea_0").height(Dialogdata.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(".PopupTabContentArea_" + sOnLoadGUID).height(Dialogdata.DialogHeight - 70 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$("#TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 70 });
                                                        //$(".PopupTabContentArea, #TreeScrollbar").animate({ height: Dialogdata.DialogHeight - 100, });
                                                        $(".dialog-box").animate({ height: Dialogdata.DialogHeight - 36 });
                                                    });
                                                }
                                                else {
                                                    var DialogDivID = "ResultDialog-" + DialogID + "-" + ID;
                                                    //$('.' + DialogDivID).dialog('destroy').remove();
                                                    var Div = "";
                                                    var AnyDivs = $('#DialogContent').find('div');
                                                    if (AnyDivs.length > 0) {
                                                        Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message" data-dname="' + DialogDivID + '" style="display:none;"></div>';
                                                    }
                                                    else {
                                                        Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message" data-dname="' + DialogDivID + '"></div>';
                                                    }
                                                    var IsDivExists = fncIsDivAlreadyExists('DialogContent', DialogDivID);
                                                    if (!IsDivExists) {
                                                        $('#DialogContent').append(Div);
                                                        $("." + DialogDivID).html(result);
                                                    }
                                                    var dlgBg = $('#dialog-background');
                                                    if (dlgBg && dlgBg.length == 0) {
                                                        $('body').append('<div id="dialog-background"></div>');
                                                    }
                                                    //sOnLoadGUID = null;
                                                    BarPosition = Dialogdata.BarPosition;
                                                    $('#VerticalLeftBar').hide();
                                                    $('#VerticalRightBar').hide();
                                                    $('#HorizontalTopBar').hide();
                                                    $('#HorizontalBottonBar').hide();
                                                    var BtnID = "DialogBarBtn-" + DialogID + "-" + ID;
                                                    var IsExists = "No";
                                                    //var VBar = '<button type="button" class="btn btn-primary btn-xs dialogBtn" id="' + BtnID + '" >Dialog' + ID + '</button>';
                                                    var VBar = '<button type="button" class="btn btn-dark btn-xs dialogBtn" id="' + BtnID + '" ><i class="fa fa-user-plus" ></i><span class="" >' + ID + '</span></button>';
                                                    //var VBar = '<div class="po-Left"><button type="button" class="btn btn-primary btn-xs dialogBtn po-LeftLink" id="' + BtnID + '" ><i class="fa fa-home"></i></button><div class="po-LeftContent hidden"><div class="po-LeftBody">Dialog' + ID + '</div></div></div>';
                                                    var Placement;
                                                    //if (BarPosition == "Vertical Left") {
                                                    //    Placement = 'right';
                                                    //}
                                                    //else if (BarPosition == "Vertical Right") {
                                                    //    Placement = 'left';
                                                    //}
                                                    //else if (BarPosition == "Horizontal Top") {
                                                    //    Placement = 'bottom';
                                                    //}
                                                    //else if (BarPosition == "Horizontal Bottom") {
                                                    //    Placement = 'top';
                                                    //}

                                                    //$('.po-Left > .po-LeftLink').popover({
                                                    //    trigger: 'hover',
                                                    //    html: true,
                                                    //    content: function () {
                                                    //        return $(this).parent().find('.po-LeftBody').html();
                                                    //    },
                                                    //    container: 'body',
                                                    //    placement: Placement
                                                    //});
                                                    if (Dialogdata.BarPosition == "Vertical Left") {
                                                        $('#VerticalLeftBar button').each(function (j, obj) {
                                                            var id = $(this).attr('id');
                                                            if (id == BtnID) {
                                                                IsExists = "Yes";
                                                            }
                                                        });
                                                        if (IsExists == "No") {
                                                            $('#VerticalLeftBar').append(VBar);
                                                        }
                                                        $('#VerticalLeftBar').show();
                                                        $('#DialogContent').addClass('col-xs-11');
                                                    }
                                                    else if (Dialogdata.BarPosition == "Vertical Right") {
                                                        $('#VerticalRightBar button').each(function (j, obj) {
                                                            var id = $(this).attr('id');
                                                            if (id == BtnID) {
                                                                IsExists = "Yes";
                                                            }
                                                        });
                                                        if (IsExists == "No") {
                                                            $('#VerticalRightBar').append(VBar);
                                                        }
                                                        $('#VerticalRightBar').show();
                                                        $('#DialogContent').addClass('col-xs-11');
                                                    }
                                                    else if (Dialogdata.BarPosition == "Horizontal Top") {
                                                        $('#HorizontalTopBar button').each(function (j, obj) {
                                                            var id = $(this).attr('id');
                                                            if (id == BtnID) {
                                                                IsExists = "Yes";
                                                            }
                                                        });
                                                        if (IsExists == "No") {
                                                            $('#HorizontalTopBar').append(VBar);
                                                        }
                                                        $('#HorizontalTopBar').show();
                                                        $('#DialogContent').addClass('col-xs-12');
                                                    }
                                                    else if (Dialogdata.BarPosition == "Horizontal Bottom") {
                                                        $('#HorizontalBottonBar button').each(function (j, obj) {
                                                            var id = $(this).attr('id');
                                                            if (id == BtnID) {
                                                                IsExists = "Yes";
                                                            }
                                                        });
                                                        if (IsExists == "No") {
                                                            $('#HorizontalBottonBar').append(VBar);
                                                        }
                                                        $('#HorizontalBottonBar').show();
                                                        $('#DialogContent').addClass('col-xs-12');
                                                    }
                                                    $("#DialogGroups").dialog({
                                                        title: ' ',
                                                        height: Dialogdata.DialogHeight,
                                                        width: Dialogdata.DialogWidth,
                                                        resizable: Dialogdata.IsResizable,
                                                        IsCloseIcon: Dialogdata.IsCloseIcon,
                                                        BarPosition: Dialogdata.BarPosition,
                                                        dialogClass: DialogOpacityValue,
                                                        position: { my: my, at: at },
                                                        buttons: [
                                                        ],
                                                        close: function (event, ui) {
                                                            $('#DialogGroups').parent().hide();
                                                            $(this).dialog('destroy').remove();
                                                        }
                                                    });
                                                }
                                                if ($this) {
                                                    $($this).removeAttr('disabled');
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                            else if (sXILinkType == "QuestionSet" || sXILinkType == "Question Set") {
                                $.ajax({
                                    url: ContentURL,
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    datatype: "HTML",
                                    cache: false,
                                    async: false,
                                    data: JSON.stringify({ XiLinkID: XiLinkID, sGUID: sGUID, BODID: BODID, ID: ID, oNVParams: oParams }),
                                    success: function (data) {
                                        if (Output.length > 0) {
                                            if (Output == "BtnMsg" || Output == "QSBtnMsg" || Output == "OneclickBtnMsg") {
                                                if (data == "Success") {
                                                    if (Output == "BtnMsg") {
                                                        fncGetCacheData(sGUID).then(function (oCache) {
                                                            var _uidialog = $('.LayoutCode_' + sGUID);
                                                            TriggerComponent(sGUID, "FormComponent", oCache, "", _uidialog);
                                                        })
                                                    }
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).html('');
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).show();
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).html('<div class="alert alert-success">' + SuccessMsg + '</div>');
                                                    //if (BO && BO.toLowerCase() == 'xidocumenttree') {
                                                    //    $('.colorbar').removeClass('red');
                                                    //    $('.colorbar').removeClass('amber');
                                                    //    $('.colorbar').addClass('green');
                                                    //}
                                                    $(function () {
                                                        setTimeout(function () {
                                                            $('.LayoutCode_' + sGUID).find('#' + Output).hide('blind', {}, 100)
                                                        }, 1000);
                                                    });
                                                }
                                                else {
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).html('');
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).show();
                                                    $('.LayoutCode_' + sGUID).find('#' + Output).html('<div class="alert alert-danger">' + FailedMsg + '</div>');
                                                    $(function () {
                                                        setTimeout(function () {
                                                            $('.LayoutCode_' + sGUID).find('#' + Output).hide('blind', {}, 100)
                                                        }, 1000);
                                                    });
                                                }
                                            }
                                            else {
                                                $('.LayoutCode_' + sGUID).find('#' + Output).html(data);
                                            }
                                            if (IsScrollToOutput.length > 0 && IsScrollToOutput.toLowerCase() == "yes") {
                                                $('html,body').animate({
                                                    scrollTop: $('.LayoutCode_' + sGUID).find('#' + Output).offset().top
                                                }, 1000);
                                            }
                                            else {
                                                $('html, body').animate({
                                                    scrollTop: $('body').offset().top //#DIV_ID is an example. Use the id of your destination on the page
                                                }, 'slow');
                                            }
                                        }
                                        //$.unblockUI();
                                        else {
                                            $('#QSStep_' + sGUID).html(data);
                                        }
                                    }
                                });
                            }
                            else if (Output.length > 0) {
                                $.ajax({
                                    url: ContentURL,
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    datatype: "HTML",
                                    cache: false,
                                    async: true,
                                    data: JSON.stringify({ XiLinkID: XiLinkID, sGUID: sGUID, BODID: BODID, ID: ID, oNVParams: oParams }),
                                    success: function (data) {
                                        //$.unblockUI();
                                        $('#' + Output.replace(/ /g, '_')).html(data);
                                    },
                                    error: function (data) {
                                    }
                                });
                            }
                            else if (data.FKiComponentID > 0) {
                                fncLoadComponent(data.FKiComponentID, null, sGUID, null, XiLinkID, BODID);
                            }
                            else {
                                $.ajax({
                                    url: ContentURL,
                                    type: 'POST',
                                    contentType: "application/json; charset=utf-8",
                                    datatype: "json",
                                    async: true,
                                    cache: false,
                                    data: JSON.stringify({ XiLinkID: XiLinkID, sGUID: sGUID, oNVParams: oParams }),
                                    success: function (data) {
                                        if (data && MenuName == 'autologin') {
                                            $('.ResponseMsg').html('<div class="LayoutCode_' + sGUID + '" data-guid="' + sGUID + '" data-name="LayoutGUID">' + data + '</div>');
                                        }
                                        else if (data) {
                                            $('.ResponseMsg').html(data);
                                            $('.Confirmation').show();
                                            $('.Confirmation').html(data);
                                        }
                                    },
                                    error: function (data) {
                                    }
                                })
                            }
                        }
                    });
                }
            }
        })
    })
}

function SaveData($this, sBOName, sShowSaveSections, bIsListAdd, ParentXIGUID, Parent1ClickXIGUID) {
    $($this).attr('disabled', 'true');
    var GUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var Context = fncGetContextFromHTMLTree('LayoutGUID', $this);
    SaveBO($this, null, GUID, Context, sBOName, false, sShowSaveSections, bIsListAdd, ParentXIGUID, Parent1ClickXIGUID).then(function (status) {
        var formid = $($this).parents("form").attr("id");
        var sIsRefresh = $($this).attr("data-IsRefresh");
        if (sIsRefresh && sIsRefresh.toLowerCase() == "Yes".toLowerCase()) {
            $('#' + formid).each(function () {
                this.reset();
                $('.LayoutCode_' + GUID).find('#root').html('');
            });
        }
        var sIsRefreshParent = $($this).attr("data-IsRefreshParentQS");
        if (sIsRefreshParent && sIsRefreshParent.length > 0) {
            var RefreshParent = sIsRefreshParent.split('_');
            if (RefreshParent.length == 2 && RefreshParent[0].toLowerCase() == "yes".toLowerCase()) {
                var oQSInstance = QSInsDict[RefreshParent[1]];
                //fncGetNextStep(StepIns, sGUID, StepIns.bIsLastStep);
                fncNavigateStep(oQSInstance.iCurrentStepIDXIGUID, RefreshParent[1], oQSInstance.XIGUID)
            }
        }
    });
}
function SaveGridData($this, BOI, sBOName) {
    $($this).attr('disabled', 'true');
    var GUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var Context = fncGetContextFromHTMLTree('LayoutGUID', $this);
    SaveEditBOURL = SaveBOGridURL;
    SaveBO($this, null, GUID, Context, sBOName, true);
}

function processFormData(formData, formid) {
    var sTagArray = new Array("input", "select", "textarea", "checkbox");

    var data = formData;
    var oDataArray = [];
    data.forEach(function (object) {
        var oData = object;
        $.each(sTagArray, function (i, option) {
            $('#' + formid + " " + option).each(function () {
                if (this.name == object.name) {
                    var sName = this.name.split('_')[0] + "_" + "sPreviousValue";
                    var nonformData = $(this).data("spreviousvalue");
                    var bIsValid = $(this).data("isvalid");
                    oData.sPreviousValue = nonformData;
                    oData.bIsValid = bIsValid;
                    oData.sMapField = this.title;
                }
            });
            oDataArray.push(oData);
        });
    });
    return formData;
}

//$(document.body).on('click', 'button.InlineEditSaveBtn', function () {
function SaveBO($this, oBOInstance, sGUID, sContext, sBOName, bIsGrid, sShowSaveSections, bIsListAdd, ParentXIGUID, Parent1ClickXIGUID) {
    return new Promise(function (resolve, reject) {
        var bIsValid = true;
        var Location = fncGetFormTypeFromHTMLTree('XISemanticDiv', $this);
        var XISemanticID = fncGetSemanticIDFromHTMLTree("XISemanticDiv", $this);
        var StepID = fncGetStepIDFromHTMLTree("XISemanticStepDiv", $this);
        var formid = "";
        if (sRowId != "" && sRowNo != "") {
            sRowNo = sRowNo.split('_')[0];
            formid = $("#" + sRowId).closest("form").attr('id');
        }
        else {
            formid = $($this).closest('form').attr('id');
        }
        var sFormGuid = formid.split('_')[1];
        var InputParams = [];
        var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
        formData = processFormData(formData, formid);
        var FormValues = [];
        var titleid = [];
        for (var i = 0; i < formData.length; i++) {
            if (sRowId != "" && sRowNo != "") {
                var attr = formData[i].name.split('_')[0];
                if (attr == sRowNo) {
                    //FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
                    FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
                }
            }
            else {
                //FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
                FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
            }
        }
        $.each(FormValues, function (i, option) {
            if (option.bIsValid == "false") {
                bIsValid = false;
            }
        });
        var mappedvalues = "";
        $('#' + formid).find('.mapbox').find('ul#rightValues').find('li').each(function (i, obj) {
            mappedvalues = mappedvalues + obj.id + ",";
        })
        var mappedname = $('#' + formid).find('.mapbox').attr('data-name');
        if (mappedvalues.length > 0) {
            mappedvalues = mappedvalues.substr(0, mappedvalues.length - 1);
            FormValues.push({ sName: mappedname, sValue: mappedvalues, bDirty: true });
        }
        var serialized = $('#' + formid).find('input[type=checkbox]').map(function () {
            return { name: $(this).attr('data-attrname'), value: this.checked ? "True" : "False", sPreviousValue: $(this).attr('data-previousvalue') };
        });
        for (var i = 0; i < serialized.length; i++) {
            var result = FormValues.filter(function (x) { return x.sName === serialized[i].name; });
            if (result.length > 0) {
                //FormValues.filter(function (x) { return x.sName === serialized[i].name; }).Data = serialized[i].value;
                FormValues.filter(function (x) {
                    if (x.sName === serialized[i].name) {
                        x.sValue = serialized[i].value;
                    }
                    return x.sName === serialized[i].name
                })
            }
            else {
                FormValues.push({ sName: serialized[i].name, sValue: serialized[i].value, sPreviousValue: serialized[i].sPreviousValue, bDirty: true });
            }
        }
        var ddlserialized = $('#' + formid).find('select').map(function () {
            return { name: this.name, value: this.value, DerivedValue: $(this).find('option:selected').text(), sPreviousValue: $(this).attr('data-previousvalue') };
        });
        for (var i = 0; i < ddlserialized.length; i++) {
            var result = FormValues.filter(function (x) { return x.sName === ddlserialized[i].name; });
            //var result = FormValues.filter(obj => {
            //    return obj.sName === ddlserialized[i].name
            //})
            if (result.length > 0) {

            }
            else {
                FormValues.push({ sName: ddlserialized[i].name, sValue: ddlserialized[i].value, sPreviousValue: ddlserialized[i].sPreviousValue, bDirty: true });
            }
        }
        //for (var j = 0; j < FormValues.length; j++) {
        //    debugger
        //    if (FormValues[j].sName.indexOf('-editor') > 0) {
        //        var mcetext = tinyMCE.get(sFormGuid + '_' + FormValues[j].sName).getContent();
        //        var sFName = FormValues[j].sName.split('-')[0];
        //        FormValues.push({ sName: sFName, sValue: mcetext, bDirty: true });
        //    }
        //}
        for (var j = 0; j < FormValues.length; j++) {
            debugger;
            //if (FormValues[j].sName.indexOf('-editor-') > 0) {
            //    var editorId = sFormGuid + '_' + FormValues[j].sName;
            //    var editorInstance = tinyMCE.get(editorId);
            //    if (editorInstance) {
            //        var mcetext = editorInstance.getContent();
            //        var sFName = FormValues[j].sName.split('-')[0];
            //        FormValues.push({ sName: sFName, sValue: mcetext, bDirty: true });
            //    } else {
            //        console.warn('TinyMCE editor not found for ID:', editorId);
            //    }
            //}
            if (FormValues[j].sName.indexOf('-editor') > 0) {
                var sFName = FormValues[j].sName.split('-')[0];
                FormValues.push({ sName: sFName, sValue: FormValues[j].sValue, bDirty: true });
            }

        }

        if (bIsGrid && bIsGrid == false) {
            var ddlserialized = $('#' + formid).find('select').map(function () {
                return { name: this.name, value: this.value, sPreviousValue: this.sPreviousValue };
                //return { name: this.name, value: this.value };
            });
            for (var i = 0; i < ddlserialized.length; i++) {
                var bFound = false;
                for (var m = 0; m < FormValues.length; m++) {
                    if (FormValues[m].sName == ddlserialized[i].name) {
                        bFound = true;
                    }
                }
                if (!bFound) {
                    FormValues.push({
                        sName: ddlserialized[i].name, sValue: ddlserialized[i].value, sPreviousValue: ddlserialized[i].sPreviousValue, bDirty: true
                    });
                }
            }
        }

        //for (var k = 0; k < FormValues.length ; k++) {
        //    var AttrI = oBOInstance.Attributes[FormValues[k].sName];
        //    AttrI.sValue = FormValues[k].sValue;
        //    oBOInstance.Attributes[FormValues[k].sName] = AttrI;
        //}

        //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
        var sMode = $('#' + formid).find('#FormMode').attr('data-value');
        var sUpdateAction = $('#' + formid).find('#UpdateAction').attr('data-value');
        if (sUpdateAction && sUpdateAction.length > 0) {
            var param1 = {};
            param1["sName"] = 'sUpdateAction';
            param1["sValue"] = sUpdateAction;
            InputParams.push(param1);
        }
        var sSaveType = "";
        if (bIsListAdd && bIsListAdd.length > 0) {
            var add = bIsListAdd.split('_');
            if (add.length == 2) {
                if (add[0] == "grid") {
                    sSaveType = "grid";
                }
            }
        }
        if (sMode && sMode.length > 0) {
            sSaveType = sMode;
        }
        var sHierarchy = $('#' + formid).find('input[type=hidden]').attr('data-hierarchy');
        if (!sHierarchy)
            sHierarchy = null;
        var pvalue = {
            Attributes: FormValues,
            sGUID: sGUID,
            sContext: sContext,
            sBOName: sBOName,
            sSaveType: sSaveType,
            sHierarchy: sHierarchy,
            oParams: InputParams
        }
        if (!bIsValid) {
            $($this).removeAttr('disabled');
        }
	if (sBOName == 'bcoChat') {
            debugger
            var hub = $.connection.oneHub;
            var msg=$("#sMessage").val();
            var name = $('#sessionUserName span').text();
            hub.invoke("Send", name , msg).done(function (err) {
                //$("#divmsg").html("Test Hi");
                $("#divmsg").html(name+"<br>"+ msg);
            });
        }
        if (bIsValid) {
            $.ajax({
                url: SaveEditBOURL,
                //url: '@Url.Action("EditData", "XiLink")',
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                cache: false,
                data: JSON.stringify(pvalue),
                success: function (data) {
                    $(".simple1clickInline").hide();
                    //$(".field-errmsg").find('span').html("");
                    $("#" + formid).find(".field-errmsg").find('span').html("");
                    $("#" + formid).find(".highlight--help").removeClass("msg-error");
                    //$(".highlight--help").removeClass("msg-error");
                    var IsError = false;
                    if (data.length > 0) {
                        var sFormGuid = formid.split('_')[1];
                        for (var len = 0; len < data.length; len++) {
                            var iBODID = data[len].BOD.BOID;
                            if (data[len].sErrorMessage == "No Access") {
                                IsError = true;
                                $('#' + formid).find(".StatusMessages").hide();
                                $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">You are not allowed to make changes</div>');
                            }
                            else if (data[len].sErrorMessage != "Failure") {
                                var IsSuccess = true;
                                var Error = "";
                                //var Scripts = data[len].BOD.Scripts;
                                var Scripts = data[len].oScriptErrors;
                                var sBoName = data[len].sBOName;
                                if (sReference && sReference.toLowerCase() == "xiguid") {
                                    sBoName = data[len].BOD.XIGUID.toString();
                                }

                                if (Scripts != null) {
                                    if (Object.keys(Scripts).length > 0) {
                                        IsError = true;
                                        for (var i = 0; i < Object.keys(Scripts).length; i++) {
                                            $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).closest('.highlight--help').addClass('msg-error');
                                            $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).html(Object.values(Scripts)[i]);
                                        }
                                    }
                                }

                                //if (Object.keys(Scripts).length > 0) {
                                //    for (var i = 0; i < Object.keys(Scripts).length; i++) {
                                //        //Object.values(Scripts)[i].IsSuccess
                                //        if (!Object.values(Scripts)[i].IsSuccess) {
                                //            var sBoName = data[len].sBOName;

                                //            var sFieldName = Object.values(Scripts)[i].sFieldName;
                                //            for (var j = 0; j < Object.values(Scripts)[i].ScriptResults.length; j++) {
                                //                if (Object.values(Scripts)[i].ScriptResults[j].iType == 30) {
                                //                    $('span.' + sBoName + "-" + sFieldName).html(Object.values(Scripts)[i].ScriptResults[j].sUserError);

                                //                    IsSuccess = false;
                                //                    CustomMessage("", null);
                                //                    //Error = Error + Scripts[i].ScriptResults[j].sMessage + "<br/>";
                                //                }
                                //            }
                                //        }
                                //    }
                                //}

                                if (IsSuccess) {
                                    //if (data[len].BOD.BOID == 2472) {
                                    //    EasyAlert.Easy({
                                    //        text: "New Data Inserted",
                                    //        time: "5000",
                                    //        bkgrndColour: "#ee6363",
                                    //        textColour: "#FFFFFF"
                                    //    });
                                    //}
                                    fncGetCacheData(sGUID).then(function (oCache) {
                                        //var _uidialog = fncgetInlineLayoutFromHTMLTree('sys-layout', $this);
                                        var _uidialog = fncgetDialogFromHTMLTree('dialog', $this);
                                        TriggerComponent(sGUID, "FormComponent", oCache, "", _uidialog);
                                    })
                                    //var AllAttrs = data[len].Attributes;
                                    var iID = data[len].iInstanceID;
                                    var sBOUID = data[len].BOD.BOID + "_" + iID;
                                    var sPK = data[len].BOD.sPrimaryKey;
                                    //var iID = AllAttrs[sPK.toLowerCase()].sValue;
                                    var BOIDXIGUID = data[len].BOIDXIGUID.toString();
                                    if (sReference && sReference.toLowerCase() == "xiguid") {
                                        iID = data[len].XIGUID;
                                        sBOUID = data[len].BOD.XIGUID.toString() + "_" + iID;
                                        //sPK = "XIGUID";
                                    }
                                    $('#' + formid).find('input[name=' + sPK.toLowerCase() + ']').val(iID);
                                    $('#' + formid).find('input[name=tr' + len + '_' + sPK.toLowerCase() + ']').val(iID);
                                    $('#' + formid).find('input[name=tr' + len + '_' + sPK.toLowerCase() + ']').css('display', 'block');
                                    //$('#' + formid).find("input[name=tr+"len"+_+" + sPK"]").val(iID);
                                    var Obj = document.getElementById(sRowId);
                                    var ComponentName = "";
                                    if (sRowId != "") {
                                        ComponentName = fncGetComponentNameFromHTMLTree('Component', Obj);
                                    }
                                    else {
                                        ComponentName = fncGetComponentNameFromHTMLTree('Component', $this);
                                    }
                                    if (ComponentName && ParentGUIDURL) {
                                        $.ajax({
                                            url: ParentGUIDURL,
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            datatype: "json",
                                            cache: false,
                                            async: false,
                                            data: JSON.stringify({ sGUID: sGUID }),
                                            success: function (sParentGUID) {
                                                if (sParentGUID) {
                                                    fncUpdateXIParams(ComponentName, sParentGUID, null);
                                                }
                                                else {
                                                    fncUpdateXIParams(ComponentName, sGUID, null);
                                                }
                                            }
                                        });
                                    }
                                    if (!IsError) {
                                        if (sBOName == "Driver_T") {
                                            var DriverSNo = $('#' + formid).attr('data-sNo');
                                            if (DriverSNo != null && DriverSNo != 0) {
                                                fncSetParam(DriverSNo, sGUID, "{XIP|sDriverSNo_" + iID + "}", "");
                                            }
                                        }
                                        $('#' + formid).find(".StatusMessages").remove();
                                        $('#' + formid).prepend($('<div class="StatusMessages"><div class="alert alert-success">Data Saved Succesfully</div></div>'));
                                        if (ParentXIGUID && ParentXIGUID.length > 0 && Parent1ClickXIGUID && Parent1ClickXIGUID.length > 0) {
                                            $('.LayoutCode_' + ParentXIGUID).find('#Alert_' + Parent1ClickXIGUID).html('<div class="StatusMessages"><div class="alert alert-success">New data addded</div></div>');
                                        }
                                        $(function () {
                                            setTimeout(function () {
                                                $('.StatusMessages').hide('blind', {}, 100)
                                                $('.InLineTop-' + BOIDXIGUID).empty();
                                                $(document).find(".close-btn-toggle").hide();
                                                $(document).find(".add_btn").show();
                                            }, 5000);
                                        });
                                        $('#' + formid).attr("data-instanceid", iID);
                                        $('#' + formid).attr("id", "Create_" + sBOUID);
                                        $($this).parent().find('.DeleteBtn').show();
                                        $($this).parents('.PopupTabContentArea').attr("id", "Popup_" + sBoName + "_" + iID)
                                        //$('#' + formid).parents('div[class^="PopupTabContentArea"]').eq(0).attr("id", '#Popup_' + sBOName + "_" + iID);
                                        //$($($this).parent().find('.DeleteBtn')).parent().find('.PopupTabContentArea').attr("id", '#Popup_' + sBOName + "_" + iInstaceID);
                                        $($this).parent().find('.DeleteBtn').attr("onclick", "DeleteBO('" + iID + "', '" + sGUID + "', '" + sBoName + "', 'form component',this)");
                                        $('.LayoutCode_' + sGUID).find('#QSStep').find('form').each(function (a, b) {
                                            var IsMergeID = $(this).attr('data-id');
                                            var sMergeBOName = $(this).attr('data-sbo');
                                            if (IsMergeID == "yes" && sBOName == sMergeBOName) {
                                                var MergeFormID = $(this).attr('id');
                                                $('#' + MergeFormID).find('input[name=' + sPK.toLowerCase() + ']').val(iID);
                                                $('#' + MergeFormID).find('input[name=tr' + len + '_' + sPK.toLowerCase() + ']').val(iID);
                                                $('#' + MergeFormID).find('input[name=tr' + len + '_' + sPK.toLowerCase() + ']').css('display', 'block');
                                            }
                                            //Refresh Component for XReconcilliation Grid Hardcoded
                                            if (sBoName == "ACXReconcilliation_T") {
                                                fncGetDTable(OneClickURL, "28430213-C8A6-448F-92E1-64879E2ECC50", null, sGUID, "");
                                            }
                                            //var disableAddbtn = $("#Add_" + sFormGuid).is(":disabled");
                                            //if (disableAddbtn) {
                                            //    $("#Add_" + sFormGuid).removeAttr('disabled');
                                            //    $("#Add_" + data[len].BOD.BOID).removeAttr('disabled');
                                            //}
                                            var AddID = data[len].BOD.BOID;
                                            if (sReference && sReference.toLowerCase() == "xiguid") {
                                                AddID = data[len].BOD.XIGUID.toString();
                                                sBOUID = data[len].BOD.XIGUID.toString() + "_" + iID;
                                            }
                                            var disableAddbtn = $("#Add_" + AddID).is(":disabled");
                                            var sFormType = $("#Create_" + sBOUID).attr("data-Type");
                                            if (disableAddbtn && sFormType == "Create") {
                                                // $("#Add_" + sFormGuid).removeAttr('disabled');
                                                $("#Add_" + AddID).removeAttr('disabled');
                                                $("#Create_" + sBOUID).attr("data-Type", "Update");

                                            }
                                            var disableBtn = $(this).attr('data-disable');
                                            if (disableBtn == "yes") {
                                                var disableFormID = $(this).attr('id');
                                                $('#' + disableFormID).find('.btnQuote').removeAttr("disabled");
                                            }
                                        });
                                        if (sShowSaveSections) {
                                            var oShowSaveSections = sShowSaveSections.split('_');
                                            $.each(oShowSaveSections, function (i, sSectionCode) {
                                                if (sSectionCode) {
                                                    var childObj1 = $('[name="' + sSectionCode + '"]');
                                                    $(childObj1).removeClass('on').addClass('off');
                                                    var oQSInstance = QSInsDict[sGUID];
                                                    if (oQSInstance != undefined) {
                                                        for (var section in oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections) {
                                                            if (sSectionCode == oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sCode) {
                                                                oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "off";
                                                            }
                                                        }
                                                    }
                                                }

                                            });
                                        }
                                        //Refresh Component for XReconcilliation Grid Hardcoded
                                        //if (sBoName == "ACXReconcilliation_T") {
                                        // fncGetDTable(OneClickURL, 2284, null, sGUID, "");
                                        // }
                                        resolve(true);
                                    }
                                    else {
                                        $('#' + formid).find(".StatusMessages").hide();
                                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Something went wrong while saving</div>');
                                    }

                                    //$('#' + formid).find(".StatusMessages").hide();

                                    //fncUpdateInstanceID(data[len], sGUID).then(function (ConfigParams) {

                                    //});

                                    //Error = "Data saved successfully";
                                    //$('.Notifybar').html(Error);
                                    if (bIsListAdd) {
                                        var add = bIsListAdd.split('_');
                                        if (add.length == 2) {
                                            if (add[0] == "oneclick") {
                                                if (add[1] == "yes") {
                                                    fncRefreshBtnFromHTMLTree('', $this)
                                                }
                                            }
                                            else if (add[0] == "grid") {
                                                fncRefreshGrid($this, sGUID);
                                            }
                                        }
                                        else if (add.length == 3) {
                                            if (add[0] == "oneclick") {
                                                if (add[1] == "yes" && add[2] && add[2].length > 0) {
                                                    var RefreshBtn = $("#RefreshBtn-" + add[2]);
                                                    if (RefreshBtn && RefreshBtn.length > 0) {
                                                        $(RefreshBtn[0]).click();
                                                        var DlgIDForm = 'GridAddFormDlg-' + add[2];
                                                        var DlgForm = $('#' + DlgIDForm);
                                                        if (DlgForm && DlgForm.length > 0) {
                                                            $("#" + DlgIDForm).dialog('close');
                                                            $("#" + DlgIDForm).dialog('destroy').remove();
                                                        }
                                                        else {
                                                        }
                                                    }
                                                    else {
                                                        var RefreshBtn = $("#hiderefresh_" + add[2]);
                                                        if (RefreshBtn && RefreshBtn.length > 0) {
                                                            $(RefreshBtn[0]).click();
                                                            var DlgIDForm = 'GridAddFormDlg-' + add[2];
                                                            var DlgForm = $('#' + DlgIDForm);
                                                            if (DlgForm && DlgForm.length > 0) {
                                                                $("#" + DlgIDForm).dialog('close');
                                                                $("#" + DlgIDForm).dialog('destroy').remove();
                                                            }
                                                            else {
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                            else if (add[0] == "grid") {
                                                fncRefreshGrid($this, sGUID);
                                            }
                                        }
                                        else {
                                            var RefreshBtn = $('.hideref-' + iBODID);
                                            if (RefreshBtn && RefreshBtn.length > 0) {
                                                $(RefreshBtn[0]).click();
                                            }
                                        }
                                    }
                                    else {
                                        var RefreshBtn = $('.hideref-' + iBODID);
                                        if (RefreshBtn && RefreshBtn.length > 0) {
                                            $(RefreshBtn[0]).click();
                                        }
                                    }
                                }
                                //else {
                                //    $(this).parent('div.Notifybar').html(Error);
                                //    //$('.Notifybar').html(Error);
                                //}                        
                            }
                            else {
                                IsError = true;
                                var Scripts = data[len].oScriptErrors;
                                var sBoName = data[len].sBOName;
                                if (sReference && sReference.toLowerCase() == "xiguid") {
                                    sBoName = data[len].BOD.XIGUID.toString();
                                }

                                if (Scripts != null) {
                                    if (Object.keys(Scripts).length > 0) {
                                        for (var i = 0; i < Object.keys(Scripts).length; i++) {
                                            $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).closest('.highlight--help').addClass('msg-error');
                                            $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).html(Object.values(Scripts)[i]);
                                        }
                                    }
                                }

                                $('#' + formid).find(".StatusMessages").hide();
                                $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Something went wrong while saving</div>');
                            }
                        }
                    }
                    $($this).removeAttr('disabled');
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        if (SaveBOFormURL && SaveBOFormURL.length > 0) {
            SaveEditBOURL = SaveBOFormURL;
        }
    });
};

function fncClientQSLoad(iQSInstanceID, sStep, sType, iQuoteID, url) {
    //url = "http://localhost:53996";
    fncSetParam(iQSInstanceID, "ClientQSParams", "{XIP|iQSInstanceID}", "").then(function (state, callback) {
        fncSetParam(sStep, "ClientQSParams", "sCurrentStepName", "").then(function (state, callback) {
            fncSetParam(sType, "ClientQSParams", "BuyType", "").then(function (state, callback) {
                fncGetQSDefByInstanceID(iQSInstanceID).then(function (state, callback) {
                    if (state) {
                        if (url && state) {
                            url = url + "/" + state.sURL;
                        }
                        fncSetParam(state.iQSDefinitionID, "ClientQSParams", "iQSDID", "").then(function (state, callback) {
                            fncSetParam(iQuoteID, "ClientQSParams", "{XIP|iInstanceID}", "").then(function (state, callback) {
                                window.open(url, '_blank');
                            });
                        });
                    }
                });
            });
        });
    });
}

function DeleteBO(iInstaceID, sGUID, sBOName, sComponentName, $this) {
    var oQSInstance = QSInsDict[sGUID];
    var pvalue = {
        iInstanceID: iInstaceID,
        sGUID: sGUID,
        sBOName: sBOName
    }
    var RemoveData = $($this).attr('data-Remove');
    $.ajax({
        url: DeleteBOURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        cache: false,
        data: JSON.stringify(pvalue),
        success: function (data) {
            if (sComponentName == "html component") {
                if (RemoveData != undefined && RemoveData != "") {
                    $($this).closest("#" + RemoveData).remove();
                }
                else {
                    $($this).closest('.additionaldrive').remove();
                    $('.resaddbtn').show();
                    if (sBOName == "Driver_T") {
                        var DriverSNo = $($this).attr('data-sNo');
                        if (DriverSNo != null && DriverSNo != 0) {
                            fncSetParam("", sGUID, "{XIP|sDriverSNo_" + iInstaceID + "}", "");
                            fncSetParam(DriverSNo, sGUID, "{XIP|sDeletedDriverSNo_" + iInstaceID + "}", "");
                            fncRemoveStepXIIValues("Additional Driver_" + DriverSNo, sGUID, oQSInstance.XIGUID)
                            var sScript = "xi.s|{xi.count|'Driver_T',{xi.p|-instanceid},'FKiQSInstanceID'}";
                            fncGetDriverCount(sScript, sGUID, "{XIP|iCount}");
                        }
                    }
                }
                //var DivID = $("#Delete_" + sBOName + "_" + iInstaceID).closest("div").attr("id");
                //$('#' + DivID).remove();
            }
            if (sComponentName == "form component") {
                //var FormID = $("#Delete_" + sBOName + "_" + iInstaceID).closest("form").attr("id");
                $('#Popup_' + sBOName + "_" + iInstaceID).remove();
            }
        },
        error: function (data) {
        }
    });
};
function fncRemoveStepXIIValues(sStepName, sGUID, iQSIID) {
    $.ajax({
        url: RemoveStepXIIValuesURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        cache: false,
        data: JSON.stringify({ sStepName: sStepName, sGUID: sGUID, iQSIID: iQSIID }),
        success: function (data) {
        },
        error: function (data) {
        }
    });
};

function ShowContentInDialogOrPopup(model, sGUID, oNVParams) {
    var Details = model.LayoutDetails;
    $('.LayoutCode_' + sGUID + ' div').each(function (j, obj) {
        var id = $(this).attr('id');
        if (id && id.length > 0) {
            var PlaceholderArea = "div" + id;
            for (var k = 0; k < Details.length; k++) {
                if (Details[k].PlaceholderArea == PlaceholderArea) {
                    $(this).attr('data-name', Details[k].PlaceholderName);
                    //$(this).attr('id', Details[k].PlaceholderUniqueName + "-" + sGUID);
                    $(this).attr('id', Details[k].PlaceholderUniqueName);
                    //$(this).attr('data-placeid', Details[k].PlaceholderUniqueName);
                    $(this).addClass('one-layout-detail');
                    $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                    $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                    $(this).attr('data-def', Details[k].XIGUID);
                    Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                }
            }
        }
    });
    fncgetlayoutcontent(model, sGUID, oNVParams);
}

function fncgetlayoutcontent(Layout, sGUID, oNVParams) {
    for (var i = 0; i < Layout.LayoutMappings.length; i++) {
        if (Layout.LayoutMappings[i].ContentType == "HTML") {
            var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
            for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                    //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(Layout.Mappings[i].HTMLCode);//Removed sGUID
                    $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(Layout.LayoutMappings[i].HTMLCode);
                }
            }
        }
        else if (Layout.LayoutMappings[i].ContentType == "XiLink") {
            if (Layout.LayoutMappings[i].XiLinkID > 0) {
                $.ajax({
                    url: ContentURL,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    datatype: "HTML",
                    cache: false,
                    async: false,
                    data: JSON.stringify({ XiLinkID: Layout.LayoutMappings[i].XiLinkIDXIGUID, sGUID: sGUID, BODID: iBODID, ID: 0, oNVParams: oNVParams }),
                    success: function (data) {
                        var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderIDXIGUID;
                        for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                            if (Layout.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                                $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append('<div class="one-link one-link-id-' + Layout.LayoutMappings[i].XiLinkIDXIGUID + '" data-def="' + Layout.LayoutMappings[i].XiLinkIDXIGUID + '">' + data + '</div>');
                            }
                        }
                    }
                });
            }
        }
        else if (Layout.LayoutMappings[i].ContentType == "XIComponent") {
            if (Layout.LayoutMappings[i].XiLinkID > 0) {
                var ComponentID = Layout.LayoutMappings[i].XiLinkID;
                var MappingID = Layout.LayoutMappings[i].MappingID;
                var IsValueSet = Layout.LayoutMappings[i].IsValueSet;
                var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
                $.ajax({
                    url: ComponentURL,
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    datatype: "HTML",
                    cache: false,
                    async: false,
                    data: JSON.stringify({ iXIComponentID: ComponentID, sGUID: sGUID, sType: 'Layout', ID: PlaceHolderID, nParams: oNVParams }),
                    success: function (data) {
                        for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                            if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                                //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                                $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(data);
                            }
                        }
                    }
                });
            }
        }
        else if (Layout.LayoutMappings[i].ContentType == "Step") {
            var LoadGUID;
            if (sOnLoadGUID && sOnLoadGUID.length > 0) {
                LoadGUID = sOnLoadGUID;
            }
            else {
                LoadGUID = sGUID;
            }
            var StepID = Layout.LayoutMappings[i].XiLinkID;
            var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
            var oParams = [];
            var Param = {};
            Param["sName"] = "{XIP|iStepDID}";
            Param["sValue"] = StepID;
            oParams.push(Param);
            $.ajax({
                url: ComponentURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "HTML",
                cache: false,
                async: false,
                data: JSON.stringify({ iXIComponentID: 0, sGUID: LoadGUID, sName: 'Step Component', nParams: oParams }),
                success: function (StepContent) {
                    //var PlaceHolderID = Layout.Mappings[i].PlaceHolderID;
                    for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                        if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                            //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).html(StepContent);//Removed sGUID
                            //$('#' + Layout.Details[j].PlaceholderUniqueName).html(StepContent);
                            //$('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).html('<div id="Component-' + StepID + '" data-type="Component" data-name="Component-' + StepID + '">' + StepContent + '</div>');
                            $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).html(StepContent);
                        }
                    }
                    //$('#Step_' + StepID).html(StepContent);
                    //if (StepContent.iLayoutID > 0) {
                    //    fncgetlayoutcontent(StepContent.Layout, sGUID)
                    //}
                    //else {
                    //    //fncGetStepData(Layout, StepContent, PlaceHolderID, sGUID, "Step_"+StepContent.ID);
                    //}
                }
            });
        }
    }
}

function fncGetStepData(Layout, StepDef, PlaceHolderID, sGUID, DivID) {
    $.ajax({
        url: StepDataURL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "HTML",
        cache: false,
        async: false,
        data: JSON.stringify({ oStepDef: StepDef, sGUID: sGUID }),
        success: function (StepData) {
            if (DivID.length > 0) {
                $('#' + DivID).append(StepData);
            }
            else {
                //var PlaceHolderID = Layout.Mappings[i].PlaceHolderID;
                for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                    if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                        $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName + "-" + sGUID).append(StepData);
                    }
                }
            }
        }
    });
}

function CreateGuid() {
    function _p8(s) {
        var p = (Math.random().toString(16) + "000000000").substr(2, 8);
        return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
    }
    return _p8() + _p8(true) + _p8(true) + _p8();
}

$(document).on('click', 'input.XiLinkBtn', function () {
    var XiLinkID = $(this).attr('data-xilinkid');
    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', this);
    if (XiLinkID > 0) {
        XIRun(null, XiLinkID, 0, null, 0, true);
    }
});

var XiLinkResponse;

function fncGetContent(XIStepID) {
    $.ajax({
        url: StepURL,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "HTML",
        cache: false,
        async: false,
        data: { iXiLinkID: XIStepID },
        success: function (data) {
            XiLinkResponse = data;
        }
    });
}


function fncGetFormTypeFromHTMLTree(parentName, childObj) {
    var ActiveInstanceID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('class') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            ActiveInstanceID = testObj.getAttribute('data-formlocation');
            return ActiveInstanceID
        }
    }
    ActiveInstanceID = testObj.getAttribute('data-formlocation');
    return ActiveInstanceID;
}

function fncRefreshBtnFromHTMLTree(parentName, childObj) {
    parentName = 'oneclickresponse';
    var ActiveInstanceID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.classList && testObj.classList.length > 0 && testObj.classList[0] != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            ActiveInstanceID = testObj.getAttribute('data-formlocation');
            return ActiveInstanceID
        }
    }
    ActiveInstanceID = $(testObj).find('.header-form-btn').find('#hiderefresh');
    $(ActiveInstanceID).click();
    return ActiveInstanceID;
}

function fncGrid1ClickIDFromHTMLTree(parentName, childObj) {
    parentName = 'datagrid';
    var gridid = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-type') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            gridid = testObj.getAttribute('id');
            return gridid
        }
    }
    gridid = testObj.getAttribute('id');
    return gridid;
}


function fncRefreshGrid($this, sGUID) {
    var gridid = fncGrid1ClickIDFromHTMLTree('', $this);
    if (gridid && gridid.length > 0) {
        var id = gridid.split('-');
        if (id.length == 2) {
            var gID = id[1];
            $.ajax({
                url: GetGridDataURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "html",
                cache: false,
                async: false,
                data: JSON.stringify({ i1ClickID: gID, sGUID: sGUID }),
                success: function (data) {
                    $('#GridComponent').replaceWith(data);
                },
                error: function (err) {
                }
            });
        }
    }
}


function fncGetSemanticIDFromHTMLTree(parentName, childObj) {
    var ActiveInstanceID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('class') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            ActiveInstanceID = testObj.getAttribute('data-semanticid');
            return ActiveInstanceID
        }
    }
    ActiveInstanceID = testObj.getAttribute('data-semanticid');
    return ActiveInstanceID;
}

function fncGetStepIDFromHTMLTree(parentName, childObj) {
    var ActiveInstanceID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('class') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return ActiveInstanceID
        }
    }
    ActiveInstanceID = testObj.getAttribute('data-stepid');
    return ActiveInstanceID;
}


function fncGetNextSemanticStep(StepID, Steps) {
    for (var i = 0; i < Steps.length; i++) {
        if (StepID == Steps[i].XIStepID) {
            if (i == 0) {
                if (Steps[i].XIStepID > 0) {
                    fncGetContent(Steps[i].XIStepID);
                    $('.XIStepContent').html(XiLinkResponse);
                }
                if (Steps[i + 1]) {
                    var NxtStepID = Steps[i + 1].XIStepID;
                    $('.XISemanticStepDiv').attr('data-stepid', NxtStepID);
                }
            }
            else if (i < Steps.length) {
                $('.XIStepContent').html('Step Order is ' + Steps[i].iOrder);
                if (i < Steps.length - 1) {
                    var NxtStepID = Steps[i + 1].XIStepID;
                    $('.XISemanticStepDiv').attr('data-stepid', NxtStepID);
                }
            }
        }
    }
    $('#XISemanticDiv').show();
}

function fncGetPreviousSemanticStep(StepID, Steps) {
    for (var i = 0; i < Steps.length; i++) {
        if (StepID == Steps[i].XIStepID) {
            if (i == 0) {
                //$('.XIStepContent').html('Step Order is ' + Steps[i].iOrder);
                //var NxtStepID = Steps[i + 1].XIStepID
                //$('.XISemanticStepDiv').attr('data-stepid', NxtStepID);
            }
            else if (i <= Steps.length - 1) {
                $('.XIStepContent').html('Step Order is ' + Steps[i - 1].iOrder);
                var NxtStepID = Steps[i - 1].XIStepID;
                $('.XISemanticStepDiv').attr('data-stepid', NxtStepID);
            }
        }
    }
    $('#XISemanticDiv').show();
}

var bchangeTimer = false;
var acount = 0;
var IsDialogOpen = false;
function fncgetautocomplete(i1ClickID, $this, event) {
    var keyCode = event.keyCode || event.which;
    if (keyCode == 9) {
        return;
    }
    var sType = $($this).attr('data-type');
    if (sType && sType.length > 0) {
        if (sType == 'smartsearch') {
            i1ClickID = $("#SmartSearchDDL :selected").val();
        }
    }
    var offest = $($this).offset();
    var height = $($this).height();
    var SearchText = $($this).val();
    var FieldPlace = $($this).attr('data-identity');
    if (SearchText.length > 0) {
        if (i1ClickID && i1ClickID.length > 0) {
            if (bchangeTimer !== false) clearTimeout(bchangeTimer);
            bchangeTimer = setTimeout(function () {
                $.ajax({
                    type: 'POST',
                    url: AutoCompleteLarge,
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({ i1ClickID: i1ClickID, sAutoText: SearchText, sField: FieldPlace }),
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (data) {
                        var DialogDivID = 'smartsearch';
                        var bIsExist = $('.' + DialogDivID);
                        if (bIsExist.length > 0) {
                            $("." + DialogDivID).html(data);
                        }
                        else {
                            $('.smartsearch-dialog').html('<div class="' + DialogDivID + '">' + data + '</div>');
                            var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
                            if (acount == 0) {
                                $("." + DialogDivID).dialog({
                                    width: 550,
                                    dialogClass: "no-titlebar",
                                    open: function (event, ui) {
                                        $(this).parent().promise().done(function () {
                                            $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + windowclose + '</div>');
                                        })
                                        $(event.target).parent().css('position', 'absolute');
                                        $(event.target).parent().css('top', offest.top + height + 15 + 'px');
                                        $(event.target).parent().css('left', offest.left + 'px');
                                    }
                                    //close: function (event, ui) {
                                    //    acount = 0;
                                    //    $('#dialog_autocomplete').dialog('close');
                                    //    //$('#my_dialog').parent().hide();
                                    //    //$(this).dialog('destroy').remove();
                                    //}
                                });
                                IsDialogOpen = true;
                            }
                            $('#' + FieldPlace).focus();
                            acount++;
                        }
                    }
                });
                bchangeTimer = false;
            }, 300);
        }
    }
    else {
        acount = 0;
        if (IsDialogOpen) {
            $('#my_dialog').dialog('close');
        }
    }
}

function fncgetlabeldata(iBOID, iInstanceID, Label, sFieldID, i1Click) {
    if (parseInt(iInstanceID) > 0 && Label != null && (parseInt(iBOID) > 0 || parseInt(i1Click) > 0)) {
        $.ajax({
            url: LabelDataURL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            cache: false,
            async: false,
            data: JSON.stringify({ iBOID: parseInt(iBOID), Label: Label, i1ClickID: parseInt(i1Click) }),
            success: function (LabelData) {
                $('#' + sFieldID).val(LabelData);
                $('#' + sFieldID).attr('data-value', iInstanceID);
                if (i1Click == 0) {
                    $('#dialog_autocomplete').dialog('close');
                }
            }
        });
    }
}

//Auto Create Layout
function fncautocreatelayout(sBO, i1ClickID) {
    if (sBO == "" || i1ClickID == 0) {
        sBO = $("#BusinessObject :selected").text();
        i1ClickID = $('#QueryID').val();
    }
    var sType = ($('#sType option:selected').text());
    $.ajax({
        url: AutoCreateLayoutURL,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: { sBO: sBO, i1ClickID: i1ClickID },
        cache: false,
        async: false,
        success: function (LayoutData) {
            $('#LayoutID').append('<option value="' + LayoutData.ID + '">' + LayoutData.PropertyName + '</option>');
            $('#LayoutID').val(LayoutData.ID);
            iCreateXiLinkID = parseInt(LayoutData.sID);
            $.ajax({
                url: LayoutMappingsURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                data: JSON.stringify({ iLayoutID: LayoutData.ID }),
                cache: false,
                async: false,
                success: function (Mappings) {
                    if (Mappings && Mappings.length > 0) {
                        $('#MapID').html('');
                        for (var i = 0; i < Mappings.length; i++) {
                            $('#MapID').append('<option value="' + Mappings[i].Value + '">' + Mappings[i].text + '</option>');
                        }
                    }
                }
            });
        }
    });
}

function fncShowLayout(ID) {
    if (editpopups.length > 0) {
        for (var i = 0; i < editpopups.length; i++) {
            editpopups[i].close();
        }
    }
    var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
    $.ajax({
        type: 'POST',
        url: AddEditLayoutURL,
        data: { ID: ID },
        cache: false,
        async: false,
        dataType: 'html',
        success: function (data) {
            with (editpopup.document) {
                open();
                write(data);
                close();
            }
            editpopups.push(editpopup);
            editpopup.moveTo(0, 0);
        }
    });
}

//XiLink Manager

function XiLinkManager(Type, Action) {
    var XiLinkID = 0;
    if (Action == "Refresh") {
        $.ajax({
            type: 'GET',
            url: XiLinkListByOrgURL,
            //data: { OrgID: OrgID },
            cache: false,
            async: false,
            dataType: 'json',
            success: function (data) {
                if (Type == "Row") {
                    $('#RowXiLinkID').empty();
                    $('#RowXiLinkID').find('option').remove();
                }
                else if (Type == "Column") {
                    $('#ColumnXiLinkID').empty();
                    $('#ColumnXiLinkID').find('option').remove();
                }
                else if (Type == "Cell") {
                    $('#CellXiLinkID').empty();
                    $('#CellXiLinkID').find('option').remove();
                }
                else if (Type == "Layout") {
                    $('#XiLinkDDL').empty();
                    $('#XiLinkDDL').find('option').remove();
                }
                else if (Type == "Menu") {
                    $('#XiLinkID').empty();
                    $('#XiLinkID').find('option').remove();
                }
                optionArray = [];
                optionArray[0] = "<option value='0'>--Select--</option>";
                $.each(data.XiLinksList, function (i, option) {
                    optionArray[i + 1] = "<option value='" + option.Value + "'>" + option.text + "</option>";
                });
                if (Type == "Row") {
                    $('#RowXiLinkID').append(optionArray.join(''));
                }
                if (Type == "Column") {
                    $('#ColumnXiLinkID').append(optionArray.join(''));
                }
                if (Type == "Cell") {
                    $('#CellXiLinkID').append(optionArray.join(''));
                }
                if (Type == "Layout") {
                    $('#XiLinkDDL').append(optionArray.join(''));
                }
                if (Type == "Menu") {
                    $('#XiLinkID').append(optionArray.join(''));
                }
            }
        });
    }
    else {
        if (Action == "Edit" || Action == "View") {
            if (Type == "Row") {
                XiLinkID = $('#RowXiLinkID').val();
            }
            else if (Type == "Column") {
                XiLinkID = $('#ColumnXiLinkID').val();
            }
            else if (Type == "Cell") {
                XiLinkID = $('#CellXiLinkID').val();
            }
            else if (Type == "Layout") {
                XiLinkID = $('#XiLinkDDL').val();
            }
            else if (Type == "Menu") {
                XiLinkID = $('#XiLinkID').val();
            }
            if (Action == "Edit") {
                if (editpopups.length > 0) {
                    for (var i = 0; i < editpopups.length; i++) {
                        editpopups[i].close();
                    }
                }
                var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                $.ajax({
                    type: 'POST',
                    url: AddEditXiLinkURL,
                    data: { XiLinkID: XiLinkID },
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (data) {
                        with (editpopup.document) {
                            open();
                            write(data);
                            close();
                        }
                        editpopups.push(editpopup);
                        editpopup.moveTo(0, 0);
                    }
                });
            }
            else {
                if (editpopups.length > 0) {
                    for (var i = 0; i < editpopups.length; i++) {
                        editpopups[i].close();
                    }
                }
                var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                $.ajax({
                    type: 'POST',
                    url: ViewXiLinkURL,
                    data: { XiLinkID: XiLinkID },
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (data) {
                        with (editpopup.document) {
                            open();
                            write(data);
                            close();
                        }
                        editpopups.push(editpopup);
                        editpopup.moveTo(0, 0);
                    }
                });
            }
        }
        else {
            var url = AddEditXiLinkURL;
            if (Poppopups.length > 0) {
                for (var i = 0; i < Poppopups.length; i++) {
                    Poppopups[i].close();
                }
            }
            var newpopup = window.open(url, '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
            Poppopups.push(newpopup);
            newpopup.moveTo(0, 0);
        }
    }
}

//XiParameter Manager

function XiParameterManager(Type, Action) {
    var XiParameterID = 0;
    if (Action == "Refresh") {
        $.ajax({
            type: 'GET',
            url: XiParameterListByOrgURL,
            cache: false,
            async: false,
            dataType: 'json',
            success: function (data) {
                if (Type == "XiParameterLayout") {
                    $('#XiParameterDDL').empty();
                    $('#XiParameterDDL').find('option').remove();
                }
                optionArray = [];
                optionArray[0] = "<option value='0'>--Select--</option>";
                $.each(data.XiLinksList, function (i, option) {
                    optionArray[i + 1] = "<option value='" + option.Value + "'>" + option.text + "</option>";
                });
                if (Type == "XiParameterLayout") {
                    $('#XiParameterDDL').append(optionArray.join(''));
                }
            }
        });
    }
    else {
        if (Action == "Edit" || Action == "View") {
            if (Type == "XiParameterLayout") {
                XiParameterID = $('#XiParameterDDL').val();
            }
            if (Action == "Edit") {
                if (editpopups.length > 0) {
                    for (var i = 0; i < editpopups.length; i++) {
                        editpopups[i].close();
                    }
                }
                var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                $.ajax({
                    type: 'POST',
                    url: AddEditXiParameterURL,
                    data: { XiParameterID: XiParameterID },
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (data) {
                        with (editpopup.document) {
                            open();
                            write(data);
                            close();
                        }
                        editpopups.push(editpopup);
                        editpopup.moveTo(0, 0);
                    }
                });
            }
            else {
                if (editpopups.length > 0) {
                    for (var i = 0; i < editpopups.length; i++) {
                        editpopups[i].close();
                    }
                }
                var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                $.ajax({
                    type: 'POST',
                    url: ViewXiParameterURL,
                    data: { XiParameterID: XiParameterID },
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (data) {
                        with (editpopup.document) {
                            open();
                            write(data);
                            close();
                        }
                        editpopups.push(editpopup);
                        editpopup.moveTo(0, 0);
                    }
                });
            }
        }
        else {
            var url = AddEditXiParameterURL;
            if (Poppopups.length > 0) {
                for (var i = 0; i < Poppopups.length; i++) {
                    Poppopups[i].close();
                }
            }
            var newpopup = window.open(url, '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
            Poppopups.push(newpopup);
            newpopup.moveTo(0, 0);
        }
    }
}

//Popup and Dialog Layout Manager

function XiLayoutManager(Type, Action) {
    var ID = 0;
    if (Action == "Refresh") {
        $.ajax({
            type: 'GET',
            url: XiLayoutListByOrgURL,
            data: { Type: Type },
            cache: false,
            async: false,
            dataType: 'json',
            success: function (data) {
                if (Type == "Dialog" || "Popup") {
                    $('#XiLayoutsDDL').empty();
                    $('#XiLayoutsDDL').find('option').remove();
                }
                optionArray = [];
                optionArray[0] = "<option value='0'>--Select--</option>";
                $.each(data.Layouts, function (i, option) {
                    optionArray[i + 1] = "<option value='" + option.Value + "'>" + option.text + "</option>";
                });
                if (Type == "Dialog" || "Popup") {
                    $('#XiLayoutsDDL').append(optionArray.join(''));
                }
            }
        });
    }
    else {
        if (Action == "Edit" || Action == "View") {
            if (Type == "XiLayout") {
                ID = $('#XiLayoutsDDL').val();
                if (ID > 0) {
                    if (Action == "Edit") {
                        if (editpopups.length > 0) {
                            for (var i = 0; i < editpopups.length; i++) {
                                editpopups[i].close();
                            }
                        }
                        var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                        $.ajax({
                            type: 'POST',
                            url: AddEditXiLayoutURL,
                            data: { LayoutID: ID },
                            cache: false,
                            async: false,
                            dataType: 'html',
                            success: function (data) {
                                with (editpopup.document) {
                                    open();
                                    write(data);
                                    close();
                                }
                                editpopups.push(editpopup);
                                editpopup.moveTo(0, 0);
                            }
                        });
                    }
                    else {
                        if (editpopups.length > 0) {
                            for (var i = 0; i < editpopups.length; i++) {
                                editpopups[i].close();
                            }
                        }
                        var editpopup = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                        $.ajax({
                            type: 'POST',
                            url: AddEditXiLayoutURL,
                            data: { LayoutID: ID },
                            cache: false,
                            async: false,
                            dataType: 'html',
                            success: function (data) {
                                with (editpopup.document) {
                                    open();
                                    write(data);
                                    close();
                                }
                                editpopups.push(editpopup);
                                editpopup.moveTo(0, 0);
                            }
                        });
                    }
                }
            }
        }
        else {
            ID = $('#XiLayoutsDDL').val();
            if (ID > 0) {
                var url = AddEditXiLayoutURL + "?LayoutID=" + ID;
                if (Poppopups.length > 0) {
                    for (var i = 0; i < Poppopups.length; i++) {
                        Poppopups[i].close();
                    }
                }
                var newpopup = window.open(url, '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
                Poppopups.push(newpopup);
                newpopup.moveTo(0, 0);
            }

        }
    }
}

function fncgetlayoutdetails(iLayoutID) {
    $.ajax({
        url: LayoutDetailsURL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        cache: false,
        async: false,
        data: JSON.stringify({ iLayoutID: parseInt(iLayoutID) }),
        success: function (LabelData) {

        }
    });
}


function RunDialog(DialogID) {
    if (DialogID.length > 0) {
        $.ajax({
            type: 'POST',
            url: DialogURL,
            data: { ID: DialogID },
            cache: false,
            async: false,
            dataType: 'json',
            success: function (Dialogdata) {
                var DialogGUID = CreateGuid();
                $.ajax({
                    type: 'POST',
                    url: PopupORDialogURL,
                    data: { XiLinkID: 0, BOID: 0, sGUID: null, ID: 0, sNewGuid: DialogGUID, PRDID: DialogID },
                    cache: false,
                    async: false,
                    dataType: 'html',
                    success: function (result) {
                        $.unblockUI();
                        if (Dialogdata.PopupSize == "Default") {
                            Dialogdata.DialogHeight = 700;
                            Dialogdata.DialogWidth = 800;
                        }
                        if (Dialogdata.IsGrouping == false) {
                            var DialogDivID = "ResultDialog-" + DialogGUID;
                            $('.' + DialogDivID).dialog('destroy').remove();
                            var Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"></div>';
                            $('#Dialogs').append(Div);
                            $("." + DialogDivID).html(result);
                            $("." + DialogDivID).dialog({
                                title: ' ',
                                height: Dialogdata.DialogHeight,
                                width: Dialogdata.DialogWidth,
                                resizable: Dialogdata.IsResizable,
                                IsCloseIcon: Dialogdata.IsCloseIcon,
                                BarPosition: Dialogdata.BarPosition,
                                buttons: [
                                ],
                                close: function (event, ui) {
                                    $('.' + DialogDivID).parent().hide();
                                    $(this).dialog('destroy').remove();
                                }
                            });
                        }
                        else {
                            var DialogDivID = "ResultDialog-" + DialogID + "-" + ID;
                            //$('.' + DialogDivID).dialog('destroy').remove();
                            var Div = "";
                            var AnyDivs = $('#DialogContent').find('div');
                            if (AnyDivs.length > 0) {
                                Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message" data-dname="' + DialogDivID + '" style="display:none;"></div>';
                            }
                            else {
                                Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message" data-dname="' + DialogDivID + '"></div>';
                            }
                            var IsDivExists = fncIsDivAlreadyExists('DialogContent', DialogDivID);
                            if (!IsDivExists) {
                                $('#DialogContent').append(Div);
                                $("." + DialogDivID).html(result);
                            }
                            BarPosition = Dialogdata.BarPosition;
                            $('#VerticalLeftBar').hide();
                            $('#VerticalRightBar').hide();
                            $('#HorizontalTopBar').hide();
                            $('#HorizontalBottonBar').hide();
                            var BtnID = "DialogBarBtn-" + DialogID + "-" + ID;
                            var IsExists = "No";
                            //var VBar = '<button type="button" class="btn btn-primary btn-xs dialogBtn" id="' + BtnID + '" >Dialog' + ID + '</button>';
                            var VBar = '<button type="button" class="btn btn-dark btn-xs dialogBtn" id="' + BtnID + '" ><i class="fa fa-user-plus" ></i><span class="" >' + ID + '</span></button>';
                            //var VBar = '<div class="po-Left"><button type="button" class="btn btn-primary btn-xs dialogBtn po-LeftLink" id="' + BtnID + '" ><i class="fa fa-home"></i></button><div class="po-LeftContent hidden"><div class="po-LeftBody">Dialog' + ID + '</div></div></div>';
                            var Placement;
                            //if (BarPosition == "Vertical Left") {
                            //    Placement = 'right';
                            //}
                            //else if (BarPosition == "Vertical Right") {
                            //    Placement = 'left';
                            //}
                            //else if (BarPosition == "Horizontal Top") {
                            //    Placement = 'bottom';
                            //}
                            //else if (BarPosition == "Horizontal Bottom") {
                            //    Placement = 'top';
                            //}

                            //$('.po-Left > .po-LeftLink').popover({
                            //    trigger: 'hover',
                            //    html: true,
                            //    content: function () {
                            //        return $(this).parent().find('.po-LeftBody').html();
                            //    },
                            //    container: 'body',
                            //    placement: Placement
                            //});
                            if (Dialogdata.BarPosition == "Vertical Left") {
                                $('#VerticalLeftBar button').each(function (j, obj) {
                                    var id = $(this).attr('id');
                                    if (id == BtnID) {
                                        IsExists = "Yes";
                                    }
                                });
                                if (IsExists == "No") {
                                    $('#VerticalLeftBar').append(VBar);
                                }
                                $('#VerticalLeftBar').show();
                                $('#DialogContent').addClass('col-xs-11');
                            }
                            else if (Dialogdata.BarPosition == "Vertical Right") {
                                $('#VerticalRightBar button').each(function (j, obj) {
                                    var id = $(this).attr('id');
                                    if (id == BtnID) {
                                        IsExists = "Yes";
                                    }
                                });
                                if (IsExists == "No") {
                                    $('#VerticalRightBar').append(VBar);
                                }
                                $('#VerticalRightBar').show();
                                $('#DialogContent').addClass('col-xs-11');
                            }
                            else if (Dialogdata.BarPosition == "Horizontal Top") {
                                $('#HorizontalTopBar button').each(function (j, obj) {
                                    var id = $(this).attr('id');
                                    if (id == BtnID) {
                                        IsExists = "Yes";
                                    }
                                });
                                if (IsExists == "No") {
                                    $('#HorizontalTopBar').append(VBar);
                                }
                                $('#HorizontalTopBar').show();
                                $('#DialogContent').addClass('col-xs-12');
                            }
                            else if (Dialogdata.BarPosition == "Horizontal Bottom") {
                                $('#HorizontalBottonBar button').each(function (j, obj) {
                                    var id = $(this).attr('id');
                                    if (id == BtnID) {
                                        IsExists = "Yes";
                                    }
                                });
                                if (IsExists == "No") {
                                    $('#HorizontalBottonBar').append(VBar);
                                }
                                $('#HorizontalBottonBar').show();
                                $('#DialogContent').addClass('col-xs-12');
                            }

                            $("#DialogGroups").dialog({
                                title: ' ',
                                height: Dialogdata.DialogHeight,
                                width: Dialogdata.DialogWidth,
                                resizable: Dialogdata.IsResizable,
                                IsCloseIcon: Dialogdata.IsCloseIcon,
                                BarPosition: Dialogdata.BarPosition,
                                position: { my: my, at: at },
                                buttons: [
                                ],
                                close: function (event, ui) {
                                    $('#DialogGroups').parent().hide();
                                    $(this).dialog('destroy').remove();
                                }
                            });
                        }

                    }
                });
            }
        });
    }
}

function fncGetDialogOpacity(iTransparent) {
    var TransValue = iTransparent / 100;
    var sClass = Transparent(TransValue);
    return sClass;
}

function Transparent(Transvalue) {
    var DiaOpacity = "";
    if (Transvalue == 0) {
        DiaOpacity = 'opacity-change0';
    }
    else if (Transvalue == 0.1) {
        DiaOpacity = 'opacity-change1';
    }
    else if (Transvalue == 0.2) {
        DiaOpacity = 'opacity-change2';
    }
    else if (Transvalue == 0.3) {
        DiaOpacity = 'opacity-change3';
    }
    else if (Transvalue == 0.4) {
        DiaOpacity = 'opacity-change4';
    }
    else if (Transvalue == 0.5) {
        DiaOpacity = 'opacity-change5';
    }
    else if (Transvalue == 0.6) {
        DiaOpacity = 'opacity-change6';
    }
    else if (Transvalue == 0.7) {
        DiaOpacity = 'opacity-change7';
    }
    else if (Transvalue == 0.8) {
        DiaOpacity = 'opacity-change8';
    }
    else if (Transvalue == 0.9) {
        DiaOpacity = 'opacity-change9';
    }
    else if (Transvalue == 1) {
        DiaOpacity = 'opacity-change10';
    }
    return DiaOpacity;
}

function dialogtitle() {
    '<div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix ui-draggable-handle"><span id="ui-id-2" class="ui-dialog-title"> </span><button type="button" class="ui-dialog-titlebar-close"></button></div>';
}
// Load Component
function fncLoadComponent(iComponentID, sName, sGUID, InputParams, XILinkID, BODID) {
    $.ajax({
        type: 'POST',
        url: ComponentURL,
        data: JSON.stringify({ iXIComponentID: iComponentID, sGUID: sGUID, nParams: InputParams, sName: sName, sType: "XiLink", ID: XILinkID, iInstanceID: 0, sContext: null, iQSIID: 0, BODID: BODID }),
        contentType: 'application/json;',
        dataType: 'html',
        traditional: true,
        success: function (data) {
            $('#Component_' + XILinkID).html(data);
        }
    });
}

//function fncUpdateXIParams(sName, sGUID, CurrentGUID, InputParams, OutputDiv) {
//    return new Promise(function (resolve, reject) {
//        $.ajax({
//            type: 'POST',
//            url: UpdateXIParamsURL,
//            data: JSON.stringify({ sGUID: sGUID, sCurrentGUID: CurrentGUID, nParams: InputParams }),
//            contentType: 'application/json;',
//            dataType: 'json',
//            traditional: true,
//            success: function (data) {
//                var DivID;
//                if (data.Registers && data.Registers.length > 0) {
//                    for (var i = 0; i < data.Registers.length; i++) {
//                        var StepID = sName.split('-')[1];
//                        var Prms = data.Registers[i].sValue.split('_');
//                        var ComponentName = Prms[0];
//                        var ID = Prms[1];
//                        if (StepID != ID) {
//                            
//                            var j = 0;
//                            if (j == 0) {
//                                DivID = ID;
//                                j++;
//                            }
//                            TriggerComponent(sGUID, ComponentName, ID, OutputDiv);

//                        }
//                        // resolve(ID);
//                        //if (data.Registers[i].sValue != sName && ComponentName != "XITreeStructure") {
//                        //    TriggerComponent(sGUID, ComponentName, ID);
//                        //}
//                        if (i == data.Registers.length) {
//                            resolve(ID);
//                        }
//                    }

//                }
//            }
//        });
//    });
//}


function fncUpdateXIParams(sName, sGUID, CurrentGUID, InputParams) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: UpdateXIParamsURL,
            data: JSON.stringify({ sGUID: sGUID, sCurrentGUID: CurrentGUID, nParams: InputParams }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                resolve(data);
            }
        });
    });
}




//function TriggerComponent(sGUID, sName, ID, OutputDiv) {
//    $.ajax({
//        type: 'POST',
//        url: ComponentURL,
//        data: JSON.stringify({ iXIComponentID: 0, sGUID: sGUID, sName: sName, sType: "QSStepSection", ID: ID }),
//        contentType: 'application/json;',
//        dataType: 'html',
//        traditional: true,
//        success: function (data) {
//            
//            if (OutputDiv != 'undefined' && OutputDiv != null) {
//                $('#' + OutputDiv).html(data);
//            }
//            else {
//                $('#Component-' + ID).html(data);
//            }
//        },
//        error: function (data) {
//        }
//    });
//}

function TriggerComponent(sGUID, sName, data, OutputDiv, _uidialog) {
    return new Promise(function (resolve, reject) {
        if (data.Registers && data.Registers.length > 0) {
            var j = 0;
            for (var i = 0; i < data.Registers.length; i++) {
                if (data.Registers[i].sType && data.Registers[i].sType == "template") {
                    var Step = data.Registers[i].sValue;
                    var sOutputArea = data.Registers[i].sValue.split('_')[0];
                    var StepID = 0;
                    fncTriggerStep(Step, StepID, sGUID, _uidialog, sOutputArea);
                }
                else {
                    var Step = data.Registers[i].sValue;
                    var StepID = data.Registers[i].sValue.split('_')[1];
                    var CompName = data.Registers[i].sValue.split('_')[0];
                    if (sName != CompName)
                        fncTriggerStep(Step, StepID, sGUID, _uidialog, OutputDiv);
                }

            }
        }
        resolve(true);
    })
}

function fncTriggerStep(Step, StepID, sGUID, _uidialog, sOutputArea) {
    $.ajax({
        type: 'POST',
        async: true,
        url: LoadStepURL,
        data: JSON.stringify({ sStep: Step, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (StepData) {
            if (sOutputArea && sOutputArea.length > 0) {
                var Output = '#' + sOutputArea;
                fncRenderStepContent(StepData, Output, 0, "trigger_template", _uidialog);
            }
            else {
                var Output = 'div[data-value="' + StepID + '"]';
                fncRenderStepContent(StepData, Output, 0, "trigger", _uidialog);
            }

            //$('div[data-value="' + StepID + '"]').replaceWith(StepData);
        },
        error: function (Errordata) {
        }
    });
}

function LoadSubnodeStep(sGUID, StepName, iQSDID, outputID, outputDiv) {
    // var StepID = 199;
    //var PlaceHolderID = Layout.Mappings[i].PlaceHolderID;
    var oParams = [];
    var Param = {};
    //Param["sName"] = "{XIP|iStepDID}";
    ////Param["sName"] = "{XIP|sStepName}";
    //Param["sValue"] = StepID;
    Param["sName"] = "{XIP|sStepName}";
    //Param["sName"] = "{XIP|sStepName}";
    Param["sValue"] = StepName;
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|iQSDID}";
    Param["sValue"] = iQSDID;
    oParams.push(Param);
    $.ajax({
        url: ComponentURL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "HTML",
        cache: false,
        async: false,
        data: JSON.stringify({ iXIComponentID: 0, sGUID: sGUID, sName: 'Step Component', nParams: oParams }),
        success: function (StepContent) {
            //$('#LeftTreeOutput').html(StepContent);
            //$('#' + Output).html(StepContent);
            if (outputID != 0) {
                $('#Component-' + outputID).html(StepContent);
            } else if (outputDiv != 'undefined') {
                $('#' + outputDiv).html(StepContent);
                //$('div[data-guid="' + sGUID + '"]  #' + outputDiv).html(StepContent);
            }
            //$(".PopupTabContentArea").height($('.ui-dialog').height() - $("#DynamicQueryForm").height() - 100);

            //var PlaceHolderID = Layout.Mappings[i].PlaceHolderID;
            //for (j = 0; j < Layout.Details.length; j++) {
            //    if (Layout.Details[j].PlaceHolderID == PlaceHolderID) {
            //        $('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(StepContent);
            //    }
            //}
            //$('#Step_' + StepID).html(StepContent);
            //if (StepContent.iLayoutID > 0) {
            //    fncgetlayoutcontent(StepContent.Layout, sGUID)
            //}
            //else {
            //    //fncGetStepData(Layout, StepContent, PlaceHolderID, sGUID, "Step_"+StepContent.ID);
            //}
        }
    });
}

function fncGetComponentParamsByStep(StepID) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: ComponentParmsByStepURL,
            data: JSON.stringify({ StepID: StepID }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function (Params) {
                resolve(Params);
            }
        });
    });
}

function fncUpdateInstanceID(data, sGUID) {
    if (UpdateInstanceIDURL) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'POST',
                url: UpdateInstanceIDURL,
                data: JSON.stringify({ oBOInstance: data, sGUID: sGUID }),
                contentType: 'application/json;',
                dataType: 'json',
                traditional: true,
                success: function (Params) {
                    resolve(Params);
                }
            });
        });
    }
    else {
        return new Promise(function (resolve, reject) {
            resolve(true);
        });
    }
}

//Get QS DefintionID from HTML Tree
function fncQSDefinitionIDFromHTMLTree(parentName, childObj) {
    var QSDefinitionID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-name') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return QSDefinitionID
        }
    }
    QSDefinitionID = testObj.getAttribute('id').split('_')[1];
    return QSDefinitionID;
}

function fncQSInfoFromHTMLTree(jqObj) {
    var QSDefinitionID = fncgetQSDefinitionIDFromHTMLTree("QuestionSet", jqObj);
    var StepDefinitionID = fncgetStepDefinitionIDFromHTMLTree("QSStep", jqObj);
    var SectionDefinitionID = fncgetSectionDefinitionIDFromHTMLTree("QSSection", jqObj);
    var QSParams = [];
    var param1 = {};
    param1["sName"] = "iQSDID";
    param1["sValue"] = QSDefinitionID;
    var param2 = {};
    param2["sName"] = "iStepDID";
    param2["sValue"] = StepDefinitionID;
    var param3 = {};
    param3["sName"] = "iSectionDID";
    param3["sValue"] = SectionDefinitionID;
    QSParams.push(param1);
    QSParams.push(param2);
    QSParams.push(param3);
    return QSParams;
}


//Get QS DefintionID from HTML Tree
function fncgetQSDefinitionIDFromHTMLTree(parentName, childObj) {
    var QSDefinitionID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-info') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return QSDefinitionID
        }
    }
    QSDefinitionID = testObj.getAttribute('data-value');
    return QSDefinitionID;
}

//Get Step DefintionID from HTML Tree
function fncgetStepDefinitionIDFromHTMLTree(parentName, childObj) {
    var StepDefinitionID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-info') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return StepDefinitionID
        }
    }
    StepDefinitionID = testObj.getAttribute('data-value');
    return StepDefinitionID;
}

//Get Section DefintionID from HTML Tree
function fncgetSectionDefinitionIDFromHTMLTree(parentName, childObj) {
    var SectionDefinitionID = 0;
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('data-info') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else {
            return SectionDefinitionID
        }
    }
    SectionDefinitionID = testObj.getAttribute('data-value');
    return SectionDefinitionID;
}

//Calling QS Event
function funCallQSEvent(QSInfo, QSEvents) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: CallQSEventURL,
            data: JSON.stringify({ QSInfo: QSInfo, QSEvents: QSEvents }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                resolve(data);
            }
        });
    });
}

//XILink Load HTML
function XILinkLoad(iXILinkID) {
    $.ajax({
        type: 'POST',
        url: XILinkLoadURL,
        data: JSON.stringify({ iXILinkID: iXILinkID }),
        contentType: 'application/json;',
        dataType: 'html',
        traditional: true,
        success: function (data) {
            $('#Dialogs').append(data);
            $('#Dialogs').dialog({
                height: 700,
                width: 800
            });
        }
    });
}

function fncXILinkLoad(iXILinkID, iInstanceID, sBOName, sAttrName, $this) {

    var sGUID = null;
    if ($this != undefined && $this != null) {
        sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    }

    if (sAttrName && sAttrName.length > 0 && sAttrName.startsWith('s')) {
        var Params = {
            sBOName: sBOName,
            sAttr: sAttrName,
            sValue: iInstanceID
        }
        $.ajax({
            url: GetBOInstanceIDURL,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            cache: false,
            async: false,
            data: JSON.stringify(Params),
            success: function (data) {
                iInstanceID = data;
            },
            error: function (textStatus, errorThrown) {
            }
        });
    }
    var InputParams = [];
    var param = {};
    param["sName"] = '{-iInstanceID}';
    param["sValue"] = iInstanceID;
    InputParams.push(param);
    var param2 = {};
    param2["sName"] = 'sBOName';
    param2["sValue"] = sBOName;
    InputParams.push(param2);
    var param3 = {};
    param3["sName"] = '{XIP|' + sBOName + '.id}';
    param3["sValue"] = iInstanceID;
    InputParams.push(param3);
    var param4 = {};
    param4["sName"] = '{XIP|sBOName}';
    param4["sValue"] = sBOName;
    InputParams.push(param4);
    XILinkLoadJson(iXILinkID, sGUID, InputParams);
}

function fncScriptEdit(iXILinkID, iInstanceID, sAttrName, sBOName, $this) {
    var sGUID = null;
    if ($this != undefined && $this != null) {
        sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    }
    var InputParams = [];
    var param = {};
    param["sName"] = '{-iInstanceID}';
    param["sValue"] = iInstanceID;
    InputParams.push(param);
    var param2 = {};
    param2["sName"] = 'sBOName';
    param2["sValue"] = sBOName;
    InputParams.push(param2);
    var param3 = {};
    param3["sName"] = '{XIP|' + sBOName + '.id}';
    param3["sValue"] = iInstanceID;
    InputParams.push(param3);
    var param4 = {};
    param4["sName"] = '{XIP|sBOName}';
    param4["sValue"] = sBOName;
    InputParams.push(param4);
    XILinkLoadJson(iXILinkID, sGUID, InputParams);

    XIRun(null, o1ClickDJ.RowXiLinkID, parseInt(iInstanceID), sGUID, o1ClickDJ.sBOName, false, parseInt(o1ClickDJ.BOID), 0, null, InputParams);
}
//XILink Load HTML
function XILinkLoadJson_old(iXILinkID, sGUID, oParams, MenuName) {
    if (MenuName != undefined) {
        if (MenuName.toLowerCase() == 'Policy Payment Charge'.toLowerCase()) {
            ActRec = 'pfpayrec'
        } else if (MenuName.toLowerCase() == 'Charges Drawdown'.toLowerCase()) {
            ActRec = 'chargesdrawdown'
        } else if (MenuName.toLowerCase() == '50/50 Payment Charge'.toLowerCase()) {
            ActRec = 'ffpayrec'
        } else if (MenuName.toLowerCase() == 'Commission Drawdown'.toLowerCase()) {
            ActRec = 'commissiondrawdown'
        } else if (MenuName.toLowerCase() == 'Supplier Payments'.toLowerCase()) {
            ActRec = 'insurerpay'
        } else if (MenuName.toLowerCase() == 'Pre-Banking'.toLowerCase()) {
            ActRec = 'prebankrec'
        } else if (MenuName.toLowerCase() == 'Bank Reconciliation'.toLowerCase()) {
            ActRec = 'bankrec'
        } else if (MenuName.toLowerCase() == 'Premium Finance Reconciliation'.toLowerCase()) {
            ActRec = 'reconcilepf'
        }
        if (oParams == null) {
            oParams = [];
        }
        var Param = {};
        Param["sName"] = "ActRec";
        Param["sValue"] = ActRec;
        oParams.push(Param);
    }
    $.ajax({
        type: 'POST',
        url: XILinkLoadJsonURL,
        data: JSON.stringify({ iXILinkID: iXILinkID, sGUID: sGUID, oParams: oParams }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        async: true,
        success: function (XIData) {
            var oXiLink = JSON.parse(XIData);
            if (oXiLink.oContent.hasOwnProperty("xilink")) {
                var oXiLinkI = oXiLink.oContent["xilink"];
                if (oXiLinkI.sOutput && oXiLinkI.sOutput.length > 0) {
                    $('#' + oXiLinkI.sOutput).html('<div class="loader"></div><span></span>');
                }
                if (oXiLinkI.oContent.hasOwnProperty("dialog")) {
                    var oDialogIns = oXiLinkI.oContent["dialog"];
                    if (oDialogIns.oContent.hasOwnProperty("dialog")) {
                        var oDialogD = oDialogIns.oContent["dialog"];
                        if (oDialogD.oContent.hasOwnProperty("layout")) {
                            var oLayIns = oDialogD.oContent["layout"];
                            if (oLayIns.oContent.hasOwnProperty("layout")) {
                                $.ajax({
                                    url: XIContentURL,
                                    type: "POST",
                                    contentType: "application/json; charset=utf-8",
                                    datatype: "HTML",
                                    cache: false,
                                    async: true,
                                    data: JSON.stringify({ oData: JSON.stringify(oLayIns).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                                    success: function (oLaydata) {
                                        var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h" title="" onclick="fncdialogchange(this, &quot;maxwidth&quot;)"></i>';
                                        var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v" onclick="fncdialogchange(this, &quot;maxheight&quot;)"></i>';
                                        var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minwidth&quot;)"></i>';
                                        var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minheight&quot;)"></i>';
                                        if (oDialogD.IsMinimiseIcon == true) {
                                            var MinDia = '<i class="Minimize fa fa-window-minimize" onclick="fncdialogchange(this, &quot;minimize&quot;)"></i>';
                                        }
                                        else {
                                            var MinDia = "";
                                        }
                                        if (oDialogD.IsMaximiseIcon == true) {
                                            var MaxDia = '<i class="Maximize far fa-window-maximize" onclick="fncdialogchange(this, &quot;maximize&quot;)"></i>';
                                        }
                                        else {
                                            var MaxDia = "";
                                        }
                                        var RestoreDia = '<i class="RestoreDown far fa-window-restore" onclick="fncdialogchange(this, &quot;restore&quot;)"></i>';
                                        var DialogDivID = "ResultDialog-" + oDialogD.ID;
                                        //$('.' + DialogDivID).dialog('destroy').remove();
                                        var Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a></div>';
                                        $('#Dialogs').append(Div);
                                        //var sLayHTML = '<div data-guid="' + oXiLinkI.sGUID + '" data-name="lguid">' + oLaydata + '</div>';
                                        $("." + DialogDivID).html(oLaydata);
                                        var TValue = oDialogD.iTransparency;
                                        TransValue = TValue / 100;
                                        Transparent(TransValue);
                                        var DialogOpacityValue = DiaOpacity;
                                        if (oDialogD.PopupSize == "Default") {
                                            oDialogD.DialogHeight = 800;
                                            oDialogD.DialogWidth = 800;
                                        }
                                        $("." + DialogDivID).dialog({
                                            title: ' ',
                                            //modal: true,
                                            height: oDialogD.DialogHeight,
                                            width: oDialogD.DialogWidth,
                                            resizable: oDialogD.IsResizable,
                                            IsCloseIcon: oDialogD.IsCloseIcon,
                                            BarPosition: oDialogD.BarPosition,
                                            dialogClass: 'titlebardata ' + DialogOpacityValue,
                                            buttons: [
                                            ],
                                            open: function () {
                                                $(this).parent().promise().done(function () {
                                                    var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                                                    $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                                                    $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + MinDia + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + '</div>');
                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();
                                                    var uidialog = $(this);
                                                    var dlgStyle = uidialog[0].attributes["style"].textContent;
                                                    if (dlgStyle && dlgStyle.length > 0) {
                                                        var Styles = dlgStyle.split(";");
                                                        for (var k = 0; k < Styles.length; k++) {
                                                            var Sty = Styles[k].trim();
                                                            var st = Sty.split(":");
                                                            if (st[0].trim() == "width") {
                                                                dlgWidth = st[1].trim();
                                                            }
                                                            else if (st[0].trim() == "height") {
                                                                dlgHeight = oDialogD.DialogHeight + "px";
                                                            }
                                                            else if (st[0].trim() == "top") {
                                                                dlgTop = st[1].trim();
                                                            }
                                                            else if (st[0].trim() == "left") {
                                                                dlgLeft = st[1].trim();
                                                            }
                                                        }
                                                        $(this).attr('data-dlgWidth', dlgWidth);
                                                        $(this).attr('data-dlgHeight', dlgHeight);
                                                        $(this).attr('data-dlgTop', dlgTop);
                                                        $(this).attr('data-dlgLeft', dlgLeft);
                                                        var TabHeight = $(this).find('#TabsArea').height();
                                                        //$(this).find(".PopupTabContentArea_0").height(oDialogD.DialogHeight - 70)//.animate({ height: Dialogdata.DialogHeight - 50 });
                                                        //$(this).find(".PopupTabContentArea_").height(oDialogD.DialogHeight - 110 - TabHeight)//.animate({ height: Dialogdata.DialogHeight - 50 - TabHeight });
                                                        //$(this).find("#TreeScrollbar").animate({ height: oDialogD.DialogHeight - 70 });
                                                        $(".PopupTabContentArea").height(dlgHeight - 200);
                                                        $("#TreeScrollbar").height(dlgHeight - 200);
                                                    }

                                                });
                                            },
                                            close: function (event, ui) {
                                                //$('#Dialogs').parent().hide();
                                                $("." + DialogDivID).dialog('close');
                                                $("." + DialogDivID).dialog('destroy').remove();
                                            }
                                        });
                                        //$('.ui-dialog-title').html('<div class="dialogtitleholder"></div>');                                        

                                    },
                                    error: function (data) {
                                        //alert(data);
                                    }
                                });
                            }
                        }
                    }
                    else if (obj.hasOwnProperty("popup")) {

                    }
                }
                else if (oXiLinkI.oContent.hasOwnProperty("layout")) {
                    $.ajax({
                        url: XIContentURL,
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        datatype: "HTML",
                        cache: false,
                        async: true,
                        data: JSON.stringify({ oData: JSON.stringify(oXiLinkI.oContent["layout"]).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                        success: function (CompnentData) {
                            //if (oXiLinkI.oDefintion.XiLinkNVs[0].Name == 'Output') {
                            $('#' + oXiLinkI.sOutput).html(CompnentData);
                            //}


                        }
                    });
                }
                else if (oXiLinkI.oContent.hasOwnProperty("xicomponent")) {
                    $.ajax({
                        url: XIContentURL,
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        datatype: "HTML",
                        cache: false,
                        async: true,
                        data: JSON.stringify({ oData: JSON.stringify(oXiLinkI.oContent["xicomponent"]).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                        success: function (CompnentData) {
                            if (oXiLinkI.oDefintion.XiLinkNVs[0].Name == 'Output') {
                                $('#' + oXiLinkI.oDefintion.XiLinkNVs[0].Value).html(CompnentData);
                            }


                        }
                    });
                }

            }
        },
        error: function (data) {
            //alert(data);
        }
    });
}
//XILink Load HTML
function XILinkLoadJson(iXILinkID, sGUID, oParams, MenuName) {
debugger
    if (oParams && oParams.length > 100) {
        oParams = JSON.parse(oParams)
    }
    var sOutputArea = '';
    if (oParams && oParams.length > 0) {
        if (oParams && oParams.length > 0) {
            var Parameter = oParams.filter(function (x) { return x.sName === "sOutput"; });
            if (Parameter && Parameter.length > 0) {
                sOutputArea = Parameter[0].sValue;
            }
        }
    }
    var sGUID1 = sGUID;
    if (iXILinkID) {
        $.ajax({
            type: 'POST',
            url: SecureKeyURL,
            data: { sSecureKey: iXILinkID },
            cache: false,
            async: true,
            dataType: 'json',
            success: function (oResponse) {
                iXILinkID = oResponse;
                if (iXILinkID == 0) {
                    return false;
                }
                var iBOIID = 0;
                var sBOName = "";
 		var sRowName = "";
                var sClkType = "";
                var sBOLabel = ""; var bIsLock = false;
                if (oParams) {
                    for (var m = 0; m < oParams.length; m++) {
                        if (oParams[m].sName == "sBOName") {
                            sBOName = oParams[m].sValue;
                        }
                        else if (oParams[m].sName == "iBOIID") {
                            iBOIID = oParams[m].sValue;
                        }
                        else if (oParams[m].sName == "sType") {
                            sClkType = oParams[m].sValue;
                        }
                        else if (oParams[m].sName == "sBOLabel") {
                            sBOLabel = oParams[m].sValue;
                        }
                        else if (oParams[m].sName == "bIsLock") {
                            bIsLock = oParams[m].sValue;
                        }
 			else if (oParams[m].sName == "rowname") {
                            sRowName = oParams[m].sValue;
                        }
                    }
                }
                //Check is already added to Taskbar
                var bFound = false;
                var TBID;
                if (sClkType && sClkType == "Menu") {
                    TBID = "Menu-" + MenuName.replace(/ /g, '') + "-" + iXILinkID;
                }
                else if (sBOLabel && iBOIID && sBOLabel.length > 0 && iBOIID > 0) {
                    TBID = "TaskBarBtn-" + sBOLabel + iBOIID + iXILinkID;
                }
                if (TBID) {
                    $('#NavigationBar').find('.btnTabs').find('.dialogNavBtn').each(function (j, obj) {
                        var TaskBtn = $(this).find('button.taskbtn');
                        if (TaskBtn && TaskBtn.length > 0) {
                            var sBtnType = TaskBtn[0].attributes["data-navtype"].textContent;
                            var TBtnID = TaskBtn[0].attributes["id"];
                            if (TBtnID) {
                                var TBtn = TaskBtn[0].attributes["id"].textContent;
                                if (sBtnType == "single") {
                                    if (TBtn == TBID) {
                                        bFound = true;
                                    }
                                }
                                else if (sBtnType == "group") {
                                    var dlg = $('div[data-identity="' + TBtn + '"]');
                                    if (dlg.length > 0) {
                                        var vBar = $('div[data-identity="' + TBtn + '"]').find('.VerticalLeftBar');
                                        if (vBar.length > 0) {
                                            $('div[data-identity="' + TBtn + '"]').find('.VerticalLeftBar').find('button').each(function () {
                                                var finaldiv = $('div[data-identity="' + TBtn + '"]').find('.ResultDialog-' + sBOName + "_" + iBOIID);
                                                if (finaldiv.length > 0) {
                                                    $('.ResultDialog-' + sBOName + "_" + iBOIID).parent().show();
                                                    $('.ResultDialog-' + sBOName + "_" + iBOIID).parent().find('.innerdialog-box').hide();
                                                    $('.ResultDialog-' + sBOName + "_" + iBOIID).show();
                                                    TBID = TBtn;
                                                    bFound = true;
                                                }

                                            });
                                        }
                                    }
                                }
                            }
                        }
                        if (bFound) {
                            return false;
                        }
                    });
                }
                if (bFound) {
                    $('div[data-identity="' + TBID + '"]').show();
                    var Zindex = 1010;
                    $('body').find('.ui-dialog').each(function () {
                        Zindex--;
                        $(this).css('z-index', Zindex);
                    });
                    $('div[data-identity="' + TBID + '"]').css('z-index', '1012');
                    return;
                }
                if (MenuName != undefined) {
                    if (MenuName.toLowerCase() == 'Charges Drawdown'.toLowerCase()) {
                        ActRec = 'chargesdrawdown';
                        EnumReconciliation = '10';
                    } else if (MenuName.toLowerCase() == 'Commission Drawdown'.toLowerCase()) {
                        ActRec = 'commissiondrawdown';
                        EnumReconciliation = '20';
                    } else if (MenuName.toLowerCase() == 'Premium Finance Reconciliation'.toLowerCase()) {
                        ActRec = 'reconcilepf';
                        EnumReconciliation = '30';
                    } else if (MenuName.toLowerCase() == 'Supplier Payments'.toLowerCase()) {
                        ActRec = 'insurerpay';
                        EnumReconciliation = '40';
                    } else if (MenuName.toLowerCase() == 'Pre-Banking'.toLowerCase()) {
                        ActRec = 'prebankrec';
                        EnumReconciliation = '60';
                    } else if (MenuName.toLowerCase() == 'Bank Reconciliation'.toLowerCase()) {
                        ActRec = 'bankrec';
                        EnumReconciliation = '50';
                    } else if (MenuName.toLowerCase() == 'Policy Payment Charge'.toLowerCase()) {
                        ActRec = 'pfpayrec';
                        EnumReconciliation = '70';
                    } else if (MenuName.toLowerCase() == '50/50 Payment Charge'.toLowerCase()) {
                        ActRec = 'ffpayrec';
                        EnumReconciliation = '80';
                    }
                    if (oParams == null) {
                        oParams = [];
                    }
                    var Param = {};
                    Param["sName"] = "ActRec";
                    Param["sValue"] = ActRec;
                    oParams.push(Param);
                    Param = {};
                    Param["sName"] = "EnumReconciliation";
                    Param["sValue"] = EnumReconciliation;
                    oParams.push(Param);
                }
                var bIsPopup = false;
                var iPopupID = 0;
                $.ajax({
                    type: 'POST',
                    url: XILinkDefinitionURL,
                    data: JSON.stringify({ XiLinkID: iXILinkID }),
                    contentType: 'application/json;',
                    dataType: 'json',
                    traditional: true,
                    async: true,
                    success: function (XIData) {
                        var XILinkClasses = "one-link one-link-id-" + XIData.XiLinkID + " one-link-" + XIData.Name.toLowerCase().replace(/ /g, '_');
                        var NVs = XIData.XiLinkNVs;
                        if (NVs != null) {
                            for (var i = 0; i < XIData.XiLinkNVs.length; i++) {
                                if (XIData.XiLinkNVs[i].Name == "StartAction" && XIData.XiLinkNVs[i].Value == "Popup") {
                                    bIsPopup = true;
                                }
                                else if (XIData.XiLinkNVs[i].Name == "PopupID") {
                                    iPopupID = XIData.XiLinkNVs[i].Value;
                                }
                            }
                            if (bIsPopup && iPopupID > 0) {
                                $.ajax({
                                    type: 'POST',
                                    url: PopupDefinitionURL,
                                    data: JSON.stringify({ PopupID: iPopupID, oParams: oParams }),
                                    contentType: 'application/json;',
                                    dataType: 'html',
                                    traditional: true,
                                    async: true,
                                    success: function (XIData) {
                                        var Popup = window.open('', '_blank');
                                        with (Popup.document) {
                                            open();
                                            write(XIData);
                                            close();
                                        }
                                        Popup.moveTo(0, 0);
                                    },
                                    error: function (error) {
                                    }
                                });
                            }
                            else {
                                $('body').prepend('<div class="loader"><span></span>');
                                $('.page-wrapper').addClass("blr");
                                $('.page-footer').addClass("blr");
                                $.ajax({
                                    type: 'POST',
                                    url: XILinkLoadJsonURL,
                                    data: JSON.stringify({ iXILinkID: iXILinkID, sGUID: sGUID, oParams: oParams }),
                                    contentType: 'application/json;',
                                    dataType: 'json',
                                    traditional: true,
                                    async: true,
                                    success: function (XIData) {
                                        /*setTimeout(function () {
                                            $(window).resize(function () {
                                                $('embed').height($('.PopupTabContentArea').height() - 100);
                                            }).trigger('resize');
                                        }, 1000);*/
                                        var oXiLink = XIData;
                                        if (oXiLink.sOutput && oXiLink.sOutput.length > 0 && oXiLink.sOutput != "Inline") {
                                            $('#' + oXiLink.sOutput).html('<div class="loader"></div>');
                                        }
                                        if (oXiLink.oContent.hasOwnProperty("dialog")) {
                                            var TaskBarID;
                                            var oDialogD = oXiLink.oContent["dialog"];
                                            var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h" title="" onclick="fncdialogchange(this, &quot;maxwidth&quot;)"></i>';
                                            var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v" onclick="fncdialogchange(this, &quot;maxheight&quot;)"></i>';
                                            var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minwidth&quot;)"></i>';
                                            var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minheight&quot;)"></i>';
                                            if (oDialogD.IsMinimiseIcon == true) {
                                                var MinDia = '<i class="Minimize fa fa-window-minimize" onclick="fncdialogchange(this, &quot;minimize&quot;)"></i>';
                                            }
                                            else {
                                                var MinDia = "";
                                            }
                                            if (oDialogD.IsMaximiseIcon == true) {
                                                var MaxDia = '<i class="Maximize far fa-window-maximize" onclick="fncdialogchange(this, &quot;maximize&quot;)"></i>';
                                            }
                                            else {
                                                var MaxDia = "";
                                            }
                                            var RestoreDia = '<i class="RestoreDown far fa-window-restore" onclick="fncdialogchange(this, &quot;restore&quot;)"></i>';
                                            var resetPosition = '<i class="resetPosition fa fa-retweet" onclick="fncdialogchange(this, &quot;defaultPosition&quot;)"></i>';
                                            var DialogOpacityValue = fncGetDialogOpacity(oDialogD.iTransparency);
                                            if (oDialogD.PopupSize == "Default") {
                                                //oDialogD.DialogHeight = screen.height - 190;
                                                //oDialogD.DialogWidth = screen.width - 400;
                                                oDialogD.DialogHeight = $(window).height() * 0.9;//
                                                oDialogD.DialogWidth = $(window).width() * 0.9;//
                                            }
                                            var uidialog = 0;
                                            if (oDialogD.IsGrouping == true) {
                                                if (oDialogD.oContent.hasOwnProperty("layout")) {
                                                    //var DlgDiv = '<div class="dialog-' + oDialogD.ID + '">';
                                                    var DialogDivID = "ResultDialog-" + sBOName + "_" + iBOIID;
                                                    var bIsDivExists = $('#DialogContent').find('.' + DialogDivID);
                                                    if (bIsDivExists && bIsDivExists.length > 0) {
                                                        $("#DialogGroups").dialog({
                                                            title: ' ',
                                                            appendTo: "body",
                                                            height: oDialogD.DialogHeight,
                                                            width: oDialogD.DialogWidth,
                                                            resizable: oDialogD.IsResizable,
                                                            IsCloseIcon: oDialogD.IsCloseIcon,
                                                            BarPosition: oDialogD.BarPosition,
                                                            dialogClass: DialogOpacityValue,
                                                            buttons: [
                                                            ],
                                                            close: function (event, ui) {
                                                                $('#DialogGroups').parent().hide();
                                                                $(this).dialog('destroy').remove();
                                                            },
                                                            position: { my: "right top", at: "right+100 top+100", of: "body" },
                                                        }).dialog("widget")
                                                            //.draggable("option", "containment", "false")
                                                            .dblclick(function () {
                                                                $(this).toggleClass("fullScreenToggle");
                                                            });
                                                        // .position({
                                                        // my: "center",
                                                        // at: "center",
                                                        // of: window
                                                        // });
                                                        ////////////////
                                                        //var headerTab = $('.nav-tabs-custom').height();
                                                        //$(".ui-dialog .PopupTabContentArea_0").css('height', ($('.ui-dialog').height() - headerTab - 37) + 'px');
                                                        //$(".ui-dialog .PopupTabContentArea_").css('height', ($('.ui-dialog').height() - 100) + 'px');
                                                        //$(".ui-dialog #TreeScrollbar").css('height', ($('.ui-dialog').height() - 100) + 'px');

                                                        //$(".ui-dialog #BOForm").css('height', (($('.ui-dialog').height()) / 2) + 'px');
                                                        //$(".ui-dialog #OneClickData").css('height', (($('.ui-dialog').height()) / 2) + 'px');

                                                        //$(".ui-dialog, .PopupTabContentArea_, .PopupTabContentArea_0, #TreeScrollbar, #BOForm, #OneClickData").resizable({
                                                        //    alsoResize: ".PopupTabContentArea_, .PopupTabContentArea_0, #TreeScrollbar, #BOForm, #OneClickData, .ui-dialog"
                                                        //});
                                                        /////////////////
                                                        var BtnID = "DialogBarBtn-" + sBOName + "-" + iBOIID;
                                                        fncDialogNavigate(BtnID);
                                                    }
                                                    else {

                                                        //$('.' + DialogDivID).dialog('destroy').remove();
                                                        //var Div = "";
                                                        //var AnyDivs = $('#DialogContent').find('.dialog-' + oDialogD.ID);
                                                        //Div = '<div class="innerdialog-box ' + DialogDivID + '" title="Confirm Message" data-dname="' + DialogDivID + '"></div>';
                                                        //if (AnyDivs.length > 0) {
                                                        //    $('#DialogContent').find('.dialog-' + oDialogD.ID).find('.innerdialog-box').hide();
                                                        //    $('#DialogContent').find('.dialog-' + oDialogD.ID).append(Div);
                                                        //}
                                                        //else {
                                                        //    DlgDiv = DlgDiv + Div + '</div>';
                                                        //    $('#DialogContent').append(DlgDiv);
                                                        //}
                                                        var GrpDlg = 'dialog-' + oDialogD.ID;
                                                        var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, false, &quot;' + GrpDlg + '&quot;)"></i>';
                                                        var AnyDivs = $('.dialog-' + oDialogD.ID);
                                                        var DlgClasses = "one-dialog one-dialog-id-" + oDialogD.ID + " one-dialog-" + oDialogD.DialogName.toLowerCase().replace(/ /g, '_');
                                                        if (AnyDivs && AnyDivs.length > 0) {
                                                            $('.dialog-' + oDialogD.ID).find('.innerdialog-box').hide();
                                                            $('.dialog-' + oDialogD.ID).find('.GroupbarWrapper').append('<div class="' + DialogDivID + ' ' + DlgClasses + ' innerdialog-box" data-def="' + oDialogD.XIGUID + '"></div>');
                                                        }
                                                        else {
                                                            var DialogBar = "";
                                                            var Div = "";
                                                            if (oDialogD.BarPosition && oDialogD.BarPosition.length > 0) {
                                                                DialogBar = '<div class="GroupbarWrapper ' + oDialogD.BarPosition + '"><div class="VerticalLeftBar" id="VBar-' + oDialogD.ID + '"></div>';
                                                                Div = '<div class="dialog-box ' + GrpDlg + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a>' + DialogBar + '<div class="' + DialogDivID + ' ' + DlgClasses + ' innerdialog-box" data-def="' + oDialogD.XIGUID + '"></div></div></div>';
                                                            }
                                                            else {
                                                                Div = '<div class="dialog-box ' + GrpDlg + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a>' + DialogBar + '<div class="' + DialogDivID + ' ' + DlgClasses + ' innerdialog-box" data-def="' + oDialogD.XIGUID + '"></div></div></div>';
                                                            }
                                                            $('#Dialogs').append(Div);
                                                        }
                                                        var oLayoutD = oDialogD.oContent["layout"];
                                                        var bIsTaskBar = oLayoutD.bIsTaskBar;
                                                        if (bIsTaskBar) {
                                                            var TBarPosition = oLayoutD.sTaskBarPosition;
                                                            if (!TBarPosition || TBarPosition.length == 0) {
                                                                TBarPosition = "left";
                                                            }
                                                            oLayoutD.LayoutCode = '<div class="NavbarWrapper ' + TBarPosition + '"><div id="NavigationBar-' + oLayoutD.ID + '"></div>' + oLayoutD.LayoutCode + '</div>'
                                                        }

                                                        var LayoutHTML = '<div class="LayoutCode_' + oLayoutD.sGUID + '" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID">' + oLayoutD.LayoutCode + '</div>';
                                                        $("." + DialogDivID).html(LayoutHTML);
                                                        var BtnID = "DialogBarBtn-" + sBOName + "-" + iBOIID;
                                                        var IsExists = "No";
                                                        var Btns = $('#VBar-' + oDialogD.ID).find('.btnTabs');
                                                        if (Btns.length > 0) {
                                                            var VBar = '<div class="dialogNavBtn"><button type="button" class="btn btn-theme btn-xs" id="' + BtnID + '" onclick="return fncDialogNavigate(&quot;' + BtnID + '&quot;,&quot;' + GrpDlg + '&quot; )" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + iBOIID + '<div class="closeNavBtn" onclick="return fncCloseGroupBtn(&quot;' + DialogDivID + '&quot;, this)">&times;</div></span></div>';
                                                            $('#VBar-' + oDialogD.ID).find('.btnTabs').append(VBar);
                                                        }
                                                        else {
                                                            var VBar = '<div class="btnTabs left"><div class="dialogNavBtn"><button type="button" class="btn btn-theme btn-xs" id="' + BtnID + '" onclick="return fncDialogNavigate(&quot;' + BtnID + '&quot;,&quot;' + GrpDlg + '&quot; )" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + iBOIID + '<div class="closeNavBtn" onclick="return fncCloseGroupBtn(&quot;' + DialogDivID + '&quot;, this)">&times;</div></span></div></div>';
                                                            $('#VBar-' + oDialogD.ID).append(VBar);
                                                        }
                                                        //var VBar = '<button type="button" class="btn btn-dark btn-xs dialogNavBtn" id="' + BtnID + '" onclick="return fncDialogNavigate(&quot;' + BtnID + '&quot;,&quot;' + GrpDlg + '&quot; )" ><i class="fa fa-user-plus" ></i><span class="" >' + iBOIID + '</span></button>';
                                                        $('#VBar-' + oDialogD.ID + ' button').each(function (j, obj) {
                                                            var id = $(this).attr('id');
                                                            if (id == BtnID) {
                                                                IsExists = "Yes";
                                                            }
                                                        });
                                                        if (IsExists == "No") {
                                                            $('#VBar-' + oDialogD.ID).append(VBar);
                                                        }
                                                        if (sClkType && sClkType.length > 0) {
                                                            TaskBarID = sClkType + "-" + iXILinkID;
                                                        }
                                                        else {
                                                            TaskBarID = "GroupDlg-" + sBOName;
                                                        }
                                                        //var IsDivExists = fncIsDivAlreadyExists('Dialog-' + oDialogD.ID, DialogDivID);
                                                        $("." + GrpDlg).dialog({
                                                            title: ' ',
                                                            //modal: true,
                                                            height: oDialogD.DialogHeight,
                                                            width: oDialogD.DialogWidth,
                                                            resizable: oDialogD.IsResizable,
                                                            IsCloseIcon: oDialogD.IsCloseIcon,
                                                            BarPosition: oDialogD.BarPosition,
                                                            dialogClass: 'titlebardata ' + DialogOpacityValue,
                                                            buttons: [
                                                            ],
                                                            open: function () {
                                                                $(this).parent().promise().done(function () {
                                                                    var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                                                                    $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                                                                    $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + MinDia + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                                                                    $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();
                                                                    $(this).children('.ui-dialog-title').html('<span class="fc-red">Alert message !!!</span>');
                                                                    //<span class="fc-green">Success message !!!</span>
                                                                    uidialog = $(this);
                                                                    var dlgStyle = uidialog[0].attributes["style"].textContent;
                                                                    if (dlgStyle && dlgStyle.length > 0) {
                                                                        var Styles = dlgStyle.split(";");
                                                                        for (var k = 0; k < Styles.length; k++) {
                                                                            var Sty = Styles[k].trim();
                                                                            var st = Sty.split(":");
                                                                            if (st[0].trim() == "width") {
                                                                                dlgWidth = st[1].trim();
                                                                            }
                                                                            else if (st[0].trim() == "height") {
                                                                                dlgHeight = oDialogD.DialogHeight + "px";
                                                                            }
                                                                            else if (st[0].trim() == "top") {
                                                                                dlgTop = st[1].trim();
                                                                            }
                                                                            else if (st[0].trim() == "left") {
                                                                                dlgLeft = st[1].trim();
                                                                            }
                                                                        }
                                                                        $(this).attr('data-dlgWidth', dlgWidth);
                                                                        $(this).attr('data-dlgHeight', dlgHeight);
                                                                        $(this).attr('data-dlgTop', dlgTop);
                                                                        $(this).attr('data-dlgLeft', dlgLeft);
                                                                        $(this).attr('data-identity', TaskBarID);
                                                                    }

                                                                });
                                                            },
                                                            close: function (event, ui) {
                                                                $('#DialogGroups').parent().hide();
                                                                $(this).dialog('destroy').remove();
                                                            }
                                                        }).dialog("widget")
                                                            //.draggable("option", "containment", "false")
                                                            .dblclick(function () {
                                                                $(this).toggleClass("fullScreenToggle");
                                                            });
                                                        $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                                                            var Details = oLayoutD.LayoutDetails;
                                                            var id = $(this).attr('id');
                                                            if (id && id.length > 0) {
                                                                var PlaceholderArea = "div" + id;
                                                                for (var k = 0; k < Details.length; k++) {
                                                                    if (Details[k].PlaceholderArea == PlaceholderArea) {
                                                                        $(this).attr('data-name', Details[k].PlaceholderName);
                                                                        $(this).attr('id', Details[k].PlaceholderUniqueName);
                                                                        $(this).addClass('one-layout-detail');
                                                                        $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                                                                        $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                                                                        $(this).attr('data-def', Details[k].XIGUID);
                                                                        Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                                                                    }
                                                                }
                                                            }
                                                        });

                                                        //Dialog Resize Code

                                                        var _uidialog = $('.dialog-' + oDialogD.ID).find('.' + DialogDivID);
                                                        fncRenderlayoutcontent(oLayoutD, _uidialog).then(function (status) {
                                                            fncApplyScroll(_uidialog, oDialogD);
                                                        });
                                                        //oDialogD.ID;
                                                        var sTaskBarLabel = oDialogD.DialogName.split(' ')[0];
                                                        var Btns = $('#NavigationBar').find('.btnTabs');
                                                        if (Btns.length > 0) {
                                                            var bGrpFound = false;
                                                            $('#NavigationBar').find('.btnTabs').find('.dialogNavBtn').each(function () {
                                                                var TaskBtn = $(this).find('button');
                                                                if (TaskBtn && TaskBtn.length > 0) {
                                                                    var TBtn = TaskBtn[0].attributes["id"].textContent;
                                                                    if (TBtn == TaskBarID) {
                                                                        bGrpFound = true;
                                                                    }
                                                                }
                                                            });
                                                            if (!bGrpFound) {
                                                                var VBar = '<div class="dialogNavBtn"><button type="button" data-navtype="group" class="btn btn-theme btn-xs" id="' + TaskBarID + '" onclick="return fncTaskBarNavigate(&quot;' + TaskBarID + '&quot;)" ><i class="fa fa-group icn" ></i></button><span class="hoverText" >' + sTaskBarLabel + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + GrpDlg + '&quot;, this)">&times;</div></span></div>';
                                                                $('#NavigationBar').find('.btnTabs').append(VBar);
                                                            }
                                                        }
                                                        else {
                                                            var VBar = '<div class="btnTabs left"><div class="dialogNavBtn"><button type="button" data-navtype="group" class="btn btn-theme btn-xs" id="' + TaskBarID + '" onclick="return fncTaskBarNavigate(&quot;' + TaskBarID + '&quot;)" ><i class="fa fa-group icn" ></i></button><span class="hoverText" >' + sTaskBarLabel + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + GrpDlg + '&quot;, this)">&times;</div></span></div></div>';
                                                            $('#NavigationBar').append(VBar);
                                                        }
                                                        $('div[data-identity="' + TaskBarID + '"]').show();
                                                    }
                                                }
                                            }
                                            else {
                                                if (oDialogD.oContent.hasOwnProperty("layout")) {
                                                    var sDlgTitle = ""; var sDlgDisplayTitle = "";
                                                    if (oDialogD.sLabel && oDialogD.sLabel.length > 0) {
                                                        sDlgDisplayTitle = oDialogD.sLabel;
                                                    }
                                                    else if (sBOLabel && iBOIID) {
                                                      //sDlgDisplayTitle = sBOLabel + "-" + iBOIID;
                                                        //sDlgTitle = sBOLabel + " " + iBOIID + iXILinkID;
                                                        sDlgDisplayTitle = sBOLabel + "-" + sRowName;
                                                        sDlgTitle = sBOLabel + " " + sRowName + iXILinkID;
                                                    }
                                                    else {
                                                        if (MenuName) {
                                                            sDlgDisplayTitle = MenuName;
                                                            sDlgTitle = MenuName;
                                                        }
                                                        else {
                                                            sDlgDisplayTitle = oDialogD.DialogName;
                                                            sDlgTitle = oDialogD.DialogName;
                                                        }
                                                    }
                                                    if (oDialogD.sLabel && oDialogD.sLabel.length > 0) {
                                                        TaskBarID = "TaskBarBtn-" + oDialogD.sLabel.replace(/ /g, '');
                                                    }
                                                    else if (sClkType && sClkType.length > 0 && sClkType == "Menu") {
                                                        TaskBarID = "Menu-" + MenuName.replace(/ /g, '') + "-" + iXILinkID;
                                                    }
                                                    else {
                                                        if (sDlgTitle.length > 0) {
                                                            TaskBarID = "TaskBarBtn-" + sDlgTitle.replace(/ /g, '');
                                                        }
                                                        else {
                                                            TaskBarID = "TaskBarBtn-" + oDialogD.ID;
                                                        }
                                                    }
                                                    var oLayoutD = oDialogD.oContent["layout"];
                                                    var bIsTaskBar = oLayoutD.bIsTaskBar;
                                                    var bParentTaskBar = false; //oLayoutD.bAddToParentTaskbar;
                                                    if (oLayoutD.bAddToParentTaskbar === undefined) {
                                                        bParentTaskBar = false;
                                                    }
                                                    else {
                                                        bParentTaskBar = oLayoutD.bAddToParentTaskbar;
                                                    }
                                                    if (bIsTaskBar) {
                                                        var TBarPosition = oLayoutD.sTaskBarPosition;
                                                        if (!TBarPosition || TBarPosition.length == 0) {
                                                            TBarPosition = "left";
                                                        }
                                                        oLayoutD.LayoutCode = '<div class="NavbarWrapper ' + TBarPosition + '"><div id="NavigationBar-' + oLayoutD.ID + '"></div>' + oLayoutD.LayoutCode + '</div>'
                                                    }
                                                    var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oLayoutD.sCssClass != null ? (oLayoutD.sCssClass.length > 0 ? (" " + oLayoutD.sCssClass) : '') : '');
                                                    var LayoutHTML = "";
                                                    if (oLayoutD.bIsFluid) {
                                                        LayoutHTML = '<div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div>';
                                                    } else {
                                                        LayoutHTML = '<div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div>';
                                                    }
                                                    var DialogDivID = "ResultDialog-" + oDialogD.ID + "_" + oLayoutD.sGUID;
                                                    var DlgClasses = "one-dialog one-dialog-id-" + oDialogD.ID + " one-dialog-" + oDialogD.DialogName.toLowerCase().replace(/ /g, '_');
                                                    var windowclose = '<i class="windowClose fa fa-times" data-bIsLock=' + bIsLock + ' onclick="fncdialogclose(this, ' + bParentTaskBar + ', &quot;' + DialogDivID + '&quot;,&quot;' + sBOName + '&quot;, &quot;' + iBOIID + '&quot;)"></i>';
                                                    //$('.' + DialogDivID).dialog('destroy').remove();
                                                    var Div = "";
                                                    if (oLayoutD.bIsFluid) {
                                                        Div = '<div class="' + XILinkClasses + '"><div class="dialog-box fluid-row ' + DialogDivID + ' ' + DlgClasses + '" title="Confirm Message" data-def="' + oDialogD.XIGUID + '"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a></div></div>';
                                                    } else {
                                                        Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"><div class="' + XILinkClasses + '" data-def="' + XIData.XILinkGUID + '"><div class="' + DlgClasses + '" data-def="' + oDialogD.XIGUID + '">' + LayoutHTML + '</div></div></div>';
                                                    }
                                                    $('#Dialogs').append(Div);
                                                    var dlgBg = $('#dialog-background');
                                                    if (dlgBg && dlgBg.length == 0) {
                                                        $('body').append('<div id="dialog-background"></div>');
                                                    }
                                                    //$("." + DialogDivID).html(LayoutHTML);
                                                    var bTaskbarExists = $('#wrapper').find('.NavbarWrapper');
                                                    //var draggableLeft = -screen.width + 400;
                                                    if (bTaskbarExists && bTaskbarExists.length > 0) {
                                                        oDialogD.DialogWidth = oDialogD.DialogWidth - 50;
                                                        //draggableLeft = 65;
                                                    }
                                                    var InPopup = '<i class="openinpopup fa fa-external-link-alt" onclick="fncOpenInPopup(&quot;' + sBOName + '&quot;, ' + iBOIID + ', ' + oLayoutD.ID + ', &quot;' + oLayoutD.sGUID + '&quot;, &quot;' + DialogDivID + '&quot;)"></i>';
                                                    var RefreshPopup = '<i class="refreshpopup fa fa-sync" id="RefreshPopup-' + oLayoutD.ID + '" onclick="fncRefreshPopup(&quot;' + DialogDivID + '&quot;, &quot;' + oLayoutD.sGUID + '&quot;, &quot;' + oLayoutD.XIGUID + '&quot;)"></i>';
                                                    $("." + DialogDivID).dialog({
                                                        title: ' ',
                                                        //modal: true,
                                                        height: oDialogD.DialogHeight,
                                                        width: oDialogD.DialogWidth,
                                                        resizable: oDialogD.IsResizable,
                                                        IsCloseIcon: oDialogD.IsCloseIcon,
                                                        BarPosition: oDialogD.BarPosition,
                                                        //dialogClass: 'titlebardata ' + DialogOpacityValue,
                                                        //dialogClass: 'titlebardata actualScreenToggle admin_v2_1 ' + DialogOpacityValue,
                                                        dialogClass: 'titlebardata sideSpaceDialog actualScreenToggle one-custom-dialog ' + DialogOpacityValue,
                                                        buttons: [],
                                                        open: function () {
                                                            $(this).parent().promise().done(function () {
                                                                var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                                                                $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                                                                $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + resetPosition + RefreshPopup + InPopup + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                                                                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                                                                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                                                                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();
                                                                $(this).find('.ui-dialog-title').html('<span class="fc-head">' + sDlgDisplayTitle + '</span>');
                                                                if ($(this).hasClass("sideSpaceDialog")) {
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 395,
                                                                        top: 70,
                                                                        left: 275,
                                                                    }, 0);
                                                                    //$(window).resize(function(){
                                                                    $(".dialog-box").animate({ height: screen.height });
                                                                    //}).trigger('resize');

                                                                }                                                                    
                                                                if ($('body').hasClass("inbox_kpi_show")) {
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 685,
                                                                        top: 70,
                                                                        left: 275
                                                                    }, 0);
                                                                }else{
                                                                    $(this).animate({
                                                                        height: $(window).height() - 80,
                                                                        width: $(window).width() - 395,
                                                                        top: 70,
                                                                        left: 275
                                                                    }, 0);
                                                                };
                                                                //<span class="fc-green">Success message !!!</span>
                                                                uidialog = $(this);
                                                                var dlgStyle = uidialog[0].attributes["style"].textContent;
                                                                if (dlgStyle && dlgStyle.length > 0) {
                                                                    var Styles = dlgStyle.split(";");
                                                                    for (var k = 0; k < Styles.length; k++) {
                                                                        var Sty = Styles[k].trim();
                                                                        var st = Sty.split(":");
                                                                        if (st[0].trim() == "width") {
                                                                            dlgWidth = st[1].trim();
                                                                        }
                                                                        else if (st[0].trim() == "height") {
                                                                            dlgHeight = oDialogD.DialogHeight + "px";
                                                                        }
                                                                        else if (st[0].trim() == "top") {
                                                                            dlgTop = st[1].trim();
                                                                        }
                                                                        else if (st[0].trim() == "left") {
                                                                            dlgLeft = st[1].trim();
                                                                        }
                                                                    }
                                                                    $(this).attr('data-dlgWidth', dlgWidth);
                                                                    $(this).attr('data-dlgHeight', dlgHeight);
                                                                    $(this).attr('data-dlgTop', dlgTop);
                                                                    $(this).attr('data-dlgLeft', dlgLeft);
                                                                    $(this).attr('data-identity', TaskBarID);
                                                                }
                                                            });
                                                            //$('body').css({ 'overflow': 'hidden' });                                                            
                                                        },
                                                        close: function (event, ui) {
                                                            var oQSInstance = QSInsDict[sGUID1];
                                                            if ((oQSInstance != null || oQSInstance != undefined) && oQSInstance.sCurrentStepName != "Quotes") {
                                                                fncNavigateStep(oQSInstance.HistoryXIGUID[oQSInstance.HistoryXIGUID.length - 1], sGUID1, oQSInstance.XIGUID);
                                                            }
                                                            //$('#Dialogs').parent().hide();
                                                            $("." + DialogDivID).parent().hide();//.dialog('close');
                                                            //$("." + DialogDivID).dialog('destroy').remove();
                                                            //$('body').css({ 'overflow': 'auto' });
                                                        }
                                                    }).dialog("widget")
                                                        .draggable({
                                                            //containment: [left, top, right, bottom]
                                                            containment: [-screen.width + 600, 0, screen.width - 200, screen.height - 200],
                                                            start: function () {
                                                                $(this).data("startingScrollTop", $(this).parent().scrollTop());
                                                                $(this).parent().scrollTop();
                                                            },
                                                            drag: function (event, ui) {
                                                                var st = parseInt($(this).data("startingScrollTop"));
                                                                ui.position.top -= $(this).parent().scrollTop() - st;
                                                            }
                                                        }).css({ position: "fixed" })
                                                        .dblclick(function () {
                                                            if ($(this).hasClass("actualScreenToggle")) {
                                                                $(this).removeClass("actualScreenToggle");
                                                                $(this).addClass("fullScreenToggle");
                                                                var maxIcon = $(this).find('i.Maximize');
                                                                fncdialogchange(maxIcon, "maximize");
                                                            }
                                                            else {
                                                                $(this).removeClass("fullScreenToggle");
                                                                $(this).addClass("actualScreenToggle");
                                                                var restoreIcon = $(this).find('i.RestoreDown');
                                                                fncdialogchange(restoreIcon, "restore");
                                                            }
                                                        })
                                                    $("." + DialogDivID).dialog("widget").resizable({ handles: "n, e, s, w" });                                                    
                                                    //if (bTaskbarExists && bTaskbarExists.length > 0) {
                                                    //    $("." + DialogDivID).parent().css({
                                                    //        position: 'absolute',
                                                    //        left: 65,
                                                    //        top: 39.5
                                                    //    });
                                                    //}
                                                    //Dialog Resize Code
                                                    $(window).resize(function () {
                                                        $('.dialog-box #PolicyMoreDetails').each(function () { $(this).closest('.dialog-box').height($(window).height() - 150) });
                                                    });
                                                    $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function () {

                                                        setTimeout(function () {
                                                            var PopupTabArea = $(".nav-tabs-custom").height();
                                                            $('.' + DialogDivID).find(".PopupTabContentArea_").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - PopupTabArea - 57, });
                                                            $('.' + DialogDivID).find(".PopupTabContentArea_0").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - 57, });
                                                            $('.' + DialogDivID).find(".OneClickResDiv").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - 57, });
                                                        }, 3000);
                                                        $("." + DialogDivID).dialog().dialog("widget").resizable({
                                                            resize: function (event, ui) {
                                                                var dHeight = 0;
                                                                var dlgH = 0;
                                                                var dbox = $("." + DialogDivID).parent();
                                                                var dlgStyle = dbox[0].attributes["style"].textContent;
                                                                if (dlgStyle && dlgStyle.length > 0) {
                                                                    var Styles = dlgStyle.split(";");
                                                                    for (var k = 0; k < Styles.length; k++) {
                                                                        var Sty = Styles[k].trim();
                                                                        var st = Sty.split(":");
                                                                        if (st[0].trim() == "height") {
                                                                            dHeight = st[1].trim();
                                                                        }
                                                                    }
                                                                    dlgH = dHeight.slice(0, -2);
                                                                }
                                                                var PopupTabArea = $(".nav-tabs-custom").height();
                                                                $('.' + DialogDivID).find(".PopupTabContentArea_").height(dlgH - PopupTabArea - 47);
                                                                //$('.' + DialogDivID).find("[data-name='Tab Content Area'],[data-name='Main Content']").height(dlgH - 83);
                                                                $('.' + DialogDivID).find(".PopupTabContentArea_0").height(dlgH - 47);
                                                                $('.' + DialogDivID).find('#MenuContent').find(".control-sidebar").height(dlgH - 36);
                                                                $('.' + DialogDivID).find("#TreeScrollbar").height(dlgH - 83);
                                                                //$(this).find(".dialog-box").height(dlgH);
                                                                //$('.' + DialogDivID).find(".scroll_vh_100").slimscroll({height: dlgH - 57,});
                                                                $('.' + DialogDivID).find(".PopupTabContentArea_").find(".scroll_vh_100").slimscroll({ height: dlgH - PopupTabArea - 57 });
                                                                $('.' + DialogDivID).find(".PopupTabContentArea_0").find(".scroll_vh_100").slimscroll({ height: dlgH - 57 });
                                                                $('.' + DialogDivID).find(".OneClickResDiv").find(".scroll_vh_100").slimscroll({ height: dlgH - 57 });

                                                                $('.' + DialogDivID).parent('.sideSpaceDialog').find(".PopupTabContentArea_0").height(dlgH - 5);
                                                                $('.' + DialogDivID).parent('.sideSpaceDialog').find(".PopupTabContentArea_").height(dlgH - PopupTabArea - 5);
                                                                $('.' + DialogDivID).parent('.sideSpaceDialog').find('#MenuContent').find(".control-sidebar").height(dlgH - 5);
                                                            }
                                                            //alsoResize: "div[aria-labelledby=ui-id-5] .PopupTabContentArea_, div[aria-labelledby=ui-id-5] .PopupTabContentArea_0, div[aria-labelledby=ui-id-5] #TreeScrollbar, div[aria-labelledby=ui-id-5] #BOForm, div[aria-labelledby=ui-id-5] #OneClickData"
                                                        });
                                                    });

                                                    $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                                                        var Details = oLayoutD.LayoutDetails;
                                                        var id = $(this).attr('id');
                                                        if (id && id.length > 0) {
                                                            var PlaceholderArea = "div" + id;
                                                            for (var k = 0; k < Details.length; k++) {
                                                                if (Details[k].PlaceholderArea == PlaceholderArea) {
                                                                    $(this).attr('data-name', Details[k].PlaceholderName);
                                                                    $(this).attr('id', Details[k].PlaceholderUniqueName);
                                                                    $(this).addClass('one-layout-detail');
                                                                    $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                                                                    $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                                                                    $(this).attr('data-def', Details[k].XIGUID);
                                                                    Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            if (this.className == "row" && oLayoutD.bIsFluid) {
                                                                this.className = "fluid-row";
                                                            }
                                                        }
                                                    });
                                                    var _layout = $('.LayoutCode_' + oLayoutD.sGUID);
                                                    fncRenderlayoutcontent(oLayoutD, _layout).then(function (status) {
                                                        fncTriggerXILink(oLayoutD.sGUID, _layout);
                                                        fncApplyScroll(_layout, oDialogD);
                                                    });
                                                    var BtnID = TaskBarID; //oDialogD.ID;
                                                    var sTaskBarLabel = oDialogD.DialogName.split(' ')[0];
                                                    if (bParentTaskBar) {
                                                        var Btns = $('#NavigationBar').find('.btnTabs');
                                                        if (sDlgTitle && sDlgTitle.length > 0) {

                                                        }
                                                        else {
                                                            sDlgTitle = sTaskBarLabel;
                                                        }
                                                        if (Btns.length > 0) {
                                                            var VBar = '<div class="dialogNavBtn" id ="' + oLayoutD.sGUID + '"><button type="button" data-navtype="single" class="btn btn-theme btn-xs taskbtn" id="' + BtnID + '" onclick="return fncTaskBarNavigate(&quot;' + BtnID + '&quot;)" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + sDlgDisplayTitle + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + DialogDivID + '&quot;, this)">&times;</div></span></div>';
                                                            $('#NavigationBar').find('.btnTabs').append(VBar);
                                                            $('#NavigationBar').addClass("showLeftBtn");
                                                        }
                                                        else {
                                                            var MinMax = '<div class="dialogNavBtn"><button type="button" data-navtype="single" class="btn btn-theme btn-xs" onclick="return fncCloseAllDialogs()" ><i class="fa fa-home"></i></button><span class="hoverText" >Minimise<div class="closeNavBtn">&times;</div></span></div>'
                                                                + '<div class="dialogNavBtn"><button type="button" data-navtype="single" class="btn btn-theme btn-xs" onclick="return fncOpenAllDialogs()" ><i class="far fa-window-maximize"></i></button><span class="hoverText" >Maximise<div class="closeNavBtn">&times;</div></span></div>'
                                                                + '<div class="dialogNavBtn"><button type="button" data-navtype="single" class="btn btn-theme btn-xs" ><i class="fas fa-th-large"></i></button><span class="hoverText" ><a href="#" class="nav-dlg-btn fas fa-square dlgsplit" data-type="2split"></a><a href="#" class="nav-dlg-btn fas fa-th-large dlgsplit"></a><a href="#" class="nav-dlg-btn fas fa-th dlgsplit"></a><a href="#" class="nav-dlg-btn">4</a><div class="closeNavBtn">&times;</div></span></div>';
                                                            var VBar = '<div class="btnTabs left">' + MinMax + '<div class="dialogNavBtn" id ="' + oLayoutD.sGUID + '"><button type="button" data-navtype="single" class="btn btn-theme btn-xs taskbtn" id="' + BtnID + '" onclick="return fncTaskBarNavigate(&quot;' + BtnID + '&quot;)" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + sDlgDisplayTitle + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + DialogDivID + '&quot;, this)">&times;</div></span></div></div>';
                                                            $('#NavigationBar').append(VBar);
                                                            $('#NavigationBar').parent().addClass("showLeftBtn");
                                                        }
                                                    }
                                                    //$('.closeNavBtn').on('click', function () {
                                                    //    //$("#TaskBarBtn-" + oDialogD.ID + ' .dialogNavBtn').remove();
                                                    //    $(this).parent().parent('.dialogNavBtn').remove();
                                                    //})
                                                }
                                            }
                                        }
                                        else if (oXiLink.oContent.hasOwnProperty("layout")) {
                                            //To do render layout from xilink
                                            var oLayoutD = oXiLink.oContent["layout"];
                                            var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oLayoutD.sCssClass != null ? (oLayoutD.sCssClass.length > 0 ? " " + oLayoutD.sCssClass : '') : '');
                                            var LayoutHTML = "";
                                            if (oLayoutD.bIsFluid) {
                                                LayoutHTML = '<div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div>';
                                            }
                                            else {
                                                LayoutHTML = '<div class="' + XILinkClasses + '"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
                                            }


                                            $('#' + oXiLink.sOutput).html(LayoutHTML);
                                            $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                                                var Details = oLayoutD.LayoutDetails;
                                                var id = $(this).attr('id');
                                                if (id && id.length > 0) {
                                                    var PlaceholderArea = "div" + id;
                                                    for (var k = 0; k < Details.length; k++) {
                                                        if (Details[k].PlaceholderArea == PlaceholderArea) {
                                                            $(this).attr('data-name', Details[k].PlaceholderName);
                                                            $(this).attr('id', Details[k].PlaceholderUniqueName);
                                                            $(this).addClass('one-layout-detail');
                                                            $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                                                            $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                                                            $(this).attr('data-def', Details[k].XIGUID);
                                                            Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                                                        }
                                                    }
                                                }
                                                else {
                                                    if (this.className == "row" && oLayoutD.bIsFluid) {
                                                        this.className = "fluid-row";
                                                    }
                                                }
                                            });
                                            var _uidialog = $('.LayoutCode_' + oLayoutD.sGUID);
                                            fncRenderlayoutcontent(oLayoutD, _uidialog);
                                        }
                                        else if (oXiLink.oContent.hasOwnProperty("xicomponent")) {
                                            var xiComponent = oXiLink.oContent["xicomponent"];
                                            if (oXiLink.sOutput && oXiLink.sOutput.length > 0 && oXiLink.sOutput != "Inline") {
                                                if (sOutputArea != '') {
                                                    fncRenderComponent(xiComponent, sOutputArea);
                                                } else {
                                                    fncRenderComponent(xiComponent, oXiLink.sOutput);
                                                }
                                            }
                                            else if (oXiLink.sOutput == "Inline" && xiComponent.oContent.hasOwnProperty("Form Component")) {
                                                sContentCode = xiComponent.oContent["Form Component"].Content;
                                                $(".simple1clickInline").html("");
                                                var sContentHTML = '<div class="simple1clickInline">' + sContentCode + '</div>';
                                                $(".simpleclick_" + iBOIID).after(sContentHTML);
                                            }

                                            else {
                                                var sGUID = CreateGuid();
                                                var sContentCode = "";
                                                if (xiComponent.oContent.hasOwnProperty("OneClickComponent")) {
                                                    sContentCode = xiComponent.oContent["OneClickComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("Form Component")) {
                                                    sContentCode = xiComponent.oContent["Form Component"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("Grid Component")) {
                                                    sContentCode = xiComponent.oContent["Grid Component"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("HTML Component")) {
                                                    sContentCode = xiComponent.oContent["HTML Component"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("InboxComponent")) {
                                                    sContentCode = xiComponent.oContent["InboxComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("ReportComponent")) {
                                                    sContentCode = xiComponent.oContent["ReportComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("GroupComponent")) {
                                                    sContentCode = xiComponent.oContent["GroupComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("MenuComponent")) {
                                                    sContentCode = xiComponent.oContent["MenuComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("MappingComponent")) {
                                                    sContentCode = xiComponent.oContent["MappingComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("MultiRowComponent")) {
                                                    sContentCode = xiComponent.oContent["MultiRowComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("QuoteReportDataComponent")) {
                                                    sContentCode = xiComponent.oContent["QuoteReportDataComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("DashBoardChartComponent")) {
                                                    sContentCode = xiComponent.oContent["DashBoardChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("PieChartComponent")) {
                                                    sContentCode = xiComponent.oContent["PieChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("CombinationChartComponent")) {
                                                    sContentCode = xiComponent.oContent["CombinationChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4HeatChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4HeatChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4LineChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4LineChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4BarChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4BarChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4ColumnBarChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4ColumnBarChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4PriceChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4PriceChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4PieChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4PieChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4GaugeChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4GaugeChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("AM4SemiPieChartComponent")) {
                                                    sContentCode = xiComponent.oContent["AM4SemiPieChartComponent"].Content;
                                                }
                                                else if (xiComponent.oContent.hasOwnProperty("ReportDataComponent")) {
                                                    sContentCode = xiComponent.oContent["ReportDataComponent"].Content;
                                                }

                                                var DialogDivID = "ResultDialog-" + sGUID;
                                                var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h" title="" onclick="fncdialogchange(this, &quot;maxwidth&quot;)"></i>';
                                                var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v" onclick="fncdialogchange(this, &quot;maxheight&quot;)"></i>';
                                                var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minwidth&quot;)"></i>';
                                                var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minheight&quot;)"></i>';
                                                var MinDia = '<i class="Minimize fa fa-window-minimize" onclick="fncdialogchange(this, &quot;minimize&quot;)"></i>';
                                                var MaxDia = '<i class="Maximize far fa-window-maximize" onclick="fncdialogchange(this, &quot;maximize&quot;)"></i>';
                                                var RestoreDia = '<i class="RestoreDown far fa-window-restore" onclick="fncdialogchange(this, &quot;restore&quot;)"></i>';
                                                var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
                                                var InPopup = '<i class="openinpopup fa fa-external-link-alt" onclick="fncOpenInPopup(&quot;' + "" + '&quot;, ' + 0 + ', ' + 0 + ', &quot;' + sGUID + '&quot;)"></i>';
                                                //var RefreshPopup = '<i class="refreshpopup fa fa-sync" onclick="fncRefreshPopup(&quot;' + DialogDivID + '&quot;, &quot;' + sGUID + '&quot;, ' + 0 + ' )"></i>';
                                                var sContentHTML = '<div class="LayoutCode_' + sGUID + ' sys-layout" data-guid="' + sGUID + '" data-name="LayoutGUID">' + sContentCode + '</div>';

                                                var Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a></div>';
                                                $('#Dialogs').append(Div);
                                                $("." + DialogDivID).html(sContentHTML);
                                                $("." + DialogDivID).dialog({
                                                    title: ' ',
                                                    appendTo: "body",
                                                    height: screen.height - 190,
                                                    width: screen.width - 50,
                                                    resizable: true,
                                                    IsCloseIcon: true,
                                                    dialogClass: DialogOpacityValue,
                                                    buttons: [
                                                    ],
                                                    open: function () {
                                                        $(this).parent().promise().done(function () {
                                                            var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                                                            $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                                                            //$(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + RefreshPopup + InPopup + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                                                            $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + InPopup + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                                                            $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                                                            $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                                                            $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();

                                                            if ($(this).hasClass("sideSpaceDialog")) {
                                                                $(this).animate({
                                                                    height: $(window).height() - 80,
                                                                    width: $(window).width() - 395,
                                                                    top: 70,
                                                                    left: 275,
                                                                }, 0);
                                                                //$(window).resize(function(){
                                                                $(".dialog-box").animate({ height: screen.height });
                                                                //}).trigger('resize');

                                                            }                                                                    
                                                            if ($('body').hasClass("inbox_kpi_show")) {
                                                                $(this).animate({
                                                                    height: $(window).height() - 80,
                                                                    width: $(window).width() - 685,
                                                                    top: 70,
                                                                    left: 275
                                                                }, 0);
                                                            }else{
                                                                $(this).animate({
                                                                    height: $(window).height() - 80,
                                                                    width: $(window).width() - 395,
                                                                    top: 70,
                                                                    left: 275
                                                                }, 0);
                                                            };

                                                            uidialog = $(this);
                                                            var dlgStyle = uidialog[0].attributes["style"].textContent;
                                                            if (dlgStyle && dlgStyle.length > 0) {
                                                                var Styles = dlgStyle.split(";");
                                                                for (var k = 0; k < Styles.length; k++) {
                                                                    var Sty = Styles[k].trim();
                                                                    var st = Sty.split(":");
                                                                    if (st[0].trim() == "width") {
                                                                        dlgWidth = st[1].trim();
                                                                    }
                                                                    else if (st[0].trim() == "height") {
                                                                        dlgHeight = screen.height + "px";
                                                                    }
                                                                    else if (st[0].trim() == "top") {
                                                                        dlgTop = st[1].trim();
                                                                    }
                                                                    else if (st[0].trim() == "left") {
                                                                        dlgLeft = st[1].trim();
                                                                    }
                                                                }
                                                                $(this).attr('data-dlgWidth', dlgWidth);
                                                                $(this).attr('data-dlgHeight', dlgHeight);
                                                                $(this).attr('data-dlgTop', dlgTop);
                                                                $(this).attr('data-dlgLeft', dlgLeft);
                                                            }
                                                        });
                                                    },
                                                    close: function (event, ui) {
                                                        $("." + DialogDivID).parent().hide();
                                                    }
                                                }).dialog("widget")
                                                    .draggable({
                                                        containment: [-screen.width + 600, 0, screen.width - 200, screen.height - 200],
                                                        start: function () {
                                                            $(this).data("startingScrollTop", $(this).parent().scrollTop());
                                                            $(this).parent().scrollTop();
                                                        },
                                                        drag: function (event, ui) {
                                                            var st = parseInt($(this).data("startingScrollTop"));
                                                            ui.position.top -= $(this).parent().scrollTop() - st;
                                                        }
                                                    }).css({ position: "fixed" })
                                                    .dblclick(function () {
                                                        if ($(this).hasClass("actualScreenToggle")) {
                                                            $(this).removeClass("actualScreenToggle");
                                                            $(this).addClass("fullScreenToggle");
                                                            var maxIcon = $(this).find('i.Maximize');
                                                            fncdialogchange(maxIcon, "maximize");
                                                        }
                                                        else {
                                                            $(this).removeClass("fullScreenToggle");
                                                            $(this).addClass("actualScreenToggle");
                                                            var restoreIcon = $(this).find('i.RestoreDown');
                                                            fncdialogchange(restoreIcon, "restore");
                                                        }
                                                    })
                                            }
                                        }
                                        $('body').find('.loader').remove();
                                        $('.page-wrapper').removeClass("blr");
                                        $('.page-footer').removeClass("blr");
                                        $('#srchView embed').height($('#srchView').height() - 5);
                                        $('embed').height($('.ui-dialog .dialog-box').innerHeight() - 90);
                                    },
                                    error: function (data) {
                                        //alert(data);
                                    }
                                });
                            }
                        }
                    }
                });
            }
        });
    }
}

function fncLoadLayout(iLayoutID, oParams, sGUID, sDivID, sType) {

debugger
    if (sDivID && sDivID.length > 0) {
        if (sGUID && sGUID.length > 0) {
            $('.' + sDivID).find('.LayoutCode_' + sGUID).append('<div class="loader"></div>');
            $('body').css({'cursor':'progress'});
        }
        else
            $('.' + sDivID).append('<div class="loader"></div>');
            $('body').css({'cursor':'progress'});
    }
    else {
        $('.Layout_' + iLayoutID).append('<div class="loader"></div>');
        $('body').css({'cursor':'progress'});
    }
debugger
    $.ajax({
        type: 'POST',
        url: LoadLayoutURL,
        data: JSON.stringify({ iLayoutID: iLayoutID, oParams: oParams, sGUID: sGUID, sType: sType }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        async: true,
        success: function (oLayoutD) {
            if (sGUID && sGUID.length > 0 && sType != 'Nanno') {
                oLayoutD.sGUID = sGUID;
            }
            var bIsTaskBar = oLayoutD.bIsTaskBar;
            if (bIsTaskBar) {
                var TBarPosition = oLayoutD.sTaskBarPosition;
                if (!TBarPosition || TBarPosition.length == 0) {
                    TBarPosition = "left";
                }
                if (oLayoutD.bIsFluid) {
                    oLayoutD.LayoutCode = '<div class="NavbarWrapper fluid-row ' + TBarPosition + '"><div id="NavigationBar"></div>' + oLayoutD.LayoutCode + '</div>'
                }
                else {
                    oLayoutD.LayoutCode = '<div class="NavbarWrapper ' + TBarPosition + '"><div id="NavigationBar"></div>' + oLayoutD.LayoutCode + '</div>'
                }

            }
            var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oLayoutD.sCssClass != null ? (oLayoutD.sCssClass.length > 0 ? " " + oLayoutD.sCssClass : '') : '');
            var LayoutHTML = "";
            if (oLayoutD.bIsFluid) {
                LayoutHTML = '<div class="inline-layout fluid-row" role="inline-layout"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout fluid-row" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
            }
            else {
                LayoutHTML = '<div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout inline-layout" role="inline-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div>';
            }
            if (sDivID && sDivID.length > 0) {
                if (sGUID && sGUID.length > 0) {
                    $('.' + sDivID).find('.LayoutCode_' + sGUID).replaceWith(LayoutHTML);
                }
                else
                    $('.' + sDivID).html(LayoutHTML);
            }
            else {
                if (oLayoutD.bIsFluid) {
                    $('.Layout_' + iLayoutID).addClass('fluid-row');
                }
                $('.Layout_' + iLayoutID).html(LayoutHTML);
            }

            $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                var Details = oLayoutD.LayoutDetails;
                var id = $(this).attr('id');
                if (id && id.length > 0) {
                    var PlaceholderArea = "div" + id;
                    for (var k = 0; k < Details.length; k++) {
                        if (Details[k].PlaceholderArea == PlaceholderArea) {
                            $(this).attr('data-name', Details[k].PlaceholderName);
                            $(this).attr('id', Details[k].PlaceholderUniqueName);
                            $(this).addClass('one-layout-detail');
                            $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                            $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                            $(this).attr('data-def', Details[k].XIGUID);
                            Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                        }
                    }
                }
                else {
                    if (this.className == "row" && oLayoutD.bIsFluid) {
                        this.className = "fluid-row";
                    }
                }
            });
            var _uidialog = $('.LayoutCode_' + oLayoutD.sGUID);
            fncRenderlayoutcontent(oLayoutD, _uidialog).then(function (status) {
                fncTriggerXILink(oLayoutD.sGUID, _uidialog);
                if (sType && sType == "Refresh") {
                    var parentuidialog = fncgetDialogFromHTMLTree('dialog', _uidialog[0]);
                    fncApplyScroll(parentuidialog, null);
                }
                else {
                    if (!oLayoutD.bIsFluid)
                        fncApplyInlineScroll(_uidialog);
                }
                $('.Layout_' + iLayoutID).find('.loader').remove();
                $('body').css({'cursor':'auto'});                
            });
        },
        error: function (data) {
debugger
        }
    });
}

function fncRenderlayoutcontent(oLayoutD, _uidialog) {
    return new Promise(function (resolve, reject) {
        for (var i = 0; i < oLayoutD.LayoutMappings.length; i++) {
            var PlaceHolderID = oLayoutD.LayoutMappings[i].PlaceHolderIDXIGUID;
            var oLayContent = oLayoutD.LayoutMappings[i].oContent;
            if (oLayContent.hasOwnProperty("xicomponent")) {
                var oInsBase = oLayContent["xicomponent"];
                if (oInsBase && oInsBase != null) {
                    if (oInsBase.oContent.hasOwnProperty("OneClickComponent")) {
                        var CompData = oInsBase.oContent["OneClickComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Form Component")) {
                        var CompData = oInsBase.oContent["Form Component"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XITreeStructure")) {
                        var CompData = oInsBase.oContent["XITreeStructure"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Tab Component")) {
                        var CompData = oInsBase.oContent["Tab Component"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MenuComponent")) {
                        var CompData = oInsBase.oContent["MenuComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QuoteReportDataComponent")) {
                        var CompData = oInsBase.oContent["QuoteReportDataComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DashBoardChartComponent")) {
                        var CompData = oInsBase.oContent["DashBoardChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("CombinationChartComponent")) {
                        var CompData = oInsBase.oContent["CombinationChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("PieChartComponent")) {
                        var CompData = oInsBase.oContent["PieChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4HeatChartComponent")) {
                        var CompData = oInsBase.oContent["AM4HeatChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4LineChartComponent")) {
                        var CompData = oInsBase.oContent["AM4LineChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4PriceChartComponent")) {
                        var CompData = oInsBase.oContent["AM4PriceChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4PieChartComponent")) {
                        var CompData = oInsBase.oContent["AM4PieChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }

                    else if (oInsBase.oContent.hasOwnProperty("AM4GaugeChartComponent")) {
                        var CompData = oInsBase.oContent["AM4GaugeChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4SemiPieChartComponent")) {
                        var CompData = oInsBase.oContent["AM4SemiPieChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4BarChartComponent")) {
                        var CompData = oInsBase.oContent["AM4BarChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4ColumnBarChartComponent")) {
                        var CompData = oInsBase.oContent["AM4ColumnBarChartComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ReportDataComponent")) {
                        var CompData = oInsBase.oContent["ReportDataComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Grid Component")) {
                        var CompData = oInsBase.oContent["Grid Component"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("HTML Component")) {
                        var CompData = oInsBase.oContent["HTML Component"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("InboxComponent")) {
                        var CompData = oInsBase.oContent["InboxComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XilinkComponent")) {
                        var CompData = oInsBase.oContent["XilinkComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIApplicationComponent")) {
                        var CompData = oInsBase.oContent["XIApplicationComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutComponent")) {
                        var CompData = oInsBase.oContent["LayoutComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MenuNodeComponent")) {
                        var CompData = oInsBase.oContent["MenuNodeComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("GroupComponent")) {
                        var CompData = oInsBase.oContent["GroupComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ScriptComponent")) {
                        var CompData = oInsBase.oContent["ScriptComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutMappingComponent")) {
                        var CompData = oInsBase.oContent["LayoutMappingComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutDetailsComponent")) {
                        var CompData = oInsBase.oContent["LayoutDetailsComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ReportComponent")) {
                        var CompData = oInsBase.oContent["ReportComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DialogComponent")) {
                        var CompData = oInsBase.oContent["DialogComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("FieldOriginComponent")) {
                        var CompData = oInsBase.oContent["FieldOriginComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIParameterComponent")) {
                        var CompData = oInsBase.oContent["XIParameterComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DataTypeComponent")) {
                        var CompData = oInsBase.oContent["DataTypeComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIComponentComponent")) {
                        var CompData = oInsBase.oContent["XIComponentComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOComponent")) {
                        var CompData = oInsBase.oContent["XIBOComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOAttributeComponent")) {
                        var CompData = oInsBase.oContent["XIBOAttributeComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOScriptComponent")) {
                        var CompData = oInsBase.oContent["XIBOScriptComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOStructureComponent")) {
                        var CompData = oInsBase.oContent["XIBOStructureComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIDataSourceComponent")) {
                        var CompData = oInsBase.oContent["XIDataSourceComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QueryManagementComponent")) {
                        var CompData = oInsBase.oContent["QueryManagementComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSConfigComponent")) {
                        var CompData = oInsBase.oContent["QSConfigComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSStepConfigComponent")) {
                        var CompData = oInsBase.oContent["QSStepConfigComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSSectionConfigComponent")) {
                        var CompData = oInsBase.oContent["QSSectionConfigComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("VisualisationComponent")) {
                        var CompData = oInsBase.oContent["VisualisationComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIUrlMappingComponent")) {
                        var CompData = oInsBase.oContent["XIUrlMappingComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSLinkComponent")) {
                        var CompData = oInsBase.oContent["QSLinkComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSLinkDefinationComponent")) {
                        var CompData = oInsBase.oContent["QSLinkDefinationComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MappingComponent")) {
                        var CompData = oInsBase.oContent["MappingComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIInfraXIBOUIComponent")) {
                        var CompData = oInsBase.oContent["XIInfraXIBOUIComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MultiRowComponent")) {
                        var CompData = oInsBase.oContent["MultiRowComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("FeedComponent")) {
                        var CompData = oInsBase.oContent["FeedComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DocumentTreeComponent")) {
                        var CompData = oInsBase.oContent["DocumentTreeComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("UserCreationComponent")) {
                        var CompData = oInsBase.oContent["UserCreationComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("KPICircleComponent")) {
                        var CompData = oInsBase.oContent["KPICircleComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIValuesComponent")) {
                        var CompData = oInsBase.oContent["XIValuesComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DynamicTreeComponent")) {
                        var CompData = oInsBase.oContent["DynamicTreeComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("SmartSearchComponent")) {
                        var CompData = oInsBase.oContent["SmartSearchComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    } else if (oInsBase.oContent.hasOwnProperty("CodeEditorComponent")) {
                        var CompData = oInsBase.oContent["CodeEditorComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
		            else if (oInsBase.oContent.hasOwnProperty("AccordionComponent")) {
                        var CompData = oInsBase.oContent["AccordionComponent"];
                        for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                            if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                            }
                        }
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSComponent")) {
                        var oQSI = oInsBase.oContent["QSComponent"];
                        if (oQSI.oContent.hasOwnProperty("fields")) {
                            var CompData = oQSI.oContent["fields"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(CompData.Content);
                                }
                            }
                        }
                        else if (Object.keys(oQSI.Steps).length > 0) {
                            var oStepI = Object.values(oQSI.Steps)[0];
                            if (oStepI.oContent.hasOwnProperty("layout")) {
                                var oInrLayoutD = oStepI.oContent["layout"];
                                var LayClasses = "one-layout one-layout-id-" + oInrLayoutD.ID + " one-layout-" + oInrLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oInrLayoutD.sCssClass != null ? (oInrLayoutD.sCssClass.length > 0 ? " " + oInrLayoutD.sCssClass : '') : '');
                                var LayoutHTML = "";
                                if (oInrLayoutD.bIsFluid) {
                                    LayoutHTML = '<div id="QSID" class="fluid-row one-qs one-qs-id-' + oQSI.FKiQSDefinitionID + ' one-qs-' + oQSI.sQSName + (oQSI.sCssClass != null ? (oQSI.sCssClass.length > 0 ? " " + oQSI.sCssClass : '') : '') + '" data-value="' + oQSI.FKiQSDefinitionIDXIGUID + '" data-info="QuestionSet" data-def="' + oQSI.FKiQSDefinitionIDXIGUID + '"><div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + oStepI.ID + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oInrLayoutD.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oInrLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oInrLayoutD.XIGUID + '">' + oInrLayoutD.LayoutCode + '</div></div></div>';
                                }
                                else {
                                    LayoutHTML = '<div id="QSID" class="one-qs one-qs-id-' + oQSI.FKiQSDefinitionID + ' one-qs-' + oQSI.sQSName + (oQSI.sCssClass != null ? (oQSI.sCssClass.length > 0 ? " " + oQSI.sCssClass : '') : '') + '" data-value="' + oQSI.FKiQSDefinitionIDXIGUID + '" data-info="QuestionSet" data-def="' + oQSI.FKiQSDefinitionIDXIGUID + '"><div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + oStepI.ID + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oInrLayoutD.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oInrLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oInrLayoutD.XIGUID + '">' + oInrLayoutD.LayoutCode + '</div></div></div>';
                                }
                                for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                    if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                        $(_uidialog).find('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(LayoutHTML);
                                    }
                                }
                                $('.LayoutCode_' + oInrLayoutD.sGUID + ' div').each(function (j, obj) {
                                    var Details = oInrLayoutD.LayoutDetails;
                                    var id = $(this).attr('id');
                                    if (id && id.length > 0) {
                                        var PlaceholderArea = "div" + id;
                                        for (var k = 0; k < Details.length; k++) {
                                            if (Details[k].PlaceholderArea == PlaceholderArea) {
                                                $(this).attr('data-name', Details[k].PlaceholderName);
                                                $(this).attr('id', Details[k].PlaceholderUniqueName);
                                                $(this).addClass('one-layout-detail');
                                                $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                                                $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                                                $(this).attr('data-def', Details[k].XIGUID);
                                                Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                                            }
                                        }
                                    }
                                    else {
                                        if (this.className == "row" && oLayoutD.bIsFluid) {
                                            this.className = "fluid-row";
                                        }
                                    }
                                });
                                fncRenderlayoutcontent(oInrLayoutD, _uidialog);
                            }
                            else {
                                //var oStepI = oLayContent["step"];
                                var PlaceholderUniqueName = "";
                                for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                    if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                        PlaceholderUniqueName = oLayoutD.LayoutDetails[j].PlaceholderUniqueName;
                                    }
                                }
                                fncRenderStepContent(oStepI, '#' + PlaceholderUniqueName, null, null, _uidialog, oLayoutD.bIsFluid)
                            }
                        }
                    }
                }
            }
            else if (oLayContent.hasOwnProperty("xilink")) {
                var oXiLink = oLayContent["xilink"];
                if (oXiLink && oXiLink != null) {
                    if (oXiLink.oContent.hasOwnProperty("xicomponent")) {
                        var oXILinkcont = oXiLink.oContent["xicomponent"];
                        if (oXILinkcont.oContent.hasOwnProperty("OneClickComponent")) {
                            var finalData = oXILinkcont.oContent["OneClickComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("Form Component")) {
                            var finalData = oXILinkcont.oContent["Form Component"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("Grid Component")) {
                            var finalData = oXILinkcont.oContent["Grid Component"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("MenuComponent")) {
                            var finalData = oXILinkcont.oContent["MenuComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QuoteReportDataComponent")) {
                            var finalData = oXILinkcont.oContent["QuoteReportDataComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("DashBoardChartComponent")) {
                            var finalData = oXILinkcont.oContent["DashBoardChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("PieChartComponent")) {
                            var finalData = oXILinkcont.oContent["PieChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("CombinationChartComponent")) {
                            var finalData = oXILinkcont.oContent["CombinationChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4HeatChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4HeatChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4LineChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4LineChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4PriceChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4PriceChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4PieChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4PieChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4GaugeChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4GaugeChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4SemiPieChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4SemiPieChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("AM4BarChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4BarChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        } else if (oXILinkcont.oContent.hasOwnProperty("AM4ColumnBarChartComponent")) {
                            var finalData = oXILinkcont.oContent["AM4ColumnBarChartComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("ReportDataComponent")) {
                            var finalData = oXILinkcont.oContent["ReportDataComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XilinkComponent")) {
                            var finalData = oXILinkcont.oContent["XilinkComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("DialogComponent")) {
                            var finalData = oXILinkcont.oContent["DialogComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("FieldOriginComponent")) {
                            var finalData = oXILinkcont.oContent["FieldOriginComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIParameterComponent")) {
                            var finalData = oXILinkcont.oContent["XIParameterComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("DataTypeComponent")) {
                            var finalData = oXILinkcont.oContent["DataTypeComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIComponentComponent")) {
                            var finalData = oXILinkcont.oContent["XIComponentComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIBOComponent")) {
                            var finalData = oXILinkcont.oContent["XIBOComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIBOAttributeComponent")) {
                            var finalData = oXILinkcont.oContent["XIBOAttributeComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIBOScriptComponent")) {
                            var finalData = oXILinkcont.oContent["XIBOScriptComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIBOStructureComponent")) {
                            var finalData = oXILinkcont.oContent["XIBOStructureComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIDataSourceComponent")) {
                            var finalData = oXILinkcont.oContent["XIDataSourceComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QueryManagementComponent")) {
                            var finalData = oXILinkcont.oContent["QueryManagementComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QSConfigComponent")) {
                            var finalData = oXILinkcont.oContent["QSConfigComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QSStepConfigComponent")) {
                            var finalData = oXILinkcont.oContent["QSStepConfigComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QSSectionConfigComponent")) {
                            var finalData = oXILinkcont.oContent["QSSectionConfigComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("VisualisationComponent")) {
                            var finalData = oXILinkcont.oContent["VisualisationComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIUrlMappingComponent")) {
                            var finalData = oXILinkcont.oContent["XIUrlMappingComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("ReportComponent")) {
                            var finalData = oXILinkcont.oContent["ReportComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QSLinkComponent")) {
                            var finalData = oXILinkcont.oContent["QSLinkComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("QSLinkDefinationComponent")) {
                            var finalData = oXILinkcont.oContent["QSLinkDefinationComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }

                        else if (oXILinkcont.oContent.hasOwnProperty("MappingComponent")) {
                            var finalData = oXILinkcont.oContent["MappingComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("MultiRowComponent")) {
                            var finalData = oXILinkcont.oContent["MultiRowComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("FeedComponent")) {
                            var finalData = oXILinkcont.oContent["FeedComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("KPICircleComponent")) {
                            var finalData = oXILinkcont.oContent["KPICircleComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("XIValuesComponent")) {
                            var finalData = oXILinkcont.oContent["XIValuesComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                        else if (oXILinkcont.oContent.hasOwnProperty("DynamicTreeComponent")) {
                            var finalData = oXILinkcont.oContent["DynamicTreeComponent"];
                            for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                                if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                                    $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(finalData.Content);
                                }
                            }
                        }
                    }
                    else if (oXiLink.oContent.hasOwnProperty("layout")) {
                        var oLay = oXiLink.oContent["layout"];
                        var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oLayoutD.sCssClass != null ? (oLayoutD.sCssClass.length > 0 ? " " + oLayoutD.sCssClass : '') : '');
                        var LayoutHTML = "";
                        if (oLayoutD.bIsFluid) {
                            LayoutHTML = '<div class="LayoutCode_' + oLay.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oLay.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLay.LayoutCode + '</div>';
                        }
                        else {
                            LayoutHTML = '<div class="LayoutCode_' + oLay.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oLay.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLay.LayoutCode + '</div>';
                        }
                        $('#' + oLayoutD.LayoutDetails[i].PlaceholderUniqueName).html(LayoutHTML);
                        $('.LayoutCode_' + oLay.sGUID + ' div').each(function (j, obj) {
                            var Details = oLay.LayoutDetails;
                            var id = $(this).attr('id');
                            if (id && id.length > 0) {
                                var PlaceholderArea = "div" + id;
                                for (var k = 0; k < Details.length; k++) {
                                    if (Details[k].PlaceholderArea == PlaceholderArea) {
                                        $(this).attr('data-name', Details[k].PlaceholderName);
                                        $(this).attr('id', Details[k].PlaceholderUniqueName);
                                        $(this).addClass('one-layout-detail');
                                        $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                                        $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                                        $(this).attr('data-def', Details[k].XIGUID);
                                        Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                                    }
                                }
                            }
                            else {
                                if (this.className == "row" && oLayoutD.bIsFluid) {
                                    this.className = "fluid-row";
                                }
                            }
                        });
                        fncRenderlayoutcontent(oLay, _uidialog);
                    }
                }
            }
            else if (oLayContent.hasOwnProperty("step")) {
                var oStepI = oLayContent["step"];
                var PlaceholderUniqueName = "";
                for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                    if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                        PlaceholderUniqueName = oLayoutD.LayoutDetails[j].PlaceholderUniqueName;
                    }
                }
                fncRenderStepContent(oStepI, '#' + PlaceholderUniqueName, null, null, _uidialog, oLayoutD.bIsFluid)
            }
            else if (oLayContent.hasOwnProperty("html")) {
                var HTMLContent = oLayContent["html"];
                var PlaceholderUniqueName = "";
                for (var j = 0; j < oLayoutD.LayoutDetails.length; j++) {
                    if (oLayoutD.LayoutDetails[j].XIGUID == PlaceHolderID) {
                        $('#' + oLayoutD.LayoutDetails[j].PlaceholderUniqueName).html(HTMLContent);
                    }
                }
            }
        }
        resolve(true);
    });
}

function fncgetDialogFromHTMLTree(parentName, childObj) {
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('role') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else if (testObj.parentNode.tagName == "HTML") {
            return null;
        }
        else {
            return testObj
        }
    }
    return testObj;
}

function fncgetInlineLayoutFromHTMLTree(parentName, childObj) {
    var testObj = childObj.parentNode;
    var count = 1;
    while (testObj.getAttribute('role') != parentName) {
        if (testObj.parentNode.tagName != "HTML") {
            testObj = testObj.parentNode;
            count++;
        }
        else if (testObj.parentNode.tagName == "HTML") {
            return null;
        }
        else {
            return testObj
        }
    }
    return testObj;
}
function fncApplyScroll(_uidialog, oDialogD) {
    //var TabHeight = $(_uidialog).find('#TabsArea').height();
    var headerTab = $('.nav-tabs-custom').height();
    if (oDialogD) {
        var TabHeight = $(_uidialog).find('#TabsArea').find('.scroll_tabs_container').height();
        var PopupTabArea = $(".nav-tabs-custom").height();
        var AlertHeight = $(_uidialog).find('#PopupAlert').height();
        if (!AlertHeight) {
            AlertHeight = 0;
        }
        $(_uidialog).find("#MenuContent").find('.control-sidebar').height(oDialogD.DialogHeight - 37);
        //$(_uidialog).find("[data-name='Tab Content Area'],[data-name='Main Content']").height(oDialogD.DialogHeight - 73);        
        $(_uidialog).find(".PopupTabContentArea_").height(oDialogD.DialogHeight - PopupTabArea - 47);
        $(_uidialog).find(".PopupTabContentArea_0").height(oDialogD.DialogHeight - 47);
        $(_uidialog).find("#TreeScrollbar").height(oDialogD.DialogHeight - 73);
        $(_uidialog).find("#GridComponent").height(oDialogD.DialogHeight - 97);
        $(_uidialog).find("#FeedData").height(oDialogD.DialogHeight - 50);

        //$(_uidialog).parent().parents('.sideSpaceDialog').find(".PopupTabContentArea_0 , .right-nav-btns").height(oDialogD.DialogHeight + 30);
        //$(_uidialog).parent().parents('.sideSpaceDialog').find(".PopupTabContentArea_").height(oDialogD.DialogHeight + 30);
        $(_uidialog).parent().parent('.sideSpaceDialog').find(".PopupTabContentArea_0").height(oDialogD.DialogHeight + 55);
        $(_uidialog).parent().parent('.sideSpaceDialog').find(".PopupTabContentArea_").height(oDialogD.DialogHeight - PopupTabArea + 55);
        $(_uidialog).parent().parent('.sideSpaceDialog').find('#MenuContent').find(".control-sidebar").height(oDialogD.DialogHeight + 55);

        $(_uidialog).closest('.admin_v2_1').find("#MenuContent").find('.control-sidebar').height(oDialogD.DialogHeight - 100);
        //$(_uidialog).closest('.admin_v2_1').find(".PopupTabContentArea_0").height(oDialogD.DialogHeight - 100 - AlertHeight);
        $(_uidialog).closest('.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").height(oDialogD.DialogHeight - 100);
        //$(_uidialog).closest('.admin_v2_1').find(".GridComponent").height(oDialogD.DialogHeight - 200)*0.1;

        //$(_uidialog).find('.PopupTabContentArea_0').find(".LeadContent-table").height(oDialogD.DialogHeight - 44 - AlertHeight);        
        //$(_uidialog).find('.PopupTabContentArea_').find(".LeadContent-table").height(oDialogD.DialogHeight - TabHeight - 40);        

        /*const content_height = $(_uidialog).height() - 44;
        $('.PopupTabContentArea_0').slimScroll({
            height: content_height,
        });*/
        //$(_uidialog).find(".scroll_vh_100").slimscroll({destroy:true});
        //$(_uidialog).find('.scroll_vh_100').attr('style', '');

        //function resizedw(){
        $(_uidialog).find(".PopupTabContentArea_").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - PopupTabArea - 57, });
        $(_uidialog).find(".PopupTabContentArea_0").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - 57, });
        $(_uidialog).find(".OneClickResDiv").find(".scroll_vh_100").slimscroll({ height: oDialogD.DialogHeight - 57, });
        //}

        /*var doit;
        $(window).resize(function () {
            clearTimeout(doit);
            doit = setTimeout(resizedw, 500);
        });*/
        //const content_height_tabview = $(_uidialog).height() - $(_uidialog).find(".nav-tabs").height() - 40; 
        //$("[data-name='Tab Content Area'],[data-name='Main Content']").slimScroll({
        //    height: content_height_tabview,
        //});
        if ($('#NavigationBar').length > 0) {
            $('body').find('.dialog-box').addClass('withNavBtn');
        } else {
            $('body').find('.dialog-box').removeClass('withNavBtn');
        }
    }
    else {
        var dHeight = "";
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
            }
        }
        if (dHeight && dHeight.length > 0) {
            var dlgH = dHeight.slice(0, -2);
            $(_uidialog).find("#MenuContent").find('.control-sidebar').height(dlgH - 15);
            $(_uidialog).find(".PopupTabContentArea_0").height(dlgH - 25);
            //$(_uidialog).find(".PopupTabContentArea_").height(dlgH - 62);
            $(_uidialog).find("#TreeScrollbar").height(dlgH - 35);
            $(_uidialog).find("#GridComponent").height(dlgH - 62);
            //$(".dialog-box .scroll_vh_100").slimscroll({destroy:true});
            //$('.dialog-box .scroll_vh_100').attr('style', '');
            /*const content_height_tabview = $(_uidialog).height() - $(_uidialog).find(".nav-tabs").height() - 40; 
            $("[data-name='Tab Content Area'],[data-name='Main Content']").slimScroll({
                height: content_height_tabview,
            });*/
        }
    }
}

function fncApplyInlineScroll(_uidialog) {
    $(_uidialog).find(".PopupTabContentArea_0").height(screen.height - 180);
    $(_uidialog).find(".PopupTabContentArea_").height(screen.height - 220);
    /*$(_uidialog).find("#TreeScrollbar").height(screen.height - 210);*/
    $(_uidialog).find(".OneClickResDiv").height(screen.height - 210);
    $(_uidialog).find("#MenuContent").find('.control-sidebar').height(screen.height - 170);
    $(_uidialog).find("#GridComponent").height(screen.height - 220);
    var PopupTabArea = $(".nav-tabs-custom").height();
}

function fncRenderStepContent(oStepI, PlaceholderUniqueName, sName, sType, _uidialog, bIsFluid) {
    return new Promise(function (resolve, reject) {
        var sIdentity = "";
        if ((sName && sName.length > 0)) {
            sIdentity = "QSStep_" + sName;
        }
        else {
            sIdentity = "QSStep_0";
        }
        var sContentHTML = "";
        if (Object.keys(oStepI.Sections).length > 0) {
            var oSecI = Object.values(oStepI.Sections)[0];
            if (oSecI == null) {
                if (bIsFluid) {
                    sContentHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"></div>';
                }
                else {
                    sContentHTML = '<div class="conStep one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"></div>';
                }

            }
            else {
                var oSecD = oSecI.oDefintion;
                if (oSecI.oContent.hasOwnProperty("xicomponent")) {
                    if (bIsFluid) {
                        sContentHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="conSection fluid-row one-qs-section one-qs-section-id-' + oSecI.FKiStepSectionDefinitionID + (oSecI.sCssClass != null ? (oSecI.sCssClass.length > 0 ? " " + oSecI.sCssClass : '') : '') + '" data-info="QSSection" data-value="' + oSecI.FKiStepSectionDefinitionIDXIGUID + '" data-def="' + oSecI.FKiStepSectionDefinitionIDXIGUID + '">';
                    }
                    else {
                        sContentHTML = '<div class="conStep one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="conSection one-qs-section one-qs-section-id-' + oSecI.FKiStepSectionDefinitionID + (oSecI.sCssClass != null ? (oSecI.sCssClass.length > 0 ? " " + oSecI.sCssClass : '') : '') + '" data-info="QSSection" data-value="' + oSecI.FKiStepSectionDefinitionIDXIGUID + '" data-def="' + oSecI.FKiStepSectionDefinitionIDXIGUID + '">';
                    }

                    var oInsBase = oSecI.oContent["xicomponent"];
                    sGUID = oInsBase.sGUID;
                    if (oInsBase.oContent.hasOwnProperty("OneClickComponent")) {
                        var CompData = oInsBase.oContent["OneClickComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Form Component")) {
                        var CompData = oInsBase.oContent["Form Component"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XITreeStructure")) {
                        var CompData = oInsBase.oContent["XITreeStructure"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Tab Component")) {
                        var CompData = oInsBase.oContent["Tab Component"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MenuComponent")) {
                        var CompData = oInsBase.oContent["MenuComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QuoteReportDataComponent")) {
                        var CompData = oInsBase.oContent["QuoteReportDataComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("Grid Component")) {
                        var CompData = oInsBase.oContent["Grid Component"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("HTML Component")) {
                        var CompData = oInsBase.oContent["HTML Component"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("InboxComponent")) {
                        var CompData = oInsBase.oContent["InboxComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XilinkComponent")) {
                        var CompData = oInsBase.oContent["XilinkComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("GroupComponent")) {
                        var CompData = oInsBase.oContent["GroupComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DialogComponent")) {
                        var CompData = oInsBase.oContent["DialogComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("FieldOriginComponent")) {
                        var CompData = oInsBase.oContent["FieldOriginComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIParameterComponent")) {
                        var CompData = oInsBase.oContent["XIParameterComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DataTypeComponent")) {
                        var CompData = oInsBase.oContent["DataTypeComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIComponentComponent")) {
                        var CompData = oInsBase.oContent["XIComponentComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOComponent")) {
                        var CompData = oInsBase.oContent["XIBOComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOAttributeComponent")) {
                        var CompData = oInsBase.oContent["XIBOAttributeComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOScriptComponent")) {
                        var CompData = oInsBase.oContent["XIBOScriptComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIBOStructureComponent")) {
                        var CompData = oInsBase.oContent["XIBOStructureComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIDataSourceComponent")) {
                        var CompData = oInsBase.oContent["XIDataSourceComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QueryManagementComponent")) {
                        var CompData = oInsBase.oContent["QueryManagementComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSConfigComponent")) {
                        var CompData = oInsBase.oContent["QSConfigComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSStepConfigComponent")) {
                        var CompData = oInsBase.oContent["QSStepConfigComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSSectionConfigComponent")) {
                        var CompData = oInsBase.oContent["QSSectionConfigComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("VisualisationComponent")) {
                        var CompData = oInsBase.oContent["VisualisationComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIApplicationComponent")) {
                        var CompData = oInsBase.oContent["XIApplicationComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutComponent")) {
                        var CompData = oInsBase.oContent["LayoutComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MenuNodeComponent")) {
                        var CompData = oInsBase.oContent["MenuNodeComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("GroupComponent")) {
                        var CompData = oInsBase.oContent["GroupComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ScriptComponent")) {
                        var CompData = oInsBase.oContent["ScriptComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutMappingComponent")) {
                        var CompData = oInsBase.oContent["LayoutMappingComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("LayoutDetailsComponent")) {
                        var CompData = oInsBase.oContent["LayoutDetailsComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ReportComponent")) {
                        var CompData = oInsBase.oContent["ReportComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIUrlMappingComponent")) {
                        var CompData = oInsBase.oContent["XIUrlMappingComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSLinkComponent")) {
                        var CompData = oInsBase.oContent["QSLinkComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("QSLinkDefinationComponent")) {
                        var CompData = oInsBase.oContent["QSLinkDefinationComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MappingComponent")) {
                        var CompData = oInsBase.oContent["MappingComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("MultiRowComponent")) {
                        var CompData = oInsBase.oContent["MultiRowComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DashBoardChartComponent")) {
                        var CompData = oInsBase.oContent["DashBoardChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("PieChartComponent")) {
                        var CompData = oInsBase.oContent["PieChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("CombinationChartComponent")) {
                        var CompData = oInsBase.oContent["CombinationChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4HeatChartComponent")) {
                        var CompData = oInsBase.oContent["AM4HeatChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4LineChartComponent")) {
                        var CompData = oInsBase.oContent["AM4LineChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4PriceChartComponent")) {
                        var CompData = oInsBase.oContent["AM4PriceChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4PieChartComponent")) {
                        var CompData = oInsBase.oContent["AM4PieChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4GaugeChartComponent")) {
                        var CompData = oInsBase.oContent["AM4GaugeChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4SemiPieChartComponent")) {
                        var CompData = oInsBase.oContent["AM4SemiPieChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4BarChartComponent")) {
                        var CompData = oInsBase.oContent["AM4BarChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AM4ColumnBarChartComponent")) {
                        var CompData = oInsBase.oContent["AM4ColumnBarChartComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("ReportDataComponent")) {
                        var CompData = oInsBase.oContent["ReportDataComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("FeedComponent")) {
                        var CompData = oInsBase.oContent["FeedComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIInfraXIBOUIComponent")) {
                        var CompData = oInsBase.oContent["XIInfraXIBOUIComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("KPICircleComponent")) {
                        var CompData = oInsBase.oContent["KPICircleComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("XIValuesComponent")) {
                        var CompData = oInsBase.oContent["XIValuesComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("DynamicTreeComponent")) {
                        var CompData = oInsBase.oContent["DynamicTreeComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
			else if (oInsBase.oContent.hasOwnProperty("CodeEditorComponent")) {
                        var CompData = oInsBase.oContent["CodeEditorComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    else if (oInsBase.oContent.hasOwnProperty("AccordionComponent")) {
                        var CompData = oInsBase.oContent["AccordionComponent"];
                        sContentHTML = sContentHTML + CompData.Content;
                    }
                    var oBttns = "";
                    if (Object.keys(oSecD.QSLinks).length > 0) {
                        var oQSL = Object.values(oSecD.QSLinks);
                        for (var n = 0; n < oQSL.length; n++) {
                            if (Object.keys(oQSL[n].XiLink).length > 0) {
                                var XiLinks = Object.values(oQSL[n].XiLink);
                                for (var b = 0; b < XiLinks.length; b++) {
                                    if (XiLinks[b].sType.toLowerCase() == "button") {
                                        if (XiLinks[b].sRunType && XiLinks[b].sRunType.toLowerCase() == "load") {
                                            oBttns = oBttns + '<button type="button" class="btn btn-theme" onclick="fncXILinkLoad(' + XiLinks[b].FKiXILInkID + ', this)">' + XiLinks[b].sName + '</button>';
                                        }
                                        else {
                                            oBttns = oBttns + '<button type="button" class="btn btn-theme" onclick="XIRun(null,' + XiLinks[b].FKiXILInkID + ', 0, &quot;' + oSecI.sGUID + '&quot;, null, false, 0, 0)">' + XiLinks[b].sName + '</button>';
                                        }
                                    }
                                    else if (XiLinks[b].sType.toLowerCase() == "trigger") {
                                        $.ajax({
                                            type: 'POST',
                                            url: AddTriggerXILinkURL,
                                            data: { iXILinkID: XiLinks[b].FKiXILInkID, sGUID: oSecI.sGUID },
                                            cache: false,
                                            async: false,
                                            dataType: 'json',
                                            success: function (oResponse) {
                                                var Data = oResponse.Data;
                                            }
                                        });
                                        //fncAddTriggerXILink(XiLinks[b].FKiXILInkID, oSecI.sGUID);
                                        //TriggerXILink.push({ Name: "XILinkID", Value: XiLinks[b].FKiXILInkID, Context: oSecI.sGUID });
                                    }
                                }
                            }
                        }
                    }
                    if (oBttns && oBttns.length > 0) {
                        oBttns = '<div>' + oBttns + '</div>';
                        sContentHTML = sContentHTML + oBttns + '</div></div>'
                    }
                    else {
                        sContentHTML = sContentHTML + '</div></div>'
                    }
                }
            }
            if ((sName && sName.length > 0)) {
                $(_uidialog).find(PlaceholderUniqueName).find('.conStep').each(function (j, obj) {
                    $(this).hide();
                });
                if (sType && sType.length > 0 && sType == "trigger") {
                    $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
                }
                else {
                    $(_uidialog).find(PlaceholderUniqueName).append(sContentHTML);
                }
            }
            else {
                if (sType && sType.length > 0 && sType == "trigger") {
                    $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
                }
                else if (sType && sType.length > 0 && sType == "trigger_template") {
                    $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
                }
                else {
                    $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
                }
            }
        }
        else if (oStepI.oContent.hasOwnProperty("layout")) {
            var oLayoutD = oStepI.oContent["layout"];
            var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_') + (oLayoutD.sCssClass != null ? (oLayoutD.sCssClass.length > 0 ? " " + oLayoutD.sCssClass : '') : '');
            var LayoutHTML = "";
            if (oLayoutD.bIsFluid) {
                LayoutHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
            }
            else {
                LayoutHTML = '<div class="conStep one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
            }
            if ((sName && sName.length > 0)) {
                $(_uidialog).find(PlaceholderUniqueName).find('.conStep').each(function (j, obj) {
                    $(this).hide();
                });
                $(_uidialog).find(PlaceholderUniqueName).append(LayoutHTML);
            }
            else {
                $(_uidialog).find(PlaceholderUniqueName).html(LayoutHTML);
            }
            //$('#' + PlaceholderUniqueName).html(LayoutHTML);
            $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                var Details = oLayoutD.LayoutDetails;
                var id = $(this).attr('id');
                if (id && id.length > 0) {
                    var PlaceholderArea = "div" + id;
                    for (var k = 0; k < Details.length; k++) {
                        if (Details[k].PlaceholderArea == PlaceholderArea) {
                            $(this).attr('data-name', Details[k].PlaceholderName);
                            $(this).attr('id', Details[k].PlaceholderUniqueName);
                            $(this).addClass('one-layout-detail');
                            $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                            $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                            $(this).attr('data-def', Details[k].XIGUID);
                            Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                        }
                    }
                }
                else {
                    if (this.className == "row" && oLayoutD.bIsFluid) {
                        this.className = "fluid-row";
                    }
                }
            });
            fncRenderlayoutcontent(oLayoutD, _uidialog).then(function (status) {
                //fncApplyScroll(uidialog, oDialogD, oLayoutD.sGUID);
            });
        }
        resolve(true);
    });
}

function fncRenderStepContentV2(oStepI, PlaceholderUniqueName, sName, sType, _uidialog, bIsFluid) {
    return new Promise(function (resolve, reject) {
        var sIdentity = "";
        if ((sName && sName.length > 0)) {
            sIdentity = "QSStep_" + sName;
        }
        else {
            sIdentity = "QSStep_0";
        }
        var sContentHTML = "";
        if (Object.keys(oStepI.Sections).length > 0) {
            var oSecI = Object.values(oStepI.Sections)[0];
            if (oSecI == null) {
                if (bIsFluid) {
                    sContentHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"></div>';
                }
                else {
                    sContentHTML = '<div class="conStep one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"></div>';
                }

            }
            else {
                var oSecD = oSecI.oDefintion;
                if (oSecI.oContent.hasOwnProperty("xicomponent")) {

                    funLoadSecContent(oSecD.ID, oSecI.sGUID, PlaceholderUniqueName, bIsFluid, sIdentity, oStepI.FKiQSStepDefinitionIDXIGUID, oSecI.FKiStepSectionDefinitionIDXIGUID, _uidialog, sName, sType);

                    var oBttns = "";
                    if (Object.keys(oSecD.QSLinks).length > 0) {
                        var oQSL = Object.values(oSecD.QSLinks);
                        for (var n = 0; n < oQSL.length; n++) {
                            if (Object.keys(oQSL[n].XiLink).length > 0) {
                                var XiLinks = Object.values(oQSL[n].XiLink);
                                for (var b = 0; b < XiLinks.length; b++) {
                                    if (XiLinks[b].sType.toLowerCase() == "button") {
                                        if (XiLinks[b].sRunType && XiLinks[b].sRunType.toLowerCase() == "load") {
                                            oBttns = oBttns + '<button type="button" class="btn btn-theme" onclick="fncXILinkLoad(' + XiLinks[b].FKiXILInkID + ', this)">' + XiLinks[b].sName + '</button>';
                                        }
                                        else {
                                            oBttns = oBttns + '<button type="button" class="btn btn-theme" onclick="XIRun(' + XiLinks[b].FKiXILInkID + ', 0, &quot;' + oSecI.sGUID + '&quot;, null, false, 0, 0)">' + XiLinks[b].sName + '</button>';
                                        }
                                    }
                                    else if (XiLinks[b].sType.toLowerCase() == "trigger") {
                                        $.ajax({
                                            type: 'POST',
                                            url: AddTriggerXILinkURL,
                                            data: { iXILinkID: XiLinks[b].FKiXILInkID, sGUID: oSecI.sGUID },
                                            cache: false,
                                            async: false,
                                            dataType: 'json',
                                            success: function (oResponse) {
                                                var Data = oResponse.Data;
                                            }
                                        });
                                        //fncAddTriggerXILink(XiLinks[b].FKiXILInkID, oSecI.sGUID);
                                        //TriggerXILink.push({ Name: "XILinkID", Value: XiLinks[b].FKiXILInkID, Context: oSecI.sGUID });
                                    }
                                }
                            }
                        }
                    }
                    //if (oBttns && oBttns.length > 0) {
                    //    oBttns = '<div>' + oBttns + '</div>';
                    //    sContentHTML = sContentHTML + oBttns + '</div></div>'
                    //}
                    //else {
                    //    sContentHTML = sContentHTML + '</div></div>'
                    //}
                }
            }
            //if ((sName && sName.length > 0)) {
            //    $(_uidialog).find(PlaceholderUniqueName).find('.conStep').each(function (j, obj) {
            //        $(this).hide();
            //    });
            //    if (sType && sType.length > 0 && sType == "trigger") {
            //        $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
            //    }
            //    else {
            //        $(_uidialog).find(PlaceholderUniqueName).append(sContentHTML);
            //    }
            //}
            //else {
            //    if (sType && sType.length > 0 && sType == "trigger") {
            //        $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
            //    }
            //    else if (sType && sType.length > 0 && sType == "trigger_template") {
            //        $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
            //    }
            //    else {
            //        $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
            //    }
            //}
        }
        else if (oStepI.oContent.hasOwnProperty("layout")) {
            var oLayoutD = oStepI.oContent["layout"];
            var LayClasses = "one-layout one-layout-id-" + oLayoutD.ID + " one-layout-" + oLayoutD.LayoutName.toLowerCase().replace(/ /g, '_');
            var LayoutHTML = "";
            if (oLayoutD.bIsFluid) {
                LayoutHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' fluid-row sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
            }
            else {
                LayoutHTML = '<div class="conStep one-qs-step one-qs-step-id-' + oStepI.FKiQSStepDefinitionID + (oStepI.sCssClass != null ? (oStepI.sCssClass.length > 0 ? " " + oStepI.sCssClass : '') : '') + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + oStepI.FKiQSStepDefinitionIDXIGUID + '" data-def="' + oStepI.FKiQSStepDefinitionIDXIGUID + '"><div class="LayoutCode_' + oLayoutD.sGUID + ' ' + LayClasses + ' sys-layout" data-guid="' + oLayoutD.sGUID + '" data-name="LayoutGUID" data-def="' + oLayoutD.XIGUID + '">' + oLayoutD.LayoutCode + '</div></div>';
            }
            if ((sName && sName.length > 0)) {
                $(_uidialog).find(PlaceholderUniqueName).find('.conStep').each(function (j, obj) {
                    $(this).hide();
                });
                $(_uidialog).find(PlaceholderUniqueName).append(LayoutHTML);
            }
            else {
                $(_uidialog).find(PlaceholderUniqueName).html(LayoutHTML);
            }
            //$('#' + PlaceholderUniqueName).html(LayoutHTML);
            $('.LayoutCode_' + oLayoutD.sGUID + ' div').each(function (j, obj) {
                var Details = oLayoutD.LayoutDetails;
                var id = $(this).attr('id');
                if (id && id.length > 0) {
                    var PlaceholderArea = "div" + id;
                    for (var k = 0; k < Details.length; k++) {
                        if (Details[k].PlaceholderArea == PlaceholderArea) {
                            $(this).attr('data-name', Details[k].PlaceholderName);
                            $(this).attr('id', Details[k].PlaceholderUniqueName);
                            $(this).addClass('one-layout-detail');
                            $(this).addClass('one-layout-detail-id-' + Details[k].PlaceHolderID);
                            $(this).addClass('one-layout-detail-' + Details[k].PlaceholderUniqueName.toLowerCase().replace(/ /g, '_'));
                            $(this).attr('data-def', Details[k].XIGUID);
                            Details[k].sCssClass != null ? (Details[k].sCssClass.length > 0 ? $(this).addClass(Details[k].sCssClass) : '') : '';
                        }
                    }
                }
                else {
                    if (this.className == "row" && oLayoutD.bIsFluid) {
                        this.className = "fluid-row";
                    }
                }
            });
            fncRenderlayoutcontent(oLayoutD, _uidialog).then(function (status) {
                //fncApplyScroll(uidialog, oDialogD, oLayoutD.sGUID);
            });
        }
        resolve(true);
    });
}

function funLoadSecContent(iSecID, sGUID, PlaceholderUniqueName, bIsFluid, sIdentity, StepDID, SectionDID, _uidialog, sName, sType) {
    $.ajax({
        url: LoadSectionContentURL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        datatype: "HTML",
        cache: false,
        async: false,
        data: JSON.stringify({ iComponentID: 0, sType: "section", iInstanceID: iSecID, sGUID: sGUID }),
        success: function (data) {
            var length = data.length;
            if (bIsFluid) {
                sContentHTML = '<div class="conStep fluid-row one-qs-step one-qs-step-id-' + StepDID + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + StepDID + '"><div class="conSection fluid-row one-qs-section one-qs-section-id-' + SectionDID + '" data-info="QSSection" data-value="' + SectionDID + '">';
            }
            else {
                sContentHTML = '<div class="conStep one-qs-step one-qs-step-id-' + StepDID + '" data-info="QSStep" data-identity="' + sIdentity + '" data-value="' + StepDID + '"><div class="conSection one-qs-section one-qs-section-id-' + SectionDID + ' " data-info="QSSection" data-value="' + SectionDID + '">';
            }
            sContentHTML = sContentHTML + data;
            sContentHTML = sContentHTML + '</div></div>';
            if ((sName && sName.length > 0)) {
                $(_uidialog).find(PlaceholderUniqueName).find('.conStep').each(function (j, obj) {
                    $(this).hide();
                });
                if (sType && sType.length > 0 && sType == "trigger") {
                    $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
                }
                else {
                    $(_uidialog).find(PlaceholderUniqueName).append(sContentHTML);
                }
            }
            else {
                if (sType && sType.length > 0 && sType == "trigger") {
                    $(_uidialog).find(PlaceholderUniqueName).replaceWith(sContentHTML);
                }
                else if (sType && sType.length > 0 && fncGetMoreDetails == "trigger_template") {
                    $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
                }
                else {
                    $(_uidialog).find(PlaceholderUniqueName).html(sContentHTML);
                }
            }
            //$(_uidialog).find(sPlaceholder).html(sContentHTML);
        },
        error: function (data) {
            //alert(data);
        }
    });
}

function fncRenderComponent(oInsBase, PlaceholderUniqueName) {
    var CompData = "";
    if (oInsBase.oContent.hasOwnProperty("OneClickComponent")) {
        CompData = oInsBase.oContent["OneClickComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("Form Component")) {
        CompData = oInsBase.oContent["Form Component"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XITreeStructure")) {
        CompData = oInsBase.oContent["XITreeStructure"];
    }
    else if (oInsBase.oContent.hasOwnProperty("Tab Component")) {
        CompData = oInsBase.oContent["Tab Component"];
    }
    else if (oInsBase.oContent.hasOwnProperty("MenuComponent")) {
        CompData = oInsBase.oContent["MenuComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QuoteReportDataComponent")) {
        CompData = oInsBase.oContent["QuoteReportDataComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("Grid Component")) {
        CompData = oInsBase.oContent["Grid Component"];
    }
    else if (oInsBase.oContent.hasOwnProperty("HTML Component")) {
        CompData = oInsBase.oContent["HTML Component"];
    }
    else if (oInsBase.oContent.hasOwnProperty("InboxComponent")) {
        CompData = oInsBase.oContent["InboxComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XilinkComponent")) {
        CompData = oInsBase.oContent["XilinkComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIApplicationComponent")) {
        CompData = oInsBase.oContent["XIApplicationComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("LayoutComponent")) {
        CompData = oInsBase.oContent["LayoutComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("MenuNodeComponent")) {
        CompData = oInsBase.oContent["MenuNodeComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("GroupComponent")) {
        CompData = oInsBase.oContent["GroupComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("ScriptComponent")) {
        CompData = oInsBase.oContent["ScriptComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("LayoutMappingComponent")) {
        CompData = oInsBase.oContent["LayoutMappingComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("LayoutDetailsComponent")) {
        CompData = oInsBase.oContent["LayoutDetailsComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("ReportComponent")) {
        CompData = oInsBase.oContent["ReportComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("DialogComponent")) {
        CompData = oInsBase.oContent["DialogComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("MappingComponent")) {
        CompData = oInsBase.oContent["MappingComponent"];
    }

    else if (oInsBase.oContent.hasOwnProperty("MultiRowComponent")) {
        CompData = oInsBase.oContent["MultiRowComponent"];

    }
    else if (oInsBase.oContent.hasOwnProperty("FieldOriginComponent")) {
        CompData = oInsBase.oContent["FieldOriginComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIParameterComponent")) {
        CompData = oInsBase.oContent["XIParameterComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("DataTypeComponent")) {
        CompData = oInsBase.oContent["DataTypeComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIComponentComponent")) {
        CompData = oInsBase.oContent["XIComponentComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIBOComponent")) {
        CompData = oInsBase.oContent["XIBOComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIBOAttributeComponent")) {
        CompData = oInsBase.oContent["XIBOAttributeComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIBOScriptComponent")) {
        CompData = oInsBase.oContent["XIBOScriptComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIBOStructureComponent")) {
        CompData = oInsBase.oContent["XIBOStructureComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIDataSourceComponent")) {
        CompData = oInsBase.oContent["XIDataSourceComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QueryManagementComponent")) {
        CompData = oInsBase.oContent["QueryManagementComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QSConfigComponent")) {
        CompData = oInsBase.oContent["QSConfigComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QSStepConfigComponent")) {
        CompData = oInsBase.oContent["QSStepConfigComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QSSectionConfigComponent")) {
        CompData = oInsBase.oContent["QSSectionConfigComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("VisualisationComponent")) {
        CompData = oInsBase.oContent["VisualisationComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("XIUrlMappingComponent")) {
        CompData = oInsBase.oContent["XIUrlMappingComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QSLinkComponent")) {
        CompData = oInsBase.oContent["QSLinkComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("QSLinkDefinationComponent")) {
        CompData = oInsBase.oContent["QSLinkDefinationComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("PieChartComponent")) {
        CompData = oInsBase.oContent["PieChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("CombinationChartComponent")) {
        CompData = oInsBase.oContent["CombinationChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4HeatChartComponent")) {
        CompData = oInsBase.oContent["AM4HeatChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4LineChartComponent")) {
        CompData = oInsBase.oContent["AM4LineChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4PriceChartComponent")) {
        CompData = oInsBase.oContent["AM4PriceChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4PieChartComponent")) {
        CompData = oInsBase.oContent["AM4PieChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4GaugeChartComponent")) {
        CompData = oInsBase.oContent["AM4GaugeChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4SemiPieChartComponent")) {
        CompData = oInsBase.oContent["AM4SemiPieChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4BarChartComponent")) {
        CompData = oInsBase.oContent["AM4BarChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AM4ColumnBarChartComponent")) {
        CompData = oInsBase.oContent["AM4ColumnBarChartComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("ReportDataComponent")) {
        CompData = oInsBase.oContent["ReportDataComponent"];
    }
else if (oInsBase.oContent.hasOwnProperty("CodeEditorComponent")) {
        CompData = oInsBase.oContent["CodeEditorComponent"];
    }
    else if (oInsBase.oContent.hasOwnProperty("AccordionComponent")) {
        CompData = oInsBase.oContent["AccordionComponent"];
    }
    $('#' + PlaceholderUniqueName).html(CompData.Content);
}

function fncTriggerMenu(sInstanceID, This, id) {
    fncCloseDlg(This);
    $('#MenuContent ul li').find("#" + id).trigger("click");
};

function ViewMore(OutputArea) {
    $("#" + OutputArea).css('display', 'block');

}
function CloseDetails(OutputArea) {
    $("#" + OutputArea).css('display', 'none');
}


function ShowLayoutContent(model, sGUID) {
    //var model = JSON.parse(XIData);
    var Details = model.LayoutDetails;
    $('.LayoutCode_' + sGUID + ' div').each(function (j, obj) {
        var id = $(this).attr('id');
        if (id && id.length > 0) {
            var PlaceholderArea = "div" + id;
            for (var k = 0; k < Details.length; k++) {
                if (Details[k].PlaceholderArea == PlaceholderArea) {
                    $(this).attr('data-name', Details[k].PlaceholderName);
                    //$(this).attr('id', Details[k].PlaceholderUniqueName + "-" + sGUID);
                    $(this).attr('id', Details[k].PlaceholderUniqueName);
                    //$(this).attr('data-placeid', Details[k].PlaceholderUniqueName);
                }
            }
        }
    });
    var _uidialog = $('.LayoutCode_' + sGUID);
    fncRenderlayoutcontent(model, _uidialog).then(function (status) {
        //fncApplyScroll(uidialog, oDialogD);
    });
}

//XI ContentLoad
function fncloadlayoutcontent(Layout, sGUID) {
    //var Layout = JSON.parse(XIData);
    for (var i = 0; i < Layout.LayoutMappings.length; i++) {
        var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
        var oContent = Layout.LayoutMappings[i].oContent;
        if (oContent.hasOwnProperty("html")) {
            var data = oContent["html"];
            var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
            for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                    //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                    $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(data);
                }
            }
        }
        else if (oContent.hasOwnProperty("xilink")) {
            $.ajax({
                url: XIContentLoadURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "HTML",
                cache: false,
                async: false,
                data: JSON.stringify({ oData: JSON.stringify(oContent["xilink"]).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                success: function (data) {
                    var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
                    for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                        if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                            //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                            $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(data);
                        }
                    }
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        else if (oContent.hasOwnProperty("xicomponent")) {
            $.ajax({
                url: XIContentLoadURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "HTML",
                cache: false,
                async: false,
                data: JSON.stringify({ oData: JSON.stringify(oContent["xicomponent"]).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                success: function (data) {
                    var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
                    for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                        if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                            //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                            $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(data);
                        }
                    }
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        else if (oContent.hasOwnProperty("step")) {
            $.ajax({
                url: XIContentLoadURL,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "HTML",
                cache: false,
                async: false,
                data: JSON.stringify({ oData: JSON.stringify(oContent["step"]).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/") }),
                success: function (data) {
                    var PlaceHolderID = Layout.LayoutMappings[i].PlaceHolderID;
                    for (var j = 0; j < Layout.LayoutDetails.length; j++) {
                        if (Layout.LayoutDetails[j].PlaceHolderID == PlaceHolderID) {
                            //$('#' + Layout.Details[j].PlaceholderUniqueName + "-" + sGUID).append(data);//Removed sGUID
                            $('#' + Layout.LayoutDetails[j].PlaceholderUniqueName).append(data);
                        }
                    }
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
    }
}

function fncToLowerCase(_this) {
    $(_this).val(function (_, val) {
        return val.toLowerCase();
    });
}
function fncToUpperCase(_this) {
    $(_this).val(function (_, val) {
        return val.toUpperCase();
    });
}


function fncLoadPostCode($this, sGUID, sCode, sType) {
    fncToUpperCase($this);
    sPostCode = $($this).val();
    if (sPostCode.length > 0) {
        $.ajax({
            url: "https://services.postcodeanywhere.co.uk/capture/Interactive/Find/v1.00/json3.ws",
            dataType: "json",
            async: true,
            data: {
                key: PCAPredicate_Key,
                countries: "GB",
                text: sPostCode,
                container: ""
            },
            success: function (data) {
                if (sType == "form") {
                    $('#Create_' + sGUID).find('.addresses').show();
                    $('#Create_' + sGUID).find('.addresses').empty();
                    $('#Create_' + sGUID).find('.addresses').append('<span onclick="fncGetAddresses(&quot;' + sPostCode + '&quot;, &quot;' + data.Items[0].Id + '&quot;, &quot;' + sGUID + '&quot;, &quot;' + sCode + '&quot;, &quot;' + sType + '&quot;)">' + data.Items[0].Description + '</span>');
                }
                else {
                    $('.LayoutCode_' + sGUID).find('.addresses').show();
                    $('.LayoutCode_' + sGUID).find('.addresses').empty();
                    $('.LayoutCode_' + sGUID).find('.addresses').append('<span onclick="fncGetAddresses(&quot;' + sPostCode + '&quot;, &quot;' + data.Items[0].Id + '&quot;, &quot;' + sGUID + '&quot;, &quot;' + sCode + '&quot;, &quot;' + sType + '&quot;)">' + data.Items[0].Description + '</span>');
                }
            },
            error: function (data) {
                //alert(data);
            }
        });
    }
    else {
        $('.LayoutCode_' + sGUID).find('.addresses').empty();
    }
}

function fncGetAddresses(PostCode, containerId, sGUID, sCode, sType) {
    $.ajax({
        url: "https://api.addressy.com/Capture/Interactive/Find/v1.00/json3.ws?Key=" + PCAPredicate_Key + "&Text=" + PostCode + "&Container=" + containerId + "&Countries=GB",
        dataType: "json",
        async: true,
        success: function (data) {
            if (sType == "form") {
                $('#Create_' + sGUID).find('.addresses').empty();
                for (var i = 0; i < data.Items.length; i++) {
                    $('#Create_' + sGUID).find('.addresses').append('<span onclick="fncGetAddressByID(&quot;' + data.Items[i].Id + '&quot;, &quot;' + sGUID + '&quot;, &quot;' + sCode + '&quot;, &quot;' + sType + '&quot;)">' + data.Items[i].Text + '</p>');
                }
                $('#Create_' + sGUID).find('.addresssug').attr('data-type', 'third');
            }
            else {
                $('.LayoutCode_' + sGUID).find('.addresses').empty();
                for (var i = 0; i < data.Items.length; i++) {
                    $('.LayoutCode_' + sGUID).find('.addresses').append('<span onclick="fncGetAddressByID(&quot;' + data.Items[i].Id + '&quot;, &quot;' + sGUID + '&quot;, &quot;' + sCode + '&quot;, &quot;' + sType + '&quot;)">' + data.Items[i].Text + '</p>');
                }
                $('.LayoutCode_' + sGUID).find('.addresssug').attr('data-type', 'third');
            }
        },
        error: function (data) {
            //alert(data);
        }
    });
}

function fncGetAddressByID(containerId, sGUID, sCode, sType) {
    $.ajax({
        url: "https://services.postcodeanywhere.co.uk/Capture/Interactive/Retrieve/v1.00/json3.ws",
        dataType: "json",
        async: true,
        data: {
            key: PCAPredicate_Key,
            id: containerId
        },
        success: function (data) {
            var l1;
            var line1 = data.Items[0].Line1;
            var CompanyName = data.Items[0].Company;
            var line2 = data.Items[0].Line2;
            var line3 = data.Items[0].Line3;
            var City = data.Items[0].City;
            var CountryName = data.Items[0].CountryName;
            if (CompanyName === undefined) {

            }
            else if (CompanyName && CompanyName != null && CompanyName.length > 0) {
                l1 = CompanyName + " " + line1;
            }
            else {
                l1 = line1;
            }
            if (line2 == "" || line2 == null) {
                line2 = City;
            }
            if (line3 == "" || line3 == null) {
                line3 = CountryName;
            }
            if (sType == "form") {
                if (sCode.toLowerCase() == 'cpc') { //Corresponding PostCode
                    $('#Create_' + sGUID).find('input[name="AddressLine1SCC"]').val(l1);
                    $('#Create_' + sGUID).find('input[name="AddressLine2SCC"]').val(data.Items[0].Line2);
                    $('#Create_' + sGUID).find('input[name="AddressLine3SCC"]').val(data.Items[0].Line3);
                    $('#Create_' + sGUID).find('input[name="TownSC"]').val(data.Items[0].City);
                } else {
                    $('#Create_' + sGUID).find('input[name="MTAddressline1"]').val(l1);
                    $('#Create_' + sGUID).find('input[name="MTAddressline2"]').val(line2);
                    $('#Create_' + sGUID).find('input[name="MTAddressline3"]').val(line3);
                    $('#Create_' + sGUID).find('input[name="sTown"]').val(City);
                }
                $('#Create_' + sGUID).find('.addresses').empty();
                $('#Create_' + sGUID).find('.addresses').hide();
            }
            else {
                if (sCode.toLowerCase() == 'cpc') { //Corresponding PostCode
                    $('.LayoutCode_' + sGUID).find('input[name="MTPremisesAddressline1"]').val(l1);
                    $('.LayoutCode_' + sGUID).find('input[name="MTPremisesAddressline2"]').val(line2);
                    $('.LayoutCode_' + sGUID).find('input[name="MTPremisesAddressline3"]').val(line3);
                    $('.LayoutCode_' + sGUID).find('input[name="TownSC"]').val(data.Items[0].City);
                } //else {
                //    $('.LayoutCode_' + sGUID).find('input[name="sAddressLine1"]').val(l1);
                //    $('.LayoutCode_' + sGUID).find('input[name="sAddressLine2"]').val(data.Items[0].Line2);
                //    $('.LayoutCode_' + sGUID).find('input[name="sAddressLine3"]').val(data.Items[0].Line3);
                //    $('.LayoutCode_' + sGUID).find('input[name="sTown"]').val(data.Items[0].City);
                //}
                else {
                    $('.LayoutCode_' + sGUID).find('input[name="MTAddressline1"]').val(l1);
                    $('.LayoutCode_' + sGUID).find('input[name="MTAddressline2"]').val(line2);
                    $('.LayoutCode_' + sGUID).find('input[name="MTAddressline3"]').val(line3);
                    $('.LayoutCode_' + sGUID).find('input[name="sTown"]').val(data.Items[0].City);
                }
                $('.LayoutCode_' + sGUID).find('.addresses').empty();
                $('.LayoutCode_' + sGUID).find('.addresses').hide();
            }
        },
        error: function (data) {
            //alert(data);
        }
    });
}

function fncremoveaddress() {
    $('.addresses').empty();
}

function fncLoadScroll(_uidialog, sLayoutType) {
    if (sLayoutType == "Dialog") {
        var bIsMax = _uidialog.attributes['data-maximize'];
        if (bIsMax) {
            bIsMax = _uidialog.attributes['data-maximize'].value;
        }
        else {
            bIsMax = 'false';
        }
        var dlgWidth = _uidialog.attributes['data-dlgWidth'].value;
        var dlgHeight = _uidialog.attributes['data-dlgHeight'].value;
        var dlgTop = _uidialog.attributes['data-dlgTop'].value;
        var dlgLeft = _uidialog.attributes['data-dlgLeft'].value;
        var TabHeight = 0;
        var TabHeight1 = $(_uidialog).find('#TabsArea').height();
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                //if (st[0].trim() == "width") {
                //    dlgWidth = st[1].trim();
                //}
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
                //else if (st[0].trim() == "top") {
                //    dlgTop = st[1].trim();
                //}
                //else if (st[0].trim() == "left") {
                //    dlgLeft = st[1].trim();
                //}
            }
        }
        var dlgH = dHeight.slice(0, -2);
        if (TabHeight1) {
            TabHeight = TabHeight1;
        }
        if (bIsMax == 'true') {
            //$(_uidialog).find(".PopupTabContentArea_").height(dlgH - 220 - TabHeight);
            $(_uidialog).find("#TreeScrollbar").height(dlgH - 210 - TabHeight);
            //$(_uidialog).find(".OneClickResDiv").height(dlgH - 230 - TabHeight);
            $(_uidialog).find("#GridComponent").height(dlgH - 220 - TabHeight);

        }
        else {
            //$(_uidialog).find(".PopupTabContentArea_").height(dlgH - 47 - TabHeight);
            $(_uidialog).find("#TreeScrollbar").height(dlgH - 47 - TabHeight);
            //$(_uidialog).find(".OneClickResDiv").height(dlgH - 17 - TabHeight);
            $(_uidialog).find("#GridComponent").height(dlgH - 25 - TabHeight);
            $(_uidialog).closest('.admin_v2_1').find("#GridComponent").css({ 'height': '100%' });
            $(_uidialog).find("[data-name='Tab Content Area']").find("#GridComponent").height(dlgH - 0);
        }
        var Calls = $(_uidialog).find("#CallsBOForm");
        if (Calls.length > 0) {
            //$(_uidialog).find("#CallsBOForm").find(".PopupTabContentArea_").height(160);
            //$(_uidialog).find("#CallsOneClickData").find(".OneClickResDiv").height(500);
        }
    }
    else {
        //$(_uidialog).find(".PopupTabContentArea_").height(screen.height - 220);
        $(_uidialog).find("#TreeScrollbar").height(screen.height - 220);
        //$(_uidialog).find(".OneClickResDiv").height(screen.height - 220);
        $(_uidialog).find("#GridComponent").height(screen.height - 220);
    }
}

function fncApplyHalfScroll(_uidialog, sLayoutType) {
    if (sLayoutType == "Dialog") {
        var dlgWidth = _uidialog.attributes['data-dlgWidth'].value;
        var dlgHeight = _uidialog.attributes['data-dlgHeight'].value;
        var dlgTop = _uidialog.attributes['data-dlgTop'].value;
        var dlgLeft = _uidialog.attributes['data-dlgLeft'].value;
        var bIsMax = _uidialog.attributes['data-maximize'];
        if (bIsMax) {
            bIsMax = _uidialog.attributes['data-maximize'].value;
        }
        else {
            bIsMax = 'false';
        }
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                //if (st[0].trim() == "width") {
                //    dlgWidth = st[1].trim();
                //}
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
                //else if (st[0].trim() == "top") {
                //    dlgTop = st[1].trim();
                //}
                //else if (st[0].trim() == "left") {
                //    dlgLeft = st[1].trim();
                //}
            }
        }
        var dlgH = dHeight.slice(0, -2);
        var TabHeight = 0;
        var TabHeight1 = $(_uidialog).find('#TabsArea').height();
        if (TabHeight1) {
            TabHeight = TabHeight1;
        }
        if (bIsMax == 'true') {
            $(_uidialog).find("#BOForm").css({ height: ((dlgH - 7) / 2) - TabHeight + 'px' });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - 7) / 2) - TabHeight + 'px' });
        }
        else {
            $(_uidialog).find("#BOForm").css({ height: ((dlgH - 7) / 2) - TabHeight + 'px' });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - 7) / 2) - TabHeight + 'px' });
        }
        $(_uidialog).find(".OneClickResDiv").height(dlgH - 17 - TabHeight);
        //$(_uidialog).find("#TreeScrollbar").height(dlgH - 210 - TabHeight);
    }
    else {
        $(_uidialog).find("#BOForm").css({ height: ((screen.height - 250) / 2) + 'px' });
        $(_uidialog).find("#OneClickData").css({ height: ((screen.height - 250) / 2) + 'px' });
    }
}


function fncdialogchange($this, Action) {
    var _thisdlg = $($this).closest('div').parent('div').parent('div.ui-dialog');
    var dlgWidth = _thisdlg.attr('data-dlgWidth');
    var dlgHeight = _thisdlg.attr('data-dlgHeight');
    var dlgTop = _thisdlg.attr('data-dlgTop');
    var dlgLeft = _thisdlg.attr('data-dlgLeft');
    var BOViewWidth = $($this).closest('div').parent('div').next('div.dialog-box').find('.PopupTabContentArea_0').outerWidth(true);
    var MenuWidth = $($this).closest('div').parent('div').next('div.dialog-box').find('#MenuArea').outerWidth(true);
    var bTaskbarExists = $('#wrapper').find('.NavbarWrapper');
    //$(_uidialog).find(".OneClickResDiv").height(screen.height - 220);
    if (Action == "maxwidth") {
        /*var LeftPosition = 10;
        var Width = screen.width - 20;
        if (bTaskbarExists && bTaskbarExists.length > 0) {
            LeftPosition = 65;
            Width = Width - 60;
        }
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: Width,
            //height: screen.height,
            left: LeftPosition,
            right: -20,
        });*/
        var wWidth = $(window).width();
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: wWidth - 160,
            height: wHeight,
            left: 50,
            right: 0,
        });
        $($this).next("i.windowminWidth").show();
        $($this).hide();
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ width: screen.width - 20 });//commented
        var dlgbox = $($this).closest('div').parent('div').next('div.dialog-box').find('.PopupTabContentArea_0');

        var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
    }
    else if (Action == "minwidth") {
        if (bTaskbarExists && bTaskbarExists.length > 0) {
            var dlgL = dlgLeft.slice(0, -2);
            if (dlgL < 65) {
                dlgLeft = 65;
            }
        }
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            //height: 'auto',
            width: dlgWidth,
            //height: dlgHeight,
            //top: '105px',
            left: dlgLeft,
        });
        var dlgW = dlgWidth.slice(0, -2)
        $($this).prev("i.windowWidth").show();
        $($this).hide();
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ width: dlgW });//commented
        var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
    }
    else if (Action == "maxheight") {
        /*$($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            height: screen.height - 150,
            top: 20,
            bottom: 20,
        });*/
        var wHeight = $(window).height();
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            height: wHeight,
            top: 0,
            bottom: 0,
        });
        $($this).next("i.windowminHeight").show();
        $($this).hide();
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ height: window.innerHeight - 34 });//commented
        var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
        $($this).closest('div').parent('div').next('div.dialog-box').find("#MenuContent").find('.control-sidebar').animate({ height: window.innerHeight - 40 });
        /*$($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(screen.height - 210);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_").height(screen.height - 210 - TabHeight);
        $($this).closest('div').parent('div').next('div.dialog-box').find("#TreeScrollbar").height(screen.height - 245);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".OneClickResDiv").height(screen.height - 200 - TabHeight);*/
        //$($this).closest('div').parent('div').next('div.dialog-box').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: window.innerHeight - 40 - headerTab });/* - TabHeight*/
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").animate({ height: window.innerHeight - 40 });

        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("#MenuContent").find('.control-sidebar').animate({ height: window.innerHeight - 100 });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: window.innerHeight - 60 - headerTab });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find(".PopupTabContentArea_0").animate({ height: window.innerHeight - 100 });

        var _dialog = $($this).closest('div').parent('div').parent('div.ui-dialog');
        var _uidialog = _dialog[0];
        var bIsMax = _uidialog.attributes['data-maximize'];
        if (bIsMax) {
            bIsMax = _uidialog.attributes['data-maximize'].value;
        }
        else {
            bIsMax = 'false';
        }
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
            }
        }
        var TabHeight = 0;
        var TabHeight1 = $(_uidialog).find('#TabsArea').height();
        if (TabHeight1) {
            TabHeight = TabHeight1;
        }
        if (bIsMax == 'true') {
            var dlgH = dlgHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({
                height: ((dlgH - TabHeight - 40) / 2) + 'px'
            });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - TabHeight - 40) / 2) + 'px' });
        }
        else {
            var dlgH = dHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({
                height: ((dlgH - TabHeight - 5) / 2) + 'px'
            });
            $(_uidialog).find("#OneClickData").css({
                height: ((dlgH - TabHeight - 5) / 2) + 'px'
            });
        }
    }
    else if (Action == "minheight") {
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            //height: '590px',
            height: dlgHeight,
            //width: '800px',
            top: dlgTop,
            //left: dlgLeft,
        });
        var dlgH = dlgHeight.slice(0, -2);
        $($this).hide();
        $($this).prev("i.windowHeight").show();
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ height: dlgH - 36 });//commented
        var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
        $($this).closest('div').parent('div').next('div.dialog-box').find("#MenuContent").find('.control-sidebar').animate({ height: dlgH - 37 });
        /*$($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(dlgH - 60);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_").height(dlgH - 60 - TabHeight);
        $($this).closest('div').parent('div').next('div.dialog-box').find("#TreeScrollbar").height(dlgH - 110);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".OneClickResDiv").height(dlgH - 50 - TabHeight);*/
        //$($this).closest('div').parent('div').next('div.dialog-box').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: dlgH - 73 });/* - TabHeight*/
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").animate({ height: dlgH - 37 });

        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("#MenuContent").find('.control-sidebar').animate({ height: dlgH - 100 });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: dlgH - 100 });/* - TabHeight*/
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find(".PopupTabContentArea_0").animate({ height: dlgH - 100 });

        var _dialog = $($this).closest('div').parent('div').parent('div.ui-dialog');
        var _uidialog = _dialog[0];
        var bIsMax = _uidialog.attributes['data-maximize'];
        if (bIsMax) {
            bIsMax = _uidialog.attributes['data-maximize'].value;
        }
        else {
            bIsMax = 'false';
        }
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
            }
        }
        var TabHeight = 0;
        var TabHeight1 = $(_uidialog).find('#TabsArea').height();
        if (TabHeight1) {
            TabHeight = TabHeight1;
        }
        if (bIsMax == 'true') {
            var dlgH = dlgHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({
                height: ((dlgH - TabHeight - 80) / 2) + 'px'
            });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - TabHeight - 80) / 2) + 'px' });
        }
        else {
            var dlgH = dHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({
                height: ((dlgH - TabHeight - 85) / 2) + 'px'
            });
            $(_uidialog).find("#OneClickData").css({
                height: ((dlgH - TabHeight - 85) / 2) + 'px'
            });
        }
    }
    else if (Action == "minimize") {
        $($this).closest('div').parent('div').parent('div.ui-dialog').attr('data-maximize', false);
        $($this).closest('div').parent('div').parent('div.ui-dialog').attr('data-minimize', true);
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            height: '35px',
            top: screen.height - 135,
            bottom: 0,
            left: 0,
            width: 800,
        }, 500);
        //$("." + DialogDivID).dialog({ dialogClass: 'classFixed', });
        //$("." + DialogDivID).closest(".ui-dialog").removeClass("classAbsolute");
        $($this).hide();
        $($this).next("i.Maximize").show();
        $($this).next('i').next("i.RestoreDown").hide();
        $($this).next('i').next('i').next("i.windowWidth").hide();
        $($this).next('i').next('i').next('i').next("i.windowminWidth").hide();
        $($this).next('i').next('i').next('i').next('i').next("i.windowHeight").hide();
        $($this).next('i').next('i').next('i').next('i').next('i').next("i.windowminHeight").hide();
        $($this).closest('div').parent('div').next('div.dialog-box').hide();
        //$(".windowWidth, .windowHeight, .dialog-box, .Minimize").hide();
        //$($this).closest('div').parent('div').next('div.dialog-box').animate({ height: oDialogD.DialogHeight - 36 });
        var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
        /*$($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(dlgHeight - 70);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_").height(dlgHeight - 110 - TabHeight);
        $($this).closest('div').parent('div').next('div.dialog-box').find("#TreeScrollbar").height(dlgHeight - 110);*/
        //$($this).closest('div').parent('div').next('div.dialog-box').find("[data-name='Tab Content Area'],[data-name='Main Content']").height(dlgH - 83);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(dlgH - 47);
    }
    else if (Action == "maximize") {
        $($this).closest('div').parent('div').parent('div.ui-dialog').attr('data-maximize', true);
        $($this).closest('div').parent('div').parent('div.ui-dialog').removeClass("actualScreenToggle");
        $($this).closest('div').parent('div').parent('div.ui-dialog').addClass("fullScreenToggle");
        /*var LeftPosition = 10;
        var Width = screen.width - 20;
        if (bTaskbarExists && bTaskbarExists.length > 0) {
            LeftPosition = 65;
            Width = Width - 60;
        }
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: Width,
            left: LeftPosition,
            right: -20,
            height: screen.height - 150,
            top: 20,
            bottom: 20,
        });*/
        var wWidth = $(window).width();
        var wHeight = $(window).height();
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: wWidth,
            height: wHeight,
            left: 0,
            right: 0,
            top: 0,
            bottom: 0,
        });
        //$("." + DialogDivID).dialog({ dialogClass: 'classAbsolute', });
        //$("." + DialogDivID).closest(".ui-dialog").removeClass("classFixed");
        $($this).hide();
        $($this).next("i.RestoreDown").show();
        $($this).prev("i.Minimize").show();
        $($this).next('i').next("i.windowWidth").hide();
        $($this).next('i').next('i').next("i.windowminWidth").show();
        $($this).next('i').next('i').next('i').next("i.windowHeight").hide();
        $($this).next('i').next('i').next('i').next('i').next("i.windowminHeight").show();
        $($this).closest('div').parent('div').next('div.dialog-box').show();//commented
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ height: window.innerHeight - 34 });//commented
        //var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var TabHeight = $('.dialog-box').find('#TabsArea').height();
        var headerTab = $('.nav-tabs-custom').height();
        $($this).closest('div').parent('div').next('div.dialog-box').find("#MenuContent").find('.control-sidebar').animate({ height: window.innerHeight - 40 });
        /*$($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(screen.height - 210);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_").height(screen.height - 248);
        $($this).closest('div').parent('div').next('div.dialog-box').find("#TreeScrollbar").height(screen.height - TabHeight - 210);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".OneClickResDiv").height(screen.height - TabHeight - 210);*/
        //$($this).closest('div').parent('div').next('div.dialog-box').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: window.innerHeight - 40 - headerTab });
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").animate({ height: window.innerHeight - 40 });

        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("#MenuContent").find('.control-sidebar').animate({ height: window.innerHeight - 100 });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: window.innerHeight - 60 - headerTab });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find(".PopupTabContentArea_0").animate({ height: window.innerHeight - 100 });

        var _dialog = $($this).closest('div').parent('div').parent('div.ui-dialog');
        var _uidialog = _dialog[0];
        if (dlgHeight && dlgHeight.length > 0) {
            var dlgH = dlgHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({
                height: ((dlgH - TabHeight - 40) / 2) + 'px'
            });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - TabHeight - 40) / 2) + 'px' });
        }
    }
    else if (Action == "restore") {
        $($this).closest('div').parent('div').parent('div.ui-dialog').attr('data-maximize', false);
        $($this).closest('div').parent('div').parent('div.ui-dialog').removeClass("fullScreenToggle");
        $($this).closest('div').parent('div').parent('div.ui-dialog').addClass("actualScreenToggle");
        if (bTaskbarExists && bTaskbarExists.length > 0) {
            var dlgL = dlgLeft.slice(0, -2);
            if (dlgL < 65) {
                dlgLeft = 65;
            }
        }
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: dlgWidth,
            left: dlgLeft,
            height: dlgHeight,
            top: dlgTop
        });
        //    .css({
        //    position: 'absolute',
        //    left: ($(window).width() - $($this).parent('.ui-dialog').outerWidth()) / 5,
        //    top: ($(window).height() - $($this).parent('.ui-dialog').outerHeight()) / 10,
        //});
        var dlgH = dlgHeight.slice(0, -2);
        $($this).hide();
        $($this).prev("i.Maximize").show();
        $($this).next("i.windowWidth").show();
        $($this).next('i').next("i.windowminWidth").hide();
        $($this).next('i').next('i').next("i.windowHeight").show();
        $($this).next('i').next('i').next('i').next("i.windowminHeight").hide();
        $($this).closest('div').parent('div').next('div.dialog-box').animate({ height: dlgH - 36 });//commented
        //var TabHeight = $($this).closest('div').parent('div').next('div.dialog-box').find('#TabsArea').height();
        var TabHeight = $('.dialog-box').find('#PopTabs').height();
        var headerTab = $('.nav-tabs-custom').height();
        $($this).closest('div').parent('div').next('div.dialog-box').find("#MenuContent").find('.control-sidebar').animate({ height: dlgH - 37 });
        /*$($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").height(dlgH - 60);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_").height(dlgH - 98 - TabHeight);
        $($this).closest('div').parent('div').next('div.dialog-box').find("#TreeScrollbar").height(dlgH - 90);
        $($this).closest('div').parent('div').next('div.dialog-box').find(".OneClickResDiv").height(dlgH - 90);*/
        //$($this).closest('div').parent('div').next('div.dialog-box').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: dlgH - 73 });
        $($this).closest('div').parent('div').next('div.dialog-box').find(".PopupTabContentArea_0").animate({ height: dlgH - 37 });

        /*$(_uidialog).closest('.admin_v2_1').find("#MenuContent").find('.control-sidebar').height(oDialogD.DialogHeight - 100);
        $(_uidialog).closest('.admin_v2_1').find(".PopupTabContentArea_0").height(oDialogD.DialogHeight - 100 - AlertHeight);
        $(_uidialog).closest('.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").height(oDialogD.DialogHeight - 100);*/

        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("#MenuContent").find('.control-sidebar').animate({ height: dlgH - 100 });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find("[data-name='Tab Content Area'],[data-name='Main Content']").animate({ height: dlgH - 100 });
        $($this).closest('div').parent('div').parent('div.ui-dialog.admin_v2_1').find(".PopupTabContentArea_0").animate({ height: dlgH - 100 });

        var _dialog = $($this).closest('div').parent('div').parent('div.ui-dialog');
        var _uidialog = _dialog[0];
        var bIsMax = _uidialog.attributes['data-maximize'];
        if (bIsMax) {
            bIsMax = _uidialog.attributes['data-maximize'].value;
        }
        else {
            bIsMax = 'false';
        }
        var dbox = $(_uidialog).find('.dialog-box');
        var dlgStyle = dbox[0].attributes["style"].textContent;
        if (dlgStyle && dlgStyle.length > 0) {
            var Styles = dlgStyle.split(";");
            for (var k = 0; k < Styles.length; k++) {
                var Sty = Styles[k].trim();
                var st = Sty.split(":");
                if (st[0].trim() == "height") {
                    dHeight = st[1].trim();
                }
            }
        }
        var TabHeight = 0;
        var TabHeight1 = $(_uidialog).find('#TabsArea').height();
        if (TabHeight1) {
            TabHeight = TabHeight1;
        }
        if (bIsMax == 'true') {
            var dlgH = dlgHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({ height: ((dlgH - 175) / 2) - TabHeight + 'px' });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - 175) / 2) - TabHeight + 'px' });
        }
        else {
            var dlgH = dHeight.slice(0, -2);
            $(_uidialog).find("#BOForm").css({ height: ((dlgH - TabHeight - 85) / 2) + 'px' });
            $(_uidialog).find("#OneClickData").css({ height: ((dlgH - TabHeight - 85) / 2) + 'px' });
        }
    }
    else if (Action == "defaultPosition") {
        $($this).closest('div').parent('div').parent('div.ui-dialog').attr('data-maximize', false);
        $($this).closest('div').parent('div').parent('div.ui-dialog').removeClass("fullScreenToggle");
        $($this).closest('div').parent('div').parent('div.ui-dialog').addClass("actualScreenToggle");
        var wHeight = $(window).height();
        $($this).closest('div').parent('div').parent('div.ui-dialog').animate({
            width: dlgWidth,
            left: dlgLeft,
            //height: dlgHeight,
            height: wHeight,
            top: dlgTop
        });
    }
}


function fncGetDTable(sURL, i1ClickID, expColumns, sGUID, sSearchText, iPageLength, sTabGUID, sSearchType, sParentWhere, sDynamicMerge, defaultSortColumn, defaultSortOrder) {
    debugger
    var offsetHeight = document.querySelector('.content-wrapper').offsetHeight - 145;

    var offsetHeightForDialog = offsetHeight - 34; // Reduce height by 50px if inside a dialog
    
    var $tableContainer = $('#OneClickResult-' + sTabGUID);
    var isInsideDialog = $tableContainer.closest('.ui-dialog').length > 0; // Check if table is inside a UI dialog
    $.fn.dataTable.ext.errMode = 'none';
    $('#OneClickResult-' + sTabGUID).DataTable({
		//scrollY: "300px",
        //scrollY: offsetHeight - 145 + 'px',
        scrollY: (isInsideDialog ? offsetHeightForDialog : offsetHeight) + 'px',
        scrollX: true,
        responsive: true,
        autoWidth: false,
        scrollCollapse: true,
        fixedHeader: true,
        fixedColumns: { leftColumns: 1, rightColumns: 0 },
        "destroy": true,
        "paging": true,
        "bProcessing": true,
        "bServerSide": true,
        "pageLength": iPageLength,
        //"lengthMenu": [5, 10, 20, 50, 75, 100],
        "aoColumns": expColumns,
        "sServerMethod": "POST",
        "sAjaxSource": sURL + "?i1ClickID=" + i1ClickID + "&sGUID=" + sGUID + "&sSearchText=" + sSearchText,//"@Html.Raw(Url.Action("GetOneClickResult", "XiLink", new { i1ClickID = o1ClickD.ID, sGUID = o1ClickD.sGUID, sSearchText = o1ClickD.SearchText }))",
        "aaSorting": [[defaultSortColumn, defaultSortOrder]],
        //"sDom": '<"dtable-header"><"datatable-scroll"><"dtable-footer">', 'bJQueryUI': false,
        "sDom": 'r<"dtble-header"<"new-query">><"datatable-scroll"t><"dtable-footer"ip>', 'bJQueryUI': false,
        "initComplete": function () {

        },
        "autoWidth": false,
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
            var sLastColumn = expColumns[expColumns.length - 1];
            $.each(aData, function (key, value) {
                if (value && (value.startsWith("�") || value.startsWith("-�")) && jQuery.isNumeric(value.split('�')[1])) {
                    $('td:eq(' + key + ')', nRow).html(jQuery.format(value, "c")).addClass('text-right');
                }
                if (value == "01-Jan-1900") {
                    $('td:eq(' + key + ')', nRow).html("");
                }
                if (value > "�0.00") {
                    $('td:eq(' + key + ')', nRow).addClass('color-positive');
                }

                if (value.startsWith("xi.")) {
                    $('td:eq(' + key + ')', nRow).html("<a class='text-ellipsis' data-toggle='tooltip' style='height: auto;width: auto' title='" + value + "'>" + value.substring(0, 30) + "</a>");
                }
                $('td:eq(' + key + ')', nRow).addClass('one-1q-field');
                var oneqfiled = expColumns[key].sName;
                if (oneqfiled.indexOf('.') > 0) {
                    var qName = oneqfiled.split('.')[1];
                    $('td:eq(' + key + ')', nRow).addClass('one-1q-field-' + qName.toLowerCase());
                }
                else {
                    $('td:eq(' + key + ')', nRow).addClass('one-1q-field-' + expColumns[key].sName.toLowerCase());
                }
                if (oneqfiled == "HiddenData") {
                    if (sLastColumn.sName == "HiddenData") {
                        $('td:eq(' + key + ')', nRow).addClass("hidden");
                    }
                    else if (sLastColumn.sName == "Actions") {
                        $('td:eq(' + (key + 1) + ')', nRow).addClass("hidden");
                    }
                }
            });
            var sLastColumn = expColumns[expColumns.length - 1];
            if (sLastColumn.sName == "HiddenData" || sLastColumn.sName == "Color") {

                $('td', nRow).addClass(aData[aData.length - 1]);
            }
            if (expColumns.length < aData.length) {
                $('td', nRow).addClass(aData[aData.length - 1]);
            }
            $(nRow).addClass('simpleclick_' + aData[0]);
            $(nRow).addClass('one-1q-row');
            //$(nRow).children('.sorting_1').next().addClass('color_Name');
            //($(nRow).find(".sorting_1").length > 0).addClass("color_Name");
            //if ($('td', nRow).find(".sorting_1").length - 1){ 
            $('td:eq(1)', nRow).addClass("color_Name");
            //}
            $('td:contains("Low")', nRow).html("<span class='color_Low'>Low</span>");
            $('td:contains("Medium")', nRow).html("<span class='color_Medium'>Medium</span>");
            $('td:contains("High")', nRow).html("<span class='color_High'>High</span>");
            $('td:contains("Active"),td:contains("New")', nRow).addClass("color-positive");
            $('td.one-1q-field-istatus:contains("In Development")', nRow).wrapInner("<span class='In_Development icn'/>");
            $('td.one-1q-field-istatus:contains("Done")', nRow).wrapInner("<span class='Done icn'/>");
            $('td.one-1q-field-istatus:contains("To Do")', nRow).wrapInner("<span class='To_Do icn'/>");
            $('th:contains("XIGUID")', nRow).each(function (idx) {
                $('td:eq(index)', nRow).addClass('red');
            })
        },
        "fnServerParams": function (aoData) {
            aoData.push({ "name": "Fields", "value": Fields });
            aoData.push({ "name": "Optrs", "value": Optrs });
            aoData.push({ "name": "Values", "value": Values });
            aoData.push({ "name": "Type", "value": sSearchType });
            aoData.push({ "name": "SearchText", "value": sSearchText });
            aoData.push({ "name": "sParentWhere", "value": sParentWhere });
            if (sDynamicMerge && sDynamicMerge.length != 0) {
                aoData.push({ "name": "NVPairs", "value": JSON.stringify(sDynamicMerge) });
            }
        },
    });
    $('#OneClickResult-' + sTabGUID).on('error.dt', function (e, settings, techNote, message) {
        fncLogJqueryError("Datatable", message);
    }).DataTable();

    
    /* let resizeTimer;
    $(document).find(".windowWidth, .windowminWidth").on("click", function () {
        debugger
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            var table = $('#OneClickResult-' + sTabGUID).DataTable();
            table.columns.adjust().responsive.recalc();
        }, 200);
    });

    $(window).on('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            var table = $('#OneClickResult-' + sTabGUID).DataTable();
            table.columns.adjust().responsive.recalc();
        }, 200); // Adjusts 200ms after resizing stops
    });

    $(".ui-dialog").on("dialogresize", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            var table = $('#OneClickResult-' + sTabGUID).DataTable();
            table.columns.adjust().responsive.recalc();
        }, 200);
    }); */
    let resizeTimer;

    // Trigger on button clicks (custom buttons like expand, min-width)
    $(document).on("click", ".windowWidth, .windowminWidth", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            //adjustDataTable();
        }, 200);
    }).trigger(".windowWidth, .windowminWidth");

    // Trigger on window resize
    $(window).on("resize", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
           // adjustDataTable();
        }, 200);
    });

    // Trigger on UI dialog resize
    $(document).on("dialogresize", ".ui-dialog", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
           // adjustDataTable();
        }, 200);
    });

    // Function to adjust DataTable
    function adjustDataTable() {
        var tableId = "#OneClickResult-" + sTabGUID;
        
        if ($.fn.dataTable.isDataTable(tableId)) {
            var table = $(tableId).DataTable();
            table.columns.adjust().responsive.recalc();
        }
    }
}

function fncCloseDlg(_this) {
    $(_this).closest('.ui-dialog').dialog().dialog('close');
}

function fncDialogNavigate(BtnID, dID) {
    var dlgDetails = BtnID.split('-');
    if (dlgDetails.length == 3) {
        var dlgID = dlgDetails[1];
        var iInsID = dlgDetails[2];
        var dlgSelect = "ResultDialog-" + dlgID + "_" + iInsID;
        //$('.dialog-' + dlgID).find('.innerdialog-box').hide();
        $('.' + dID).find('.' + dlgSelect).parent().find('.innerdialog-box').hide();
        $('.' + dID).find('.' + dlgSelect).show();
    }
}

function fncTaskBarNavigate(BtnID) {
    var _uidialog = $('div[data-identity="' + BtnID + '"]');
    var bIsMinimize = $(_uidialog).attr('data-minimize');
    if (bIsMinimize && bIsMinimize == 'true') {
        var _max = $(_uidialog).find('i.Maximize');
        fncdialogchange(_max, "maximize");
        $(_uidialog).attr('data-minimize', 'false');
    }
    $('div[data-identity="' + BtnID + '"]').show();
    var Zindex = 1010;
    $('body').find('.ui-dialog').each(function () {
        Zindex--;
        $(this).css('z-index', Zindex);
    });
    $('div[data-identity="' + BtnID + '"]').css('z-index', '1012');
    //var dlgDetails = BtnID.split('-');
    //if (dlgDetails.length == 2) {
    //    var dlgID = dlgDetails[1];
    //    var dlgSelect = "dialog-" + dlgID; //data-indentity="dialog-2336"
    //    $('div[data-indentity="dialog-' + BtnID + '"]').show();
    //}
}

function fncCloseNavBtn(BtnID, _this) {
    if (_this) {
        $(_this).parent().parent('.dialogNavBtn').remove();
        //$('div[data-identity="' + BtnID + '"]').remove();
        //$("." + BtnID).dialog('close');
        $("." + BtnID).dialog('destroy').remove();
        var CustomDialogs = $('body').find('.one-custom-dialog');
        if (CustomDialogs && CustomDialogs.length > 0) {

        }
        else if (CustomDialogs && CustomDialogs.length == 0) {
            $('#dialog-background').remove();
        }
    }
    else {
        if (BtnID && BtnID.length > 0) {
            var sBtnGUID = BtnID.split('_');
            if (sBtnGUID.length > 1) {
                sBtnGUID = sBtnGUID[1];
                $('#NavigationBar').find('#' + sBtnGUID).remove();
                $("." + BtnID).dialog('destroy').remove();
            }
        }
    }
}

function fncCloseGroupBtn(BtnID, _this) {
    $(_this).parent().parent('.dialogNavBtn').remove();
    //$('div[data-identity="' + BtnID + '"]').remove();
    //$("." + BtnID).dialog('close');
    $("." + BtnID).remove();
}

function fncdialogclose(_this, bIsTaskbar, dlgdivID, sBoName, iBOIID) {
debugger
    var bIsLock = $(_this).attr('data-bIsLock');
    if (bIsLock != undefined && bIsLock != "" && UpdateLockURL != undefined) {
        $.ajax({
            type: 'POST',
            url: UpdateLockURL,
            data: { sBoName: sBoName, sInstanceID: iBOIID, sIsLock: bIsLock },
            cache: false,
            async: true,
            dataType: 'json',
            success: function (data) {
            }
        });
    }
    if (bIsTaskbar == false) {
        $("." + dlgdivID).dialog('close');
        $("." + dlgdivID).dialog('destroy').remove();
        var CustomDialogs = $('body').find('.one-custom-dialog');
        if (CustomDialogs && CustomDialogs.length > 0) {

        }
        else if (CustomDialogs && CustomDialogs.length == 0) {
            $('#dialog-background').remove();
        }
    }
    else {
        var NavBar = $('#NavigationBar');
        if (NavBar && NavBar.length > 0) {
            $(_this).parent().parent().parent().hide();
        }
        else {
            $("." + dlgdivID).dialog('close');
            $("." + dlgdivID).dialog('destroy').remove();
        }
    }
    if (dlgdivID == 'cifsrchdlg') {
        $('#QuickSearchTextBox').val('');
    }
}

function fncGetMoreDetails(This) {
    var selector = $(This).attr('id');
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', This);
    var oParams = [];
    var Param = {};
    Param["sName"] = "sOutput";
    Param["sValue"] = "AddonsArea_" + selector.split('_')[1];
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|Aggregations.ID}";
    Param["sValue"] = selector.split('_')[1];
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|iProductVersionID}";
    Param["sValue"] = "xi.s|{xi.a|'Aggregations','','FKiProductVersionID','',id:" + selector.split('_')[1] + "}";
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|iQSDefinitionID}";
    Param["sValue"] = "xi.s|{xi.a|'ProductVersion_T',{xi.a|'Aggregations','','FKiProductVersionID','',id:" + selector.split('_')[1] + "},'FKiQSDefinitionIDXIGUID'}";
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|FKiQSInstanceID}";
    Param["sValue"] = "xi.s|{xi.a|'Aggregations','','FKiQSInstanceID','',id:" + selector.split('_')[1] + "}";
    oParams.push(Param);
    XILinkLoadJson('2DCA3FC2-892D-440A-BBED-61722D317721', sGUID, oParams)
    var btnText = $(This).attr('data-btn');
    var ClosebtnText = $(This).attr('data-clsebtn');
    if ($("." + selector).is(":visible")) {
        $("." + selector).hide();
        $("#" + selector).html('More Details');
        $("#" + selector).removeClass("closeanchor").addClass("btn Moredetailsbtn");
        if (btnText != undefined && btnText != "") {
            $("#" + selector).html(btnText);
        }
        else {
            $("#" + selector).html('More Details');
            $("#" + selector).removeClass("closeanchor").addClass("btn Moredetailsbtn");
        }
    }
    else {
        $("." + selector).show();
        $("#" + selector).html('Close Details');
        $("#" + selector).removeClass("btn Moredetailsbtn").addClass("closeanchor");
        if (ClosebtnText != undefined && ClosebtnText != "") {
            $("#" + selector).html(ClosebtnText);
        }
        else {
            $("#" + selector).html('Close Details');
            $("#" + selector).removeClass("btn Moredetailsbtn").addClass("closeanchor");
        }
    }
    $("#AddonsArea_" + selector.split('_')[1]).show();
    XILinkLoadJson('4C0CA1A1-8DC4-4EF9-9030-02B050B86F80', sGUID, oParams)
}

function getMonth(monthStr) {
    if (monthStr && monthStr.length > 0) {
        if (monthStr.toLowerCase() == "jan") {
            return 1;
        }
        else if (monthStr.toLowerCase() == "feb") {
            return 2;
        }
        else if (monthStr.toLowerCase() == "mar") {
            return 3;
        }
        else if (monthStr.toLowerCase() == "apr") {
            return 4;
        }
        else if (monthStr.toLowerCase() == "may") {
            return 5;
        }
        else if (monthStr.toLowerCase() == "jun") {
            return 6;
        }
        else if (monthStr.toLowerCase() == "jul") {
            return 7;
        }
        else if (monthStr.toLowerCase() == "aug") {
            return 8;
        }
        else if (monthStr.toLowerCase() == "sep") {
            return 9;
        }
        else if (monthStr.toLowerCase() == "oct") {
            return 10;
        }
        else if (monthStr.toLowerCase() == "nov") {
            return 11;
        }
        else if (monthStr.toLowerCase() == "dec") {
            return 12;
        }
        else {
            return monthStr;
        }
        //new Date(monthStr + '-1-01').getMonth() + 1
    }
}

function fncOpenAllDialogs() {
    $('body').find('.ui-dialog').each(function () {
        $(this).show()
        var bIsMinimize = $(this).attr('data-minimize');
        if (bIsMinimize && bIsMinimize == 'true') {
            var _max = $(this).find('i.Maximize');
            fncdialogchange(_max, "maximize");
            $(this).attr('data-minimize', 'false');
        };
    });
}

function fncCloseAllDialogs() {
    $('body').find('.ui-dialog').each(function () {
        $(this).hide();
    });
}

function fncOpenInPopup(sBOName, iBOIID, iLayoutID, sGUID, sDivID) {
    //Don't delete
    //var HTMLObj = $('html');
    //var cloneHTMLObj = $(HTMLObj).clone();
    //var InnerHTML = $($('.' + sDivID)).html();
    //$(cloneHTMLObj).find("body").empty();
    //$(cloneHTMLObj).find("body").html(InnerHTML);
    //var Popup = window.open('', '_blank');
    //with (Popup.document) {
    //    open();
    //    write($(cloneHTMLObj).html());
    //    close();
    //}
    //Popup.moveTo(0, 0);

    $.ajax({
        type: 'POST',
        url: OpenInPopupURL,
        data: JSON.stringify({ sBOName: sBOName, iBOIID: iBOIID, iLayoutID: iLayoutID, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'html',
        traditional: true,
        async: true,
        success: function (XIData) {
            fncCloseNavBtn(sDivID);
            var Popup = window.open('', '_blank');
            with (Popup.document) {
                open();
                write(XIData);
                close();
            }
            Popup.moveTo(0, 0);
        },
        error: function (error) {
        }
    });
}

function fncOpenInPopups($this, dlgdivID, sGUID, sBoName, iBOIID) {
    var HTMLObj = $('html');
    var cloneHTMLObj = $(HTMLObj).clone();
    var InnerHTML = $($('.compPop-' + sGUID)).find('.xicomponent').html();
    $(cloneHTMLObj).find("body").empty();
    $(cloneHTMLObj).find("body").html(InnerHTML);
    var Popup = window.open('', '_blank');
    with (Popup.document) {
        open();
        write($(cloneHTMLObj).html());
        close();
    }
    Popup.moveTo(0, 0);
}

function fncRefreshPopup(DivID, sGUID, iLayoutID) {
    fncLoadLayout(iLayoutID, null, sGUID, DivID, "Refresh");
}

function fncToolTip(iBODID, _this) {
    $(_this).popover({
        trigger: 'hover',
        //placement: 'right',
        html: true,
        //selector: '#OneClickResult-ReportID td',
        content: function () {
            //if ((MouseOver && MouseOver.length>0) || ColumnName=="ID") {
            var ID = $(_this).attr('data-instanceid');
            var BOID = iBODID;
            var Column = $(_this).attr('data-columnname');
            if (Column && Column.length > 0 && ID && ID != 0) {
                var pvalue = {
                    BOID: BOID,
                    ID: parseInt(ID),
                    ColumnName: Column
                }
                fncGetPopoverHTML(pvalue).then(function (rowvalues) {
                    $(".popcontainer").empty();
                    $(".popcontainer").append(rowvalues);
                });

                var rowdata = $('.popcontainer').html();
                return $(rowdata).html();
            }

            //}
        },
        container: 'body',
        placement: 'auto top'
    });
}

function fncGetPopoverHTML(Data) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/XiLink/ListHover',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            cache: false,
            async: true,
            data: JSON.stringify(Data),
            success: function (data) {
                //$('.popcontainer').html('');
                if (data) {
                    var rowvalues = "";
                    rowvalues = rowvalues + '<div class="popover-time">';
                    rowvalues = rowvalues + '<table class="table popover-table">';
                    rowvalues = rowvalues + '<tbody>';
                    rowvalues = rowvalues + '<thead>';
                    rowvalues = rowvalues + '</tr>';
                    if (data) {
                        for (i = 0; i < data.NVPairs.length; i++) {
                            //data[i].sValue = 'http://192.168.7.7/Converter/Content/images/lead-logo.png';
                            rowvalues = rowvalues + '<tr>';
                            rowvalues = rowvalues + '<td class="pop-property">' + data.NVPairs[i].sName + '</td>';
                            //rowvalues = rowvalues + '<td>|</td>';

                            var IsImage = false;
                            var BOFields = data.BOD.Attributes;
                            for (j = 0; j < BOFields.length; j++) {
                                if (data.NVPairs[i].sName == BOFields[j].Name) {
                                    var FileType = BOFields[j].FKiFileTypeID;
                                    if (FileType > 0) {
                                        IsImage = true;
                                    }
                                }
                            }
                            if (IsImage) {
                                rowvalues = rowvalues + '<td><img src="' + data.NVPairs[i].sValue + '"></td>';
                            }
                            else {
                                rowvalues = rowvalues + '<td class="pop-value">' + data.NVPairs[i].sValue + '</td>';
                            }
                            //rowvalues = rowvalues + '<td><img src="/Content/Files/png/2018/3/16/images_1_168_137.png"></td>';
                            rowvalues = rowvalues + '</tr>';
                        }
                    }
                    //else {
                    //    rowvalues = rowvalues + '<tr>';
                    //    rowvalues = rowvalues + '<td> No foregin key group </td>';
                    //    rowvalues = rowvalues + '</tr>';
                    //}
                    rowvalues = rowvalues + '</thead>';
                    rowvalues = rowvalues + '</tbody>';
                    rowvalues = rowvalues + '</table>';
                    rowvalues = rowvalues + '</div>';
                    //$(".popcontainer").empty();
                    //$(".popcontainer").append(rowvalues);
                    //var rowdata = $('.popcontainer').html();
                    resolve(rowvalues);
                    //return rowvalues;
                }
                else {
                    $(".popcontainer").empty();
                }
            },
            error: function (data) {
                $(".popcontainer").empty();
            }
        });
    });
}
function fncGetAccidentTypeFieldDependency($this, $sDepBOField) {
    var Field = $($this).attr('data-attrname');
    if (Field == "iStatus" && $sDepBOField == "iJunkResaonID") {
        var FieldVal = $($this).find('option:selected').val();
        var parentName = 'bofield';
        var childObj1 = $($this).closest('.form-group').next('.form-group').find('[name="' + $sDepBOField + '"]');
        if (FieldVal == "50") {
            fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
        }
        else {
            fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
        }
    }
    else if ($($this).find('option:selected').val() == "1") {
        var parentName = 'bofield';
        var childObj1 = $($this).closest('.form-group').next('.form-group').find('[name="' + $sDepBOField + '"]');
        fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
    }
    else {
        var parentName = 'bofield';
        var childObj1 = $($this).closest('.form-group').next('.form-group').find('[name="' + $sDepBOField + '"]');
        fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
    }
}

function fncGetGroupNameDependency($this, sDepBOField, sDepBoName, BOID, sGUID) {
    var vBOID = "";
    if ($this.tagName && $this.tagName == 'SELECT') {
        if ($($this).val() && $($this).val() != '0')
            vBOID = $($this).val();
    }
    else {
        if (BOID && BOID != "0")
            vBOID = BOID;
    }
    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);

    //if (BOID && BOID != "0")
    //    vBOID = BOID;
    //else if ($($this).val() && $($this).val() != '0')
    //    vBOID = $($this).val();
    //var GUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    if (vBOID && sGUID) {
        var DepBO = sDepBOField.split(',');
        var parentName = 'bofield';
        for (var k = 0; k < DepBO.length; k++) {
            var childObj1 = $("[name=" + DepBO[k] + "]");
            sDepBoName = $(childObj1[0]).attr('data-sbo');
            fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            fncDependencyGroupDropDown(childObj1[0], vBOID, sDepBoName, sGUID);
        }
    }
}
//For Menu Type change Dependency
function fncGetMenuDependency($this, sDepBOField, sGUID) {
    var id = $("#DDL-06796b7d-86ac-49e5-accb-bca1a9a87960" + sGUID).val();
    var DepBO = sDepBOField.split(',');
    var parentName = 'bofield';
    for (var k = 0; k < DepBO.length; k++) {

        if (id == 20 || id == 30) {
            if (k != 0) {
                var childObj1 = $("[name=" + DepBO[k - 1] + "]");
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
            }
            else {
                var childObj1 = $("[name=" + DepBO[2] + "]");
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
                //fncDependencyMenuDropDown(childObj1[0], sGUID);
            }
        }
        else {
            if (k != 0) {
                var childObj1 = $("[name=" + DepBO[k - 1] + "]");
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else {
                var childObj1 = $("[name=" + DepBO[2] + "]");
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
            }
        }
    }
}
//For Structure Details in simple1click
function fncGetStructureDependency($this, sDepBOField, sGUID) {
    var parentName = 'bofield';
    var childObj1 = $("[name='" + sDepBOField + "']");
    var cvi = $("#DDL-OnRowClickType" + sGUID + " option:selected").val();
    if ($($this).val() == 20 || cvi == 20) {
        // var childObj1 = $("[name='" + sDepBOField + "']");
        fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
        fncGetStructureDependencyDrpDown(childObj1[0], sGUID);
    }
    else {
        fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
    }
}
function fncGetStructureDependencyDrpDown(childObj1, sGUID) {
    $.ajax({
        url: BOStructureDependencyDropDown,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: false,
        cache: false,
        data: JSON.stringify({ sGUID: sGUID }),
        success: function (data) {
            var selected = '';
            var value = $(childObj1).attr('data-nonformdata');
            $(childObj1).html('');
            $(childObj1).append('<option value="0" ' + selected + '>Please Select</option>');
            for (var k = 0; k < data.length; k++) {
                selected = '';
                if (value && value == data[k].ID) {
                    selected = 'selected';
                }
                else {
                    selected = value;
                }
                $(childObj1).append('<option value="' + data[k].ID + '"' + selected + '>' + data[k].sCode + '</option>');
            }
        }
    });
}

function fncGetBoNameDependency($this, sDepBOField, sDepBoName, BOID, sGUID) {
    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var vBOID = "";
    if (BOID && BOID != "0")
        vBOID = BOID;
    else if ($($this).val() && $($this).val() != '0')
        vBOID = $($this).val();

    if (vBOID && sGUID) {
        var DepBO = sDepBOField.split(',');
        var parentName = 'bofield';
        for (var k = 0; k < DepBO.length; k++) {
            //var t = DepBO[k];
            var childObj1 = $("[name=" + DepBO[k] + "]");
            //childObj1 = $($this).closest('.form-group').next('.form-group').next().find('[name="' + DepBO[k] + '"]');
            //var BOID = $($this).val();
            fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            fncBoDependencyGroupDropDown(childObj1[0], vBOID, sDepBoName, sGUID);
        }
    }
}
function fncBoDependencyGroupDropDown(childObj1, BOID, sDepBoName, sGUID) {
    $.ajax({
        url: BODependencyGroupDropDown,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: false,
        cache: false,
        data: JSON.stringify({ BOID: BOID, sGUID: sGUID, BOName: sDepBoName }),
        success: function (data) {
            var selected = '';
            var value = $(childObj1).attr('data-nonformdata');
            if (!value || value == '0')
                selected = 'selected';

            $(childObj1).html('');
            $(childObj1).append('<option value="0" ' + selected + '>Please Select</option>');
            for (var k = 0; k < data.length; k++) {
                selected = '';
                if (value && value == data[k].Name) {
                    selected = 'selected';
                }
                $(childObj1).append('<option value="' + data[k].Name + '"' + selected + '>' + data[k].LabelName + '</option>');
            }
        }
    });
}

function fncBOFieldDependencyDropDown($this, $sDepBOField) {
    var depText = $($this).find(':selected').text();
    $('[name= "' + $sDepBOField + '"]').val(depText);
}

function fncDependencyGroupDropDown(childObj1, BOID, sDepBoName, sGUID) {
    $.ajax({
        url: GetDependencyGroupDropDown,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: false,
        cache: false,
        data: JSON.stringify({ BOID: BOID, sGUID: sGUID, BOName: sDepBoName }),
        success: function (data) {
            var selected = '';
            var value = $(childObj1).attr('data-nonformdata');
            if (!value || value == '0') {
                selected = 'selected';
            }
            else {
                selected = value;
            }
            $(childObj1).html('');
            //$(childObj1).append('<option value="0" ' + selected + '>Please Select</option>');
            for (var k = 0; k < data.length; k++) {
                selected = '';
                if (value && value == data[k].ID) {
                    selected = 'selected';
                }
                else {
                    selected = value;
                }
                $(childObj1).append('<option value="' + data[k].XIGUID + '"' + selected + '>' + data[k].GroupName + '</option>');

            }
        }
    });
}
function fncGetBOAttrDependency($this, sDepBOField, sDepBoName) {
    var iParentIID = $($this).val();
    var GUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    if (iParentIID && GUID) {
        var DepBO = sDepBOField.split(',');
        var parentName = 'bofield';
        for (var k = 0; k < DepBO.length; k++) {
            var childObj1 = $("[name=" + DepBO[k] + "]");
            var ChildAttr = $(childObj1[0]).attr('data-attrid');
            var sParentBO = $($this).attr('data-sbo');
            fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            //fncDependencyGroupDropDown(childObj1[0], vBOID, sDepBoName, GUID);
            $.ajax({
                url: GetBoAttributeDependencyDropDown,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                cache: false,
                data: JSON.stringify({ ChildAttr: ChildAttr, sValue: iParentIID, sParentBo: sParentBO }),
                success: function (data) {
                    if (data != null && data.length > 0) {
                        var ElementType = "";
                        var Element = $('[data-attrid=' + ChildAttr + ']').get(0);
                        if (Element != undefined) {
                            ElementType = $('[data-attrid=' + ChildAttr + ']').get(0).tagName;
                        }
                        $('[data-attrid=' + ChildAttr + ']').empty();
                        if (ElementType.toLowerCase() == "select") {
                            if (data && data.length > 0) {
                                $('[data-attrid=' + ChildAttr + ']').append('<option value="0" disabled selected>Please Select</option>');
                                var sFieldValue = $('[data-attrid=' + ChildAttr + ']').val();
                                for (var k = 0; k < data.length; k++) {
                                    if (data[k].sOptionValue == sFieldValue) {

                                        $('[data-attrid=' + ChildAttr + ']').append('<option value="' + data[k].sOptionValue + '" selected>' + data[k].sOptionName + '</option>');
                                    }
                                    else {
                                        $('[data-attrid=' + ChildAttr + ']').append('<option value="' + data[k].sOptionValue + '">' + data[k].sOptionName + '</option>');
                                    }
                                }
                            }
                        }
                        if (ElementType.toLowerCase() == "input" || ElementType.toLowerCase() == "textarea") {
                            if (data && data.length > 0) {
                                for (var k = 0; k < data.length; k++) {
                                    $('[data-attrid=' + ChildAttr + ']').val(data[k].sOptionName);
                                    $('[data-attrid=' + ChildAttr + ']').val(data[k].sOptionName);
                                }
                            }
                            else {
                                $('[data-attrid=' + ChildAttr + ']').val("");
                            }
                        }
                    }
                }
            });
        }
    }

    //var Value = $($this).val();

}
function fncGetDependencySearch(ID, sParentBO, $this, iDependentFieldID) {
    var Value = $($this).val();
    $.ajax({
        url: GetDependencyDropDownSearch,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: false,
        cache: false,
        data: JSON.stringify({ ParentID: ID, sValue: Value, sParentBo: sParentBO }),
        success: function (data) {
            if (data != null && data.length > 0) {
                var ElementType = "";
                var Element = $('[data-attrid=' + iDependentFieldID + ']').get(0);
                if (Element != undefined) {
                    ElementType = $('[data-attrid=' + iDependentFieldID + ']').get(0).tagName;
                }
                $('[data-attrid=' + iDependentFieldID + ']').empty();
                if (ElementType.toLowerCase() == "select") {
                    if (data && data.length > 0) {
                        $('[data-attrid=' + iDependentFieldID + ']').append('<option value="0" disabled selected>Please Select</option>');
                        var sFieldValue = $('[data-attrid=' + iDependentFieldID + ']').val();
                        for (var k = 0; k < data.length; k++) {
                            if (data[k].sOptionValue == sFieldValue) {

                                $('[data-attrid=' + iDependentFieldID + ']').append('<option value="' + data[k].sOptionValue + '" selected>' + data[k].sOptionName + '</option>');
                            }
                            else {
                                $('[data-attrid=' + iDependentFieldID + ']').append('<option value="' + data[k].sOptionValue + '">' + data[k].sOptionName + '</option>');
                            }
                        }
                    }
                }
                if (ElementType.toLowerCase() == "input" || ElementType.toLowerCase() == "textarea") {
                    if (data && data.length > 0) {
                        for (var k = 0; k < data.length; k++) {
                            $('[data-attrid=' + iDependentFieldID + ']').val(data[k].sOptionName);
                            $('[data-attrid=' + iDependentFieldID + ']').val(data[k].sOptionName);
                        }
                    }
                    else {
                        $('[data-attrid=' + iDependentFieldID + ']').val("");
                    }
                }
            }
        }
    });
}
function fncGetDependencyDropdownChange($this, $sDepBOField) {
    var cvi = $("#" + $this.id + " option:selected").val();
    if ($($this).val() != "0") {
        var DepBO = $sDepBOField.split(',');
        var parentName = 'bofield';
        for (var k = 0; k < DepBO.length; k++) {
            var t = DepBO[k];
            var childObj1 = $("[name=" + DepBO[k] + "]");
            //childObj1 = $($this).closest('.form-group').next('.form-group').next().find('[name="' + DepBO[k] + '"]');
            var BOID = $($this).val();
            if (cvi == "bo" && t == "sBOSelectedFields") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else if (cvi == "bo" && t == "iNotificationBO") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else if (cvi == "oneclick" && t == "fkiOneClick") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else if ((cvi == "Dialog" || cvi == "Popup") && t == "fkidepOneClick") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            //else if (cvi == "Popup" && t == "fkidepOneClick") {
            //    fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            //}
            else if ((cvi == "Alert" || cvi == "Flash") && t == "sAlertText") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else if ((cvi == "snippet" || cvi == "bargraph" || cvi == "piechart" || cvi == "chats") && t == "fkidepOneClick") {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            else {
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'hide');
            }
            //else {
            //    fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            //}
            //fncDependencyGroupDropDown(childObj1[0], BOID, sDepBoName);
        }

    }
}

var sPreviousValue = "";
function fncGetReqChaseFieldDependency($this, $sDepBOField) {
    var sValue = $($this).val();
    if (sPreviousValue == "") {
        sPreviousValue = $($this).attr("data-nonformdata");
    }
    if (sPreviousValue != "" && sValue != "" && sValue != "29" && sValue != "30") {
        var iDays = 0;
        var iPreviousValue = parseInt(sPreviousValue);
        var iValue = parseInt(sValue);
        if (iPreviousValue < iValue) {
            iDays = iValue - iPreviousValue;
            var formid = $($this).closest("form").attr("id");
            var sDate = $('#' + formid).find('[name= "' + $sDepBOField + '"]').val();
            var date = new Date(sDate);
            if (!isNaN(date.getTime())) {
                date.setDate(date.getDate() + iDays);
                $('[name= "' + $sDepBOField + '"]').val(date.toInputFormat());
                $($this).attr("data-nonformdata", iValue);
            }
        }
        else {
            iDays = iPreviousValue - iValue;
            var formid = $($this).closest("form").attr("id");
            var sDate = $('#' + formid).find('[name= "' + $sDepBOField + '"]').val();
            var date = new Date(sDate);
            if (!isNaN(date.getTime())) {
                date.setDate(date.getDate() - iDays);
                $('[name= "' + $sDepBOField + '"]').val(date.toInputFormat());
                $($this).attr("data-nonformdata", iValue);
            }
        }
        sPreviousValue = "";
    }

}
Date.prototype.toInputFormat = function () {
    const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];
    var yyyy = this.getFullYear().toString();
    var mm = (this.getMonth()).toString(); // getMonth() is zero-based
    var dd = this.getDate().toString();
    //return yyyy + "-" + (mm[1]?mm:"0"+mm[0]) + "-" + (dd[1]?dd:"0"+dd[0]); // padding
    return (dd[1] ? dd : "0" + dd[0]) + "-" + (mm[1] ? monthNames[mm] : monthNames[mm[0]]) + "-" + yyyy;
};
function fnccheckBODependency($this) {
    var sShowField; var sHideField; var iSHType; var parentName; var bIsChecked = false;
    if ($this) {
        var _sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
        var oQSInstance = QSInsDict[_sGUID];
        var DependentField = "";
        if ($this.id && $this.id.length > 0) {
            DependentField = $('#Field-' + $this.id.split('-')[1]).val();
        }
        var tagname = $this.tagName;
        if (tagname == "SELECT") {
            sShowField = $($this).find(':selected').attr('data-show');
            sHideField = $($this).find(':selected').attr('data-hide');
            iSHType = $($this).find(':selected').attr('data-shtype');
            bIsChecked = true;
            if (DependentField != "" && $("#" + $this.id + " option:selected").val().toLowerCase() == "se") {
                $("#select-" + DependentField).val("true");
                sShowField = "ABC";
                iSHType = "20";
            }
            else {
                if (DependentField == 1362) {
                    $("#select-" + DependentField).val("");
                    sShowField = "";
                    iSHType = "";
                }
            }
        }
        else {
            sShowField = $($this).attr('data-show');
            sHideField = $($this).attr('data-hide');
            iSHType = $($this).attr('data-shtype');
            if (tagname == "OPTION") {
                bIsChecked = true;
            }
            else {
                if ($($this).is(':checked')) {
                    bIsChecked = true;
                }
            }
        }
        if (iSHType == 10) {
            parentName = 'bofield';
            if (sShowField && sShowField.length > 0) {
                var childObj1 = $($this).closest('.form-group').next('.form-group').find('[name="' + sShowField + '"]');
                //$('[name="' + sShowField + '"]').show();
                fncHideShowBOFromHTMLTree(parentName, childObj1[0], 'show');
            }
            if (sHideField && sHideField.length > 0) {
                var childObj2 = $($this).closest('.form-group').next('.form-group').find('[name="' + sHideField + '"]');
                fncHideShowBOFromHTMLTree(parentName, childObj2[0], 'hide');
                //$('[name="' + sHideField + '"]').hide();
                //Hide dependecy Fields recursively
                //var HideRemaining = $('.questionset-section select[name="' + sHideField + '"]');
                //if (HideRemaining) {
                //    $(HideRemaining).find('option[value="0"]').prop('selected', true);//set DropDwon Value To 0
                //    fnccheckme(HideRemaining[0]);
                //}
            }
        }
        else if (iSHType == 20) {
            if (sShowField && sShowField.length > 0) {
                var sShwFlds = sShowField.split('/');
                for (var x = 0; x < sShwFlds.length; x++) {
                    var childObj1 = $('[name="' + sShwFlds[x] + '"]');
                    $(childObj1).removeClass('on').addClass('off');
                    for (var section in oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections) {
                        if (sShwFlds[x] == oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sCode) {
                            if (bIsChecked) {
                                oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "off";
                            }
                            else {
                                oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "on";
                                $(childObj1).removeClass('off').addClass('on');
                            }
                        }
                    }
                }
            }
            if (sHideField && sHideField.length > 0) {
                var sHidflds = sHideField.split('/');
                for (var s = 0; s < sHidflds.length; s++) {
                    var childObj2 = $('[name="' + sHidflds[s] + '"]');
                    $(childObj2).removeClass('off').addClass('on');
                    for (var section in oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections) {
                        if (sHidflds[s] == oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sCode) {
                            oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "on";
                        }
                    }
                }
                //Find Form elements and Set Null Values
                var SetValueToNull = $(childObj2).find('form');
                if (SetValueToNull) {
                    //$(childObj2).find('form').find('input').each(function(a,b){ $(b).attr('value',"") });
                    $(childObj2).find('form').find('select').each(function (a, b) { $(b).find('option[value="0"]').prop('selected', true) })
                }
            }
        }
        else if (iSHType == 30) {
            if (sShowField && sShowField.length > 0) {
                var childObj1 = $('[name="' + sShowField + '"]');
                $(childObj1).removeClass('on');
                var dicSteps = oQSInstance.QSDefinition.Steps;
                for (var key in dicSteps) {
                    var singlestep = dicSteps[key];
                    if (singlestep.sCode == sShowField) {
                        singlestep.sIsHidden = "off";
                    }
                }
            }
            if (sHideField && sHideField.length > 0) {
                var childObj2 = $('[name="' + sHideField + '"]');
                $(childObj2).addClass('on');
                var dicSteps = oQSInstance.QSDefinition.Steps;
                for (var key in dicSteps) {
                    var singlestep = dicSteps[key];
                    if (singlestep.sCode == sHideField) {
                        singlestep.sIsHidden = "on";
                    }
                }
            }
        }
    }
}

function fncHideShowBOFromHTMLTree(parentName, childObj, Type) {
    if (childObj) {
        var ActiveInstanceID = 0;
        var testObj = childObj.parentNode;
        var count = 1;
        if (testObj) {
            if (testObj.className != "questionset-section") {//This Codition without ParentNode
                while (testObj.getAttribute('data-controltype') != parentName) {// This Condition With ParentNode
                    if (testObj.parentNode.tagName != "HTML") {
                        testObj = testObj.parentNode;
                        count++;
                    }
                    else {
                        return ActiveInstanceID;
                    }
                }
            } else {
                testObj = childObj;
            }
        }
        if (Type == "show") {
            $(testObj).removeClass('on').addClass('off');
        }
        else if (Type == "hide") {
            $(testObj).find('select').val('');
            $(testObj).removeClass('off').addClass('on');
        }
    }
}
function fncRemoveSection($this, iSectionID, iComponentID) {
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var pvalue = {
        sSectionID: iSectionID,
        iXIComponentID: iComponentID,
        sGUID: sGUID,
    }
    $.ajax({
        url: RemoveSectionURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        data: JSON.stringify(pvalue),
        success: function (data) {
            if (iComponentID == 3) {
                var RefreshBtn = $('#Component-' + iSectionID).find('#hiderefresh');
                if (RefreshBtn && RefreshBtn.length > 0) {
                    $(RefreshBtn[0]).click();
                }
                else {
                    RefreshBtn = $('#Component-' + iSectionID).find('.hideref-' + data.BOID);
                    if (RefreshBtn && RefreshBtn.length > 0) {
                        $(RefreshBtn[0]).click();
                    }
                }
            }
        }
    });
}
function fnccheckme($this) {
    var bIsRemoveDependency;
    var DependencyType;
    var sShowField; var sHideField; var iSHType; var parentName; var bIsChecked = false;
    if ($this) {
        var _sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
        var oQSInstance = QSInsDict[_sGUID];
        var DependentField = "";
        if ($this.id && $this.id.length > 0) {
            DependentField = $('#Field-' + $this.id.split('-')[1]).val();
        }
        var tagname = $this.tagName;
        if (tagname == "SELECT") {
            sShowField = $($this).find(':selected').attr('data-show');
            sHideField = $($this).find(':selected').attr('data-hide');
            iSHType = $($this).find(':selected').attr('data-shtype');
            bIsRemoveDependency = $($this).find(':selected').attr('data-isRemoveDependency');
            DependencyType = $($this).find(':selected').attr('data-dependencytype');
            bIsChecked = true;
            if (DependentField != "" && $("#" + $this.id + " option:selected").val().toLowerCase() == "se") {
                $("#select-" + DependentField).val("true");
                sShowField = "ABC";
                iSHType = "20";
            }
            else {
                if (DependentField == 1362) {
                    $("#select-" + DependentField).val("");
                    sShowField = "";
                    iSHType = "";
                }
            }
        }
        else {
            sShowField = $($this).attr('data-show');
            sHideField = $($this).attr('data-hide');
            iSHType = $($this).attr('data-shtype');
            bIsRemoveDependency = $($this).attr('data-isRemoveDependency');
            DependencyType = $($this).attr('data-dependencytype');
            if (tagname == "OPTION") {
                bIsChecked = true;
            }
            else {
                if ($($this).is(':checked')) {
                    bIsChecked = true;
                }
            }
        }
        if (iSHType == 10) {
            parentName = 'qsfield';
            if (sShowField && sShowField.length > 0) {
                var childObj1 = $('[name="' + sShowField + '"]');
                fncHideShowFromHTMLTree(parentName, childObj1[0], 'show');
            }
            if (sHideField && sHideField.length > 0) {
                var childObj2 = $('[name="' + sHideField + '"]');
                fncHideShowFromHTMLTree(parentName, childObj2[0], 'hide');

                //Hide dependecy Fields recursively
                var HideRemaining = $('.questionset-section select[name="' + sHideField + '"]');
                if (HideRemaining) {
                    $(HideRemaining).find('option[value="0"]').prop('selected', true);//set DropDwon Value To 0
                    $(HideRemaining).find('option[value=""]').prop('selected', true);
                    fnccheckme(HideRemaining[0]);
                }
                var HideInputField = $('.questionset-section input[name="' + sHideField + '"]');
                if (HideInputField) {
                    var ISMerge = $(HideInputField).attr("data-Merge");
                    if (ISMerge != "" && ISMerge == "True") {
                        var MergeVal = $(HideInputField).attr("data-MergeValue");
                        $('[name="' + sHideField + '"]').val(MergeVal);
                    }
                    else {
                        $('[name="' + sHideField + '"]').val("");
                    }
                }
            }
        }
        else if (iSHType == 20) {
            if (sShowField && sShowField.length > 0) {
                var sShwFlds = sShowField.split('/');
                for (var x = 0; x < sShwFlds.length; x++) {
                    var childObj1 = $('[name="' + sShwFlds[x] + '"]');
                    if (bIsRemoveDependency == "True") {
                        var iSectionId = $('[name="' + sShwFlds[x] + '"]').attr('data-value');
                        if (iSectionId) {
                            var section = iSectionId + "_Sec";
                            var iXIComponentID = oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].iXIComponentID;
                            fncRemoveSection($this, iSectionId, iXIComponentID);
                        }
                    }
                    $(childObj1).removeClass('on').addClass('off');
                    for (var section in oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections) {
                        if (sShwFlds[x] == oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sCode) {
                            if (bIsChecked) {
                                oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "off";
                            }
                            else {
                                oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "on";
                                $(childObj1).removeClass('off').addClass('on');
                            }
                        }
                    }
                }
            }
            if (sHideField && sHideField.length > 0) {
                var sHidflds = sHideField.split('/');
                for (var s = 0; s < sHidflds.length; s++) {
                    var childObj2 = $('[name="' + sHidflds[s] + '"]');
                    if (bIsRemoveDependency == "True") {
                        if (DependencyType == 10) {
                            var iSectionId = $('[name="' + sHidflds[s] + '"]').attr('data-value');
                            if (iSectionId) {
                                var section = iSectionId + "_Sec";
                                var iXIComponentID = oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].iXIComponentID;
                                var forms = $(childObj2).find('form');
                                var iID = 0;
                                var iCount = 1;
                                $(childObj2).find('form').each(function (a, b) {
                                    if (iCount < forms.length) {
                                        iID = $(this).find('input[name=id]').val();
                                        if (!iID) {
                                            iID = 0;
                                        }
                                        var ident = "Popup_" + sBON + "_" + iID;
                                        if (iID > 0) {
                                            var sBON = $(this).attr('data-sbo');
                                            var ident = "Popup_" + sBON + "_" + iID;
                                            $('#' + ident).remove();
                                        }
                                        else {
                                            $(this).find('input[name=id]').val('');
                                            $(this).find('.StatusMessages').remove();
                                        }
                                    }
                                    else {
                                        iID = $(this).find('input[name=id]').val();
                                        if (!iID) {
                                            iID = 0;
                                        }
                                        var ident = "Popup_" + sBON + "_" + iID;
                                        if (iID > 0) {
                                            var sBON = $(this).attr("data-sbo");
                                            var ident = "Popup_" + sBON + "_" + iID;
                                            $('#' + ident).find('input[name=id]').val('');
                                            $('#' + ident).attr("data-instanceid", "");
                                            $('#' + ident).attr("id", "Popup_" + sBON + "_0");
                                            //$('#' + ident).remove();
                                        }
                                        else {
                                            $(this).find('input[type="hidden"]').val('');
                                            $(this).find('.StatusMessages').remove();
                                        }
                                    }
                                    iCount++;
                                });
                                if (iID > 0) {
                                    fncRemoveSection($this, iSectionId, iXIComponentID);
                                }
                            }
                        }
                        else {
                            var iSectionId = $('[name="' + sHidflds[s] + '"]').attr('data-value');
                            if (iSectionId) {
                                var section = iSectionId + "_Sec";
                                var iXIComponentID = oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].iXIComponentID;
                                fncRemoveSection($this, iSectionId, iXIComponentID);
                                $('.additionaldrive').remove();
                            }
                        }
                    }
                    $(childObj2).removeClass('off').addClass('on');
                    for (var section in oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections) {
                        if (sHidflds[s] == oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sCode) {
                            oQSInstance.QSDefinition.Steps[oQSInstance.sCurrentStepName].Sections[section].sIsHidden = "on";
                        }
                    }


                    //Find Form elements and Set Null Values
                    var SetValueToNull = $(childObj2).find('form');
                    if (SetValueToNull) {
                        //$(childObj2).find('form').find('input').each(function(a,b){ $(b).attr('value',"") });
                        $(childObj2).find('form').find('select').each(function (a, b) {
                            $(b).find('option[value="0"]').prop('selected', true)
                            $(b).find('option[value=""]').prop('selected', true)
                            fnccheckme(b[0]);
                        })
                        $(childObj2).find('form').find('input[type=text]').each(function (a, b) {
                            var ISMerge = $(b).attr("data-Merge");
                            if (ISMerge != "" && ISMerge == "True") {
                                var MergeVal = $(b).attr("data-MergeValue");
                                $('[name="' + b.name + '"]').val(MergeVal);
                            }
                            else {
                                $(b).val("");
                            }
                        })
                    }
                }
            }
        }
        else if (iSHType == 30) {
            if (sShowField && sShowField.length > 0) {
                var childObj1 = $('[name="' + sShowField + '"]');
                $(childObj1).removeClass('on');
                var dicSteps = oQSInstance.QSDefinition.Steps;
                for (var key in dicSteps) {
                    var singlestep = dicSteps[key];
                    if (singlestep.sCode == sShowField) {
                        singlestep.sIsHidden = "off";
                    }
                }
                //for (var m = 0; m < QuestionSet.QSDefinition.QSSteps.length; m++) {
                //    if (QuestionSet.QSDefinition.QSSteps[m].sCode == sShowField) {
                //        QuestionSet.QSDefinition.QSSteps[m].sIsHidden = "off";
                //        break;
                //    }
                //}
            }
            if (sHideField && sHideField.length > 0) {
                var childObj2 = $('[name="' + sHideField + '"]');
                $(childObj2).addClass('on');
                var dicSteps = oQSInstance.QSDefinition.Steps;
                for (var key in dicSteps) {
                    var singlestep = dicSteps[key];
                    if (singlestep.sCode == sHideField) {
                        singlestep.sIsHidden = "on";
                    }
                }
                //for (var m = 0; m < QuestionSet.QSDefinition.QSSteps.length; m++) {
                //    if (QuestionSet.QSDefinition.QSSteps[m].sCode == sHideField) {
                //        QuestionSet.QSDefinition.QSSteps[m].sIsHidden = "on";
                //        break;
                //    }
                //}
            }
        }
    }
}
var sValidateAttr = [];
var bPostHideSaveandNext = "no";
var ValidationType = 0;
var bIsInternal = false;
var sQSType = "";
var sGUID = "";
function fncRunQSStepAction(Type, $this) {
debugger
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var oQSInstance = QSInsDict[sGUID];
    bPostHideSaveandNext = $($this).attr("data-isPostHidden");
    if (bPostHideSaveandNext && bPostHideSaveandNext.length > 0) {

    }
    else {
        bPostHideSaveandNext = "no";
    }
    if (oQSInstance.sQSType != null && oQSInstance.sQSType.toLowerCase() == "internal") {
        sQSType = oQSInstance.sQSType;
        bIsInternal = true;
    }
    sValidateAttr = [];
    $('.LayoutCode_' + sGUID).find('#QSNxtBtn').hide();
    $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').show();
    var iQSIID = oQSInstance.XIGUID;
    CurrentStepID = oQSInstance.iCurrentStepIDXIGUID;
    if (oQSInstance.QSDefinition.sName.toLowerCase() == "compare the market internal") {
        ValidationType = 10;
    }
    else if (oQSInstance.QSDefinition.sName.toLowerCase() == "go compare motorhome") {
        ValidationType = 20;
    }
    else if (oQSInstance.QSDefinition.sName.toLowerCase() == "money super market motorhome") {
        ValidationType = 20;
    }

    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var FParent = $($this).parent();
    var SParent = $(FParent).parent();
    var sformid = $(SParent).siblings("div.conQSStep").find('form').attr('id');
    fncMapAndValidate(sGUID, Type, sformid, $this)
}

function fncQsSaveAndQSLink(ReportID, GroupID, BOID, CreateXILinkID, InstanceID, $This) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    var bIsLoading = $($This).attr("data-IsLoading");
    sValidateAttr = [];
    fncMapAndValidate(sGUID, 'Save').then(function (state, callback) {
        if (sValidateAttr == undefined || sValidateAttr.length == 0) {
            var sType = $($This).text();
            if (sType == "Continue") {
                var sFormiID = fncGetFormIIDFromHTMLTree($This);
                //var sForm = $($This).parent().parent().find('form')[0];
                //var sFormiID = $(sForm).attr('data-instanceid');
                var sInstanceID = 0; var iCount = 0;
                //if (sFormiID) {
                //    sInstanceID = sFormiID.split('_')[2];
                //    iCount = sFormiID.split('_').length;
                //}
                if (sFormiID == undefined || sFormiID == "0") {
                    $(".Confirmation").dialog({
                        title: "Confirmation",
                        autoOpen: true,
                        modal: true,
                        width: 400,
                        left: '600px',
                        top: '80px',
                        buttons: [
                            {
                                text: "Yes",
                                click: function () {

                                    $(".Confirmation").dialog('close');
                                    $(".Confirmation").dialog('destroy');
                                    if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                                        $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                                    }
                                    fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID, null, $This);
                                    //if (CreateXILinkID == '5459') {
                                    //    XIRun(CreateXILinkID, 0, sGUID, null, false);
                                    //}
                                    //else {
                                    //    fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                                    //}

                                }
                            },
                            {
                                text: "No",
                                click: function () {

                                    $(".Confirmation").dialog('close');
                                    $(".Confirmation").dialog('destroy');
                                    //fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                                }
                            }
                        ]
                    });
                }
                else {
                    if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                        $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                    }
                    fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID, null, $This);
                }
            }
            else {
                if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                    $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                }
                fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID, null, $This);
            }
        }
    });

}
function fncLoadFPComponent($This) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    var LoginType = $($This).attr("data-type");
    var AccountC = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
    var AccountType = $(AccountC).closest('form').attr("data-AccountType");
    var formData = JSON.parse(JSON.stringify(jQuery('.AccountComponent').serializeArray()));
    var UserName = "";
    if (formData && formData.length > 0) {
        for (var k = 0; k < formData.length; k++) {
            if (formData[k].name == "UserName") {
                UserName = formData[k].value;
            }
        }
    }
    fncSetParam(UserName, sGUID, "{XIP|sEmail}", "").then(function (state, callback) {
        fncSetParam(LoginType, sGUID, "{XIP|sLoginType}", "").then(function (state, callback) {
            if (AccountType == "login") {
                $.ajax({
                    url: GetApplicationLayoutURL,
                    type: "Post",
                    contentType: "application/json; charset=utf-8",
                    datatype: "json",
                    cache: false,
                    async: true,
                    //data: "",
                    success: function (data) {
                        //if (!data && data.oResult)
                        for (var i = 0; i < data.oResult.LayoutDetails.length; i++) {
                            $('#' + data.oResult.LayoutDetails[i].PlaceholderUniqueName).html("");
                        }
                        ShowContentInDialogOrPopup(data.oResult, sGUID);
                    }
                })
            }
            else {
                var oQSInstance = QSInsDict[sGUID];
                var CurrentStepName = oQSInstance.sCurrentStepName;
                var StepIns = oQSInstance.Steps[CurrentStepName];
                fncGetNextStep(StepIns, sGUID, StepIns.bIsLastStep);
            }
            //fncRunQSStepAction('SaveNext', $This);
        });
    });
}
function fncLoadLoginComponent($This) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    var LoginType = $($This).attr("data-type");
    //var oQSInstance = QSInsDict[sGUID];
    //var CurrentStepName = oQSInstance.sCurrentStepName;
    //var StepIns = oQSInstance.Steps[CurrentStepName];
    var AccountC = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
    var formid = $(AccountC).closest('form').attr('id');
    var IsTwoWay = $(AccountC).closest('form').attr('data-Twoway');
    var IsSMS = $(AccountC).closest('form').attr('data-IsSMS');
    var IsEmail = $(AccountC).closest('form').attr('data-IsEmail');
    var OTPCase = $(AccountC).closest('form').attr('data-OTPCase');
    var OTPLength = $(AccountC).closest('form').attr('data-OTPLength');
    var OTPType = $(AccountC).closest('form').attr('data-OTPType');
    var AccountType = $($This).attr("data-AccountType");
    var UserName = "";
    var password = "";
    var ConfirmPWD = "";
    //var Form = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
    var formData = JSON.parse(JSON.stringify(jQuery('.AccountComponent').serializeArray()));
    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', Form);
    var FormType = $('div.LayoutCode_' + sGUID).find('.AccountComponent').attr('data-FormType');
    var oParams = [];
    var Param1 = {};
    Param1["sName"] = "IsTwoWay";
    Param1["sValue"] = IsTwoWay;
    oParams.push(Param1);
    var Param2 = {};
    Param2["sName"] = "bsms";
    Param2["sValue"] = IsSMS;
    oParams.push(Param2);
    var Param3 = {};
    Param3["sName"] = "bmail";
    Param3["sValue"] = IsEmail;
    oParams.push(Param3);
    var Param4 = {};
    Param4["sName"] = "iotpcase";
    Param4["sValue"] = OTPCase;
    oParams.push(Param4);
    var Param5 = {};
    Param5["sName"] = "iotplength";
    Param5["sValue"] = OTPLength;
    oParams.push(Param5);
    var Param6 = {};
    Param6["sName"] = "iotptype";
    Param6["sValue"] = OTPType;
    oParams.push(Param6);
    if (formData && formData.length > 0) {
        for (var k = 0; k < formData.length; k++) {
            if (formData[k].name == "UserName") {
                UserName = formData[k].value;
            }
            if (formData[k].name == "Password") {
                password = formData[k].value;
            }
            if (formData[k].name == "ConfirmPassword") {
                ConfirmPWD = formData[k].value;
            }
            if (formData[k].name == "ConfirmPassword") {
                ConfirmPWD = formData[k].value;
            }
        }
    }
    fncSetParam(UserName, sGUID, "{XIP|sEmail}", "").then(function (state, callback) {
        fncSetParam(LoginType, sGUID, "{XIP|sLoginType}", "").then(function (state, callback) {
            if (FormType == "Login") {
                $.ajax({
                    url: LoginAccountComponentURL,
                    type: "Post",
                    contentType: "application/json; charset=utf-8",
                    datatype: "json",
                    cache: false,
                    async: true,
                    data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: AccountType, oParams: oParams }),
                    success: function (data) {
                        //if (!data && data.oResult)
                        for (var i = 0; i < data.oResult.LayoutDetails.length; i++) {
                            $('#' + data.oResult.LayoutDetails[i].PlaceholderUniqueName).html("");
                        }
                        ShowContentInDialogOrPopup(data.oResult, sGUID);
                        //if (data != null && data != "Failure") {
                        //    //fncSetParam(data, sGUID, sName, "");
                        //    if (data.xiStatus == 30) {

                        //    }
                        //}
                        //else {
                        //    FinalValid = false;
                        //    $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                        //    $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                        //    $('#' + formid).find(".StatusMessages").hide();
                        //    $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                        //    $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Error</div>');
                        //}
                    }
                });
            }
            //fncGetNextStep(StepIns, sGUID, StepIns.bIsLastStep);
            //fncRunQSStepAction('SaveNext', $This);
        });
    });
}
function fncQsCancel(BOID, CreateXILinkID, InstanceID, $This) {
    var bIsLoading = $($This).attr("data-IsLoading");
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    var sType = $($This).text();
    var sFormiID = fncGetFormIIDFromHTMLTree($This);
    var sFormType = fncGetFormDataTypeFromHTMLTree($This);
    if (sFormType != "" && sFormType != undefined && sFormType.toLowerCase() == "Create".toLowerCase()) {
        $(".CancelConfirmation").dialog({
            title: "Cancel Confirmation",
            autoOpen: true,
            modal: true,
            width: 400,
            left: '600px',
            top: '80px',
            buttons: [
                {
                    text: "Yes",
                    click: function () {

                        $(".CancelConfirmation").dialog('close');
                        $(".CancelConfirmation").dialog('destroy');
                        if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                            $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                        }
                        XIRun(null, CreateXILinkID, 0, sGUID, null, false, 0);
                        //fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                    }
                },
                {
                    text: "No",
                    click: function () {

                        $(".CancelConfirmation").dialog('close');
                        $(".CancelConfirmation").dialog('destroy');
                        //fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                    }
                }
            ]
        });
    }
    else {
        if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
            $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
        }
        XIRun(null, CreateXILinkID, 0, sGUID, null, false, 0);
    }
}

function SaveMultipleRows() {
    var NVPairs = [];
    var OneClickID = "";
    var BOName = "";
    var flag = "";
    $('.NameValuePairs1').each(function (i, section) {

        var Hidden = $(section).find('input[type=hidden]').val();
        if (Hidden != "") {
            var HiddenSplit = Hidden.split('+');
            OneClickID = HiddenSplit[0];
            BOName = HiddenSplit[1];
            sGUID = HiddenSplit[2];
        }
        $('.NVPairs1').each(function (i, item) {

            var sString = "";
            var value = "";
            var DateType = "";
            var ExposedCheckedBox = "";
            var Input = $(item).find('input[type=text]').val();
            var Hiddenval = $(item).find('input[type=hidden]').val();
            if (Hiddenval == "flag") {
                flag = "1";
            }
            var CheckBoxval = $(item).find('input[type=checkbox]').is(':checked');

            var ExposedCheckBoxval = $(item).find('input[name=ExposedCheck]').is(':checked'); //$(item).find('input[type=checkbox]').is(':checked');
            var DatePickerWhere = $(item).find("#DatePickerWhere").val();
            var SelectArray = $(item).find('select');
            $(SelectArray).each(function (i, select) {
                if (i == 0) {
                    value = $(select).val();
                }
                else if (i == 1) {
                    sString = $(select).val();
                }
                else if (Input == undefined || Input == "0") {
                    Input = $(select).val();
                }
                else {
                    DateType = $(select).val();
                }
                if (DateType != "") {
                    //sString = sString + '#' + DateType;
                    Input = DateType;
                }
                if (Hiddenval != undefined && Hiddenval != "flag" && Hiddenval != "") {
                    Input = Hiddenval
                }
                if (CheckBoxval == true && Input == undefined) {
                    Input = "1";
                }
                else if (ExposedCheckBoxval == false && Input == undefined) {
                    Input = "0";
                }
                if (ExposedCheckBoxval == true) {
                    ExposedCheckedBox = "1";
                }
                else if (ExposedCheckBoxval == false) {
                    ExposedCheckedBox = "0";
                }
            })
            if (DatePickerWhere != undefined && DatePickerWhere != "") {
                Input = Input + "#" + DatePickerWhere;
            }
            if (sString == "") {
                NVPairs.push(value);
            }
            else {
                NVPairs.push(value + "^" + sString + "^" + Input + "^" + ExposedCheckedBox);
                if (sString == "ASC" || sString == "DESC") {
                    flag = "0";
                }
            }
        });
    });
    var model = { OneClickID: OneClickID, BOName: BOName, NVPairs: NVPairs, flag: flag };
    var skey = "Definition_oneclick_" + OneClickID;
    var Type = 'Application';
    $.ajax({
        url: SaveMultiRowURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        cache: false,
        async: false,
        data: JSON.stringify(model),
        success: function (data) {
            CustomMessage(data.ResponseMessage, data.Status);
            $.ajax({
                type: 'POST',
                url: '/XIApplications/RemoveCacheByKey',
                data: { skey: skey, cachetype: Type },
                cache: false,
                async: false,
                dataType: 'json',
                success: function (data) {
                    // trigger table once after removing the data

                },
                error: function (response) {
                }
            });
            //opener.window.location.reload();
        }
    });

}
function fnc1ClickBtn($this, XiLinkID, ID, sGUID, BO, IsMerge, BODID, iQSDID, MenuName, InputParams, bISActivity) {
    bButton = true;
    var tr = $($this).closest('tr');
    var tddata = [];
    $(tr).children('td').each(function (ii, vv) {
        tddata[ii] = $(this).text();
    });
    var RowData = tddata;
    var id = RowData[0];
    var InputParams = [];
    var param4 = {};
    param4["sName"] = '{XIP|iInstanceID}';
    param4["sValue"] = id;
    InputParams.push(param4);
    XIRun($this, XiLinkID, ID, sGUID, BO, IsMerge, BODID, iQSDID, MenuName, InputParams, bISActivity);
}

function fncMapAndValidate(sGUID, Type, formid, $this) {
    return new Promise(function (resolve, reject) {
        var FieldValues = [];
        var bIsValid = true;
        var oQSInstance = QSInsDict[sGUID];
        var CurrentStepName = oQSInstance.sCurrentStepName;
        var StepIns = oQSInstance.Steps[CurrentStepName];
        StepDef = oQSInstance.QSDefinition.Steps[CurrentStepName];
        FinalValid = true;
        var formData = "";
        if (formid && StepDef.sCode == "POSTTRAN") {
            formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
        }
        else {
            formid = 'QSStepForm';
            formData = JSON.parse(JSON.stringify(jQuery('div.LayoutCode_' + sGUID).find('.' + formid).serializeArray()));
        }
        var i, j, titleid = [];
        for (var i = 0; i < formData.length; i++) {
            var FieldName = formData[i].name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == formData[i].name) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var result = FieldValues.filter(function (x) { return x.Label === FieldName; });

            //if (FieldValues.find(m => m.Label == formData[i].name)) { ////IE fix
            if (result && result.length > 0) {
                //FieldValues.find(m => m.Label == formData[i].name).Data = formData[i].value; ////IE Fix
                var sfield = FieldValues.filter(function (x) { return x.Label === FieldName; });
                if (sfield && sfield.length > 0) {
                    sfield[0].Data = formData[i].value;
                }
            }
            else {
                var ddlText = $('select[name="' + FieldName + '"] option:selected').text();
                FieldValues.push({ Label: FieldName, Data: formData[i].value, sDerivedValue: ddlText });
            }
        }
        //var Radioserialized = $('.' + formid).find('input[type=radio]').map(function () {
        //    if (this.name != null && this.name != "") {
        //        var field = this.name.split('_')[1];
        //        return { name: field, value: this.value, DerivedValue: this.value = "10" ? "Yes" : "No" };
        //    }

        //});
        var serialized = $('.' + formid).find('input[type=checkbox]').map(function () {
            var FieldName = this.name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == this.name) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            return { name: FieldName, value: this.checked ? "true" : "false", DerivedValue: this.checked ? "True" : "False" };
        });
        var ddlserialized = $('.' + formid).find('select').map(function () {
            var FieldName = this.name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == this.name) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            return { name: FieldName, value: this.value, DerivedValue: $(this).find('option:selected').text() };
        });
        var MultipleDDLserialized = $('.' + formid).find('select[multiple]').map(function () {
            var FieldName = this.name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == this.name) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var SelectedText = $(this).find('option:selected').map(function () {
                return $(this).text();
            }).get().join(',');
            var SelectedValues = $(this).find('option:selected').map(function () {
                return $(this).val();
            }).get().join(',');
            return { name: FieldName, value: SelectedValues/*$(this).val().join(',')*/, DerivedValue: SelectedText };
        })
        var AutoCompleteData = $('.' + formid).find('input[type=text].autocomplete').map(function () {
            var fldid = $(this).attr('id');
            var id = fldid.replace('FieldShow', 'Field');
            var sFldName = $('#' + id).attr('name');
            //var FieldName = this.name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == sFldName) {
                            sFldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var sFldVal = $('#' + id).val();
            var sFldDrvdVal = $('#' + fldid).val();
            return { name: sFldName, value: sFldVal, DerivedValue: sFldDrvdVal };
        });
        for (var i = 0; i < serialized.length; i++) {
            var FieldName = serialized[i].name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == serialized[i].name) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var result = FieldValues.filter(function (x) { return x.Label === FieldName; });

            //if (FieldValues.find(m => m.Label == serialized[i].name)) {////IE fix
            if (result && result.length > 0) {
                //FieldValues.find(m => m.Label == serialized[i].name).Data = serialized[i].value; ////IE fix
                if (FieldValues.filter(function (x) { return x.Label === FieldName; })[0]) {
                    FieldValues.filter(function (x) { return x.Label === FieldName; })[0].Data = serialized[i].value;
                    FieldValues.filter(function (x) { return x.Label === FieldName; })[0].sDerivedValue = serialized[i].DerivedValue;
                }
            }
            else {
                FieldValues.push({ Label: FieldName, Data: serialized[i].value, sDerivedValue: serialized[i].DerivedValue });
            }
        }
        for (var i = 0; i < ddlserialized.length; i++) {
            var FieldName = ddlserialized[i].name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == FieldName) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var result = FieldValues.filter(function (x) {
                if (x.Label === FieldName) {
                    x.sDerivedValue = ddlserialized[i].DerivedValue;
                }
                return x.Label === FieldName
            })
            if (result.length > 0) {

            }
            else {
                FieldValues.push({ Label: FieldName, Data: ddlserialized[i].value, sDerivedValue: ddlserialized[i].DerivedValue });
            }
        }
        for (var i = 0; i < MultipleDDLserialized.length; i++) {
            var FieldName = MultipleDDLserialized[i].name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == FieldName) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var result = FieldValues.filter(function (x) {
                if (x.Label === FieldName) {
                    x.Data = MultipleDDLserialized[i].value;
                    x.sDerivedValue = MultipleDDLserialized[i].DerivedValue;
                }
                return x.Label === FieldName
            })
            if (result.length > 0) {

            }
            else {
                FieldValues.push({ Label: FieldName, Data: MultipleDDLserialized[i].value, sDerivedValue: MultipleDDLserialized[i].DerivedValue });
            }
        }
        for (var i = 0; i < AutoCompleteData.length; i++) {
            var FieldName = AutoCompleteData[i].name;
            if (bUIDRef == "true" || bUIDRef == "True") {
                for (var sdid in StepDef.Sections) {
                    for (var FieldD in StepDef.Sections[sdid].FieldDefs) {
                        if (StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.XIGUID.toString() == FieldName) {
                            FieldName = StepDef.Sections[sdid].FieldDefs[FieldD].FieldOrigin.sName;
                        }
                    }
                }
            }
            var result = FieldValues.filter(function (x) {
                return x.Label === FieldName;
            });

            //if (FieldValues.find(m => m.Label == serialized[i].name)) {////IE fix
            if (result && result.length > 0) {
                //FieldValues.find(m => m.Label == serialized[i].name).Data = serialized[i].value; ////IE fix
                if (FieldValues.filter(function (x) { return x.Label === FieldName; })[0]) {
                    FieldValues.filter(function (x) { return x.Label === FieldName; })[0].Data = AutoCompleteData[i].value;
                    FieldValues.filter(function (x) { return x.Label === FieldName; })[0].sDerivedValue = AutoCompleteData[i].DerivedValue;
                }
            }
            else {
                FieldValues.push({ Label: FieldName, Data: AutoCompleteData[i].value, sDerivedValue: AutoCompleteData[i].DerivedValue });
            }
        }
        //for (var i = 0; i < Radioserialized.length; i++) {
        //    var result = FieldValues.filter(function (x) {
        //        if (x.Label === Radioserialized[i].name) {
        //            x.sDerivedValue = Radioserialized[i].DerivedValue;
        //        }
        //        return x.Label === Radioserialized[i].name
        //    })
        //    if (result.length > 0) {

        //    }
        //    else {
        //        FieldValues.push({ Label: Radioserialized[i].name, Data: Radioserialized[i].value, sDerivedValue: Radioserialized[i].DerivedValue });
        //    }
        //}
        if (StepDef.Sections && Object.keys(StepDef.Sections).length > 0) {
            var SecIns; var oDataIdentifiers = [];
            for (var sdid in StepDef.Sections) {
                if (StepDef.Sections[sdid].sIsHidden == "off" || StepDef.Sections[sdid].sIsHidden == "") {
                    for (var siid in StepIns.Sections) {
                        if (sdid == siid) {
                            SecIns = StepIns.Sections[siid];
                            SecDef = StepDef.Sections[sdid];
                        }
                    }
                    if (SecDef.iDisplayAs == 30) {
                        for (var j = 0; j < FieldValues.length; j++) {
                            var FieldDefID = FieldValues[j].Label;
                            var XiValue = SecIns.XIValues[FieldDefID];
                            if (XiValue) {
                                XiValue.sValue = FieldValues[j].Data;
                                XiValue.sDerivedValue = FieldValues[j].sDerivedValue;
                                SecIns.XIValues[FieldDefID] = XiValue;
                                var FldOrg = SecDef.FieldDefs[FieldDefID];
                                if (bUIDRef == "true" || bUIDRef == "True") {
                                    fncGetValidationMessage(StepDef.XIGUID.toString(), FldOrg.FieldOrigin, FieldValues[j].Data, FieldValues, sGUID)
                                }
                                else {
                                    fncGetValidationMessage(StepIns.FKiQSStepDefinitionID, FldOrg.FieldOrigin, FieldValues[j].Data, FieldValues, sGUID)
                                }
                            }
                        }
                        StepIns.Sections[SecIns.FKiStepSectionDefinitionIDXIGUID + "_Sec"] = SecIns;
                    }
                    else {
                        if (SecDef.iDisplayAs == 40) {
                            //SaveComponent(sGUID, SecDef.sName );
                            if (StepDef.sCode == "POSTTRAN") {
                                var Param = {};
                                for (var j = 0; j < FieldValues.length; j++) {
                                    Param = {};
                                    Param["sName"] = FieldValues[j].Label;
                                    Param["sValue"] = FieldValues[j].Data;
                                    SecIns.FormValues.push(Param);
                                }
                            }
                            else {
                                var ComponentID = SecDef.iXIComponentID;
                                if (ComponentID == 2 || ComponentID == 13 || ComponentID == 3 || ComponentID == 19) {
                                    $('#Component-' + SecDef.ID).find('form').each(function (a, b) {
                                        var savetype = $(this).attr('data-savetype');
                                        var sBO = $(this).attr('data-sbo');
                                        if (savetype == "yes") {
                                            if (ComponentID == 13) {
                                                SaveEditBOURL = SaveGridURL;
                                            }
                                            var formid = $(this).attr('id');
                                            var sDependencyIdentifier = $('#' + formid).attr("data-DependencyIdentifier");
                                            var sDependencyIdentifierValue = "";
                                            if (sDependencyIdentifier && InputParams) {
                                                for (var len = 0; len < InputParams.length; len++) {
                                                    if (InputParams[len].sName == sDependencyIdentifier) {
                                                        sDependencyIdentifierValue = InputParams[len].sValue;
                                                    }
                                                }
                                            }
                                            var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
                                            if (formData && formData.length > 0) {
                                                var FormValues = [];
                                                var titleid = [];
                                                formData = processFormData(formData, formid);
                                                $.each(formData, function (i, option) {
                                                    if (option.bIsValid == "false") {
                                                        bIsValid = false;
                                                    }
                                                    //if (formData[i].name.indexOf('-editor') > 0) {
                                                    //    if (tinyMCE.get(formid.split('_')[1] + '_' + formData[i].name) != null) {
                                                    //        var mcetext = tinyMCE.get(formid.split('_')[1] + '_' + formData[i].name).getContent();
                                                    //        var sFName = formData[i].name.split('-')[0];
                                                    //        FormValues.push({ sName: sFName, sValue: mcetext, bDirty: true });
                                                    //    }
                                                    //}
                                                    if (FormValues[j].sName.indexOf('-editor') > 0) {
                                                        var editorId = sFormGuid + '_' + FormValues[j].sName;
                                                        var editorInstance = tinyMCE.get(editorId);
                                                        if (editorInstance) {
                                                            var mcetext = editorInstance.getContent();
                                                            var sFName = FormValues[j].sName.split('-')[0];
                                                            FormValues.push({ sName: sFName, sValue: mcetext, bDirty: true });
                                                        } else {
                                                            console.warn('TinyMCE editor not found for ID:', editorId);
                                                        }
                                                    }
                                                });
                                                if (sDependencyIdentifierValue) {
                                                    formData.filter(function (x) {
                                                        if (x.name.toLowerCase() === "id") {
                                                            x.value = sDependencyIdentifierValue;
                                                        }
                                                        //return x.sName === serialized[i].name
                                                    })
                                                }
                                                var j = 0;
                                                for (var i = 0; i < formData.length; i++) {
                                                    if (val == formData[i].name) {
                                                        if (formData[i].value != "") {
                                                            j++;
                                                            FormValues[i - j].sValue = formData[i].value;
                                                        }
                                                        else { formData[i].name = ""; }
                                                    }
                                                    else {
                                                        FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
                                                    }
                                                    var val = formData[i].name;
                                                }
                                                var ddlserialized = $('#' + formid).find('select').map(function () {
                                                    return { name: this.name, value: this.value, DerivedValue: $(this).find('option:selected').text() };
                                                });
                                                for (var i = 0; i < ddlserialized.length; i++) {
                                                    var result = FormValues.filter(function (x) {
                                                        return x.sName === ddlserialized[i].name
                                                    })
                                                    if (result.length > 0) {

                                                    }
                                                    else {
                                                        FormValues.push({ sName: ddlserialized[i].name, sValue: ddlserialized[i].value, sPreviousValue: ddlserialized[i].sPreviousValue, bDirty: true });
                                                    }
                                                }
                                                var pvalue = {
                                                    Attributes: FormValues,
                                                    sGUID: sGUID,
                                                    sContext: null,
                                                    sBOName: sBO
                                                }
                                                if (!bIsValid) {
                                                    FinalValid = false;
                                                    $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                    $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                }
                                                if (bIsValid) {
                                                    $.ajax({
                                                        url: SaveEditBOURL,
                                                        //url: '@Url.Action("EditData", "XiLink")',
                                                        type: 'POST',
                                                        contentType: "application/json; charset=utf-8",
                                                        datatype: "json",
                                                        cache: false,
                                                        async: false,
                                                        data: JSON.stringify(pvalue),
                                                        success: function (data) {
                                                            if (data.length > 0) {
                                                                var sFormGuid = formid.split('_')[1];
                                                                $('#' + formid).find(".field-errmsg").find('span').html("");
                                                                $('#' + formid).find(".highlight--help").removeClass("msg-error");
                                                                for (var len = 0; len < data.length; len++) {
                                                                    if (data[len].sErrorMessage != "Failure") {
                                                                        var IsSuccess = true;
                                                                        var Scripts = data[len].oScriptErrors;
                                                                        var sBoName = data[len].sBOName;
                                                                        if (Scripts != null) {
                                                                            if (Object.keys(Scripts).length > 0) {
                                                                                sValidateAttr = [];
                                                                                for (var i = 0; i < Object.keys(Scripts).length; i++) {
                                                                                    $('span.' + sFormGuid + "-" + sBoName + "-" + Object.keys(Scripts)[i]).closest('.highlight--help').addClass('msg-error');
                                                                                    $('span.' + sFormGuid + "-" + sBoName + "-" + Object.keys(Scripts)[i]).html(Object.values(Scripts)[i]);
                                                                                    IsSuccess = false;
                                                                                    FinalValid = false;
                                                                                    if (sValidateAttr.length == 0) {
                                                                                        var sValidateAttrSelector = "input[name='" + Object.keys(Scripts)[i] + "']";
                                                                                        sValidateAttr.push(sValidateAttrSelector);
                                                                                        sValidateAttrSelector = "select[name='" + Object.keys(Scripts)[i] + "']";
                                                                                        sValidateAttr.push(sValidateAttrSelector);
                                                                                    }
                                                                                }
                                                                                $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                                                $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                                            }
                                                                        }
                                                                        if (IsSuccess) {
                                                                            var sPK = data[len].BOD.sPrimaryKey;
                                                                            var iID = data[len].iInstanceID;
                                                                            var sBOUID = data[len].BOD.BOID + "_" + iID;
                                                                            $('#' + formid).find('input[name=' + sPK.toLowerCase() + ']').val(iID);
                                                                            $('#' + formid).find('input[name=tr' + len + '_' + sPK.toLowerCase() + ']').val(iID);
                                                                            $('#' + formid).find(".StatusMessages").hide();
                                                                            //$('#' + formid).prepend($('<div class="StatusMessages"><div class="alert alert-success">Data Saved Succesfully</div></div>'));
                                                                            $('#' + formid).attr("data-instanceid", iID);
                                                                            //$('#' + formid).attr("id", "Create_" + sBOUID);
                                                                            var sIdentifier = $('#' + formid).attr("data-identifier");
                                                                            if (sIdentifier) {
                                                                                var param = {};
                                                                                param["sName"] = sIdentifier;
                                                                                param["sValue"] = iID;
                                                                                InputParams.push(param);
                                                                                var sCacheInstance = $('#' + formid).attr("data-CacheInstance");
                                                                                if (sCacheInstance) {
                                                                                    fncSetParam(iID, sGUID, "{XIP|" + sCacheInstance + "}", "", "");
                                                                                }
                                                                            }
                                                                            var RefreshBtn = $('.hideref-' + data[len].BOD.BOID);
                                                                            if (RefreshBtn && RefreshBtn.length > 0) {
                                                                                $(RefreshBtn[0]).click();
                                                                            }
                                                                        }
                                                                    }
                                                                    else {
                                                                        $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                                        $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                                        FinalValid = false;
                                                                        var Scripts = data[len].oScriptErrors;
                                                                        var sBoName = data[len].sBOName;
                                                                        if (Scripts != null) {
                                                                            if (Object.keys(Scripts).length > 0) {
                                                                                IsError = true;
                                                                                for (var i = 0; i < Object.keys(Scripts).length; i++) {
                                                                                    $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).closest('.highlight--help').addClass('msg-error');
                                                                                    $('span.' + sFormGuid + "-" + sBoName.replace(' ', '') + "-" + Object.keys(Scripts)[i]).html(Object.values(Scripts)[i]);
                                                                                }
                                                                            }
                                                                        }
                                                                        //IsError = true;
                                                                        $('#' + formid).find(".StatusMessages").hide();
                                                                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                                                        $('.StatusMessages').html('<div class="alert alert-danger">Something went wrong while saving</div>');
                                                                    }
                                                                }

                                                            }
                                                        },
                                                        error: function () {

                                                        }
                                                    })
                                                }
                                            }
                                        }
                                    });
                                }
                                else if (ComponentID == 55) {
                                    SaveMultipleRows();
                                }
                                else if (ComponentID == 61) {
                                    SaveCheckboxComponent(sGUID);
                                }
                                else if (ComponentID == 72) {
                                    var AccountC = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
                                    var formid = $(AccountC).closest('form').attr('id');
                                    var IsTwoWay = $(AccountC).closest('form').attr('data-Twoway');
                                    var IsSMS = $(AccountC).closest('form').attr('data-IsSMS');
                                    var IsEmail = $(AccountC).closest('form').attr('data-IsEmail');
                                    var OTPCase = $(AccountC).closest('form').attr('data-OTPCase');
                                    var OTPLength = $(AccountC).closest('form').attr('data-OTPLength');
                                    var OTPType = $(AccountC).closest('form').attr('data-OTPType');
                                    var UserName = "";
                                    var password = "";
                                    var ConfirmPWD = "";
                                    //var Form = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
                                    var formData = JSON.parse(JSON.stringify(jQuery('.AccountComponent').serializeArray()));
                                    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', Form);
                                    var FormType = $('div.LayoutCode_' + sGUID).find('.AccountComponent').attr('data-FormType');
                                    var oParams = [];
                                    var Param1 = {};
                                    Param1["sName"] = "IsTwoWay";
                                    Param1["sValue"] = IsTwoWay;
                                    oParams.push(Param1);
                                    var Param2 = {};
                                    Param2["sName"] = "bsms";
                                    Param2["sValue"] = IsSMS;
                                    oParams.push(Param2);
                                    var Param3 = {};
                                    Param3["sName"] = "bmail";
                                    Param3["sValue"] = IsEmail;
                                    oParams.push(Param3);
                                    var Param4 = {};
                                    Param4["sName"] = "iotpcase";
                                    Param4["sValue"] = OTPCase;
                                    oParams.push(Param4);
                                    var Param5 = {};
                                    Param5["sName"] = "iotplength";
                                    Param5["sValue"] = OTPLength;
                                    oParams.push(Param5);
                                    var Param6 = {};
                                    Param6["sName"] = "iotptype";
                                    Param6["sValue"] = OTPType;
                                    oParams.push(Param6);
                                    if (formData && formData.length > 0) {
                                        for (var k = 0; k < formData.length; k++) {
                                            if (formData[k].name == "UserName") {
                                                UserName = formData[k].value;
                                            }
                                            if (formData[k].name == "Password") {
                                                password = formData[k].value;
                                            }
                                            if (formData[k].name == "ConfirmPassword") {
                                                ConfirmPWD = formData[k].value;
                                            }
                                            if (formData[k].name == "ConfirmPassword") {
                                                ConfirmPWD = formData[k].value;
                                            }
                                        }
                                    }
                                    if (FormType == "Registration") {
                                        LoginAccountComponentURL = SaveAccountComponentURL;
                                    }
                                    else if (FormType == "FP") {
                                        fncSetParam("RP", sGUID, "{XIP|sLoginType}", "");
                                        fncSetParam("Reset Password", sGUID, "NextStep", "");
                                        LoginAccountComponentURL = ForgotPasswordURL;
                                    }
                                    else if (FormType == "Authentication") {
                                        fncSetParam("Authentication", sGUID, "{XIP|sLoginType}", "");
                                        fncSetParam("Reset Password", sGUID, "NextStep", "");
                                        LoginAccountComponentURL = ForgotPasswordURL;
                                    }
                                    else if (FormType == "RP") {
                                        fncSetParam("Your Quotes", sGUID, "NextStep", "");
                                        LoginAccountComponentURL = ResetPasswordURL;
                                    }
                                    if (FormType == "Login") {
                                        $.ajax({
                                            url: LoginAccountComponentURL,
                                            type: "Post",
                                            contentType: "application/json; charset=utf-8",
                                            datatype: "json",
                                            cache: false,
                                            async: true,
                                            data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: FormType, oParams: oParams }),
                                            success: function (data) {
                                                if (data != null && data != "Failure") {
                                                    //fncSetParam(data, sGUID, sName, "");
                                                    if (data.xiStatus == 30) {
                                                        FinalValid = false;
                                                        $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                        $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                        $('#' + formid).find(".StatusMessages").hide();
                                                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">' + data.oResult + '</div>');
                                                    }
                                                }
                                                else {
                                                    FinalValid = false;
                                                    $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                    $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                    $('#' + formid).find(".StatusMessages").hide();
                                                    $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                                    $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Error</div>');
                                                }
                                            }
                                        });
                                    }
                                    else if ((FormType == "Registration" && password == ConfirmPWD) || FormType != "Registration") {
                                        $.ajax({
                                            url: LoginAccountComponentURL,
                                            type: "Post",
                                            contentType: "application/json; charset=utf-8",
                                            datatype: "json",
                                            cache: false,
                                            async: true,
                                            data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: FormType }),
                                            success: function (data) {
                                                if (data != null && data != "Failure") {
                                                    //fncSetParam(data, sGUID, sName, "");
                                                    if (data.xiStatus == 30) {
                                                        FinalValid = false;
                                                        $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                        $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                        $('#' + formid).find(".StatusMessages").hide();
                                                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">' + data.oResult + '</div>');
                                                    }
                                                }
                                                else {
                                                    FinalValid = false;
                                                    $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                                    $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                                    $('#' + formid).find(".StatusMessages").hide();
                                                    $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                                    $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Error</div>');
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        FinalValid = false;
                                        $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                        $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                        $('#' + formid).find(".StatusMessages").hide();
                                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Password and confirmation password must be same</div>');

                                    }
                                    //SaveAccountComponent(sGUID);
                                }
                            }
                        }
                    }
                }
                else {
                    for (var siid in StepIns.Sections) {
                        if (sdid == siid) {
                            SecIns = StepIns.Sections[siid];
                            SecDef = StepDef.Sections[sdid];

                            if (SecDef.iDisplayAs == 30) {
                                for (var j = 0; j < FieldValues.length; j++) {
                                    var FieldDefID = FieldValues[j].Label;
                                    var XiValue = SecIns.XIValues[FieldDefID];
                                    if (XiValue) {
                                        var ISMerge = $("[name=" + FieldDefID + "]").attr('data-Merge');
                                        if (ISMerge != "" && ISMerge == "True") {
                                        }
                                        else {
                                            XiValue.sValue = "";
                                            XiValue.sDerivedValue = "";
                                        }
                                        SecIns.XIValues[FieldDefID] = XiValue;
                                        //fncGetValidationMessage(StepIns.FKiQSStepDefinitionID, FldOrg.FieldOrigin, FieldValues[j].Data, FieldValues, sGUID)
                                    }
                                }
                                StepIns.Sections[SecIns.FKiStepSectionDefinitionIDXIGUID + "_Sec"] = SecIns;
                            }
                        }
                    }
                }
            }
        }
        else {
            if (StepIns.XIValues && Object.keys(StepIns.XIValues).length > 0) {
                for (var j = 0; j < FieldValues.length; j++) {
                    var FieldDefID = FieldValues[j].Label;
                    var XiValue = StepIns.XIValues[FieldDefID];
                    if (XiValue) {
                        var FldOrg = StepDef.FieldDefs[FieldDefID];
                        if (FldOrg.FieldOrigin.FK1ClickID > 0) {
                            var Value = $('#Field-' + StepDef.ID + '-' + FieldDef.ID).attr('data-value');
                            XiValue.sValue = Value;
                            StepIns.XIValues[FieldDefID] = XiValue;
                        }
                        else {
                            XiValue.sValue = FieldValues[j].Data;
                            XiValue.sDerivedValue = FieldValues[j].sDerivedValue;
                            StepIns.XIValues[FieldDefID] = XiValue;
                            if (bUIDRef == "true" || bUIDRef == "True") {
                                fncGetValidationMessage(StepDef.XIGUID.toString(), FldOrg.FieldOrigin, FieldValues[j].Data, "", sGUID)
                            }
                            else {
                                fncGetValidationMessage(StepIns.FKiQSStepDefinitionID, FldOrg.FieldOrigin, FieldValues[j].Data, "", sGUID)
                            }
                        }
                    }
                }
            }
            else {
                if (StepDef.iDisplayAs == 40) {
                    //SaveComponent(sGUID, StepDef.sName );
                }
            }
        }
        for (var oScript in StepDef.Scripts) {
            if (StepDef.Scripts[oScript] != null && StepDef.Scripts[oScript].sScript != null && oScript != 'Step Visibility') {
                var sVadilation = "Please Check your data";
                if (StepDef.Scripts[oScript].ScriptResults.length > 0) {
                    sVadilation = StepDef.Scripts[oScript].ScriptResults[0].sUserError;
                }
                fncValidateStep(StepDef.Scripts[oScript].sScript, sVadilation, StepDef.ID, FieldValues, sGUID);
            }
        }
        oQSInstance.Steps[CurrentStepName] = StepIns;
        if (FinalValid) {
            if (Type == "Save") {
                fncSaveQSStep(StepIns, sGUID).then(function (state, callback) {
                    resolve(true);
                });
            }
            else if (Type == "SaveClose") {
                fncSaveQSStep(StepIns, sGUID, Type, $this)
            }
            else if (Type == "SaveNext") {

                if (CurrentStepName == "PFSummary") {
                    if ($('.TermsCheckbox').is(':checked') && $('.TermsDeclarationCheckbox').is(':checked')) {
                        fncGetNextStep(StepIns, sGUID, StepIns.bIsLastStep);
                        if (bPostHideSaveandNext.toLowerCase() == "yes") {
                            $('.LayoutCode_' + sGUID).find("#QSNxtBtn").hide();
                            $('.LayoutCode_' + sGUID).find("#QSLoadingBtn").hide();
                        }
                    }
                    else {
                        $(".TermsWindow").dialog({
                            title: "Terms and Condictions",
                            autoOpen: true,
                            modal: true,
                            width: 1000,
                            left: '400px',
                            top: '80px',
                            buttons: [
                                {
                                    text: "Ok",
                                    click: function () {
                                        $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
                                        $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
                                        $(".TermsWindow").dialog('close');
                                        $(".TermsWindow").dialog('destroy');
                                    }
                                }
                            ]
                        });
                    }
                }
                else {
                    fncGetNextStep(StepIns, sGUID, StepIns.bIsLastStep);
                    if (bPostHideSaveandNext.toLowerCase() == "yes") {
                        $('.LayoutCode_' + sGUID).find("#QSNxtBtn").hide();
                        $('.LayoutCode_' + sGUID).find("#QSLoadingBtn").hide();
                    }
                }
                //fncGetNextStep(QuestionSet, sGUID, StepIns.bIsLastStep).then(function (Status) {
                //    CurrentStepID = QuestionSet.iCurrentStepID;
                //    var CurrentStepName = QuestionSet.sCurrentStepName;
                //    StepDef = QuestionSet.QSDefinition.Steps[CurrentStepName];
                //});
            }
        }
        else {
            if (sValidateAttr != undefined && sValidateAttr.length > 0) {
                for (var j = 0; j < sValidateAttr.length; j++) {
                    //$("input[name='inputControlNameHere']").focus();
                    $(sValidateAttr[j]).focus();
                }
            }
            resolve(true);
        }

    }); bas
}


function fncSignalR(sLayGUID, iQSDID, sXiLink) {
    var oQSInstance = QSInsDict[sLayGUID];
    var disconnectType = "";
    var chat = $.connection.notifyHub;
    chat.client.addNewMessageToPage = function (model) {
        if (model.QSInstanceID == oQSInstance.XIGUID) {
            sGUID = sLayGUID;
            fncBindSignalrData(model);
        }
        if (model.ProductversionID == null || model.ProductversionID == "" || model.ProductversionID == "undefined") {
            disconnectType = "explicitly closed";
            $.connection.hub.stop();
        }
    };
    start();
    function start() {
        $.connection.hub.start({ transport: ['longPolling', 'webSockets'] }, function () { console.log('connection started!'); }).done(function () {
            var ConnectionID = $.connection.hub.id;
            fncSetParam(ConnectionID, sLayGUID, "SignalRConnectionID", "", "");
            //XIRun(5012, 0, sLayGUID, null, false, 0, iQSDID);
            XIRun(null, sXiLink, 0, sLayGUID, null, false, 0, iQSDID);
            // Call the Send method on the hub.
            //alert('Now connected, connection ID=' + $.connection.hub.id);
            chat.server.send();
            // Clear text box and reset focus for next comment.
            //$('#message').val('').focus();
            //});
        });
    }
    $.connection.hub.connectionSlow(function () {
        alert("We found an issue with your internet connection, Please check it might be slow");
        // LOG HERE THAT CONNECTION IS SLOW WITH QSINSTANCEID
        fncLogSignalrEvents("Connectionslow Event fired", oQSInstance.ID);
        console.log("SignalR Connectionslow called");

    });
    $.connection.hub.reconnecting(function () {
        // LOG HERE            
        fncLogSignalrEvents("reconnecting Event fired", oQSInstance.ID);
        console.log("SignalR reconnecting called");
        start();
    });
    $.connection.hub.disconnected(function () {
        // LOG HERE            
        if (disconnectType != "explicitly closed") {
            fncLogSignalrEvents("disconnected Event fired", oQSInstance.ID);
            console.log("SignalR disconnected called");
            start();
            $('.MQuotesDiv').each(function () {
                var type = $(this).attr("data-Type");
                if (type == 'failed') {
                    $(this).find('.load').html('unable to retrieve a quote');
                    $(this).find(".loadBuy").hide();
                    $(this).find('.MoreDetailsContent').hide();
                    $(this).find('.riskBtn').hide();
                }
            });
            if ($('.divBestprice').find('img').length) {
                $(this).hide();
            }
            alert("We found some issue with your network, Please try to recalculate");
            if ($.connection.hub.lastError) {
                var errormessage = $.connection.hub.lastError.message;
                fncLogSignalrEvents("disconnected Error Message: " + errormessage, oQSInstance.ID);
                //alert("Disconnected. Reason: " +  $.connection.hub.lastError.message);
            }
        }
        else {
            fncUpdaeQuoteRank(oQSInstance.ID);
        }
        //setTimeout(function () {
        //    $.connection.hub.start();
        //    console.log("SignalR reconnect called after disconnecting");
        //    fncLogSignalrEvents("Reconnecting after disconnected");
        //}, 5000); // Restart connection after 5 seconds.
    });
}

function SaveCheckboxComponent(sGUID) {
    var Checkbox = $('div.LayoutCode_' + sGUID).find('.CheckboxComponent');
    var Category = $(Checkbox[0]).find('div.category');
    for (var m = 0; m < Category.length; m++) {
        var Type = $(Category[m]).attr('data-type');
        var Checked = $(Category[m]).find('input[name="checkb"]');
        var iType = 0;
        if (Type == "inline") {
            iType = 10;
        }
        else if (Type == "row") {
            iType = 20;
        }
        var i1ClickID = 0;
        $.ajax({
            url: GetCacheParameterValueURL,
            type: "POST",
            async: false,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            cache: false,
            data: JSON.stringify({ sParamName: "{XIP|1ClickID}", sGUID: sGUID }),
            success: function (data) {
                i1ClickID = data["{XIP|1ClickID}"];
                var FormValues = [];
                if (Checked.length > 0) {
                    for (var n = 0; n < Checked.length; n++) {
                        FormValues = [];
                        var ID = 0; var value = 0; var isSave = false; var isDelete = false;
                        if ($(Checked[n]).prop("checked") == true) {
                            value = Checked[n].value;
                            ID = $(Checked[n]).attr('data-id');
                            isSave = true;
                        }
                        else {
                            ID = $(Checked[n]).attr('data-id');
                            if (parseInt(ID) > 0) {
                                value = Checked[n].value;
                                isSave = true;
                                isDelete = true;
                            }
                        }
                        if (isSave) {
                            FormValues.push({ sName: 'iType', sValue: iType, bDirty: true });
                            FormValues.push({ sName: 'FKiActionID', sValue: value, bDirty: true });
                            FormValues.push({ sName: 'FKi1ClickID', sValue: i1ClickID, bDirty: true });
                            FormValues.push({ sName: 'ID', sValue: ID, bDirty: true });
                            if (isDelete) {
                                FormValues.push({ sName: 'XIDeleted', sValue: "1", bDirty: true });
                            }

                            var pvalue = {
                                Attributes: FormValues,
                                sGUID: sGUID,
                                sBOName: 'XI1ClickAction'
                            }
                            $.ajax({
                                url: SaveEditBOURL,
                                type: 'POST',
                                contentType: "application/json; charset=utf-8",
                                datatype: "json",
                                async: false,
                                cache: false,
                                data: JSON.stringify(pvalue),
                                success: function (data) {

                                }
                            })
                        }
                    }

                }
            }
        });

    }
}
function SaveAccountComponent($This) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    var AccountC = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
    var formid = $(AccountC).closest('form').attr('id');
    var UserName = "";
    var password = "";
    var ConfirmPWD = "";
    var IsTwoWay = $(AccountC).closest('form').attr('data-Twoway');
    var IsSMS = $(AccountC).closest('form').attr('data-IsSMS');
    var IsEmail = $(AccountC).closest('form').attr('data-IsEmail');
    var OTPCase = $(AccountC).closest('form').attr('data-OTPCase');
    var OTPLength = $(AccountC).closest('form').attr('data-OTPLength');
    var OTPType = $(AccountC).closest('form').attr('data-OTPType');
    //var Form = $('div.LayoutCode_' + sGUID).find('.AccountComponent');
    var formData = JSON.parse(JSON.stringify(jQuery('.AccountComponent').serializeArray()));
    //var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', Form);
    var FormType = $('div.LayoutCode_' + sGUID).find('.AccountComponent').attr('data-FormType');
    var AccountType = $('div.LayoutCode_' + sGUID).find('.AccountComponent').attr('data-AccountType');
    if (formData && formData.length > 0) {
        for (var k = 0; k < formData.length; k++) {
            if (formData[k].name == "UserName") {
                UserName = formData[k].value;
            }
            if (formData[k].name == "Password") {
                password = formData[k].value;
            }
            if (formData[k].name == "ConfirmPassword") {
                ConfirmPWD = formData[k].value;
            }
            if (formData[k].name == "ConfirmPassword") {
                ConfirmPWD = formData[k].value;
            }
        }
    }
    var oParams = [];
    var Param1 = {};
    Param1["sName"] = "IsTwoWay";
    Param1["sValue"] = IsTwoWay;
    oParams.push(Param1);
    var Param2 = {};
    Param2["sName"] = "bsms";
    Param2["sValue"] = IsSMS;
    oParams.push(Param2);
    var Param3 = {};
    Param3["sName"] = "bmail";
    Param3["sValue"] = IsEmail;
    oParams.push(Param3);
    var Param4 = {};
    Param4["sName"] = "iotpcase";
    Param4["sValue"] = OTPCase;
    oParams.push(Param4);
    var Param5 = {};
    Param5["sName"] = "iotplength";
    Param5["sValue"] = OTPLength;
    oParams.push(Param5);
    var Param6 = {};
    Param6["sName"] = "iotptype";
    Param6["sValue"] = OTPType;
    oParams.push(Param6);
    if (FormType == "Registration") {
        LoginAccountComponentURL = SaveAccountComponentURL;
    }
    else if (FormType == "FP") {
        fncSetParam("RP", sGUID, "{XIP|sLoginType}", "");
        fncSetParam("Reset Password", sGUID, "NextStep", "");
        LoginAccountComponentURL = ForgotPasswordURL;
    }
    else if (FormType == "Authentication") {
        fncSetParam("Authentication", sGUID, "{XIP|sLoginType}", "");
        fncSetParam("Reset Password", sGUID, "NextStep", "");
        LoginAccountComponentURL = ForgotPasswordURL;
    }
    else if (FormType == "RP") {
        fncSetParam("Your Quotes", sGUID, "NextStep", "");
        LoginAccountComponentURL = ResetPasswordURL;
    }
    fncSetParam(UserName, sGUID, "{XIP|sEmail}", "").then(function (state, callback) {
        //fncSetParam(LoginType, sGUID, "{XIP|sLoginType}", "").then(function (state, callback) {
        if (FormType == "Login") {
            $.ajax({
                url: LoginAccountComponentURL,
                type: "Post",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                cache: false,
                async: true,
                data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: AccountType, oParams: oParams }),
                success: function (data) {
                    if (data != null && data.oResult == "Login") {
                        //if (data == "Login") {
                        $.ajax({
                            url: LandingPageUrl,
                            type: "Post",
                            contentType: "application/json; charset=utf-8",
                            datatype: "json",
                            cache: false,
                            async: true,
                            //data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: FormType, sAccountType: AccountType }),
                            success: function (data) {
                                $("#ApplicationMain").html(data);
                            }
                        })
                    }

                    else if (data != null && data.xiStatus == 0) {
                        $.ajax({
                            url: GetApplicationLayoutURL,
                            type: "Post",
                            contentType: "application/json; charset=utf-8",
                            datatype: "json",
                            cache: false,
                            async: true,
                            //data: "",
                            success: function (data) {
                                //if (!data && data.oResult)
                                for (var i = 0; i < data.oResult.LayoutDetails.length; i++) {
                                    $('#' + data.oResult.LayoutDetails[i].PlaceholderUniqueName).html("");
                                }
                                ShowContentInDialogOrPopup(data.oResult, sGUID);
                            }
                        })
                    }
                    else if (data.xiStatus == 30) {
                        $('#' + formid).find(".StatusMessages").hide();
                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">' + data.oResult + '</div > ');
                    }
                    //else {
                    //    $("#ApplicationMain").html(data);
                    //}
                }
            });
        }
        else if ((FormType == "Registration" && password == ConfirmPWD) || (FormType == "RP" && password == ConfirmPWD) || (FormType != "Registration" && FormType != "RP")) {
            $.ajax({
                url: LoginAccountComponentURL,
                type: "Post",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                cache: false,
                async: true,
                data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: FormType, sAccountType: AccountType }),
                success: function (data) {
                    if (data != null && data.oResult == "Login") {
                        //if (data == "Login") {
                        $.ajax({
                            url: LandingPageUrl,
                            type: "Post",
                            contentType: "application/json; charset=utf-8",
                            datatype: "json",
                            cache: false,
                            async: true,
                            //data: JSON.stringify({ UserName: UserName, Password: password, sGUID: sGUID, sType: FormType, sAccountType: AccountType }),
                            success: function (data) {
                                $("#ApplicationMain").html(data);
                            }
                        })
                    }
                    else if (data != null && data.xiStatus == 0) {
                        $.ajax({
                            url: GetApplicationLayoutURL,
                            type: "Post",
                            contentType: "application/json; charset=utf-8",
                            datatype: "json",
                            cache: false,
                            async: true,
                            //data: "",
                            success: function (data) {
                                //if (!data && data.oResult)
                                for (var i = 0; i < data.oResult.LayoutDetails.length; i++) {
                                    $('#' + data.oResult.LayoutDetails[i].PlaceholderUniqueName).html("");
                                }
                                ShowContentInDialogOrPopup(data.oResult, sGUID);
                            }
                        })
                    }
                    else if (data.xiStatus == 30) {
                        $('#' + formid).find(".StatusMessages").hide();
                        $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
                        $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">' + data.oResult + '</div > ');
                    }
                }
            });
        }
        else {
            FinalValid = false;
            //$('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
            //$('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
            $('#' + formid).find(".StatusMessages").hide();
            $('#' + formid).prepend($('<div class="StatusMessages"></div>'));
            $('#' + formid).find('.StatusMessages').html('<div class="alert alert-danger">Password and confirmation password must be same</div>');

        }
        //});
    });

}

function fncUpdaeQuoteRank(iQSIID) {
    $.ajax({
        url: UpdateQuoteRankURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        cache: false,
        async: true,
        data: JSON.stringify({ iQSIID: iQSIID }),
        success: function (data) {

        }
    });
}

function fncBindSignalrData(Model) {
    console.log("SignalR Binding Started for " + Model.ProductversionID);
    var currencySymbol = "�";
    // Best Price
    var productIdversion = Model.ProductversionID;
    var QSInstanceID = Model.QSInstanceID;
    var productid = Model.ProductID;
    if (!sQSType || sQSType != null || sQSType == "") {
        sQSType = Model.sQSType;
    }
    // monthly and Yearly price here base on Product and versionids
    if (productIdversion > 0) {
        var QuoteID = productIdversion + "_QuoteID";
        var FinalQuote = productIdversion + "_FinalQuote";
        var FinalpricetagID = productIdversion + "_rfinalquote";
        var monthlydefault = productIdversion + "_zdefaultdeposit";
        var monthlyprice = productIdversion + "_rmonthlyprice";
        var monthlytotal = productIdversion + "_rmonthlytotal";
        var ree = $("#" + FinalpricetagID).length;
        var iQtPrice = 0; //parseInt(Model.rFinalQuote);
        if (Model.rFinalQuote == 0 && Model.iQuoteStatus == 30) {
            iQtPrice = 999998;
        }
        //else if (Model.iQuoteStatus == 10 || Model.iQuoteStatus == 20) {
        //    iQtPrice = 999999;
        //}
        else if (Model.rFinalQuote < 0) {
            iQtPrice = 0;
        }
        else if (Model.bIsIndicativePrice) {
            iQtPrice = parseInt(Model.rFinalQuote * 1000);
        }
        else {
            iQtPrice = parseInt(Model.rFinalQuote * 100);
        }
        $('#QuoteSection_' + productIdversion).css({ "order": iQtPrice });
        $("#" + monthlydefault).html("1 x " + currencySymbol + parseFloat(Model.zDefaultDeposit).toFixed(2));
        $("#" + monthlyprice).html("10 x " + currencySymbol + parseFloat(Model.rMonthlyPrice).toFixed(2));
        $("#" + monthlytotal).html(currencySymbol + parseFloat(Model.rMonthlyTotal).toFixed(2));
        $("#" + QuoteID).html(Model.QuoteID);
        $("#" + FinalQuote).html(parseFloat(Model.rFinalQuote).toFixed(2));
        var compulsorytagID = productIdversion + "_rcompulsoryexcess";
        var voluntorytagID = productIdversion + "_rvoluntaryexcess";
        $('.' + productIdversion + '_Type').attr("data-Type", "Success");
        var totaltageID = productIdversion + "_rtotalexcess";
        var IsStepLock = Model.IsLockStep;
        var Stars = "";
        if (Model.iQuoteStatus == 0 || (sQSType != null && sQSType != "" && sQSType.toLowerCase() == "internal")) {
            $("#" + FinalpricetagID).show();
            $('#AnnualPrice_' + productIdversion + '_Decline').hide();
            if (Model.bIsIndicativePrice) {
                Stars = "**";
            }
            if (Model.iQuoteStatus == 10 && Model.rFinalQuote > 0) {
                $('#' + productIdversion + '_IndicativeRefer').show();
                $('.' + productIdversion + '_Indicative').show();
            }
            else if (Model.iQuoteStatus == 20 && Model.rFinalQuote > 0) {
                $('#' + productIdversion + '_IndicativeDecline').show();
                $('.' + productIdversion + '_Indicative').show();
            }
            else if (Model.iQuoteStatus == 0 && Model.rFinalQuote > 0) {
                $('.quoteBtns_' + Model.iQuoteID).show();
            }
            if (Model.iQuoteStatus == 50) {
                $("#" + FinalpricetagID).html("Refer");
            } else {
                $("#" + FinalpricetagID).html(currencySymbol + parseFloat(Model.rFinalQuote).toFixed(2) + Stars);
            }
            //if (!Model.bIsLock) {
            //}
            $('#divMoreDetails_' + productIdversion + '_Decline').hide();

            $('#Annual_' + productIdversion + "_israted").show();
            $('#Monthly_' + productIdversion + "_israted").show();
            if (!IsStepLock) {
                $('#' + productIdversion + "_BuyBtn").removeAttr("disabled");
                $('#' + productIdversion + '_BuyBtn').show();
                if (Model.sTranstype == null || Model.sTranstype.toLowerCase() == "new" || Model.sTranstype.toLowerCase() == "mta" || Model.sTranstype == "renewal") {
                    $('.loadBuy').hide();
                    $('#' + Model.iQuoteID).hide();
                }
            }
            else {
                $('#ReCalculate').attr("disabled", true);
            }
            $('#Monthly_' + productIdversion + '_Decline').hide();
            $('#Monthly_' + productIdversion).show();
            $('#divMoreDetails_' + productIdversion + '_Decline').hide();
            $('#divMoreDetails_' + productIdversion).show();
            $("#" + compulsorytagID).html(currencySymbol + parseFloat(Model.rCompulsoryExcess).toFixed(2));
            $("#" + voluntorytagID).html(currencySymbol + parseFloat(Model.rVoluntaryExcess).toFixed(2));
            $("#" + totaltageID).html(currencySymbol + parseFloat(Model.rTotalExcess).toFixed(2));
            $('#' + productIdversion + "_israted").show();
            $('#cover_' + productIdversion + "_israted").show();
            $('#Building_' + productIdversion + "_isnonrated").show();
            $('#Building_' + productIdversion + "_isnonrated").html(currencySymbol + '0.00');
            $('#Content_' + productIdversion + "_isnonrated").show();
            $('#Content_' + productIdversion + "_isnonrated").html(currencySymbol + '0.00');
            $('#Personal_' + productIdversion + "_isnonrated").show();
            $('#Personal_' + productIdversion + "_isnonrated").html(currencySymbol + '0.00');
            $('#LegalExpense_' + productIdversion + "_israted").show();
            $('#Excess_' + productIdversion + "_israted").show();
            $('#Coverabroad_' + productIdversion + "_israted").show();
            if (Model.bIsApplyFlood == "True") {
                if (Model.bIsFlood == "True") {
                    $('#' + productIdversion + "_bisflood_notincluded").show();
                }
                else {
                    $('#' + productIdversion + "_bisflood_included").show();
                }
            }
            if (Model.bIsIndicativePrice) {
                $("#Indicative_" + productIdversion).show();
                if (sQSType == null || sQSType == "" || sQSType == undefined || sQSType.toLowerCase() == "public") {
                    if (Model.sTranstype == null || Model.sTranstype.toLowerCase() == "new" || Model.sTranstype.toLowerCase() == "mta" || Model.sTranstype == "renewal") {
                        $('.loadBuy').hide();
                        $('#' + Model.iQuoteID).hide();
                    }
                    $('#' + productIdversion + '_BuyBtn').show();
                    $('#DeclineBuy_' + productIdversion).show();
                    $('#Building_' + productIdversion + "_israted").show();
                    $('#Content_' + productIdversion + "_israted").show();
                    $('#Building_' + productIdversion + "_isnonrated").hide();
                    $('#Content_' + productIdversion + "_isnonrated").hide();
                }
            }
        }
        else if (Model.iQuoteStatus == 20 && (sQSType == null || sQSType == "")) {
            $('#' + productIdversion + "_israted").show();
            $('#cover_' + productIdversion + "_israted").show();
            $('#NCPDetails_' + productIdversion).show();
            $("#" + FinalpricetagID).hide();
            $('#Annual_' + productIdversion + "_israted").show();
            $('#Monthly_' + productIdversion + "_israted").show();
            $('#AnnualPrice_' + productIdversion + '_Decline').show();
            if (Model.sTranstype == null || Model.sTranstype.toLowerCase() == "new" || Model.sTranstype.toLowerCase() == "mta" || Model.sTranstype == "renewal") {
                $('.loadBuy').hide();
                $('#' + Model.iQuoteID).hide();
            }
            $('#' + productIdversion + '_BuyBtn').show();
            $('#Monthly_' + productIdversion + '_Decline').show();
            $('#Monthly_' + productIdversion).hide();
            $('#divMoreDetails_' + productIdversion + '_Decline').show();
            $('#divMoreDetails_' + productIdversion).hide();
            $('#Building_' + productIdversion + "_israted").show();
            $('#Content_' + productIdversion + "_israted").show();
            $('#Personal_' + productIdversion + "_israted").show();
            $('#LegalExpense_' + productIdversion + "_israted").show();
            $('#Excess_' + productIdversion + "_israted").show();
            $('#Coverabroad_' + productIdversion + "_israted").show();
            if (Model.bIsIndicativePrice) {
                $("#Indicative_" + productIdversion).show();
                $('#NCPDetails_' + productIdversion).hide();
                $('#AnnualPrice_' + productIdversion + '_Decline').show();
                $('#Monthly_' + productIdversion + '_Decline').show();
            }
            if (Model.rFinalQuote > 0) {
                Stars = "**";
                $("#Indicative_" + productIdversion).show();
                $('#NCPDetails_' + productIdversion).hide();
                $('#divMoreDetails_' + productIdversion + '_Decline').hide();
                $('#divMoreDetails_' + productIdversion).show();
                $("#" + FinalpricetagID).show();
                $("#" + FinalpricetagID).html(currencySymbol + parseFloat(Model.rFinalQuote).toFixed(2) + Stars);
                $('#Monthly_' + productIdversion + '_Decline').hide();
                $('#Monthly_' + productIdversion).show();
                $('#AnnualPrice_' + productIdversion + '_Decline').hide();
            }
            $('#DeclineBuy_' + productIdversion).show();
            $("#" + compulsorytagID).html(currencySymbol + parseFloat(Model.rCompulsoryExcess).toFixed(2));
            $("#" + voluntorytagID).html(currencySymbol + parseFloat(Model.rVoluntaryExcess).toFixed(2));
            $("#" + totaltageID).html(currencySymbol + parseFloat(Model.rTotalExcess).toFixed(2));
            if (Model.bIsApplyFlood == "True") {
                if (Model.bIsFlood == "True") {
                    $('#' + productIdversion + "_bisflood_notincluded").show();
                }
                else {
                    $('#' + productIdversion + "_bisflood_included").show();
                }
            }
        }
        else {
            $("#" + FinalpricetagID).hide();
            $('#AnnualPrice_' + productIdversion + '_Decline').show();
            $('#' + productIdversion + '_BuyBtn').hide();
            $('#Monthly_' + productIdversion + '_Decline').show();
            $('#Monthly_' + productIdversion).hide();
            $('#divMoreDetails_' + productIdversion + '_Decline').show();
            $('#divMoreDetails_' + productIdversion).hide();
            $('#NCPDetails_' + productIdversion).show();
            if (Model.iQuoteStatus == 10) {
                $('#Excess_' + productIdversion + "_israted").show();
                $('#Coverabroad_' + productIdversion + "_israted").show();
                $('#Annual_' + productIdversion + "_israted").show();
                $('#Monthly_' + productIdversion + "_israted").show();
                $('#' + productIdversion + "_israted").show();
                $('#' + productIdversion + '_BuyBtn').hide();
                $('#DeclineBuy_' + productIdversion).show();
                $('#Building_' + productIdversion + "_israted").show();
                $('#Content_' + productIdversion + "_israted").show();
                $('#Building_' + productIdversion + "_isnonrated").hide();
                $('#Content_' + productIdversion + "_isnonrated").hide();
                $('#LegalExpense_' + productIdversion + "_israted").show();
            }
            if (Model.iQuoteStatus == 20) {
                $('#Excess_' + productIdversion + "_israted").show();
                $('#Coverabroad_' + productIdversion + "_israted").show();
                $('#Annual_' + productIdversion + "_israted").show();
                $('#Monthly_' + productIdversion + "_israted").show();
                $('#' + productIdversion + "_israted").show();
                $('#' + productIdversion + '_BuyBtn').hide();
                $('#DeclineBuy_' + productIdversion).show();
                $('#Building_' + productIdversion + "_israted").show();
                $('#Content_' + productIdversion + "_israted").show();
                $('#Building_' + productIdversion + "_isnonrated").hide();
                $('#Content_' + productIdversion + "_isnonrated").hide();
                $('#LegalExpense_' + productIdversion + "_israted").show();
            }
            if (Model.rFinalQuote > 0) {
                Stars = "**";
                $("#Indicative_" + productIdversion).show();
                $('#NCPDetails_' + productIdversion).hide();
                $('#divMoreDetails_' + productIdversion + '_Decline').hide();
                $('#divMoreDetails_' + productIdversion).show();
                $("#" + FinalpricetagID).show();
                $("#" + FinalpricetagID).html(currencySymbol + parseFloat(Model.rFinalQuote).toFixed(2) + Stars);
                $('#Monthly_' + productIdversion + '_Decline').hide();
                $('#Monthly_' + productIdversion).show();
                $('#AnnualPrice_' + productIdversion + '_Decline').hide();
            }
            if (Model.iQuoteStatus == 30) {
                $('#NCPDetails_' + productIdversion).show();
                $('#QuoteSection_' + productIdversion).addClass('bQred');
                $('#divExcess_' + productIdversion).html('N/A');
                if (Model.rFinalQuote != 0) {
                    $("#" + FinalpricetagID).show();
                    $("#" + FinalpricetagID).html(currencySymbol + parseFloat(Model.rFinalQuote).toFixed(2));
                    $('#AnnualPrice_' + productIdversion + '_Decline').hide();
                }
                else {
                    $('#AnnualPrice_' + productIdversion + '_Decline').html('N/A');
                }
                $('#Monthly_' + productIdversion + '_Decline').html('N/A');
                $('#DeclineBuy_' + productIdversion).show();
                $('#divMoreDetails_' + productIdversion + '_Decline').hide();
                $('#divMoreDetails_' + productIdversion).show();
                $('#' + productIdversion + "_isnonrated").show();
                $('#' + productIdversion + "_isnonrated").html(' ');
                $('#cover_' + productIdversion + "_isnonrated").show();
                $('#cover_' + productIdversion + "_isnonrated").html(' ');
                $('#Building_' + productIdversion + "_isnonrated").show();
                $('#Building_' + productIdversion + "_isnonrated").html('N/A');
                $('#Content_' + productIdversion + "_isnonrated").show();
                $('#Content_' + productIdversion + "_isnonrated").html('N/A');
                $('#Personal_' + productIdversion + "_isnonrated").show();
                $('#Personal_' + productIdversion + "_isnonrated").html('N/A');
                $('#Excess_' + productIdversion + "_isnonrated").show();
                $('#Excess_' + productIdversion + "_isnonrated").html('N/A');
                $('#LegalExpense_' + productIdversion + "_isnonrated").show();
                $('#LegalExpense_' + productIdversion + "_isnonrated").html('N/A');
                $('#Coverabroad_' + productIdversion + "_isnonrated").show();
                $('#Coverabroad_' + productIdversion + "_isnonrated").html('N/A');
                $('#Annual_' + productIdversion + "_isnonrated").show();
                $('#Annual_' + productIdversion + "_isnonrated").html('N/A');
                $('#Monthly_' + productIdversion + "_isnonrated").show();
                $('#Monthly_' + productIdversion + "_isnonrated").html('N/A');
                if (Model.bIsIndicativePrice) {
                    $("#Indicative_" + productIdversion).show();
                    $('#NCPDetails_' + productIdversion).hide();
                }
            }
            if (Model.iQuoteStatus == 50) {
                $('#Excess_' + productIdversion + "_isnonrated").show();
                $('#Excess_' + productIdversion + "_isnonrated").html('N/A');
                $('#Coverabroad_' + productIdversion + "_isnonrated").show();
                $('#Coverabroad_' + productIdversion + "_isnonrated").html('N/A');
            }
            $("#" + compulsorytagID).html(currencySymbol + parseFloat(Model.rCompulsoryExcess).toFixed(2));
            $("#" + voluntorytagID).html(currencySymbol + parseFloat(Model.rVoluntaryExcess).toFixed(2));
            $("#" + totaltageID).html(currencySymbol + parseFloat(Model.rTotalExcess).toFixed(2));
            //else {
            $('#DeclineBuy_' + productIdversion).show();
            if (Model.bIsApplyFlood == "True") {
                if (Model.bIsFlood == "True") {
                    $('#' + productIdversion + "_bisflood_notincluded").show();
                }
                else {
                    $('#' + productIdversion + "_bisflood_included").show();
                }
            }
            //}

        }
        // Excess data binding here
        //if (Model.iQuoteStatus != 30) {
        //    $("#" + FinalpricetagID).show();
        //    $('#AnnualPrice_' + productIdversion + '_Decline').hide();
        //    var Stars = "";
        //    if (Model.bIsIndicativePrice) {
        //        Stars = "**";
        //    }
        //    $("#" + FinalpricetagID).html(currencySymbol + parseFloat(Model.rFinalQuote).toFixed(2) + Stars);
        //    $('#Annual_' + productIdversion + "_israted").show();
        //    $('#Monthly_' + productIdversion + "_israted").show();
        //    $('#Monthly_' + productIdversion + '_Decline').hide();
        //    $('#Monthly_' + productIdversion).show();
        //}
    }
    else {
        if (parseFloat(Model.rBestQuote).toFixed(2) != 0.00 || Model.iQuoteStatus == 30) {
            $("#divBestprice").show();
            $("#divBestprice").html('Your cheapest quote is ' + currencySymbol + parseFloat(Model.rBestQuote).toFixed(2) + '');
            $("#divDecline").hide();
        }
        else {
            $("#divBestprice").hide();
            $("#divDecline").show();
        }
    }
    console.log("SignalR Binding completed for " + Model.ProductversionID);
}
function fncAmendDetails($this) {
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var oQSInstance = QSInsDict[sGUID];
    $.ajax({
        url: AmendDetailsURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        async: true,
        data: JSON.stringify({ iQSIID: oQSInstance.XIGUID, sGUID: sGUID, sType: 'public' }),
        success: function (data) {
            $('.LayoutCode_' + sGUID).find('#QSStep').html(data);
            if (bIsInternal == false) {
                $('html, body').animate({
                    scrollTop: $('.LayoutCode_' + sGUID).find('#QSStep').offset().top //#DIV_ID is an example. Use the id of your destination on the page
                }, 'slow');
            }
        }
    });
}
function fncLogSignalrEvents(sLogMessage, iQSIID) {
    $.ajax({
        url: LogSignalRURL,
        type: 'Post',
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        data: JSON.stringify({ iQSIID: iQSIID, sLog: sLogMessage }),
        async: false,
        cache: false,
        success: function (res) {
        },
        error: function (response) {
        }
    });
}

function fncLogJqueryError(sContext, sMessage) {
    $.ajax({
        url: LogJQueryURL,
        type: 'Post',
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        data: JSON.stringify({ sContext: sContext, sMessage: sMessage }),
        async: false,
        cache: false,
        success: function (res) {
        },
        error: function (response) {
        }
    });
}


function BuyQuoteBtnOld(Amount, sID, $this) {
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    $(".Buybtn").attr("disabled", true);
    $($this).text("Processing");
    var _this = $this;
    var vlaue = Amount;
    var sGuid = sGUID;
    vlaue = vlaue.slice(1);
    var ID = sID;
    QuoteID = ID;
    //fncSetParam(QuoteID, sGuid, "{XIP|Aggregations.id}", "");
    //window.location.href = '@Url.Action("Payment", "Payment")?ID=' + ID + '&Amount=' + vlaue;
    //window.location.href = '@Url.Action("test", "Payment")?ID=' + ID;
    $('#QuoteAMt').attr('data-value', vlaue);
    //fncRunQSStepAction('SaveNext', this);
    //$('#FinalAmt').html(vlaue);
    fncSetParam(QuoteID, sGuid, "{XIP|sQuoteGUID}", "Aggregations").then(function (state, callback) {
        fncRunQSStepAction('SaveNext', _this);
        setTimeout(function () {
            $('#FinalAmt').html(vlaue);

            /* Do something */
            if (callback) {
                callback();
            }
        }, 10000);
    });
}
function AddBasket(sID, $this) {
    //debugger
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var _this = $this;
    $($this).attr("disabled", true);
    var QuoteID = $('#' + sID + "_QuoteID").html();
    $.ajax({
        async: false,
        type: 'POST',
        url: ExecuteXIAlgorithmURL,
        data: JSON.stringify({ QuoteID: QuoteID, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
        }
    });
}
function BuyQuoteBtn(sID, $this) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    //function BuyQuoteBtn(ProductVersionID, $this) {
    // 
    //var sID = $("#ProductVersionID_QuoteID").Val();
    // var Amount = $("#ProductVersionID_FinalQuote").Val();

    $(".loadBuy").attr("disabled", true);
    $($this).text("Processing");
    var _this = $this;
    //var vlaue = Amount;
    var sGuid = sGUID;
    //vlaue = vlaue.slice(1);
    //var ID = sID;
    QuoteID = $('#' + sID + "_QuoteID").html();
    //fncSetParam(QuoteID, sGuid, "{XIP|Aggregations.id}", "");
    //window.location.href = '@Url.Action("Payment", "Payment")?ID=' + ID + '&Amount=' + vlaue ;
    //window.location.href = '@Url.Action("test", "Payment")?ID=' + ID;
    //$('#QuoteAMt').attr('data-value',vlaue);
    //fncRunQSStepAction('SaveNext', this);
    //$('#FinalAmt').html(vlaue);
    var sQuoteScript = "xi.s|{xi.a|'Aggregations',{xi.a|'Aggregations','" + QuoteID + "','FKiQSInstanceID',,'ID'},'iBuyStatus','0','FKiQSInstanceID'}";
    $.ajax({
        async: false,
        type: 'POST',
        url: ExecuteXIScriptURL,
        data: JSON.stringify({ sScript: sQuoteScript, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            //if (data != null && Value != data) {
            //    IsValid = false;
            //}
        }
    });
    var sScript = "xi.s|{xi.a|'Aggregations','" + QuoteID + "','iBuyStatus','10',''}";
    $.ajax({
        async: false,
        type: 'POST',
        url: ExecuteXIScriptURL,
        data: JSON.stringify({ sScript: sScript, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            //if (data != null && Value != data) {
            //    IsValid = false;
            //}
        }
    });
    fncSetParam(QuoteID, sGuid, "{XIP|iQuoteID}", "").then(function (state, callback) {
        fncRunQSStepAction('SaveNext', _this);
        setTimeout(function () {
            //$('#FinalAmt').html(vlaue);

            /* Do something */
            if (callback) {
                callback();
            }
        }, 10000);
    });

    //notifyState('State 1', function() {

    //});
}
function toggleIcon(e) {
    $(e.target)
        .prev('.panel-heading')
        .find(".more-less")
        .toggleClass('glyphicon-info-sign glyphicon-remove-sign');
}
$('.panel-group').on('hidden.bs.collapse', toggleIcon);
$('.panel-group').on('shown.bs.collapse', toggleIcon);

function myFunction(id) {
    var addoni = document.getElementById(id);
    addoni.classList.toggle("show");
}
function QuotePayment(PaymentType, BuyPrice, sguid) {
    var sGUIDN = sGUID;
    //$(".btn-pay").attr("disabled", true);
    $.ajax({
        url: UpdateQuoteURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        async: true,
        data: JSON.stringify({ sPaymentType: PaymentType, sGUID: sGUIDN }),
        success: function (data) {
            debugger
            if (data == "3") {
                if (PaymentType == "Annual") {
                    var url = PaymentURL + '?ID=' + sguid + '_' + sGUIDN + '&QuoteID=' + sguid + '&sLayoutGUID=' + sGUIDN;
                    $('#iframe_a').attr('src', url)
                }
                else {
                    $(".TermsWindow").dialog({
                        title: "Terms and Condictions",
                        autoOpen: true,
                        modal: true,
                        width: 1000,
                        left: '400px',
                        top: '80px',
                        buttons: [
                            {
                                text: "Ok",
                                click: function () {
                                    if ($('.TermsCheckbox').is(':checked') && $('.TermsDeclarationCheckbox').is(':checked')) {
                                        var url = PaymentURL + '?ID=' + sguid + '_' + sGUIDN + '&QuoteID=' + sguid + '&sLayoutGUID=' + sGUIDN;
                                        //var url = '@Url.Action("Payment", "Payment")?ID=' + sguid + '_' + sGUIDN + '&Amount=' + BuyPrice;
                                        $('#iframe_a').attr('src', url)
                                    }
                                    else {
                                        $('#iframe_a').attr('src', '')
                                    }
                                    $(".TermsWindow").dialog('close');
                                    $(".TermsWindow").dialog('destroy');
                                }
                            }
                        ]
                    });
                }
            }
        },
        error: function (data) {
        }
    });
};
function fncLoadRiskFactors(sQuoteGUID) {
    var QuotesGUID = $('#' + sQuoteGUID + "_QuoteID").html();
    XIRun(null, 2954, QuotesGUID, sGUIDN, "RiskFactors", false, 0, 0);
};
function fncBasketDetails($this, XiLinkID, sBO, QuoteID) {
    var sQuoteID = $('#' + QuoteID + "_QuoteID").html();
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var oQSInstance = QSInsDict[sGUID];

    //var sQuoteID = $($this).closest('#' + QuoteID + "_QuoteID").html();
    //XILinkLoadJson(13094, sGUIDN, null)
    if (XiLinkID == undefined || XiLinkID == "" || XiLinkID == null) {
        XIRun(null, 13094, null, sGUID, "BaskDetails", false, 0, 0);
    }
    else {
        var sNewGUID = CreateGuid();
        if (sBO == "QSInstance") {
            sQuoteID = oQSInstance.XIGUID;
        }
        fncSetParam(sGUID, sNewGUID, "{XIP|sParentQSGUID}", sBO).then(function (state, callback) {
            fncSetParam(sQuoteID, sNewGUID, "{XIP|i" + sBO + ".ID}", sBO).then(function (state, callback) {
                fncSetParam('yes', sNewGUID, 'fkiqsinstanceidxiguid', sBO, 'Aggregations').then(function (state, callback) {
                    XIRun(null, XiLinkID, null, sNewGUID, "BaskDetails", false, 0, 0);
                })
            })
        });
    }
};

function fncFAdjustments($this, XiLinkID, sBO, QuoteID) {
    var selector = $($this).attr('id');
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var oParams = [];
    var Param = {};
    Param["sName"] = "{XIP|Aggregations.ID}";
    Param["sValue"] = QuoteID;
    oParams.push(Param);
    Param = {};
    Param["sName"] = "iQuoteID";
    Param["sValue"] = QuoteID;
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|iProductVersionID}";
    Param["sValue"] = selector.split('_')[0];
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{xi.p|iProductVersionID}";
    Param["sValue"] = selector.split('_')[0];
    oParams.push(Param);
    Param = {};
    Param["sName"] = "{XIP|FKiQSInstanceID}";
    Param["sValue"] = "xi.s|{xi.a|'Aggregations','','FKiQSInstanceID','',id:" + QuoteID + "}";
    oParams.push(Param);
    //XIRun(XiLinkID, sGUID, oParams)
    XIRun($this, XiLinkID, null, sGUID, sBO, false, 0, null, null, oParams)
};
function fncLoadClientPage(URL, UserID, QSInstanceID) {
    if (UserID > 0) {
        window.open(URL + "?UserID=" + UserID + "&QSInstanceID=" + QSInstanceID, "_blank");
    } else {
        var tab = null;
        $.ajax({
            url: "Account/RedirectToClientPage",
            type: "Post",
            contentType: "application/json; charset=utf-8",
            datatype: "html",
            cache: false,
            async: true,
            data: JSON.stringify({ UserID: UserID, QSInstanceID: QSInstanceID }),
            success: function (data) {
                if (data == null) {
                    CustomMessage("Error Occured", false);
                }
                else {
                    tab = window.open(URL, "_self");
                    tab.parent.document.write(data);
                }
            },
            error: function (data) {
                CustomMessage("Error Occured", false);
            }
        });
    }
};
function fncLoadInviteRenewal(iInstanceID, $this) {
    var sMainGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var sGUID = CreateGuid();
    var sEmail = $($this).attr('data-sEmail');
    var sPolicyNo = $($this).attr('data-spolicyno');
    var PolicyPremium = $($this).attr('data-policypremium');
    var PolicyInsurer = $($this).attr('data-policyInsurer');
    var PolicyID = $($this).attr('data-policyID');
    // var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    fncSetParam(iInstanceID, sGUID, "{XIP|iRenewalQSInstanceID}", "").then(function (state, callback) {
        fncSetParam("QS Instance", sGUID, "sBOName", "").then(function (state, callback) {
            fncSetParam(iInstanceID, sGUID, "iInstanceID", "").then(function (state, callback) {
                fncSetParam(sEmail, sGUID, "{XIP|sEmail}", "").then(function (state, callback) {
                    fncSetParam("Renewal", sGUID, "{XIP|sTranstype}", "").then(function (state, callback) {
                        fncSetParam(sPolicyNo, sGUID, "-spolicyno", "").then(function (state, callback) {
                            fncSetParam(PolicyPremium, sGUID, "-policypremium", "").then(function (state, callback) {
                                fncSetParam(PolicyInsurer, sGUID, "-policyinsurer", "").then(function (state, callback) {
                                    fncSetParam(PolicyID, sGUID, "-policyid", "").then(function (state, callback) {
                                        fncSetParam(sMainGUID, sGUID, "{XIP|sMainQSGUID}", "").then(function (state, callback) {
                                            //var QuotesGUID = $('#' + sQuoteGUID + "_QuoteID").html();
                                            //XILinkLoadJson(8699, sGUID, null)
                                            XIRun(null, 8699, null, sGUID, "QS Instance", false, 0, 0);
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
    });
};
function fncGetNextStep(StepIns, sGUID, IsLastStep) {
    var oQSInstance = QSInsDict[sGUID];
    var QS = { oStepI: StepIns, sGUID: sGUID, sType: "public", iQSIID: oQSInstance.XIGUID };
    $.ajax({
        url: NextStepURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        async: true,
        data: JSON.stringify(QS),
        success: function (data) {
            $('.LayoutCode_' + sGUID).find('#QSStep').html(data);
            //$('#QSStep_' + sGUID).focus();
            if (bIsInternal == false) {
                $('html, body').animate({
                    scrollTop: $('.LayoutCode_' + sGUID).find('#QSStep').offset().top //#DIV_ID is an example. Use the id of your destination on the page
                }, 'slow');
            }
            var QTAMT = $('#QuoteAMt').attr('data-value');
            $('#FinalAmt').html(QTAMT);
            //resolve= true;
        },
        error: function (data) {
            $('.LayoutCode_' + sGUID).find('#QSStep').html('<h2>Something went wrong!! Please try again or contact admin</h2>');
        }
    });
    //return new Promise(function (resolve, reject) {

    //});
}
function fncSaveQSStep(StepIns, sGUID, Type, $this) {
    return new Promise(function (resolve, reject) {
        var oQSInstance = QSInsDict[sGUID];
        var QS = { oStepI: StepIns, sGUID: sGUID, iQSIID: oQSInstance.XIGUID };
        $.ajax({
            url: SaveQSStepURL,
            type: "Post",
            contentType: "application/json; charset=utf-8",
            datatype: "html",
            cache: false,
            async: true,
            data: JSON.stringify(QS),
            success: function (data) {
                if (data == null) {
                    CustomMessage("Error Occured", false);
                }
                else {
                    CustomMessage("Saved Successfully", true);
                }
                if (Type == "SaveClose") {
                    $($this).closest('div[class^="dialog-box"]').dialog('close');
                }
                resolve(true);
            },
            error: function (data) {
                CustomMessage("Error Occured", false);
                if (Type == "SaveClose") {
                    $($this).closest('div[class^="dialog-box"]').dialog('close');
                }
            }
        });
    });
}
function fncGetPreviousStep($this) {
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    $('.LayoutCode_' + sGUID).find('#QSBckBtn').hide();
    $('.LayoutCode_' + sGUID).find('#QSBckLoadingBtn').show();
    var oQSInstance = QSInsDict[sGUID];
    var iQSIID = oQSInstance.XIGUID;
    $.ajax({
        url: PrevStepURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        async: true,
        data: JSON.stringify({ iQSIID: iQSIID, sGUID: sGUID, sType: "public" }),
        success: function (data) {
            $('.LayoutCode_' + sGUID).find('#QSBckLoadingBtn').hide();
            $('.LayoutCode_' + sGUID).find('#QSBckBtn').show();
            $('.LayoutCode_' + sGUID).find('#QSStep').html(data);
            if (bIsInternal == false) {
                $('html, body').animate({
                    scrollTop: $('body').offset().top //#DIV_ID is an example. Use the id of your destination on the page
                }, 'slow');
            }
        },
        error: function (data) {
        }
    });
}
function fncGetCurrentStep(formid, sAttributes, sGUID, iBOIID, bISAddCache) {
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var FormValues = [];
    var titleid = [];
    for (var i = 0; i < formData.length; i++) {
        if (sRowId != "" && sRowNo != "") {
            var attr = formData[i].name.split('_')[0];
            if (attr == sRowNo) {
                FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
            }
        }
        else {
            FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
        }
    }
    var oQSInstance = QSInsDict[sGUID];
    var iQSIID = oQSInstance.XIGUID;
    $.ajax({
        url: CurrentStepURL,
        type: "Post",
        contentType: "application/json; charset=utf-8",
        datatype: "html",
        cache: false,
        async: true,
        data: JSON.stringify({ Attributes: FormValues, iQSIID: iQSIID, sAttributes: sAttributes, sGUID: sGUID, sType: "public", iInstaceID: iBOIID, bISAddCache: bISAddCache }),
        success: function (data) {
            if (data != null && data != "") {
                $('.LayoutCode_' + sGUID).find('#QSBckLoadingBtn').hide();
                $('.LayoutCode_' + sGUID).find('#QSBckBtn').show();
                $('.LayoutCode_' + sGUID).find('#QSStep').html(data);
                if (bIsInternal == false) {
                    $('html, body').animate({
                        scrollTop: $('body').offset().top //#DIV_ID is an example. Use the id of your destination on the page
                    }, 'slow');
                }
            }
        },
        error: function (data) {
        }
    });
}
function fncGetValidationMessage(StepID, FieldOrigin, Value, FieldValues, sGUID) {
    var IsOverRide = OverRideStatusDict[sGUID];
    var Visibility = $('.LayoutCode_' + sGUID).find('div[data-controltype="section"]').filter('.on').find('[name="' + FieldOrigin.sName + '"]')[0];
    var VisibilityF = $('.LayoutCode_' + sGUID).find('div[data-controltype="qsfield"]').filter('.on').find('[name="' + FieldOrigin.sName + '"]')[0];
    if (!Visibility && !VisibilityF) {
        var IsVal = FinalValid;
        IsValid = true;
        var XIDataType = FieldOrigin.DataType;
        if (FieldOrigin.sScript != null && FieldOrigin.iScriptType == 30) {
            var sfield1val;
            var arr = FieldOrigin.sScript.split(' ');
            var field1 = FieldValues.filter(function (x) { return x.Label === arr[0]; });
            if (field1 && field1.length > 0) {
                sfield1val = field1[0].Data;
            }
            if (eval(sfield1val + arr[1] + arr[2])) {
                FieldOrigin.bIsMandatory = true;
            }
            else {
                FieldOrigin.bIsMandatory = false;
            }
        }
        if (FieldOrigin.bIsMandatory && (Value == null || Value.length == 0)) {
            IsValid = false;
        }

        if (XIDataType.sRegex && XIDataType.sRegex != null && XIDataType.sRegex.length > 0 && FieldOrigin.bIsMandatory) {
            var regex = new RegExp(XIDataType.sRegex);
            var matches = regex.test(Value);
            if (!matches) {
                IsValid = false;
            }
        }
        if (XIDataType.sBaseDataType == "datetime") {
            var yearfield = Value.split('-')[2];
            var monthfield = Value.split('-')[1];
            monthfield = getMonth(monthfield);
            var dayfield = Value.split('-')[0];
            //var S1, E1;
            if (FieldOrigin.sMinResolvedValue || FieldOrigin.sMaxResolvedValue) {
                if (FieldOrigin.sMinResolvedValue) {
                    var S1;
                    if (FieldOrigin.sMinResolvedValue) {
                        S1 = FieldOrigin.sMinResolvedValue.split('-');
                    }
                    if (FieldOrigin.sMinResolvedValue && XIDataType.sBaseDataType == "datetime") {
                        var StartDate = "";
                        if (FieldOrigin.sFormat) {
                            S1[1] = getMonth(S1[1]);
                            StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        }
                        else {
                            StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        }
                        //var StartDate = new Date(parseInt(S1[1]), parseInt(S1[2]) - 1, parseInt(S1[0]));
                        //var EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                        if (InputDate >= StartDate) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
                if (FieldOrigin.sMaxResolvedValue) {
                    var E1;
                    if (FieldOrigin.sMaxResolvedValue) {
                        E1 = FieldOrigin.sMaxResolvedValue.split('-');
                    }
                    if (FieldOrigin.sMaxResolvedValue && XIDataType.sBaseDataType == "datetime") {
                        //var StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        var EndDate = "";
                        if (FieldOrigin.sFormat) {
                            E1[1] = getMonth(E1[1]);
                            EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        }
                        else {
                            EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        }
                        //var EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                        if (InputDate <= EndDate) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
                if (FieldOrigin.sMinResolvedValue && FieldOrigin.sMaxResolvedValue) {
                    var S1, E1;
                    if (FieldOrigin.sMinResolvedValue) {
                        S1 = FieldOrigin.sMinResolvedValue.split('-');
                    }
                    if (FieldOrigin.sMaxResolvedValue) {
                        E1 = FieldOrigin.sMaxResolvedValue.split('-');
                    }

                    if (FieldOrigin.sMinResolvedValue && FieldOrigin.sMaxResolvedValue && XIDataType.sBaseDataType == "datetime") {
                        var StartDate = "";
                        if (FieldOrigin.sFormat) {
                            S1[1] = getMonth(S1[1]);
                            StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        }
                        else {
                            StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        }
                        var EndDate = "";
                        if (FieldOrigin.sFormat) {
                            E1[1] = getMonth(E1[1]);
                            EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        }
                        else {
                            EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        }
                        //var StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                        //var EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                        var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                        if (InputDate >= StartDate && InputDate <= EndDate) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
            }
            else {
                if (XIDataType.sStartRange) {
                    S1 = XIDataType.sStartRange.split('/');
                }
                if (XIDataType.sEndRange) {
                    E1 = XIDataType.sEndRange.split('/');
                }
                if (XIDataType.sStartRange && XIDataType.sEndRange && XIDataType.sBaseDataType == "datetime") {
                    var StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                    var EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                    var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                    if (InputDate >= StartDate && InputDate <= EndDate) {

                    }
                    else {
                        IsValid = false;
                    }
                }
                else if (XIDataType.sStartRange && Value.length > XIDataType.sStartRange) {
                    var StartDate = new Date(parseInt(S1[0]), parseInt(S1[1]) - 1, parseInt(S1[2]));
                    var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                    if (InputDate >= StartDate) {

                    }
                    else {
                        IsValid = false;
                    }
                }
                else if (XIDataType.sEndRange && Value.length < XIDataType.sEndRange) {
                    var EndDate = new Date(parseInt(E1[0]), parseInt(E1[1]) - 1, parseInt(E1[2]));
                    var InputDate = new Date(parseInt(yearfield), parseInt(monthfield) - 1, parseInt(dayfield));
                    if (InputDate <= EndDate) {

                    }
                    else {
                        IsValid = false;
                    }
                }
            }
        }
        else if (XIDataType.sBaseDataType == "int") {
            if (FieldOrigin.bIsMandatory) {
                if (FieldOrigin.sScript != null && FieldOrigin.iScriptType == 10) {
                    var arr = FieldOrigin.sScript.split(' ');
                    var sfield1val;
                    var field1 = FieldValues.filter(function (x) { return x.Label === arr[0]; });
                    if (field1 && field1.length > 0) {
                        sfield1val = field1[0].Data;
                    }
                    //var field1 = FieldValues.find(m => m.Label == arr[0]).Data; ////IE fix
                    var field2 = arr[1];
                    var sfield3val;
                    var field3 = FieldValues.filter(function (x) { return x.Label === arr[2]; });
                    if (field3 && field3.length > 0) {
                        sfield3val = field3[0].Data;
                    }
                    //var field3 = FieldValues.find(m => m.Label == arr[2]).Data; ////IE fix
                    var f1 = parseInt(sfield1val);
                    var f2 = parseInt(sfield3val);
                    if (eval(f1 + field2 + f2)) {

                    }
                    else {
                        IsValid = false;
                    }
                }
                else {
                    var sMinValue = ""; var sMaxValue = "";
                    if (FieldOrigin.sMinResolvedValue) {
                        sMinValue = FieldOrigin.sMinResolvedValue;
                        if (FieldOrigin.sMinResolvedValue.indexOf("T") == 0 || FieldOrigin.sMinResolvedValue.indexOf("M") == 0 || FieldOrigin.sMinResolvedValue.indexOf("Y") == 0) {
                            FieldOrigin.sMinResolvedValue = FieldOrigin.sMinResolvedValue;
                            var S1 = FieldOrigin.sMinResolvedValue.split('-');
                            sMinValue = S1[0];
                        }
                    }
                    if (FieldOrigin.sMaxResolvedValue) {
                        sMaxValue = FieldOrigin.sMaxResolvedValue;
                        if (FieldOrigin.sMaxResolvedValue.indexOf("T") == 0 || FieldOrigin.sMaxResolvedValue.indexOf("M") == 0 || FieldOrigin.sMaxResolvedValue.indexOf("Y") == 0) {
                            FieldOrigin.sMaxResolvedValue = FieldOrigin.sMaxResolvedValue;
                            var E1 = FieldOrigin.sMaxResolvedValue.split('-');
                            sMaxValue = E1[0];
                        }
                    }
                    if (sMinValue && sMaxValue) {
                        if (Value >= sMinValue && Value <= sMaxValue) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                    else if (sMinValue) {
                        if (Value >= sMinValue) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                    else if (sMaxValue) {
                        if (Value <= sMaxValue) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
                if (!Value || Value == "") {
                    IsValid = false;
                }
            }
        }
        else if (XIDataType.sBaseDataType == "decimal") {
            if (FieldOrigin.bIsMandatory) {
                if (FieldOrigin.sScript != null && FieldOrigin.sName == "iClientDeposit") {
                    var arr = FieldOrigin.sScript.split(' ');
                    var sfield1val;
                    var field1 = FieldValues.filter(function (x) { return x.Label === arr[0]; });
                    if (field1 && field1.length > 0) {
                        sfield1val = field1[0].Data;
                    }
                    //var field1 = FieldValues.find(m => m.Label == arr[0]).Data; ////IE fix
                    var field2 = arr[1];
                    var sfield3val;
                    var field3 = FieldValues.filter(function (x) { return x.Label === arr[2]; });
                    if (field3 && field3.length > 0) {
                        sfield3val = field3[0].Data;
                    }
                    //var field3 = FieldValues.find(m => m.Label == arr[2]).Data; ////IE fix
                    var f1 = parseFloat(sfield1val);
                    var f2 = parseFloat(sfield3val);
                    if (eval(f1 + field2 + f2)) {

                    }
                    else {
                        IsValid = false;
                    }
                }
                if (FieldOrigin.sName == "rOverrideQuote" || FieldOrigin.sName == "rOverrideAdmin") {
                    var sfield1val;
                    var field1 = FieldValues.filter(function (x) { return x.Label === "rDefaultQuote" });
                    if (field1 && field1.length > 0) {
                        sfield1val = field1[0].Data;
                    }
                    var EqComp = "==";
                    //var field1 = FieldValues.find(m => m.Label == "rDefaultQuote").Data;////IE fix
                    var field2 = "!=";
                    var sfield3val;
                    var field3 = FieldValues.filter(function (x) { return x.Label === "rOverrideQuote" });
                    if (field3 && field3.length > 0) {
                        sfield3val = field3[0].Data;
                    }
                    //var field3 = FieldValues.find(m => m.Label == "rOverrideQuote").Data;////IE fix
                    var f1 = parseFloat(sfield1val);
                    var f2 = parseFloat(sfield3val);
                    var sDefaultAdminval;
                    var DefaultAdmin = FieldValues.filter(function (x) { return x.Label === "rDefaultAdmin" });
                    if (DefaultAdmin && DefaultAdmin.length > 0) {
                        sDefaultAdminval = DefaultAdmin[0].Data;
                    }
                    //var DefaultAdmin = FieldValues.find(m => m.Label == "rDefaultAdmin").Data;////IE fix
                    var sOverrideAdminval;
                    var OverrideAdmin = FieldValues.filter(function (x) { return x.Label === "rOverrideAdmin" });
                    if (OverrideAdmin && OverrideAdmin.length > 0) {
                        sOverrideAdminval = OverrideAdmin[0].Data;
                    }
                    //Override Commision
                    var sDefaultCommisionval;
                    var DefaultCommision = FieldValues.filter(function (x) { return x.Label === "rDefaultCommision" });
                    if (DefaultCommision && DefaultCommision.length > 0) {
                        sDefaultCommisionval = DefaultCommision[0].Data;
                    }
                    //var DefaultAdmin = FieldValues.find(m => m.Label == "rDefaultAdmin").Data;////IE fix
                    var sOverrideCommisionval;
                    var OverrideCommision = FieldValues.filter(function (x) { return x.Label === "rOverrideCommision" });
                    if (OverrideCommision && OverrideCommision.length > 0) {
                        sOverrideCommisionval = OverrideCommision[0].Data;
                    }
                    //var OverrideAdmin = FieldValues.find(m => m.Label == "rOverrideAdmin").Data;////IE fix
                    var rDefaultAdmin = parseFloat(sDefaultAdminval);
                    var rOverrideAdmin = parseFloat(sOverrideAdminval);
                    var rDefaultCommision = parseFloat(sDefaultCommisionval);
                    var rOverrideCommision = parseFloat(sOverrideCommisionval);
                    if (eval(f1 + field2 + f2) || eval(rDefaultAdmin + field2 + rOverrideAdmin) || eval(rDefaultCommision + field2 + rOverrideCommision)) {
                        SecDef.FieldDefs.sCode.FieldOrigin.bIsMandatory = true;
                    }
                    else {
                        SecDef.FieldDefs.sCode.FieldOrigin.bIsMandatory = false;
                    }
                    if (IsOverRide == "True" && FieldOrigin.sName == "rOverrideQuote" && eval(f1 + EqComp + f2)/* && QSDID == 1337 && (sQuoteStatus == 10 || sQuoteStatus == 20)*/) {
                        IsValid = false;
                    }
                }
                if (!Value) {
                    IsValid = false;
                }
            }
        }
        else if (XIDataType.sBaseDataType == "boolean") {
            if (FieldOrigin.bIsMandatory) {
                if (Value == "false" || !Value) {
                    IsValid = false;
                }
            }
        }
        else if (XIDataType.sName == "password") {
            if (FieldOrigin.bIsMandatory) {
                if (FieldOrigin.bIsCompare) {
                    var CompareField = FieldOrigin.sCompareField;
                    if (CompareField != null) {
                        var sval;
                        var sDat = FieldValues.filter(function (x) { return x.Label === CompareField });
                        if (sDat && sDat.length > 0) {
                            sval = sDat[0].Data;
                        }
                        //if (Value == FieldValues.find(m => m.Label == CompareField).Data) {
                        if (Value == sval) {
                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
                else if (Value == "" || !Value == null) {
                    IsValid = false;
                }
            }
        }
        else {
            if (FieldOrigin.bIsMandatory) {
                if ((XIDataType.sStartRange && XIDataType.sStartRange > 0) || (XIDataType.sEndRange && XIDataType.sEndRange > 0)) {
                    if (XIDataType.sStartRange && XIDataType.sEndRange) {
                        if (Value && Value.length >= XIDataType.sStartRange && Value.length <= XIDataType.sEndRange) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                    else if (XIDataType.sStartRange && Value.length > XIDataType.sStartRange) {
                        if (Value && Value.length >= XIDataType.sStartRange) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                    else if (XIDataType.sEndRange && Value.length < XIDataType.sEndRange) {
                        if (Value && Value.length <= XIDataType.sEndRange) {

                        }
                        else {
                            IsValid = false;
                        }
                    }
                }
            }
        }
        if (FieldOrigin.bIsMandatory && FieldOrigin.bIsCompare) {
            var CompareField = FieldOrigin.sCompareField;
            if (CompareField != null) {
                var sval;
                var sDat = FieldValues.filter(function (x) { return x.Label === CompareField });
                if (sDat && sDat.length > 0) {
                    sval = sDat[0].Data;
                }
                //if (Value == FieldValues.find(m => m.Label == CompareField).Data) {
                if (sval && Value == sval) {
                }
                else {
                    IsValid = false;
                }
            }
            else {
                if (FieldOrigin.sScript != null && FieldOrigin.iScriptType == 20) {
                    $.ajax({
                        async: false,
                        type: 'POST',
                        url: ExecuteXIScriptURL,
                        data: JSON.stringify({ sScript: FieldOrigin.sScript, sGUID: sGUID }),
                        contentType: 'application/json;',
                        dataType: 'json',
                        traditional: true,
                        success: function (data) {
                            if (data != null && Value != data) {
                                IsValid = false;
                            }
                        }
                    });
                }
            }
        }
        else if (FieldOrigin.bIsMandatory && FieldOrigin.sScript != null && FieldOrigin.sScript != "") {
            var sScript = FieldOrigin.sScript;
            for (var i = 0; i < FieldValues.length; i++) {
                var FieldName = FieldValues[i].Label;
                //var VisibilityF = $('.LayoutCode_@Model.sGUID').find('div[data-controltype="qsfield"]').filter('.on').find('[name="' + FieldName + '"]')[0];
                //var VisibilityS = $('.LayoutCode_@Model.sGUID').find('div[data-controltype="section"]').filter('.on').find('[name="' + FieldName + '"]')[0];
                if (sScript.indexOf("QS." + FieldName) != -1) {
                    sScript = sScript.replace("QS." + FieldName, FieldValues[i].Data)
                }
                //else if (data.sValue.indexOf("QS." + FieldName) != -1 && (VisibilityF || VisibilityS)) {
                //    data.sValue = data.sValue.replace("QS.", "")
                //}
            }
            $.ajax({
                async: false,
                type: 'POST',
                url: ExecuteXIScriptURL,
                data: JSON.stringify({ sScript: sScript, sGUID: sGUID }),
                contentType: 'application/json;',
                dataType: 'json',
                traditional: true,
                success: function (result) {
                    if (result != null && result == 'true') {
                    }
                    else {
                        IsValid = false;
                    }
                }
            })
        }

        if (!IsValid) {
            if (sValidateAttr.length == 0) {
                var sValidateAttrSelector = "input[name='" + FieldOrigin.sName + "']";
                sValidateAttr.push(sValidateAttrSelector);
                sValidateAttrSelector = "select[name='" + FieldOrigin.sName + "']";
                sValidateAttr.push(sValidateAttrSelector);
            }
            FinalValid = false;
            var FieldOrgID = FieldOrigin.ID.toString();
            if (bUIDRef == 'true' || bUIDRef == 'True') {
                FieldOrgID = FieldOrigin.XIGUID.toString();
            }
            $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).show();
            if (ValidationType == 10) {
                $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).html('<img src="' + sImagePath + '/input-failure.png" width="20" height="20" />');
            }
            else if (FieldOrigin.sValidationMessage != null) {
                $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).html(FieldOrigin.sValidationMessage);
                //$('.highlight--help').addClass('msg-error');
                $('.LayoutCode_' + sGUID).find('.input-success-error').remove();
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).html("<div>" + FieldOrigin.sValidationMessage + "</div>");
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).closest('.highlight--help').addClass('msg-error');
            }
            else {
                $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).html(XIDataType.sValidationMessage);
                //$('.highlight--help').addClass('msg-error');
                $('.LayoutCode_' + sGUID).find('.input-success-error').remove();
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).html("<div>" + XIDataType.sValidationMessage + "</div>");
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).closest('.highlight--help').addClass('msg-error');
            }
            $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
            $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
        }
        else {
            if (ValidationType == 10) {
                $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).html('<img src="' + sImagePath + '/input-success.png" width="20" height="20" />');
            }
            else {
                $('.LayoutCode_' + sGUID).find('.ErrorMsg-' + StepID + '-' + FieldOrgID).hide();
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).html('');
                $('.LayoutCode_' + sGUID).find('.field-errmsg-' + StepID + '-' + FieldOrgID).closest('.highlight--help').removeClass('msg-error');
            }
        }
    }
}

function fncValidateStep(Script, ValidationMsg, StepID, FieldValues, sGUID) {

    if (Script != null) {
        var sScript = Script;
        for (var i = 0; i < FieldValues.length; i++) {
            var FieldName = FieldValues[i].Label;
            //var VisibilityF = $('.LayoutCode_@Model.sGUID').find('div[data-controltype="qsfield"]').filter('.on').find('[name="' + FieldName + '"]')[0];
            //var VisibilityS = $('.LayoutCode_@Model.sGUID').find('div[data-controltype="section"]').filter('.on').find('[name="' + FieldName + '"]')[0];
            if (sScript.indexOf("QS." + FieldName) != -1) {
                sScript = sScript.replace("QS." + FieldName, FieldValues[i].Data)
            }
            //else if (Script.indexOf("QS." + FieldName) != -1 && (VisibilityF || VisibilityS)) {
            //    data.sValue = data.sValue.replace("QS.", "")
            //}
        }
        $.ajax({
            async: false,
            type: 'POST',
            url: ExecuteXIScriptURL,
            data: JSON.stringify({ sScript: sScript, sGUID: sGUID }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function (result) {
                if (result != null && result == 'true') {
                }
                else {
                    IsValid = false;
                }
            }
        })
        if (!IsValid) {
            FinalValid = false;
            $('.LayoutCode_' + sGUID).find('.Step-errmsg-' + StepID).html("<div>" + ValidationMsg + "</div>");
            $('.LayoutCode_' + sGUID).find('.Step-errmsg-' + StepID).closest('.highlight--help').addClass('msg-error');
            $('.LayoutCode_' + sGUID).find('#QSLoadingBtn').hide();
            $('.LayoutCode_' + sGUID).find('#QSNxtBtn').show();
        }
    }
}

function fncSetParam(QuoteID, sGUID, Name, sBO, sType, sSession) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: SetParamUrl,
            data: JSON.stringify({ sID: QuoteID, sGUID: sGUID, sName: Name, sBO: sBO, sType: sType, sSession: sSession }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function () {
                resolve(true);
            }
        });
    });
}

function fncHideShowFromHTMLTree(parentName, childObj, Type) {
    if (childObj) {
        var ActiveInstanceID = 0;
        var testObj = childObj.parentNode;
        var count = 1;
        if (testObj.className != "questionset-section") {//This Codition without ParentNode
            while (testObj.getAttribute('data-controltype') != parentName) {// This Condition With ParentNode
                if (testObj.parentNode.tagName != "HTML") {
                    testObj = testObj.parentNode;
                    count++;
                }
                else {
                    return ActiveInstanceID;
                }
            }
        } else {
            testObj = childObj;
        }

        if (Type == "show") {
            $(testObj).removeClass('on').addClass('off');
        }
        else if (Type == "hide") {
            $(testObj).removeClass('off').addClass('on');
        }
    }
}
function fncGetFormIIDFromHTMLTree(This) {
    var sFormIID = 0;
    var HTMLObj = This.parentNode;
    if (HTMLObj) {
        var obj = $(HTMLObj).find('form').length;
        if (obj != 0) {
            var sForm = $(HTMLObj).find('form')[0];
            if (sForm) {
                sFormIID = $(sForm).attr('data-instanceid');
                return sFormIID;
            }
        }
        else {
            //HTMLObj = HTMLObj.parentNode;
            var sParentFormIID = fncGetFormIIDFromHTMLTree(HTMLObj);
            if (sParentFormIID && sParentFormIID.length > 0 && sParentFormIID != 0) {
                return sParentFormIID;
            }
            else {
                return sFormIID;
            }
        }
    }
}
function fncGetFormDataTypeFromHTMLTree(This) {
    var sFormType = "";
    var HTMLObj = This.parentNode;
    if (HTMLObj) {
        var obj = $(HTMLObj).find('form').length;
        if (obj != 0) {
            var sForm = $(HTMLObj).find('form')[0];
            if (sForm) {
                sFormType = $(sForm).attr('data-type');
                return sFormType;
            }
        }
        else {
            var sParentFormType = fncGetFormDataTypeFromHTMLTree(HTMLObj);
            if (sParentFormType && sParentFormType.length > 0 && sParentFormType != 0) {
                return sParentFormType;
            }
            else {
                return sFormType;
            }
        }
    }
}

function fncShowDialogTitle(MenuName, sOnLoadGUID, sDlgID) {
    $.ajax({
        type: 'POST',
        url: GetCacheParameterValueURL,
        data: { sParamName: "{XIP|iQSInstanceID}", sGUID: sOnLoadGUID },
        cache: false,
        async: true,
        dataType: 'json',
        success: function (oResponse) {
            var Value = oResponse["{XIP|iQSInstanceID}"];
            if (Value && Value.length > 0) {
                $('.' + sDlgID).parent().find('.ui-dialog-title').html('<span class="fc-head">' + Value + '</span>');
            }
            else {
                if (MenuName) {
                    $('.' + sDlgID).parent().find('.ui-dialog-title').html('<span class="fc-head">' + MenuName + '</span>');
                }
            }
        }
    });
}

//$(document).ready(function () {
//    $(window).resize(function () {
//        $('embed').height($('.PopupTabContentArea').height() - 100);
//    }).trigger('resize');
//});
$(document).ready(function () {
    $(window).resize(function () {
        $('.conSection .PopupTabContentArea').height() - 50;
    }).trigger('resize');
});

function fncGetQSDefByInstanceID(iQSInstanceID) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: GetQSDefinitionURL,
            data: JSON.stringify({ sQSIID: iQSInstanceID, sType: "Public" }),
            contentType: 'application/json;',
            dataType: 'json',
            traditional: true,
            success: function (data) {
                resolve(data);
            }
        });
    });
}

function fncGetPolicyMoreDetails(sXiLinkID, sGUID, sClassID, sCode) {
    $.ajax({
        type: 'POST',
        url: GetTemplateDefinitionURL,
        data: JSON.stringify({ sClassID: sClassID, sCode: sCode, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            XILinkLoadJson(sXiLinkID, sGUID, null)
        }
    });

}

function fncTriggerXILink(sGUID, _layout) {
    $.ajax({
        type: 'POST',
        url: TriggerXILinkURL,
        data: { sGUID: sGUID },
        cache: false,
        async: true,
        dataType: 'json',
        success: function (oResponse) {
            $(_layout).find('span[data-fid="rShort"]').html(oResponse.short);
            $(_layout).find('span[data-fid="rMedium"]').html(oResponse.medium);
            $(_layout).find('span[data-fid="rLong"]').html(oResponse.long);
            $(_layout).find('span[data-fid="rLongPlus"]').html(oResponse.longplus);
            $(_layout).find('span[data-fid="rBalance"]').html(oResponse.Balance);
            $(_layout).find('#PopupAlert').html(oResponse.AlertInfo);
        }
    });
}

//function fncAddTriggerXILink(iXILinkID, sGUID) {
//    $.ajax({
//        type: 'POST',
//        url: AddTriggerXILinkURL,
//        data: { iXILinkID: iXILinkID, sGUID: sGUID },
//        cache: false,
//        async: true,
//        dataType: 'json',
//        success: function (oResponse) {
//            var Data = oResponse.Data;
//        }
//    });
//}

function fncGetDriverCount(sScript, sGUID, sName) {
    $.ajax({
        async: false,
        type: 'POST',
        url: ExecuteXIScriptURL,
        data: JSON.stringify({ sScript: sScript, sGUID: sGUID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            if (data != null) {
                fncSetParam(data, sGUID, sName, "");
            }
        }
    });
}
function fncRegenerateQuotes($this, XILinkID, iQuoteID) {
    var NewGUID = CreateGuid();
    fncSetParam(iQuoteID, NewGUID, "-iQuoteID", "").then(function (state, callback) {
        XIRun($this, XILinkID, 0, NewGUID, "Aggregations", false);
    });
}
function fncCopyQS($this, XILinkID, iInstanceID) {

    var NewGUID = CreateGuid();
    if (iInstanceID == undefined || iInstanceID == "") {
        var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
        var oQSInstance = QSInsDict[sGUID];
        iInstanceID = oQSInstance.XIGUID;
    }
    fncSetParam(iInstanceID, NewGUID, "{-iInstanceID}", "").then(function (state, callback) {
        XIRun($this, XILinkID, iInstanceID, NewGUID, "QS Instance", false);
    });
}

$('#Popuptbl').on('click', 'input.Preview', function () {

    var ID = $(this).attr('id');
    var model = { XiLinkID: ID };
    $.ajax({
        type: 'POST',
        url: XIPreviewURL,
        data: model,
        dataType: 'json',
        success: function (oXiLink) {

            //var oXiLink = JSON.parse(data);
            var PopGUID = CreateGuid();
            var _uidialog = $('.LayoutCode_' + PopGUID);
            if (oXiLink != null) {
                if (oXiLink.oContent.hasOwnProperty("dialog")) {
                    var oDialogIns = oXiLink.oContent["dialog"];
                }
                else if (oXiLink.oContent.hasOwnProperty("layout")) {
                    fncRenderlayoutcontent(oXiLink, _uidialog).then(function (status) {
                    });
                }
                else if (oXiLink.oContent.hasOwnProperty("component")) { }
                else if (oXiLink.oContent.hasOwnProperty("xilink")) {
                    var oXiLinkI = oXiLink.oContent["xilink"];
                }
            }
        },
        error: function (oXiLink) {

        }
    });
})

function fncDialogPopout(_this) {
    var sHTML = $(_this).parent('.xicomponent');
    var sPopData = $($(sHTML)[0]).attr('data-id');
    $('.fa-external-link-alt').remove();
    var sDataSplit = sPopData.split('_');
    var sID = sDataSplit[0];
    var sName = sDataSplit[1];
    var bParentTaskBar = true;
    var sDlgDisplayTitle = sPopData;
    var sGUID = CreateGuid();
    var DialogDivID = '.compPop-' + sGUID;
    var sDialogDivID = 'compPop-' + sGUID;
    var BtnID = "DialogBarBtn-" + sName + "-" + sID;

    var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h" title="" onclick="fncdialogchange(this, &quot;maxwidth&quot;)"></i>';
    var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v" onclick="fncdialogchange(this, &quot;maxheight&quot;)"></i>';
    var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minwidth&quot;)"></i>';
    var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minheight&quot;)"></i>';
    var MinDia = '<i class="Minimize fa fa-window-minimize" onclick="fncdialogchange(this, &quot;minimize&quot;)"></i>';
    var MaxDia = '<i class="Maximize far fa-window-maximize" onclick="fncdialogchange(this, &quot;maximize&quot;)"></i>';
    var RestoreDia = '<i class="RestoreDown far fa-window-restore" onclick="fncdialogchange(this, &quot;restore&quot;)"></i>';
    var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + true + ', &quot;' + sDialogDivID + '&quot;)"></i>';
    var InPopup = '<i class="openinpopup fa fa-external-link-alt" onclick="fncOpenInPopups(&quot;' + $(_this).parent('.xicomponent') + '&quot;, &quot;' + sDialogDivID + '&quot;, &quot;' + sGUID + '&quot;)"></i>';
    var RefreshPopup = '<i class="refreshpopup fa fa-sync" onclick="fncRefreshPopup(&quot;' + sDialogDivID + '&quot;, &quot;' + sGUID + '&quot;, ' + 1 + ' )"></i>';

    var DialogHeight = 800;

    $('#Dialogs').append("<div class=compPop-" + sGUID + "></div>");
    $('.compPop-' + sGUID).html(sHTML);
    $(".compPop-" + sGUID).dialog({
        title: ' ',
        width: 800,
        height: 600,
        buttons: [
        ],
        open: function () {
            $(this).parent().promise().done(function () {
                var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + RefreshPopup + InPopup + MinDia + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();
                $(this).children('.ui-dialog-title').html('<span class="fc-red">Alert message !!!</span>');
                uidialog = $(this);
                var dlgStyle = uidialog[0].attributes["style"].textContent;
                if (dlgStyle && dlgStyle.length > 0) {
                    var Styles = dlgStyle.split(";");
                    for (var k = 0; k < Styles.length; k++) {
                        var Sty = Styles[k].trim();
                        var st = Sty.split(":");
                        if (st[0].trim() == "width") {
                            dlgWidth = st[1].trim();
                        }
                        else if (st[0].trim() == "height") {
                            dlgHeight = DialogHeight + "px";
                        }
                        else if (st[0].trim() == "top") {
                            dlgTop = st[1].trim();
                        }
                        else if (st[0].trim() == "left") {
                            dlgLeft = st[1].trim();
                        }
                    }
                    $(this).attr('data-dlgWidth', dlgWidth);
                    $(this).attr('data-dlgHeight', dlgHeight);
                    $(this).attr('data-dlgTop', dlgTop);
                    $(this).attr('data-dlgLeft', dlgLeft);
                    $(this).attr('data-identity', BtnID);
                }

            });
        },
        close: function (event, ui) {
            $('#Dialogs').parent().hide();
            $(this).dialog('destroy').remove();
        }
    }).dialog("widget")
        .dblclick(function () {
            $(this).toggleClass("fullScreenToggle");
        });

    if (bParentTaskBar) {
        var Btns = $('#NavigationBar').find('.btnTabs');

        if (Btns.length > 0) {
            var VBar = '<div class="dialogNavBtn" id ="' + sGUID + '"><button type="button" data-navtype="single" class="btn btn-theme btn-xs taskbtn" id="' + BtnID + '" onclick="return fncTaskBarNavigate(&quot;' + BtnID + '&quot;)" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + sDlgDisplayTitle + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + sDialogDivID + '&quot;, this)">&times;</div></span></div>';
            $('#NavigationBar').find('.btnTabs').append(VBar);
        }
        else {
            var MinMax = '<div class="dialogNavBtn"><button type="button" data-navtype="single" class="btn btn-theme btn-xs" onclick="return fncCloseAllDialogs()" ><i class="fa fa-home"></i></button><span class="hoverText" >Minimise<div class="closeNavBtn">&times;</div></span></div><div class="dialogNavBtn"><button type="button" data-navtype="single" class="btn btn-theme btn-xs" onclick="return fncOpenAllDialogs()" ><i class="far fa-window-maximize"></i></button><span class="hoverText" >Maximise<div class="closeNavBtn">&times;</div></span></div>';
            var VBar = '<div class="btnTabs left">' + MinMax + '<div class="dialogNavBtn" id ="' + sGUID + '"><button type="button" data-navtype="single" class="btn btn-theme btn-xs taskbtn" id="' + BtnID + '" onclick="return fncTaskBarNavigate(&quot;' + BtnID + '&quot;)" ><i class="fa fa-user-plus" ></i></button><span class="hoverText" >' + sDlgDisplayTitle + '<div class="closeNavBtn" onclick="return fncCloseNavBtn(&quot;' + sDialogDivID + '&quot;, this)">&times;</div></span></div></div>';
            $('#NavigationBar').append(VBar);
        }
    }
}

function fncGetNextHTMLRows(_this, sParams, sDivID, sGUID, sVisualisation) {
    $('#' + sDivID).block({
        message: '<h4> Processing ... </h4>',
        blockMsgClass: 'report-success',
    });
    var myCount = $(_this).attr('data-count');
    var iPageCount = parseInt(myCount) + 1;
    $.ajax({
        url: GetNextHTMLRowsURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ sParams: sParams, iPageCount: iPageCount, sGUID: sGUID, sVisualisation: sVisualisation }),
        success: function (data) {
            $('#' + sDivID).unblock();
            $('#' + sDivID).html(data);
        },
        error: function (data) {
            $('#' + sDivID).html("Error Occured");
        }
    })
}


function fncGetPrevHTMLRows(_this, sParams, sDivID, sGUID, sVisualisation) {
    $('#' + sDivID).block({
        message: '<h4> Processing ... </h4>',
        blockMsgClass: 'report-success',
    });
    var myCount = $(_this).attr('data-count');
    var iPageCount = parseInt(myCount) - 1;
    $.ajax({
        url: GetNextHTMLRowsURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ sParams: sParams, iPageCount: iPageCount, sGUID: sGUID, sVisualisation: sVisualisation }),
        success: function (data) {
            $('#' + sDivID).unblock();
            $('#' + sDivID).html(data);
        },
        error: function (data) {
            $('#' + sDivID).html("Error Occured");
        }
    })
}


function fncLoadConfig(sAction, sController, sHostingPath, sName) {
    var sURL = sHostingPath + "/" + sController + "/" + sAction + "?sType=Load";
    $.ajax({
        url: sURL,
        type: 'GET',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        success: function (data) {
            var DialogDivID = sAction + "-div";
            var Div = '<div class="' + DialogDivID + '"></div>';
            $('#Dialogs').html(Div);
            $("." + DialogDivID).html(data);
            var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
            $("." + sAction + "-div").dialog({
                title: sName,
                //modal: true,
                height: 600,
                width: 1200,
                open: function () {
                    $(this).parent().promise().done(function () {
                        $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + windowclose + '</div>');
                    })
                }
            });
        },
        error: function (data) {

        }
    })
}

function fncGetParentDDL(sBO) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: GetParentDDLURL,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: true,
            cache: false,
            data: JSON.stringify({ sBO: sBO }),
            success: function (data) {
                resolve(data);
            },
            error: function (data) {

            }
        })
    })
}

function fncXIScriptEditor(_this, sAttr, FKiBOID, FKiBOAttributeID, sBOName, sAttrName) {
    var sValue = $('#' + sAttr).val();
    $.ajax({
        url: GetXIScriptEditorURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        data: JSON.stringify({ sValue: sValue, FKiBOID: FKiBOID, FKiBOAttributeID: FKiBOAttributeID, sBOName: sBOName, sAttrName: sAttrName }),
        async: true,
        cache: false,
        success: function (data) {
            var DialogDivID = "XIScriptEditor-div";
            var Div = '<div class="' + DialogDivID + '"></div>';
            $('#Dialogs').html(Div);
            $("." + DialogDivID).html(data);
            var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
            $("." + DialogDivID).dialog({
                title: 'XI Script Editor',
                //modal: true,
                height: 850,
                width: 1600,
                open: function () {
                    $(this).parent().promise().done(function () {
                        $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + windowclose + '</div>');
                    })
                }
            });
        },
        error: function (data) {

        }
    })
}

function fncValidateXIScript(_this, ScriptID) {
    var Type = $('#Editor-' + ScriptID).find('#XIScriptType :selected').val();
    var sScript = CodeEditor.getValue();
    $.ajax({
        url: GetXIScriptValidatorURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ XIScript: sScript, sScriptType: Type }),
        success: function (data) {

        },
        error: function (data) {

        }
    })
}

function fncLoadScriptToEditor(_this, sScrptId, sType) {
    var iScriptID = $(_this).attr('id');
    $.ajax({
        url: LoadScriptToEditorURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ iScriptID: iScriptID }),
        success: function (data) {
            if (sType && sType == "Element Template") {
                fncInjectScript(data);
            }
            else {
                $('#' + sScrptId).find('.CodeMirror').remove();
                $('#' + sScrptId).find('#Editor-' + sScrptId).html(data);
                fncLoadEditor(sScrptId);
            }
            //$('#XIEditor').html(data);
        },
        error: function (data) {

        }
    })
}

function fncAddNewScript(sScrptId) {
    $.ajax({
        url: AddNewScriptURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ sScrptId: sScrptId }),
        success: function (data) {
            $('#XIEditor').html(data);
        },
        error: function (data) {

        }
    })
}

function fncLoadEditor(sScrptId) {
    CodeMirror.commands.autocomplete = function (cm) {
        cm.showHint({ hint: CodeMirror.hint.anyword });
    }
    var source = { app: ["name", "score", "birthDate"], version: ["name", "score", "birthDate"], dbos: ["name", "population", "size"] };
    CodeEditor = CodeMirror.fromTextArea(document.getElementById('Editor-' + sScrptId), {
        mode: "text/x-csharp",  //c#??
        //lineNumbers: true,     // ????
        theme: '3024-night',
        indentUnit: 4,         // ?????4
        matchBrackets: true,   // ???? Set to true matching brackets to be highlighted whenever the cursor is next to them.
        autoCloseBrackets: true,//???? that will auto-close brackets and quotes when typed.
        lineWrapping: false,   // ????
        continueLineComment: true,
        draganddrop: true,        //????
        showCursorWhenSelecting: true, //????
        hintOptions: {
            tables: source
        },
        extraKeys: { "Ctrl-Space": "autocomplete" } //????
    });
}

function fncInjectScript(data) {
    var cm = $('.CodeMirror')[0].CodeMirror;
    var doc = cm.getDoc();
    var cursor = doc.getCursor(); // gets the line number in the cursor position
    //var line = doc.getLine(cursor.line); // get the line contents
    //var pos = { // create a new object to avoid mutation of the original selection
    //    line: cursor.line,
    //    ch: line.length - 1 // set the character position to the end of the line
    //}
    doc.replaceRange(data, cursor); // adds a new line
}

function fncMergeScriptParams(sScrptId) {
    var EditorText = CodeEditor.getValue();
    re = /\[{.*?\}]/g;
    var matches = EditorText.match(re);
    if (matches) {
        var Dialog = '<div id="XIScriptParams"><div class="row"><div class="box box-primary"><div class="box-body">';
        for (var i = 0; i < matches.length; i++) {
            var Match = matches[i].replace("[{", "").replace("}]", "");
            Dialog = Dialog + '<div class="form-group col-md-12 NVPairs"><div class="col-md-4"><label for="inputEmail" class="gn"> ' + Match + ' <span class="danger"></span></label></div>';
            Dialog = Dialog + '<div class="col-md-5"><input type="text" name="Names" placeholder="Value" class="form-control" value="" data-name="' + Match + '" /></div></div>';
        }
        Dialog = Dialog + '<div class="form-group"><div class="col-md-2"><input type="button" value="Replace" class="btn btn-theme btnQuote replacebtn" /></div></div></div></div></div></div>';
        var DialogDivID = "XIScriptParamMerge-div";
        var Div = '<div class="' + DialogDivID + '"></div>';
        $('#Dialogs').html(Div);
        $("." + DialogDivID).html(Dialog);
        var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
        $("." + DialogDivID).dialog({
            title: 'XI Script Param Merge',
            //modal: true,
            height: 450,
            width: 800,
            open: function () {
                $(this).parent().promise().done(function () {
                    $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + windowclose + '</div>');
                })
            }
        });
    }
}


$(document).on('click', 'input.replacebtn', function () {
    var EditorText = CodeEditor.getValue();
    var Params = $('#XIScriptParams').find('div.NVPairs');
    for (var i = 0; i < Params.length; i++) {
        var Param = Params[i];
        var Name = $(Params[i]).find('input[type=text]').attr('data-name');
        var Value = $(Params[i]).find('input[type=text]').val();
        EditorText = EditorText.replace("[{" + Name + "}]", Value);
    }
    CodeEditor.getDoc().setValue(EditorText);
    fncdialogclose(this, false, 'XIScriptParamMerge-div');
})


function fncFilterInstanceTree1(_this, sType) {
    //var BuildingID = $('.BuildBtns').find('button.active');
    var bid = $('.slctpro').attr('data-bid');
    var SearchText = $(_this).val();
    var sParentID = $('.slctpro').attr('data-parentid');
    var sFolder = $('.slctpro').attr('data-foldername');
    var iBODID = $(_this).attr('data-ibodid');
    var sGUID = $(_this).attr('data-treeguid');
    //if (SearchText.length > 0) {
    if (changeTimer !== false) clearTimeout(changeTimer);
    changeTimer = setTimeout(function () {
        $.ajax({
            type: 'POST',
            url: FilterInstanceTreeURL,
            data: { sSearchText: SearchText, sParentID: sParentID, iBODID: iBODID, iBuildingID: bid, sFolder: sFolder },
            cache: false,
            async: true,
            dataType: 'html',
            success: function (data) {
                var elements = $(data);
                var UL = $(data).find('ul.trNodes');
                var found = $('.instancetree', elements);
                var ULHTML = UL[0].outerHTML;
                $('#TreeStrctr-' + sGUID).find('.instancetree').find('ul.trNodes').replaceWith(ULHTML);
            }, error: function (err) {
            }
        });
        changeTimer = false;
    }, 300);
    //}
}

function fncGetInstanceTree(_this, sType) {
    var bid = $('.slctpro').attr('data-bid');
    var sParentID = $(_this).attr('data-nodeid');
    var Level = $(_this).attr('data-level');
    if (Level == 1) {
        sParentID = "";
    }
    var sFolder = $('.slctpro').attr('data-foldername');
    var iBODID = $(_this).attr('data-ibodid');
    var Level = $(_this).attr('data-level');
    console.log("Level" + Level);
    Level++;
    $.ajax({
        type: 'POST',
        url: GetInstanceTreeURL,
        data: { sParentID: sParentID, iBODID: iBODID, iBuildingID: bid, sFolder: sFolder },
        cache: false,
        async: true,
        dataType: 'json',
        success: function (data) {
            if (data && data.length > 0) {
                var bHasChilds;
                var sHTMLNav = '<div class="side-nav-level-' + Level + ' submenu"><ul class="sub-level-content">';
                for (var b = 0; b < data.length; b++) {
                    for (var key in data[b].Attributes) {
                        var id; var parentid; var name; var type;
                        if (data[b].Attributes.hasOwnProperty(key)) {
                            if (key == 'id') {
                                id = data[b].Attributes[key].sValue;
                            }
                            else if (key == 'sparentid') {
                                parentid = data[b].Attributes[key].sValue;
                            }
                            else if (key == 'sname') {
                                name = data[b].Attributes[key].sValue;
                            }
                            else if (key == 'stype') {
                                type = data[b].Attributes[key].sValue;
                            }
                        }
                    }
                    bHasChilds = data[b].bHasChilds;
                    if (bHasChilds) {
                        sHTMLNav = sHTMLNav + '<li data-level="' + Level + '" onmouseenter="fncLoadMenuContent(this)" onclick="fncLoadMenuContent1(this)" data-name="' + name + '" data-nodeid="' + id + '"><a class="level-dropdown">' + name + '</a></li>'
                    }
                    else {
                        sHTMLNav = sHTMLNav + '<li data-level="' + Level + '" onmouseenter="fncLoadMenuContent(this)" onclick="fncLoadMenuContent1(this)" data-name="' + name + '" data-nodeid="' + id + '"><a class="">' + name + '</a></li>'
                    }

                }
                sHTMLNav = sHTMLNav + '</ul></div>';
                var divid = "side-nav-level" + Level;
                var isExists = $(_this).find('.' + divid);
                if (isExists.length > 0) {
                    $(_this).find('.' + divid).replaceWith(sHTMLNav);
                }
                else {
                    $(_this).append(sHTMLNav);
                }
            }
        }, error: function (err) {
        }
    });
}

function SearchData($this) {
    $.blockUI({
        //'<h4>Please Wait...</h4>'
        message: '<img src="' + sImagePath + '/loading.gif" style="width: 30px;" /> Please wait...',
        blockMsgClass: 'report-success',
    });
    var sGUid = CreateGuid();
    var iInstanceID = $('#iInstanceID').val();
    var StructureID = $('select[name="iStructure"] option:selected').val();
    var TemplateID = $('select[name="sTemplate"] option:selected').val();
    var tempvalue = {
        iStructureID: StructureID,
        iInstanceID: iInstanceID,
        sTemplateID: TemplateID,
        sGUID: sGUid
    }
    $('#HTMLDataResult').html('');
    $.ajax({
        url: HTMLDataTemplate,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "HTML",
        cache: false,
        data: JSON.stringify(tempvalue),
        success: function (data) {
            $('#HTMLDataResult').html('<div><button style="float: right;" id="myButton" class="label-success btn btn-success btn-xs" onclick=DownloadfncPDFRecipt("' + iInstanceID + '","' + sGUid + '","' + StructureID + '","' + TemplateID + '") >Export to PDF</button><div id="imgh" style="float: right;"><img src="' + sImagePath + '/loading.gif"/></div></div>' + data);
            $('#imgh').hide();
            $.unblockUI();
        },
        error: function (textStatus, errorThrown) {
            $('#HTMLDataResult').html('<label><strong>No Records Found</strong></label>');
            $.unblockUI();
        }
    });
}
//DashBoard Snippet
function fncDeviceGaugeChart(JsonData) {
    if (JsonData != null) {
        var data = '';
        var sites = '';
        $.each(JsonData.OneClickRes, function (i, item) {
            data = item;
            sites = parseInt(i);
        });
        var Name = "#gaugechart-" + JsonData.OnClickResultID;
        if (Name && Name.length > 0) {
            var gaugechart = c3.generate({
                bindto: Name,
                data: {
                    columns: [
                        [data, sites]
                    ],
                    type: 'gauge'
                },
                gauge: {
                    label: {
                    },
                    min: 0,
                    max: 100,
                    width: 25
                },
                size: {
                    height: 130
                }
            });
        }
    }
}
////Bar Graph
//function fncDeviceComChart(jsonData, Keys) {
//    if (jsonData != null) {
//        var Year = [];
//        var chartdata = {};
//        var LeadCount = [];
//        var OnCoverCount = [];
//        var QuoteCount = [];

//        $.each(jsonData, function (i, item) {
//            if (jsonData != null) {
//                Year.push(item.sRowXiLinkType + '-01');
//                LeadCount.push(parseInt(item.LeadCount));
//                OnCoverCount.push(parseInt(item.CategoryID));
//                QuoteCount.push(parseInt(item.iPaginationCount));
//                chartdata.Year = Year;
//                chartdata.LeadCount = LeadCount;
//                chartdata.OnCoverCount = OnCoverCount;
//                chartdata.QuoteCount = QuoteCount;
//            }
//        });
//        if (chartdata.Year.length != 0) {
//            chartdata.Year.unshift('x');
//        }
//        chartdata.LeadCount.unshift('d1');
//        chartdata.OnCoverCount.unshift('d2');
//        chartdata.QuoteCount.unshift('d3');
//        var chart = c3.generate({
//            bindto: '#combinationChart',
//            data: {
//                x: 'x',
//                columns: [chartdata.Year, chartdata.LeadCount, chartdata.OnCoverCount, chartdata.QuoteCount],
//                names: {
//                    d1: Keys[1],
//                    d3: Keys[3],
//                    d2: Keys[2],
//                },
//                type: 'bar',
//                types: {
//                    d2: 'bar',
//                    d3: 'line',
//                },
//                //colors: {
//                //    d1: '#4a8ddc',
//                //    d2: '#4c5d8a',
//                //    d3: '#f3c911',
//                //    //d1: '#e58c7a',
//                //    //d2: '#d789a3',
//                //    //d3: '#eeba6f',

//                //}
//            },
//            bar: {
//                width: {
//                    ratio: 0.5
//                }
//            },
//            legend: {
//                position: 'inset',
//                inset: {
//                    anchor: 'top-left',
//                    x: -5,
//                    y: -35,
//                    step: 1
//                }
//            },
//            padding: {
//                top: 35
//            },
//            size: {
//                height: 220,
//                // width: 600
//            },
//            axis: {
//                x: {
//                    type: 'timeseries',
//                    tick: {
//                        multiline: false,
//                        format: '%Y %B',
//                    },

//                }
//            }
//        });
//    }

//}

//PieChart
function fncDevicepieChart(jsonData) {
    var sPieGUID = "";
    $("#Piechart").empty();
    if (jsonData != null) {
        if (jsonData.PieData.length > 0) {
            var data = {};
            var status = [];
            var sites = [];
            for (i = 0; i < jsonData.PieData.length; i++) {

                if (jsonData.PieData[i].label) {
                    sites.push(jsonData.PieData[i].label);
                    status.push(jsonData.PieData[i].iStatus);
                    data[jsonData.PieData[i].label] = jsonData.PieData[i].value;
                    data[jsonData.PieData[i].name] = jsonData.PieData[i].iStatus;
                }
            }
            var Name = "#Piechart-" + jsonData.ReportID;
            if (jsonData.Type == 'Dashboard') {
                var chart = c3.generate({
                    bindto: Name,
                    data: {
                        json: [data],
                        keys: {
                            value: sites,
                        },
                        type: 'pie',

                        size: {
                            //width: 380,
                            //height: 300
                        },
                        onclick: function (d, element) {
                            if (jsonData.IsColumnClick == true) {
                                var SearchText = jsonData.OnClickColumn + "=" + d.id;
                                var Url = "@Url.Action('ReportResult', 'Inbox')?QueryID=" + jsonData.OnClickResultID + '&PageIndex=1&ResultIn=Popup&SearchText=' + SearchText;
                                window.open(Url, '_blank', 'scrollbars=1,resizable=1,width=' + screen.width + ', height=' + screen.height);
                            }
                        },
                    },
                    tooltip: {
                        format: {
                            value: function (value, ratio, id) {
                                //var format = d3.format('');
                                return d3.format('')(value);
                            }
                        }
                    },
                    size: {
                        //width:380,
                        //height: 300
                    },
                    legend: {
                        position: 'right'
                    },
                });
            }
            else if (jsonData.Type == "Run") {
                var chart = c3.generate({
                    bindto: Name,
                    data: {
                        json: [data],
                        keys: {
                            value: sites,
                        },
                        type: 'pie',

                        size: {
                            //width: 380,
                            //height: 300
                        },
                        onclick: function (d, element) {
                            if (jsonData.IsColumnClick == true) {

                                var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                                var SearchText = d.id;
                                var oParams = [];
                                var Param = {};
                                Param["sName"] = "{XIP|sStatusName}";
                                Param["sValue"] = d.id;
                                oParams.push(Param);
                                XILinkLoadJson(sRowXiLinkID, null, oParams, null);
                            }
                            else {
                            }
                        },
                    },
                    color: {
                        pattern: ['#4a8ddc', '#4c5d8a', '#f3c911', '#dc5b57', '#33ae81', '#95c8f0', '#dd915f', '#DD4477', '#66AA00', '#B82E2E', '#316395', '#994499', '#22AA99', '#AAAA11', '#6633CC', '#E67300', '#8B0707', '#329262', '#5574A6', '#3B3EAC']
                    },
                    tooltip: {
                        format: {
                            value: function (value, ratio, id) {
                                return d3.format('')(value);
                            }
                        }
                    },
                    size: {
                        //width: 380,
                        //height: 300
                    },
                    legend: {
                        position: 'bottom'
                    },
                });
            }
            else {
                var chart = c3.generate({
                    //bindto: "#Piechart",
                    data: {
                        json: [data],
                        keys: {
                            value: sites,
                        },
                        type: 'pie',
                    },
                    tooltip: {
                        format: {
                            value: function (value, ratio, id) {
                                //var format = d3.format('');
                                return d3.format('')(value);
                            }
                        }
                    },
                    size: {
                        //width: 300,
                        //height: 200
                    },
                    legend: {
                        position: 'right'
                    },
                });
            }
        }
    }
}
//Bar Graph
function fncDeviceComChart(jsonData, Keys) {
    if (jsonData != null) {
        var Year = [];
        var chartdata = {};
        var LeadCount = [];
        var OnCoverCount = [];
        var QuoteCount = [];
        $.each(jsonData, function (i, item) {
            if (jsonData != null) {
                for (var itm in item.Attributes) {
                    LeadCount.push((item.Attributes[itm].sValue));
                    OnCoverCount.push(item.Attributes[itm].sName);
                }

                chartdata.Year = Year;
                chartdata.LeadCount = LeadCount;
            }
        });
        var result = {};
        OnCoverCount.forEach((key, i) => result[key] = LeadCount[i]);
        var result1 = Object.keys(result).map(function (key) {
            // Using String() to convert key to String type 
            // Using obj[key] to retrieve key value 
            return [String(key), result[key]];
        });
        chartdata.Data = result1;
        if (jsonData.length > 0) {
            var chart = c3.generate({
                bindto: '#combinationChart-' + parseInt(jsonData.OneClick),
                data: {
                    x: chartdata.Data[0][0],
                    columns: chartdata.Data,
                    type: 'bar',
                    axis: {
                        x: {
                            type: "timeseries",
                            label: true,
                            tick: {
                                format: "%Y-%m-%d"
                            }
                        }
                    },
                    onclick: function (d, element) {
                        var info = $('#combinationChart-' + parseInt(jsonData.OneClick));
                        var thObj = $(info).find('table tr th')[0];
                        var SearchText = $(thObj)[0].innerHTML;
                        var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                        var oParams = [];
                        var Param = {};
                        Param["sName"] = "{XIP|sStatusName}";
                        Param["sValue"] = SearchText;
                        oParams.push(Param);
                        XILinkLoadJson(sRowXiLinkID, null, oParams, null);
                    },
                },
                color: {
                    pattern: ['#4a8ddc', '#4c5d8a', '#f3c911', '#dc5b57', '#33ae81', '#95c8f0', '#dd915f', '#DD4477', '#66AA00', '#B82E2E', '#316395', '#994499', '#22AA99', '#AAAA11', '#6633CC', '#E67300', '#8B0707', '#329262', '#5574A6', '#3B3EAC']
                },
                bar: {
                    width: {
                        ratio: 0.5
                    }
                },
                legend: {
                    //position: 'inset',
                    position: 'center',
                    inset: {
                        anchor: 'top-left',
                        x: -5,
                        y: -35,
                        step: 1
                    }
                },
                padding: {
                    top: 35
                },
                size: {
                    //height: 220,
                    // width: 600
                }, axis: {
                    x: {
                        type: 'category', // this needed to load string x value
                        label: {
                            text: jsonData.BarAxis,
                            position: 'outer-right'
                        }
                    },
                },
            });
            setTimeout(function () {
                chart.resize({
                    height: $(".chartSize .box-body").innerHeight(),
                    //width: $(".chartSize .box-body").innerWidth()
                });
            });
            jQuery(window).trigger('resize');
        }
    }
}
function fncGetGaugeDependency($this, XiLinkID) {
    var obj = [];
    var oNVParams = new Object();
    oNVParams.sName = '{XIP|i1ClickID}';
    oNVParams.sValue = $this.value;
    obj.push(oNVParams);
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    $.ajax({
        url: ContentURL,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        async: true,
        cache: false,
        data: JSON.stringify({ XiLinkID: XiLinkID, sGUID: sGUID, BODID: 0, ID: 0, oNVParams: obj }),
        success: function (data) {
            var sContentCode = "";
            var DialogDivID = "ResultDialog-" + sGUID;
            var windowMaxWidth = '<i class="windowWidth fa fa-arrows-alt-h" title="" onclick="fncdialogchange(this, &quot;maxwidth&quot;)"></i>';
            var windowMaxHeight = '<i class="windowHeight fa fa-arrows-alt-v" onclick="fncdialogchange(this, &quot;maxheight&quot;)"></i>';
            var windowMinWidth = '<i class="windowminWidth fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minwidth&quot;)"></i>';
            var windowMinHeight = '<i class="windowminHeight fa fa-compress-alt" onclick="fncdialogchange(this, &quot;minheight&quot;)"></i>';
            var MinDia = '<i class="Minimize fa fa-window-minimize" onclick="fncdialogchange(this, &quot;minimize&quot;)"></i>';
            var MaxDia = '<i class="Maximize far fa-window-maximize" onclick="fncdialogchange(this, &quot;maximize&quot;)"></i>';
            var RestoreDia = '<i class="RestoreDown far fa-window-restore" onclick="fncdialogchange(this, &quot;restore&quot;)"></i>';
            var windowclose = '<i class="windowClose fa fa-times" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
            var InPopup = '<i class="openinpopup fa fa-external-link-alt" onclick="fncOpenInPopup(&quot;' + "" + '&quot;, ' + 0 + ', ' + 0 + ', &quot;' + sGUID + '&quot;)"></i>';
            //var RefreshPopup = '<i class="refreshpopup fa fa-sync" onclick="fncRefreshPopup(&quot;' + DialogDivID + '&quot;, &quot;' + sGUID + '&quot;, ' + 0 + ' )"></i>';
            var sContentHTML = '<div class="LayoutCode_' + sGUID + ' sys-layout" data-guid="' + sGUID + '" data-name="LayoutGUID">' + data + '</div>';

            var Div = '<div class="dialog-box ' + DialogDivID + '" title="Confirm Message"><a><span class="ui-button-icon-primary ui-icon ui-icon-closethick"></span></a></div>';
            $('#Dialogs').append(Div);
            $("." + DialogDivID).html(sContentHTML);
            $("." + DialogDivID).dialog({
                title: ' ',
                appendTo: "body",
                height: screen.height - 190,
                width: screen.width - 50,
                resizable: true,
                IsCloseIcon: true,
                buttons: [
                ],
                open: function () {
                    $(this).parent().promise().done(function () {
                        var dlgWidth; var dlgHeight; var dlgTop; var dlgLeft;
                        $(this).children('.ui-dialog-titlebar').children("div.dialogIcons").remove();
                        //$(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + RefreshPopup + InPopup + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                        $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons" data-dinfo = "">' + InPopup + MaxDia + RestoreDia + windowMaxWidth + windowMinWidth + windowMaxHeight + windowMinHeight + windowclose + '</div>');
                        $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.RestoreDown').hide();
                        $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminWidth').hide();
                        $(this).children('.ui-dialog-titlebar').children('.dialogIcons').children('i.windowminHeight').hide();

                        uidialog = $(this);
                        var dlgStyle = uidialog[0].attributes["style"].textContent;
                        if (dlgStyle && dlgStyle.length > 0) {
                            var Styles = dlgStyle.split(";");
                            for (var k = 0; k < Styles.length; k++) {
                                var Sty = Styles[k].trim();
                                var st = Sty.split(":");
                                if (st[0].trim() == "width") {
                                    dlgWidth = st[1].trim();
                                }
                                else if (st[0].trim() == "height") {
                                    dlgHeight = screen.height + "px";
                                }
                                else if (st[0].trim() == "top") {
                                    dlgTop = st[1].trim();
                                }
                                else if (st[0].trim() == "left") {
                                    dlgLeft = st[1].trim();
                                }
                            }
                            $(this).attr('data-dlgWidth', dlgWidth);
                            $(this).attr('data-dlgHeight', dlgHeight);
                            $(this).attr('data-dlgTop', dlgTop);
                            $(this).attr('data-dlgLeft', dlgLeft);
                        }
                    });
                },
                close: function (event, ui) {
                    $("." + DialogDivID).parent().hide();
                }
            }).dialog("widget")
                .draggable({
                    containment: [-screen.width + 600, 0, screen.width - 200, screen.height - 200],
                    start: function () {
                        $(this).data("startingScrollTop", $(this).parent().scrollTop());
                        $(this).parent().scrollTop();
                    },
                    drag: function (event, ui) {
                        var st = parseInt($(this).data("startingScrollTop"));
                        ui.position.top -= $(this).parent().scrollTop() - st;
                    }
                }).css({ position: "fixed" })
                .dblclick(function () {
                    if ($(this).hasClass("actualScreenToggle")) {
                        $(this).removeClass("actualScreenToggle");
                        $(this).addClass("fullScreenToggle");
                        var maxIcon = $(this).find('i.Maximize');
                        fncdialogchange(maxIcon, "maximize");
                    }
                    else {
                        $(this).removeClass("fullScreenToggle");
                        $(this).addClass("actualScreenToggle");
                        var restoreIcon = $(this).find('i.RestoreDown');
                        fncdialogchange(restoreIcon, "restore");
                    }
                })
        },
        error: function (data) {
        }
    })
}
function fncConfirmation(_this, ID, BOAction, iID, iBOIID, iBODID, sGUID, ConfirmAttributes) {
    var formid = $(_this).find('.CreateForm').attr('id');
    if (formid == undefined) {
        formid = $(_this).closest('form').attr('id');
    }
    var YesBtnText = "Yes";
    var NoBtnText = "No";
    $("#BoActionDlg").html("Are you sure you want to reset..");
    $("#BoActionDlg").dialog({
        title: "Reset Dialog",
        autoOpen: true,
        modal: true,
        width: 400,
        left: '600px',
        top: '80px',
        buttons: [
            {
                text: YesBtnText,
                click: function () {
                    fncGetCurrentStep(formid, ConfirmAttributes, sGUID, iBOIID, false);
                    //XIRun(null, YesXIlink, 0, sGUID, null, false, 0, 0, null, data.InputParams)
                    $('#BoActionDlg').dialog('close');
                    $('#BoActionDlg').dialog('destroy');
                }
            },
            {
                text: NoBtnText,
                click: function () {
                    fncGetCurrentStep(formid, ConfirmAttributes, sGUID, iBOIID, true);
                    $('#BoActionDlg').dialog('close');
                    $('#BoActionDlg').dialog('destroy');
                    //fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                }
            }
        ]
    });
}

function fncTriggerBOAction(ID, BOAction, iID, iBOIID, iBODID, sGUID) {
debugger
    var Params = {
        iActionID: ID,
        sBOAction: BOAction,
        sID: iID,
        iBOIID: iBOIID,
        sBODID: iBODID,
        sGUID: sGUID
    }
    if (BOAction == "QuestionSet") {
        var InputParams = [];
        var param1 = {};
        param1["sName"] = '-iBOIID';
        param1["sValue"] = iBOIID;
        InputParams.push(param1);
        //function XIRun($this, XiLinkID, ID, sGUID, BO, IsMerge, BODID, iQSDID, MenuName, oParams, bISActivity) {
        XIRun(null, iID, 0, sGUID, null, false, 0, 0, null, InputParams)
    } else if (BOAction == "XILink") {
        var InputParams = [];
        var param1 = {};
        param1["sName"] = '{XIP|iInstanceID}';
        param1["sValue"] = iBOIID;
        InputParams.push(param1);
if(ID==1028){
XIRun(null, iID, 0, null, null, false, 0, 0, null, InputParams)
}
else{
XILinkLoadJson(iID, null, InputParams)
}
    }
else if (BOAction == "XILinkRun") {
        var InputParams = [];
        var param1 = {};
        param1["sName"] = '{XIP|iInstanceID}';
        param1["sValue"] = iBOIID;
        InputParams.push(param1);
        XIRun(null, iID, 0, null, null, false, 0, 0, null, InputParams)
    }
    else {
        $.ajax({
            url: TriggerBOActionURL,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            cache: false,
            data: JSON.stringify(Params),
            success: function (data) {
                if (data) {
                    if (data.sType && data.sType == "Dialog") {
                        var YesBtnText = "Yes";
                        var NoBtnText = "No";
                        if (data.sYesBtnText) {
                            YesBtnText = data.sYesBtnText;
                        }
                        if (data.sNoBtnText) {
                            NoBtnText = data.sNoBtnText;
                        }
                        if (data.sDialogText) {
                            $("#BoActionDlg").html(data.sDialogText);
                        }
                        var YesXIlink = data.ButtonXIlink;
                        $("#BoActionDlg").dialog({
                            title: "Confirmation",
                            autoOpen: true,
                            modal: true,
                            width: 400,
                            left: '600px',
                            top: '80px',
                            buttons: [
                                {
                                    text: YesBtnText,
                                    click: function () {
                                        XIRun(null, YesXIlink, 0, sGUID, null, false, 0, 0, null, data.InputParams)
                                        $('#BoActionDlg').dialog('close');
                                        $('#BoActionDlg').dialog('destroy');
                                    }
                                },
                                {
                                    text: NoBtnText,
                                    click: function () {

                                        $('#BoActionDlg').dialog('close');
                                        $('#BoActionDlg').dialog('destroy');
                                        //fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID);
                                    }
                                }
                            ]
                        });
                    }
                    else
                        $('.ResponseMsg').html('Action executed successfully');
                }
            },
            error: function (textStatus, errorThrown) {
            }
        });
    }
}

//var timerid;
//$(window).resize(function () {
//    (timerid && clearTimeout(timerid));
//    timerid = setTimeout(function () {
//        var Dlgs = $('.ui-dialog');
//        $('.ui-dialog').each(function (data, obj) {
//            var _dlgWidth = $(this).outerWidth();
//            var _dlgHeight = $(this).outerHeight();
//            var wWidth = $(window).width();
//            var wHeight = $(window).height();
//            if (wWidth - 100 < _dlgWidth) {
//                $(this).animate({
//                    width: wWidth - 100,
//                });
//            }
//            else {
//                var dlgW = $(this).attr('data-dlgWidth');
//                if (dlgW) {
//                    var dlgWidth = parseInt(dlgW.slice(0, -2));
//                    if (dlgWidth >= (wWidth - 100)) {
//                        $(this).animate({
//                            width: wWidth - 100
//                        });
//                    }
//                }
//                else {
//                    $(this).animate({
//                        width: dlgWidth,
//                    });
//                }

//            }
//            if (wHeight - 100 < _dlgHeight) {
//                $(this).animate({
//                    height: wHeight - 100
//                });
//            }
//            else {
//                var dlgH = $(this).attr('data-dlgHeight');
//                if (dlgH) {
//                    var dlgHeight = dlgH.slice(0, -2);
//                    if (dlgHeight > (wHeight - 100)) {
//                        $(this).animate({
//                            height: wHeight - 100
//                        });
//                    }
//                }
//                else {
//                    $(this).animate({
//                        height: dlgHeight,
//                    });
//                }
//            }
//        });


//        var wWidth = $(window).width();
//        var dWidth = wWidth * 0.9;
//        var wHeight = $(window).height();
//        var dHeight = wHeight * 0.9;
//        //$("#data-dialog-id").dialog("option", "width", dWidth);
//        //$("#data-dialog-id").dialog("option", "height", dHeight);
//    }, 200);
//});


function funAddNode(sParentID, iID, sDocName, sType, sFolderTreeGUID, batchid, cType, iOldDocID) {
    var foldertree = $("#TreeStrctr-" + sFolderTreeGUID).jstree(true);
    if (foldertree && cType == 'new') {
        var Node = foldertree.get_node(sParentID);
        var nodedata = [];
        nodedata.push({ sType: sType, vbatchid: batchid });
        foldertree.create_node(Node, { text: sDocName, type: 'default', id: iID, data: nodedata });
        $("#TreeStrctr-" + sFolderTreeGUID).find('.jstree-leaf').each(function () {
            var id = $(this).attr('id');
            var PNodeType = foldertree.get_node(id).data[0].sType;
            if (PNodeType == "20") {
                $(this).find('i.jstree-themeicon').removeClass('fa-folder').addClass('fa-file-pdf-o');
            }
        });
        $("#TreeStrctr-" + sFolderTreeGUID).jstree("open_all");
        $('#' + iID).find('i.jstree-themeicon').removeClass('fa-folder').addClass('fa-file-pdf-o');
    }
    else if (foldertree) {
        foldertree.get_node(iOldDocID).id = iID;
    }
}

function fncReportdata(JsonData, bool) {
    var table_one = [];
    var TableData;
    $('#tData-' + JsonData.ID + ' tr').each(function () {
        var temp_string1 = "";
        $(this).find("td").each(function () {
            var temp_string2 = $(this).text();
            if (temp_string2 == "") {
                temp_string1 += 0 + "/";
            }
            else {
                temp_string1 += temp_string2 + "/";
            }
        });
        temp_string1 = temp_string1.slice(0, -1);
        table_one.push(temp_string1);
    });
    var list = JsonData.ReportList;
    if (list != null) {
        //if(list[0].Attributes != null){
        var Headers = Object.keys(list[0].Attributes);
        var visu = JsonData.oXIVisualisations;
        var html;
        $('#tData-' + JsonData.ID).html('');
        if (visu != null) {
            $.each(list, function (i) {
                // $.each(visu, function (key1, val1) {
                html += '<tr id="' + i + '">';
                $.each(Headers, function (key, val) {
                    //var color='';
                    var value = list[i].Attributes[val].sValue;
                    for (var k = 0; k < visu.length; k++) {
                        if (value == "" || value == null) {
                            value = 0;
                        }
                        if (bool == true) {
                            var tableSplit = table_one[i].split("/");
                            var erc = (value == parseInt(value));
                            if (erc == true) {
                                var xa = visu[k].sName;
                                var result = eval(visu[k].sName);//'value<3'
                                if (result) {

                                    var color = visu[k].sValue;
                                    if (value != 0) {
                                        var interger = parseInt(tableSplit[key]);
                                        //if (interger < parseInt(value)) {

                                        if (val == "0-1") {
                                            html += '<td class="blink" style="text-align:center;background-color:' + color + '"><strong>' + value + '</strong></td>';
                                        }
                                        //}
                                        else
                                            html += '<td style="text-align:center;background-color:' + color + '">' + value + '</td>';
                                    }
                                    else {
                                        html += '<td></td>';
                                    }
                                    break;
                                }
                            }
                            else {
                                html += '<td>' + value + '</td>';
                                break;
                            }
                        }
                        else {
                            var erc = (value == parseInt(value));
                            if (erc == true) {
                                var xa = visu[k].sName;
                                var result = eval(visu[k].sName);//'value<3'
                                if (result) {

                                    var color = visu[k].sValue;
                                    if (value != 0) {
                                        html += '<td style="text-align:center;background-color:' + color + '">' + value + '</td>';
                                    }
                                    else {
                                        html += '<td></td>';
                                    }
                                    break;
                                }
                            }
                            else {
                                html += '<td>' + value + '</td>';
                                break;
                            }
                        }
                    }
                });
                html += '</tr>';
            });
        }
        else {
            $.each(list, function (i) {
                html += '<tr>';
                $.each(Headers, function (key, val) {
                    var value = list[i].Attributes[val].sValue;

                    html += '<td>' + value + '</td>'
                });
                html += '</tr>';

            });
        }
        $('#tData-' + JsonData.ID).append(html);
    }
}
function HeatMap(id) {
    var ids = id.toString();
    var Params = {
        OneclicksID: ids
    }
    $.ajax({
        url: HeatMapReport,
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        cache: false,
        data: JSON.stringify(Params),
        success: function (data) {
            fncReportdata(data, false);
        },
        error: function (textStatus, errorThrown) {
            //alert("error" + textStatus.status);
        }
    });
}


////////////////////////////////////////////////////////////
///////////////////////TEME CHANGER/////////////////////////
////////////////////////////////////////////////////////////
function Themechange(thmColour, LColour, DColour, LGColour, FSize, FSizeLg, FSizeXl) {
    //const themeSwitchers = document.querySelectorAll('span.selectTheme');
    const dynamicInputs = document.querySelectorAll('input.input-color-picker');
    const handleThemeUpdate = (cssVars) => {
        const root = document.querySelector(':root');
        const keys = Object.keys(cssVars);
        keys.forEach(key => {
            root.style.setProperty(key, cssVars[key]);
        });
    }
    const thmColor = thmColour;
    const bgColor = thmColour;
    const color = thmColour;
    const lightColor = LColour;
    const darkColor = DColour;
    const lightGrayColor = LGColour;
    const fontSize = FSize;
    const fontSizeLg = FSizeLg;
    const fontSizeXl = FSizeXl;
    handleThemeUpdate({
        '--primary-thm-color': thmColor,
        '--primary-bg-color': bgColor,
        '--primary-color': color,
        '--primary-light-color': lightColor,
        '--primary-dark-color': darkColor,
        '--primary-light-gray-color': lightGrayColor,
        '--primary-font-size': fontSize,
        '--primary-font-size-lg': fontSizeLg,
        '--primary-font-size-xl': fontSizeXl,
    });

    console.log(thmColor, bgColor, color, lightColor, darkColor, lightGrayColor, fontSize, fontSizeLg, fontSizeXl)
    $("input.input-color-picker[data-id='thm-color']").val(thmColor)
    $("input.input-color-picker[data-id='bg-color']").val(bgColor)
    $("input.input-color-picker[data-id='color']").val(color)
    $("input.input-color-picker[data-id='light-color']").val(lightColor)
    $("input.input-color-picker[data-id='dark-color']").val(darkColor)
    $("input.input-color-picker[data-id='light-gray-color']").val(lightGrayColor)
    $("input.font-size-change[data-id='font-size']").val(fontSize)
    $("input.font-size-change[data-id='font-size-lg']").val(fontSizeLg)
    $("input.font-size-change[data-id='font-size-xl']").val(fontSizeXl)
    dynamicInputs.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
            console.log(cssPropName)
            handleThemeUpdate({
                [cssPropName]: e.target.value
            });
        });
    });
}

//CREATEIF JS
$('#CIFAdminHome').parent().parents('section.fluid-container').css({ 'height': 'auto' })


function fnc1ClickAction(_this, FKiBOID, iPopupID, iXiLinkID, sGUID) {
    debugger
    bButton = true;
    var tr = $(_this).closest('tr');
    var tddata = [];
    $(tr).children('td').each(function (ii, vv) {
        tddata[ii] = $(this).text();
    });
    var RowData = tddata;
    var id = RowData[0];
    var InputParams = [];
    var param4 = {};
    param4["sName"] = '{-iInstanceID}';
    param4["sValue"] = id;
    InputParams.push(param4);
    XILinkLoadJson(iPopupID, sGUID, InputParams);
}

function fncGetBOAttributeValue(BOID, iInstanceID) {
    // 
    return new Promise(function (resolve, reject) {
        var Params = {
            iBODID: BOID,
            iBOIID: iInstanceID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: GetBOAttributeValueURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'json',
            success: function (data) {
                resolve(data);
            }
        })
    })
}


function fncUpdateXITree(BOID, iInstanceID, iNodeID, sTreeGUID) {
    // 
    fncGetBOAttributeValue(BOID, iInstanceID).then(function (data) {
        var foldertree = $("#TreeStrctr-" + sTreeGUID).jstree(true);
        var $node = foldertree.get_node(iNodeID);
        var OldName = $node.text;
        var NewName = "";
        if (OldName.indexOf("(") > 0) {
            var index = OldName.indexOf("(");
            NewName = OldName.substr(0, index) + "(" + data + ")";
        }
        else {
            NewName = $node.text + "(" + data + ")";
        }
        $("#TreeStrctr-" + sTreeGUID).jstree('rename_node', $node, NewName);
        $node.data[0].sInsID = iInstanceID;
        $node.data[0].sNameAttribute = data;
    })
}

function fncUpdateXIValueTree(iInstanceID, iNodeID, sTreeGUID) {
    var foldertree = $("#TreeStrctr-" + sTreeGUID).jstree(true);
    var $node = foldertree.get_node(iNodeID);
    var OldName = $node.text;
    var NewName = "";
    if (OldName.indexOf("(") > 0) {
        var index = OldName.indexOf("(");
        NewName = OldName.substr(0, index) + "(" + iInstanceID + ")";
    }
    else {
        NewName = $node.text + "(" + iInstanceID + ")";
    }
    $("#TreeStrctr-" + sTreeGUID).jstree('rename_node', $node, NewName);
    $node.data[0].sInsID = iInstanceID;
    $node.data[0].sNameAttribute = iInstanceID;
}

function fncHTMLMerge(sBO, iInstanceID, i1ClickID, sHTML) {
    if (i1ClickID == "") {
        i1ClickID = 0;
    }
    if (iInstanceID == "") {
        iInstanceID = 0;
    }
    var Params = {
        sBO: sBO,
        iInstanceID: parseInt(iInstanceID),
        i1ClickID: parseInt(i1ClickID),
        sHTML: sHTML
    }
    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: HTMLMergeURL,
        data: JSON.stringify(Params),
        cache: false,
        async: true,
        dataType: 'HTML',
        success: function (data) {
            var edit = window.open('', '_blank', "scrollbars=1,resizable=1,width=" + screen.width + ", height=" + screen.height);
            with (edit.document) {
                open();
                write(data);
                close();
            }
            //$('.htmloutput').html(data);
        }
    })
}

function fncGetCacheData(sGUID) {
    return new Promise(function (resolve, reject) {
        var Params = {
            sGUID: sGUID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: GetCacheDataURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                resolve(data);
            }
        })
    })
}


//CreateIF Start

function fncViewDocument(_this) {
    $('.ciftab').removeClass('active');
    $('a[data-name="View"]').addClass('active');
    var id = $('.cifsrchdlg');
    if (id && id != null && id.length > 0) {

    }
    else {
        id = $('#DocTabs');
    }
    var sRole = $('#cifrole').attr('data-value');
    if (sRole == "CreateIFAdmin") {
        id = $('#DocTabsAdmin');
    }
    var docid = 0;
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _layout = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    if (_uidialog && _uidialog != null) {
        $(_uidialog).find('#PDFPreview').html('<div class="loader"></div>');
        var Info = $(_uidialog).attr('data-info');
        var Splits = Info.split(':');
        for (var u = 0; u < Splits.length; u++) {
            var Splits2 = Splits[u].split('__');
            if (Splits2[0] == "ID") {
                docid = Splits2[1];
            }
        }
    }
    else {
        $(_layout).find('#PDFPreview').html('<div class="loader"></div>');
        docid = $('.slctpro').attr('data-currentdocid');
    }
    fncLoadPDFPreview(id[0], docid);
}

function fncDownloadDocument(_this) {
    $('.ciftab').removeClass('active');
    $('a[data-name="View"]').addClass('active');
    var id = $('.cifsrchdlg');
    if (id && id != null && id.length > 0) {

    }
    else {
        id = $('#DocTabs');
    }
    var sRole = $('#cifrole').attr('data-value');
    if (sRole == "CreateIFAdmin") {
        id = $('#DocTabsAdmin');
    }
    var docid = 0;
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _layout = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    if (_uidialog && _uidialog != null) {
        $(_uidialog).find('#PDFPreview').html('<div class="loader"></div>');
        var Info = $(_uidialog).attr('data-info');
        var Splits = Info.split(':');
        for (var u = 0; u < Splits.length; u++) {
            var Splits2 = Splits[u].split('__');
            if (Splits2[0] == "ID") {
                docid = Splits2[1];
            }
        }
    }
    else {
        $(_layout).find('#PDFPreview').html('<div class="loader"></div>');
        docid = $('.slctpro').attr('data-currentdocid');
    }
    var Params = {
        iID: docid
    }
    $.ajax({
        type: 'GET',
        contentType: "application/json; charset=utf-8",
        url: DownloadDocURL,
        data: JSON.stringify(Params),
        cache: false,
        async: true,
        dataType: 'JSON',
        success: function (data) {

        }
    })
}

function fncUpLoadDocument(_this, sType) {
    $('.ciftab').removeClass('active');
    $('a[data-name="Upload"]').addClass('active');
    var sRole = $('#cifrole').attr('data-value');
    if (sRole == "Approver" || sRole == "CreateIFAdmin") {
        var id = $('.cifsrchdlg');
        if (id && id != null && id.length > 0) {
            id = id[0];
        }
        else {
            id = $('#DocTabs');
            id = id[0];
        }
        if (sRole == "CreateIFAdmin") {
            id = _this;
        }
        if (sType && sType == "NewDocument" && sRole == "Approver") {
            $('.slctpro').attr('data-currentdocid', 0);
            id = $('#CIFFolderTree');
            id = id[0];
        }
        else if (sType && sType == "NewDocument" && sRole == "CreateIFAdmin") {
            id = $('#TreeScrollbar');
            id = id[0];
        }
        var PID = ""; var BID = ""; var FolderPath = ""; var docid = "";
        var _layout = "";
        var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
        if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
            sLayoutType = "inline-layout";
            _layout = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
        }
        if (_uidialog && _uidialog != null) {
            $(_uidialog).find('#PDFPreview').html('<div class="loader"></div>');
            var Info = $(_uidialog).attr('data-info');
            var Splits = Info.split(':');
            for (var u = 0; u < Splits.length; u++) {
                var Splits2 = Splits[u].split('__');
                if (Splits2[0] == "ID") {
                    docid = Splits2[1];
                }
                else if (Splits2[0] == "sParentID") {
                    PID = Splits2[1];
                }
                else if (Splits2[0] == "iBuildingID") {
                    BID = Splits2[1];
                }
                else if (Splits2[0] == "iVersionBatchID") {
                    VBatchID = Splits2[1];
                }
                else if (Splits2[0] == "sFolderName") {
                    FolderPath = Splits2[1];
                }
            }
        }
        else {
            $(_layout).find('#PDFPreview').html('<div class="loader"></div>');
            if (sType && sType == 'Admin') {
                PID = $(_this).attr('data-parentid');
                BID = $(_this).attr('data-buildingid');
                FolderPath = $(_this).attr('data-foldername');
            }
            else {
                PID = $('.slctpro').attr('data-parentid'); //$(_this).attr('data-parentid');
                BID = $('.slctpro').attr('data-bid'); //$(_this).attr('data-buildingid');
                FolderPath = $('.slctpro').attr('data-foldername');
                docid = $('.slctpro').attr('data-currentdocid');
            }
        }


        var InputParams = [];
        var param1 = {};
        param1["sName"] = 'sParentID';
        param1["sValue"] = PID;
        InputParams.push(param1);
        var param2 = {};
        param2["sName"] = 'iBuildingID';
        param2["sValue"] = BID;
        InputParams.push(param2);
        var param3 = {};
        var Upload = 'Upload';
        if (sType && sType == "NewDocument") {
            Upload = "NewDocument"
        }
        else if (sType && sType == "Admin") {
            Upload = "Admin"
        }
        param3["sName"] = Upload;
        param3["sValue"] = 'Yes';
        InputParams.push(param3);
        var param4 = {};
        param4["sName"] = '{XIP|iBuildingiD}';
        param4["sValue"] = BID;
        InputParams.push(param4);
        var param5 = {};
        param5["sName"] = '{XIP|sParentID}';
        param5["sValue"] = PID;
        InputParams.push(param5);
        var param6 = {};
        param6["sName"] = 'sFolderName';
        param6["sValue"] = FolderPath;
        InputParams.push(param6);
        var sTreeGuid = $('#TreeScrollbar').attr('data-guid');
        var param8 = {};
        param8["sName"] = 'sFolderTreeGUID';
        param8["sValue"] = sTreeGuid;
        InputParams.push(param8);
        var param7 = {};
        param7["sName"] = 'iDocID';
        param7["sValue"] = docid;
        InputParams.push(param7);
        if (PID && BID)
            fncLoadPDFPreview(id, 0, InputParams);
    }
    else {
        $('#PDFPreview').html('<h4>No Access</h4>');
    }
}

function fncLoadDocHistory(_this, sType) {
    $('.ciftab').removeClass('active');
    if (sType && sType == "comments") {
        $('a[data-name="Comments"]').addClass('active');
    }
    else {
        $('a[data-name="History"]').addClass('active');
    }


    var PID = ""; var BID = ""; var VBatchID = ""; var docid = 0;
    var _layout = "";
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _layout = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    if (_uidialog && _uidialog != null) {
        $(_uidialog).find('#PDFPreview').html('<div class="loader"></div>');
        var Info = $(_uidialog).attr('data-info');
        var Splits = Info.split(':');
        for (var u = 0; u < Splits.length; u++) {
            var Splits2 = Splits[u].split('__');
            if (Splits2[0] == "ID") {
                docid = Splits2[1];
            }
            else if (Splits2[0] == "sParentID") {
                PID = Splits2[1];
            }
            else if (Splits2[0] == "iBuildingID") {
                BID = Splits2[1];
            }
            else if (Splits2[0] == "iVersionBatchID") {
                VBatchID = Splits2[1];
            }
        }
    }
    else {
        $(_layout).find('#PDFPreview').html('<div class="loader"></div>');
        PID = $('.slctpro').attr('data-parentid'); //$(_this).attr('data-parentid');
        BID = $('.slctpro').attr('data-bid'); //$(_this).attr('data-buildingid');
        VBatchID = $('.slctpro').attr('data-vbatchid');
        docid = $('.slctpro').attr('data-currentdocid');
    }
    var id = $('.cifsrchdlg');
    if (id && id != null && id.length > 0) {

    }
    else {
        id = $('#DocTabs');
    }
    var InputParams = [];
    var param1 = {};
    param1["sName"] = 'sParentID';
    param1["sValue"] = PID;
    InputParams.push(param1);
    var param2 = {};
    param2["sName"] = 'iBuildingID';
    param2["sValue"] = BID;
    InputParams.push(param2);
    var Type = 'History';
    if (sType && sType.length > 0) {
        Type = sType;
        //docid = $('.slctpro').attr('data-currentdocid');
    }
    var param3 = {};
    param3["sName"] = Type;
    param3["sValue"] = 'Yes';
    InputParams.push(param3);
    var param4 = {};
    param4["sName"] = '{XIP|iBuildingID}';
    param4["sValue"] = BID;
    InputParams.push(param4);
    var param5 = {};
    param5["sName"] = '{XIP|sParentID}';
    param5["sValue"] = PID;
    InputParams.push(param5);
    var param6 = {};
    param6["sName"] = '{XIP|iVersionBatchID}';
    param6["sValue"] = VBatchID;
    InputParams.push(param6);
    if (sType && sType.length > 0) {
        InputParams = [];
        var param1 = {};
        param1["sName"] = '';
        param1["sValue"] = 0;
        InputParams.push(param1);
        var param2 = {};
        param2["sName"] = '';
        param2["sValue"] = 0;
        InputParams.push(param2);
        var Type = 'History';
        if (sType && sType.length > 0) {
            Type = sType;
            //docid = $('.slctpro').attr('data-currentdocid');
        }
        var param3 = {};
        param3["sName"] = Type;
        param3["sValue"] = 'Yes';
        InputParams.push(param3);
    }
    if (PID && BID && VBatchID)
        fncLoadPDFPreview(_this, docid, InputParams);
}

function fncLoadPDFPreview(_this, id, Params) {
    var iVersionBatchID = $(_this).attr('data-versionbatchid');
    var iDocVersionID = iVersionBatchID;
    var iStatus = $(_this).attr('data-status');
    $('.slctpro').attr('data-appstatus', iStatus);
    $('.slctpro').attr('data-vbatchid', iVersionBatchID);
    if (id && parseInt(id) > 0) {
        $('.slctpro').attr('data-currentdocid', id);
    }

    if (Params && Params.length > 0) {

    }
    else {
        $('.treenode').removeClass('active');
        $(_this).addClass('active');
    }
    var iStepID = "Step with Level3 Layout"; //"Step with FormComponent";
    var BtnType = "";
    if (Params && Params.length > 2) {
        BtnType = Params[2].sName;
        var BtnValue = Params[2].sValue;
        if (BtnType && BtnValue && (BtnType == 'History' && BtnValue == 'Yes')) {
            iStepID = "Step with OneClick Component";
        }
    }
    var iBuildingID = 0;
    var sParentID = 0;
    var iVersionBatchID = 0;
    var sLayoutType = "Dialog";
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _uidialog = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', _this);
    var ParentID = $(_this).attr('data-nodeid');
    var PID = $(_this).attr('data-pid');
    $('.slctpro').attr('data-parentid', PID);
    var sFolderName = $(_this).attr('data-foldername');
    $('.slctpro').attr('data-foldername', sFolderName);
    //var jCompDef = '@Html.Raw(Json.Encode(Model.oDefintion))';
    //var JParams = JSON.parse(jCompDef).Params;
    ////var JParams = JSON.parse(jCompDef).Params;c
    //for (var i = 0; i < JParams.length; i++) {
    //    var InputParams = [];
    //    if (JParams[i].sName.toLowerCase() == "nodeClickparamname".toLowerCase()) {

    //    }
    //}
    var InputParams = [];
    var WrapperParms = {};
    WrapperParms["sName"] = "pdfview";
    WrapperParms["sValue"] = "XIC|null";
    var param7 = {};
    param7["sName"] = 'iInstanceID';
    param7["sValue"] = id;
    InputParams.push(param7);
    if (Params && Params.length > 0 && BtnType != 'comments') {
        for (var k = 0; k < Params.length; k++) {
            if (Params[k].sName.length > 0) {
                var param9 = {};
                param9["sName"] = Params[k].sName;
                param9["sValue"] = Params[k].sValue;
                param9["sType"] = "merge";
                InputParams.push(param9);
                if (Params[k].sName == '{XIP|iBuildingID}') {
                    iBuildingID = Params[k].sValue;
                }
                if (Params[k].sName == '{XIP|sParentID}') {
                    sParentID = Params[k].sValue;
                }
                if (Params[k].sName == '{XIP|iVersionBatchID}') {
                    iVersionBatchID = Params[k].sValue;
                }
            }
        }
    }
    WrapperParms["nSubParams"] = InputParams;
    var NewParams = [];
    NewParams.push(WrapperParms);
    var param4 = {};
    param4["sName"] = '{XIP|iBuildingID}';
    param4["sValue"] = iBuildingID;
    var param5 = {};
    param5["sName"] = '{XIP|sParentID}';
    param5["sValue"] = sParentID;
    var param6 = {};
    param6["sName"] = '{XIP|iVersionBatchID}';
    param6["sValue"] = iVersionBatchID;
    var param8 = {};
    param8["sName"] = '{XIP|iDocID}';
    param8["sValue"] = iDocVersionID;
    var param9 = {};
    param9["sName"] = '{XIP|iDocVersionID}';
    param9["sValue"] = id;
    NewParams.push(param9);
    NewParams.push(param8);
    NewParams.push(param4);
    NewParams.push(param5);
    NewParams.push(param6);
    fncUpdateXIParams("", sGUID, sGUID, NewParams).then(function (data) {
        var QSInfo = fncQSInfoFromHTMLTree(_this);
        var iQSDID = QSInfo[0].sValue;// 3067;
        if (BtnType == 'NewDocument' || BtnType == 'Upload') {
            iStepID = "Step with FormComponent";
        }
        else if (BtnType == 'comments') {
            iStepID = "Step with Comments";
        }
        var sRole = $('#cifrole').attr('data-value');
        if (sRole && sRole.length > 0 && sRole == 'Approver' || sRole == 'CreateIFUser') {
            iQSDID = 1581
        }
        else {
            iQSDID = 3067
        }
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            url: LoadStepContentURL,
            datatype: 'json',
            cache: false,
            async: true,
            //data: { sStep: DefaultStep, iQSID: iQSDID, i1ClickID: i1ClickID, sDefaultStep: DefaultStep, iInstanceID: iInstanceID, sGUID: sGUID },
            data: { sStep: iStepID, iQSID: iQSDID, i1ClickID: "0", sDefaultStep: iStepID, iInstanceID: "0", sGUID: sGUID },
            success: function (oStepI) {
                var Output = "CIFFileUpload";
                if (BtnType == 'History' || BtnType == 'Upload' || BtnType == 'View' || BtnType == 'comments') {
                    Output = "PDFPreview";
                }
                var iBODID = 0;
                $(_this).attr('data-load', 'true');
                fncRenderStepContent(oStepI, '#' + Output, "", null, _uidialog).then(function (ConfigParams) {
                    $(_uidialog).find('#' + Output).find('.loader').remove();
                    fncLoadScroll(_uidialog, sLayoutType);
                });
                //if (Output != 'undefined') {
                //    $('#' + Output).html(StepData);
                //}

            }
        })
    })
}

function fncCIFGridClick(_this) {
    fncLoadMenuContent1(_this)
    //XILinkLoadJson(9838)
}

function fncLoadMenuContent1(_this) {
    $('#RightContent').html('<div class="loader"></div>');
    $('.menuitem').removeClass('active');
    $(_this).addClass('active');
    var sLayoutType = "Dialog";
    var MenuID = "1276"; //$(_this).attr('data-nodeid');
    var Folders = $('.slctpro').attr('data-proname');
    var FolderName = "Health & Safety File"; //$(_this).attr('data-name');
    $('.slctpro').attr('data-foldername', Folders + "//" + FolderName);
    var BuildingID = $('.BuildBtns').find('button.active');
    var bid = $('.slctpro').attr('data-bid');
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _uidialog = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', _this);
    var ParentID = $(_this).attr('data-nodeid');
    //var JParams = JSON.parse(jMenuCompDef).Params;
    //for (var i = 0; i < JParams.length; i++) {
    //debugger
    var InputParams = [];
    // if (JParams[i].sName.toLowerCase() == "MenuClickParam".toLowerCase()) {
    var WrapperParms = {};
    WrapperParms["sName"] = "treeinstance";
    WrapperParms["sValue"] = "XIC|null";
    var param2 = {};
    param2["sName"] = 'ParentID';
    param2["sValue"] = MenuID;
    InputParams.push(param2);
    var param3 = {};
    param3["sName"] = 'BuildingID';
    param3["sValue"] = bid;
    InputParams.push(param3);
    var param4 = {};
    param4["sName"] = 'FolderName';
    param4["sValue"] = FolderName;
    InputParams.push(param4);
    if (FolderName == 'Red Documents' || FolderName == 'Amber Documents') {
        var param5 = {};
        param5["sName"] = 'sFilterType';
        param5["sValue"] = FolderName;
        InputParams.push(param5);
    }
    WrapperParms["nSubParams"] = InputParams;
    //}
    //}
    //debugger
    var NewParams = [];
    NewParams.push(WrapperParms);
    fncUpdateXIParams("", sGUID, sGUID, NewParams).then(function (data) {
        var iStepID = "Step with Level2 Layout";
        var iQSDID = 1581;
        $.ajax({
            contentType: 'application/json; charset=utf-8',
            url: LoadStepContentURL,
            datatype: 'json',
            cache: false,
            async: true,
            //data: { sStep: DefaultStep, iQSID: iQSDID, i1ClickID: i1ClickID, sDefaultStep: DefaultStep, iInstanceID: iInstanceID, sGUID: sGUID },
            data: { sStep: iStepID, iQSID: iQSDID, i1ClickID: "0", sDefaultStep: iStepID, iInstanceID: "0", sGUID: sGUID },
            success: function (oStepI) {
                var Output = "HomeGrid";
                var iBODID = 0;
                $(_this).attr('data-load', 'true');
                fncRenderStepContent(oStepI, '#' + Output, "", null, _uidialog).then(function (ConfigParams) {
                    $(_uidialog).find('#' + Output).find('.loader').remove();
                    fncLoadScroll(_uidialog, sLayoutType);
                });
                //if (Output != 'undefined') {
                //    $('#' + Output).html(StepData);
                //}

            },
            error: function (data) {

            }
        })
    })
}

function fncPrintDocument(_this) {
    $('.ciftab').removeClass('active');
    $('a[data-name="View"]').addClass('active');
    var id = $('.cifsrchdlg');
    if (id && id != null && id.length > 0) {

    }
    else {
        id = $('#DocTabs');
    }
    var sRole = $('#cifrole').attr('data-value');
    if (sRole == "CreateIFAdmin") {
        id = $('#DocTabsAdmin');
    }
    var docid = 0;
    var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
    if (_uidialog == null || (_uidialog && _uidialog.length == 0)) {
        sLayoutType = "inline-layout";
        _layout = fncgetInlineLayoutFromHTMLTree('inline-layout', _this);
    }
    if (_uidialog && _uidialog != null) {
        //$(_uidialog).find('#PDFPreview').html('<div class="loader"></div>');
        var Info = $(_uidialog).attr('data-info');
        var Splits = Info.split(':');
        for (var u = 0; u < Splits.length; u++) {
            var Splits2 = Splits[u].split('__');
            if (Splits2[0] == "ID") {
                docid = Splits2[1];
            }
        }
    }
    else {
        //$(_layout).find('#PDFPreview').html('<div class="loader"></div>');
        docid = $('.slctpro').attr('data-currentdocid');
    }
    if (docid > 0) {
        var Params = {
            iDocID: docid
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: PrintPDFURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                var sDOC = '<embed src="' + data + '" style="width:100%;" type="application/pdf">';
                var div = '<div class="fluid-container vh-fluid"><div class="fluid-cell"><div class="fluid-container vh-100"><div class="fluid-cell twelve-twelve" id="srchResult">' + sDOC + '</div></div></div></div>';
                $('#CIFDlg').html('<div class="cifsrchdlg">' + div + '</div>');
                var DialogDivID = 'cifsrchdlg';
                var windowclose = '<i class="windowClose fa fa-close" onclick="fncdialogclose(this, ' + false + ', &quot;' + DialogDivID + '&quot;)"></i>';
                //$('.' + DialogDivID).html('');
                $("." + DialogDivID).dialog({
                    title: 'Search Result',
                    //modal: true,
                    height: screen.height - 150,
                    width: screen.width,
                    position: { my: 'center center', at: 'left+200 top+400' },
                    open: function () {
                        $(this).parent().promise().done(function () {
                            $(this).children('.ui-dialog-titlebar').append('<div class="dialogIcons">' + windowclose + '</div>');
                        })
                    }
                });
            }
        })
    }
}

//CreateIF End

/*function Themechange(BColour,FColour)
{
    const themeSwitchers = document.querySelectorAll('span.selectTheme');
    const dynamicInputs = document.querySelectorAll('input.input-color-picker');
    const handleThemeUpdate = (cssVars) => {
        const root = document.querySelector(':root');
        const keys = Object.keys(cssVars);
        keys.forEach(key => {
            root.style.setProperty(key, cssVars[key]);
        });
    }
    const bgColor = BColour
    const color = FColour
    handleThemeUpdate({
        '--primary-bg-color': bgColor,
        '--primary-color': color
    });

    console.log(bgColor, color)
    $("input.input-color-picker[data-id='color']").val(color)
    $("input.input-color-picker[data-id='bg-color']").val(bgColor)
    dynamicInputs.forEach((item) => {
        item.addEventListener('input', (e) => {
            const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
            console.log(cssPropName)
            handleThemeUpdate({
                [cssPropName]: e.target.value
            });
        });
    });
    //const themeSwitchers = document.querySelectorAll('span.selectTheme');
    //const dynamicInputs = document.querySelectorAll('input.input-color-picker');  
    //const handleThemeUpdate = (cssVars) => {
    //  const root = document.querySelector(':root');
    //  const keys = Object.keys(cssVars);
    //  keys.forEach(key => {
    //    root.style.setProperty(key, cssVars[key]);
    //  });
    //}  
    //themeSwitchers.forEach((item) => {
    //  item.addEventListener('click', (e) => {
    //  
    //    const bgColor = e.target.getAttribute('data-bg-color')
    //    const color = e.target.getAttribute('data-color')
    //    handleThemeUpdate({
    //      '--primary-bg-color': bgColor,
    //      '--primary-color': color
    //    });

    //    console.log(bgColor, color)
    //    $("input.input-color-picker[data-id='color']").val(color)
    //    $("input.input-color-picker[data-id='bg-color']").val(bgColor)
    //  });
    //});  
    //dynamicInputs.forEach((item) => {
    //  item.addEventListener('input', (e) => {
    //    const cssPropName = `--primary-${e.target.getAttribute('data-id')}`;
    //    console.log(cssPropName)
    //    handleThemeUpdate({
    //      [cssPropName]: e.target.value
    //    });
    //  });
    //}); 
}*/

////////////////////////////////////////////////////////////
/////////////////////TEME CHANGER END///////////////////////
////////////////////////////////////////////////////////////
$('.ui-dialog.admin_v2_1 #Menu').prepend("<button type='button' class='MenuContentBtn'>User menu</button>");
//$('.ui-dialog.admin_v2_1 #MenuContent .right-nav-btns').hide()
$('.MenuContentBtn').on('click', function () {
    $('.ui-dialog.admin_v2_1 #MenuContent .right-nav-btns').toggle();
});
////////////////////////////////////////////////////////////
function fncDeviceAM4BarChart(jsonData, Keys) {

    if (jsonData != null) {
        //var chartdata = {};
        //var Year = [];
        var XaxisName = [];
        //var LeadCount = [];
        //var QuoteCount = [];
        var dataProvider = [];
        $.each(jsonData, function (i, item) {
            if (jsonData != null) {
                if (item.Attributes != null) {
                    // for (var items in Keys) {
                    dataProvider.push({
                        title: item.Attributes[Keys[0]].sValue,
                        value: parseInt(item.Attributes[Keys[1]].sValue),
                    });
                    //LeadCount.push(item.Attributes[Keys[items]].sValue);
                    // }
                }
            }
        });
        //var Dyanamicobj = {};
        //$.each(jsonData, function (i, item) {
        //    Object.defineProperty(Dyanamicobj, XaxisName[i], { value: LeadCount[i] });
        //    dataProvider.push(Dyanamicobj);
        //});

        // Themes begin
        am4core.useTheme(am4themes_dataviz);
        am4core.useTheme(am4themes_animated);
        // Themes end

        // Create chart instance
        var Name = "combinationChart-" + parseInt(jsonData.OneClick);
        var chart = am4core.create(Name, am4charts.XYChart);
        //chart.exporting.menu = new am4core.ExportMenu();

        // Data for both series
        chart.data = dataProvider;
        //var title = chart.titles.create();
        //title.text = "Speed";
        //title.fontSize = 25;
        //title.marginBottom = 30;
        chart.colors.list = [
            /*am4core.color("#845EC2"),
            am4core.color("#D65DB1"),
            am4core.color("#FF6F91"),
            am4core.color("#FF9671"),
            am4core.color("#FFC75F"),
            am4core.color("#F9F871"),*/
            am4core.color("#4a8ddc"),
            am4core.color("#4c5d8a"),
            am4core.color("#f3c911"),
            am4core.color("#dc5b57"),
            am4core.color("#33ae81"),
            am4core.color("#95c8f0"),
            am4core.color("#dd915f"),
            am4core.color("#DD4477"),
            am4core.color("#66AA00"),
            am4core.color("#B82E2E"),
            am4core.color("#316395"),
            am4core.color("#994499"),
            am4core.color("#22AA99"),
            am4core.color("#AAAA11"),
            am4core.color("#6633CC"),
            am4core.color("#E67300"),
            am4core.color("#8B0707"),
            am4core.color("#329262"),
            am4core.color("#5574A6"),
            am4core.color("#3B3EAC"),
        ];
        /* Create axes */
        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.fill = am4core.color("#fdd400");
        categoryAxis.dataFields.category = 'title';
        categoryAxis.renderer.minGridDistance = 5;
        categoryAxis.renderer.labels.template.rotation = -90;

        /* Create value axis */
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxis.fill = am4core.color("#fdd400");
        //valueAxis.renderer.labels.template.rotation = -60;
        /* Create series */
        // for (i = 0; i < jsonData.length - 1; i++) {
        var columnSeries = chart.series.push(new am4charts.ColumnSeries());
        columnSeries.name = 'title';

        columnSeries.dataFields.valueY = 'value';
        columnSeries.dataFields.categoryX = 'title';

        columnSeries.columns.template.tooltipText = "[#fff font-size: 8px]{name} in {categoryX}:\n[/][#fff font-size: 10px]{valueY}[/] [#fff]{additional}[/]"
        columnSeries.columns.template.propertyFields.fillOpacity = "fillOpacity";
        columnSeries.columns.template.propertyFields.stroke = "stroke";
        columnSeries.columns.template.propertyFields.strokeWidth = "strokeWidth";
        //columnSeries.strokeWidth = 0.5;

        columnSeries.columns.template.strokeWidth = -1;
        columnSeries.columns.template.propertyFields.strokeDasharray = "columnDash";
        columnSeries.tooltip.label.textAlign = "middle";
        ////*Red Colour append to bar graphs
        //var columnTemplate = columnSeries.columns.template;
        //columnTemplate.tooltipText = "{categoryX}: [bold]{valueY}[/]";
        //columnTemplate.fillOpacity = .8;
        //columnTemplate.strokeOpacity = 0;
        //columnTemplate.fill = am4core.color("red");

        columnSeries.columns.template.events.on("hit", ev => {

            var Searchtext = ev.target._dataItem.dataContext['title'];
            var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
            var oParams = [];
            var Param = {};
            Param["sName"] = "{XIP|sStatusName}";
            Param["sValue"] = Searchtext;
            oParams.push(Param);
            XILinkLoadJson(sRowXiLinkID, null, oParams, null);
        },
            this
        );
        //}
        // Set cell size in pixels
        var cellSize = 20;
        chart.events.on("datavalidated", function (ev) {

            // Get objects of interest
            var chartheight = ev.target;
            var categoryAxis = chartheight.yAxes.getIndex(0);

            // Calculate how we need to adjust chart height
            var adjustHeight = chartheight.data.length * cellSize - categoryAxis.pixelHeight;

            // get current chart height
            var targetHeight = chartheight.pixelHeight + adjustHeight;

            // Set it on chart's container
            chartheight.svgContainer.htmlElement.style.height = targetHeight + "px";
        });
        chart.cursor = new am4charts.XYCursor();
        var label = chart.chartContainer.createChild(am4core.Label);
        label.text = "AM4BarChart";
        label.align = "center";
        //var label2 = chart.chartContainer.createChild(am4core.Label);
        //label2.text = jsonData.CreatedDate;
        //label2.align = "right";
    }
    $('g[fill="#000000"] g').css("display", "none");
}
function fncDeviceAM4HeatComChart(jsonData, Keys) {
    if (jsonData != null) {
        var chartdata = {};
        var Year = [];
        var LeadCount = [];
        var OnCoverCount = [];
        var QuoteCount = [];
        $.each(jsonData, function (i, item) {
            if (jsonData != null) {
                // if (jsonData.BarAxis != "" && jsonData.BarAxis != null) {
                if (jsonData.BarAxis.toLowerCase() == "timeseries") {
                    //Year.push(item.sRowXiLinkType + '-01');
                    Year.push(item.sRowXiLinkType);
                }
                else {
                    Year.push(item.sRowXiLinkType);
                }
                LeadCount.push(parseInt(item.LeadCount));
                OnCoverCount.push((item.sHours));
                QuoteCount.push((item.sStatusName));
            }
        });
        var dataProvider = [];
        for (i = 0; i < Year.length; i++) {
            // if(key!="undefined"){
            var Dyanamicobj = {};
            Object.defineProperty(Dyanamicobj, Keys[0], { value: Year[i] });
            Object.defineProperty(Dyanamicobj, Keys[1], { value: QuoteCount[i] });
            Object.defineProperty(Dyanamicobj, Keys[2], { value: OnCoverCount[i] });
            Object.defineProperty(Dyanamicobj, Keys[3], { value: LeadCount[i] });
            dataProvider.push(Dyanamicobj);
            //}
        }
        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end
        var Name = "AM4HeatChart-" + parseInt(jsonData.OneClick);
        var chart = am4core.create(Name, am4charts.XYChart);
        //chart.exporting.menu = new am4core.ExportMenu();

        chart.maskBullets = false;
        chart.data = dataProvider;
        var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
        xAxis.dataFields.category = "" + Keys[0] + "";
        yAxis.dataFields.category = "" + Keys[1] + "";

        xAxis.renderer.grid.template.disabled = true;
        xAxis.renderer.minGridDistance = 40;

        yAxis.renderer.grid.template.disabled = true;
        yAxis.renderer.inversed = true;
        yAxis.renderer.minGridDistance = 30;

        var series = chart.series.push(new am4charts.ColumnSeries());
        series.dataFields.categoryX = "" + Keys[0] + "";
        series.dataFields.categoryY = "" + Keys[1] + "";
        series.dataFields.value = "" + Keys[3] + "";
        series.sequencedInterpolation = true;
        series.defaultState.transitionDuration = 3000;

        var bgColor = new am4core.InterfaceColorSet().getFor("background");

        var columnTemplate = series.columns.template;
        columnTemplate.strokeWidth = 1;
        columnTemplate.strokeOpacity = 0.2;
        columnTemplate.stroke = bgColor;
        //columnTemplate.tooltipText = "{weekday}, {hour}: {value.workingValue.formatNumber('#.')}";
        columnTemplate.tooltipText = "{" + Keys[0] + "}, " + "{" + Keys[1] + "}, {" + Keys[2] + "}" + " : {value.workingValue.formatNumber('#.')}";
        columnTemplate.width = am4core.percent(100);
        columnTemplate.height = am4core.percent(100);

        series.heatRules.push({
            target: columnTemplate,
            property: "fill",
            min: am4core.color(bgColor),
            max: chart.colors.getIndex(0)
        });

        // heat legend
        var heatLegend = chart.bottomAxesContainer.createChild(am4charts.HeatLegend);
        heatLegend.width = am4core.percent(100);
        heatLegend.series = series;
        heatLegend.valueAxis.renderer.labels.template.fontSize = 9;
        heatLegend.valueAxis.renderer.minGridDistance = 30;
        // Set cell size in pixels
        var cellSize = 8;
        chart.events.on("datavalidated", function (ev) {
            // Get objects of interest
            var chart = ev.target;
            var categoryAxis = chart.yAxes.getIndex(0);

            // Calculate how we need to adjust chart height
            var adjustHeight = chart.data.length * cellSize - categoryAxis.pixelHeight;

            // get current chart height
            var targetHeight = chart.pixelHeight + adjustHeight;

            // Set it on chart's container
            chart.svgContainer.htmlElement.style.height = targetHeight + "px";
        });
        //// heat legend behavior
        //series.columns.template.events.on("over", function (event) {
        //    
        //  handleHover(event.target);
        //})
        //series.columns.template.events.on("hit", function (event) {
        //    
        //    handleHover(event.target);
        //})
        series.columns.template.events.on(
            "hit",
            ev => {
                var Weekday = ev.target._dataItem.dataContext[Keys[0]];
                var Searchtext = ev.target._dataItem.dataContext[Keys[1]];
                var Hour = ev.target._dataItem.dataContext[Keys[2]];
                var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                //var SearchText = jsonData.OnClickColumn + "=" + d.id;
                // var SearchText = d.id;
                //sPieGUID = CreateGuid();
                var oParams = [];
                var Param = {};
                Param["sName"] = "{XIP|sStatusName}";
                Param["sValue"] = Searchtext;
                oParams.push(Param);
                var Param1 = {};
                Param1["sName"] = "{XIP|AMPM}";
                Param1["sValue"] = Hour;
                oParams.push(Param1);
                var Param2 = {};
                Param2["sName"] = "{XIP|WeekDay}";
                Param2["sValue"] = Weekday;
                oParams.push(Param2);
                if (jsonData.RowXilinkID != "") {
                    XILinkLoadJson(sRowXiLinkID, null, oParams, null);
                }
            },
            this
        );
        var label = chart.chartContainer.createChild(am4core.Label);
        label.text = "AM4HeatChart";
        label.align = "center";

        series.columns.template.events.on("out", function (event) {
            heatLegend.valueAxis.hideTooltip();
        })
    }
    $('g[fill="#000000"] g').css("display", "none");
}

function fncDeviceAM4LineComChart(jsonData, Keys) {

    if (jsonData != null) {
        var chartdata = {};
        var Year = [];
        var LeadCount = [];
        var OnCoverCount = [];
        var QuoteCount = [];
        $.each(jsonData, function (i, item) {
            if (jsonData != null) {
                // if (jsonData.BarAxis != "" && jsonData.BarAxis != null) {
                if (jsonData.BarAxis.toLowerCase() == "timeseries") {
                    //Year.push(item.sRowXiLinkType + '-01');
                    Year.push(item.sRowXiLinkType);
                }
                else {
                    Year.push(item.sRowXiLinkType);
                }
                LeadCount.push(parseInt(item.LeadCount));
                OnCoverCount.push(parseInt(item.CategoryID));
                QuoteCount.push(parseInt(item.iPaginationCount));
            }
        });
        var dataProvider = [];
        for (i = 0; i < Year.length; i++) {
            var date = new Date();
            date.setHours(0, 0, 0, 0);
            date.setDate(0);
            // if(key!="undefined"){
            var Dyanamicobj = {};
            Object.defineProperty(Dyanamicobj, Keys[0], { value: Year[i] });
            Object.defineProperty(Dyanamicobj, Keys[1], { value: LeadCount[i] });
            Object.defineProperty(Dyanamicobj, Keys[2], { value: OnCoverCount[i] });
            Object.defineProperty(Dyanamicobj, Keys[3], { value: QuoteCount[i] })
            dataProvider.push(Dyanamicobj);
            //}
        }

        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end

        var Name = "AM4LineChart-" + parseInt(jsonData.OneClick);
        var chart = am4core.create(Name, am4charts.XYChart);

        chart.colors.list = [
            am4core.color("#4a8ddc"),
            am4core.color("#4c5d8a"),
            am4core.color("#f3c911"),
            am4core.color("#dc5b57"),
            am4core.color("#33ae81"),
            am4core.color("#95c8f0"),
            am4core.color("#dd915f"),
            am4core.color("#DD4477"),
            am4core.color("#66AA00"),
            am4core.color("#B82E2E"),
            am4core.color("#316395"),
            am4core.color("#994499"),
            am4core.color("#22AA99"),
            am4core.color("#AAAA11"),
            am4core.color("#6633CC"),
            am4core.color("#E67300"),
            am4core.color("#8B0707"),
            am4core.color("#329262"),
            am4core.color("#5574A6"),
            am4core.color("#3B3EAC"),
        ];

        chart.data = dataProvider;

        var label = chart.chartContainer.createChild(am4core.Label);
        label.text = "AM4LineChart";
        label.align = "center";
        //chart.exporting.menu = new am4core.ExportMenu();
        chart.dateFormatter.inputDateFormat = "MMMM";
        var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
        dateAxis.renderer.minGridDistance = 60;
        dateAxis.startLocation = 0.5;
        dateAxis.endLocation = 0.5;
        // Set cell size in pixels
        var cellSize = 50;
        chart.events.on("datavalidated", function (ev) {
            // Get objects of interest
            var chartheight = ev.target;
            var categoryAxis = chartheight.yAxes.getIndex(0);

            // Calculate how we need to adjust chart height
            var adjustHeight = chartheight.data.length * cellSize - categoryAxis.pixelHeight;

            // get current chart height
            var targetHeight = chartheight.pixelHeight + adjustHeight;

            // Set it on chart's container
            chartheight.svgContainer.htmlElement.style.height = targetHeight + "px";
        });
        ////
        ////var categoryAxis = chart.yAxes.getIndex(0);

        ////  // Calculate how we need to adjust chart height
        ////  var adjustHeight = chart.data.length * cellSize - categoryAxis.pixelHeight;

        ////  // get current chart height
        ////  var targetHeight = chart.pixelHeight + adjustHeight;

        ////  // Set it on chart's container
        ////  chart.svgContainer.htmlElement.style.height = targetHeight + "px";
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxis.tooltip.disabled = true;
        //var series;
        //addSeries();
        //addSeries();
        for (i = 0; i < Keys.length - 1; i++) {
            var series = chart.series.push(new am4charts.LineSeries());
            series.dataFields.dateX = "" + Keys[0] + "";
            series.name = "" + Keys[i + 1] + "";
            series.dataFields.valueY = "" + Keys[i + 1] + "";
            //series.tooltipHTML = "<img src='https://www.amcharts.com/lib/3/images/car.png' style='vertical-align:bottom; margin-right: 10px; width:28px; height:21px;'><span style='font-size:14px; color:#000000;'><b>{valueY.value}</b></span>";
            series.tooltipText = "[#fff font-size: 10px]{name} in {categoryX}:\n[/][#fff font-size: 10px]{valueY}[/] [#fff]{additional}[/]";
            //series.tooltipText = "[#000]{valueY.value}[/]";
            series.tooltip.background.fill = am4core.color("#FFF");
            series.tooltip.getStrokeFromObject = true;
            series.tooltip.background.strokeWidth = 3;
            //series.tooltip.getFillFromObject = false;
            series.fillOpacity = 0.6;
            series.strokeWidth = 2;
            series.tensionX = 0.8;
            series.stacked = true;

            series.segments.template.interactionsEnabled = true;
            series.segments.template.events.on(
                "hit", function (ev) {
                    var Searchtext = ev.target.dataItem.component.tooltipDataItem.dataContext["" + Keys[0] + ""];
                    var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                    var oParams = [];
                    var Param = {};
                    Param["sName"] = "{XIP|sStatusName}";
                    //Param["sValue"] = listofdata;
                    Param["sValue"] = Searchtext;
                    oParams.push(Param);
                    XILinkLoadJson(sRowXiLinkID, null, oParams, null);
                }
            );
        }
        chart.cursor = new am4charts.XYCursor();
        chart.cursor.xAxis = dateAxis;
        chart.scrollbarX = new am4core.Scrollbar();

        // Add a legend
        chart.legend = new am4charts.Legend();
        chart.legend.position = "top";
    }
    $('g[fill="#000000"] g').css("display", "none");
}
function fncDeviceAM4pieChart(jsonData, Color, NewGUID) {

    var sPieGUID = "";
    if (jsonData != null) {
        if (jsonData.PieData != null && jsonData.PieData.length > 0) {
            var data = {};
            var status = [];
            var sites = [];
            for (i = 0; i < jsonData.PieData.length; i++) {

                if (jsonData.PieData[i].label) {
                    sites.push(jsonData.PieData[i].label);
                    status.push(jsonData.PieData[i].iStatus);
                    data[jsonData.PieData[i].label] = jsonData.PieData[i].value;
                }
            }
            // var data = jsonData;
            var dataProvider = [];

            for (var key in data) {
                // if(key!="undefined"){
                dataProvider.push({
                    value: data[key],
                    title: key,
                });
                //  }
            }
            var Name1 = "AM4Piechart-" + NewGUID + "-" + jsonData.sOneClickID;
            var chart = am4core.create(Name1, am4charts.PieChart);
            chart.id = Name1;
            chart.hiddenState.properties.opacity = 0; // 
chart.logo.disabled = true;
            //chart.exporting.menu = new am4core.ExportMenu();
            // Set data	
            chart.data = dataProvider;
            chart.data.id = Name1;// 'title';
// Set inner radiusif 
            if (jsonData.bDonutchart != null && jsonData.bDonutchart == true) {
                if (jsonData.sLegendRadius == null || jsonData.sLegendRadius == "") { chart.innerRadius = am4core.percent(50); }
                else { chart.innerRadius = am4core.percent(jsonData.sLegendRadius); }
            }
            // Create slices
            var pieSeries = chart.series.push(new am4charts.PieSeries());

            pieSeries.dataFields.value = 'value';
            pieSeries.dataFields.category = 'title';
            pieSeries.ticks.template.disabled = true;
            pieSeries.labels.template.disabled = true;
            pieSeries.hiddenState.properties.endAngle = -90;
            pieSeries.slices.template.tooltipText = true;
            //pieSeries.ticks.template.disabled = true;
            //pieSeries.alignLabels = false;
            // Calculate the total value of all slices
            var total = chart.data.reduce(function (sum, item) {
                return sum + item.value;
            }, 0);

            if (jsonData.bToolTip != null && jsonData.bToolTip == true) {
                pieSeries.slices.template.tooltipText = "{category}: {value}";
            }
            var colorSet = new am4core.ColorSet();
            colorSet.list = Color.map(function (color) {
                return new am4core.color(color);
            });
            pieSeries.colors = colorSet;
            // These functions are here so the inner labels don't show whenever the slice is less than 5 percent
            pieSeries.labels.template.adapter.add('radius', function (radius, target) {
                if (target.dataItem && (target.dataItem.values.value.percent < 5)) {
                    target.dataItem.label.text = '';
                    return 0;
                }
                return radius;
            });
            //var label = chart.chartContainer.createChild(am4core.Label);
            //label.text = "AM4PieChart";
            //label.align = "Right";
            if (jsonData.bIsLegends != null && jsonData.bIsLegends == true) {
                chart.legend = new am4charts.Legend();

                if (jsonData.sLegendPosition == null || jsonData.sLegendPosition == "") {
                    chart.legend.position = "bottom";
                }
                else if (jsonData.sLegendPosition =='middle') {
                    // Add a label in the center of the pie chart
                    var label = pieSeries.createChild(am4core.Label);
                    label.text = total.toString()+'Total'; // Display the total value
                    label.horizontalCenter = "middle";
                    label.verticalCenter = "middle";
                    label.fontSize = 20;
                }
                else {
                    chart.legend.position = jsonData.sLegendPosition;
                }
                // Set maximum radius for the pie chart
                //if (jsonData.sLegendRadius == null || jsonData.sLegendRadius == "")
                //    {
                //        pieSeries.maxRadius = am4core.percent(50); // Adjust as needed

                //    }
                //    else {
                //        pieSeries.maxRadius = am4core.percent(jsonData.sLegendRadius); // Adjust as needed

                //    }
                pieSeries.maxRadius = am4core.percent(jsonData.sLegendRadius); // Adjust as needed
                chart.legend.valueLabels.template.text = "{value}";
                chart.legend.maxWidth = 150;
                chart.legend.fontSize = "12px";
                chart.legend.maxHeight = 200;
                chart.legend.scrollable = true;
                chart.legend.labels.template.maxWidth = 80;
                chart.legend.labels.template.truncate = true;
                chart.legend.itemContainers.template.paddingTop = 5;
                chart.legend.itemContainers.template.paddingBottom = 0;
                var markerTemplate = chart.legend.markers.template;
                markerTemplate.width = 10;
                markerTemplate.height = 10;
            }
            var cellSize = 20;
            chart.events.on("datavalidated", function (ev) {
                // Get objects of interest
                var chart = ev.target;
                //var categoryAxis = 1000;

                // Calculate how we need to adjust chart height
                var adjustHeight = chart.data.length * cellSize - 10;

                // get current chart height
                var targetHeight = chart.pixelHeight + adjustHeight;

                // Set it on chart's container
                chart.svgContainer.htmlElement.style.height = targetHeight + "px";
            });
            pieSeries.slices.template.events.on("hit", (ev) => {
                // event.target is the clicked Slice
                var chartdiv = $('#' + Name1); var sGUID = '';
                if (chartdiv && chartdiv.length > 0) {  sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', chartdiv[0]); }
                var Searchtext = ev.target._dataItem.dataContext["title"];
                var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                var oParams = [];
                var Param1 = {}; var Param2 = {}; var Param3 = {};
                Param1["sName"] = "{XIP|sStatusName}";
                //Param["sValue"] = listofdata;
                Param1["sValue"] = Searchtext;
                oParams.push(Param1);
                // Extracting the integer value using regular expressions
                var intValue = Searchtext.match(/\d+/);
                var intValueParsed = parseInt(intValue);
                Param2["sName"] = '{-iInstanceID}';
                Param2["sValue"] = intValueParsed;
                // Parsing the integer value to ensure it's an integer               
                oParams.push(Param2);
 		Param3["sName"] = "sName";
                //Param["sValue"] = listofdata;
                Param3["sValue"] = Searchtext;
                oParams.push(Param3);
                debugger;
                XILinkLoadJson(sRowXiLinkID, sGUID, oParams, null);
            });
        }

    }

    //$("#Piechart").chart.logo.disabled = true;
    //$('g[fill="#000000"] g').css("display", "none");
}
function fncDeviceAM4SemipieChart(jsonData) {
    var sPieGUID = "";
    $("#SemiPiechart").empty();
    if (jsonData != null) {
        if (jsonData.PieData.length > 0) {
            var data = {};
            var status = [];
            var sites = [];
            for (i = 0; i < jsonData.PieData.length; i++) {

                if (jsonData.PieData[i].label) {
                    sites.push(jsonData.PieData[i].label);
                    status.push(jsonData.PieData[i].iStatus);
                    data[jsonData.PieData[i].label] = jsonData.PieData[i].value;
                }
            }
            // var data = jsonData;
            var dataProvider = [];

            for (var key in data) {
                // if(key!="undefined"){
                dataProvider.push({
                    value: data[key],
                    title: key,
                });
                //  }
            }
            var Name1 = "SemiPiechart-" + jsonData.ReportID;
            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end

            var chart = am4core.create(Name1, am4charts.PieChart);
            // var chart = am4core.create(Name1, am4charts.PieChart);
            chart.hiddenState.properties.opacity = 0; // this creates initial fade-in
            //chart.exporting.menu = new am4core.ExportMenu();
            chart.data = dataProvider;
            chart.data.id = 'title';


            chart.radius = am4core.percent(70);
            chart.innerRadius = am4core.percent(40);
            chart.startAngle = 180;
            chart.endAngle = 360;

            var series = chart.series.push(new am4charts.PieSeries());
            series.dataFields.value = "value";
            series.dataFields.category = "title";

            series.slices.template.cornerRadius = 10;
            series.slices.template.innerCornerRadius = 7;
            series.slices.template.draggable = true;
            series.slices.template.inert = true;
            series.alignLabels = false;

            series.hiddenState.properties.startAngle = 90;
            series.hiddenState.properties.endAngle = 90;

            chart.legend = new am4charts.Legend();
            var label = chart.chartContainer.createChild(am4core.Label);
            label.text = "AM4SemiPieChart";
            label.align = "center";
            var cellSize = 20;
            chart.events.on("datavalidated", function (ev) {
                // Get objects of interest
                var chart = ev.target;
                //var categoryAxis = 1000;

                // Calculate how we need to adjust chart height
                var adjustHeight = chart.data.length * cellSize - 10;

                // get current chart height
                var targetHeight = chart.pixelHeight + adjustHeight;

                // Set it on chart's container
                chart.svgContainer.htmlElement.style.height = targetHeight + "px";
            });
            series.slices.template.events.on("hit", (ev) => {
                // event.target is the clicked Slice
                var Searchtext = ev.target._dataItem.dataContext["title"]
                var sRowXiLinkID = parseInt(jsonData.RowXilinkID);
                //var SearchText = jsonData.OnClickColumn + "=" + d.id;
                // var SearchText = d.id;
                //sPieGUID = CreateGuid();
                var oParams = [];
                var Param = {};
                Param["sName"] = "{XIP|sStatusName}";
                //Param["sValue"] = listofdata;
                Param["sValue"] = Searchtext;
                oParams.push(Param);
                XILinkLoadJson(sRowXiLinkID, null, oParams, null);
            });
        }
    }
    $('g[fill="#000000"] g').css("display", "none");
}
function fncDeviceAM4GaugeChart(JsonData) {
    if (JsonData != null) {
        am4core.useTheme(am4themes_animated);

        // create chart
        var Name = "gaugechart-" + JsonData.OnClickResultID;
        var chart = am4core.create(Name, am4charts.GaugeChart);
        chart.innerRadius = am4core.percent(82);

        /**
         * Normal axis
         */

        var axis = chart.xAxes.push(new am4charts.ValueAxis());
        axis.strictMinMax = true;
        axis.renderer.inside = true;
        axis.renderer.line.strokeOpacity = 1;
        axis.renderer.ticks.template.strokeOpacity = 1;
        axis.renderer.grid.template.disabled = true;
        axis.renderer.labels.template.radius = 50;

        /**
         * Axis for ranges
         */
        var colorSet = new am4core.ColorSet();

        var axis2 = chart.xAxes.push(new am4charts.ValueAxis());
        axis2.min = 0;
        axis2.max = 100;
        axis2.strictMinMax = true;
        axis2.renderer.labels.template.disabled = true;
        axis2.renderer.ticks.template.disabled = true;
        axis2.renderer.grid.template.disabled = true;
        var val = parseInt(JsonData.GaugeChartvalue.replace("%", ""));

        var range0 = axis2.axisRanges.create();
        range0.endValue = val;
        range0.Value = 100;
        range0.axisFill.fillOpacity = 1;
        range0.axisFill.fill = colorSet.getIndex(0);

        var range1 = axis2.axisRanges.create();
        range1.value = 50;
        range1.endValue = 100;
        range1.axisFill.fillOpacity = 1;
        range1.axisFill.fill = colorSet.getIndex(1);
        /**
         * Label
         */

        var label = chart.radarContainer.createChild(am4core.Label);
        label.isMeasured = false;
        label.fontSize = 45;
        //label.x = am4core.percent(10);
        //label.y = am4core.percent(100);
        label.horizontalCenter = "middle";
        label.verticalCenter = "bottom";
        label.text = JsonData.GaugeChartvalue;

        /**
         * Hand
         */

        var hand = chart.hands.push(new am4charts.ClockHand());
        hand.axis = axis2;
        hand.innerRadius = am4core.percent(20);
        hand.startWidth = 10;
        hand.pin.disabled = true;
        hand.value = val;

        hand.events.on("propertychanged", function (ev) {
            range0.endValue = ev.target.value;
            range1.value = ev.target.value;
            axis2.invalidate();
        });
        $('g[fill="#000000"] g').css("display", "none");
    }
}
function fncLoadSection(_this, sValue) {
    var Value = $(_this).find('option:selected').val();
    var sMessage = $('input[name="sFeedMessage"]').val();
    if (Value && Value.length > 0) {
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: GetBOIURL,
            datatype: 'json',
            cache: false,
            async: true,
            //data: { sStep: DefaultStep, iQSID: iQSDID, i1ClickID: i1ClickID, sDefaultStep: DefaultStep, iInstanceID: iInstanceID, sGUID: sGUID },
            data: JSON.stringify({ iBOIID: Value, sBO: "OrgObjectType" }),
            success: function (sBO) {
                var InputParams = [];
                var param1 = {};
                param1["sName"] = '{XIP|ActiveBO}';
                param1["sValue"] = sBO;
                InputParams.push(param1);
                var param2 = {};
                param2["sName"] = 'iOrgObjectTypeID';
                param2["sValue"] = Value;
                InputParams.push(param2);
                var param3 = {};
                param3["sName"] = 'sFeedMessage';
                param3["sValue"] = sMessage;
                InputParams.push(param3);
                var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', _this);
                fncUpdateXIParams("", sGUID, "", InputParams).then(function (data) {
                    var QSInfo = fncQSInfoFromHTMLTree(_this);
                    var iQSDID = QSInfo[0].sValue;
                    $.ajax({
                        contentType: 'application/json; charset=utf-8',
                        url: LoadStepContentURL,
                        datatype: 'json',
                        cache: false,
                        async: false,
                        //data: { sStep: DefaultStep, iQSID: iQSDID, i1ClickID: i1ClickID, sDefaultStep: DefaultStep, iInstanceID: iInstanceID, sGUID: sGUID },
                        data: { sStep: 11245, iQSID: iQSDID, i1ClickID: "0", sDefaultStep: 11245, iInstanceID: "0", sGUID: sGUID },
                        success: function (oStepI) {
                            var _uidialog = fncgetDialogFromHTMLTree('dialog', _this);
                            var Output = "DependencyArea";
                            $(_this).attr('data-load', 'true');
                            fncRenderStepContent(oStepI, '#' + Output, "", null, _uidialog).then(function (ConfigParams) {
                                $(_uidialog).find('#' + Output).find('.loader').remove();
                                fncLoadScroll(_uidialog, sLayoutType);
                            });

                        }
                    })
                })
            }
        })

    }
}
function fncUpdateNannoFeed(sMessage, Time, sMesID, iLayoutID, sBOInfo, sOrgs, iOrg, sMode, sType, bUpdate) {
    var Split = sBOInfo.split('-');
    var iBODID = Split[0];
    var iBOIID = Split[1];
    var Orgs = sOrgs.split(',');
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: GetBONameURL,
        datatype: 'json',
        cache: false,
        async: true,
        data: JSON.stringify({ iBODID: iBODID }),
        success: function (sBO) {
            var InputParams = [];
            var param1 = {};
            param1["sName"] = '{XIP|ActiveBO}';
            param1["sValue"] = sBO;
            InputParams.push(param1);
            var param2 = {};
            param2["sName"] = '{-iInstanceID}';
            param2["sValue"] = iBOIID;
            InputParams.push(param2);
            if (sMode == "Scroll") {
                param2 = {};
                param2["sName"] = 'iNannoOrgID';
                param2["sValue"] = sOrgs;
                InputParams.push(param2);
            }
            else {
                param2 = {};
                param2["sName"] = 'iNannoOrgID';
                param2["sValue"] = iOrg;
                InputParams.push(param2);
            }
            var LayoutClass = 'Layout_' + iLayoutID + ' ' + iBOIID + '-' + iBODID + '-' + sMesID;

            if (sMode == "Scroll") {
                if (sType == "Right") {
                    var sSenderHTML = "";
                    sSenderHTML = '<div class="user reciever"><div class="msgContent viewed"><p>' + sMessage + '</p><div class="' + LayoutClass + '"></div><span class="msgTime">' + Time + '</span></div></div>';
                    $('.Nanno-' + iOrg).prepend(sSenderHTML);
                    var NannoDiv = $('.Nanno-' + iOrg);
                    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', NannoDiv[0]);
                    fncLoadLayout(iLayoutID, InputParams, sGUID, iBOIID + '-' + iBODID + '-' + sMesID, 'Nanno');
                }
                else if (sType == "Left") {
                    var sRecieveHTML = "";
                    sRecieveHTML = '<div class="user sender"><div class="msgContent"><p>' + sMessage + '</p><div class="' + LayoutClass + '"></div><span class="msgTime">' + Time + '</span></div></div>';
                    $('.Nanno-' + iOrg).prepend(sRecieveHTML);
                    var NannoDiv = $('.Nanno-' + iOrg);
                    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', NannoDiv[0]);
                    fncLoadLayout(iLayoutID, InputParams, sGUID, iBOIID + '-' + iBODID + '-' + sMesID, 'Nanno');
                }
            }
            else if (sMode == "SignalR") {
                var sSenderHTML = "";
                sSenderHTML = '<div class="user reciever"><div class="msgContent viewed"><p>' + sMessage + '</p><div class="' + LayoutClass + '"></div><span class="msgTime">' + Time + '</span></div></div>';
                if (bUpdate) {
                    //$('.Nanno-' + iOrg).append(sSenderHTML);
                }
                else {
                    $('.Nanno-' + iOrg).append(sSenderHTML);
                }
                var iNannoOrgID = 0;
                var NannoDiv = $('div[data-type="nanno"]');
                if (NannoDiv && NannoDiv.length > 0) {
                    iNannoOrgID = $(NannoDiv[0]).attr('data-identity');
                    var bValidOrg = false;
                    var sRecieveHTML = "";
                    sRecieveHTML = '<div class="user sender"><div class="msgContent"><p>' + sMessage + '</p><div class="' + LayoutClass + '"></div><span class="msgTime">' + Time + '</span></div></div>';
                    for (var i = 0; i < Orgs.length; i++) {
                        if (bUpdate) {
                            if (Orgs[i] == iNannoOrgID) {
                                bValidOrg = true;
                            }
                        }
                        else {
                            if (Orgs[i] == iNannoOrgID) {
                                bValidOrg = true;
                            }
                            $('.Nanno-' + Orgs[i]).append(sRecieveHTML);
                        }
                    }

                }
                var NannoDiv = $('.Nanno-' + iOrg);
                if (NannoDiv && NannoDiv.length > 0) {
                    var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', NannoDiv[0]);
                    fncLoadLayout(iLayoutID, InputParams, sGUID, iBOIID + '-' + iBODID + '-' + sMesID, 'Nanno');
                }
                if (bValidOrg) {
                    var NannoDiv = $('.Nanno-' + iNannoOrgID);
                    if (NannoDiv && NannoDiv.length > 0) {
                        var sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', NannoDiv[0]);
                        fncLoadLayout(iLayoutID, InputParams, sGUID, iBOIID + '-' + iBODID + '-' + sMesID, 'Nanno');
                    }

                }

            }
        }
    });
}
function fncLoadFeed(SecID, sGUID, iPage) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: LoadFeedURL,
        datatype: 'json',
        cache: false,
        async: true,
        //data: { sStep: DefaultStep, iQSID: iQSDID, i1ClickID: i1ClickID, sDefaultStep: DefaultStep, iInstanceID: iInstanceID, sGUID: sGUID },
        data: JSON.stringify({ SecID: SecID, sGUID: sGUID, sPageNo: iPage }),
        success: function (Data) {
        }
    })
}
function fncShowGroup($this) {
    ChangeFields = $("#ChangeFields").val();
    var MyURL = '/xilink/ChangeFields';;//ChangeFields.replace("/QueryGeneration", "");
    var AutoCompleteURL = '@Url.Action("GetAutoCompleteList", "BusinessObjects")';
    var formid = $($this).find('.CreateForm').attr('id');
    if (formid == undefined) {
        formid = $($this).closest('form').attr('id');
    }
    $('#' + formid).attr("data-instanceid");
    var AttributeID = $($this).attr('id');
    var NewSGUID = formid.split('_')[1];
    var selectedOption = $($this).children(":selected").attr("data-hide");
    var Value = {
        sShowField: $('#' + formid).find('#' + AttributeID).children(":selected").attr("data-show"),
        sHideField: $('#' + formid).find('#' + AttributeID).children(":selected").attr("data-hide"),
        BOID: $('#' + formid).attr("data-sbo"),
        sID: $('#' + formid).attr("data-instanceid")
    }
    $.ajax({
        type: 'POST',
        url: MyURL,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(Value),
        cache: false,
        async: true,
        dataType: 'json',
        success: function (data) {
            // 
            if (selectedOption == "ScriptGroup") {
                $("#ScriptIcon").show();
                $("#ScriptIcon").removeAttr("style");
            } else {
                $("#ScriptIcon").hide();
            }
            if (data != 0) {
                $.each(data[0], function (index, value) {
                    $('#' + value.ID).remove();
                });
                if (data.length > 1) {
                    $.each(data[1], function (index, value) {
                        if (value.OptionList.length > 0) {
                            var Html = '<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><div class="select-wrapper"><select name="' + value.Name + '" class="form-control" id="DDL-' + value.Name + NewSGUID + '" ' + value.sEventHandler + ' data-attrname="' + value.Name + '">'
                            Html += '<option value="" class="ps">Please Select</option>'
                            $.each(value.OptionList, function (index1, value1) {
                                if (value1.sOptionName != null && value1.sOptionName == "please select") {
                                    bIsSelect = true;
                                }
                                else {
                                    if (value.sResolvedValue == undefined) {
                                        Html += '<option value="' + value1.sValues + '">' + value1.sOptionName + '</option>'
                                    }
                                    else if (value1.sOptionName == value.sResolvedValue) {
                                        Html += '<option value="' + value1.sValues + '" selected>' + value1.sOptionName + '</option>'
                                    }
                                    else {
                                        Html += '<option value="' + value1.sValues + '">' + value1.sOptionName + '</option>'
                                    }
                                }
                            })
                            Html += '</select><span class="select_wrapper_caret fas fa-chevron-down"></span></div></div></div></div>'
                            $($this).closest('div[class^="form-group"]').after(Html);
                        }
                        else if (value.sFKBOSize == "20") {
                            var NewGUID = CreateGuid();
                            var Html = '<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><div class="select-wrapper">';
                            if (value.Value != "0" && value.Value != "" && value.Value != null) {
                                Html += '<input id="DDLShow-' + value.Name + NewGUID + '" type="text" data-name="' + value.Name + '" class="form-control XIField ' + value.Name + '" value="' + value.sResolvedValue + '" data-sPreviousValue="' + value.Value + '" />';
                            }
                            else {
                                Html += '<input id="DDLShow-' + value.Name + NewGUID + '" type="text" data-name="' + value.Name + '" value="" class="form-control XIField ' + value.Name + '" placeholder="' + value.sPlaceHolder + '" data-sPreviousValue="' + value.Value + '" />';
                            }
                            Html += '<input id="DDL-' + value.Name + NewGUID + '" type="hidden" name="' + value.Name + '" value="' + value.Value + '" data-IsValid="" />';
                            var url = "'/BusinessObjects/GetAutoCompleteList'";
                            Html += '<script>    var iBODID = 0;    var sBOName = "' + value.sFKBOName + '";    var fieldVal = "' + value.Value + '";    var AutoCompleteList = 0;    var sRefType = "bo";    if ("' + value.iOneClickID + '" > 0) {        sRefType = "1click";        iBODID = "' + value.iOneClickID + '";    }    $.ajax({        url:  ' + url + ',type: "POST", contentType: "application/json; charset=utf-8", datatype: "json",async: true,cache: false, data: JSON.stringify({ sType: sRefType, iBODID: iBODID, sBOName: sBOName, sGUID: "" }),success: function (data) {AutoCompleteList = data; if ("' + value.iOneClickID + '" > 0) { AutoCompleteDict.push({ key: "' + value.iOneClickID + '", value: AutoCompleteList});}else if ("' + value.sFKBOName + '" != "") {AutoCompleteDict.push({ key: "' + value.sFKBOName + '",value: AutoCompleteList});}if ("' + value.Value + '" && "' + value.Value + '".length > 0) {var autoDict = 0;if ("' + value.iOneClickID + '" > 0) {for (var k = 0; k < AutoCompleteDict.length; k++) { if (AutoCompleteDict[k].key == "' + value.iOneClickID + '") { autoDict = AutoCompleteDict[k].value;}}}else if ("' + value.sFKBOName + '" != "") { for (var k = 0; k < AutoCompleteDict.length; k++) {if (AutoCompleteDict[k].key == "' + value.sFKBOName + '") {autoDict = AutoCompleteDict[k].value;}}} var sResValue = Object.keys(autoDict).filter(function (key) { return autoDict[key] === "' + value.Value + '" })[0];$("#DDLShow-' + value.Name + NewGUID + '").val(sResValue); }}});';
                            Html += '$(document.body).on("change", "#DDLShow-' + value.Name + NewGUID + '", function () {var sResult = 0; if ("' + value.iOneClickID + '" > 0) {for (var k = 0; k < AutoCompleteDict.length; k++) {if (AutoCompleteDict[k].key == "' + value.iOneClickID + '") {sResult = AutoCompleteDict[k].value;}}} else if ("' + value.sFKBOName + '" != "") { for (var k = 0; k < AutoCompleteDict.length; k++) {if (AutoCompleteDict[k].key == "' + value.sFKBOName + '") {sResult = AutoCompleteDict[k].value;}}}var txtValue = $(this).val(); $("#DDL-' + value.Name + NewGUID + '").val("");if (typeof txtValue !== "undefined" && txtValue != "") {var sValue = sResult[$(this).val()];if (typeof sValue !== "undefined" && sValue != "") { $("#DDL-' + value.Name + NewGUID + '").val(sValue); $("#DDL-' + value.Name + NewGUID + '").attr("data-IsValid", true);  $("span.' + value.Name + value.sFKBOName + NewGUID + '").closest(".highlight--help").removeClass("msg-error"); $("span.' + value.Name + value.sFKBOName + NewGUID + '").html("");} else { $("#DDL-' + value.Name + NewGUID + '").attr("data-IsValid", false); $("span.' + value.Name + value.sFKBOName + NewGUID + '").closest(".highlight--help").addClass("msg-error"); $("span.' + value.Name + value.sFKBOName + NewGUID + '").html("Please select ' + value.LabelName + '");}}});';
                            Html += '$("#DDLShow-' + value.Name + NewGUID + '").autocomplete({source: function (request, response) {var DOptionsList = 0;if ("' + value.iOneClickID + '" > 0) { for (var k = 0; k < AutoCompleteDict.length; k++) {if (AutoCompleteDict[k].key == "' + value.iOneClickID + '") {DOptionsList = AutoCompleteDict[k].value;}}} else if ("' + value.sFKBOName + '" != "") {for (var k = 0; k < AutoCompleteDict.length; k++) { if (AutoCompleteDict[k].key == "' + value.sFKBOName + '") {DOptionsList = AutoCompleteDict[k].value;}}} $("#DDL-' + value.Name + NewGUID + '").val(""); var List = []; $.each(DOptionsList, function (key, value) { List.push({ label: key, value: value }); }); response($.ui.autocomplete.filter(List, request.term));}, select: function (event, ui) { $("#DDLShow-' + value.Name + NewGUID + '").val(ui.item.label); $("#DDL-' + value.Name + NewGUID + '").val(ui.item.value);return false;},focus: function (event, ui) {$("#DDLShow-' + value.Name + NewGUID + '").val(ui.item.label); $("#DDL-' + value.Name + NewGUID + '").val(ui.item.value); return false;}});</script>';
                            Html += '</div></div></div></div>'
                            $($this).closest('div[class^="form-group"]').after(Html);
                        }
                        else if (value.sFKBOSize == "30") {
                            var Html = "";
                            Html = '<input type="text" class="form-control autocomplete" value="' + value.sResolvedValue + '" placeholder="Type Here" id="DDL-"' + value.sName + NewGUID + '" data-value="0" onkeyup="fncgetautocomplete("' + value.iOneClickID + '", this, event)" data-sPreviousValue="' + value.sPreviousValue + '" /> <input id="DDL-"' + value.sName + NewGUID + '" type="hidden" name="' + value.Name + '" value="' + value.sResolvedValue + '" />';
                        }
                        else if (value.TypeID == 20) {
                            var checked = "";
                            if (value.Value != "False" || value.Value != "" || value.Value != null) {
                                checked = "checked";
                            }
                            $($this).closest('div[class^="form-group"]').after('<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><input type="checkbox" class="CheckboxClass" data-attrname="' + value.Name + '" "' + checked + '"/><i class="check"></i><input type="hidden" class="form-control" id="' + value.Name + '" name="' + value.Name + '" placeholder="' + value.sPlaceHolder + '"/></div></div></div>');
                        }
                        else if (value.Value == null || value.Value == "") {
                            //$('#' + formid).find('.form-group').last().after('<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><input type="text" class="form-control" id="' + value.Name + '" name="' + value.Name + '"/></div></div></div>');
                            $($this).closest('div[class^="form-group"]').after('<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><input type="text" class="form-control" id="' + value.Name + '" name="' + value.Name + '" placeholder="' + value.sPlaceHolder + '"/></div></div></div>');
                        }
                        else {
                            //$('#' + formid).find('.form-group').last().after('<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><input type="text" class="form-control" id="' + value.Name + '" name="' + value.Name + '"/></div></div></div>');
                            $($this).closest('div[class^="form-group"]').after('<div class="form-group highlight--help off" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.LabelName + '</label><div class="form-input"><input type="text" class="form-control" id="' + value.Name + '" name="' + value.Name + '" value="' + value.Value + '"/></div></div></div>');
                        }
                    });
                }
            }
        },
        error: function (data) {
        }
    })
}

function fncGetDependentFields($this, i1ClickID) {
    var formid = $($this).closest('form').attr('id');
    var NewSGUID = formid.split('_')[1];
    var iInstanceID = $('.fc-head').text().split(' ')[$('.fc-head').text().split(' ').length - 1];
    var MyURL = "/SystemsDNA/xilink/GetDependentFields";
    var obj = [];
    var oNVParams = new Object();
    oNVParams.sName = '{XIP|AlgorithmActionID}';
    oNVParams.sValue = $this.value;
    obj.push(oNVParams);
    oNVParams = new Object();
    oNVParams.sName = '-iInstanceID';
    oNVParams.sValue = iInstanceID;
    obj.push(oNVParams);
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    $.ajax({
        type: 'POST',
        url: MyURL,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ i1ClickID: i1ClickID, sGUID: sGUID, oNVParams: obj }),
        cache: false,
        async: true,
        dataType: 'json',
        success: function (datas) {
            $(".undefined").remove();
            $.each(datas, function (index, value) {
                if (value.FieldDDL.length > 0) {
                    var Html = '<div class="form-group highlight--help off ' + value.ID + '" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.sDisplayName + '</label><div class="form-input"><div class="select-wrapper"><select name="' + value.sValue + '" class="form-control" id="DDL-' + value.sValue + NewSGUID + '" data-attrname="' + value.sValue + '">'
                    Html += '<option value="" class="ps">Please Select</option>'
                    $.each(value.FieldDDL, function (index1, value1) {
                        if (value1.Expression != null && value1.Expression == "please select") {
                            bIsSelect = true;
                        }
                        else {
                            if (value.sPreviousValue == undefined) {
                                Html += '<option value="' + value1.text + '">' + value1.text + '</option>'
                            }
                            else if (value1.text == value.sPreviousValue) {
                                Html += '<option value="' + value1.text + '" selected>' + value1.text + '</option>'
                            }
                            else {
                                Html += '<option value="' + value1.text + '">' + value1.text + '</option>'
                            }
                        }
                    })
                    Html += '</select></div></div></div></div>'
                    $('#' + formid).find('.form-group').last().after(Html);
                } else {
                    if (value.Format == "10") {
                        $('#' + formid).find('.form-group').last().after('<div class="form-group highlight--help off ' + value.ID + '" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.sDisplayName + '</label><div class="form-input"><input type="text" class="form-control" placeholder="Required" id="' + value.sValue + '" name="' + value.sValue + '"/></div></div></div>');
                    } else if (value.Format == "20") {
                        $('#' + formid).find('.form-group').last().after('<div class="form-group highlight--help off ' + value.ID + '" id="' + value.ID + '"><div class="wrap-width clearfix control-block"> <label class="form-label">' + value.sDisplayName + '</label><div class="form-input"><input type="text" class="form-control" placeholder="Optional" id="' + value.sValue + '" name="' + value.sValue + '"/></div></div></div>');
                    }
                }
            })
        },
        error: function (datas) {
        }
    })
}
function fncLoadDefaultFKPopup(_this, iPopupID, sValue, sFKBOName, sAttr) {
    var sValue = $(_this).parent('div').parent('div').find('.select-wrapper').find('select').find('option:selected').val();
    if (sValue && parseInt(sValue) > 0)
        fncXILinkLoad(iPopupID, sValue, sFKBOName, sAttr)
}

function fncResloveLinkSecureKey(iLinkID) {
    return new Promise(function (resolve, reject) {
        var Params = {
            sSecureKey: iLinkID
        }
        $.ajax({
            type: 'POST',
            url: SecureKeyURL,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'json',
            success: function (oResponse) {
                resolve(oResponse);
            },
            error: function (err) {

            }
        });
    })
}

function fncCheckLinkAccess(iLinkID) {
    return new Promise(function (resolve, reject) {
        var Params = {
            iLinkID: iLinkID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: CheckLinkAccessURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                resolve(data);
            }
        })
    })
}
function fncGetDynamicDefaultPopup(iBODID, iBOIID) {
    return new Promise(function (resolve, reject) {
        debugger
        var Params = {
            iBODID: iBODID,
            iBOIID: iBOIID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: GetDynamicDefaultPopupURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                debugger
                resolve(data);
            }
        })
    })
}

function fncLoadDefaultObjectPopup(iBODID, iBOIID) {
    fncGetDefaultPopup(iBODID).then(function (iPopupID) {
        fncXILinkLoad(iPopupID, iBOIID)
    })
}
function fncGetDefaultPopup_Chat(iBODID) {
    return new Promise(function (resolve, reject) {
        var Params = {
            iBODID: iBODID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: GetDefaultPopupChatURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                resolve(data);
            }
        })
    })
}


function fncGetDefaultPopup(iBODID) {
    return new Promise(function (resolve, reject) {
        var Params = {
            iBODID: iBODID
        }
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: GetDefaultPopupURL,
            data: JSON.stringify(Params),
            cache: false,
            async: true,
            dataType: 'JSON',
            success: function (data) {
                resolve(data);
            }
        })
    })
}

function fncDialogSplit(_this) {
    var SplitType = $(_this).attr('data-type');
    if (SplitType && SplitType.length > 0) {
        var Tasks = $('#NavigationBar').find('.btnTabs').find('.taskbtn');
        if (Tasks && Tasks.length > 0) {
            for (var k = 0; k < Tasks.length > 0; k++) {
                var TaskID = $(Tasks[k]).attr('id');
                $('div[data-identity=' + TaskID + ']').width(500);
            }
        }

    }
}

//--------------------DATABASE SCHEMA AND DATA COMPARSION---------------------------//
function DBSchemaCompare($this, DBType, iInstanceID, sBOName, jsonNodes, ComparingType) {

    $.blockUI({
        //'<h4>Please Wait...</h4>'
        message: '<img src="' + sImagePath + '/loading.gif" style="width: 30px;" /> Please wait...',
        blockMsgClass: 'report-success',
    });
    var formid = $($this).closest('form').attr('id');
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var FormValues = [];
    for (var i = 0; i < formData.length; i++) {
        //FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
        FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
    }
    if (DBType == "Schema") {
        //var DBSchemaCompare1='/DataBase/NewDataSchemaComapare';
        $.ajax({
            url: SchemaCompareforDB,
            // url:'@Url.Action("DataSchemaComapare","DataBase")',
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "JSON",
            cache: false,
            async: false,
            data: JSON.stringify({ oNVParams: FormValues, sBoName: sBOName }),
            success: function (data) {
                //$('#HTMLDataResult').html('<div><button style="float: right;" id="myButton" class="label-success btn btn-success btn-xs" onclick=DownloadfncPDFRecipt("' + iInstanceID + '","' + sGUid + '","' + StructureID + '","' + TemplateID + '") >Export to PDF</button><div id="imgh" style="float: right;"><img src="' + sImagePath + '/loading.gif"/></div></div>' + data);
                $('#DBSchemaComparison').html(data);
                $('#imgh').hide();
                $.unblockUI();
            },
            error: function (textStatus, errorThrown) {
                $('#DBSchemaComparison').html('<label><strong>No Records Found</strong></label>');
                $.unblockUI();
            }
        });
    }
    else if (DBType == "FullSchema") {
        $.ajax({
            url: SchemaCompareforDB,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "JSON",
            cache: false,
            async: false,
            data: JSON.stringify({ oNVParams: FormValues, sBoName: sBOName, sType: "FullSchema" }),
            success: function (data) {
                $('#DBSchemaComparison').html(data);
                $('#imgh').hide();
                $.unblockUI();
            },
            error: function (textStatus, errorThrown) {
                $('#DBSchemaComparison').html('<label><strong>No Records Found</strong></label>');
                $.unblockUI();
            }
        });
    }

    else if (DBType == 'Data') {
        var selectedText = $('#QueryType').find("option:selected").text();
        var selectedValue = $('#QueryType').val();
        var Checkbox = $('#Excludelargetables').val();
        var bolist = $('#searchtables').val();
        var valutag = $('#Searchtasknames').val();
        oTable = $('#DBCompareDataTable').DataTable();   //pay attention to capital D, which is mandatory to retrieve "api" datatables' object, as @Lionel said
        var StructureName = $('select[name="StructureName"] option:selected').text();
        var iBOID = $('#id_BOs').val();
        $('#DBSchemaComparison').html('');
        $('#DBSchemaComparison').append('<div id="DataCom"></div>');
        $('#DataCom').append('<div id="DataCompared"> <div class="table-responsive" id="Dialog_DataCompare"><table class="table table-striped custom-table dark-head dark-head2 table-condensed" id="DBCompareDataTable"><thead><tr><th>S.No</th><th>Table Name</th><th>Different Records</th><th>Only in Source</th><th>Only in Target</th><th>Identical Records</th><th>Comparing With</th></tr></thead><tbody></tbody></table></div></div>');

        var CompareType; var BOname; var ParentIDs;
        $('.ui-dialog-title').html('Data Compare');
        $('#DBCompareDataTable').DataTable({
            initComplete: function () {
                var input = $('.dataTables_filter input').unbind(),
                    self = this.api(),
                    $searchButton = $('<button>')
                        .text('search')
                        .click(function () {
                            self.search(input.val()).draw();
                        }),
                    $clearButton = $('<button>')
                        .text('clear')
                        .click(function () {
                            input.val('');
                            $searchButton.click();
                        })
                $('.dataTables_filter').append($searchButton, $clearButton);
            },

            "destroy": true,
            "searching": true,
            //"bJQueryUI": true,
            "bProcessing": true,
            "bServerSide": true,
            //"dom": '<"top"i>rt<"bottom"flp><"clear">',
            "pageLength": 10,
            "lengthMenu": [10, 20, 50, 75, 100, 150, 200],
            "language": {
                searchPlaceholder: "Search....."
            },
            "sAjaxSource": DBDataCompare,
            "sServerMethod": "POST",
            "fnServerParams": function (aoData) {

                //aoData.push({ "name": "oNVParams", "value": FormValues },{ "name": "BONames", "value": sBOName }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID }, { "name": "ComparedType", "value": ComparingType  })
                //aoData.push({ "name": "sSourceDB", "value": $('input[name="8bce138c-b9bc-43fd-b2b6-ff21f6188a5a"]').val() }, { "name": "sTargetDB", "value": $('input[name="5477126a-fddd-4fa1-8d9a-9890cde2528c"]').val() }, { "name": "sSourceEnvID", "value": $('input[name="5307229f-d0af-44b5-8227-5a8d8f672e75"]').val() }, { "name": "sTargetEnvID", "value": $('input[name="cf1bb769-3d94-4851-b703-65a43453046a"]').val() }, { "name": "sApplicationID", "value": $('input[name="75a95f03-b045-4e56-98bd-454c2f93baaa"]').val() }, { "name": "BONames", "value": sBOName }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID }, { "name": "ComparedType", "value": ComparingType });
                aoData.push({ "name": "sSourceDB", "value": $('input[name="FKiSourceDB"]').val() }, { "name": "sTargetDB", "value": $('input[name="FKiTargetDB"]').val() }, { "name": "sSourceEnvID", "value": $('input[name="FKiSourceEnvID"]').val() }, { "name": "sTargetEnvID", "value": $('input[name="FKiTargetEnvID"]').val() }, { "name": "sApplicationID", "value": $('input[name="FKiApplicationID"]').val() }, { "name": "BONames", "value": bolist }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID }, { "name": "QueryType", "value": selectedValue }, { "name": "sExcludeLargeTables", "value": Checkbox }, { "name": "sTagText", "value": valutag });
            },
            "aoColumns": [
                {
                    "sName": "Sno", "Sno": false
                },

                {
                    "sName": "sTableName", "bSortable": true
                },
                {
                    "sName": "iDiffCount"
                },
                {
                    "sName": "iSourceCount"
                },
                {
                    "sName": "iTargetCount"
                },
                {
                    "sName": "iEqualCount"
                },
                {
                    "sName": "sPrimaryKey", "bSortable": false
                },

            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                $('td:eq(2)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen' onclick=fnDifferentRecords('" + aData[1] + "','" + aData[2] + "','" + aData[6] + "','" + iInstanceID + "');>" + aData[2] + "</a></div>");
                $('td:eq(3)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen' onclick=fnSourceTargetRecords('" + aData[1] + "','" + 'Source' + "','" + aData[3] + "','" + aData[6] + "','" + iInstanceID + "');>" + aData[3] + "</a></div>");
                $('td:eq(4)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen' onclick=fnSourceTargetRecords('" + aData[1] + "','" + 'Target' + "','" + aData[4] + "','" + aData[6] + "','" + iInstanceID + "');>" + aData[4] + "</a></div>");
            }

        });
        $('#DBSchemaComparison').append('<input type="button" value="Script Generation for Target" class="btn btn-primary" onclick="fnSourceToTargetUpdate(' + iInstanceID + ',false)"/>');
        $('#DBSchemaComparison').append('<input type="text" placeholder="Enter File Path" id="FilePath" /><input type="button" value="Save & Script Generation for Insert & Update" class="btn btn-primary" onclick="fnSourceToTargetUpdate(' + 0 + ',true)"/>');
    }

    else if (DBType == 'Structure') {
        // 
        //var sourceDB = $('select[name="FKiSourceDB"] option[disabled]:selected').val();
        $('#DBSchemaComparison').html('');
        $('#DBSchemaComparison').append('<div id="BOStructre"></div><div id="div_BostructureData"><div class="col-md-4"><select name="BOs" id="id_BOs" class="form-control"><option value="">select</option></select></div><div class="col-md-4"><select name="StructureName" id="id_BOStructureDetails" class="form-control"><option value="">select</option> </select></div><div class="col-md-4" id="BosStructredData"><span><input type="text" id="StructureBOID" placeholder="Enter InstanceID" style="color:black" /></span><div id="BOTreeStructure"></div></div></div>');
        $.ajax({
            type: 'POST',
            url: TableSelectionCompare,
            data: { oNVParams: FormValues, AllBos: true },
            dataType: 'json',
            cache: false,
            async: false,
            success: function (data) {

                $('#BOTreeStructure').css('display', '');
                var html = '<option value="">select</option>';
                if (data && data.length > 0)
                    $.each(data, function (i, item) {
                        html += '<option value="' + item.Attributes.boid.sValue + '">' + item.Attributes.name.sValue + '</option>';
                    });
                $('#id_BOs').empty();
                $('#id_BOs').html(html);

                $.unblockUI();

            }
        });
    }
    //else if (DBType == "DStructure1") {
    //    if ($('#StructureBOID').val() != null) {
    //        var ParentIDs = "";
    //        var idList = [];
    //        // var jsonNodes = $('#DatabaseTreeStructure').jstree(true).get_json('#', { flat: true });
    //        if (jsonNodes != undefined) {
    //            $.each(jsonNodes, function (i, val) {
    //                idList.push($(val).attr('id'));
    //            })
    //            if (idList.length > 0) {
    //                $.each(idList, function (i, val) {

    //                    ParentIDs += idList[i] + ",";
    //                })
    //                ParentIDs = ParentIDs.slice(0, -1);
    //            }
    //        }
    //        var tempvalue = {
    //            sSourceDB: $('input[name="FKiSourceDB"]').val(),
    //            sTargetDB: $('input[name="FKiTargetDB"]').val(),
    //            sSourceEnvID: $('input[name="FKiSourceEnvID"]').val(),
    //            sTargetEnvID: $('input[name="FKiTargetEnvID"]').val(),
    //            sApplicationID: $('input[name="FKiApplicationID"]').val(),
    //            ParentID: ParentIDs,
    //            iInstanceID: $('#StructureBOID').val(),
    //            StructureName: $('select[name="StructureName"] option:selected').text(),
    //            Boid: $('#id_BOs').val()
    //        }
    //        $.ajax({
    //            type: 'POST',
    //            url: DBDataCompare,//'/DataBase/NewDBDataCompare' ,//'@Url.Action("DBDataCompare", "DataBase")',
    //            data: tempvalue,
    //            cache: false,
    //            async: false,
    //            dataType: 'json',
    //            success: function (data) {
    //                if (data.length > 0) {
    //                    $.each(data, function (i, val) {
    //                        //alert(data[i][3]);
    //                        if (parseInt(val[2]) > 0 || parseInt(val[5]) > 0 || parseInt(val[6]) > 0)
    //                            fncUpdateDataXITree(val[3], 1);
    //                        else
    //                            fncUpdateDataXITree(val[3], 0);
    //                    })
    //                }
    //                //else
    //                //alert("Record not found");
    //                if (data.aaData.length > 0) {
    //                    $.each(data.aaData, function (i, val) {
    //                        //alert(data[i][3]);
    //                        if (parseInt(val[2]) > 0 || parseInt(val[3]) > 0 || parseInt(val[4]) > 0)
    //                            fncUpdateDataXITree(val[7], 1);
    //                        else
    //                            fncUpdateDataXITree(val[7], 0);
    //                    })
    //                }
    //            }
    //        });
    //    }
    //else {
    //    alert('Please Enter InstanceID');
    //}
    // }
    else {
        //  alert('Please Enter Source and Destination');
    }
    $.unblockUI();

}


function DBNonzeroDataCompare($this, DBType, iInstanceID, sBOName, jsonNodes) {
    $.blockUI({
        //'<h4>Please Wait...</h4>'
        message: '<img src="' + sImagePath + '/loading.gif" style="width: 30px;" /> Please wait...',
        blockMsgClass: 'report-success',
    });
    var formid = $($this).closest('form').attr('id');
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var FormValues = [];
    for (var i = 0; i < formData.length; i++) {
        FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });
    }
    if (DBType == 'Data') {
        oTable = $('#DBCompareDataTable').DataTable();
        var StructureName = $('select[name="StructureName"] option:selected').text();
        var iBOID = $('#id_BOs').val();
        $('#DBSchemaComparison').html('');
        $('#DBSchemaComparison').append('<div id="DataCom"></div>');
        $('#DataCom').append('<div id="DataCompared"> <div class="table-responsive" id="Dialog_DataCompare"><table class="table table-striped custom-table dark-head dark-head2 table-condensed" id="DBCompareDataTable"><thead><tr><th>S.No</th><th>Table Name</th><th>Different Records</th><th>Only in Source</th><th>Only in Target</th><th>Identical Records</th><th>Comparing With</th></tr></thead><tbody></tbody></table></div></div>');
        var CompareType; var BOname; var ParentIDs;
        $('.ui-dialog-title').html('Data Compare');
        $('#DBCompareDataTable').DataTable({
            'columnDefs': [{
                'targets': 0,
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<input type="checkbox" name="CheckBoxForNonZero" value="'
                        + $('<div/>').text(data).html() + '" id="' + full[1] + '" checked>';
                }
            }],
            'order': [1, 'asc'],
            initComplete: function () {
                var input = $('.dataTables_filter input').unbind(),
                    self = this.api(),
                    $searchButton = $('<button>')
                        .text('search')
                        .click(function () {
                            self.search(input.val()).draw();
                        }),
                    $clearButton = $('<button>')
                        .text('clear')
                        .click(function () {
                            input.val('');
                            $searchButton.click();
                        })
                $('.dataTables_filter').append($searchButton, $clearButton);
            },
            "destroy": true,
            "searching": true,
            "bProcessing": true,
            "bServerSide": true,
            "pageLength": 10,
            "lengthMenu": [10, 20, 50, 75, 100, 150, 200],
            "language": {
                searchPlaceholder: "Search....."
            },
            "sAjaxSource": DBNonZeroDataCompare,
            "sServerMethod": "POST",
            "fnServerParams": function (aoData) {
                // aoData.push({ "name": "sSourceDB", "value": $('input[name="FKiSourceDB"]').val() }, { "name": "sTargetDB", "value": $('input[name="FKiTargetDB"]').val() }, { "name": "sSourceEnvID", "value": $('input[name="FKiSourceEnvID"]').val() }, { "name": "sTargetEnvID", "value": $('input[name="FKiTargetEnvID"]').val() }, { "name": "sApplicationID", "value": $('input[name="FKiApplicationID"]').val() }, { "name": "BONames", "value": sBOName }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID });
                // aoData.push({ "name": "sSourceDB", "value": $('input[name="8bce138c-b9bc-43fd-b2b6-ff21f6188a5a"]').val() }, { "name": "sTargetDB", "value": $('input[name="5477126a-fddd-4fa1-8d9a-9890cde2528c"]').val() }, { "name": "sSourceEnvID", "value": $('input[name="5307229f-d0af-44b5-8227-5a8d8f672e75"]').val() }, { "name": "sTargetEnvID", "value": $('input[name="cf1bb769-3d94-4851-b703-65a43453046a"]').val() }, { "name": "sApplicationID", "value": $('input[name="75a95f03-b045-4e56-98bd-454c2f93baaa"]').val() }, { "name": "BONames", "value": sBOName }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID });
                aoData.push({ "name": "sSourceDB", "value": $('input[name="FKiSourceDB"]').val() }, { "name": "sTargetDB", "value": $('input[name="FKiTargetDB"]').val() }, { "name": "sSourceEnvID", "value": $('input[name="FKiSourceEnvID"]').val() }, { "name": "sTargetEnvID", "value": $('input[name="FKiTargetEnvID"]').val() }, { "name": "sApplicationID", "value": $('input[name="FKiApplicationID"]').val() }, { "name": "BONames", "value": sBOName }, { "name": "iInstanceID", "value": iInstanceID }, { "name": "StructureName", "value": StructureName }, { "name": "ParentID", "value": jsonNodes }, { "name": "Boid", "value": iBOID });
            },
            "aoColumns": [
                {
                    "sName": "Sno", "Sno": false
                },

                {
                    "sName": "sTableName", "bSortable": true
                },
                {
                    "sName": "iDiffCount"
                },
                {
                    "sName": "iSourceCount"
                },
                {
                    "sName": "iTargetCount"
                },
                {
                    "sName": "iEqualCount"
                },
                {
                    "sName": "sPrimaryKey", "bSortable": false
                },

            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                $('td:eq(2)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen';>" + aData[2] + "</a></div>");
                $('td:eq(3)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen';>" + aData[3] + "</a></div>");
                $('td:eq(4)', nRow).html("<div class=''><a class='btn btn-sm bg-green-seagreen';>" + aData[4] + "</a></div>");
            }

        });
        $('#DBSchemaComparison').append('<input type="text" id="nonzeroFilePath" placeholder="Enter File Path"/><input type="button" value="Insertion Script Generation for Target" class="btn btn-primary" onclick="fnNonZeroSourceToTargetUpdate(' + iInstanceID + ',true)"/>');
    }

    else {
        //  alert('Please Enter Source and Destination');
    }
    $.unblockUI();

}

function GetallData($this, sType) {

    //var Test = data;
    var ParentIDs = "";
    var idList = [];
    var jsonNodes = $('#DatabaseTreeStructure').jstree(true).get_json('#', { flat: true });
    $.each(jsonNodes, function (i, val) {
        idList.push($(val).attr('id'));
    })
    if (idList.length > 0) {
        $.each(idList, function (i, val) {

            ParentIDs += idList[i] + ",";
        })
        ParentIDs = ParentIDs.slice(0, -1);
    }
    //Compare('Data', null, ParentIDs);
    DBSchemaCompare($this, sType, $('#StructureBOID').val(), '', idList, $('input[name=ComparingType]:checked').val());
}



function fnSourceTargetRecords(sTableName, from, Count, sPrimaryKey, iInstanceID) {

    $('#div_AppendButton').empty();
    $('#id_heading').empty();
    //$('#div_Table').css('display', 'none');
    if (Count > 0) {
        $('#id_heading').text('Only in ' + from + ' (' + Count + ')');
        //$('#div_Table').css('display', '');
        //$('#div_Table').dialog('open');

        var dialogDiv = $('#div_Table');
        $("#div_Table").dialog("open");

        if (dialogDiv.length == 0) {
            dialogDiv = $("<div id='div_Table'></div>").appendTo('body');

            dialogDiv.dialog({
                modal: true,
                buttons: [
                    {
                        text: "Process",
                        class: 'large',
                        click: function () {
                            //              
                        }
                    },
                    {
                        text: "Cancel",
                        class: 'large',
                        click: function () {
                            $(this).dialog('close');
                        }
                    }]
            });
        } else {
            dialogDiv.dialog("open");
        }
        //$('#tBody').html(HtmlBody);
        $('#div_AppendButton').html("<input type='button' onclick=fnUpdateRecords('" + from + "','" + sTableName + "','Data','" + iInstanceID + "',false) value='Generate Script' class='btn btn-danger' />");
        $('#div_AppendButton').append("<input type='text' value='' id='UpdateDataFilePath' class='col-md-2'/> <input type='button' onclick=fnUpdateRecords('" + from + "','" + sTableName + "','Data','" + iInstanceID + "',true) value='Save & Generate Script' class='btn btn-danger'/>");
        $('#div_AppendButton').append("<input type='button' onclick=fncShowData('" + from + "','" + sTableName + "','Data','" + iInstanceID + "','" + sPrimaryKey + "') value='Show Data' class='btn btn-danger' />");
    }
}
function fncShowData(sActionType, sTableName, sDBType, iInstanceID, sPrimaryKey) {
    var model = {
        sTableName: sTableName,
        sActionType: sActionType,
        iInstancID: iInstanceID
    }
    GetTableRecords(model).then(function (TableRecords) {
        if (TableRecords && (TableRecords.Source || TableRecords.Target)) {

            var ValueName = "";
            //var fncName = "fnUpdateRecords(" + from +"," + sTableName + ",'Data','"+iInstanceID+"')";
            //var fncName = "fnUpdateRecords('" + from + "','" + sTableName + ",'Data',"' + iInstanceID + ");
            var ClassName = "cls_" + sActionType + "Records";
            var id = sActionType;
            if (sActionType == 'Source') {
                SourceTarget = TableRecords.Source;
                ValueName = "Update Target";
                $('.ui-dialog-title').html('Update to Target');
            }
            else {
                SourceTarget = TableRecords.Target;
                ValueName = "Update Source";
                $('.ui-dialog-title').html('Delete Records in Target');
            }

            fnAppendHeadings(SourceTarget[0], sActionType);
            var HtmlBody = '';
            $.each(SourceTarget, function (i, sRecord) {
                HtmlBody += "<tr><td><input type='checkbox' id='" + i + "' checked class='" + ClassName + "' /></td>";
                $.each(sRecord, function (j) {
                    var sGUID = "";
                    if (sRecord[j].sName.toLowerCase() == sPrimaryKey.toLowerCase())
                        sGUID = 'id=' + id + '_' + i + '';
                    HtmlBody += '<td ' + sGUID + '>' + sRecord[j].sValue + '</td>';
                });
                HtmlBody += '</tr>';
            });
            $('#tBody').html(HtmlBody);
        }
    });
}

/****************************************************************** Generate Table Headings ******************************************************************************************/
function fnAppendHeadings(Record, from) {
    if (Record && Record.length > 0) {
        var Headers = [];
        $.each(Record, function (i, item) {
            Headers.push(item.sName);
        })
        var HtmlHeader = '<tr><th>Update</th>';
        $.each(Headers, function (i, Header) {
            var th = '<th>' + Header + '</th>';
            if ((from == 'Source' || from == 'Target'))
                HtmlHeader += th;
            else
                HtmlHeader += th + th;
        });
        HtmlHeader += '</tr>';
        $('#tHead').html(HtmlHeader);
    }
}

/******************************************************************Toggle the Different Reocrd Checkboxes******************************************************************************************/
function fnDiffChange(from, i) {
    if (from == 'Source')
        if ($('#id_Source_' + i).is(":checked"))
            $('#id_Target_' + i).attr('checked', false);
    if (from == 'Target')
        if ($('#id_Target_' + i).is(":checked"))
            $('#id_Source_' + i).attr('checked', false);
}

/*****************************************************************Update Target By Default ****************************************************************************************/
function fnSourceToTargetUpdate(iID, bSavingType, selectedValue) {
    var selectedValue = $('#QueryType').val();
    var FilePath = null;
    if (bSavingType == true) {
        FilePath = $('#FilePath').val();
    }
    $.ajax({
        type: 'GET',
        url: TargetInsertUpdate,//'/DataBase/InsertUpdateTarget', //'@Url.Action("InsertUpdateTarget", "DataBase")',
        cache: false,
        data: { iInstanceID: iID, bSavingType: bSavingType, FilePath: FilePath, QueryType: selectedValue },
        async: false,
        dataType: 'HTML',
        success: function (html) {
            if (FilePath != null) {
                alert("Files Saved in " + FilePath);
            }
            else {
                $('#Dialog_Script').empty();
                $('#Dialog_Script').html(html);
                $('#Dialog_Script').dialog('open');
            }
            //Popup();
        }
    })
}

function fnNonZeroSourceToTargetUpdate(iID, bSavingType) {
    var TableNames = "";
    $("input[type='checkbox'][name='CheckBoxForNonZero']:checked").each(function () {
        TableNames += $(this).attr('id') + ",";
    });
    TableNames = TableNames.substring(0, TableNames.length - 1);
    var FilePath = $('#nonzeroFilePath').val();
    $.ajax({
        type: 'GET',
        url: NonZeroTargetInsertUpdate,
        cache: false,
        data: { iInstanceID: iID, bSavingType: bSavingType, FilePath: FilePath, ExceptTables: TableNames },
        async: false,
        dataType: 'HTML',
        success: function (html) {
            if (FilePath != null && FilePath != "") {
                alert("Files Saved in " + FilePath);
            }

        }
    })
}

/*****************************************************************Update Source To Target and Target To Source based on sActionType ****************************************************************************************/
function fnUpdateRecords(sActionType, sTableName, sDBType, iID, bSavingType, selectedValue) {

    var SourceChecked = $('.cls_SourceRecords:checked');
    var TargetChecked = $('.cls_TargetRecords:checked');
    var sFilePath = $('#UpdateDataFilePath').val();
    var selectedValue = $('#QueryType').val();
    var GUIDs = [];
    if (SourceChecked && SourceChecked.length > 0)
        $.each(SourceChecked, function (i, item) {
            var id = $(item).attr('id');
            if (id && parseInt(id) >= 0) {
                var sGUID = $('#Source_' + parseInt(id)).text();
                if (sGUID)
                    GUIDs.push(sGUID);
            }
        });
    if (!GUIDs || GUIDs.length == 0)
        GUIDs = null;
    if (GUIDs) {
        $('#Dialog_Script').empty();
        var model = {
            sTableName: sTableName,
            GUIDs: GUIDs,
            sActionType: sActionType,
            sDBType: sDBType,
            iInstanceID: iID,
            bSavingType: bSavingType,
            QueryType: selectedValue,
            FilePath: $('#UpdateDataFilePath').val()
        }
        $.ajax({
            type: 'POST',
            url: DataBaseUpdateTarget,
            cache: false,
            async: false,
            data: model,
            dataType: 'html',
            success: function (html) {
                if (sFilePath != null && sFilePath != "") {
                    alert("Files Saved in " + sFilePath);
                }
                else {
                    $('#Dialog_Script').html(html);
                    $('#Dialog_Script').dialog('open');
                }
            }
        })
    }
    else
        alert("Please click on Show Data button before saving.");
};

/******************************  Display  Different Records as Table *************************************/
function fnDifferentRecords(sTableName, Count, sPrimaryKey, iID) {
    $('#div_AppendButton').empty();
    $('#id_heading').empty();
    $('#div_Table').css('display', 'none');
    if (Count > 0) {
        $('#div_Table').css('display', '');
        $('#id_heading').text('Different Records (' + Count + ')');
        //$('#div_Table').css('display', '');
        $('#div_Table').dialog('open');
        $('.ui-dialog-title').html('Compare Different Records');
        var finalArray = [];
        //calling Server method for getting Table Different records
        var model = {
            sTableName: sTableName,
            sActionType: 'Difference',
            sDBType: 'Data',
            iInstanceID: iID
        };
        finalArray.push(model);
        var rec = JSON.stringify(finalArray);
        GetTableRecords(model).then(function (TableRecords) {
            if (TableRecords && TableRecords.Source && TableRecords.Target) {
                var Source = TableRecords.Source;
                var Target = TableRecords.Target;
                fnAppendHeadings(Source[0]);
                var HtmlBody = '';// var style = ''; var sGUID = ''; var tGUID = '';
                $.each(Source, function (n, sRecord) {
                    //for (var n = 0; n <= Source.length-1; n++) {
                    var XIGUID = Source[n];
                    $.each(Target, function (r, tRecord) {

                        //for (var r = 0; r <= Target.length-1; r++) {
                        var TXIGUID = Target[r];
                        if (XIGUID.find(o => o["sName"] == 'XIGUID').sValue == TXIGUID.find(o => o["sName"] == 'XIGUID').sValue) {
                            HtmlBody += "<tr><td><input type='checkbox' checked id='" + n + "' class='cls_SourceRecords' onchange=fnDiffChange('Source'," + n + ") /><span><i class='fa fa-exchange'></i></span><input type='checkbox' id='id_Target_" + n + "' onchange=fnDiffChange('Target'," + n + ") class='cls_TargetRecords' /></td>";

                            $.each(XIGUID, function (j) {
                                var style = '';
                                var sGUID = '';
                                var tGUID = '';
                                if (XIGUID[j].sValue != TXIGUID[j].sValue) {
                                    style = 'style="color:red"';
                                }
                                if (XIGUID[j].sName.toLowerCase() == sPrimaryKey.toLowerCase()) {
                                    sGUID = 'id=Source_' + n + '';
                                    tGUID = 'id=Target_' + n + '';
                                }
                                HtmlBody += '<td ' + sGUID + ' ' + style + '>' + XIGUID[j].sValue + '</td><td ' + tGUID + '' + style + '>' + TXIGUID[j].sValue + '</td>';
                            });
                            HtmlBody += '</tr>';
                        }


                        // }
                    });
                    // HtmlBody += '</tr>';
                    // }
                });
                //$.each(Source, function (i, sRecord) {
                //    HtmlBody += "<tr><td><input type='checkbox' checked id='" + i + "' class='cls_SourceRecords' onchange=fnDiffChange('Source'," + i + ") /><span><i class='fa fa-exchange'></i></span><input type='checkbox' id='id_Target_" + i + "' onchange=fnDiffChange('Target'," + i + ") class='cls_TargetRecords' /></td>";
                //    var tRecord = Target[i];
                //    $.each(sRecord, function (j) {
                //        var style = '';
                //        var sGUID = '';
                //        var tGUID = '';
                //        if (Source[i].sName.toLowerCase() == tRecord[j].sName.toLowerCase() == "xiguid") {
                //            //if (sRecord[i].sValue != tRecord[j].sValue) {
                //                style = 'style="color:red"';
                //            //}
                //        }
                //        if (sRecord[j].sValue != tRecord[j].sValue) {
                //            style = 'style="color:red"';
                //        }
                //        if (sRecord[j].sName.toLowerCase() == sPrimaryKey.toLowerCase()) {
                //            sGUID = 'id=Source_' + i + '';
                //            tGUID = 'id=Target_' + i + '';
                //        }
                //        HtmlBody += '<td ' + sGUID + ' ' + style + '>' + sRecord[j].sValue + '</td><td ' + tGUID + '' + style + '>' + tRecord[j].sValue + '</td>';
                //    });
                //    HtmlBody += '</tr>';
                //});
                $('#tBody').html(HtmlBody);
                $('#div_AppendButton').html('<input type="button" value="Generate Script" onclick=fnUpdateRecords("Difference","' + sTableName + '","Data","' + iID + '",false) class="btn btn-danger"/>')
                $('#div_AppendButton').append('<input type="text" id="UpdateDataFilePath" placeholder="Schema File Path" /><input type="button" value="Save & Generate Script" onclick=fnUpdateRecords("Difference","' + sTableName + '","Data","' + iID + '",true) class="btn btn-danger"/>')

            }

        });
    }
};
function GetTableRecords(model) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: GetTableRecord,//'/DataBase/GetTableRecords',//"@Url.Action("GetTableRecords", "DataBase")",
            data: model,
            dataType: 'json',
            success: function (data) {
                resolve(data);
            }
        });
    });
}

function fnc1clickDelete(ID, FKialgorithmID, BOID, _this, sGUID) {
    $(_this).attr("disabled", true);
    //var QuoteID = $('#' + sID + "_QuoteID").html();
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', _this);
    $.ajax({
        async: false,
        type: 'POST',
        url: Execute1ClickDeleteAlgURL,
        data: JSON.stringify({ sGUID: sGUID, FKialgorithmID: FKialgorithmID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            var RefreshBtn = $('.hideref-' + BOID);
            if (RefreshBtn && RefreshBtn.length > 0) {
                $(RefreshBtn[0]).click();
            }
        }
    });
}
function fncRunProcessController(_this, ProcessControllerIDs, sGUID, sID) {
    //debugger
    $(_this).attr("disabled", true);
    if (sGUID == null) {
        sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', _this);
    }
    var QuoteID = $('#' + sID + "_QuoteID").html();
    $.ajax({
        async: false,
        type: 'POST',
        url: ExecuteProcessControllerURL,
        data: JSON.stringify({ sGUID: sGUID, sProcessControllerIDs: ProcessControllerIDs, QuoteID: QuoteID }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        success: function (data) {
            $(_this).parent().prepend('<div>' + data + '</div>');
            // debugger
        }
    });
}

function TempDB($this, BOName, sType) {
    $.blockUI({
        //'<h4>Please Wait...</h4>'
        message: '<img src="' + sImagePath + '/loading.gif" style="width: 30px;" /> Please wait...',
        blockMsgClass: 'report-success',
    });
    var formid = $($this).closest('form').attr('id');
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var FormValues = [];
    for (var i = 0; i < formData.length; i++) {
        //FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
        FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });

    }
    TestConnection($this, BOName);
    if (sType == "TempDB") {
        $.ajax({
            url: TempDBCreation,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            datatype: "JSON",
            async: true,
            cache: false,
            data: JSON.stringify({ oNVParam: FormValues }),
            //data: JSON.stringify({ plainText: SourceFile, passPhrase: stringToWriteToPdf }),
            success: function (data) {
                alert(data);
                $.unblockUI();
            }
        });
    }
}
function TestConnection($this, BOName) {
    var formid = $($this).closest('form').attr('id');
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var FormValues = [];
    for (var i = 0; i < formData.length; i++) {
        //FormValues.push({ sName: formData[i].name, sValue: formData[i].value, bDirty: true });
        FormValues.push({ sName: formData[i].name, sValue: formData[i].value, sPreviousValue: formData[i].sPreviousValue, bDirty: true, bIsValid: formData[i].bIsValid });

    }
    $.ajax({
        url: TestConnectionString,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "JSON",
        async: true,
        cache: false,
        data: JSON.stringify({ oNVParam: FormValues }),
        success: function (data) {
            if (data == "Success") {
                alert("Success!! Connection string is valid, Database Exists.", true);
            }
            else {
                alert("Failure!! Connection error, Please check the connection string", false);
            }
        }
    });
}

function fncQsSaveAndMailQSLink(ReportID, GroupID, BOID, CreateXILinkID, InstanceID, $This) {
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $This);
    //var FParent = $($This).parent();
    //var SParent = $(FParent).parent();
    //var ComponentDiv = $(SParent).parent().find('.oneclickresponse');
    //var DatatableID = $(ComponentDiv).find('.dataTable').attr('id');
    //var Datatable = $('#' + DatatableID).dataTable();
    var sNewGUID = CreateGuid();
    var sAttachement = "";
    var Instances = [];
    for (var i = 0; i < InputParams.length; i++) {
        if (InputParams[i].sName == "trundefined_FKiACTransactionID") {
            Instances.push(InputParams[i].sValue);
        }
    }
    sAttachement = Instances.join(',')
    var bIsLoading = $($This).attr("data-IsLoading");

    var oQSInstance = QSInsDict[sGUID];

    sValidateAttr = [];
    fncMapAndValidate(sGUID, 'Save').then(function (state, callback) {

        if (sValidateAttr == undefined || sValidateAttr.length == 0) {
            var sType = $($This).text();
            if (sType == "Continue") {
                var sFormiID = fncGetFormIIDFromHTMLTree($This);
                //var sForm = $($This).parent().parent().find('form')[0];
                var sInstanceID = 0; var iCount = 0;
                if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                    $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                }
                fnc1clickcreate(ReportID, GroupID, BOID, CreateXILinkID, null, $This);
            }
            else {
                if (bIsLoading != "" && bIsLoading != undefined && bIsLoading.toLowerCase() == "yes") {
                    $($This).html('<img src="' + sImagePath + '/loading.gif" style="width: 20px;" /> Please wait');
                }
                fncSetParam(oQSInstance.XIGUID, sNewGUID, "{XIP|iQSInstanceID}", "").then(function (state, callback) {
                    fncSetParam(sAttachement, sNewGUID, "{XIP|sAttachementID}", "").then(function (state, callback) {
                        XIRun(null, CreateXILinkID, null, sNewGUID, null, false, null, 121, null)
                    });
                })
            }
        }
    });

}

function NotificationBells(XiLinkID) {
    if (XiLinkID > 0) {
        XIRun(null, XiLinkID, 0, null, null, true);
        //XIRun($this, XiLinkID, ID, sGUID, BO, IsMerge, BODID, iQSDID, MenuName, oParams, bISActivity)
    }
}
function fncChangeTemplate($this, sDepBOField) {
    var formid = $($this).find('.CreateForm').attr('id');
    if (formid == undefined) {
        formid = $($this).closest('form').attr('id');
    }
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var AttributeID = $($this).attr('data-attrname');
    var iInstaceID;
    var sCode;
    for (var i = 0; i < formData.length; i++) {
        if (formData[i].name == "id") {
            iInstaceID = formData[i].value;
        }
        else if (formData[i].name == AttributeID) {
            sCode = formData[i].value;
        }
    }
    var YesBtnText = "Yes";
    var NoBtnText = "No";
    var NewSGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    var oQSInstance = QSInsDict[NewSGUID];
    var iQSIID = oQSInstance.ID;
    $("#BoActionDlg").html("Do you want to override the subject and body..");
    $("#BoActionDlg").dialog({
        title: "Confirmation Dialog",
        autoOpen: true,
        modal: true,
        width: 400,
        left: '600px',
        top: '80px',
        buttons: [
            {
                text: YesBtnText,
                click: function () {
                    $.ajax({
                        url: ExecuteFollowUpAlgURl,
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        datatype: "json",
                        cache: false,
                        async: false,
                        data: JSON.stringify({ iInstaceID: iInstaceID, sCode: sCode, iQSIID: iQSIID, sGUID: NewSGUID }),
                        success: function (sdata) {
                            var DepBO = sDepBOField.split(',');
                            if (sdata != null && sdata.length > 0 && DepBO != null && DepBO.length > 0) {
                                for (var k = 0; k < sdata.length; k++) {
                                    for (var i = 0; i < DepBO.length; i++) {
                                        if (sdata[k].sName == DepBO[i]) {
                                            var Attribure = $("[name=" + DepBO[i] + "]");
                                            var ElementType = "";
                                            var Element = $(Attribure).get(0);
                                            if (Element != undefined) {
                                                ElementType = $(Attribure).get(0).tagName;
                                            }
                                            $(Attribure).empty();
                                            if (ElementType.toLowerCase() == "input" || ElementType.toLowerCase() == "textarea") {
                                                $(Attribure).val(sdata[k].sValue);
                                                $(Attribure).val(sdata[k].sValue);
                                            }
                                        }
                                    }
                                }
                            }
                            $('#BoActionDlg').dialog('close');
                            $('#BoActionDlg').dialog('destroy');
                        },
                        error: function (data) {
                            $('#BoActionDlg').dialog('close');
                            $('#BoActionDlg').dialog('destroy');
                        }
                    })
                }
            },
            {
                text: NoBtnText,
                click: function () {
                    $('#BoActionDlg').dialog('close');
                    $('#BoActionDlg').dialog('destroy');
                }
            }
        ]
    });
}
function fncPriceCalculation($this, sDepBOField) {
    var formid = $($this).closest('form').attr('id');
    var formData = JSON.parse(JSON.stringify(jQuery('#' + formid).serializeArray()));
    formData = processFormData(formData, formid);
    var IsIPT;
    var SalePrice;
    var CostPrice;
    var SalePriceExIPT;
    var rIPT;
    var Margin;
    $.each(formData, function (i, v) {
        if (v.name.toLowerCase() == "bIsIPT".toLowerCase()) {
            IsIPT = v.value;
        }
        else if (v.name.toLowerCase() == "rSalePrice".toLowerCase()) {
            SalePrice = v.value;
        }
        else if (v.name.toLowerCase() == "rCostPrice".toLowerCase()) {
            CostPrice = v.value;
        }
        else if (v.name.toLowerCase() == "rIPT".toLowerCase()) {
            rIPT = v.value;
        }
    });
    if (IsIPT.toLowerCase() == "true") {
        SalePriceExIPT = (parseFloat(SalePrice) * 100) / (100 + parseFloat(rIPT));
    }
    else {
        SalePriceExIPT = parseFloat(SalePrice);
    }
    Margin = SalePriceExIPT.toFixed(2) - parseFloat(CostPrice);
    if (sDepBOField.indexOf(',') > 1) {
        data = sDepBOField.split(',')
        if (data != null && data.length > 0) {
            for (var k = 0; k < data.length; k++) {
                if (data[k] == "rMargin" && Margin < 0) {
                    $('#' + data[k]).val(Margin).css("background", "#F00 !important");
                }
                else if (data[k] == "rMargin" && Margin > 0) {
                    $('#' + data[k]).val(Margin);
                }
                else {
                    $('#' + data[k]).val(SalePriceExIPT.toFixed(2));
                }
            }
        }
    }
    else {
        if (Margin < 0) {
            $('#' + sDepBOField).val(Margin).css("background", "#F00 !important");
        }
        else {
            $('#' + sDepBOField).val(Margin)
        }
    }
}
function fncGetDependentFieldValues($this, i1ClickID, sField) {
    var formid = $($this).closest('form').attr('id');
    var NewSGUID = formid.split('_')[1];
    var iInstanceID = $('.fc-head').text().split(' ')[$('.fc-head').text().split(' ').length - 1];
    $('#DDL_' + sField + NewSGUID.replaceAll('-', '_') + '_chosen').remove();

    var obj = [];
    var oNVParams = new Object();
    oNVParams.sName = '{XIP|FKiInsurerID}';
    oNVParams.sValue = $this.value;
    obj.push(oNVParams);
    sGUID = fncGetGUIDFromHTMLTree('LayoutGUID', $this);
    $.ajax({
        type: 'POST',
        url: GetDependentFieldValuesURL,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ i1ClickID: i1ClickID, sGUID: sGUID, oNVParams: obj }),
        cache: false,
        async: true,
        dataType: 'json',
        success: function (datas) {
            var Html = '<option value="" class="ps">Please Select</option>'
            $.each(datas, function (index, value) {
                if (value != null) {
                    Html += '<option value="' + value.id.sValue + '">' + value.sname.sValue + '</option>'
                }
                else {
                    Html += '<option value="' + value.id.sValue + '">' + value.sname.sValue + '</option>'
                }
            })
            $('#DDL-' + sField + NewSGUID).html(Html);
            $('#DDL-' + sField + NewSGUID).show();
        },
        error: function (datas) {
        }
    })
}
function fncFAdjustmentCal($this, sDepBOFields, sFunctionScript) {

    //// CommissionOverride
    var Price = $('#rPrice').val();
    var CommissionOverride = $($this).val();
    var AdminFee = $('#rAdmin').val();
    $('#zCommission').val(CommissionOverride);
    var Commission = (Price * CommissionOverride) / 100;
    $('#rPaymentToInsurer').val(Price - (Commission + AdminFee));
    $('#zCommissionAdjustment').val(Commission - $('#rCommissionRebate').val());

    // commission rebate
    $('#rPrice').val($('#rPrice').val() + $('#rCommissionRebate').val());
    $('#rCommission').val($('#rCommission').val() - $('#rCommissionRebate').val());

    //////////////// Commission final
    $('#zCommissionAdjustment').val($('#rCommission').val() - $('#rCommissionRebate').val());

    ////rOverrideAdmin
    $('#rAdmin').val($('#rOverrideAdmin').val());
    $('#rPrice').val($('#rPrice').val() - $('#rOverrideAdmin').val());

}
$('.asidetoglbtn>a').on('click', function () {
    //$(this).find('i.fa').toggleClass('fa-times');
    $(this).toggleClass('active');
    $('body').toggleClass('sidebar-collapse');
    $('.one-layout-detail-bcl1menu,.one-layout-detail-sahomemenu,.one-layout-detail-mavericksmenudetailsnew').toggleClass('openSideMenu');
    $('.sidebar-collapse .sidebar-menu li.treeview>.menu-open').hide();
    $('.sidebar-collapse .sidebar-menu li.treeview').removeClass('active')
    //$('.NavbarWrapper').toggleClass('openZeeadminMenuRight');
    $(document).find('.header-search-toggle').removeClass('header-search-open');
});
$('.header-search-toggle').on('click', function () {
    $(this).toggleClass('header-search-open');
    $(document).find('.openSideMenu').removeClass('openSideMenu');
});
$(document).ready(function(){
    /* $(document).find('.sidebar-collapse').find('.sidebar-menu > .treeview.active').mouseover(function ()
        {
            debugger
        }
    );
    $(document).find('.sidebar-collapse').find('.sidebar-menu > .treeview.active').mouseout(function ()
        {
            debugger
            $(this).find('.menu-open').hide();
            //$(this).parent().removeClass('active');
        }   
    ).mouseout(); */
    $(document).on("mouseleave",".sidebar-collapse .sidebar-menu", function(){
        //$(this).addClass("feather")
        //debugger
        $('.sidebar-collapse .treeview.active').find('.menu-open').css({'display': 'none'});
        $('.sidebar-collapse .treeview.active').removeClass('active');
    }).trigger('.sidebar-collapse');
});

$(document).on('click', '#QSStep .left-nav-btns>li', function () {
    $(this).parent().addClass('tab_selected').siblings().removeClass('tab_selected');
});
// $(document).ready(function(){
//     if ($('.NavbarWrapper').find('.col-md-2').is(':empty')){
//         debugger
//         $('.col-md-2').addClass('abc');
//     }
// });
// function isEmpty( el ){
//     return !$.trim(el.html())
// }
// $(document).ready(function(){
//         if ($('.NavbarWrapper').find('.col-md-2').is(':empty')){
//             //$('.col-md-2').empty();
//             $('.col-md-2').addClass('abc');
//         }
// });
//if($.trim($(".col-md-2").html())=='');
//function fncAsyncQuotesSignalR(sLayGUID, iQSDID, sXiLink) {
//    var oQSInstance = QSInsDict[sLayGUID];
//    var disconnectType = "";
//    var chat = $.connection.notifyHub;
//    chat.client.addNewMessageToPage = function (model) {
//        if (model.QSInstanceID == oQSInstance.ID) {
//            sGUID = sLayGUID;
//            fncBindSignalrData(model);
//        }
//        if (model.ProductversionID == null || model.ProductversionID == "" || model.ProductversionID == "undefined") {
//            disconnectType = "explicitly closed";
//            $.connection.hub.stop();
//        }
//    };
//    $.connection.hub.start({ transport: ['longPolling', 'webSockets'] }, function () { console.log('connection started!'); }).done(function () {
//        var ConnectionID = $.connection.hub.id;
//        fncSetParam(ConnectionID, sLayGUID, "SignalRConnectionID", "", "");
//        //XIRun(5012, 0, sLayGUID, null, false, 0, iQSDID);
//        //XIRun(null, sXiLink, 0, sLayGUID, null, false, 0, iQSDID);
//        // Call the Send method on the hub.
//        //alert('Now connected, connection ID=' + $.connection.hub.id);
//        chat.server.send();
//        // Clear text box and reset focus for next comment.
//        //$('#message').val('').focus();
//        //});
//    });
//    $.connection.hub.connectionSlow(function () {
//        alert("We found an issue with your internet connection, Please check it might be slow");
//        // LOG HERE THAT CONNECTION IS SLOW WITH QSINSTANCEID
//        fncLogSignalrEvents("Connectionslow Event fired", oQSInstance.ID);
//        console.log("SignalR Connectionslow called");

//    });
//    $.connection.hub.reconnecting(function () {
//        // LOG HERE
//        fncLogSignalrEvents("reconnecting Event fired", oQSInstance.ID);
//        console.log("SignalR reconnecting called");
//    });
//    $.connection.hub.disconnected(function () {
//        // LOG HERE
//        if (disconnectType != "explicitly closed") {
//            fncLogSignalrEvents("disconnected Event fired", oQSInstance.ID);
//            console.log("SignalR disconnected called");
//            $('.MQuotesDiv').each(function () {
//                var type = $(this).attr("data-Type");
//                if (type == 'failed') {
//                    $(this).find('.load').html('NA');
//                    $(this).find(".loadBuy").hide();
//                    $(this).find('.MoreDetailsContent').hide();
//                    $(this).find('.riskBtn').hide();
//                }
//            });
//            if ($('.divBestprice').find('img').length) {
//                $(this).hide();
//            }
//            alert("We found some issue with your network, Please try to recalculate");
//            if ($.connection.hub.lastError) {
//                var errormessage = $.connection.hub.lastError.message;
//                fncLogSignalrEvents("disconnected Error Message: " + errormessage, oQSInstance.ID);
//                //alert("Disconnected. Reason: " +  $.connection.hub.lastError.message);
//            }
//        }
//        else {
//            fncUpdaeQuoteRank(oQSInstance.ID);
//        }
//        //setTimeout(function () {
//        //    $.connection.hub.start();
//        //    console.log("SignalR reconnect called after disconnecting");
//        //    fncLogSignalrEvents("Reconnecting after disconnected");
//        //}, 5000); // Restart connection after 5 seconds.
//    });
//}
function fncGetTablesplitting(sURL, i1ClickID, sGUID, sTabGuid, expcol, searchtext, optr, searchwhere) {
    $.ajax({
        type: 'POST',
        url: sURL,
        data: { i1ClickID: i1ClickID, sGUID: sGUID, SearchText: searchtext, Optr: optr, SearchWhere: searchwhere },
        cache: false,
        async: true,
        dataType: 'json',
        success: function (Data) {
            var rowvalues = "";
            $.each(Data.aaData, function (key, value) {
                rowvalues = rowvalues + '<div  class="popover-time">';
                rowvalues = rowvalues + '<table class="table popover-table">';
                rowvalues = rowvalues + '<thead><strong>' + key + '</strong></thead>';
                Header = '';
                $.each(Data.Headers, function (n, headervalue) {
                    if (n >= Data.NOofFields) {
                        Header = Header + '<th>' + headervalue + '</th>';
                    }
                })
                Header = '<tr>' + Header + '</tr>';
                var rowdata = Header + "<tr>";
                $.each(value, function (i, data) {
                    $.each(data, function (k, rowvalue) {
                        rowdata = rowdata + '<td>' + rowvalue + '</td>';
                    })
                    rowdata = rowdata + '</tr>';
                });
                rowvalues = rowvalues + rowdata + '</table></div>';
            })
            $('#OneClickResult1-' + sTabGuid).html(rowvalues);
        },
        error: function (err) {

        }
    })
}
function fnc1clickPopup($this, XilinkGuid, sGuid) {
    $.ajax({
        type: 'POST',
        url: XILinkLoadJsonURL,
        data: JSON.stringify({ iXILinkID: XilinkGuid, sGUID: sGuid }),
        contentType: 'application/json;',
        dataType: 'json',
        traditional: true,
        async: true,
        success: function (XIData) {
        }
    });
}
function fncListRefresh(sigRObj) {
    var BOName = sigRObj.BOName;
    var BOXIGUID = sigRObj.BODID;
    var BOIID = sigRObj.BOIID;
    var HTML = sigRObj.HTML;
    var NVs = sigRObj.oNVs;
    if (NVs && NVs.length > 0) {
        for (var l = 0; l < NVs.length; l++) {
            $('#' + BOName + "_" + BOXIGUID).find('tr.simpleclick_' + BOIID).find('td.one-1q-field-' + NVs[l].sName.toLowerCase()).html(NVs[l].sValue);
            $('#' + BOName + "_" + BOXIGUID).find('tr.simpleclick_' + BOIID).find('td.one-1q-field-\\[' + NVs[l].sName.toLowerCase() + '\\]').html(NVs[l].sValue);
            $('#' + BOName + "_" + BOXIGUID).find('tr.simpleclick_' + BOIID).find('td.one-1q-field-' + NVs[l].sName.toLowerCase()).css({ 'color': 'red' });
            $('#' + BOName + "_" + BOXIGUID).find('tr.simpleclick_' + BOIID).find('td.one-1q-field-\\[' + NVs[l].sName.toLowerCase() + '\\]').css({ 'color': 'red' });
        }
        $('#SignR-' + BOName + "-" + BOIID).html('<div class="StatusMessages"><div class="alert alert-danger">Data Updated</div></div>');
    }
    //$('#' + BOName + "_" + BOXIGUID).html('<div class="StatusMessages"><div class="alert alert-danger">' + HTML + '</div></div>');
}

function Snooze() {
    $('#SnoozeTimeperiod').show();
    $('#btnsnooze').hide();
}
function SnoozeSubmit(NotificationID) {
    var Time = $('#SnoozeTimeperiodList').val();
    $.ajax({

        url: SnoozeLimit,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        datatype: "JSON",
        async: true,
        cache: false,
        data: JSON.stringify({ NotificationID: NotificationID, SnoozeTime: Time }),

        success: function (oResponse) {
            debugger
        },
        error: function (err) {

        }
    });
}

function fncDynamicCss(sName) {
    $.ajax({
        url: CssUrl,
        //url: '@Url.Action("Customcss", "XiLink")',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        cache: false,
        async: true,
        data: JSON.stringify({ sName: sName }),
        success: function (data) {
            let style = document.createElement('style');
            style.type = 'text/css';
            document.getElementsByTagName('head')[0].appendChild(style);
            let sCss = "";

            //$.each(result, function (key, data) {
            sCss += data;

            //});

            style.innerHTML = sCss;

        }
    });
}

function fncAttrEditor(sIdentifier) {
    tinymce.remove();
    if (typeof (tinyMCE) != "undefined") {
        if (tinyMCE.activeEditor == null || tinyMCE.activeEditor.isHidden() != false) {
            tinyMCE.editors = []; // remove any existing references
        }
    }
    tinyMCE.editors = [];
    //tinymce.EditorManager.execCommand('mceRemoveEditor', true, '#' + sIdentifier);
    //tinymce.EditorManager.execCommand('mceAddEditor', true, '#' + sIdentifier);
    //$("#" + sIdentifier).tinymce().remove();
    //$("#UserResetDialog").dialog({
    //    width: 1005, height: 610, modal: true, autoOpen: false,
    //    open: function (event, ui) {

    //    }
    //})
    //$("#UserResetDialog").dialog("open");
    $('#' + sIdentifier).show();
    tinymce.remove('textarea#' + sIdentifier);
    tinymce.init({
        selector: 'textarea#' + sIdentifier,
        width: "1000",
        height: "200",
        menubar: false,
        statusbar: false,
        branding: false,
        //entity_encoding: 'raw',
        mode: "specific_textareas",
        editor_selector: "mceEditor",
        toolbar: 'formatselect | bold italic strikethrough forecolor backcolor permanentpen formatpainter | link image media pageembed | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent | removeformat | addcomment | mybutton mybutton1 mybutton2',
        setup: function (editor) {

        }
    });

}

function gitClone() {
debugger
        $.ajax({
type: 'GET',
url:CodeEditorURL,
contentType: "application/json; charset=utf-8",
        dataType: 'html',
            success: function (data) {
                $("#BCL1Dashboard").prepend(data);
            },
            error: function (er) {

            }
        });
    }
//Search filter btn
$(document).ready(function(){
    //$('.one-1q-filter-form').append('<span class="search_filter_btn"></span>')
    //$('.CreateForm').hide();
    $(document).on('click','.search_filter_btn', function(e){
        debugger            
        $(this).parent().parent('.one-1q-filter-form').children('.CreateForm').addClass('open');
        $(this).addClass('open');
        e.stopPropagation();
    });
    $(document).on('click','.search_filter_btn.open', function(e){            
        $(this).parent().parent('.one-1q-filter-form').children('.CreateForm').removeClass('open');
        $(this).removeClass('open');
        e.stopPropagation();
    });
    /* debugger
    $('.search_filter_btn').toggle(function () {
        //$("#user_button").addClass("active");
        $(this).parent().parent('.one-1q-filter-form').children('.CreateForm').addClass('open');
    }, function () {
        //$("#user_button").removeClass("active");
        $(this).parent().parent('.one-1q-filter-form').children('.CreateForm').removeClass('open');
    }); */
    $(document).on('click', '.report-item .card-option', function () {
        debugger
        $(this).parent().parent('.report-item').addClass('report-open').siblings().removeClass('report-open');
    });
    $(document).on('click', '.report-item.report-open .card-option', function () {
        $(this).parent().parent('.report-item').removeClass('report-open');
    });
});
    
    /*Static Data Table Example*/
    $(document).ready(function() {
        var stock_data = [
            {
                "name": "Name1",
                "symbol": "Symbol1",
                "last": [2.57, 2.54, 2.54, 2.56, 2.57, 2.58, 2.59]
            },
            {
                "name": "Name2",
                "symbol": "Symbol2",
                "last": [1.12, 1.11, 1.08, 1.08, 1.09, 1.11, 1.08]
            },
            {
                "name": "Name3",
                "symbol": "Symbol3",
                "last": [3.40, 3.39, 3.46, 3.51, 3.50, 3.48, 3.49]
            },
            {
                "name": "Name4",
                "symbol": "Symbol4",
                "last": [16.20, 16.40, 16.36, 16.35, 16.61, 16.46, 16.19]
            },
            {
                "name": "Name5",
                "symbol": "Symbol5",
                "last": [82.51, 83.47, 83.40, 83.68, 83.81, 83.29, 83.72]
            }
        ];
     });
       
	/* $(document).ready(function() {
        $('#example').dataTable({"bDestroy": true});
    }); */
    /*Static Data Table Example end*/
    /* $(document).ready(function() {
        $(document).on("click", function(){
            $(".chat-history ul").stop().animate({ scrollTop: $(".chat-history ul")[0].scrollHeight}, 1000);
        });
    }); */
    /*$(document).ready(function() {
        $(function () {
            var ChatDiv = $('.chat-history ul');
            var height = ChatDiv[0].scrollHeight;
            ChatDiv.scrollTop(height);
        });*/

        /* var height = 0;
        $('.chat-history ul li').each(function(i, value){
            height += parseInt($(this).height());
        });

        height += '';

        $('.chat-history ul').animate({scrollTop: height}); */
    /*});*/
/* $(document).on("click",'.sidebar-menu li a',function(){
    $(this).parent("li").toggleClass("active");
    var nextUl = $(this).next('ul');
    if(nextUl[0] != undefined){
        nextUl.toggle();
    };
}); */
/* $(document).on("click", '.sidebar-menu li a', function(e){
    debugger
    e.preventDefault(); // Prevent the default link behavior
    $(this).parent("li").toggleClass("active");
    var nextUl = $(this).next('ul');
    if (nextUl.length > 0) {
        nextUl.toggle();
    }
}); */

function fncLoadDocSelector(iInstanceID, BODGuid, AttrDGuid) {
    debugger
    var InputParams = [];
    var param1 = {};
    param1["sName"] = '{XIP|iInstanceID}';
    param1["sValue"] = iInstanceID;
    InputParams.push(param1);
    var param2 = {};
    param2["sName"] = 'BOIDXIGUID';
    param2["sValue"] = BODGuid;
    InputParams.push(param2);
    var param3 = {};
    param3["sName"] = 'AttrDXIGUID';
    param3["sValue"] = AttrDGuid;
    InputParams.push(param3);
    XILinkLoadJson('60A5E5A2-EDD7-44B7-92BF-5C5271CE9CF0', null, InputParams)
}
$(document).ready(function() {
    $(document).on('mouseenter', '.leftDetails_toggle', function() {
        var relatedHoverDetails = $(this).next('.hoverDetails');
        $('.hoverDetails').not(relatedHoverDetails).hide(); // Hide other hoverDetails
        relatedHoverDetails.show();
    });

    $(document).on('mouseleave', '.leftDetails_toggle, .hoverDetails', function() {
        if (!$('.leftDetails_toggle:hover').length && !$('.hoverDetails:hover').length) {
            $('.hoverDetails').hide();
        }
    });

    $(document).on('mouseenter', '.hoverDetails', function() {
        $(this).show();
    });
});