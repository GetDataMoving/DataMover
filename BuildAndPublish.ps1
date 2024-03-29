[String] $GetDataMovingOrgDirectoryPath = [IO.Path]::Combine($Env:UserProfile, "source\repos\GetDataMoving");
[String] $ReleasesDirectoryPath = [IO.Path]::Combine($GetDataMovingOrgDirectoryPath, "Releases");
[String] $DataMoverRepoDirectoryPath = [IO.Path]::Combine($GetDataMovingOrgDirectoryPath, "DataMover");
[String] $DataMoverProjectDirectoryPath = [IO.Path]::Combine($DataMoverRepoDirectoryPath, "DataMover");
[String] $DataMoverProjectFilePath = [IO.Path]::Combine($DataMoverProjectDirectoryPath, "DataMover.csproj");
[String] $PublishOutputDirectoryPath = [IO.Path]::Combine($DataMoverRepoDirectoryPath, "PublishOutput");
[String] $PublishOutputExeFilePath = [IO.Path]::Combine($PublishOutputDirectoryPath, "DataMover.exe");
[String] $WrappersDirectoryPath = [IO.Path]::Combine($DataMoverRepoDirectoryPath, "Wrappers");

If (![IO.Directory]::Exists($PublishOutputDirectoryPath))
{
	[void] [IO.Directory]::CreateDirectory($PublishOutputDirectoryPath)
}
Copy-Item -Path ([String]::Format("{0}\*", $WrappersDirectoryPath)) -Destination ([String]::Format("{0}\", $PublishOutputDirectoryPath))

Set-Location -Path $DataMoverProjectDirectoryPath;
dotnet build --configuration "Release"
dotnet publish -p:PublishProfile=PortableFolder
Set-Location -Path $DataMoverRepoDirectoryPath;

[System.Version] $PublishedExeFileVersion = (Get-Item $PublishOutputExeFilePath).VersionInfo.FileVersionRaw
[String] $PublishedExeFileVersionText = [String]::Format("v{0}.{1}.{2}",
	$PublishedExeFileVersion.Major,
	$PublishedExeFileVersion.Minor,
	$PublishedExeFileVersion.Build);
[String] $ProjectVersionText = "v";
If ([IO.File]::Exists($DataMoverProjectFilePath))
{
	$ProjectVersionText += ([xml](Get-Content -Path $DataMoverProjectFilePath)).SelectSingleNode('//Project/PropertyGroup[1]/Version/text()').Value;
}
If ($PublishedExeFileVersionText -eq $ProjectVersionText)
{
	[String] $ReleaseFilePath = [IO.Path]::Combine($ReleasesDirectoryPath, [String]::Format("{0}.zip", $ProjectVersionText));
	Write-Host -Object ([String]::Format("PublishedExeFileVersionText: {0}", $PublishedExeFileVersionText));
	Write-Host -Object ([String]::Format("ProjectVersionText: {0}", $ProjectVersionText));
	Write-Host -Object ([String]::Format("ReleaseFilePath: {0}", $ReleaseFilePath));
	If ([IO.File]::Exists($ReleaseFilePath))
	{
		[void] [IO.File]::Delete($ReleaseFilePath);
	}
	Compress-Archive -Path ([String]::Format("{0}\*", $PublishOutputDirectoryPath)) -DestinationPath $ReleaseFilePath -Force;
}
