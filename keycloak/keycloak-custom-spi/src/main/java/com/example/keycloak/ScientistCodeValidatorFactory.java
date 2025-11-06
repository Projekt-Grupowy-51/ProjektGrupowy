package com.example.keycloak;

import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;
import org.keycloak.validate.Validator;
import org.keycloak.validate.ValidatorFactory;

public class ScientistCodeValidatorFactory implements ValidatorFactory {

    @Override
    public Validator create(KeycloakSession session) {
        return new ScientistCodeValidator();
    }

    @Override
    public void init(org.keycloak.Config.Scope config) {
        // no-op (safe to use FQN)
    }

    @Override
    public void postInit(KeycloakSessionFactory factory) {
        // no-op
    }

    @Override
    public void close() {
        // no-op
    }

    @Override
    public String getId() {
        return ScientistCodeValidator.ID;
    }
}
