IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'MasstransitSaga')
BEGIN
  CREATE DATABASE [MasstransitSaga];
END 