import os
import subprocess

dataMoverExecutablePath = os.path.join(os.path.dirname(os.path.realpath(__file__)), "DataMover.exe")
if not dataMoverExecutablePath:
    raise Exception("Unable to determine DataMover.exe path.")
if not os.path.isfile(dataMoverExecutablePath):
    raise FileNotFoundError(dataMoverExecutablePath)

class DataMoverResult:
    def __init__(self, command, returnCode, standardOutput, standardError, value = None):
        self.Command = command
        self.ReturnCode = returnCode
        self.StandardOutput = standardOutput
        self.StandardError = standardError
        self.Value = value

def GetVersion():
    cmdArgs = [dataMoverExecutablePath, "version"]
    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def GetHelp(command = ""):
    cmdArgs = [dataMoverExecutablePath, "help"]
    if command:
        cmdArgs.append('"' + command + '"')
    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

#region PostgreSQL
def PG2SS(postgreSQLConnectionString,
        postgreSQLSchema,
        postgreSQLTableOrView,
        sqlServerConnectionString,
        sqlServerSchema,
        sqlServerTable,
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "pg2df"]
    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-srcSchema")
    cmdArgs.append('"' + postgreSQLSchema + '"')

    cmdArgs.append("-srcTable")
    cmdArgs.append('"' + postgreSQLTableOrView + '"')

    cmdArgs.append("-ssConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-trgSchema")
    cmdArgs.append('"' + sqlServerSchema + '"')

    cmdArgs.append("-trgTable")
    cmdArgs.append('"' + sqlServerTable + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if truncateTargetTable:
        cmdArgs.append("-trunc")

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def PG2DF(postgreSQLConnectionString,
        postgreSQLSchema,
        postgreSQLTableOrView,
        delimitedFilePath,
        columnDelimiter = ",",
        hasHeaderRow = False,
        columns = [],
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "pg2df"]
    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-srcSchema")
    cmdArgs.append('"' + postgreSQLSchema + '"')

    cmdArgs.append("-srcTable")
    cmdArgs.append('"' + postgreSQLTableOrView + '"')

    cmdArgs.append("-dfPath")
    cmdArgs.append('"' + delimitedFilePath + '"')

    cmdArgs.append("-cd")
    if columnDelimiter == "\t":
        columnDelimiter = "{tab}"
    cmdArgs.append('"' + columnDelimiter + '"')

    if hasHeaderRow:
        cmdArgs.append("-head")

    if columns:
        cmdArgs.append("-cols")
        cmdArgs.append('"' + ",".join(columns) + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if (
        truncateTargetTable
        and os.path.isfile(delimitedFilePath)
        ):
        cmdArgs.append("-trunc")    

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def PGQE(postgreSQLConnectionString,
        query,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "pgqe"]
    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-q")
    cmdArgs.append('"' + query + '"')

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def PGQES(postgreSQLConnectionString,
        query,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "pgqes"]
    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-q")
    cmdArgs.append('"' + query + '"')

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    scalarValue = ""
    if result.stdout:
        resultLine = "Other"
        for line in result.stdout.replace("\r\n", "\n").replace("\r", "\n").splitlines():
            if line == "END RESULTS:":
                resultLine = "Out Result"
            if resultLine == "In Result":
                scalarValue += f'{line}\n'
            if line == "BEGIN RESULTS:":
                resultLine = "In Result"
        if scalarValue.endswith('\n'):
            scalarValue = scalarValue[:-1]
        if scalarValue.endswith('\r'):
            scalarValue = scalarValue[:-1]
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, scalarValue)
#endregion PostgreSQL

#region SQL Server
def SS2PG(sqlServerConnectionString,
        sqlServerSchema,
        sqlServerTableOrView,
        postgreSQLConnectionString,
        postgreSQLSchema,
        postgreSQLTable,
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ss2df"]
    cmdArgs.append("-ssConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-srcSchema")
    cmdArgs.append('"' + sqlServerSchema + '"')

    cmdArgs.append("-srcTable")
    cmdArgs.append('"' + sqlServerTableOrView + '"')

    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-trgSchema")
    cmdArgs.append('"' + postgreSQLSchema + '"')

    cmdArgs.append("-trgTable")
    cmdArgs.append('"' + postgreSQLTable + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if truncateTargetTable:
        cmdArgs.append("-trunc")

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def SS2DF(sqlServerConnectionString,
        sqlServerSchema,
        sqlServerTableOrView,
        delimitedFilePath,
        columnDelimiter = ",",
        hasHeaderRow = False,
        columns = [],
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ss2df"]
    cmdArgs.append("-ssConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-srcSchema")
    cmdArgs.append('"' + sqlServerSchema + '"')

    cmdArgs.append("-srcTable")
    cmdArgs.append('"' + sqlServerTableOrView + '"')

    cmdArgs.append("-dfPath")
    cmdArgs.append('"' + delimitedFilePath + '"')

    cmdArgs.append("-cd")
    if columnDelimiter == "\t":
        columnDelimiter = "{tab}"
    cmdArgs.append('"' + columnDelimiter + '"')

    if hasHeaderRow:
        cmdArgs.append("-head")

    if columns:
        cmdArgs.append("-cols")
        cmdArgs.append('"' + ",".join(columns) + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if (
        truncateTargetTable
        and os.path.isfile(delimitedFilePath)
        ):
        cmdArgs.append("-trunc")    

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def SSQE(sqlServerConnectionString,
        query,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ssqe"]
    cmdArgs.append("-ssConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-q")
    cmdArgs.append('"' + query + '"')

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def SSQES(sqlServerConnectionString,
        query,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ssqes"]
    cmdArgs.append("-ssConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-q")
    cmdArgs.append('"' + query + '"')

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    scalarValue = ""
    if result.stdout:
        resultLine = "Other"
        for line in result.stdout.replace("\r\n", "\n").replace("\r", "\n").splitlines():
            if line == "END RESULTS:":
                resultLine = "Out Result"
            if resultLine == "In Result":
                scalarValue += f'{line}\n'
            if line == "BEGIN RESULTS:":
                resultLine = "In Result"
        if scalarValue.endswith('\n'):
            scalarValue = scalarValue[:-1]
        if scalarValue.endswith('\r'):
            scalarValue = scalarValue[:-1]
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, scalarValue)
#endregion SQL Server

#region Delimited File
def DF2PG(delimitedFilePath,
        postgreSQLConnectionString,
        postgreSQLSchema,
        postgreSQLTable,
        columnDelimiter = ",",
        hasHeaderRow = False,
        columns = [],
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ss2df"]
    cmdArgs.append("-dfPath")
    cmdArgs.append('"' + delimitedFilePath + '"')

    cmdArgs.append("-cd")
    if columnDelimiter == "\t":
        columnDelimiter = "{tab}"
    cmdArgs.append('"' + columnDelimiter + '"')

    if hasHeaderRow:
        cmdArgs.append("-head")

    if columns:
        cmdArgs.append("-cols")
        cmdArgs.append('"' + ",".join(columns) + '"')

    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + postgreSQLConnectionString + '"')

    cmdArgs.append("-trgSchema")
    cmdArgs.append('"' + postgreSQLSchema + '"')

    cmdArgs.append("-trgTable")
    cmdArgs.append('"' + postgreSQLTable + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if truncateTargetTable:
        cmdArgs.append("-trunc")

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)

def DF2SS(delimitedFilePath,
        sqlServerConnectionString,
        sqlServerSchema,
        sqlServerTable,
        columnDelimiter = ",",
        hasHeaderRow = False,
        columns = [],
        columnMatchingMethod = "Name",
        truncateTargetTable = False,
        loggingLevel = "Exception"):
    cmdArgs = [dataMoverExecutablePath, "ss2df"]
    cmdArgs.append("-dfPath")
    cmdArgs.append('"' + delimitedFilePath + '"')

    cmdArgs.append("-cd")
    if columnDelimiter == "\t":
        columnDelimiter = "{tab}"
    cmdArgs.append('"' + columnDelimiter + '"')

    if hasHeaderRow:
        cmdArgs.append("-head")

    if columns:
        cmdArgs.append("-cols")
        cmdArgs.append('"' + ",".join(columns) + '"')

    cmdArgs.append("-pgConn")
    cmdArgs.append('"' + sqlServerConnectionString + '"')

    cmdArgs.append("-trgSchema")
    cmdArgs.append('"' + sqlServerSchema + '"')

    cmdArgs.append("-trgTable")
    cmdArgs.append('"' + sqlServerTable + '"')

    cmdArgs.append("-m")
    cmdArgs.append('"' + columnMatchingMethod + '"')

    if truncateTargetTable:
        cmdArgs.append("-trunc")

    cmdArgs.append("-ll")
    cmdArgs.append('"' + loggingLevel + '"')

    result = subprocess.run(cmdArgs, capture_output=True, text=True)
    return DataMoverResult(" ".join(cmdArgs), result.returncode, result.stdout, result.stderr, None)
#endregion Delimited File
