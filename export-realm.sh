#!/bin/bash

CONTAINER_NAME=keycloak
EXPORT_DIR=/opt/keycloak/data/import
LOCAL_EXPORT_DIR=./keycloak-export
REALM_NAME=vidmark

mkdir -p $LOCAL_EXPORT_DIR

echo "Exporting realm $REALM_NAME from container $CONTAINER_NAME..."

docker exec $CONTAINER_NAME /opt/keycloak/bin/kc.sh export --dir $EXPORT_DIR --realm $REALM_NAME --users realm_file

docker cp $CONTAINER_NAME:$EXPORT_DIR $LOCAL_EXPORT_DIR

echo "Exported realm saved to $LOCAL_EXPORT_DIR"