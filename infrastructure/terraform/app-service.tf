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

  lifecycle {
    ignore_changes = [
      site_config[0].application_stack[0].docker_image_name
    ]
  }
}
