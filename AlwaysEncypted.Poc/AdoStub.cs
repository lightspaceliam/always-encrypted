using AlwaysEncrypted.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AlwaysEncypted.Poc
{
    public class AdoStub : BaseStub
    {
        public AdoStub(
            IConfiguration configuration,
            ILogger<AdoStub> logger) 
            : base(configuration, logger) { }

        internal override async Task Run()
        {
            const string PATIENT_SSN = "795-73-9838";
            const string UPDATED_PATIENT_SSN = "123-37-6541";
            var birthDate = DateTime.Parse("1980-01-01", Culture);

            var searchedForPatient = await FindBySsnAsync(PATIENT_SSN);

            if(searchedForPatient == null)
            {
                Logger.LogInformation($"\nCREATE\n\nAdoPatient not found, so lets create John Smith.");
                await CreateAsync(new AdoPatient
                {
                    Ssn = PATIENT_SSN,
                    FirstName = "John",
                    LastName = "Smith",
                    BirthDate = birthDate,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                });
            }

            searchedForPatient = await FindBySsnAsync(PATIENT_SSN);
            if (searchedForPatient == null)
            {
                Logger.LogInformation($"Patient not found. Something went wrong.");
            }
            else
            {
                Logger.LogInformation($"\nFIND\n\nAdoPatient found by plain text Ssn {PATIENT_SSN}\nId: {searchedForPatient.Id}\nSsn: {searchedForPatient.Ssn}\nName: {searchedForPatient.FirstName}, {searchedForPatient.LastName}\nBirth Date: {searchedForPatient.BirthDate}");
            }

            Logger.LogInformation($"\nREAD\n\nList of patients.");
            foreach (var patient in await ReadAsync())
            {
                Logger.LogInformation($"\nId: {patient.Id}\nSsn: {patient.Ssn} (Encryption Type Deterministic) Translated back to plain text\nFirst Name: {patient.FirstName} (no encryption)\nLastName: {patient.LastName} (no encryption)\nBirth Date: {patient.BirthDate} (Encryption Type Randomized) Translated back to plain text");
            }

            if (searchedForPatient != null)
            {
                var updatedPatient = await UpdateAsync(new AdoPatient
                {
                    Id = searchedForPatient.Id,
                    FirstName = "Jane",
                    LastName = "Petes",
                    Ssn = UPDATED_PATIENT_SSN,
                    BirthDate = DateTime.Parse("1970-01-01", Culture),
                });
                Logger.LogInformation($"\nUpdate\n\nAdoPatient\nId: {updatedPatient.Id}\nSsn: {updatedPatient.Ssn} (Encryption Type Deterministic) Translated back to plain text\nFirst Name: {updatedPatient.FirstName} (no encryption)\nLastName: {updatedPatient.LastName} (no encryption)\nBirth Date: {updatedPatient.BirthDate} (Encryption Type Randomized) Translated back to plain text");
            }

            Logger.LogInformation($"\nREAD\n\nList of patients after update.");
            foreach (var patient in await ReadAsync())
            {
                Logger.LogInformation($"\nId: {patient.Id}\nSsn: {patient.Ssn} (Encryption Type Deterministic) Translated back to plain text\nFirst Name: {patient.FirstName} (no encryption)\nLastName: {patient.LastName} (no encryption)\nBirth Date: {patient.BirthDate} (Encryption Type Randomized) Translated back to plain text");
            }

            if (searchedForPatient != null)
            {
                Logger.LogInformation($"\nDELETE\n\n");
                await DeleteAsync(searchedForPatient.Id);
            }

            var patients = await ReadAsync();
            Logger.LogInformation($"\nResult\n\nThere are {patients.Count} patients.");
        }

        private async Task CreateAsync(AdoPatient entity)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(GetConnectionString()))
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    await sqlConnection.OpenAsync();
                    sqlCommand.CommandText = @"
                    INSERT INTO [dbo].[AdoPatients] 
                        (
                            [Ssn]
                            , [FirstName]
                            , [LastName]
                            , [BirthDate]
                            , [Created]
                            , [LastModified]
                            ) VALUES (
                            @SSN
                            , @FirstName
                            , @LastName
                            , @BirthDate
                            , @CreatedDate
                            , @LastModifiedDate
                        );
                    ";

                    var sqlParameterSsn = sqlCommand.CreateParameter();
                    sqlParameterSsn.ParameterName = @"@SSN";
                    sqlParameterSsn.DbType = DbType.AnsiStringFixedLength;
                    sqlParameterSsn.Direction = ParameterDirection.Input;
                    sqlParameterSsn.Value = entity.Ssn;
                    sqlParameterSsn.Size = 11;
                    sqlCommand.Parameters.Add(sqlParameterSsn);

                    var sqlParameterFirstName = sqlCommand.CreateParameter();
                    sqlParameterFirstName.ParameterName = @"@FirstName";
                    sqlParameterFirstName.DbType = DbType.String;
                    sqlParameterFirstName.Direction = ParameterDirection.Input;
                    sqlParameterFirstName.Value = entity.FirstName;
                    sqlParameterFirstName.Size = 50;
                    sqlCommand.Parameters.Add(sqlParameterFirstName);

                    var sqlParameterLastName = sqlCommand.CreateParameter();
                    sqlParameterLastName.ParameterName = @"@LastName";
                    sqlParameterLastName.DbType = DbType.String;
                    sqlParameterLastName.Direction = ParameterDirection.Input;
                    sqlParameterLastName.Value = entity.LastName;
                    sqlParameterLastName.Size = 50;
                    sqlCommand.Parameters.Add(sqlParameterLastName);

                    var sqlParameterBirthdate = sqlCommand.CreateParameter();
                    sqlParameterBirthdate.ParameterName = @"@BirthDate";
                    sqlParameterBirthdate.SqlDbType = SqlDbType.Date;
                    sqlParameterBirthdate.Direction = ParameterDirection.Input;
                    sqlParameterBirthdate.Value = entity.BirthDate;
                    sqlCommand.Parameters.Add(sqlParameterBirthdate);

                    var sqlParameterCreated = sqlCommand.CreateParameter();
                    sqlParameterCreated.ParameterName = @"@CreatedDate";
                    sqlParameterCreated.SqlDbType = SqlDbType.DateTime;
                    sqlParameterCreated.Direction = ParameterDirection.Input;
                    sqlParameterCreated.Value = entity.BirthDate;
                    sqlCommand.Parameters.Add(sqlParameterCreated);

                    var sqlParameterLastModified = sqlCommand.CreateParameter();
                    sqlParameterLastModified.ParameterName = @"@LastModifiedDate";
                    sqlParameterLastModified.SqlDbType = SqlDbType.DateTime;
                    sqlParameterLastModified.Direction = ParameterDirection.Input;
                    sqlParameterLastModified.Value = entity.LastModified;
                    sqlCommand.Parameters.Add(sqlParameterLastModified);

                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Create operation error: {ex}");
            }
        }

        private async Task<List<AdoPatient>> ReadAsync()
        {
            var patients = new List<AdoPatient>();
            try
            {
                using (var sqlConnection = new SqlConnection(GetConnectionString()))
                {
                    await sqlConnection.OpenAsync();
                    var query = @"
                        SELECT  [Id]
                                , [FirstName]
                                , [LastName]
                                , [Ssn]
                                , [BirthDate]
                                , [Created]
                                , [LastModified]
                        FROM    [dbo].[AdoPatients]
                        ORDER BY [LastName] ASC;
                    ";
                    using (var sqlCommand = new SqlCommand(query, sqlConnection))
                    using (var sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (!sqlDataReader.HasRows && sqlDataReader.IsClosed)
                        {
                            throw new Exception($"SqlDataReader - Has Rows: {sqlDataReader.HasRows}, Is Closed: {sqlDataReader.IsClosed}.");
                        }
                        while (sqlDataReader.Read())
                        {
                            patients.Add(new AdoPatient
                            {
                                Id = Convert.ToInt32(sqlDataReader["Id"]),
                                Ssn = sqlDataReader["Ssn"].ToString(),
                                FirstName = sqlDataReader["FirstName"].ToString(),
                                LastName = sqlDataReader["LastName"].ToString(),
                                BirthDate = (DateTime)sqlDataReader["BirthDate"],
                                Created = (DateTime)sqlDataReader["Created"],
                                LastModified = (DateTime)sqlDataReader["LastModified"],
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Read operation error: {ex}");
            }
            return patients;
        }

        private async Task<AdoPatient> UpdateAsync(AdoPatient entity)
        {
            try
            {
                var query = $@"
                    UPDATE	[dbo].[AdoPatients]
                    SET		[FirstName]=@FirstName
		                    , [LastName]=@LastName
		                    , [Ssn]=@Ssn
		                    , [BirthDate]=@BirthDate
		                    , [LastModified]=@LastModified
                    WHERE	[Id]=@Id
                ";
                using (var sqlConnection = new SqlConnection(GetConnectionString()))
                using (var sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@FirstName", entity.FirstName);
                    sqlCommand.Parameters.AddWithValue("@LastName", entity.LastName);

                    var sqlParameterSsn = sqlCommand.CreateParameter();
                    sqlParameterSsn.ParameterName = @"@Ssn";
                    sqlParameterSsn.DbType = DbType.AnsiStringFixedLength;
                    sqlParameterSsn.Direction = ParameterDirection.Input;
                    sqlParameterSsn.Value = entity.Ssn;
                    sqlParameterSsn.Size = 11;
                    sqlCommand.Parameters.Add(sqlParameterSsn);

                    var sqlParameterBirthdate = sqlCommand.CreateParameter();
                    sqlParameterBirthdate.ParameterName = @"@BirthDate";
                    sqlParameterBirthdate.SqlDbType = SqlDbType.Date;
                    sqlParameterBirthdate.Direction = ParameterDirection.Input;
                    sqlParameterBirthdate.Value = entity.BirthDate;
                    sqlCommand.Parameters.Add(sqlParameterBirthdate);

                    sqlCommand.Parameters.AddWithValue("@LastModified", DateTime.UtcNow);
                    sqlCommand.Parameters.AddWithValue("@Id", entity.Id);

                    await sqlConnection.OpenAsync();
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Update operation error: {ex}");
            }
            var updatedPatient = await FindBySsnAsync(entity.Ssn);
            return updatedPatient;
        }

        private async Task DeleteAsync(int id)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(GetConnectionString()))
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    await sqlConnection.OpenAsync();

                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    sqlCommand.CommandText = @"
                        DELETE
                        FROM    [dbo].[AdoPatients]
                        WHERE   [Id]=@Id
                    ";

                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Delete operation error: {ex}");
            }
        }

        private async Task<AdoPatient> FindBySsnAsync(string plainTextSsn)
        {
            var patient = new AdoPatient();
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    await connection.OpenAsync();
                    cmd.CommandText = @"
                    SELECT  [Id]
                            , [FirstName]
                            , [LastName]
                            , [Ssn]
                            , [BirthDate]
                            , [Created]
                            , [LastModified]
                    FROM    [dbo].[AdoPatients] 
                    WHERE   [Ssn]=@Ssn
                ";

                    var sqlParameterSsn = cmd.CreateParameter();
                    sqlParameterSsn.ParameterName = @"@Ssn";
                    sqlParameterSsn.DbType = DbType.AnsiStringFixedLength;
                    sqlParameterSsn.Direction = ParameterDirection.Input;
                    sqlParameterSsn.Value = plainTextSsn;
                    sqlParameterSsn.Size = 11;
                    cmd.Parameters.Add(sqlParameterSsn);

                    using (SqlDataReader sqlDataReader = cmd.ExecuteReader())
                    {
                        if (!sqlDataReader.HasRows)
                        {
                            return null;
                        }
                        while (sqlDataReader.Read())
                        {
                            patient = new AdoPatient
                            {
                                Id = Convert.ToInt32(sqlDataReader["Id"]),
                                Ssn = sqlDataReader["Ssn"].ToString(),
                                FirstName = sqlDataReader["FirstName"].ToString(),
                                LastName = sqlDataReader["LastName"].ToString(),
                                BirthDate = (DateTime)sqlDataReader["BirthDate"],
                                Created = (DateTime)sqlDataReader["Created"],
                                LastModified = (DateTime)sqlDataReader["LastModified"],
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Read/Find operation error: {ex}");
            }
            return patient;
        }

        private string GetConnectionString()
        {
            var connStringBuilder = new SqlConnectionStringBuilder(Configuration["ConnectionStrings:AdoSqlConnectionString"]);
            connStringBuilder.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled;

            return connStringBuilder.ConnectionString;
        }
    }
}
