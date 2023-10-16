resource "azurerm_service_plan" "app_service_plan" {
  name                = "asp-${var.app_name}-${var.environment_short}"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_user_assigned_identity" "app_service" {
  name                = "id-${var.app_name}-${var.environment_short}-app"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
}

resource "azurerm_linux_web_app" "app_service" {
  name                = "app-${var.app_name}-${var.environment_short}"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.app_service.id]
  }

  site_config {
    application_stack {
      docker_registry_url      = var.docker_registry_url
      docker_registry_username = var.docker_registry_username
      docker_registry_password = var.docker_registry_password
      docker_image_name        = var.default_docker_image_name
    }
  }

  connection_string {
    name  = "MealmapDb"
    type  = "SQLAzure"
    value = "Server=tcp:${azurerm_mssql_server.sql.name}.database.windows.net;Database=${azurerm_mssql_database.sql_db.name};Authentication=Active Directory Default;User Id=${azurerm_user_assigned_identity.app_service.client_id};TrustServerCertificate=True"
  }

  app_settings = {
    ASPNETCORE_URLS      = "http://*:80"
    AzureAd__Instance    = "https://login.microsoftonline.com/"
    AzureAd__ClientId    = azuread_application.api.application_id
    AzureAd__TenantId    = data.azuread_client_config.current.tenant_id
    Angular__Authority   = "https://login.microsoftonline.com/${data.azuread_client_config.current.tenant_id}"
    Angular__ClientId    = azuread_application.spa.application_id
    Angular__ApiScope    = "api://${azuread_application.api.application_id}/access"
    Angular__RedirectUri = "https://${local.hostname}"
  }
  https_only = true

  lifecycle {
    ignore_changes = [
      site_config[0].application_stack[0].docker_image_name
    ]
  }
}

resource "mssql_user" "app_sql_db_user" {
  depends_on = [azurerm_mssql_firewall_rule.allow_azure_services]

  server {
    host = "${azurerm_mssql_server.sql.name}.database.windows.net"
    login {
      username = local.sql_server_admin_login
      password = random_password.sql_server_admin_password.result
    }
  }
  database  = azurerm_mssql_database.sql_db.name
  username  = azurerm_user_assigned_identity.app_service.name
  object_id = azurerm_user_assigned_identity.app_service.client_id
  roles     = ["db_datareader", "db_datawriter"]
}

resource "azurerm_app_service_custom_hostname_binding" "custom" {
  hostname            = local.hostname
  app_service_name    = azurerm_linux_web_app.app_service.name
  resource_group_name = azurerm_resource_group.mealmap.name

  lifecycle {
    ignore_changes = [ssl_state, thumbprint]
  }
}

resource "azurerm_app_service_managed_certificate" "managed_cert" {
  custom_hostname_binding_id = azurerm_app_service_custom_hostname_binding.custom.id
}

resource "azurerm_app_service_certificate_binding" "cert_binding" {
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.custom.id
  certificate_id      = azurerm_app_service_managed_certificate.managed_cert.id
  ssl_state           = "SniEnabled"
}
