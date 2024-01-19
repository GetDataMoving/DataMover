Param
(
    [Parameter(Mandatory=$false)]
	[ValidateSet("Major", "Minor", "Build")]
    [String] $Segment = "Build"
)

[String] $GetDataMovingOrgDirectoryPath = [IO.Path]::Combine($Env:UserProfile, "source\repos\GetDataMoving");
[String] $DataMoverRepoDirectoryPath = [IO.Path]::Combine($GetDataMovingOrgDirectoryPath, "DataMover");
ForEach ($Directory In (Get-ChildItem -Path $DataMoverRepoDirectoryPath | Where-Object -FilterScript { $_.PSIsContainer }))
{
	[String] $ProjectFile = [IO.Path]::Combine($Directory.FullName, [String]::Format("{0}.csproj", $Directory.Name))

	If ([IO.File]::Exists($ProjectFile))
	{
		[String] $ProjectName = [IO.Path]::GetFileNameWithoutExtension($ProjectFile);

		[xml] $ProjectFileXML = [xml](Get-Content -Path $ProjectFile)
		[System.Xml.XmlText] $VersionNode = $ProjectFileXML.SelectSingleNode('//Project/PropertyGroup[1]/Version/text()');
		[String] $CurrentVersionText = $VersionNode.Value;
		[String[]] $CurrentVersionSegments = $CurrentVersionText.Split(".");

		[Int32] $CurrentMajor = [Convert]::ToInt32($CurrentVersionSegments[0]);
		[Int32] $CurrentMinor = [Convert]::ToInt32($CurrentVersionSegments[1]);
		[Int32] $CurrentBuild = [Convert]::ToInt32($CurrentVersionSegments[2]);

		[String] $NewVersionText = $VersionNode.Value;
		[Int32] $NewMajor = $CurrentMajor;
		[Int32] $NewMinor = $CurrentMinor;
		[Int32] $NewBuild = $CurrentBuild;
		Switch ($Segment)
		{
			"Major"
			{
				$NewMajor ++;
				$NewMinor = 0;
				$NewBuild = 0;
			}
			"Minor"
			{
				$NewMinor ++;
				$NewBuild = 0;
			}
			"Build"
			{
				$NewBuild ++;
			}
		}
		$NewVersionText = [String]::Format("{0}.{1}.{2}", $NewMajor, $NewMinor, $NewBuild);
		Write-Host -Object ([String]::Format("Bumping {0} Number`n{1}`n`tCurrent Version: {2}`n`t`tMajor: {3}, Minor: {4}, Build: {5}`n`tNew Version: {6}`n`t`tMajor: {7}, Minor: {8}, Build: {9}",
				$Segment,
				$ProjectName,
				$CurrentVersionText, $CurrentMajor, $CurrentMinor, $CurrentBuild,
				$NewVersionText, $NewMajor, $NewMinor, $NewBuild
		));
		$VersionNode.Value = $NewVersionText;
		[void] $ProjectFileXML.Save($ProjectFile)
	}
}