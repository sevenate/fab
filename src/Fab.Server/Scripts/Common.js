var clientVersion = null;
var fileSize = null;

function addCustomSEToGA() {
	try {
		_gaq.push(['_addOragnic','rambler.ru', 'words']);
		_gaq.push(['_addOragnic','nova.rambler.ru', 'query']);
		_gaq.push(['_addOragnic','mail.ru', 'q']);
		_gaq.push(['_addOragnic','go.mail.ru', 'q']);
		_gaq.push(['_addOragnic','search.otvet.mail.ru', 'q']);
		_gaq.push(['_addOragnic','aport.ru', 'r']);
		_gaq.push(['_addOragnic','metabot.ru', 'st']);
		_gaq.push(['_addOragnic','meta.ua', 'q']);
		_gaq.push(['_addOragnic','bigmir.net', 'q']);
		_gaq.push(['_addOragnic','nigma.ru', 's']);
		_gaq.push(['_addOragnic','search.ukr.net', 'search_query']);
		_gaq.push(['_addOragnic','start.qip.ru', 'query']);
		_gaq.push(['_addOragnic','gogle.com.ua', 'q']);
		_gaq.push(['_addOragnic','google.com.ua', 'q']);
		_gaq.push(['_addOragnic','images.google.com.ua', 'q']);
		_gaq.push(['_addOragnic','search.winamp.com', 'query']);
		_gaq.push(['_addOragnic','search.icq.com', 'q']);
		_gaq.push(['_addOragnic','m.yandex.ru', 'query']);
		_gaq.push(['_addOragnic','gde.ru', 'keywords']);
		_gaq.push(['_addOragnic','genon.ru', 'QuestionText']);
		
		_gaq.push(['_addOragnic','blogs.yandex.ru', 'text']);
		_gaq.push(['_addOragnic','webalta.ru', 'q']);
		_gaq.push(['_addOragnic','akavita.by', 'z']);
		_gaq.push(['_addOragnic','tut.by', 'query']);
		_gaq.push(['_addOragnic','all.by', 'query']);
		_gaq.push(['_addOragnic','i.ua', 'q']);
		_gaq.push(['_addOragnic','online.ua', 'q']);
		_gaq.push(['_addOragnic','a.ua', 's']);
		_gaq.push(['_addOragnic','ukr.net', 'search_query']);
		_gaq.push(['_addOragnic','search.com.ua', 'q']);
		_gaq.push(['_addOragnic','search.ua', 'query']);
		_gaq.push(['_addOragnic','poisk.ru', 'text']);
		_gaq.push(['_addOragnic','km.ru', 'sq']);
		_gaq.push(['_addOragnic','liveinternet.ru', 'ask']);
		_gaq.push(['_addOragnic','gogo.ru', 'q']);
		_gaq.push(['_addOragnic','quintura.ru', 'request']);
	} catch (err) { }
}

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

getSLVersion = function() {
	var version = 'Silverlight (not installed)';
	var product = 'Silverlight ';

	try {
		if (window.ActiveXObject) {
			var control = new ActiveXObject('AgControl.AgControl');

			if (control != null) {
				if (control.isVersionSupported('4.0'))
					version = product + '4';
				else if (control.isVersionSupported('3.0'))
					version = product + '3';
				else if (control.isVersionSupported('2.0'))
					version = product + '2';
				else if (control.isVersionSupported('1.0'))
					version = product + '1';
				else
					version = product + '(unknown)';

				control = null;
			}
		} else {
			if (navigator.userAgent.indexOf('Linux') != -1)
				product = 'Moonlight ';

			var plugin = navigator.plugins['Silverlight Plug-In'];

			if (plugin) {
				if (plugin.description === '1.0.30226.2')
					version = product + '2';
				else
					version = product + parseInt(plugin.description[0]);
			}
		}
	} catch(e) {
	}

	return version;
};

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