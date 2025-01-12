resource "azurerm_windows_web_app" "main" {
  name                = "localvenue-webapp-${var.azure_name_suffix}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id
  depends_on          = [azurerm_mysql_flexible_database.main]

  site_config {
    always_on = false #needed for service plans with F1 (free) SKU    
  }

  identity {
    type = "SystemAssigned"
  }

  zip_deploy_file = var.zip_deploy_file_path
  #ENVIRONMENT VARIABLE
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE" = "1",
    "ASPNETCORE_ENVIRONMENT"   = "Production"
    "TMDB__TOKEN"              = var.tmdb_token
    "TMDB__BASEURL"            = var.tmdb_base
  }
  connection_string {
    name  = "VenueContext"
    type  = "MySql"
    value = local.mysql_connection_string
  }
}

output "web_app_name" {
  value = azurerm_windows_web_app.main.name
}

output "api_uri" {
  value = "https://${azurerm_windows_web_app.main.name}.azurewebsites.net"
}
