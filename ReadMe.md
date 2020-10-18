# Always Encrypted.

## Nuget Packages:

- Install-Package Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider
- Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory

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
- Have to use ADO.NET to handle encryption / decryption. No Linq2Sql yet.
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

