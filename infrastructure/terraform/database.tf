resource "azuread_group" "sql_aad_admins" {
  display_name     = "${var.app_name}-${var.environment_short}-sql-aad-admins"
  owners           = [data.azuread_client_config.current.object_id]
  security_enabled = true
}

resource "azuread_group_member" "admin" {
  for_each = var.administrators

  group_object_id  = azuread_group.sql_aad_admins.id
  member_object_id = each.value.object_id
}

resource "azurerm_user_assigned_identity" "sql_server" {
  name                = "id-${var.app_name}-${var.environment_short}-sql"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
}

resource "azurerm_mssql_server" "sql" {
  name                = "sql-${var.app_name}-${var.environment_short}"
  resource_group_name = azurerm_resource_group.mealmap.name
  location            = var.location
  version             = "12.0"
  minimum_tls_version = "1.2"

  azuread_administrator {
    login_username              = azuread_group.sql_aad_admins.display_name
    object_id                   = azuread_group.sql_aad_admins.object_id
    tenant_id                   = data.azuread_client_config.current.tenant_id
    azuread_authentication_only = true
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.sql_server.id]
  }

  primary_user_assigned_identity_id = azurerm_user_assigned_identity.sql_server.id
}

resource "azurerm_mssql_database" "sql_db" {
  name                 = "sqldb-${var.app_name}-${var.environment_short}"
  server_id            = azurerm_mssql_server.sql.id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  license_type         = "LicenseIncluded"
  sku_name             = "Basic"
  storage_account_type = "Local"
  geo_backup_enabled   = false
}
