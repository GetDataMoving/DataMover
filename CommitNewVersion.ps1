Param
(
    [Parameter(Mandatory=$true)]
    [String] $Message
)

[String] $DataMoverProjectFilePath = [IO.Path]::Combine($Env:UserProfile, "source", "repos", "GetDataMoving", "DataMover", "DataMover", "DataMover.csproj");
[String] $VersionText = ([xml](Get-Content -Path $DataMoverProjectFilePath)).SelectSingleNode('//Project/PropertyGroup[1]/Version/text()').Value;
[String] $Tag = "v" + $VersionText
git add .
git commit -m"$Message"
git push
git tag "$Tag"
git push --tags
