resource "azuread_application" "spa" {
  display_name = "${var.app_name}-${var.environment_short}-spa"

  single_page_application {
    redirect_uris = ["https://${local.hostname}/"]
  }
}

resource "azuread_service_principal" "spa" {
  application_id               = azuread_application.spa.application_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]
  login_url                    = "https://${local.hostname}"

  feature_tags {
    enterprise = true
    gallery    = true
  }
}

resource "random_uuid" "api_scope" {}

resource "azuread_application" "api" {
  display_name = "${var.app_name}-${var.environment_short}-api"

  api {
    oauth2_permission_scope {
      admin_consent_description  = "Access the application."
      admin_consent_display_name = "Access"
      enabled                    = true
      id                         = random_uuid.api_scope.id
      type                       = "Admin"
      value                      = "access"
    }

    known_client_applications = [azuread_application.spa.application_id]
  }

  lifecycle {
    ignore_changes = [identifier_uris]
  }
}

resource "null_resource" "app_reg_api_uri" {
  triggers = {
    api_application_id = azuread_application.api.application_id
  }
  provisioner "local-exec" {
    interpreter = ["pwsh", "-Command"]
    command     = "./scripts/set-app-reg-uri.ps1 -app_id ${azuread_application.api.application_id}"
  }
}

resource "azuread_service_principal" "api" {
  depends_on = [azuread_application.api, null_resource.app_reg_api_uri]

  application_id               = azuread_application.api.application_id
  app_role_assignment_required = false
  owners                       = [data.azuread_client_config.current.object_id]

  feature_tags {
    enterprise = true
  }
}

resource "azuread_application_pre_authorized" "spa_on_api" {
  depends_on = [
    azuread_application.spa,
    azuread_service_principal.spa,
    azuread_application.api,
    null_resource.app_reg_api_uri,
    azuread_service_principal.api
  ]

  application_object_id = azuread_application.api.object_id
  authorized_app_id     = azuread_application.spa.application_id
  permission_ids        = [random_uuid.api_scope.id]
}

data "azuread_application_published_app_ids" "well_known" {}

resource "azuread_service_principal" "msgraph" {
  application_id = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph
  use_existing   = true
}

resource "azuread_service_principal_delegated_permission_grant" "spa_on_msgraph" {
  depends_on = [
    azuread_application.spa,
    azuread_service_principal.spa,
    azuread_application.api,
    null_resource.app_reg_api_uri,
    azuread_service_principal.api,
    azuread_application_pre_authorized.spa_on_api
  ]

  service_principal_object_id          = azuread_service_principal.spa.object_id
  resource_service_principal_object_id = azuread_service_principal.msgraph.object_id
  claim_values                         = ["User.Read"]
}
