using AlwaysEncrypted.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlwaysEncypted.Poc
{
    public class LinqStub : BaseStub
    {
        private readonly AlwaysEncryptedDbContext _context;

        public LinqStub(
            IConfiguration configuration, 
            ILogger<AdoStub> logger,
            AlwaysEncryptedDbContext context) 
            : base(configuration, logger) 
        {
            _context = context;
        }

        internal override async Task Run()
        {
            Logger.LogInformation("LinqStub");
            const string SSN = "123-12-1234";
            var birthDate = DateTime.Parse("1999-01-01", Culture);

            var ssn = SSN;
            var johnSmithExists = await _context.LinqPatients
                .Where(p => p.Ssn == ssn)
                .AnyAsync();
            Logger.LogInformation($"LinqStub\n\nSTRING COMPARISON\nDoes John exist: {johnSmithExists}.");

            if (!johnSmithExists)
            {
                await _context.LinqPatients.AddAsync(new LinqPatient
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Ssn = SSN,
                    BirthDate = birthDate,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                });

                await _context.SaveChangesAsync();
                Logger.LogInformation("LinqStub\n\nCREATE\nSuccessfully created John.");
            }

            Logger.LogInformation("LinqStub\n\nREAD\nRead all patients after create.");
            foreach (var patient in await _context.LinqPatients.ToListAsync())
            {
                Logger.LogInformation($"LinqStub\n\nId: {patient.Id}, Name: {patient.FirstName} {patient.LastName}, Ssn: {patient.Ssn}, BirthDate: {patient.BirthDate}.");
            }

            var linqPatientEntry = await _context.LinqPatients
                .Where(p => p.Ssn == ssn)
                .FirstOrDefaultAsync();

            if(linqPatientEntry == null)
            {
                Logger.LogInformation("LinqStub\n\nFind\nJohn could not be found.");
            }

            var updatedSsn = "987-98-9876";
            var updatedBirthDate = DateTime.Parse("2020-01-01", Culture);
            linqPatientEntry.FirstName = "Jane";
            linqPatientEntry.LastName = "Petes";
            linqPatientEntry.Ssn = updatedSsn;
            linqPatientEntry.BirthDate = updatedBirthDate;
            linqPatientEntry.LastModified = DateTime.UtcNow;

            _context.LinqPatients.Update(linqPatientEntry);
            await _context.SaveChangesAsync();
            Logger.LogInformation("LinqStub\n\nUPDATE\nSuccessfully updated John to Jane.");

            Logger.LogInformation("LinqStub\n\nREAD\nRead all patients after update.");
            foreach (var patient in await _context.LinqPatients.ToListAsync())
            {
                Logger.LogInformation($"LinqStub\n\nId: {patient.Id}, Name: {patient.FirstName} {patient.LastName}, Ssn: {patient.Ssn}, BirthDate: {patient.BirthDate}.");
            }

            var janePetes = await _context.LinqPatients
                .Where(p => p.Ssn == updatedSsn)
                .FirstOrDefaultAsync();

            if(janePetes != null)
            {
                _context.LinqPatients.Remove(janePetes);
                await _context.SaveChangesAsync();

                Logger.LogInformation("LinqStub\n\nDELETE\nSuccessfully deleted Jane.");
            }

            var patients = await _context.LinqPatients
                .ToListAsync();

            Logger.LogInformation($"LinqStub\n\nAfter deleting from LinqPatients table, there are: {patients.Count} patients.");
        }
    }
}
