<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="unReadOnline.Views.My.Index" %>
<%@ Import Namespace="unReadOnline.Models.Entitys" %>
<%@ Import Namespace="unReadOnline.ViewsData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>我的uLinks</h2>
    <% BaseUnReadLinkData blData = (BaseUnReadLinkData)ViewData.Model; %>
    <div class="uLinks list">
        <% foreach (UnReadLink unReadLink in blData.UnReadLinks)
           { %>
                <div class="linkItem">
                    <h4 class="linkTitle"><a class="linkA" href="<%= unReadLink.Url %>" target="_blank"><%= unReadLink.Title %></a></h4>
                    <div class="itemInfo">
                        <div class="linkDes">
                            <%= string.IsNullOrEmpty(unReadLink.Description) ? unReadLink.Title : unReadLink.Description %>
                        </div>
                        <p class="linkFoot">
                            <%= unReadLink.DateCreated.ToString("MM-dd HH:mm:ss") %>&nbsp;
                            <a class="linkDel" href="MyULink.mvc/Delete/<%= unReadLink.Id %>" onclick="return confirm('你确定要删除该项么？');">删除</a>
                            <a class="linkEdit" href="MyULink.mvc/Edit/<%= unReadLink.Id %>">编辑</a>
                        </p>
                    </div>
                </div>
        <% } %>
    </div>
</asp:Content>
