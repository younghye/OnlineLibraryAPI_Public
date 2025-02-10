
CREATE TABLE dbo.Book (
	ItemID int NOT NULL,
	ISBN nvarchar(30) NOT NULL,
	Publisher nvarchar(100) NOT NULL,
	CONSTRAINT PK_Book PRIMARY KEY CLUSTERED (ItemID)
);

CREATE TABLE dbo.DVD(
	ItemID int NOT NULL,
	Duration nvarchar(30) NULL,
	Director nvarchar(100) NOT NULL,
	CONSTRAINT PK_Dvd PRIMARY KEY CLUSTERED (ItemID)
);

CREATE TABLE dbo.Software(
	ItemID int NOT NULL,
	[Version] nvarchar(10) NOT NULL,
	CONSTRAINT PK_Software PRIMARY KEY CLUSTERED (ItemID)
);

CREATE TABLE dbo.Category(
	CategoryID int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(30) NOT NULL,
	CONSTRAINT PK_Category PRIMARY KEY CLUSTERED (CategoryID)
);

CREATE TABLE dbo.Genre(
	GenreID int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	CONSTRAINT PK_Genre PRIMARY KEY CLUSTERED (GenreID)
);

CREATE TABLE dbo.[Type](
	TypeID int IDENTITY(1,1) NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	CONSTRAINT PK_Type PRIMARY KEY CLUSTERED (TypeID)
);

CREATE TABLE dbo.Customer(
	CustomerID int NOT NULL,
	LibraryCardNumber bigint IDENTITY(1000000000,1) NOT NULL,
	DateOfBirth date NULL,
	Deleted date NULL,
	CONSTRAINT PK_Customer PRIMARY KEY CLUSTERED (LibraryCardNumber)
);

CREATE TABLE dbo.Employee(
	EmployeeID int NOT NULL,
	UserName nvarchar(50) NOT NULL,
	[Password] nvarchar(50) NOT NULL,
	[Role] nvarchar(15) NOT NULL,
	CONSTRAINT PK_Employee PRIMARY KEY CLUSTERED (EmployeeID)
);

CREATE TABLE dbo.Person(
	PersonID int IDENTITY(1,1) NOT NULL,
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	[Address] nvarchar(100) NULL,
	PhoneNumber nvarchar(15) NOT NULL,
	Email nvarchar(50) NOT NULL,
	CONSTRAINT PK_Person PRIMARY KEY CLUSTERED (PersonID)
);

CREATE TABLE dbo.ItemCopy(
	Barcode bigint IDENTITY(1000000000,1) NOT NULL,
	ItemID int NOT NULL,
	Price money NULL,
	[Status] nvarchar(30) NOT NULL,
	Deleted date NULL,
	CONSTRAINT PK_ItemCopy PRIMARY KEY CLUSTERED (Barcode)
);

CREATE TABLE dbo.Items(
	ItemID int IDENTITY(1,1) NOT NULL,
	TypeID int NOT NULL,
	CategoryID int NOT NULL,
	GenreID int NOT NULL,
	Title nvarchar(100) NOT NULL,
	DateOfPublication date NOT NULL,
	Producer nvarchar(100) NOT NULL,
	Deleted date NULL,
	CONSTRAINT PK_Items PRIMARY KEY CLUSTERED (ItemID)
);

CREATE TABLE dbo.Loan(
	LoanID int IDENTITY(1,1) NOT NULL,
	LibraryCardNumber bigint NOT NULL,
	DateOfLoan date NOT NULL,
	TotalAmount money NULL,
	Quantities int NULL,
	CONSTRAINT PK_Loan PRIMARY KEY CLUSTERED (LoanID)
);

CREATE TABLE dbo.LoanDetail(
	LoanID int NOT NULL,
	Barcode bigint NOT NULL,
	ReturnDueDate date NOT NULL,
	ReturnDate date NULL,
	Fine money NULL,
	Note nvarchar(100) NULL,
	CONSTRAINT PK_LoanDetail PRIMARY KEY CLUSTERED (LoanID, Barcode)
);

/* Add foreign key */ 
ALTER TABLE Book ADD CONSTRAINT FK_Book_Items FOREIGN KEY (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE
go
ALTER TABLE Customer ADD CONSTRAINT FK_Customer_Person FOREIGN KEY (CustomerID) REFERENCES Person (PersonID) ON DELETE CASCADE
go
ALTER TABLE DVD ADD CONSTRAINT FK_DVD_Items FOREIGN KEY (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE
go
ALTER TABLE Employee ADD CONSTRAINT FK_Employee_Person FOREIGN KEY (EmployeeID) REFERENCES Person (PersonID) ON DELETE CASCADE
go
ALTER TABLE ItemCopy ADD CONSTRAINT FK_ItemCopy_Items FOREIGN KEY (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE
go
ALTER TABLE Items ADD CONSTRAINT FK_Items_Type FOREIGN KEY (TypeID) REFERENCES Type (TypeID)
go
ALTER TABLE Items ADD CONSTRAINT FK_Items_Category FOREIGN KEY (CategoryID) REFERENCES Category (CategoryID)
go
ALTER TABLE Items ADD CONSTRAINT FK_Items_Genre FOREIGN KEY (GenreID) REFERENCES Genre (GenreID)
go
ALTER TABLE Loan ADD CONSTRAINT FK_Loan_Customer FOREIGN KEY (LibraryCardNumber) REFERENCES Customer (LibraryCardNumber)
go
ALTER TABLE LoanDetail ADD CONSTRAINT FK_LoanDetail_Loan FOREIGN KEY (LoanID) REFERENCES Loan (LoanID)
go
ALTER TABLE LoanDetail ADD CONSTRAINT FK_LoanDetail_ItemCopy FOREIGN KEY (Barcode) REFERENCES ItemCopy (Barcode)
go
ALTER TABLE Reservation ADD CONSTRAINT FK_Reservation_Customer FOREIGN KEY (LibraryCardNumber) REFERENCES Customer (LibraryCardNumber)
go
ALTER TABLE Reservation ADD CONSTRAINT FK_Reservation_ItemCopy FOREIGN KEY (Barcode) REFERENCES ItemCopy (Barcode)
go
ALTER TABLE Reservation ADD CONSTRAINT FK_Reservation_Loan FOREIGN KEY (LoanID) REFERENCES Loan (LoanID)
go
ALTER TABLE Software ADD CONSTRAINT FK_Software_Items FOREIGN KEY (ItemID) REFERENCES Items (ItemID) ON DELETE CASCADE
go


INSERT INTO dbo.Category ([Name]) VALUES('Education');
INSERT INTO dbo.Category ([Name]) VALUES('Family');

INSERT INTO dbo.Genre ([Name]) VALUES('Action');
INSERT INTO dbo.Genre ([Name]) VALUES('Comedy');
INSERT INTO dbo.Genre ([Name]) VALUES('Pomentic');

INSERT INTO dbo.[Type] ([Name]) VALUES('BOOK');
INSERT INTO dbo.[Type] ([Name]) VALUES('DVD');
INSERT INTO dbo.[Type] ([Name]) VALUES('SOFTWARE');



