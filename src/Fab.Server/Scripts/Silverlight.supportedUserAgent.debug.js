///////////////////////////////////////////////////////////////////////////////
//
//  Silverlight.supportedUserAgent.js   	version 5.0.61118.0
//
//  This file is provided by Microsoft as a helper file for websites that
//  incorporate Silverlight Objects. This file is provided under the Microsoft
//  Public License available at 
//  http://code.msdn.microsoft.com/SLsupportedUA/Project/License.aspx.  
//  You may not use or distribute this file or the code in this file except as 
//  expressly permitted under that license.
// 
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

if (!window.Silverlight)
{
    window.Silverlight = { };
}

///////////////////////////////////////////////////////////////////////////////
//
// supportedUserAgent:
//
// NOTE: This function is strongly tied to current implementations of web 
// browsers. The implementation of this function will change over time to 
// account for new Web browser developments. Visit 
// http://code.msdn.microsoft.com/SLsupportedUA often to ensure that you have
// the latest version.
//
// Determines if the client browser is supported by Silverlight. 
//
//  params:
//   version [string] 
//         determines if a particular version of Silverlight supports
//         this browser. Acceptable values are "1.0" and "2.0"
//   userAgent [string]
//         optional. User Agent string to be analized. If null then the
//         current browsers user agent string will be used.
//
//  return value: boolean
//
///////////////////////////////////////////////////////////////////////////////
Silverlight.supportedUserAgent = function (version, userAgent) {
    try {
        var ua = null;

        if (userAgent) {
            ua = userAgent;
        }
        else {
            ua = window.navigator.userAgent;
        }

        var slua = { OS: 'Unsupported', Browser: 'Unsupported' };

        //Silverlight does not support pre-Windows NT platforms
        if (ua.indexOf('Windows NT') >= 0 || ua.indexOf('Mozilla/4.0 (compatible; MSIE 6.0)') >= 0) {
            slua.OS = 'Windows';
        }
        else if (ua.indexOf('PPC Mac OS X') >= 0) {
            slua.OS = 'MacPPC';
        }
        else if (ua.indexOf('Intel Mac OS X') >= 0) {
            slua.OS = 'MacIntel';
        }
        else if (ua.indexOf('Linux') >= 0) {
            slua.OS = 'Linux';
        }

        var sl_version = parseInt(version);
        var ieVer = parseInt(ua.split('MSIE')[1]);

        if (slua.OS != 'Unsupported') {
            if (ua.indexOf('MSIE') >= 0) {
                if (sl_version < 5) {
                    if (ua.indexOf('Win64') == -1) {
                        if (ieVer >= 6) {
                            slua.Browser = 'MSIE';
                        }
                    }
                }
                if (sl_version >= 5) {

                    var versionArr = ua.split('Windows NT ')[1].split('.');
                    var major = parseInt(versionArr[0]);
                    var minor = parseInt(versionArr[1]);

                    // Win 2000 does not support any SL5
                    if ((major == 5) && (minor == 0)) {
                        return false;
                    }
                    // Win XP doe not support 64 bit
                    if ((major == 5) && (minor == 1)) {
                        if (ua.indexOf('Win64') == -1) {
                            if (ieVer >= 6 && ieVer < 9) {
                                slua.Browser = 'MSIE';
                            }
                        }
                    }
                    // Win XP and Server 2003 do not support 64 bit
                    if ((major == 5) && (minor == 2)) {
                        if (ua.indexOf('Win64') == -1) {
                            if (ieVer >= 6) {
                                slua.Browser = 'MSIE';
                            }
                        }
                    }
                    // Win Vista 64 bit edition and Server 2K8 < R2 not supported for 64 bit
                    if ((major == 6) && (minor == 0)) {
                        if (ua.indexOf('Win64') >= 0) {
                            if ((ua.indexOf('Trident/4.0' >= 0)) && (ieVer >= 7)) {
                                slua.Browser = 'MSIE';
                            }
                            else {
                                return false;
                            }
                        }
                    }
                    // Win 7 and Win 2008 R2 Supported
                    if (ua.indexOf('Windows NT 6.1') >= 0) {
                        if (ieVer >= 7) {
                            slua.Browser = 'MSIE';
                        }
                    }
                }
            }
            // no other browsers support 64 bit
            else if (ua.indexOf('Firefox') >= 0) {
                if (ua.indexOf('Win64') == -1) {
                    var versionArr = ua.split('Firefox/')[1].split('.');
                    var major = parseInt(versionArr[0]);
                    var minor = parseInt(versionArr[1]);
                    if (sl_version < 5) {
                        if (major >= 2) {
                            slua.Browser = 'Firefox';
                        }
                    }
                    if (sl_version >= 5) {
                        if ((major == 3) && (minor >= 6)) {
                            if ((ua.indexOf('10_4') == -1) && (ua.indexOf('10.4') == -1)) {
                                slua.Browser = 'Firefox';
                            }
                        }
                        if (major >= 4) {
                            if ((ua.indexOf('10_4') == -1) && (ua.indexOf('10.4') == -1)) {
                                slua.Browser = 'Firefox';
                            }
                        }
                    }
                }
            }
            else if (ua.indexOf('Chrome') >= 0) {
                if (ua.indexOf('Win64') == -1) {
                    var versionArr = ua.split('Chrome/')[1].split('.');
                    var major = parseInt(versionArr[0]);
                    var minor = parseInt(versionArr[1]);
                    if (sl_version < 5) {
                        if ((major >= 4) && (major < 12)) {
                            if (slua.OS != 'MacIntel') {
                                slua.Browser = 'Chrome';
                            }
                        }
                    }
                    if (sl_version >= 5) {
                        if (major >= 12) {
                            if (slua.OS != 'MacIntel') {
                                slua.Browser = 'Chrome';
                            }
                        }
                    }

                    slua.Browser = 'Chrome';
                }
            }
            else if (ua.indexOf('Safari') >= 0) {
                var versionArr = ua.split('Version/')[1].split('.');
                var major = parseInt(versionArr[0]);
                var minor = parseInt(versionArr[1]);
                if (sl_version < 5) {
                    if (major >= 3) {
                        slua.Browser = 'Safari';
                    }
                }
                if (sl_version >= 5) {
                    if (major >= 3) {
                        if ((ua.indexOf('10_4') == -1) && (ua.indexOf('10.4') == -1)) {
                            slua.Browser = 'Safari';
                        }
                    }
                }
            }
        }

        // detect all unsupported platform combinations (IE on Mac, Safari on Win)
        var supUA = (!(slua.OS == 'Unsupported' ||                             //Unsupported OS
                            slua.Browser == 'Unsupported' ||                        //Unsupported Browser
                            (slua.OS == 'Windows' && slua.Browser == 'Safari') ||   //Safari is not supported on Windows
                            (slua.OS.indexOf('Mac') >= 0 && slua.Browser == 'MSIE') ||  //IE is not supported on Mac
                            (slua.OS.indexOf('Mac') >= 0 && slua.Browser == 'Chrome') //Google Chrome is not supported on Mac
                                ));

        // Detection of specific platform/versions

        // Google Chrome only on Windows and only in SL4+
        if ((slua.OS.indexOf('Windows') >= 0 && slua.Browser == 'Chrome' && sl_version < 4)) {
            return false;
        }

        // detect unsupported MacPPC platforms (basically anything but 1.0)
        if ((slua.OS == 'MacPPC') && (sl_version > 1)) {
            return ((supUA && (slua.OS != 'MacPPC')));
        }

        // detect unsupported Linux platforms per Moonlight
        if ((slua.OS == 'Linux') && (sl_version > 2)) {
            return ((supUA && (slua.OS != 'Linux')));
        }

        // detect other unsupported 1.0 platforms (win2k)
        if (version == '1.0') {
            return (supUA && (ua.indexOf('Windows NT 5.0') < 0));
        }
        else {
            return (supUA);
        }
    }
    catch (e) {
        return false;
    }
}
