<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Certificate.aspx.cs" Inherits="The_Analytical_Dojo.Certificate" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Certificate | The Analytical Dojo</title>
    <link href="Content/dojo-theme.css" rel="stylesheet" />
    <script src="Scripts/theme.js"></script>
</head>
<body class="dojo cert-page">
    <form id="form1" runat="server">
        <input type="hidden" name="dojoCsrf" value="<%= H(Csrf) %>" />

        <div class="cert-toolbar">
            <a class="text-link" href="StudentMainPage.aspx">← Home</a>
            <div class="hero-actions">
                <asp:Button ID="Download" runat="server" CssClass="btn" Text="↓ Download certificate" OnClick="Download_Click" />
                <button type="button" class="btn ghost" onclick="window.print()">Print / Save PDF</button>
            </div>
        </div>

        <asp:Literal ID="Cert" runat="server" />
    </form>
</body>
</html>
