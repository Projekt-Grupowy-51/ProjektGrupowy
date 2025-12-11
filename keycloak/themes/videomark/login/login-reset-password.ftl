<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=true displayMessage=!messagesPerField.existsError('username'); section>
    <#if section = "header">
        <header id="kc-header" class="videomark-header">
            <div class="kc-logo-text">VidMark</div>
            <p>Video Labeling Platform</p>
        </header>
    <#elseif section = "form">
        <main id="kc-content" class="videomark-content">
            <div id="kc-form">
                <div id="kc-form-wrapper">
                    <form id="kc-reset-password-form" class="videomark-form" action="${url.loginAction}" method="post">
                        <div class="${properties.kcFormGroupClass!} form-group">
                            <div class="${properties.kcLabelWrapperClass!}">
                                <label for="username" class="${properties.kcLabelClass!} control-label">
                                    <#if !realm.loginWithEmailAllowed>
                                        ${msg("username")}
                                    <#elseif !realm.registrationEmailAsUsername>
                                        ${msg("usernameOrEmail")}
                                    <#else>
                                        ${msg("email")}
                                    </#if>
                                </label>
                            </div>
                            <div class="${properties.kcInputWrapperClass!}">
                                <div class="${properties.kcInputGroup!}">
                                    <input type="text" id="username" name="username" class="${properties.kcInputClass!} form-control" 
                                           autofocus value="${(auth.attemptedUsername!'')}" 
                                           autocomplete="username"
                                           aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                                    />
                                </div>
                                <#if messagesPerField.existsError('username')>
                                    <span id="input-error-username" class="${properties.kcInputErrorMessageClass!}" aria-live="polite">
                                        ${kcSanitize(messagesPerField.get('username'))?no_esc}
                                    </span>
                                </#if>
                            </div>
                        </div>

                        <div class="${properties.kcFormGroupClass!}">
                            <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!} form-group">
                                <input class="btn-primary" type="submit" value="${msg("doSubmit")}" id="kc-login"/>
                            </div>
                        </div>
                        <div class="${properties.kcFormGroupClass!}">
                            <div id="kc-info" class="login-pf-signup">
                                <span><a href="${url.loginUrl}">${kcSanitize(msg("backToLogin"))?no_esc}</a></span>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </main>
    <#elseif section = "info">
        <div class="alert alert-info">
            <#if realm.duplicateEmailsAllowed>
                ${msg("emailInstructionUsername")}
            <#else>
                ${msg("emailInstruction")}
            </#if>
        </div>
    </#if>
</@layout.registrationLayout>
