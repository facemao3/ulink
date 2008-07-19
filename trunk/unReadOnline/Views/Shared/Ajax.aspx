<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ajax.aspx.cs" Inherits="unReadOnline.Views.Shared.Ajax" %>
<%
    Response.Clear();
    Response.ContentType = "application/json";
    Response.Write(ViewData.Model.ToJson());   
%>
