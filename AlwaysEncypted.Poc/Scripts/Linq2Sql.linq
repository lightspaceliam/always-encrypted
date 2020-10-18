<Query Kind="Program">
  <Connection>
    <ID>d8875de8-b7a0-4e38-b673-4348bd99956c</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>localhost</Server>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Client</Database>
  </Connection>
</Query>

void Main()
{
	var patients = Patients
		.Select(p => p.FirstName)
		.ToList();
	
	patients.Dump();
	Patients.Dump();
}

// You can define other methods, fields, classes and namespaces here
