if(!window.Silverlight)window.Silverlight={};Silverlight.supportedUserAgent=function(version,userAgent){try{var ua=null;if(userAgent)ua=userAgent;else ua=window.navigator.userAgent;var slua={OS:"Unsupported",Browser:"Unsupported"};if(ua.indexOf("Windows NT")>=0||ua.indexOf("Mozilla/4.0 (compatible; MSIE 6.0)")>=0)slua.OS="Windows";else if(ua.indexOf("PPC Mac OS X")>=0)slua.OS="MacPPC";else if(ua.indexOf("Intel Mac OS X")>=0)slua.OS="MacIntel";else if(ua.indexOf("Linux")>=0)slua.OS="Linux";var sl_version=parseInt(version),ieVer=parseInt(ua.split("MSIE")[1]);if(slua.OS!="Unsupported")if(ua.indexOf("MSIE")>=0){if(sl_version<5)if(ua.indexOf("Win64")==-1)if(ieVer>=6)slua.Browser="MSIE";if(sl_version>=5){var versionArr=ua.split("Windows NT ")[1].split("."),major=parseInt(versionArr[0]),minor=parseInt(versionArr[1]);if(major==5&&minor==0)return false;if(major==5&&minor==1)if(ua.indexOf("Win64")==-1)if(ieVer>=6&&ieVer<9)slua.Browser="MSIE";if(major==5&&minor==2)if(ua.indexOf("Win64")==-1)if(ieVer>=6)slua.Browser="MSIE";if(major==6&&minor==0)if(ua.indexOf("Win64")>=0)if(ua.indexOf("Trident/4.0">=0)&&ieVer>=7)slua.Browser="MSIE";else return false;if(ua.indexOf("Windows NT 6.1")>=0)if(ieVer>=7)slua.Browser="MSIE"}}else if(ua.indexOf("Firefox")>=0){if(ua.indexOf("Win64")==-1){var versionArr=ua.split("Firefox/")[1].split("."),major=parseInt(versionArr[0]),minor=parseInt(versionArr[1]);if(sl_version<5)if(major>=2)slua.Browser="Firefox";if(sl_version>=5){if(major==3&&minor>=6)if(ua.indexOf("10_4")==-1&&ua.indexOf("10.4")==-1)slua.Browser="Firefox";if(major>=4)if(ua.indexOf("10_4")==-1&&ua.indexOf("10.4")==-1)slua.Browser="Firefox"}}}else if(ua.indexOf("Chrome")>=0){if(ua.indexOf("Win64")==-1){var versionArr=ua.split("Chrome/")[1].split("."),major=parseInt(versionArr[0]),minor=parseInt(versionArr[1]);if(sl_version<5)if(major>=4&&major<12)if(slua.OS!="MacIntel")slua.Browser="Chrome";if(sl_version>=5)if(major>=12)if(slua.OS!="MacIntel")slua.Browser="Chrome";slua.Browser="Chrome"}}else if(ua.indexOf("Safari")>=0){var versionArr=ua.split("Version/")[1].split("."),major=parseInt(versionArr[0]),minor=parseInt(versionArr[1]);if(sl_version<5)if(major>=3)slua.Browser="Safari";if(sl_version>=5)if(major>=3)if(ua.indexOf("10_4")==-1&&ua.indexOf("10.4")==-1)slua.Browser="Safari"}var supUA=!(slua.OS=="Unsupported"||slua.Browser=="Unsupported"||slua.OS=="Windows"&&slua.Browser=="Safari"||slua.OS.indexOf("Mac")>=0&&slua.Browser=="MSIE"||slua.OS.indexOf("Mac")>=0&&slua.Browser=="Chrome");if(slua.OS.indexOf("Windows")>=0&&slua.Browser=="Chrome"&&sl_version<4)return false;if(slua.OS=="MacPPC"&&sl_version>1)return supUA&&slua.OS!="MacPPC";if(slua.OS=="Linux"&&sl_version>2)return supUA&&slua.OS!="Linux";if(version=="1.0")return supUA&&ua.indexOf("Windows NT 5.0")<0;else return supUA}catch(e){return false}}