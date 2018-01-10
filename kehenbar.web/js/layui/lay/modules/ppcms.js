layui.define("jquery", function (exports) {
    var $ = layui.jquery;

    var obj = {
        //提交数据到服务器
        send: function (url, o, func) {            
            url = url || "";
            o = o || {};
            func = func || function (d) { };

            $.ajax({
                url: url,
                type: "Post",
                async:false,
                data: o,
                success: function (data) {
                    func(data);
                },
                error: function (data) {
                    func(data);
                }
            })
        }
        //异步解析模版
        , loadByTemp: function (templist,callback) {
        
            var iframe = $('<iframe id="kehenbar-ajax-load" name="ajaxLoad" style="display:none"></iframe>');
            $("#kehenbar-ajax-load")[0] || $("body").append(iframe);

            var myform = $("<form target='ajaxLoad'></form>");
            myform.attr("action", "/database/AjaxLoad")
                .append("<textarea name='content'>" + templist + "</textarea>")
                .submit();
            
            var iframe = $("#kehenbar-ajax-load"), timer = setInterval(function () {
                var res;
                try {
                    res = iframe.contents().find('body').find("textarea").text();
                } catch (e) {
                    layer.msg('网络错误');
                    clearInterval(timer);
                }
                if (res) {
                    clearInterval(timer);
                    iframe.contents().find('body').html('');
                    
                    callback(res);
                }
            }, 30);
        }
        //自定义按钮事件-保存
        , buttonFuncSave: function (o, callback) {
            obj.send("/custom/save", o, function (res) {
                
                callback(res);
            })
        }
        //自定义按钮事件-新增
        , buttonFuncInsert: function (o, callback) {
            obj.send("/custom/insert", o, function (res) {

                callback(res);
            })
        }
        //自定义按钮事件-删除
        , buttonFuncDelete: function (o, callback) {
            obj.send("/custom/delete", o, function (res) {

                callback(res);
            })
        }
    };

    //输出test接口
    exports('kehenbar', obj);
});