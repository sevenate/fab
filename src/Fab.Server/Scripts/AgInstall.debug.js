///////////////////////////////////////////////////////////////////////////////
//
//  AgInstall.js   				version 2.0.31030.0M
//
//  This file is provided by Microsoft as a helper file for websites that
//  incorporate Silverlight Objects. This file is provided under the Microsoft
//  Public License available at 
//  http://code.msdn.microsoft.com/silverlightjs/Project/License.aspx.  
//  You may not use or distribute this file or the code in this file except as 
//  expressly permitted under that license.
// 
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

window.AgInstall = {
    INSTALL_REQUIRED: "install",
    UPGRADE_REQUIRED: "upgrade",
    RESTART_REQUIRED: "restart",
    INSTALLING: "installing",
    NOT_SUPPORTED: "notSupported",

    WIDE: "Wide",
    NARROW: "Narrow",
    SMALL: "Small",
    ENCODER: "Encoder",

    IsIE6: typeof window.XMLHttpRequest == "undefined" && window.external,
    IsIE: /msie/i.test(navigator.userAgent) && !/opera/i.test(navigator.userAgent),
    
    DefaultConfig:
    {
        title: "Silverlight Project",
        picture: "images/wide/light.jpg",
        description:
        {
            install:
            {
                wide: "<p>The application you are trying to use requires that you update  your Web browser with Microsoft Silverlight.</p><p>Silverlight delivers amazing Web experiences across leading news, sports, and entertainment sites.</p><p>Silverlight brings Web experiences to life with:</p><ul><li>Amazing 2D/3D animations and special effects</li><li>Smooth, high-quality video that includes HD video</li><li>Enhanced security, fast performance, and a quick install in seconds</li></ul>",
                narrow: "<p>The application you are trying to use requires that you update your Web browser with Microsoft Silverlight.</p><p>Silverlight brings Web experiences to life through 2D/3D animations, HD video, fast performance, enhanced security, and a quick install.</p>",
                small: "<p>The application you are trying to use requires that you update your Web browser with Microsoft Silverlight.</p>",
                encoder: "<p>The media you are trying to play requires updating your browser with Microsoft Silverlight technology.</p>"
            },
            upgrade:
            {
                wide: "<p>The application you are trying to use requires an updated version of Microsoft Silverlight.</p><p>Silverlight delivers amazing Web experiences across leading news, sports, and entertainment sites.</p><p>Silverlight brings Web experiences to life with:</p><ul><li>Amazing 2D/3D animations and special effects</li><li>Smooth, high-quality video that includes HD video</li><li>Enhanced security, fast performance, and a quick install in seconds</li></ul>",
                narrow: "<p>The application you are trying to use requires an updated version of Microsoft Silverlight.</p><p>Silverlight brings Web experiences to life through 2D/3D animations, HD video, enhanced security, fast performance, and a quick install.",
                small: "<p>The application you are trying to use requires an updated version of Microsoft Silverlight.</p>",
                encoder: "<p>The media you are trying to play requires an updated version of Microsoft Silverlight.</p>"
            },
            installing:
            {
                wide: "<p>The update is installing. Please wait for the installation to complete, then restart your Web browser. </p>",
                narrow: "<p>The update is installing. Please wait for the installation to complete, then restart your Web browser. </p>",
                small: "<p>The update is installing. Please wait for the installation to complete, then restart your Web browser. </p>",
                encoder: "<p>The update is installing. Please wait for the installation to complete, then restart your Web browser. </p>"
            },
            restart:
            {
                wide: "<p>The update is almost done. Please restart the Web browser to experience the application.</p>",
                narrow: "<p>The update is almost done. Please restart the Web browser to experience the application.</p>",
                small: "<p>The update is almost done. Please restart the Web browser to experience the application.</p>",
                encoder: "<p>The update is almost done. Please restart the Web browser to experience the media.</p>"
            },
            notSupported:
            {
                wide: "<p>This Web browser is not compatible with this application.</p>",
                narrow: "<p>This Web browser is not compatible with this application.</p>",
                small: "<p>This Web browser is not compatible with this application.</p>",
                encoder: "<p>This Web browser is not compatible with this media playback.</p>"
            }
        },
        buttonLabel: "UPDATE AND START &gt;"
    },

    Template: function(t, i) {
        if (t == AgInstall.ENCODER)
            return "<div class=\"slInstallEncoder\"><div class=\"slInstallWrapper\"><p class=\"slInstallEncoderHeader\" style=\"font-size:18px;\">" + i.config.title + "</p><div class=\"slInstallEncoderPlayBtn\"><a href=\"javascript:Silverlight.getSilverlight('{1}');\"><img width=\"109\" height=\"102\" src=\"images/encoder/playBtn.png\" /></a></div><div class=\"slInstallEncoderTitle\"><a class=\"slButtonEncoder\" href=\"javascript:Silverlight.getSilverlight('{1}');\">" + i.config.buttonLabel + "</a></div><div class=\"slInstallEncoderContent\"><div class=\"slDescription\"></div></div><div class=\"slInstallEncoderLogo\"><img width=\"177\" height=\"15\" src=\"images/encoder/logo.png\" /></div></div><div class=\"slInstallEncoderOverlay\"></div></div>";
        else if (t == AgInstall.WIDE || t == AgInstall.NARROW)
            return "<div class=\"slInstall\"><div class=\"slInstallPopup" + t + "\"><div class=\"slPopupContent" + t + "\"><div class=\"slScreenshot" + t + "\"><img src=\"" + i.config.picture + "\" /></div><div class=\"slTextContent" + t + "\"><div class=\"slHeadline" + t + "\">" + i.config.title + "</div><div class=\"slDescription\"></div><img class=\"slDivider\" src=\"" + (t == AgInstall.WIDE ? "images/wide/divider.png" : "images/narrow/divider.png") + "\" /><a class=\"slButton" + t + "\" href=\"javascript:Silverlight.getSilverlight('{1}');\">" + i.config.buttonLabel + "</a></div></div><div class=\"slPopupTop" + t + "\"></div><div class=\"slPopupLoop" + t + "\"></div><div class=\"slPopupBot" + t + "\"></div></div></div>";
        else if (t == AgInstall.SMALL)
            return "<div class=\"slInstallSmall\"><div class=\"slInstallContentSmall\"><p class=\"slTextContent\"><div class=\"slDescription\"></div></p></div><div class=\"slInstallUpdateLinkSmall\"><a class=\"slButtonSmall\" href=\"javascript:Silverlight.getSilverlight('{1}');\">" + i.config.buttonLabel + "</a></div><img class=\"slInstallLogoSmall\" src=\"images/small/powered_by_sl.jpg\" /></div>";
    },

    // Iterate objects and copy contents where it is missing in the original
    Fill: function(f, t) {
        for (var k in f) {
            if (!t[k] || (typeof t[k] != typeof f[k]))
                t[k] = f[k];
            else if (typeof f[k] == "object")
                AgInstall.Fill(f[k], t[k]);
        }
    },

    Installer: function(params) {

        var NEED_INSTALL = 'install';
        this.config = params.installerUiConfig;

        // Iterate config and replace any missing parts with default
        if (!this.config)
            this.config = AgInstall.DefaultConfig;
        else
            AgInstall.Fill(AgInstall.DefaultConfig, this.config);

        params.properties.alt = NEED_INSTALL;
        var version = params.properties.version;

        var element = document.getElementById(params.parentElement);
        if (!element) {
            alert("Silverlight could not find a DOM element with id '" + params.parentElement + "'.");
            return;
        }

        // Perform the injection ourselves, so make sure Silverlight knows not to do it
        params.parentElement = null;
        var obj = Silverlight.createObjectEx(params);

        if (obj == NEED_INSTALL) {
            // Installation is required.
            var t = AgInstall.WIDE;

            var w = element.offsetWidth;
            var h = element.offsetHeight;
            if (w <= 350 || h <= 475)
                t = AgInstall.SMALL;
            else if ((w <= 600 && w >= 350) && (h <= 700 && h >= 475))
                t = AgInstall.NARROW;
            this.type = this.config.type ? this.config.type : t;
            // Ensure that the styles are attached.
            if (!Silverlight.attachedInstallDependencies) {
                document.write('<link rel="stylesheet" type="text/css" href="', 'Content/slInstall.css', '"/>');
                // If we are in IE6, attach a stylesheet that allows for png transparency
                if (AgInstall.IsIE6)
                    document.write('<link rel="stylesheet" type="text/css" href="', 'Content/ie6.css', '"/>');
                Silverlight.attachedInstallDependencies = true;
            }
            if (!Silverlight.attachedTemplateDependencies || !Silverlight.attachedTemplateDependencies[this.type]) {
                document.write('<link rel="stylesheet" type="text/css" href="', 'Content/slInstall' + this.type + '.css', '"/>');
            }
            if (!Silverlight.installControls)
                Silverlight.installControls = [];
            Silverlight.installControls.push(this);

            element.innerHTML = AgInstall.Template(this.type, this).replace(/\{1\}/g, version);

            this.root = element;
            element.style.textAlign = "center";
            var divs = this.root.getElementsByTagName("*");
            for (var i = 0; i < divs.length; i++) {
                this[divs[i].className] = divs[i];
            }
            this.state = AgInstall.UPGRADE_REQUIRED;

            // Update the installer to a predefined state
            this.update = function(state) {
                // If no state was passed, just redraw with the current state
                if (!state)
                    state = this.state;
                if (this.slDivider && (this.type == AgInstall.WIDE || this.type == AgInstall.NARROW))
                    this.slDivider.style.display = 'block';
                this['slButton' + this.type].style.display = 'block';
                this.state = state;

                //alert(state + " " + this.type.toLowerCase());
                var desc = this.config.description[state][this.type.toLowerCase()];
                switch (state) {
                    case AgInstall.INSTALLING:
                        if (this.slDivider)
                            this.slDivider.style.display = 'none';
                        this['slButton' + this.type].style.display = 'none';
                        break;
                    case AgInstall.RESTART_REQUIRED:
                        if (this.slDivider)
                            this.slDivider.style.display = 'none';
                        this['slButton' + this.type].style.display = 'none';
                        break;
                    case AgInstall.NOT_SUPPORTED:
                        if (this.slDivider)
                            this.slDivider.style.display = 'none';
                        this['slButton' + this.type].style.display = 'none';
                        break;
                }
                this.slDescription.innerHTML = desc;
                // The layout updating is slightly different for the small install experience
                if (this.type == AgInstall.SMALL) {
                    var contentSmallHeight = this.slInstallSmall.offsetHeight;
                    var calcContentSmallHeight = contentSmallHeight - 59;
                    this.slInstallSmall.style.height = Math.max(calcContentSmallHeight, this.root.offsetHeight - 2) + "px";
                }
                else if (this.type == AgInstall.ENCODER) {
                    this.root.style.textAlign = "center";
                    w = this.config.pictureWidth;
                    h = this.config.pictureHeight;
                    var small = this.config.pictureWidth < 300 || this.config.pictureHeight < 250;
                    if (small || (state != AgInstall.INSTALL_REQUIRED && state != AgInstall.UPGRADE_REQUIRED))
                        this.slInstallEncoderPlayBtn.style.display = 'none';
                    else
                        this.slInstallEncoderPlayBtn.style.display = 'block';

                    //var w = Math.max(300, this.config.pictureWidth);
                    //var h = Math.max(250, this.config.pictureHeight);
                    this.slInstallEncoder.style["background"] = "#000000 url(" + this.config.picture + ") no-repeat scroll center center";
                    this.slInstallWrapper.style.width = this.slInstallEncoder.style.width = (w - (AgInstall.IsIE ? 0 : 1)) + "px";
                    this.slInstallWrapper.style.height = this.slInstallEncoder.style.height = (h - (AgInstall.IsIE ? 0 : 1)) + "px";
                    this.slInstallEncoderOverlay.style.width = this.slInstallEncoder.style.width = w + "px";
                    this.slInstallEncoderOverlay.style.height = this.slInstallEncoder.style.height = h + "px";
                    var calcContentHeight = this.slInstallEncoder.offsetHeight - ( small ? 0 : 102 ) - this['slInstallEncoderTitle'].offsetHeight - 55 - 18;
                    this['slInstallEncoderContent'].style.height = calcContentHeight + "px";
                }
                else {
                    var t = this['slPopupTop' + this.type].offsetHeight;
                    var b = this['slPopupBot' + this.type].offsetHeight;
                    var h = Math.max(t + b, this['slPopupContent' + this.type].offsetHeight + 20);
                    this['slPopupLoop' + this.type].style.height = Math.max(0, h - t - b) + "px";
                    this['slInstallPopup' + this.type].style.height = h + "px";
                    this['slInstallPopup' + this.type].style.marginTop = -Math.round(this['slInstallPopup' + this.type].offsetHeight * 0.5) + "px"; //Math.round((this.root.offsetHeight / 2) - (this['slInstallPopup' + this.type].offsetHeight / 2)) + "px";
                }
            };
        }
        // No installation required, inject the control
        else
            element.innerHTML = obj;
    }
}