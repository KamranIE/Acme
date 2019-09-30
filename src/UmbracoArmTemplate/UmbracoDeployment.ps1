try 
{ 
	$var = Get-AzADUser -ErrorAction Stop ### Just a dummy operation to check if already connected to Azure. If successful no need to reconnect
} 
catch
{ 
	Write-Host "You're not connected. Please Connect using your credentials in the popup"; 
	##Connect-AzAccount ## On Azure Subscription should be used - For local use uncomment
 }
 
 $resourceGroupName = "UmbracoResourceGroupDeployment001"
 $globalLocation = "Australia Southeast"
 $dbServerName = "genuniqueservername"

 $dynamicParams = @{ 
   resGrpName = $resourceGroupName
   globalLocation = $globalLocation
 }

 Write-Host "The resource group name to be created is $($dynamicParams.resGrpName)"

 $resourceGroupExists = 1
 try
 {
	Get-AzResourceGroup -Name $resourceGroupName -ErrorAction Stop  ## -ErrorAction Stop will force to fall in the exception block on any exception 
 }
 catch
 {
   $resourceGroupExists = 0
 }
 
 if($resourceGroupExists -eq 0)
 {
	 New-AzDeployment `
	  -Name AcmeDeployment `
	  -Location $globalLocation `
	  -TemplateFile "$PSScriptRoot\UmbracoResourceGroupTemplate.json" `
	  -TemplateParameterFile "$PSScriptRoot\UmbracoResourceGroupTemplate.parameters.json" `
	  @dynamicParams
 }
 else
 {
	Write-Host "Resource Group $resourceGroupName already exists. Moving forward with resources deployment" -ForegroundColor white -BackgroundColor red; 
 } 
 
 ## Check if we need to deploy resources. If at least database is there assume we don't need to deploy resources
 ## Usually ARM automatically skips the existing resources. This is being done specifically to safe some CPU cycles but specifically to
 ## to avoid any errors e.g. bacpac file import fails if db already exists.

 $dbAlreadyExists = 1
 try
 {
	Get-AzSqlDatabase -ResourceGroupName "$resourceGroupName" -ServerName "$dbServerName" -ErrorAction Stop  ## -ErrorAction Stop will force to fall in the exception block on any exception 
 }
 catch
 {
   $dbAlreadyExists = 0
 }

 try
 {
	if ($dbAlreadyExists -eq 0) 
	{
		New-AzResourceGroupDeployment `
			-ResourceGroupName $resourceGroupName `
			-TemplateFile "$PSScriptRoot\UmbracoResourcesTemplate.json" `
			-TemplateParameterFile "$PSScriptRoot\UmbracoResourcesTemplate.parameters.json" `
			-ErrorAction Stop 
	}
	else
	{
	    Write-Host "database server $dbServerName already exists. Skipping resources deployment." -ForegroundColor white -BackgroundColor red; 
	}
 }
 catch
 {
    Write-Host $_.Exception.Message
    Write-Host $_.Exception.ItemName
	$userInput = Read-Host 'An exception encountered. Going to delete the resource group $($dynamicParams.resGrpName). Press ''Y'' if you agree OR any other key to keep it'

	if ($userInput -eq 'y' -OR  $userInput -eq 'Y')
	{
	   Get-AzResourceGroup -Name $resourceGroupName | Remove-AzResourceGroup -Verbose -Force  ## delete resource group in case of an error
	}
 }

 ## Disconnect-AzAccount # For Azure DevOps Disconnection not needed - For local use uncomment