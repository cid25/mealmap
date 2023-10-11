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
    random = {
      source  = "hashicorp/random"
      version = "3.5.1"
    }
    mssql = {
      source  = "betr-io/mssql"
      version = "0.2.7"
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
