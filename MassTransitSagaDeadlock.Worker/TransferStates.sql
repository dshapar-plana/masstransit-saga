IF EXISTS (SELECT * FROM sysobjects WHERE name='TransferStates' and xtype='U')
BEGIN
  DROP TABLE [dbo].[TransferStates]
END

CREATE TABLE [dbo].[TransferStates] (
  [TransferFundsID] nvarchar(256) NOT NULL,
  [CreatedAtDatetime] datetime2 NOT NULL,
  [StartedAtDatetime] datetime2 NOT NULL,
  [CurrentState] nvarchar(256) NOT NULL,
  [FromWalletID] int NOT NULL,
  [ToWalletID] int NOT NULL,
  [Amount] decimal (18) NOT NULL,
  [Comment] nvarchar(MAX) NOT NULL,
  [Errors] nvarchar(MAX) NULL,
  [Metadata] nvarchar(MAX) NULL,
  [ExternalTransactionID] varchar(256) NULL,
  [Hash] varchar(64) NOT NULL,
  [Salt] varchar(64) NOT NULL
);

DROP INDEX IF EXISTS nc_TransferStates_Report
ON [dbo].[TransferStates];

CREATE NONCLUSTERED INDEX nc_TransferLineItems_Report
  ON [dbo].[TransferStates] (FromWalletID, TransferFundsID, CurrentState, ExternalTransactionID);

GO