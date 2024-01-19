[String] $GetDataMovingOrgDirectoryPath = [IO.Path]::Combine($Env:UserProfile, "source\repos\GetDataMoving");
[String] $ReleasesDirectoryPath = [IO.Path]::Combine($GetDataMovingOrgDirectoryPath, "Releases");
[String] $DataMoverRepoDirectoryPath = [IO.Path]::Combine($GetDataMovingOrgDirectoryPath, "DataMover");
[String] $DataMoverProjectDirectoryPath = [IO.Path]::Combine($DataMoverRepoDirectoryPath, "DataMover");
[String] $DataMoverProjectFilePath = [IO.Path]::Combine($DataMoverProjectDirectoryPath, "DataMover.csproj");

Set-Location -Path $DataMoverRepoDirectoryPath;
[String] $LastReleaseTag = (ConvertFrom-Json -InputObject (gh release view --json "tagName")).tagName;
Set-Location -Path $DataMoverRepoDirectoryPath;

[String] $Title = "Version ";
[String] $Tag = "v";
If ([IO.File]::Exists($DataMoverProjectFilePath))
{
	[String] $VersionText = ([xml](Get-Content -Path $DataMoverProjectFilePath)).SelectSingleNode('//Project/PropertyGroup[1]/Version/text()').Value;
	$Title += $VersionText;
	$Tag += $VersionText;
}
If ($LastReleaseTag -ne $ProjectVersionText)
{
	[String] $ReleaseFilePath = [IO.Path]::Combine($ReleasesDirectoryPath, [String]::Format("{0}.zip", $Tag));
	gh release create --latest --notes-from-tag --title "$Title" --latest $Tag "$ReleaseFilePath"
}
