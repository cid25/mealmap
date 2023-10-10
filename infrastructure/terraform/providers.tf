terraform {

  required_providers {
    azuread = {
      source  = "hashicorp/azuread"
      version = "2.43.0"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.75.0"
    }
  }

  backend "azurerm" {
    container_name = "mealmap"
    key            = "tfstate"
  }

  required_version = ">= 1.5.7"
}

provider "azurerm" {
  skip_provider_registration = true
  features {}
}
