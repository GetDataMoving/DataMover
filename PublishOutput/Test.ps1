. ([IO.Path]::Combine([IO.Path]::GetDirectoryName($PSCommandPath), "PSWrapper.ps1"));

[String] $SQLServerInstance = [System.Net.Dns]::GetHostName(); #Debugging and Testing is done on the local install of SQL.
[String] $SQLDatabase = "master";
[String] $SQLServerConnectionString = (
        [String]::Format("Server={0};", $SQLServerInstance) +
        [String]::Format("Database={0};", $SQLDatabase) +
        "TrustServerCertificate=True;" +
        "Trusted_Connection=True;" +
        [String]::Format("Workstation ID={0};", [System.Net.Dns]::GetHostName()) +
        [String]::Format("Application Name={0};", [IO.Path]::GetFileName($PSCommandPath))
    );
[String] $SQLQuery = "SELECT COUNT(*) FROM [master].[sys].[databases]";

[String] $Result = Invoke-DataMoverSSQES -SQLServerConnectionString $SQLServerConnectionString -Query $SQLQuery -LoggingLevel "Verbose";
Write-Host -Object ([String]::Format("This is the result: {0}", $Result))
