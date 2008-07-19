/// <reference path="jquery-1.2.3-intellisense.js" />

$(function(){ InitULink(); });
InitULink = function(){
    LinkItem();
    Login();
    Logout();
    LoadRegister();
    Register();
    LoadAdduLinkForm();
    Pager();
};

//登录
Login = function(){
    //显示登录表单
    $(".m.login").click(function(){
        BlockBG(function(){
	        $("#divMembershipBox").css("display","none");
	    });
	    $("#divMembershipBox").css({"display":"block", "top": getViewPortScrollY()+150 });
	    $("#divRegister").css("display","none");
        $("#divLogin").css("display","block");
        $("#txtLoginUserName").focus();
    });
    $("#btnLogin").click(function(){
        if($("#txtLoginUserName").val() == "" || $("#txtLoginPassword").val() == ""){
            ShowMessage("用户名或者密码不能为空，请检查！");
        }else{
            ShowLoading("登录中，请稍后...");
            jQuery.ajax({
                type:"post",
                url:"/PEuLink/User.mvc/Login",
                data:{userName: $("#txtLoginUserName").val(), password: $("#txtLoginPassword").val(), rememberMe: $("#chkLoginRememberMe")[0].checked ? true : false },
                success:function(data){
                    HideLoading();
                    data = parseJson(data);
                    if(data.isSuccessful){
                        ShowMessage("登录成功");
                        document.location.href="/PEuLink/";
                    }else{
                        ShowMessage("用户名或者密码错误，请重新登录");
                    }
                },
                error:function(){
                    HideLoading();
                    ShowMessage("登录失败，请重新登录");
                }
            });
        }
    });
};

//注销
Logout = function(){
    $(".m.logout").click(function(){
        ShowLoading("正在注销，请稍后...");
        jQuery.ajax({
            type:"post",
            url:"/PEuLink/User.mvc/Logout",
            success:function(data){
                HideLoading();
                data = parseJson(data);
                if(data.isSuccessful){
                    ShowMessage("你已成功退出");
                    document.location.href="/PEuLink/";
                }else{
                    ShowMessage("注销失败，请重试");
                }
            },
            error:function(){
                HideLoading();
                ShowMessage("请求服务器失败，请重试");
            }
        });
    });
};

//加载注册表单
LoadRegister = function(){
	$(".m.register").click(function(){
	    var btn = this;
	    btn.disable = false;
	    BlockBG(function(){
	        $("#divMembershipBox").css("display","none");
	        //$(this).remove();
	    });
	    $("#divMembershipBox").css({"display":"block", "top": getViewPortScrollY()+150 });
	    $("#divLogin").css("display","none");
	    $("#divRegister").css("display","block");
	    $(".form #txtUserName").focus();
	});
    $("#divMembershipClose.closeButton").click(function(){
        $("#divMembershipBox").css("display","none");
	    unBlockBG();
    });
};

//提交注册信息
Register = function(){
	$("#btnRegister").click(function(){
	    var userName = $(".form #txtUserName").val();
	    var password = $(".form #txtPassword").val();
	    var confirmPwd = $(".form #txtConfirm").val();
	    var email = $(".form #txtEmail").val();
	    if(userName == "" || password == "" || email == ""){
	        ShowMessage("用户名、密码、Email 不能为空！请检查。");
	        return;
	    }else if(password != confirmPwd){
	        ShowMessage("输入的两次密码不一致，请重新输入！");
	        return;
	    }else if(!ValEmail(email)){
	        ShowMessage("Email地址格式不对，请重新输入！");
	        return;
	    }
	    
	    ShowLoading("向服务器注册中，请稍后...");
		jQuery.ajax({
			url:"/PEuLink/User.mvc/Register",
			type: "post",
			data:{userName:userName, password:password, email:email },
			success:function(data){
			    HideLoading();
			    data = parseJson(data);
			    if(data.isSuccessful){
				    ShowMessage("注册成功,请登录");
				    $("#divMembershipClose.closeButton").click();
				    $(".m.login").click();
				}else{
				    ShowMessage(data.errorMessage);
				}
			},
			error:function(data){
			    HideLoading();
			    ShowMessage("请求服务器失败，请重试");
			}
		});
	});
};
//加载添加uLink表单
LoadAdduLinkForm = function(){
    $(".uLink.Add").click(function(){
        if(!$.cookie("AdduLink")){
            ShowMessage("对不起，匿名用户不可以添加uLink，请先登录或注册！");
            //$(".m.login").click();
        }else{
            ShowLoading("正在加载表单，请稍后...");
            jQuery.ajax({
                type: "get",
                url: "/PEuLink/MyULink.mvc/LoadAddForm",
                cache: true,
                success: function(data){
                    HideLoading();
                    data = parseJson(data);
                    if(data.isSuccessful){
                        $(data.returnHtml).appendTo("body");//将添加uLink表单附加到body中
                        BlockBG(function(){
                            $("#divAdduLink").remove();
                            unBlockBG();
                        });
                        $(".closeButton.AdduLink").click(function(){//为关闭按钮绑定点击事件
                            $("#divAdduLink").remove();
                            unBlockBG();
                        });
                        $("#txtTitle").focus();
                        AdduLink();//为提交按钮绑定点击事件
                    }else{
                        ShowMessage(data.errorMessage);
                    }
                },
                error: function(){
                    HideLoading();
                    ShowMessage("很抱歉，请求出错，请检查你的网络并重试");
                }
            });
        }
    });
};

