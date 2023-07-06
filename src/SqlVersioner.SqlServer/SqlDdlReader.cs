namespace SqlVersioner.SqlServer
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using SqlVersioner.Abstractions.Database;
  using SqlVersioner.Abstractions.Logging;
  
  /// <summary>
  /// <see cref="ISqlDdlReader"/> Sql DDL reader implementation for SQL Server.
  /// </summary>
  /// <remarks>
  ///  <para>This class is used to read SQL DDL objects.</para>
  /// </remarks>
  public class SqlDdlReader : ISqlDdlReader
  {
    private readonly ISqlConnectionFactory factory;
    private readonly ILogger logger;

    public SqlDdlReader(ISqlConnectionFactory factory)
    {
      if (factory == null) throw new ArgumentNullException(nameof(factory), "The sql connection factory cannot be null.");
      
      this.factory = factory;
      this.logger = factory.Logger;
    }

    public ILogger Logger
    {
      get { return this.logger; }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SqlObject>> OpenSqlSchemasAsync(CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();

      Logger.LogDetailed(" >>>> Reading Schemas");
      return ReadAsync(@"SELECT S.NAME  AS [SCHEMA_NAME],
S.NAME  AS [OBJECT_NAME],
'SCHEMA' AS [TYPE],
'CREATE SCHEMA ' + S.NAME + ';' AS [DEFINITION]
FROM SYS.SCHEMAS S
WHERE S.NAME NOT IN ('GUEST', 'SYS', 'DB_OWNER', 'DB_ACCESSADMIN', 'DB_SECURITYADMIN', 'DB_DDLADMIN',
                     'DB_BACKUPOPERATOR', 'DB_DATAREADER', 'DB_DATAWRITER', 'DB_DENYDATAREADER', 'DB_DENYDATAWRITER');", cancelToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SqlObject>> OpenSqlTablesAsync(CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();
      
      Logger.LogNormal(" >>>> Reading Tables");
      return ReadAsync(@"WITH ALL_TABLES AS (
SELECT SCHEMA_NAME(O.[SCHEMA_ID])                AS [SCHEMA_NAME],
       O.[NAME]                                    AS [OBJECT_NAME],
       'TABLE'                                   AS [TYPE],
       SCHEMA_NAME(O.[SCHEMA_ID]) + '.' + O.NAME AS [FULL_NAME]
FROM SYS.ALL_OBJECTS O
WHERE O.TYPE = 'U' AND schema_name(o.[SCHEMA_ID]) <> 'sys')
SELECT T.[SCHEMA_NAME], T.[OBJECT_NAME], T.[TYPE], dbo.FN_EXPORT_TABLE_DDL(T.FULL_NAME) AS [DEFINITION] FROM ALL_TABLES T", cancelToken);
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SqlObject>> OpenSqlFunctionsAsync(CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();

      Logger.LogNormal(" >>>> Reading Functions");
      return ReadAsync(@"WITH ALL_FUNCTIONS AS (
SELECT T.SCHEMA_NAME, T.OBJECT_NAME, OBJECT_ID(T.[FULL_NAME]) AS [OBJECT_ID] , T.FULL_NAME FROM (
SELECT UPPER(SCHEMA_NAME(O.[SCHEMA_ID])) AS [SCHEMA_NAME], O.[NAME] AS [OBJECT_NAME], '[' + UPPER(SCHEMA_NAME(O.[SCHEMA_ID])) + '].[' + O.[NAME] + ']' AS [FULL_NAME]
FROM SYS.ALL_OBJECTS O WHERE TYPE = 'FN' AND SCHEMA_NAME(SCHEMA_ID) <> 'SYS') T)
SELECT F.[SCHEMA_NAME], F.[OBJECT_NAME], 'FUNCTION' AS [TYPE], [DEFINITION]
FROM SYS.SQL_MODULES M
INNER JOIN ALL_FUNCTIONS F ON F.OBJECT_ID = M.OBJECT_ID;", cancelToken);
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SqlObject>> OpenSqlProceduresAsync(CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();
      
      Logger.LogNormal(" >>>> Reading Procedures");
      return ReadAsync(@"WITH ALL_PROCEDURES AS (
SELECT T.SCHEMA_NAME, T.OBJECT_NAME, OBJECT_ID(T.[FULL_NAME]) AS [OBJECT_ID] , T.FULL_NAME FROM (
SELECT UPPER(SCHEMA_NAME(O.[SCHEMA_ID])) AS [SCHEMA_NAME], O.[NAME] AS [OBJECT_NAME], '[' + UPPER(SCHEMA_NAME(O.[SCHEMA_ID])) + '].[' + O.[NAME] + ']' AS [FULL_NAME]
FROM SYS.ALL_OBJECTS O WHERE TYPE = 'P' AND SCHEMA_NAME(SCHEMA_ID) <> 'SYS') T)
SELECT P.[SCHEMA_NAME], P.[OBJECT_NAME], 'PROCEDURE' AS [TYPE], [DEFINITION]
FROM SYS.SQL_MODULES M
INNER JOIN ALL_PROCEDURES P ON P.OBJECT_ID = M.OBJECT_ID;", cancelToken);
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SqlObject>> OpenSqlViewsAsync(CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();
      
      Logger.LogNormal(" >>>> Reading Views");
      return ReadAsync(@"WITH ALL_VIEWS AS (
SELECT V.[FULL_NAME], V.[SCHEMA], V.[OBJECT_NAME], OBJECT_ID(V.FULL_NAME) AS [OBJECT_ID] FROM (
SELECT UPPER(CTE.[TABLE_SCHEMA]) AS [SCHEMA]
    , UPPER(CTE.[TABLE_NAME]) AS [OBJECT_NAME]
    , UPPER('[' + CTE.TABLE_SCHEMA + '].[' + CTE.TABLE_NAME + ']') AS [FULL_NAME]
FROM INFORMATION_SCHEMA.VIEWS CTE) V)
SELECT V.[SCHEMA], V.[OBJECT_NAME], 'VIEW' AS [TYPE], [DEFINITION]
FROM SYS.SQL_MODULES M
INNER JOIN ALL_VIEWS V ON V.OBJECT_ID = M.OBJECT_ID;", cancelToken);
    }
    
    private async Task<IReadOnlyList<SqlObject>> ReadAsync(string sqlCommand, CancellationToken cancelToken = default)
    {
      cancelToken.ThrowIfCancellationRequested();
      
      await CreateAsync(cancelToken);
      
      if (string.IsNullOrEmpty(sqlCommand)) throw new ArgumentNullException(nameof(sqlCommand), "The sql command cannot be null or empty.");

      await using (var connection = await factory.CreateConnectionAsync(cancelToken).ConfigureAwait(false))
      await using (var dataReader = await connection.ExecuteReaderAsync(sqlCommand, cancelToken).ConfigureAwait(false))
      {
        var rows = new List<SqlObject>();
        await foreach (var value in dataReader
                         .AsAsyncEnumerable()
                         .ConfigureAwait(false)
                         .WithCancellation(cancelToken))
          rows.Add(value);
        return rows.AsReadOnly();
      }
    }

    private async ValueTask CreateAsync(CancellationToken cancelToken = default)
    {
      await using (var connection = await factory.CreateConnectionAsync(cancelToken).ConfigureAwait(false))
      {
        await connection.ExecuteNonQueryAsync(@"
    IF NOT EXISTS ( SELECT TOP 1 * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FN_EXPORT_TABLE_DDL]') )
        BEGIN
        EXECUTE ('CREATE FUNCTION [DBO].FN_EXPORT_TABLE_DDL(@TABLE_NAME SYSNAME) RETURNS NVARCHAR(MAX)
              BEGIN
                DECLARE @OBJECT_NAME SYSNAME, @OBJECT_ID INT;

                /* BEGIN - OBJECT_NAME, OBJECT_ID */
                SELECT @OBJECT_NAME = ''['' + S.NAME + ''].['' + O.NAME + '']'', @OBJECT_ID = O.[OBJECT_ID]
                FROM SYS.OBJECTS O WITH (NOWAIT)
                             JOIN SYS.SCHEMAS S WITH (NOWAIT) ON O.[SCHEMA_ID] = S.[SCHEMA_ID]
                WHERE S.NAME + ''.'' + O.NAME = @TABLE_NAME
                  AND O.[TYPE] = ''U''
                  AND O.IS_MS_SHIPPED = 0;
                /* END - OBJECT_NAME, OBJECT_ID */
                
                DECLARE @SQL NVARCHAR(MAX) = '''';
                    
                /* BEGIN - INDEX-COLUMN */
                WITH INDEX_COLUMN AS
                         (SELECT IC.[OBJECT_ID]
                               , IC.INDEX_ID
                               , IC.IS_DESCENDING_KEY
                               , IC.IS_INCLUDED_COLUMN
                               , C.NAME
                          FROM SYS.INDEX_COLUMNS IC WITH (NOWAIT)
                    JOIN SYS.COLUMNS C WITH (NOWAIT) ON IC.[OBJECT_ID] = C.[OBJECT_ID] AND IC.COLUMN_ID = C.COLUMN_ID
                WHERE IC.[OBJECT_ID] = @OBJECT_ID),
                /* END - INDEX_COLUMN  */
                    
                /*BEGIN - FK_COLUMNS */
                    FK_COLUMNS AS
                    (SELECT K.CONSTRAINT_OBJECT_ID
                        , CNAME  = C.NAME
                        , RCNAME = RC.NAME
                    FROM SYS.FOREIGN_KEY_COLUMNS K WITH (NOWAIT)
                    JOIN SYS.COLUMNS RC WITH (NOWAIT)
                        ON RC.[OBJECT_ID] = K.REFERENCED_OBJECT_ID AND RC.COLUMN_ID = K.REFERENCED_COLUMN_ID
                    JOIN SYS.COLUMNS C WITH (NOWAIT)
                        ON C.[OBJECT_ID] = K.PARENT_OBJECT_ID AND C.COLUMN_ID = K.PARENT_COLUMN_ID
                    WHERE K.PARENT_OBJECT_ID = @OBJECT_ID)
                /*END - FK_COLUMNS */
                
                    SELECT @SQL =
               /* BEGIN - CREATE TABLE */
                       ''CREATE TABLE '' + @OBJECT_NAME + CHAR(13) + ''('' + CHAR(13) + STUFF((SELECT CHAR(9) + '', ['' + C.NAME + ''] '' +
                        IIF(C.IS_COMPUTED = 1, ''AS '' + CC.[DEFINITION],
                    UPPER(TP.NAME) +
                    CASE
                    WHEN TP.NAME IN (''VARCHAR'', ''CHAR'', ''VARBINARY'', ''BINARY'', ''TEXT'')
                    THEN ''('' +
                    IIF(C.MAX_LENGTH = -1, ''MAX'', CAST(C.MAX_LENGTH AS VARCHAR(5))) +
                    '')''
                    WHEN TP.NAME IN (''NVARCHAR'', ''NCHAR'', ''NTEXT'')
                    THEN ''('' + IIF(
                    C.MAX_LENGTH =
                    -1, ''MAX'',
                    CAST(C.MAX_LENGTH / 2 AS VARCHAR(5))) +
                    '')''
                    WHEN TP.NAME IN (''DATETIME2'', ''TIME2'', ''DATETIMEOFFSET'')
                    THEN ''('' + CAST(C.SCALE AS VARCHAR(5)) + '')''
                    WHEN TP.NAME = ''DECIMAL''
                    THEN ''('' +
                    CAST(C.[PRECISION] AS VARCHAR(5)) +
                    '','' +
                    CAST(C.SCALE AS VARCHAR(5)) +
                    '')''
                    ELSE ''''
                    END +
                    IIF(C.COLLATION_NAME IS NOT NULL, '' COLLATE '' + C.COLLATION_NAME, '''') +
                    IIF(C.IS_NULLABLE = 1, '' NULL'', '' NOT NULL'') +
                    IIF(DC.[DEFINITION] IS NOT NULL, '' DEFAULT'' + DC.[DEFINITION], '''') +
                    IIF(IC.IS_IDENTITY = 1, '' IDENTITY('' +
                    CAST(ISNULL(IC.SEED_VALUE, ''0'') AS CHAR(1)) + '','' +
                    CAST(ISNULL(IC.INCREMENT_VALUE, ''1'') AS CHAR(1)) + '')'', '''')) + CHAR(13)
                    FROM SYS.COLUMNS C WITH (NOWAIT)
                    JOIN SYS.TYPES TP WITH (NOWAIT) ON C.USER_TYPE_ID = TP.USER_TYPE_ID
                    LEFT JOIN SYS.COMPUTED_COLUMNS CC WITH (NOWAIT)
                        ON C.[OBJECT_ID] = CC.[OBJECT_ID] AND C.COLUMN_ID = CC.COLUMN_ID
                    LEFT JOIN SYS.DEFAULT_CONSTRAINTS DC WITH (NOWAIT)
                        ON C.DEFAULT_OBJECT_ID != 0 AND
                        C.[OBJECT_ID] = DC.PARENT_OBJECT_ID AND
                        C.COLUMN_ID = DC.PARENT_COLUMN_ID
                    LEFT JOIN SYS.IDENTITY_COLUMNS IC WITH (NOWAIT)
                    ON C.IS_IDENTITY = 1 AND
                    C.[OBJECT_ID] = IC.[OBJECT_ID] AND
                    C.COLUMN_ID = IC.COLUMN_ID
                    WHERE C.[OBJECT_ID] = @OBJECT_ID
                    ORDER BY C.COLUMN_ID
                    FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''),
                    1, 2, CHAR(9) + '' '')
                    + ISNULL((SELECT CHAR(9) + '', CONSTRAINT ['' + K.NAME + ''] PRIMARY KEY ('' +
                     (SELECT STUFF((SELECT '', ['' + C.NAME + ''] '' +
                    CASE WHEN IC.IS_DESCENDING_KEY = 1 THEN ''DESC'' ELSE ''ASC'' END
                    FROM SYS.INDEX_COLUMNS IC WITH (NOWAIT)
                    JOIN SYS.COLUMNS C WITH (NOWAIT)
                    ON C.[OBJECT_ID] = IC.[OBJECT_ID] AND C.COLUMN_ID = IC.COLUMN_ID
                    WHERE IC.IS_INCLUDED_COLUMN = 0
                    AND IC.[OBJECT_ID] = K.PARENT_OBJECT_ID
                    AND IC.INDEX_ID = K.UNIQUE_INDEX_ID
                    FOR XML PATH(N''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 2, '''')) + '')'' + CHAR(13)
                    FROM SYS.KEY_CONSTRAINTS K WITH (NOWAIT)
                    WHERE K.PARENT_OBJECT_ID = @OBJECT_ID
                    AND K.[TYPE] = ''PK''), '''') + '')'' + CHAR(13)
                    + ISNULL((SELECT (SELECT CHAR(13) +
                    ''ALTER TABLE '' + @OBJECT_NAME + '' WITH''
                    + CASE
                        WHEN FK.IS_NOT_TRUSTED = 1
                        THEN '' NOCHECK''
                        ELSE '' CHECK''
                    END +
                    /* BEGIN - FK CONSTRAINT */
                    '' ADD CONSTRAINT ['' + FK.NAME + ''] FOREIGN KEY(''
                    + STUFF((SELECT '', ['' + K.CNAME + '']''
                        FROM FK_COLUMNS K
                        WHERE K.CONSTRAINT_OBJECT_ID = FK.[OBJECT_ID]
                        FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 2, '''')
                    + '')'' +
                    '' REFERENCES ['' + SCHEMA_NAME(RO.[SCHEMA_ID]) + ''].['' + RO.NAME + ''] (''
                    + STUFF((SELECT '', ['' + K.RCNAME + '']''
                        FROM FK_COLUMNS K
                        WHERE K.CONSTRAINT_OBJECT_ID = FK.[OBJECT_ID]
                        FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 2, '''')
                    + '')''
                    + CASE
                        WHEN FK.DELETE_REFERENTIAL_ACTION = 1 THEN '' ON DELETE CASCADE''
                        WHEN FK.DELETE_REFERENTIAL_ACTION = 2 THEN '' ON DELETE SET NULL''
                        WHEN FK.DELETE_REFERENTIAL_ACTION = 3 THEN '' ON DELETE SET DEFAULT''
                    ELSE ''''
                    END
                    + CASE
                        WHEN FK.UPDATE_REFERENTIAL_ACTION = 1 THEN '' ON UPDATE CASCADE''
                        WHEN FK.UPDATE_REFERENTIAL_ACTION = 2 THEN '' ON UPDATE SET NULL''
                        WHEN FK.UPDATE_REFERENTIAL_ACTION = 3 THEN '' ON UPDATE SET DEFAULT''
                    ELSE ''''
                    END
                    /* END - FK CONSTRAINT */
                    
                    /* BEGIN - FK CHECK */
                        + CHAR(13) + ''ALTER TABLE '' + @OBJECT_NAME + '' CHECK CONSTRAINT ['' + FK.NAME + '']'' + CHAR(13)
                    FROM SYS.FOREIGN_KEYS FK WITH (NOWAIT)
                    JOIN SYS.OBJECTS RO WITH (NOWAIT) ON RO.[OBJECT_ID] = FK.REFERENCED_OBJECT_ID
                    WHERE FK.PARENT_OBJECT_ID = @OBJECT_ID
                    FOR XML PATH(N''''), TYPE).value(''.'', ''NVARCHAR(MAX)'')), '''')
                    /* END - FK CHECK */
                    
                    /* BEGIN - INDEXES */
                        + ISNULL(((SELECT CHAR(13) + ''CREATE'' + CASE WHEN I.IS_UNIQUE = 1 THEN '' UNIQUE'' ELSE '''' END
                        + '' NONCLUSTERED INDEX ['' + I.NAME + ''] ON '' + @OBJECT_NAME + '' ('' +
                    STUFF((SELECT '', ['' + C.NAME + '']'' +
                        CASE WHEN C.IS_DESCENDING_KEY = 1 THEN '' DESC'' ELSE '' ASC'' END
                        FROM INDEX_COLUMN C
                        WHERE C.IS_INCLUDED_COLUMN = 0
                        AND C.INDEX_ID = I.INDEX_ID
                        FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 2, '''') + '')''
                        + ISNULL(CHAR(13) + ''INCLUDE ('' + STUFF((SELECT '', ['' + C.NAME + '']''
                            FROM INDEX_COLUMN C
                            WHERE C.IS_INCLUDED_COLUMN = 1
                            AND C.INDEX_ID = I.INDEX_ID
                            FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 2, '''') +
                            '')'', '''') + CHAR(13)
                    FROM SYS.INDEXES I WITH (NOWAIT)
                    WHERE I.[OBJECT_ID] = @OBJECT_ID
                    AND I.IS_PRIMARY_KEY = 0 AND I.[TYPE] = 2 
                    FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)'')
                    ), '''')
                    /* END - INDEXES */
                    RETURN @SQL;
            END ')
        END
", cancelToken);
      }
    }
    
    private async ValueTask DropAsync()
    {
      await using (var connection = await factory.CreateConnectionAsync())
      {
        await connection.ExecuteNonQueryAsync("DROP FUNCTION IF EXISTS [dbo].[FN_EXPORT_TABLE_DDL];");
      }
    }
    
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
      await DropAsync();
    }
  }
}