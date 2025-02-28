### Visit this repository first
https://github.com/younghye/OnlineLibraryClient_Public

## Data Access Object(DAO) Design Pattern
![DAO Pattern](https://github.com/younghye/OnlineLibraryAPI_Public/blob/9c8f64b1e82e4529fc73edaf8db348c8cb3f06df/DAO%20Pattern.jpg)

## Entity Relationship Diagram
![ERD](https://github.com/younghye/OnlineLibraryAPI_Public/blob/9c8f64b1e82e4529fc73edaf8db348c8cb3f06df/ERD.jpg)

## Setup
1. Setting up "Visual Studio" and clone a "OnlineLibraryAPI_Public" repository.<br />
https://github.com/younghye/OnlineLibraryAPI_Public
2. Update variables of JWT and DB connection strings in the "appsettings.json" file.
### DB
1. Setting up "Sql Server Management studio". 
2. Execute "SQLQuery" and "Stored_Procedure" files in the "OlineLibraryAPI_Public" repository.

## Demo
https://blue-cliff-0bdeaa900.5.azurestaticapps.net/access<br />
> [!NOTE]
> I use serverless Azure SQL databases is automatically pauses databases during inactive periods and when any activity occurs, the database will automatically resume from the paused state.
The first connection to the database always fails, as the auto-resume takes a number of minutes. Please try again after 1-2 minutes.


### Login 
UserName: Test<br /> 
Password: Test@1234 
