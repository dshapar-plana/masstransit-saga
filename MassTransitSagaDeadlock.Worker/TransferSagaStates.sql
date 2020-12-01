--IF EXISTS (SELECT * FROM sysobjects WHERE name='TransferSagaStates' and xtype='U')
--BEGIN
--  DROP TABLE [dbo].[TransferSagaStates]
--END

USE [master]
GO

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'masstransit_saga')
BEGIN
  CREATE DATABASE [masstransit_saga]
END
GO
 
USE [masstransit_saga]
GO

 
IF EXISTS(select * from sys.databases WHERE name=DB_NAME())
BEGIN
  DECLARE @SQL nvarchar(max) = 'ALTER DATABASE "'+db_name()+'" SET RECOVERY SIMPLE'; 
  EXEC sys.sp_ExecuteSQL @stmt=@SQL;
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TransferSagaStates' and xtype='U')
BEGIN

CREATE TABLE [dbo].[TransferSagaStates] (
  [CorrelationID] uniqueidentifier NOT NULL CONSTRAINT pk_TransferSagaStates_CorrelationID PRIMARY KEY CLUSTERED,
  [CreatedAtDatetime] datetime2 NOT NULL,
  [StartedAtDatetime] datetime2 NOT NULL,
  [CurrentState] nvarchar(256) NOT NULL,
  [FromWalletID] int NOT NULL,
  [ToWalletID] int NOT NULL,
  [Amount] decimal(18) NOT NULL,
  [Comment] nvarchar(MAX) NOT NULL,
  [Errors] nvarchar(MAX) NULL,
  [Metadata] nvarchar(MAX) NULL,
  [ExternalTransactionId] VARCHAR (256) NULL,
  [Hash] varchar(64) NOT NULL,
  [Salt] varchar(64) NOT NULL
);
END

--IF EXISTS (SELECT * FROM sysobjects WHERE name='TransferSagaState' and xtype='U')
--BEGIN
--  DROP TABLE [dbo].[TransferSagaState]
--END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TransferSagaState' and xtype='U')
BEGIN
 
CREATE TABLE [dbo].[TransferSagaState] (
  [CorrelationID] uniqueidentifier NOT NULL CONSTRAINT pk_TransferSagaState_CorrelationID PRIMARY KEY CLUSTERED,
  [CreatedAtDatetime] datetime2 NOT NULL,
  [StartedAtDatetime] datetime2 NOT NULL,
  [CurrentState] nvarchar(256) NOT NULL,
  [FromWalletID] int NOT NULL,
  [ToWalletID] int NOT NULL,
  [Amount] decimal(18) NOT NULL,
  [Comment] nvarchar(MAX) NOT NULL,
  [Errors] nvarchar(MAX) NULL,
  [Metadata] nvarchar(MAX) NULL,
  [ExternalTransactionId] VARCHAR (256) NULL,
  [Hash] varchar(64) NOT NULL,
  [Salt] varchar(64) NOT NULL
);
END
