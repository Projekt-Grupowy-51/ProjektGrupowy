<#import "template.ftl" as layout>
<#import "user-profile-commons.ftl" as userProfileCommons>
<#import "register-commons.ftl" as registerCommons>
<@layout.registrationLayout displayMessage=messagesPerField.exists('global') displayRequiredFields=false; section>
    <#if section = "header">
        <header id="kc-header" class="videomark-header">
            <div class="kc-logo-text">VidMark</div>
            <p>Video Labeling Platform</p>
        </header>
    <#elseif section = "form">
        <main id="kc-content" class="videomark-content">
            <div id="kc-form">
                <div id="kc-form-wrapper">
                    <form id="kc-register-form" class="videomark-form" action="${url.registrationAction}" method="post">

                        <@userProfileCommons.userProfileFormFields; callback, attribute>
                            <#if callback = "afterField">
                            <#-- render password fields just under the username or email (if used as username) -->
                                <#if passwordRequired?? && (attribute.name == 'username' || (attribute.name == 'email' && realm.registrationEmailAsUsername))>
                                    <div class="${properties.kcFormGroupClass!} form-group">
                                        <div class="${properties.kcLabelWrapperClass!}">
                                            <label for="password" class="${properties.kcLabelClass!} control-label">${msg("password")}*</label>
                                        </div>
                                        <div class="${properties.kcInputWrapperClass!}">
                                            <div class="${properties.kcInputGroup!}">
                                                <input type="password" id="password" class="${properties.kcInputClass!} form-control" name="password"
                                                       autocomplete="new-password"
                                                       aria-invalid="<#if messagesPerField.existsError('password','password-confirm')>true</#if>"
                                                />
                                                <button class="${properties.kcFormPasswordVisibilityButtonClass!}" type="button" aria-label="${msg('showPassword')}"
                                                        aria-controls="password"  data-password-toggle
                                                        data-icon-show="${properties.kcFormPasswordVisibilityIconShow!}" data-icon-hide="${properties.kcFormPasswordVisibilityIconHide!}"
                                                        data-label-show="${msg('showPassword')}" data-label-hide="${msg('hidePassword')}">
                                                    <i class="${properties.kcFormPasswordVisibilityIconShow!}" aria-hidden="true"></i>
                                                </button>
                                            </div>

                                            <#if messagesPerField.existsError('password')>
                                                <span id="input-error-password" class="${properties.kcInputErrorMessageClass!}" aria-live="polite">
		                                        ${kcSanitize(messagesPerField.get('password'))?no_esc}
		                                    </span>
                                            </#if>
                                        </div>
                                    </div>

                                    <div class="${properties.kcFormGroupClass!} form-group">
                                        <div class="${properties.kcLabelWrapperClass!}">
                                            <label for="password-confirm"
                                                   class="${properties.kcLabelClass!} control-label">${msg("passwordConfirm")}*</label> 
                                        </div>
                                        <div class="${properties.kcInputWrapperClass!}">
                                            <div class="${properties.kcInputGroup!}">
                                                <input type="password" id="password-confirm" class="${properties.kcInputClass!} form-control"
                                                       name="password-confirm"
                                                       autocomplete="new-password"
                                                       aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
                                                />
                                                <button class="${properties.kcFormPasswordVisibilityButtonClass!}" type="button" aria-label="${msg('showPassword')}"
                                                        aria-controls="password-confirm"  data-password-toggle
                                                        data-icon-show="${properties.kcFormPasswordVisibilityIconShow!}" data-icon-hide="${properties.kcFormPasswordVisibilityIconHide!}"
                                                        data-label-show="${msg('showPassword')}" data-label-hide="${msg('hidePassword')}">
                                                    <i class="${properties.kcFormPasswordVisibilityIconShow!}" aria-hidden="true"></i>
                                                </button>
                                            </div>

                                            <#if messagesPerField.existsError('password-confirm')>
                                                <span id="input-error-password-confirm" class="${properties.kcInputErrorMessageClass!}" aria-live="polite">
		                                        ${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}
		                                    </span>
                                            </#if>
                                        </div>
                                    </div>
                                </#if>
                            </#if>
                        </@userProfileCommons.userProfileFormFields>

                        <@registerCommons.termsAcceptance/>

                        <#if recaptchaRequired??>
                            <div class="form-group">
                                <div class="${properties.kcInputWrapperClass!}">
                                    <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
                                </div>
                            </div>
                        </#if>

                        <div class="${properties.kcFormGroupClass!}">
                            <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!} form-group">
                                <input class="btn-primary" type="submit" value="${msg("doRegister")}" id="kc-login"/>
                            </div>
                        </div>
                        <div class="${properties.kcFormGroupClass!}">
                            <div id="kc-info" class="login-pf-signup">
                                <span>${msg("Have account?")} <a href="${url.loginUrl}">${msg("doLogIn")}</a></span>
                            </div>
                             <div class="required-fields-info">
                                <span class="required-field-text">* ${msg("requiredFields")}</span>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </main>
        <script type="module" src="${url.resourcesPath}/js/passwordVisibility.js"></script>
        <script src="${url.resourcesPath}/js/kc.profile.roles.js"></script>
    </#if>
</@layout.registrationLayout>
