##!/bin/bash

# Step 1: Install az cli and terraform
#> choco install terraform
#> winget install -e --id Microsoft.AzureCLI
# Step 2: Restart your terminal
# Step 3a: Login to Azure in az cli
# Step 3b: Probably restart your terminal again
# Step 4: Run this script

dotnet publish -c Release

OS=$(uname -s)

if [ "$OS" = "Linux" ] || [ $OS = "Darwin" ]; then # Linux or MacOS
    zip -r ./LocalVenue/bin/Release/publish.zip ./LocalVenue/bin/Release/net8.0/publish/* 
else # Windows
    powershell 'Compress-Archive -Path ./LocalVenue/bin/Release/net8.0/publish/* -DestinationPath ./LocalVenue/bin/Release/publish.zip -Force'
fi

terraform init --upgrade
terraform apply --parallelism=25 -auto-approve

# Step 5 (optional): Deploy the app, if Terraform failed or is slow to redeploy once the resources are created
#> az webapp deploy --resource-group localvenue-<suffix> --name localvenue-webapp-<suffix> --src-path "./LocalVenue/bin/Release/publish.zip"

rm ./LocalVenue/bin/Release/publish.zip # Clean up the zip file

# Step 6: Cleanup can be done with 
#> terraform destroy -auto-approve