//添加一个uLink
AdduLink = function(){
    $("#btnAdduLink").click(function(){
        var t=$("#txtTitle").val(),u=$("#txtURL").val(),d=$("#txtDes").val(),c=$("#sltCategory").val(),p=$("#chkPublic")[0].checked,f=$("#chkForever")[0].checked,val=true;
        if($.trim(t)==""){$("#valTitle").html("题目不能为空！").css("display","");val=false;}else{$("#valTitle").css("display","none");}
        if($.trim(u)==""){$("#valURL").html("链接不能为空！").css("display","");val=false;}else{$("#valURL").css("display","none");}
        if(!val){return false;}
        ShowLoading("正在提交表单，请稍后...");
        var pageNum = parseInt($(".uLinks.pageNum").html());
        jQuery.ajax({
            type: "post",
            url: "/PEuLink/MyULink.mvc/Add",
            data: {
                Title:$("#txtTitle").val(), 
                URL:$("#txtURL").val(), 
                Description:$("#txtDes").val(), 
                Category:$("#sltCategory").val(), 
                Public:$("#chkPublic")[0].checked,
                Forever: $("#chkForever")[0].checked
            },
            success: function(data){
                HideLoading();
                data = parseJson(data);
                if(data.isSuccessful){
                    ShowMessage("添加成功！");
                    $("#divAdduLink").remove();
                    unBlockBG();
                    RefreshList("All", pageNum, true);
                }else{
                    ShowMessage("请求出错，原因：" + data.errorMessage);
                }
            },
            error: function(){
                HideLoading();
                ShowMessage("很抱歉，提交出错，请检查你的网络并重试");
            }
        });
    });
};

//异步刷新uLink列表
//rftype为刷新的类型：All:返回所有uLinks；User:返回当前用户的uLinks
//showMsg为刷新的时候是否显示提示信息
RefreshList = function(forwho, pageNum, showMsg){
    if(forwho){
        var url = "/PEuLink/Home.mvc/RefreshList";
    }
    if(forwho && pageNum){
        url = url + "/" + pageNum;
    }else{
        pageNum = 1;
    }
    if(showMsg){
        ShowLoading("正在刷新列表，请稍后...");
    }
    jQuery.ajax({
        type: "get",
        url: url,
        data:{ forwho:forwho },
        success: function(data){
            HideLoading();
            data = parseJson(data);
            if(data.isSuccessful){
                if(showMsg){
                    HideLoading();
                }
                if(data.returnHtml){
                    $(".uLinks.list").html(data.returnHtml);
                    $(".uLinks.pageNum").html(pageNum);
                    $(".uLinks.pageCount").html(data.errorMessage);
                    LinkItem();
                }
            }else{
                if(showMsg){
                    ShowMessage("刷新失败，原因：列表返回失败！");
                }
            }
        },
        error: function(){
            if(showMsg){
                ShowMessage("刷新失败，原因：服务器没有响应！");
            }
        }
        });
};

//分页按钮
Pager = function(){
    var pager = $(".uLinks.pager a");
    var pageNum = parseInt($(".uLinks.pageNum").html());
    var pageCount = parseInt($(".uLinks.pageCount").html());
    pager.eq(0).click(function(){ //第一页
        if(pageNum==1) return false;
        RefreshList("All",1,true);
        pageNum = 1;
        pageCount = parseInt($(".uLinks.pageCount").html());
    });
    pager.eq(1).click(function(){ //前一页
        if(pageNum==1) return false;
        RefreshList("All",pageNum-1,true);
        pageNum--;
        pageCount = parseInt($(".uLinks.pageCount").html());
    });
    pager.eq(2).click(function(){ //后一页
        if(pageNum==pageCount) return false;
        RefreshList("All",pageNum+1,true);
        pageNum++;
        pageCount = parseInt($(".uLinks.pageCount").html());
    });
    pager.eq(3).click(function(){ //最后页
        if(pageNum==pageCount) return false;
        RefreshList("All",pageCount,true);
        pageCount = parseInt($(".uLinks.pageCount").html());
        pageNum = pageCount;
    });
};

