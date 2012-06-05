
﻿Silverlight.inject=function(params)
{new AgInstall.Installer(params);}
Silverlight.onGetSilverlight=function()
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update(AgInstall.INSTALLING);pageTracker._trackEvent('Silverlight Experience','Installation','Started');};Silverlight.onRestartRequired=function()
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update(AgInstall.RESTART_REQUIRED);pageTracker._trackEvent('Silverlight Experience','Installation','Restart Required');};Silverlight.onUpgradeRequired=function()
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update(AgInstall.UPGRADE_REQUIRED);pageTracker._trackEvent('Silverlight Experience','Supported','Upgrade Required');};Silverlight.onInstallRequired=function()
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update(AgInstall.INSTALL_REQUIRED);pageTracker._trackEvent('Silverlight Experience','Supported','Install Required');};function pageLoaded()
{if(Silverlight.supportedUserAgent())
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update();}
else
{for(var i in Silverlight.installControls)
Silverlight.installControls[i].update(AgInstall.NOT_SUPPORTED);pageTracker._trackEvent('Silverlight Experience','Not Supported');}}
function onSilverlightError(sender,args)
{if(Silverlight.IsVersionAvailableOnError(sender,args))
{}
var appSource="";if(sender!=null&&sender!=0)
{appSource=sender.getHost().Source;}
var errorType=args.ErrorType;var iErrorCode=args.ErrorCode;if(errorType=="ImageError"||errorType=="MediaError")
{return;}
var errMsg="Unhandled Error in Silverlight Application "+appSource+"\n";errMsg+="Code: "+iErrorCode+"    \n";errMsg+="Category: "+errorType+"       \n";errMsg+="Message: "+args.ErrorMessage+"     \n";if(errorType=="ParserError")
{errMsg+="File: "+args.xamlFile+"     \n";errMsg+="Line: "+args.lineNumber+"     \n";errMsg+="Position: "+args.charPosition+"     \n";}
else if(errorType=="RuntimeError")
{if(args.lineNumber!=0)
{errMsg+="Line: "+args.lineNumber+"     \n";errMsg+="Position: "+args.charPosition+"     \n";}
errMsg+="MethodName: "+args.methodName+"     \n";}
throw new Error(errMsg);}
function downloadProgress(sender,eventArgs)
{sender.findName("loadingProgress").Text=Math.round(eventArgs.progress*100)+"%";sender.findName("progressBarScale").ScaleX=eventArgs.progress;}
getSilverlightVersion=function()
{var version='Silverlight (not installed)';var product='Silverlight ';try
{if(window.ActiveXObject)
{var control=new ActiveXObject('AgControl.AgControl');if(control!=null)
{if(control.isVersionSupported('4.0'))
version=product+'4';else if(control.isVersionSupported('3.0'))
version=product+'3';else if(control.isVersionSupported('2.0'))
version=product+'2';else if(control.isVersionSupported('1.0'))
version=product+'1';else
version=product+'(unknown)';control=null;}}
else
{if(navigator.userAgent.indexOf('Linux')!=-1)
product='Moonlight ';var plugin=navigator.plugins['Silverlight Plug-In'];if(plugin)
{if(plugin.description==='1.0.30226.2')
version=product+'2';else
version=product+parseInt(plugin.description[0]);}}}
catch(e){}
return version;}
startSilverlight=function()
{Silverlight.inject({source:"the.xap",parentElement:"silverlightControlHost",properties:{width:"100%",height:"100%",background:"white",version:"3.0.40624.0",SplashScreenSource:"splash",onSourceDownloadProgressChanged:"downloadProgress",onError:"onSilverlightError"},installerUiConfig:{title:"nReez File Manager",picture:"images/wide/logo.jpg",description:{install:{wide:"<p>This web site is powered by the Microsoft Silverlight, but it looks like your Web browser has no Silverlight yet.</p><p>Silverlight brings Web experiences to life with:</p><ul><li>Amazing 2D/3D animations and special effects</li><li>Smooth, high-quality video that includes HD video</li><li>Enhanced security, fast performance, and a quick install in seconds</li></ul><p>Click below to quickly update your Web browser with Microsoft Silverlight and get into nReez web site.</p>",narrow:"<p>This web site is powered by the Microsoft Silverlight.</p><p>Silverlight brings Web experiences to life through 2D/3D animations, HD video, fast performance, enhanced security, and a quick install.</p><p>Click below to quickly update your Web browser with Microsoft Silverlight and get into nReez web site.</p>",small:"<p>The nReez web site requires that you update your Web browser with Microsoft Silverlight.</p>"},upgrade:{wide:"<p>This web site is powered by the Microsoft Silverlight, but it looks like your Web browser has older version of the Silverlight.</p><p>Silverlight brings Web experiences to life with:</p><ul><li>Amazing 2D/3D animations and special effects</li><li>Smooth, high-quality video that includes HD video</li><li>Enhanced security, fast performance, and a quick install in seconds</li></ul><p>Click below to quickly update your Web browser with latest Microsoft Silverlight and get into nReez web site.</p>",narrow:"<p>This web site requires an updated version of Microsoft Silverlight.</p><p>Silverlight brings Web experiences to life through 2D/3D animations, HD video, fast performance, enhanced security, and a quick install.</p><p>Click below to quickly update your Web browser with latest Microsoft Silverlight and get into nReez web site.</p>",small:"<p>The nReez web site requires an updated version of Microsoft Silverlight.</p>"},installing:{wide:"<p>You are almost ready to view web site content. Please wait for the installation update to complete...</p>",narrow:"<p>You are almost ready to view web site content. Please wait for the installation update to complete...</p>",small:"<p>The update is installing. Please wait for the installation update to complete...</p>"},restart:{wide:"<p>The update is done. Please restart your Web browser to experience the nReez web site.</p>",narrow:"<p>The update is done. Please restart your Web browser to experience the nReez web site.</p>",small:"<p>The update is done. Please restart your Web browser to experience the nReez web site.</P>"},notSupported:{wide:"<p>This web site is powered by the <a href=\"http://www.microsoft.com/silverlight/\">Microsoft Silverlight</a>, but unfortunately your Web browser (or operation system) is not compatible with it. Please, try to visit us with a compatible Web browser like <a href=\"http://www.microsoft.com/windows/internet-explorer/default.aspx\">Microsoft Internet Explorer</a> of <a href=\"http://www.mozilla.com/en-US/firefox/\">Mozilla Firefox</a> (running on Mircosoft Windows).</p>",narrow:"<p>This web site is powered by the <a href=\"http://www.microsoft.com/silverlight/\">Microsoft Silverlight</a>, but unfortunately your Web browser (or operation system) is not compatible with it. Please, try to visit us with a compatible Web browser like <a href=\"http://www.microsoft.com/windows/internet-explorer/default.aspx\">Microsoft Internet Explorer</a> of <a href=\"http://www.mozilla.com/en-US/firefox/\">Mozilla Firefox</a> (running on Mircosoft Windows).</p>",small:"<p>The nReez web site is powered by the <a href=\"http://www.microsoft.com/silverlight/\">Microsoft Silverlight</a>, but unfortunately your Web browser (or operation system) is not compatible with it. Please, try to visit us with a compatible Web browser like <a href=\"http://www.microsoft.com/windows/internet-explorer/default.aspx\">Microsoft Internet Explorer</a> of <a href=\"http://www.mozilla.com/en-US/firefox/\">Mozilla Firefox</a> (running on Mircosoft Windows).</p>"}},buttonLabel:"UPDATE AND START"}});}