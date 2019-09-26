try 
{ 
	$var = Get-AzADUser -ErrorAction Stop ### Just a dummy operation to check if already connected to Azure. If successful no need to reconnect
} 
catch
{ 
	Write-Host "You're not connected. Please Connect using your credentials in the popup"; 
	#Connect-AzAccount ## On Azure Subscription should be used
 }
 
 $resourceGroupName = "UmbracoResourceGroupDeployment001"
 $globalLocation = "Australia Southeast"

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
 
 try
 {
	 New-AzResourceGroupDeployment `
		 -ResourceGroupName $resourceGroupName `
		 -TemplateFile "$PSScriptRoot\UmbracoResourcesTemplate.json" `
		 -TemplateParameterFile "$PSScriptRoot\UmbracoResourcesTemplate.parameters.json" `
		 -ErrorAction Stop 
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

 ## Disconnect-AzAccount # Azure DevOps - Disconnection not needed