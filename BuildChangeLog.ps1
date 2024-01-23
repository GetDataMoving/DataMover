[String] $CurrentWorkingDirectoryPath = $PWD;
[String] $RepoPDirectoryPath = [IO.Path]::Combine($Env:UserProfile, "source\repos\GetDataMoving\DataMover");
[String] $ChangeLogFilePath = [IO.Path]::Combine($Env:UserProfile, "source\repos\GetDataMoving\DataMover\CHANGELOG.md");
Set-Location -Path $RepoPDirectoryPath;
[Collections.Hashtable] $Entries = [Collections.Hashtable]::new();
[String] $Tags = (git log --tags --simplify-by-decoration --pretty="format:%ci   %d" | Out-String);
ForEach ($Line In ($Tags -split "`r`n"))
{
    [String[]] $LineElements = $Line -split "   ";
    If ($LineElements.Length -eq 2)
    {
        [String] $DateTimeString = $LineElements[0].Trim();
        [DateTime] $DateTime = [Convert]::ToDateTime($DateTimeString)
        $DateTime = $DateTime.ToUniversalTime();
        [String] $Tag = $LineElements[1].Trim();
        If ($Tag)
        {
            $Tag = $Tag.Substring(6, $Tag.Length - 7);
            [void] $Entries.Add($DateTime.ToString("yyyyMMdd.HHmmss.1"), @{
                "DateTime" = $DateTime;
                "Tag" = $Tag;
            });
        }
    }
}
[String] $Commits = (git log --pretty='format:%ai   %h   %H   %s' --all | Out-String);
ForEach ($Line In ($Commits -split "`r`n"))
{
    [String[]] $LineElements = $Line -split "   ";
    If ($LineElements.Length -eq 4)
    {
        [String] $DateTimeString = $LineElements[0].Trim();
        [DateTime] $DateTime = [Convert]::ToDateTime($DateTimeString).ToUniversalTime();
        [String] $AbbreviatedHash = $LineElements[1].Trim();
        [String] $FullHash = $LineElements[2].Trim();
        [String] $Message = $LineElements[3].Trim();
        [void] $Entries.Add($DateTime.ToString("yyyyMMdd.HHmmss.0"), @{
            "DateTime" = $DateTime;
            "AbbreviatedHash" = $AbbreviatedHash;
            "FullHash" = $FullHash;
            "Message" = $Message;
        });
    }
}
[Collections.Generic.List[PSObject]] $Releases = [Collections.Generic.List[PSObject]]::new();
[PSObject] $LastRelease = [PSObject]::new();
Add-Member -InputObject $LastRelease -TypeName "String" -NotePropertyName "Tag" -NotePropertyValue "Unreleased";
Add-Member -InputObject $LastRelease -TypeName "DateTime" -NotePropertyName "DateTime" -NotePropertyValue ([DateTime]::UtcNow);
Add-Member -InputObject $LastRelease -TypeName "Collections.Generic.List[PSObject]" -NotePropertyName "Commits" -NotePropertyValue ([Collections.Generic.List[PSObject]]::new());

ForEach ($Key In ($Entries.Keys | Sort-Object -Descending))
{
    If ($Key.EndsWith("1"))
    {
        [void] $Releases.Add($LastRelease);
        [PSObject] $LastRelease = [PSObject]::new();
        Add-Member -InputObject $LastRelease -TypeName "String" -NotePropertyName "Tag" -NotePropertyValue ($Entries[$Key]["Tag"]);
        Add-Member -InputObject $LastRelease -TypeName "DateTime" -NotePropertyName "DateTime" -NotePropertyValue ($Entries[$Key]["DateTime"]);
        Add-Member -InputObject $LastRelease -TypeName "Collections.Generic.List[PSObject]" -NotePropertyName "Commits" -NotePropertyValue ([Collections.Generic.List[PSObject]]::new());
    }
    ElseIf ($Key.EndsWith("0"))
    {
        [String] $CommitType = [String]::Empty;
        If ($Entries[$Key]["Message"].StartsWith("fix"))
            { $CommitType = "Fix"; }
        ElseIf ($Entries[$Key]["Message"].StartsWith("feat"))
            { $CommitType = "Feature"; }
        ElseIf ($Entries[$Key]["Message"].StartsWith("repo"))
            { $CommitType = "Repository"; }
        [PSObject] $Commit = [PSObject]::new();
        Add-Member -InputObject $Commit -TypeName "DateTime" -NotePropertyName "DateTime" -NotePropertyValue ($Entries[$Key]["DateTime"]);
        Add-Member -InputObject $Commit -TypeName "String" -NotePropertyName "Type" -NotePropertyValue $CommitType;
        Add-Member -InputObject $Commit -TypeName "String" -NotePropertyName "AbbreviatedHash" -NotePropertyValue ($Entries[$Key]["AbbreviatedHash"]);
        Add-Member -InputObject $Commit -TypeName "String" -NotePropertyName "FullHash" -NotePropertyValue ($Entries[$Key]["FullHash"]);
        Add-Member -InputObject $Commit -TypeName "String" -NotePropertyName "Message" -NotePropertyValue ($Entries[$Key]["Message"]);
        [void] $LastRelease.Commits.Add($Commit);
    }
}

[String] $Markdown = "# CHANGELOG`n---`n`n";
ForEach ($Release In $Releases)
{
    If ($Release.Tag -ne "Unreleased")
    {
        $Markdown += [String]::Format("## [{0}](https://github.com/GetDataMoving/DataMover/releases/tag/{0}) ({1})`n`n",
            $Release.Tag,
            $Release.DateTime.ToString("yyyy-MM-dd")
        );
        If ($Release.Commits.Count -gt 0)
        {
            $Markdown += "### Commits`n"
            ForEach ($Commit In $Release.Commits)
            {
                $Markdown += [String]::Format("* [{0}](https://github.com/GetDataMoving/DataMover/commit/{1}) {2} {3}`n",
                    $Commit.AbbreviatedHash,
                    $Commit.FullHash,
                    $Commit.Message,
                    $Commit.Type
                );
            }
            $Markdown += "`n";
        }
    }
}
[void] [IO.File]::WriteAllText($ChangeLogFilePath, $Markdown);
Set-Location -Path $CurrentWorkingDirectoryPath;