LinkItem = function(){
 $(".linkTitle").hover(function(){
    $(this).addClass("linkTitleHover");
 },function(){
    $(this).removeClass("linkTitleHover");
 }).click(function(e){
    if(e.target==this){//防止冒泡，防止子元素触发该事件
        var obj=$(this).next(".itemInfo");
        if(obj.css("display")=="none"){
            obj.css("display","block");
        }else{
            obj.css("display","none");
        }
    }
 });
 $(".toggleAllDes").click(function(){
    if($(this).html()=="折叠全部"){
        $(".itemInfo").css("display","none");
        $(this).html("展开全部");
    }else{
        $(".itemInfo").css("display","block");
        $(this).html("折叠全部");
    }
 });
};

//显示提示信息
var ShowMessage = function(msg){
    if(msg){
        msg = "<p>" + msg + "</p>";
        $(".messages").show();
        $(msg)
            .appendTo(".messages")
            .slideDown("normal")
            .animate({opacity: 1.0}, 3000)
            .slideUp("normal",function(){
                $(this).remove();
                if($(".messages").html()=="")
                    $(".messages").hide();
            });
    }
};

//显示Loading
var ShowLoading = function(msg){
    var loading = $(".loading");
    loading.css({"display":"block","top":getViewPortHeight()/2 + getViewPortScrollY()-150,"left":getViewPortWidth()/2-100});
    if(msg){
        $(".loadingMsg").html(msg);
    }else{
        $(".loadingMsg").html("数据加载中...");
    }
};

//隐藏Loading
var HideLoading = function(){
    $(".loading").css("display","none");
    $(".loadingMsg").html("");
};

//将Json字符串转换为对象
var parseJson = function(jsonStr){
    return eval("(" + jsonStr + ")");    
};

//以下为获取窗口高度、宽度
var getViewPortWidth = function()
{
    var width = 0;

    if ((document.documentElement) && (document.documentElement.clientWidth))
    {
        width = document.documentElement.clientWidth;
    }
    else if ((document.body) && (document.body.clientWidth))
    {
        width = document.body.clientWidth;
    }
    else if (window.innerWidth)
    {
        width = window.innerWidth;
    }

    return width;
};

var getViewPortHeight = function()
{
    var height = 0;

    if (window.innerHeight)
    {
        height = window.innerHeight - 18;
    }
    else if ((document.documentElement) && (document.documentElement.clientHeight))
    {
        height = document.documentElement.clientHeight;
    }

    return height;
};

var getContentHeight = function()
{
    if ((document.body) && (document.body.offsetHeight))
    {
        return document.body.offsetHeight;
    }

    return 0;
};

var getViewPortScrollX = function()
{
    var scrollX = 0;

    if ((document.documentElement) && (document.documentElement.scrollLeft))
    {
        scrollX = document.documentElement.scrollLeft;
    }
    else if ((document.body) && (document.body.scrollLeft))
    {
        scrollX = document.body.scrollLeft;
    }
    else if (window.pageXOffset)
    {
        scrollX = window.pageXOffset;
    }
    else if (window.scrollX)
    {
        scrollX = window.scrollX;
    }

    return scrollX;
};

var getViewPortScrollY = function()
{
    var scrollY = 0;

    if ((document.documentElement) && (document.documentElement.scrollTop))
    {
        scrollY = document.documentElement.scrollTop;
    }
    else if ((document.body) && (document.body.scrollTop))
    {
        scrollY = document.body.scrollTop;
    }
    else if (window.pageYOffset)
    {
        scrollY = window.pageYOffset;
    }
    else if (window.scrollY)
    {
        scrollY = window.scrollY;
    }

    return scrollY;
};//以上为获取窗口高度、宽度

//验证Email
var ValEmail = function(email){
    var regExp = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    return regExp.test(email);
};

//验证不为空
var NotNull = function(tagEle, errorMsg){
    if($(tagEle).val() == "")
    {
        ShowMessage(errorMsg);
    }
};
//页面变暗，不可操作.callback参数为 点击变暗的页面时进行的处理事件
var BlockBG = function(callback){
    $('<div class="blockBG"></div>').appendTo("body");
	$(".blockBG").css({"height":getViewPortHeight(), "width":getViewPortWidth() }).focus();
	$(".blockBG").click(function(){
	    if(callback) callback();
	    unBlockBG();
	});
};
//页面恢复可操作
var unBlockBG = function(){
    $(".blockBG").remove();
};

//为显示的列表偶数行添加样式,从零算起，所以这里用odd(奇数)
evenList = function(){
    $(".linkTitle:odd").css({ "background-color":"#F4FDDD" });
};