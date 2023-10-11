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
  hostname            = "${var.environment_short == "prod" ? "" : join(var.environment_short, ".")}${var.app_name}.${var.base_domain_name}"
  app_service_name    = azurerm_linux_web_app.app_service.name
  resource_group_name = azurerm_resource_group.mealmap.name
}
