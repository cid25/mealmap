resource "azuread_group" "sql_aad_admins" {
  display_name     = "${var.app_name}-${var.environment_short}-sql-aad-admins"
  owners           = [data.azuread_client_config.current.object_id]
  security_enabled = true
}

resource "azuread_group_member" "admin" {
  for_each = var.sql_administrators

  group_object_id  = azuread_group.sql_aad_admins.id
  member_object_id = each.value.object_id
}

resource "azurerm_user_assigned_identity" "sql_server" {
  name                = "id-${var.app_name}-${var.environment_short}-sql"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
}

resource "azurerm_mssql_server" "sql" {
  name                         = "sql-${var.app_name}-${var.environment_short}"
  resource_group_name          = azurerm_resource_group.mealmap.name
  location                     = var.location
  version                      = "12.0"
  minimum_tls_version          = "1.2"
  administrator_login          = local.sql_server_admin_login
  administrator_login_password = random_password.sql_server_admin_password.result

  azuread_administrator {
    login_username              = azuread_group.sql_aad_admins.display_name
    object_id                   = azuread_group.sql_aad_admins.object_id
    tenant_id                   = data.azuread_client_config.current.tenant_id
    azuread_authentication_only = false
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.sql_server.id]
  }

  primary_user_assigned_identity_id = azurerm_user_assigned_identity.sql_server.id
}

resource "azurerm_mssql_firewall_rule" "allow_azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.sql.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_mssql_database" "sql_db" {
  name                 = "sqldb-${var.app_name}-${var.environment_short}"
  server_id            = azurerm_mssql_server.sql.id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  license_type         = "LicenseIncluded"
  sku_name             = "Basic"
  storage_account_type = "Local"
  geo_backup_enabled   = false

  lifecycle {
    ignore_changes = [
      geo_backup_enabled
    ]
  }
}
