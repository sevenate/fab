var clientVersion = null;
var fileSize = null;

Silverlight.inject = function(params) {
	new AgInstall.Installer(params);
};

Silverlight.onGetSilverlight = function() {
	for (var i in Silverlight.installControls)
		Silverlight.installControls[i].update(AgInstall.INSTALLING);
	_gaq.push(['_trackEvent', 'Silverlight Experience', 'Installation', 'Started']);
};

Silverlight.onRestartRequired = function() {
	for (var i in Silverlight.installControls)
		Silverlight.installControls[i].update(AgInstall.RESTART_REQUIRED);
	_gaq.push(['_trackEvent', 'Silverlight Experience', 'Installation', 'Restart Required']);
};

Silverlight.onUpgradeRequired = function() {
	for (var i in Silverlight.installControls)
		Silverlight.installControls[i].update(AgInstall.UPGRADE_REQUIRED);
	_gaq.push(['_trackEvent', 'Silverlight Experience', 'Supported', 'Upgrade Required']);
};

Silverlight.onInstallRequired = function() {
	for (var i in Silverlight.installControls)
		Silverlight.installControls[i].update(AgInstall.INSTALL_REQUIRED);
	_gaq.push(['_trackEvent', 'Silverlight Experience', 'Supported', 'Install Required']);
};

function addSeparatorsNF(nStr, inD, outD, sep) {
	nStr += '';
	var dpos = nStr.indexOf(inD);
	var nStrEnd = '';
	if (dpos != -1) {
		nStrEnd = outD + nStr.substring(dpos + 1, nStr.length);
		nStr = nStr.substring(0, dpos);
	}
	var rgx = /(\d+)(\d{3})/;
	while (rgx.test(nStr)) {
		nStr = nStr.replace(rgx, '$1' + sep + '$2');
	}
	return nStr + nStrEnd;
}

function getXapSize(url) {
	var self = this;

	// Mozilla/Safari
	if (window.XMLHttpRequest) {
		self.xmlHttpReq = new XMLHttpRequest();
	}
	// IE
	else if (window.ActiveXObject) {
		self.xmlHttpReq = new ActiveXObject('Microsoft.XMLHTTP');
	}

	self.xmlHttpReq.open('HEAD', url, true);
	self.xmlHttpReq.onreadystatechange = function() {
		if (self.xmlHttpReq.readyState == 4) {
			fileSize = self.xmlHttpReq.getResponseHeader('Content-Length');
		}
	};

	self.xmlHttpReq.send(null);
}

function downloadProgress(sender, eventArgs) {
	sender.findName('loadingProgress').Text = Math.round(eventArgs.progress * 100) + '%';
	sender.findName('progressBarScale').ScaleX = eventArgs.progress;
	if (fileSize != null) {
		sender.findName('loadingText').Text = 'Downloaded ';
		sender.findName('downloadedBytes').Text = addSeparatorsNF((eventArgs.progress * fileSize).toFixed(0), ',', ',', ' ');
		sender.findName('totalBytes').Text = addSeparatorsNF((fileSize / 1).toFixed(0), ',', ',', ' ');
		sender.findName('downloadDetails').Visibility = 'Visible';
	}
}

function onSilverlightError(sender, args) {
	if (Silverlight.IsVersionAvailableOnError(sender, args)) {
		//run error handling code
	}

	var appSource = '';
	if (sender != null && sender != 0) {
		appSource = sender.getHost().Source;
	}

	var errorType = args.ErrorType;
	var iErrorCode = args.ErrorCode;

	if (errorType == 'ImageError' || errorType == 'MediaError') {
		return;
	}

	var errMsg = 'Unhandled Error in Silverlight Application ' + appSource + '\n';

	errMsg += 'Code: ' + iErrorCode + '    \n';
	errMsg += 'Category: ' + errorType + '       \n';
	errMsg += 'Message: ' + args.ErrorMessage + '     \n';

	if (errorType == 'ParserError') {
		errMsg += 'File: ' + args.xamlFile + '     \n';
		errMsg += 'Line: ' + args.lineNumber + '     \n';
		errMsg += 'Position: ' + args.charPosition + '     \n';
	} else if (errorType == 'RuntimeError') {
		if (args.lineNumber != 0) {
			errMsg += 'Line: ' + args.lineNumber + '     \n';
			errMsg += 'Position: ' + args.charPosition + '     \n';
		}

		errMsg += 'MethodName: ' + args.methodName + '     \n';
	}

	throw new Error(errMsg);
}

function getClientParam(){
	return (clientVersion != null && clientVersion != '' ? ('?' + clientVersion) : '');
}

function pageLoaded() {
	if (Silverlight.supportedUserAgent('5.0', null)) {
		// silverlight is supported update any installers
		for (var i in Silverlight.installControls)
			Silverlight.installControls[i].update();
		getXapSize('ClientBin/SilverFAB.xap' + getClientParam());
	} else {
		// silverlight is not supported, need to tell installers to report this
		for (var j in Silverlight.installControls)
			Silverlight.installControls[j].update(AgInstall.NOT_SUPPORTED);
		_gaq.push(['_trackEvent', 'Silverlight Experience', 'Not Supported']);
	}
}