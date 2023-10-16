param ($app_id)

$tenant_id = $env:ARM_TENANT_ID;
$client_id = $env:ARM_CLIENT_ID;
$client_secret = $env:ARM_CLIENT_SECRET;

az login --service-principal --tenant $tenant_id --username $client_id --password $client_secret;

az ad app update --id $app_id --identifier-uris "api://$app_id";
