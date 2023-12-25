using BDMCommandLine;

namespace DataMover.Core
{
    public static class CommandArguments
    {
        public static CommandArgumentBase LoggingLevel => CommandArgumentBase.CreateOptionArgument("LoggingLevel", "ll", "The level of output information to return", false,
            new CommandArgumentOptionBase[]{
                new ("Quiet", "No output"),
                new ("Exception", "Only exceptions"),
                new ("Verbose", "All messages"),
            },
            "Exception"
        );


        public static CommandArgumentBase SQLServerConnectionString => CommandArgumentBase.CreateSimpleArgument("SQLServerConnectionString", "ssConn", "Connection string for SQL Server", true);
        public static CommandArgumentBase PostgreSQLConnectionString => CommandArgumentBase.CreateSimpleArgument("PostgreSQLConnectionString", "pgConn", "Connection string for PostgreSQL", true);
        public static CommandArgumentBase DelimitedFilePath => CommandArgumentBase.CreateSimpleArgument("DelimitedFilePath", "dfPath", "File path for delimited file", true);


        public static CommandArgumentBase SourceSchema => CommandArgumentBase.CreateSimpleArgument("SourceSchema", "srcSchema", "Source schema", true);
        public static CommandArgumentBase SourceTable => CommandArgumentBase.CreateSimpleArgument("SourceTable", "srcTable", "Source table or view", true);


        public static CommandArgumentBase TargetSchema => CommandArgumentBase.CreateSimpleArgument("TargetSchema", "trgSchema", "Target schema", true);
        public static CommandArgumentBase TargetTable => CommandArgumentBase.CreateSimpleArgument("TargetTable", "trgTable", "Target table", true);
        public static CommandArgumentBase TruncateTarget => CommandArgumentBase.CreateFlagArgument("TruncateTarget", "trunc", "Truncates the target table before copy.", false, false);

        public static CommandArgumentBase ColumnMatchingMethod => CommandArgumentBase.CreateOptionArgument("ColumnMatchingMethod", "m", "The method by wich to match columns between source and target", false,
            new CommandArgumentOptionBase[]{
                new ("Name", "Match on name (case insensitive)."),
                new ("Position", "Match on ordinal position.")
            },
            "Name"
        );


        public static CommandArgumentBase Query => CommandArgumentBase.CreateSimpleArgument("Query", "q", "Query to execute", true);


        public static CommandArgumentBase ColumnDelimiter => CommandArgumentBase.CreateSimpleArgument("ColumnDelimiter", "cd", "Delimiter to use for columns", false, ",");
        public static CommandArgumentBase HasHeaderRow => CommandArgumentBase.CreateFlagArgument("HasHeaderRow", "head", "Delimited file has header row.", false, true);

        public static CommandArgumentBase Columns => CommandArgumentBase.CreateArrayArgument("Columns", "cols", "Comma separated list of column names.", false);
    }
}
