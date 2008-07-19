<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="unReadOnline.Views.Error.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div style="background-image: url('Views/Images/error.gif'); background-repeat: no-repeat; width: 100%; height: 100%;">

<div style="padding:10px 10px 10px 80px;">
    <p style="padding:10px 80px 10px 150px;">
        <span style="color: #E32E00;margin-left:-10px;"  >出错啦！</span><br />
        <%= ViewData["errorMsg"] %>
    </p>
</div>
<div style="margin-top:50px;">
    <p style="padding:180px 180px 10px;">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        对不起，您正要打开的网页可能已经被删除、更名或暂时不可用。如有疑问，请与我们<a href="http://www.net4.com.cn">联系</a>，或<a runat="server" href="~/">回到网站首页</a>重新选择。 
    </p>
    </div>
</div>
</asp:Content>
