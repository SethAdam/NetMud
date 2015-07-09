﻿@Code
    ViewData("Title") = "Game Client"
End Code

<div class="textParserClient">
    <div id="OutputArea" class="parserOutput">
    </div>
    <input type="text" id="input" name="input" class="parserInput" />
</div>
@Styles.Render("~/Content/GameClient.css")
@Scripts.Render("~/Scripts/GameClient.js")