USE Clinic;
GO

--DROP TABLE [dbo].[Patients]

CREATE TABLE [dbo].[Patients](
	[PatientId] [int] IDENTITY(1,1),
	[SSN] [char](11) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[MiddleName] [nvarchar](50) NULL,
	[StreetAddress] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[ZipCode] [char](5) NULL,
	[State] [char](2) NULL,
	[BirthDate] [date] NOT NULL
	PRIMARY KEY CLUSTERED ([PatientId] ASC) ON [PRIMARY] 
);
GO

INSERT INTO Patients (SSN, FirstName, LastName, BirthDate) VALUES 
	('123456789','Liam','Hando','1974-01-10'),
	('987654321','Dirk','D','1982-01-10'),
	('987654326','Scott','C','1980-01-10'),
	('789456123','Mitch','Sut','1990-01-10'),
	('321654987','Blake','Sz','1998-01-10');

SELECT	*
FROM	Patients;