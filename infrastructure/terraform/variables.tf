variable "app_name" {
  type = string
}

variable "environment_short" {
  type = string
}

variable "location" {
  type = string
}

variable "administrators" {
  type = map(object({ object_id = string }))
}
