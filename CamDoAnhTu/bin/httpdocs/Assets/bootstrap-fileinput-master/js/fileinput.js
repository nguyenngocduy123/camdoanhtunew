/*!
 * @copyright Copyright &copy; Kartik Visweswaran, Krajee.com, 2014 - 2015
 * @version 4.2.3
 *
 * File input styled for Bootstrap 3.0 that utilizes HTML5 File Input's advanced 
 * features including the FileReader API. 
 * 
 * The plugin drastically enhances the HTML file input to preview multiple files on the client before
 * upload. In addition it provides the ability to preview content of images, text, videos, audio, html, 
 * flash and other objects. It also offers the ability to upload and delete files using AJAX, and add 
 * files in batches (i.e. preview, append, or remove before upload).
 * 
 * Author: Kartik Visweswaran
 * Copyright: 2015, Kartik Visweswaran, Krajee.com
 * For more JQuery plugins visit http://plugins.krajee.com
 * For more Yii related demos visit http://demos.krajee.com
 */
(function ($) {
    "use strict";

    $.fn.fileinputLocales = {};

    String.prototype.repl = function (from, to) {
        return this.split(from).join(to);
    };
    var isIE = function (ver) {
            var div = document.createElement("div"), status;
            div.innerHTML = "<!--[if IE " + ver + "]><i></i><![endif]-->";
            status = (div.getElementsByTagName("i").length === 1);
            document.body.appendChild(div);
            div.parentNode.removeChild(div);
            return status;
        },
        previewCache = {
            data: {},
            init: function (obj) {
                var content = obj.initialPreview, id = obj.id;
                if (content.length > 0 && !isArray(content)) {
                    content = content.split(obj.initialPreviewDelimiter);
                }
                previewCache.data[id] = {
                    content: content,
                    config: obj.initialPreviewConfig,
                    tags: obj.initialPreviewThumbTags,
                    delimiter: obj.initialPreviewDelimiter,
                    template: obj.previewGenericTemplate,
                    msg: function (n) {
                        return obj.getMsgSelected(n);
                    },
                    initId: obj.previewInitId,
                    footer: obj.getLayoutTemplate('footer'),
                    isDelete: obj.initialPreviewShowDelete,
                    caption: obj.initialCaption,
                    actions: function (showUpload, showDelete, disabled, url, key) {
                        return obj.renderFileActions(showUpload, showDelete, disabled, url, key);
                    }
                };
            },
            fetch: function (id) {
                return previewCache.data[id].content.filter(function (n) {
                    return n !== null;
                });
            },
            count: function (id, all) {
                return !!previewCache.data[id] && !!previewCache.data[id].content ?
                    (all ? previewCache.data[id].content.length : previewCache.fetch(id).length) : 0;
            },
            get: function (id, i, isDisabled) {
                var ind = 'init_' + i, data = previewCache.data[id], config = data.config[i],
                    previewId = data.initId + '-' + ind, out, $tmp, frameAttr = {},
                    frameClass = ' file-preview-initial';
                isDisabled = isDisabled === undefined ? true : isDisabled;
                if (data.content[i] === null) {
                    return '';
                }
                if (!isEmpty(config) && !isEmpty(config.frameClass)) {
                    frameClass += ' ' + config.frameClass;
                }
                out = data.template
                    .repl('{previewId}', previewId)
                    .repl('{frameClass}', frameClass)
                    .repl('{fileindex}', ind)
                    .repl('{content}', data.content[i])
                    .repl('{footer}', previewCache.footer(id, i, isDisabled));
                if (data.tags.length && data.tags[i]) {
                    out = replaceTags(out, data.tags[i]);
                }
                if (!isEmpty(config) && !isEmpty(config.frameAttr)) {
                    $tmp = $(document.createElement('div')).html(out);
                    $tmp.find('.file-preview-initial').attr(config.frameAttr);
                    out = $tmp.html();
                    $tmp.remove();
                }
                return out;
            },
            add: function (id, content, config, tags, append) {
                var data = $.extend(true, {}, previewCache.data[id]), index;
                if (!isArray(content)) {
                    content = content.split(data.delimiter);
                }
                if (append) {
                    index = data.content.push(content) - 1;
                    data.config[index] = config;
                    data.tags[index] = tags;
                } else {
                    index = content.length;
                    data.content = content;
                    data.config = config;
                    data.tags = tags;
                }
                previewCache.data[id] = data;
                return index;
            },
            set: function (id, content, config, tags, append) {
                var data = $.extend(true, {}, previewCache.data[id]), i;
                if (!isArray(content)) {
                    content = content.split(data.delimiter);
                }
                if (append) {
                    for (i = 0; i < content.length; i++) {
                        data.content.push(content[i]);
                    }
                    for (i = 0; i < config.length; i++) {
                        data.config.push(config[i]);
                    }
                    for (i = 0; i < tags.length; i++) {
                        data.tags.push(tags[i]);
                    }
                } else {
                    data.content = content;
                    data.config = config;
                    data.tags = tags;
                }
                previewCache.data[id] = data;
            },
            unset: function (id, index) {
                var chk = previewCache.count(id);
                if (!chk) {
                    return;
                }
                if (chk === 1) {
                    previewCache.data[id].content = [];
                    previewCache.data[id].config = [];
                    return;
                }
                previewCache.data[id].content[index] = null;
                previewCache.data[id].config[index] = null;
            },
            out: function (id) {
                var html = '', data = previewCache.data[id], caption, len = previewCache.count(id, true);
                if (len === 0) {
                    return {content: '', caption: ''};
                }
                for (var i = 0; i < len; i++) {
                    html += previewCache.get(id, i);
                }
                caption = data.msg(previewCache.count(id));
                return {content: html, caption: caption};
            },
            footer: function (id, i, isDisabled) {
                var data = previewCache.data[id];
                isDisabled = isDisabled === undefined ? true : isDisabled;
                if (data.config.length === 0 || isEmpty(data.config[i])) {
                    return '';
                }
                var config = data.config[i],
                    caption = isSet('caption', config) ? config.caption : '',
                    width = isSet('width', config) ? config.width : 'auto',
                    url = isSet('url', config) ? config.url : false,
                    key = isSet('key', config) ? config.key : null,
                    disabled = (url === false) && isDisabled,
                    actions = data.isDelete ? data.actions(false, true, disabled, url, key) : '',
                    footer = data.footer.repl('{actions}', actions);
                return footer
                    .repl('{caption}', caption)
                    .repl('{width}', width)
                    .repl('{indicator}', '')
                    .repl('{indicatorTitle}', '');
            }
        },
        getNum = function (num, def) {
            def = def || 0;
            if (typeof num === "number") {
                return num;
            }
            if (typeof num === "string") {
                num = parseFloat(num);
            }
            return isNaN(num) ? def : num;
        },
        hasFileAPISupport = function () {
            return window.File && window.FileReader;
        },
        hasDragDropSupport = function () {
            var $div = document.createElement('div');
            return !isIE(9) && ($div.draggable !== undefined || ($div.ondragstart !== undefined && $div.ondrop !== undefined));
        },
        hasFileUploadSupport = function () {
            return hasFileAPISupport() && window.FormData;
        },
        addCss = function ($el, css) {
            $el.removeClass(css).addClass(css);
        },
        STYLE_SETTING = 'style="width:{width};height:{height};"',
        OBJECT_PARAMS = '      <param name="controller" value="true" />\n' +
            '      <param name="allowFullScreen" value="true" />\n' +
            '      <param name="allowScriptAccess" value="always" />\n' +
            '      <param name="autoPlay" value="false" />\n' +
            '      <param name="autoStart" value="false" />\n' +
            '      <param name="quality" value="high" />\n',
        DEFAULT_PREVIEW = '<div class="file-preview-other">\n' +
            '       {previewFileIcon}\n' +
            '   </div>',
        defaultFileActionSettings = {
            removeIcon: '<i class="glyphicon glyphicon-trash text-danger"></i>',
            removeClass: 'btn btn-xs btn-default',
            removeTitle: 'Remove file',
            uploadIcon: '<i class="glyphicon glyphicon-upload text-info"></i>',
            uploadClass: 'btn btn-xs btn-default',
            uploadTitle: 'Upload file',
            indicatorNew: '<i class="glyphicon glyphicon-hand-down text-warning"></i>',
            indicatorSuccess: '<i class="glyphicon glyphicon-ok-sign file-icon-large text-success"></i>',
            indicatorError: '<i class="glyphicon glyphicon-exclamation-sign text-danger"></i>',
            indicatorLoading: '<i class="glyphicon glyphicon-hand-up text-muted"></i>',
            indicatorNewTitle: 'Not uploaded yet',
            indicatorSuccessTitle: 'Uploaded',
            indicatorErrorTitle: 'Upload Error',
            indicatorLoadingTitle: 'Uploading ...'
        },
        tMain1 = '{preview}\n' +
            '<div class="kv-upload-progress hide"></div>\n' +
            '<div class="input-group {class}">\n' +
            '   {caption}\n' +
            '   <div class="input-group-btn">\n' +
            '       {remove}\n' +
            '       {cancel}\n' +
            '       {upload}\n' +
            '       {browse}\n' +
            '   </div>\n' +
            '</div>',
        tMain2 = '{preview}\n<div class="kv-upload-progress hide"></div>\n{remove}\n{cancel}\n{upload}\n{browse}\n',
        tPreview = '<div class="file-preview {class}">\n' +
            '    <div class="close fileinput-remove">&times;</div>\n' +
            '    <div class="{dropClass}">\n' +
            '    <div class="file-preview-thumbnails">\n' +
            '    </div>\n' +
            '    <div class="clearfix"></div>' +
            '    <div class="file-preview-status text-center text-success"></div>\n' +
            '    <div class="kv-fileinput-error"></div>\n' +
            '    </div>\n' +
            '</div>',
        tIcon = '<span class="glyphicon glyphicon-file kv-caption-icon"></span>',
        tCaption = '<div tabindex="-1" class="form-control file-caption {class}">\n' +
            '   <span class="file-caption-ellipsis">&hellip;</span>\n' +
            '   <div class="file-caption-name"></div>\n' +
            '</div>',
        tModal = '<div id="{id}" class="modal fade">\n' +
            '  <div class="modal-dialog modal-lg">\n' +
            '    <div class="modal-content">\n' +
            '      <div class="modal-header">\n' +
            '        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>\n' +
            '        <h3 class="modal-title">Detailed Preview <small>{title}</small></h3>\n' +
            '      </div>\n' +
            '      <div class="modal-body">\n' +
            '        <textarea class="form-control" style="font-family:Monaco,Consolas,monospace; height: {height}px;" readonly>{body}</textarea>\n' +
            '      </div>\n' +
            '    </div>\n' +
            '  </div>\n' +
            '</div>',
        tProgress = '<div class="progress">\n' +
            '    <div class="{class}" role="progressbar"' +
            ' aria-valuenow="{percent}" aria-valuemin="0" aria-valuemax="100" style="width:{percent}%;">\n' +
            '        {percent}%\n' +
            '     </div>\n' +
            '</div>',
        tFooter = '<div class="file-thumbnail-footer">\n' +
            '    <div class="file-caption-name">{caption}</div>\n' +
            '    {actions}\n' +
            '</div>',
        tActions = '<div class="file-actions">\n' +
            '    <div class="file-footer-buttons">\n' +
            '        {upload}{delete}{other}' +
            '    </div>\n' +
            '    <div class="file-upload-indicator" tabindex="-1" title="{indicatorTitle}">{indicator}</div>\n' +
            '    <div class="clearfix"></div>\n' +
            '</div>',
        tActionDelete = '<button type="button" class="kv-file-remove {removeClass}" ' +
            'title="{removeTitle}"{dataUrl}{dataKey}>{removeIcon}</button>\n',
        tActionUpload = '<button type="button" class="kv-file-upload {uploadClass}" title="{uploadTitle}">' +
            '   {uploadIcon}\n</button>\n',
        tGeneric = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}">\n' +
            '   {content}\n' +
            '   {footer}\n' +
            '</div>\n',
        tHtml = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}">\n' +
            '    <object data="{data}" type="{type}" width="{width}" height="{height}">\n' +
            '       ' + DEFAULT_PREVIEW + '\n' +
            '    </object>\n' +
            '   {footer}\n' +
            '</div>',
        tImage = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}">\n' +
            '   <img src="{data}" class="file-preview-image" title="{caption}" alt="{caption}" ' + STYLE_SETTING + '>\n' +
            '   {footer}\n' +
            '</div>\n',
        tText = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}">\n' +
            '   <div class="file-preview-text" title="{caption}" ' + STYLE_SETTING + '>\n' +
            '       {data}\n' +
            '   </div>\n' +
            '   {footer}\n' +
            '</div>',
        tVideo = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}"' +
            ' title="{caption}" ' + STYLE_SETTING + '>\n' +
            '   <video width="{width}" height="{height}" controls>\n' +
            '       <source src="{data}" type="{type}">\n' +
            '       ' + DEFAULT_PREVIEW + '\n' +
            '   </video>\n' +
            '   {footer}\n' +
            '</div>\n',
        tAudio = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}"' +
            ' title="{caption}" ' + STYLE_SETTING + '>\n' +
            '   <audio controls>\n' +
            '       <source src="' + '{data}' + '" type="{type}">\n' +
            '       ' + DEFAULT_PREVIEW + '\n' +
            '   </audio>\n' +
            '   {footer}\n' +
            '</div>',
        tFlash = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}"' +
            ' title="{caption}" ' + STYLE_SETTING + '>\n' +
            '   <object type="application/x-shockwave-flash" width="{width}" height="{height}" data="{data}">\n' +
            OBJECT_PARAMS + '       ' + DEFAULT_PREVIEW + '\n' +
            '   </object>\n' +
            '   {footer}\n' +
            '</div>\n',
        tObject = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}"' +
            ' title="{caption}" ' + STYLE_SETTING + '>\n' +
            '   <object data="{data}" type="{type}" width="{width}" height="{height}">\n' +
            '       <param name="movie" value="{caption}" />\n' +
            OBJECT_PARAMS + '         ' + DEFAULT_PREVIEW + '\n' +
            '   </object>\n' +
            '   {footer}\n' +
            '</div>',
        tOther = '<div class="file-preview-frame{frameClass}" id="{previewId}" data-fileindex="{fileindex}"' +
            ' title="{caption}" ' + STYLE_SETTING + '>\n' +
            '   ' + DEFAULT_PREVIEW + '\n' +
            '   {footer}\n' +
            '</div>',
        defaultLayoutTemplates = {
            main1: tMain1,
            main2: tMain2,
            preview: tPreview,
            icon: tIcon,
            caption: tCaption,
            modal: tModal,
            progress: tProgress,
            footer: tFooter,
            actions: tActions,
            actionDelete: tActionDelete,
            actionUpload: tActionUpload
        },
        defaultPreviewTemplates = {
            generic: tGeneric,
            html: tHtml,
            image: tImage,
            text: tText,
            video: tVideo,
            audio: tAudio,
            flash: tFlash,
            object: tObject,
            other: tOther
        },
        defaultPreviewTypes = ['image', 'html', 'text', 'video', 'audio', 'flash', 'object'],
        defaultPreviewSettings = {
            image: {width: "auto", height: "160px"},
            html: {width: "213px", height: "160px"},
            text: {width: "160px", height: "160px"},
            video: {width: "213px", height: "160px"},
            audio: {width: "213px", height: "80px"},
            flash: {width: "213px", height: "160px"},
            object: {width: "160px", height: "160px"},
            other: {width: "160px", height: "160px"}
        },
        defaultFileTypeSettings = {
            image: function (vType, vName) {
                return (vType !== undefined) ? vType.match('image.*') : vName.match(/\.(gif|png|jpe?g)$/i);
            },
            html: function (vType, vName) {
                return (vType !== undefined) ? vType === 'text/html' : vName.match(/\.(htm|html)$/i);
            },
            text: function (vType, vName) {
                return (vType !== undefined && vType.match('text.*')) || vName.match(/\.(txt|md|csv|nfo|php|ini)$/i);
            },
            video: function (vType, vName) {
                return (vType !== undefined && vType.match(/\.video\/(ogg|mp4|webm)$/i)) || vName.match(/\.(og?|mp4|webm)$/i);
            },
            audio: function (vType, vName) {
                return (vType !== undefined && vType.match(/\.audio\/(ogg|mp3|wav)$/i)) || vName.match(/\.(ogg|mp3|wav)$/i);
            },
            flash: function (vType, vName) {
                return (vType !== undefined && vType === 'application/x-shockwave-flash') || vName.match(/\.(swf)$/i);
            },
            object: function () {
                return true;
            },
            other: function () {
                return true;
            }
        },
        isEmpty = function (value, trim) {
            return value === null || value === undefined || value.length === 0 || (trim && $.trim(value) === '');
        },
        isArray = function (a) {
            return Array.isArray(a) || Object.prototype.toString.call(a) === '[object Array]';
        },
        isSet = function (needle, haystack) {
            return (typeof haystack === 'object' && needle in haystack);
        },
        getElement = function (options, param, value) {
            return (isEmpty(options) || isEmpty(options[param])) ? value : $(options[param]);
        },
        uniqId = function () {
            return Math.round(new Date().getTime() + (Math.random() * 100));
        },
        htmlEncode = function (str) {
            return String(str).repl('&', '&amp;')
                .repl('"', '&quot;')
                .repl("'", '&#39;')
                .repl('<', '&lt;')
                .repl('>', '&gt;');
        },
        replaceTags = function (str, tags) {
            var out = str;
            tags = tags || {};
            $.each(tags, function (key, value) {
                if (typeof value === "function") {
                    value = value();
                }
                out = out.repl(key, value);
            });
            return out;
        },
        objUrl = window.URL || window.webkitURL,
        FileInput = function (element, options) {
            var self = this;
            self.$element = $(element);
            if (!self.validate()) {
                return;
            }
            self.isPreviewable = hasFileAPISupport();
            self.isIE9 = isIE(9);
            self.isIE10 = isIE(10);
            if (self.isPreviewable || self.isIE9) {
                self.init(options);
                self.listen();
            } else {
                self.$element.removeClass('file-loading');
            }
        };

    FileInput.prototype = {
        constructor: FileInput,
        validate: function () {
            var self = this, $exception;
            if (self.$element.attr('type') === 'file') {
                return true;
            }
            $exception = '<div class="help-block alert alert-warning">' +
            '<h4>Invalid Input Type</h4>' +
            'You must set an input <code>type = file</code> for <b>bootstrap-fileinput</b> plugin to initialize.' +
            '</div>';
            self.$element.after($exception);
            return false;
        },
        init: function (options) {
            var self = this, $el = self.$element, t;
            $.each(options, function (key, value) {
                self[key] = (key === 'maxFileCount' || key === 'maxFileSize') ? getNum(value) : value;
            });
            self.fileInputCleared = false;
            self.fileBatchCompleted = true;
            if (isEmpty(self.allowedPreviewTypes)) {
                self.allowedPreviewTypes = defaultPreviewTypes;
            }
            if (!self.isPreviewable) {
                self.showPreview = false;
            }
            self.uploadFileAttr = !isEmpty($el.attr('name')) ? $el.attr('name') : 'file_data';
            self.reader = null;
            self.formdata = {};
            self.filestack = [];
            self.ajaxRequests = [];
            self.isError = false;
            self.ajaxAborted = false;
            self.dropZoneEnabled = hasDragDropSupport() && self.dropZoneEnabled;
            self.isDisabled = self.$element.attr('disabled') || self.$element.attr('readonly');
            self.isUploadable = hasFileUploadSupport() && !isEmpty(self.uploadUrl);
            self.slug = typeof options.slugCallback === "function" ? options.slugCallback : self.slugDefault;
            self.mainTemplate = self.showCaption ? self.getLayoutTemplate('main1') : self.getLayoutTemplate('main2');
            self.captionTemplate = self.getLayoutTemplate('caption');
            self.previewGenericTemplate = self.getPreviewTemplate('generic');
            if (isEmpty(self.$element.attr('id'))) {
                self.$element.attr('id', uniqId());
            }
            if (self.$container === undefined) {
                self.$container = self.createContainer();
            } else {
                self.refreshContainer();
            }
            self.$progress = self.$container.find('.kv-upload-progress');
            self.$btnUpload = self.$container.find('.kv-fileinput-upload');
            self.$captionContainer = getElement(options, 'elCaptionContainer', self.$container.find('.file-caption'));
            self.$caption = getElement(options, 'elCaptionText', self.$container.find('.file-caption-name'));
            self.$previewContainer = getElement(options, 'elPreviewContainer', self.$container.find('.file-preview'));
            self.$preview = getElement(options, 'elPreviewImage', self.$container.find('.file-preview-thumbnails'));
            self.$previewStatus = getElement(options, 'elPreviewStatus', self.$container.find('.file-preview-status'));
            self.$errorContainer = getElement(options, 'elErrorContainer',
                self.$previewContainer.find('.kv-fileinput-error'));
            if (!isEmpty(self.msgErrorClass)) {
                addCss(self.$errorContainer, self.msgErrorClass);
            }
            self.$errorContainer.hide();
            self.fileActionSettings = $.extend(defaultFileActionSettings, options.fileActionSettings);
            self.previewInitId = "preview-" + uniqId();
            self.id = self.$element.attr('id');
            previewCache.init(self);
            self.initPreview(true);
            self.initPreviewDeletes();
            self.options = options;
            self.setFileDropZoneTitle();
            self.uploadCount = 0;
            self.uploadPercent = 0;
            self.$element.removeClass('file-loading');
            t = self.getLayoutTemplate('progress');
            self.progressTemplate = t.replace('{class}', self.progressClass);
            self.progressCompleteTemplate = t.replace('{class}', self.progressCompleteClass);
            self.setEllipsis();
        },
        parseError: function (jqXHR, errorThrown, fileName) {
            var self = this, errMsg = $.trim(errorThrown + ''),
                dot = errMsg.slice(-1) === '.' ? '' : '.',
                text = jqXHR.responseJSON !== undefined && jqXHR.responseJSON.error !== undefined
                    ? jqXHR.responseJSON.error : jqXHR.responseText;
            if (self.showAjaxErrorDetails) {
                text = $.trim(text.replace(/\n\s*\n/g, '\n'));
                text = text.length > 0 ? '<pre>' + text + '</pre>' : '';
                errMsg += dot + text;
            } else {
                errMsg += dot;
            }
            return fileName ? '<b>' + fileName + ': </b>' + jqXHR : errMsg;
        },
        raise: function (event, params) {
            var self = this, e = $.Event(event);
            if (params !== undefined) {
                self.$element.trigger(e, params);
            } else {
                self.$element.trigger(e);
            }
            if (!e.result) {
                return e.result;
            }
            switch (event) {
                // ignore these events
                case 'filebatchuploadcomplete':
                case 'filebatchuploadsuccess':
                case 'fileuploaded':
                case 'fileclear':
                case 'filecleared':
                case 'filereset':
                case 'fileerror':
                case 'filefoldererror':
                case 'fileuploaderror':
                case 'filebatchuploaderror':
                case 'filedeleteerror':
                case 'filecustomerror':
                case 'filesuccessremove':
                    break;
                // receive data response via `filecustomerror` event`
                default:
                    self.ajaxAborted = e.result;
                    break;
            }
            return true;
        },
        getLayoutTemplate: function (t) {
            var self = this,
                template = isSet(t, self.layoutTemplates) ? self.layoutTemplates[t] : defaultLayoutTemplates[t];
            if (isEmpty(self.customLayoutTags)) {
                return template;
            }
            return replaceTags(template, self.customLayoutTags);
        },
        getPreviewTemplate: function (t) {
            var self = this,
                template = isSet(t, self.previewTemplates) ? self.previewTemplates[t] : defaultPreviewTemplates[t];
            template = template.repl('{previewFileIcon}', self.previewFileIcon);
            if (isEmpty(self.customPreviewTags)) {
                return template;
            }
            return replaceTags(template, self.customPreviewTags);
        },
        getOutData: function (jqXHR, responseData, filesData) {
            var self = this;
            jqXHR = jqXHR || {};
            responseData = responseData || {};
            filesData = filesData || self.filestack.slice(0) || {};
            return {
                form: self.formdata,
                files: filesData,
                extra: self.getExtraData(),
                response: responseData,
                reader: self.reader,
                jqXHR: jqXHR
            };
        },
        setEllipsis: function () {
            var self = this, $capCont = self.$captionContainer, $cap = self.$caption,
                $div = $cap.clone().css('height', 'auto').hide();
            $capCont.parent().before($div);
            $capCont.removeClass('kv-has-ellipsis');
            if ($div.outerWidth() > $cap.outerWidth()) {
                $capCont.addClass('kv-has-ellipsis');
            }
            $div.remove();
        },
        listen: function () {
            var self = this, $el = self.$element, $cap = self.$captionContainer, $btnFile = self.$btnFile,
                $form = $el.closest('form');
            $el.on('change', $.proxy(self.change, self));
            $(window).on('resize', function () {
                self.setEllipsis();
            });
            $btnFile.off('click').on('click', function () {
                self.raise('filebrowse');
                if (self.isError && !self.isUploadable) {
                    self.clear();
                }
                $cap.focus();
            });
            $form.off('reset').on('reset', $.proxy(self.reset, self));
            self.$container.off('click')
                .on('click', '.fileinput-remove:not([disabled])', $.proxy(self.clear, self))
                .on('click', '.fileinput-cancel', $.proxy(self.cancel, self));
            if (self.isUploadable && self.dropZoneEnabled && self.showPreview) {
                self.initDragDrop();
            }
            if (!self.isUploadable) {
                $form.on('submit', $.proxy(self.submitForm, self));
            }
            self.$container.find('.kv-fileinput-upload').off('click').on('click', function (e) {
                var $btn = $(this), $form, isEnabled = !$btn.hasClass('disabled') && isEmpty($btn.attr('disabled'));
                if (!self.isUploadable) {
                    if (isEnabled && $btn.attr('type') !== 'submit') {
                        $form = $btn.closest('form');
                        // downgrade to normal form submit if possible
                        if ($form.length) {
                            $form.trigger('submit');
                        }
                        e.preventDefault();
                    }
                    return;
                }
                e.preventDefault();
                if (isEnabled) {
                    self.upload();
                }
            });
        },
        submitForm: function () {
            var self = this, $el = self.$element, files = $el.get(0).files;
            if (files && files.length < self.minFileCount && self.minFileCount > 0) {
                self.noFilesError({});
                return false;
            }
            return !self.abort({});
        },
        abort: function (params) {
            var self = this, data;
            if (self.ajaxAborted && typeof self.ajaxAborted === "object" && self.ajaxAborted.message !== undefined) {
                data = $.extend(self.getOutData(), params);
                data.abortData = self.ajaxAborted.data || {};
                data.abortMessage = self.ajaxAborted.message;
                self.showUploadError(self.ajaxAborted.message, data, 'filecustomerror');
                return true;
            }
            return false;
        },
        noFilesError: function (params) {
            var self = this, label = self.minFileCount > 1 ? self.filePlural : self.fileSingle,
                msg = self.msgFilesTooLess.replace('{n}', self.minFileCount).replace('{files}', label),
                $error = self.$errorContainer;
            $error.html(msg);
            self.isError = true;
            self.updateFileDetails(0);
            $error.fadeIn(800);
            self.raise('fileerror', [params]);
            self.clearFileInput();
            addCss(self.$container, 'has-error');
        },
        setProgress: function (p) {
            var self = this, pct = Math.min(p, 100),
                template = pct < 100 ? self.progressTemplate : self.progressCompleteTemplate;
            if (!isEmpty(template)) {
                self.$progress.html(template.repl('{percent}', pct));
            }
        },
        upload: function () {
            var self = this, totLen = self.getFileStack().length, params = {},
                i, outData, len, hasExtraData = !$.isEmptyObject(self.getExtraData());
            if (totLen < self.minFileCount && self.minFileCount > 0) {
                self.noFilesError(params);
                return;
            }
            if (!self.isUploadable || self.isDisabled || (totLen === 0 && !hasExtraData)) {
                return;
            }
            self.resetUpload();
            self.$progress.removeClass('hide');
            self.uploadCount = 0;
            self.uploadPercent = 0;
            self.lock();
            self.setProgress(0);
            if (totLen === 0 && hasExtraData) {
                self.uploadExtraOnly();
                return;
            }
            len = sel