[String] $DataMoverExecutablePath = [IO.Path]::Combine([IO.Path]::GetDirectoryName($PSCommandPath), "DataMover.exe");
If (![IO.File]::Exists($DataMoverExecutablePath))
{
    Throw [IO.FileNotFoundException]::new($DataMoverExecutablePath);
}

Function Invoke-DataMoverPG2SS()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLSchema,

        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLTable,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerSchema,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerTable,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Name", "Position")]
        [String] $ColumnMatchingMethod = "Position",
       
        [Parameter(Mandatory=$false)]
        [Switch] $TruncateTargetTable = $false,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "pg2ss"

        [String]::Format("-pgConn `"{0}`"", $PostgreSQLConnectionString),
        [String]::Format("-srcSchema `"{0}`"", $PostgreSQLSchema),
        [String]::Format("-srcTable `"{0}`"", $PostgreSQLTable),

        [String]::Format("-ssConn `"{0}`"", $SQLServerConnectionString),
        [String]::Format("-trgSchema `"{0}`"", $SQLServerSchema),
        [String]::Format("-trgTable `"{0}`"", $SQLServerTable),

        [String]::Format("-m `"{0}`"", $ColumnMatchingMethod),
        [String]::Format("{0}",
            ($TruncateTargetTable ? "-trunc" : "")
        ),
        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}

Function Invoke-DataMoverSS2PG()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $SQLServerConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerSchema,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerTable,

        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLSchema,

        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLTable,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Name", "Position")]
        [String] $ColumnMatchingMethod = "Position",
       
        [Parameter(Mandatory=$false)]
        [Switch] $TruncateTargetTable = $false,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "ss2pg"

        [String]::Format("-ssConn `"{0}`"", $SQLServerConnectionString),
        [String]::Format("-srcSchema `"{0}`"", $SQLServerSchema),
        [String]::Format("-srcTable `"{0}`"", $SQLServerTable),

        [String]::Format("-pgConn `"{0}`"", $PostgreSQLConnectionString),
        [String]::Format("-trgSchema `"{0}`"", $PostgreSQLSchema),
        [String]::Format("-trgTable `"{0}`"", $PostgreSQLTable),

        [String]::Format("-m `"{0}`"", $ColumnMatchingMethod),
        [String]::Format("{0}",
            ($TruncateTargetTable ? "-trunc" : "")
        ),
        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}

Function Invoke-DataMoverDF2SS()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $DelimitedFilePath,

        [Parameter(Mandatory=$true)]
        [String] $ColumnDelimiter,
       
        [Parameter(Mandatory=$false)]
        [Switch] $HasHeaderRow = $false,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerSchema,

        [Parameter(Mandatory=$true)]
        [String] $SQLServerTable,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Name", "Position")]
        [String] $ColumnMatchingMethod = "Position",
       
        [Parameter(Mandatory=$false)]
        [Switch] $TruncateTargetTable = $false,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "df2ss"

        [String]::Format("-dfPath `"{0}`"", $DelimitedFilePath),
        [String]::Format("-cd `"{0}`"", $ColumnDelimiter),
        [String]::Format("{0}",
            ($HasHeaderRow ? "-head" : "")
        ),

        [String]::Format("-ssConn `"{0}`"", $SQLServerConnectionString),
        [String]::Format("-trgSchema `"{0}`"", $SQLServerSchema),
        [String]::Format("-trgTable `"{0}`"", $SQLServerTable),

        [String]::Format("-m `"{0}`"", $ColumnMatchingMethod),
        [String]::Format("{0}",
            ($TruncateTargetTable ? "-trunc" : "")
        ),
        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}

Function Invoke-DataMoverPGQE()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $Query,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "pgqe",

        [String]::Format("-pgConn `"{0}`"", $PostgreSQLConnectionString),
        [String]::Format("-q `"{0}`"", $Query),

        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}

Function Invoke-DataMoverPGQES()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $PostgreSQLConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $Query,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "pgqes",

        [String]::Format("-pgConn `"{0}`"", $PostgreSQLConnectionString),
        [String]::Format("-q `"{0}`"", $Query),

        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    [String] $Result = $Process.StandardOutput.ReadToEnd();
    [String] $ResultLine = "Other";
    ForEach ($Line In ($Result -split "`n"))
    {
        If ($Line.Equals("END RESULTS:"))
        {
            $ResultLine = "Out Result";
        }
        If ($ResultLine.Equals("In Result"))
        {
            $ReturnValue += [String]::Format("{0}`r`n", $Line);
        }
        If ($Line.Equals("BEGIN RESULTS:"))
        {
            $ResultLine = "In Result";
        }
    }
    If ($ReturnValue.EndsWith("`n"))
    {
        $ReturnValue = $ReturnValue.Substring(0, $ReturnValue.Length - 1);
    }
    If ($ReturnValue.EndsWith("`r"))
    {
        $ReturnValue = $ReturnValue.Substring(0, $ReturnValue.Length - 1);
    }
    Return $ReturnValue;
}

Function Invoke-DataMoverSSQE()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $SQLServerConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $Query,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "ssqe",

        [String]::Format("-ssConn `"{0}`"", $SQLServerConnectionString),
        [String]::Format("-q `"{0}`"", $Query),

        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}

Function Invoke-DataMoverSSQES()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$true)]
        [String] $SQLServerConnectionString,

        [Parameter(Mandatory=$true)]
        [String] $Query,

        [Parameter(Mandatory=$false)]
        [ValidateSet("Quiet", "Exception", "Verbose")]
        [String] $LoggingLevel = "Verbose"
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    $ProcessStartInfo.Arguments = @(
        "ssqes",

        [String]::Format("-ssConn `"{0}`"", $SQLServerConnectionString),
        [String]::Format("-q `"{0}`"", $Query),

        [String]::Format("-ll `"{0}`"", $LoggingLevel)
    );
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    [String] $Result = $Process.StandardOutput.ReadToEnd();
    [String] $ResultLine = "Other";
    ForEach ($Line In ($Result -split "`n"))
    {
        If ($Line.Equals("END RESULTS:"))
        {
            $ResultLine = "Out Result";
        }
        If ($ResultLine.Equals("In Result"))
        {
            $ReturnValue += [String]::Format("{0}`r`n", $Line);
        }
        If ($Line.Equals("BEGIN RESULTS:"))
        {
            $ResultLine = "In Result";
        }
    }
    If ($ReturnValue.EndsWith("`n"))
    {
        $ReturnValue = $ReturnValue.Substring(0, $ReturnValue.Length - 1);
    }
    If ($ReturnValue.EndsWith("`r"))
    {
        $ReturnValue = $ReturnValue.Substring(0, $ReturnValue.Length - 1);
    }
    Return $ReturnValue;
}

Function Invoke-DataMoverHelp()
{
    [OutputType([String])]
    Param
    (
        [Parameter(Mandatory=$false)]
        [String] $Command
    )
    [String] $ReturnValue = [String]::Empty;
    [System.Diagnostics.ProcessStartInfo] $ProcessStartInfo = [System.Diagnostics.ProcessStartInfo]::new();
    $ProcessStartInfo.FileName = $DataMoverExecutablePath
    $ProcessStartInfo.RedirectStandardError = $true;
    $ProcessStartInfo.RedirectStandardOutput = $true;
    $ProcessStartInfo.RedirectStandardInput = $true;
    $ProcessStartInfo.UseShellExecute = $false;
    If ($Command)
    {
        $ProcessStartInfo.Arguments = @(
            "help",
    
            [String]::Format("-c `"{0}`"", $Command)
        );
    }
    Else
    {
        $ProcessStartInfo.Arguments = @("help")
    }
    [System.Diagnostics.Process] $Process = [System.Diagnostics.Process]::new();
    $Process.StartInfo = $ProcessStartInfo;
    [void] $Process.Start();
    [void] $Process.WaitForExit();
    $ReturnValue = $Process.StandardOutput.ReadToEnd();
    Return $ReturnValue;
}
