SitecoreExtension-DynamicKeyPlaceholder
=======================================

SitecoreExtension for adding DynamicKeyPlaceholders allowing for multiple spots with placeholders on one page.

Using the dynamic spot:

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Spots.ascx.cs" Inherits="Commodities.Web.UI.Spots.Containers.Spots" %>
<%@ Register TagPrefix="sce" Namespace="SitecoreExtension.DynamicKeyPlaceholder.Controls" Assembly="SitecoreExtension.DynamicKeyPlaceholder" %>
<div class="container container-white">
    <div class="container-inner">
        <div class="spots">
            <sce:DynamicKeyPlaceholder runat="server" ID="SpotsPlaceholder" Key="SpotsPlaceholder" editable="true" />
        </div>
    </div>
</div>
