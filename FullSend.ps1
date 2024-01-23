. .\BuildAndPublish.ps1 "Build"
. .\BumpAndPublish.ps1 -Segment "Build"
. .\CommitNewVersion.ps1
. .\PublishRelease.ps1
# git log --graph --abbrev-commit --decorate --format=format:'%C(bold green)%as%C(reset) %C(bold blue)%h%C(reset) %C(white)%s%C(reset) %d' --all --no-abbrev