<#import "template.ftl" as layout>
<@layout.mainLayout active='account' bodyClass='user'; section>

    <header id="kc-header" class="videomark-header">
        <div class="kc-logo-text">VidMark</div>
        <p>Video Labeling Platform</p>
    </header>

    <div class="row">
        <div class="col-md-10">
            <h2>${msg("editAccountHtmlTitle")}</h2>
        </div>
    </div>

    <form action="${url.accountUrl}" class="videomark-form" method="post">
        <input type="hidden" id="stateChecker" name="stateChecker" value="${stateChecker}">

        <div class="form-group">
            <div class="form-group ${messagesPerField.printIfExists('username','has-error')}">
                <label for="username" class="control-label">${msg("username")}</label>
                <input type="text" class="form-control" id="username" name="username" <#if !realm.editUsernameAllowed>disabled="disabled"</#if> value="${(account.username!'')}"/>
            </div>
        </div>

        <div class="form-group ${messagesPerField.printIfExists('email','has-error')}">
            <label for="email" class="control-label">${msg("email")}</label>
            <input type="text" class="form-control" id="email" name="email" value="${(account.email!'')}"/>
        </div>

        <div class="form-group ${messagesPerField.printIfExists('firstName','has-error')}">
            <label for="firstName" class="control-label">${msg("firstName")}</label>
            <input type="text" class="form-control" id="firstName" name="firstName" value="${(account.firstName!'')}"/>
        </div>

        <div class="form-group ${messagesPerField.printIfExists('lastName','has-error')}">
            <label for="lastName" class="control-label">${msg("lastName")}</label>
            <input type="text" class="form-control" id="lastName" name="lastName" value="${(account.lastName!'')}"/>
        </div>

        <div class="form-group">
            <div id="kc-form-buttons" class="submit">
                <div class="">
                    <#if url.referrerURI??><a href="${url.referrerURI}" class="btn-link">${kcSanitize(msg("backToApplication")?no_esc)}</a></#if>
                    <button type="submit" class="btn-primary" name="submitAction" value="Save">${msg("doSave")}</button>
                    <button type="submit" class="btn-default" name="submitAction" value="Cancel">${msg("doCancel")}</button>
                </div>
            </div>
        </div>
    </form>

</@layout.mainLayout>
