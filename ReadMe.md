# AlwaysEncrypted.Poc
The purpose of this POC (Proof of Concept) is to experiment with Always Encrypted and see how it can be used with .NET Core Code First Migrations. I have found it works well, although there are some trade off’s, documented later in this document.

### Tech
AlwaysEncripted.Poc consists of two projects:

**AlwaysEncrypted.Entities**
Traditional .NET Core class library, facilitation Code First Migrations.
**AlwaysEncrypted.Poc**
.NET Core console app configured to perform CRUD (Create, Read, Update, Delete) database operations.

### Installation
Ensure you have the following installed:

* [.NET Core](https://dotnet.microsoft.com/download)
* [SQL Server 2019](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads)
* [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
* [.NET Core Code First Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
* [Always Encrypted](https://docs.microsoft.com/en-us/sql/relational-databases/security/encryption/always-encrypted-database-engine?view=sql-server-ver15)
* [Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider](https://www.nuget.org/packages/Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider/)
* [Microsoft.IdentityModel.Clients.ActiveDirectory](https://www.nuget.org/packages/Microsoft.IdentityModel.Clients.ActiveDirectory/)
* You will need to create login user on your database and update AlwaysEncrypted.Poc/appsettings.Developement.json

This POC implements [Always Encrypted Using Windows Certificate Store](https://docs.microsoft.com/en-us/azure/azure-sql/database/always-encrypted-certificate-store-configure), so you will need to: 

**Add User Login**
```sql
CREATE USER [{user-name}]
WITH PASSWORD = '{password-that-adheres-to-azure-password-policy}'
, DEFAULT_SCHEMA = dbo;

--  Role/s.
ALTER ROLE {some_role} ADD MEMBER [{user-name}] 
```
**Encrypt Columns with (SSMS)**
1. Right click on table Patient & select Encrypt Columns...
2. Check column Ssn and set encryption type **Deterministic**
3. Check column BirthDate and set encryption type **Randomized**

Follow reference: [Getting Started with Always Encrypted with SSMS](https://channel9.msdn.com/Shows/Data-Exposed/Getting-Started-with-Always-Encrypted-with-SSMS)

## Encryption

Encryption Types:
- Randomized
	High security
	Can't query, comparisons, ... This may introduce further complications if you need to perform aggregations, calculations or other

- Deterministic
	Can query
	Joins
	Group By
	Less Security

##  Master Key Configurations

**CMK**
Column
Master
Key

**CEK**
Column
Encryption
Key

## Pros & Cons

### Pros
- Relatively easy to use
- Encryption is handled by the framework 
- Works well with Code First Migrations
- Azure Key Vault or Windows Certificate Store
- You can choose encryption types and what columns you want to encrypt
- You can continue to use Linq as long as you don’t use the encrypted columns. Not sure if this is a pro ;-D

### Cons
- Have to use [ADO.NET](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/) to handle encryption / decryption. No Linq2Sql yet.
- Code First Migrations cannot modify already encrypted columns however, I didn’t try executing TSQL in MigrationBuilder.
- Whilst I don’t have a lot of experience with reusable patterns for ADO.NET, I believe there is a reduction in reusable code.
- More boiler plate code needs to be written as opposed to database operations with Entity Framework and Linq.

## Resources

**Summary**
https://channel9.msdn.com/Shows/Data-Exposed/Getting-Started-with-Always-Encrypted-with-SSMS
**Always Encrypted Using Windows Certificate Store**
https://docs.microsoft.com/en-us/azure/azure-sql/database/always-encrypted-certificate-store-configure

**Always Encrypted Using Azure Key Vault**
https://docs.microsoft.com/en-us/azure/azure-sql/database/always-encrypted-azure-key-vault-configure?tabs=azure-powershell

