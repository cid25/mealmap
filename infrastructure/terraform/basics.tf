data "azuread_client_config" "current" {}

resource "azurerm_resource_group" "mealmap" {
  name     = "rg-${var.app_name}-${var.environment_short}"
  location = var.location
}

locals {
  sql_server_admin_login = "sqlserveradmin"
}

resource "random_password" "sql_server_admin_password" {
  length           = 16
  special          = true
  min_special      = 1
  min_numeric      = 1
  min_upper        = 1
  min_lower        = 1
  override_special = "$%&-_+{}<>"
}
