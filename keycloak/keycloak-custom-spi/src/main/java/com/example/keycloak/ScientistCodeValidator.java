package com.example.keycloak;

import org.keycloak.validate.Validator;
import org.keycloak.validate.ValidationContext;
import org.keycloak.validate.ValidationError;
import org.keycloak.validate.ValidatorConfig;

import java.util.List;

public class ScientistCodeValidator implements Validator {
    public static final String ID = "scientist-code-validator";
    private boolean isScientist = false;

    public String getId() { return ID; }

    @Override
    public ValidationContext validate(Object value, String inputHint, ValidationContext context, ValidatorConfig config) {
        if ("Role".equals(inputHint)) {
            String roleValue = null;
            if (value instanceof String) roleValue = (String) value;
            else if (value instanceof String[]) roleValue = ((String[]) value)[0];
            else if (value instanceof List<?> list && !list.isEmpty()) roleValue = list.get(0).toString();

            isScientist = "Scientist".equals(roleValue);
        }

        if ("scientistCode".equals(inputHint)) {
                String code = null;
                if (value instanceof String) code = (String) value;
                else if (value instanceof String[]) code = ((String[]) value)[0];
                else if (value instanceof List<?> list && !list.isEmpty()) code = list.get(0).toString();
        
                if (isScientist) {
                    String expectedCode = System.getenv("SCIENTIST_CODE");
                    if (code == null || !code.equals(expectedCode)) {
                        context.addError(new ValidationError(ID, inputHint, "Scientist code is invalid."));
                    }
                } else if (code != null && !code.isEmpty()) {
                    context.addError(new ValidationError(ID, inputHint, "Only scientists can enter a scientist code."));
                }
            }

        return context;
    }
}
