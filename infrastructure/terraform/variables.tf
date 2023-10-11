variable "app_name" {
  type        = string
  description = "The canonical name of the application.  Used to construct resource names."
}

variable "environment_short" {
  type        = string
  description = "The short form of the target environment. Used to construct resource names."
}

variable "location" {
  type        = string
  description = "The Azure location/region to create the infrastructure in."
}

variable "sql_administrators" {
  type        = map(object({ object_id = string }))
  description = "Map of object ids of AAD/Microsoft Entra users/principals that are granted admin privileges for the SQL server and databases."
}

variable "docker_registry_url" {
  type = string
}

variable "docker_registry_username" {
  type = string
}

variable "docker_registry_password" {
  type      = string
  sensitive = true
}

variable "default_docker_image_name" {
  type        = string
  description = "The image to spin up when creating the app for the first time, e.g. ghcr.io/cid25/mealmap:latest."
}
