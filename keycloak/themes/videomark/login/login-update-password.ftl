<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('password','password-confirm'); section>
    <#if section = "header">
        <header id="kc-header" class="videomark-header">
            <div class="kc-logo-text">VidMark</div>
            <p>Video Labeling Platform</p>
        </header>
    <#elseif section = "form">
        <main id="kc-content" class="videomark-content">
            <div id="kc-form">
                <div id="kc-form-wrapper">
                    <div class="card-content">
                        <form id="kc-passwd-update-form" class="videomark-form" action="${url.loginAction}" method="post">
                            <input type="text" id="username" name="username" value="${username}" autocomplete="username"
                                   readonly="readonly" style="display:none;"/>
                            <input type="password" id="password" name="password" autocomplete="current-password" style="display:none;"/>

                            <div class="${properties.kcFormGroupClass!} form-group">
                                <div class="${properties.kcLabelWrapperClass!}">
                                    <label for="password-new" class="${properties.kcLabelClass!} control-label">${msg("passwordNew")}*</label>
                                </div>
                                <div class="${properties.kcInputWrapperClass!}">
                                    <div class="${properties.kcInputGroup!}">
                                        <input type="password" id="password-new" name="password-new" class="${properties.kcInputClass!} form-control"
                                               autofocus autocomplete="new-password"
                                               aria-invalid="<#if messagesPerField.existsError('password','password-confirm')>true</#if>"
                                        />
                                        <button class="${properties.kcFormPasswordVisibilityButtonClass!}" type="button" aria-label="${msg('showPassword')}"
                                                aria-controls="password-new" data-password-toggle
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
                                    <label for="password-confirm" class="${properties.kcLabelClass!} control-label">${msg("passwordConfirm")}*</label>
                                </div>
                                <div class="${properties.kcInputWrapperClass!}">
                                    <div class="${properties.kcInputGroup!}">
                                        <input type="password" id="password-confirm" name="password-confirm" class="${properties.kcInputClass!} form-control"
                                               autocomplete="new-password"
                                               aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
                                        />
                                        <button class="${properties.kcFormPasswordVisibilityButtonClass!}" type="button" aria-label="${msg('showPassword')}"
                                                aria-controls="password-confirm" data-password-toggle
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

                            <div class="${properties.kcFormGroupClass!}">
                                <div id="kc-form-buttons" class="${properties.kcFormButtonsClass!} form-group">
                                    <#if isAppInitiatedAction??>
                                        <input class="${properties.kcButtonClass!} btn-primary" type="submit" value="${msg("doSubmit")}" />
                                        <button class="${properties.kcButtonClass!} ${properties.kcButtonDefaultClass!} btn-secondary" type="submit" name="cancel-aia" value="true" />${msg("doCancel")}</button>
                                    <#else>
                                        <input class="${properties.kcButtonClass!} btn-primary" type="submit" value="${msg("doSubmit")}" />
                                    </#if>
                                </div>
                            </div>

                            <div class="${properties.kcFormGroupClass!}">
                                <div class="required-fields-info">
                                    <span class="required-field-text">* ${msg("requiredFields")}</span>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </main>
        <script type="module" src="${url.resourcesPath}/js/passwordVisibility.js"></script>
    </#if>
</@layout.registrationLayout>
