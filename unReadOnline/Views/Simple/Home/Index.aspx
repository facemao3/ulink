<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="unReadOnline.Views.Simple.Home.Index" %>
<%@ Import Namespace="unReadOnline.Models.Entitys" %>
<%@ Import Namespace="unReadOnline.ViewsData" %>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>这是在Simple主题下的模板</h2>
    <div class="uLinks list">
        这里什么都没有，只是一个测试修改ViewLocator来动态切换模板的文件。<br /><br /><br />
        <a href="http://QLeelulu.cnblogs.com">我的博客</a><br />
        <a href="http://code.google.com/p/ulink/downloads/list">本站源码下载</a>(改来改去的，代码很乱的哦...)
        <p>
            ASP.NET MVC QQ交流群：1215279
        </p>
    </div>
    
</asp:Content>
