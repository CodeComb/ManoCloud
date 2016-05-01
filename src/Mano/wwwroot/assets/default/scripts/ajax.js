$(function(){
    //用来异步修改自我介绍
    $("#introduction-form").submit(function(){
        $.post("/Account/Introduction", 
            $(this).serialize(),
            function(data){
                createTipBox(data);
            });
        return false;
    });
    //异步返回编辑模态框,并且绑定异步提交事件
    $(".ajax-edit").click(function(){
        var editUrl=$(this).attr("editUrl");
        var modalId=$(this).attr("modalId");
        $.get(
            editUrl,
            {},
            function(data){
                if(data==="error")
                {
                    createTipBox("数据出错");
                }else{
                    data+="<script></script>"
                    $("#"+modalId).find(".modal-content").html(data);
                    $("#"+modalId).modal("show");
                }
            });
    });
});
//异步编辑提交
function editSubmit(){
     var editUrl=$(".ajax-edit").attr("editUrl");
        var modalId=$(".ajax-edit").attr("modalId");
    var id=$("#ajax-edit-form").find("input[name='id']").val();
            var getUrl=$("#ajax-edit-form").find("input[name='id']").attr("getUrl");
            $.post(editUrl,
            $("#ajax-edit-form").serialize(),
                function(data){
                    if(data==="ok")
                    {
                        $("#"+modalId).modal("hide");
                        createTipBox("编辑成功");
                        replaceTbody(id,getUrl,modalId);
                    }else{
                        $("#ajax-edit-form").find("#ajax-error-message").html(data);
                    }
                });
}
//异步添加提交
function ajaxSubmit(){
    var _this=$("#ajax-add-form");
     $(this).find("#ajax-error-message").html('');
        var postUrl=$(_this).find("input[name='id']").attr("postUrl");
        var getUrl=$(_this).find("input[name='id']").attr("getUrl");
        var modalId=$(_this).find("input[name='id']").attr("modalId");
        addAjax(_this,postUrl,getUrl,modalId);
}
//异步删除
function deleteItem(postUrl,id,pid)
{
    $.post(postUrl,
        {id:+id,pid:pid},
        function(data){
            if(data==="ok"){
                createTipBox("删除成功");
            }
            else{
                createTipBox(data);
            }
        });
}
//异步添加记录方法
function addAjax(_this,postUrl,getUrl,modalId){
    var id=$(_this).find("input[name='id']").val();
    $.post(postUrl,
    $(_this).serialize(),
    function(data){
        if(data==='ok')
        {
            $("#"+modalId).modal("hide"); 
            createTipBox("添加成功"); 
            replaceTbody(id,getUrl,modalId);
        }else{
            $(_this).find("#ajax-error-message").html(data);
        }
    });
}
//异步获得视图,并且替换Tbody
function replaceTbody(id,getUrl,modalId){
    $.get(getUrl,
       {id:+id},
       function(data){
          $("#ajaxTbody").html(data);
       });
}
//用来生成提示框
    function createTipBox(data){
       $(".submit-tip span").html(data);
       $(".submit-tip").stop(true,true).css("display","block").animate({ 
            opacity:0
        }, 3000,function(){
            $(this).css({
                "opacity":1,
                "display":'none'
            });
        });   
    }
    //生成删除确定框
    function deleteDialog(postUrl,id,pid) {
    var html = '<div class="message-bg"></div><div class="message-outer"><div class="message-container">' +
        '<h3>提示信息</h3>' +
        '<p>点击删除按钮后，该记录将被永久删除，您确定要这样做吗？</p>' +
        '<div class="message-buttons"><a href="javascript:deleteItem(\''+postUrl+'\',\''+id+'\',\''+pid+'\');deleteRow(\'' + pid + '\')" class="btn btn-danger">删除</a> <a href="javascript:closeDialog()" class="btn">取消</a></div>' +
        '</div></div>';
    var dom = $(html);
    $('body').append(dom);
    $(".message-outer").css('margin-top', -($(".message-outer").outerHeight() / 2));
    setTimeout(function () { $(".message-bg").addClass('active'); $(".message-outer").addClass('active'); }, 10);
    }
    //取消删除
    function closeDialog() {
    $('.message-bg').removeClass('active');
    $('.message-outer').removeClass('active');
    setTimeout(function () {
        $('.message-bg').remove();
        $('.message-outer').remove();
    }, 200);
    }
    //删除行
    function deleteRow(id)
    {
    $('#' + id).remove();
    closeDialog();
    }