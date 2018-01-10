; (function ($) {
    
    var Upload = function (ele, opt, callback, err) {

        var defaults = {
            action: ""
            , method: "post"
            , enctype: "multipart/form-data"
        }

        this.element = ele;
        this.callback = callback;
        this.err = err;
        this.options = $.extend({}, defaults, opt);
    };

    Upload.prototype.init = function () {

        var that = this;
        upForm.call(that, that);
    };

    var upForm = function (obj) {

        obj.attr = obj.attr || {};
        var iframe = $("<iframe id='file_upload_iframe' name='file_upload_iframe' style='display:none'></iframe>");
        $("#file_upload_iframe")[0] || $('body').append(iframe);

        var form = $("<form target='file_upload_iframe'></form>");
        form.attr(obj.options)
            .append("<input type='file' name='fileData' class='file-upload-input'/>")
            .append("<input type='button' class='file-upload-btn' value='上传'>")
            .appendTo(obj.element)
            .find(".file-upload-btn").on("click", function () {
                
                $(this).parent().submit();
                backMsg.call(obj.element, $("#file_upload_iframe"), obj);
            })
    }
    , backMsg = function (iframe,obj) {
        var res;
        var timer = setInterval(function () {
            
            try {
                res = iframe.contents().find('body').text();
                typeof obj.callback === "function" && obj.callback(res);
            } catch (e) {      
                typeof obj.err ==="function" && obj.err("上传接口存在跨域");
                clearInterval(timer);
            }
            if (res) {
                clearInterval(timer);
                iframe.contents().find('body').html('');
                try {
                    res = JSON.parse(res);
                    typeof obj.callback === "function" && obj.callback(res);
                } catch (e) {
                    typeof obj.err === "function" && obj.err("请对上传接口返回JSON字符");
                }
            }
        }, 30)
    };


    $.fn.upload = function (opt,callback,err) {

        var _upload = new Upload(this, opt, callback, err);
        _upload.init();
    }

})(jQuery)