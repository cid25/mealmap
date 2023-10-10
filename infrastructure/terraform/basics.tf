data "azuread_client_config" "current" {}

resource "azurerm_resource_group" "mealmap" {
  name     = "rg-${var.app_name}-${var.environment_short}"
  location = var.location
}
