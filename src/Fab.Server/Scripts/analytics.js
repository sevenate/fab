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

getSLVersion = function() {
	var version = 'not installed';
	var product = 'Silverlight ';

	try {
		if (window.ActiveXObject) {
			var control = new ActiveXObject('AgControl.AgControl');

			if (control != null) {
				if (control.isVersionSupported('5.0'))
                	version = product + '5';
				else if (control.isVersionSupported('4.0'))
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