<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="unReadOnline.Views.Home.Index" %>
<%@ Import Namespace="unReadOnline.Models.Entitys" %>
<%@ Import Namespace="unReadOnline.ViewsData" %>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>所有uLinks</h2>
    <% BaseUnReadLinkData blData = (BaseUnReadLinkData)ViewData.Model; %>
    <a class="toggleAllDes" href="javascript:void(0)" >折叠全部</a>
    <div class="uLinks list">
        <% bool isAdmin = Roles.IsUserInRole("Administrators"); 
           foreach (UnReadLink unReadLink in blData.UnReadLinks)
           { %>
                <div class="linkItem">
                    <h4 class="linkTitle"><a class="linkA" href="<%= unReadLink.Url %>" target="_blank"><%= unReadLink.Title %></a></h4>
                    <div class="itemInfo">
                        <div class="linkDes">
                            <%= string.IsNullOrEmpty(unReadLink.Description) ? unReadLink.Title : unReadLink.Description %>
                        </div>
                        <p class="linkFoot">
                            <%= unReadLink.DateCreated.ToString("MM-dd HH:mm:ss") %>
                            <% if (isAdmin)
                               { %>
                                &nbsp;
                                <a class="linkDel" href="Manage.mvc/DelULink/<%= unReadLink.Id %>" onclick="return confirm('你确定要删除该项么？');">删除</a>
                                <a class="linkEdit" href="Manage.mvc/EditULink/<%= unReadLink.Id %>">编辑</a>
                            <% } %>
                        </p>
                    </div>
                </div>
        <% } %>
    </div>
    <div class="uLinks pager">
        <a href="javascript:void(0)">首页</a>
        <a href="javascript:void(0)">前一页</a>
        当前第<span class="uLinks pageNum"><%= blData.CurrentPage.ToString() %></span>页&nbsp;
        共<span class="uLinks pageCount"><%= blData.PageCount %></span>页
        <a href="javascript:void(0)">后一页</a>
        <a href="javascript:void(0)">尾页</a>
    </div>
</asp:Content>
