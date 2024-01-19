. .\BuildAndPublish.ps1 "Build"
. .\BumpAndPublish.ps1 -Segment "Build"
. .\CommitNewVersion.ps1
. .\PublishRelease.ps1
