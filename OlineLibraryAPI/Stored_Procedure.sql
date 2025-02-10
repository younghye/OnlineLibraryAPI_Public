USE [OlineLibrary]
GO

/****** Object:  StoredProcedure [dbo].[PROC_SearchCustomer]    Script Date: 9/04/2024 10:31:32 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PROC_DeleteItem]
(
	@itemID INT	
)
AS

IF EXISTS (
	SELECT(1) FROM Items as i INNER JOIN ItemCopy as ic ON i.ItemID = ic.ItemID 
	WHERE ic.ItemID = @itemID AND ic.Status = 'CHECK_OUT')
	
	BEGIN
		RETURN -1;
	END
ELSE
	BEGIN
		UPDATE Items SET deleted = CURRENT_TIMESTAMP WHERE ItemID = @itemID
		UPDATE ItemCopy SET deleted = CURRENT_TIMESTAMP WHERE ItemID = @itemID
		RETURN @@ROWCOUNT
	END
GO

CREATE PROCEDURE [dbo].[PROC_InsertCustomer]
(
	@firstName VARCHAR(50),
	@lastName VARCHAR(50),
	@address VARCHAR(100),
	@phoneNumber VARCHAR(15),
	@email VARCHAR(50),
	@dateOfBirth DATE
)
AS

IF NOT EXISTS (
	SELECT * FROM Person P Left Join Customer C on C.CustomerID = P.PersonID 
	WHERE P.Email = @email and C.CustomerID IS NOT NULL)
	BEGIN
		DECLARE @personID INT 
		INSERT INTO Person(FirstName,LastName,Address,PhoneNumber,Email) 
		VALUES(@firstName,@lastName,@address,@phoneNumber,@email);
		SET @personID = SCOPE_IDENTITY() 
		
		DECLARE @libraryCardNumber BIGINT
		INSERT INTO Customer(CustomerID, DateOfBirth) VALUES(@personID, @dateOfBirth);
		SET @libraryCardNumber = SCOPE_IDENTITY() 
		SELECT * FROM Customer WHERE CustomerID = @personID
	END
ELSE
	BEGIN
		SELECT -1
	END
GO

CREATE PROCEDURE [dbo].[PROC_InsertEmployee]
(
	@firstName VARCHAR(50),
	@lastName VARCHAR(50),
	@address VARCHAR(100),
	@phoneNumber VARCHAR(15),
	@email VARCHAR(50),
	@userName VARCHAR(50),
	@password VARCHAR(50),
	@role VARCHAR(15)
)
AS

IF NOT EXISTS (
	SELECT * FROM Person P Left Join Employee E on E.EmployeeID = P.PersonID 
	WHERE P.Email = @email and E.EmployeeID IS NOT NULL)
	
	IF NOT EXISTS(SELECT * FROM Employee WHERE USERNAME = @userName)
		BEGIN
			DECLARE @personID INT 
			INSERT INTO Person(FirstName,LastName,Address,PhoneNumber,Email) 
			VALUES(@firstName,@lastName,@address,@phoneNumber,@email);
			SET @personID = SCOPE_IDENTITY()

			INSERT INTO Employee(EmployeeID, UserName, Password, Role) 
			VALUES(@personID, @userName, @password, @role);
			return @personID
		END
	ELSE
		BEGIN
			RETURN -2
		END
ELSE
	BEGIN
		return -1
	END
GO

Create PROCEDURE [PROC_InsertItem]
    @typeID INT,
	@categoryID INT,
	@genreID INT,
	@title NVARCHAR(100),
	@dateOfPublication DATE,
	@producer NVARCHAR(100),
    @isbn	NVARCHAR (30),
    @publisher NVARCHAR (100),
	@duration NVARCHAR (30),
    @director NVARCHAR (100),
	@version NVARCHAR (10)
	
AS
EXEC [dbo].[PROC_SearchItem] null, @typeID, @title, @producer, null, null
If @@RowCount = 0
	BEGIN
		SET NOCOUNT ON;
		DECLARE @itemID INT 

		INSERT INTO Items(TypeID,CategoryID,GenreID,Title,DateOfPublication,Producer) 
		VALUES(@typeID,@categoryID,@genreID,@title,@dateOfPublication,@producer);
		SET @itemID = SCOPE_IDENTITY()

		IF @typeID = 1
			BEGIN
    			INSERT INTO Book(ItemID,ISBN,Publisher) VALUES(@itemID,@isbn,@publisher);
			END
		ELSE IF @typeID = 2
			BEGIN
    			INSERT INTO DVD(ItemID,Duration,Director) VALUES(@itemID,@duration,@director);
			END
		ELSE IF @typeID = 3
			BEGIN
    			INSERT INTO Software(ItemID,[Version]) VALUES(@itemID,@version);
			END

		RETURN @itemID
	END
ELSE
	BEGIN
		return -1
	END
go

CREATE PROCEDURE [dbo].[PROC_UpdateItem]
	@itemID INT,
    @typeID INT,
	@categoryID INT,
	@genreID INT,
	@title NVARCHAR(100),
	@dateOfPublication DATE,
	@producer NVARCHAR(100),
    @isbn	NVARCHAR (30),
    @publisher NVARCHAR (100),
	@duration NVARCHAR (30),
    @director NVARCHAR (100),
	@version NVARCHAR (10)
	
AS
BEGIN

	UPDATE Items SET TypeID = @typeID, CategoryID = @categoryID, GenreID = @genreID, Title = @title, 
	DateOfPublication = @dateOfPublication, Producer = @producer 
	WHERE ItemID=@itemID;

	IF @typeID = 1
		BEGIN
    		UPDATE Book SET ISBN = @isbn, Publisher = @publisher WHERE ItemID=@itemID;
		END
	ELSE IF @typeID = 2
		BEGIN
			UPDATE DVD SET Duration = @duration, Director = @director WHERE ItemID=@itemID;
		END
	ELSE IF @typeID = 3
		BEGIN
			UPDATE Software SET [Version] = @version WHERE ItemID=@itemID;
		END
		RETURN @@ROWCOUNT
END
GO

CREATE PROCEDURE [dbo].[PROC_InsertLoanDetails] 
	@loanID int,
	@barcode int,
	@returnDueDate date,
	@status varchar(30)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO LoanDetail (LoanID, Barcode, ReturnDueDate) VALUES(@loanID,@barcode,@returnDueDate);
	
	UPDATE ItemCopy SET Status = @Status WHERE Barcode =  @barcode;
END
go

CREATE PROCEDURE [PROC_SearchCustomer]
(
	@customerID INT,
	@firstName VARCHAR(50),
	@lastName VARCHAR(50),
	@phoneNumber VARCHAR(15)
)
AS
BEGIN
	SELECT * From Customer INNER JOIN Person ON CustomerID = PersonID 
	WHERE (CustomerID = @customerID OR  @customerID IS NULL OR @customerID = '') 
	AND (FirstName = @firstName OR @firstName IS NULL OR @firstName = '') 
	AND (LastName = @lastName OR @lastName IS NULL OR @lastName = '') 
	AND (PhoneNumber = @phoneNumber OR @phoneNumber IS NULL OR @phoneNumber = '')
	ORDER BY CustomerID;
END
go

CREATE PROCEDURE [PROC_SearchEmployee]
(
	@employeeID INT,
	@firstName VARCHAR(50),
	@lastName VARCHAR(50),
	@role VARCHAR(15),
	@phoneNumber VARCHAR(15)
)
AS
BEGIN
	SELECT * From Employee INNER JOIN Person ON EmployeeID = PersonID 
	WHERE (EmployeeID = @employeeID OR  @employeeID IS NULL OR @employeeID = '') 
	AND (FirstName = @firstName OR @firstName IS NULL OR @firstName = '') 
	AND (LastName = @lastName OR @lastName IS NULL OR @lastName = '') 
	AND ([Role] = @role OR @role IS NULL OR @role = '') 
	AND (PhoneNumber = @phoneNumber OR @phoneNumber IS NULL OR @phoneNumber = '')
	ORDER BY EmployeeID;
END
go

CREATE PROCEDURE [PROC_SearchItem]
(
	@barcode BIGINT,
	@typeID INT,
	@title VARCHAR(100),
	@producer VARCHAR(100),
	@publishDateFrom DATETIME,
	@publishDateTo DATETIME
)
AS
BEGIN
	DECLARE @from DATETIME = NULL
	DECLARE @to DATETIME = NULL

	IF (@publishDateFrom IS NOT NULL AND @publishDateTo IS NULL)
	BEGIN
		SET @from = @publishDateFrom
		SET @to = CONVERT(DATETIME, 2958463.99999999)
	END
	ELSE IF (@publishDateFrom IS NULL AND @publishDateTo IS NOT NULL)
	BEGIN
		SET @from = CONVERT(DATETIME, -53690.0)
		SET @to = @publishDateTo
	END
	ELSE IF (@publishDateFrom IS NOT NULL AND @publishDateTo IS NOT NULL)
	BEGIN
		SET @from = @publishDateFrom
		SET @to = @publishDateTo
	END

	SELECT i.*, ic.ItemID, ic.Barcode, ic.price, ic.Status, ic.Deleted, b.*, d.*, s.*, t.*, g.*, c.* 
	FROM Items AS i 
	LEFT JOIN ItemCopy AS ic ON i.ItemID = ic.ItemID AND ic.Deleted is null
    LEFT JOIN Book AS b ON i.ItemID = b.ItemID 
    LEFT JOIN DVD AS d ON i.ItemID = d.ItemID 
    LEFT JOIN Software AS s ON i.ItemID = s.ItemID 
	INNER JOIN Category AS c ON i.CategoryID = c.CategoryID
	INNER JOIN Genre AS g ON i.GenreID = g.GenreID
	INNER JOIN Type AS t ON i.TypeID = t.TypeID
	WHERE (i.TypeID = @typeID OR @typeID IS NULL)
	AND (i.Title = @title OR @title IS NULL OR @title = '')
	AND (i.Producer = @producer OR @producer IS NULL OR @producer = '')
	AND (ic.Barcode = @barcode OR @barcode IS NULL OR @barcode = '')
	AND (i.DateOfPublication BETWEEN @from AND @to OR (@from IS NULL AND @to IS NULL))
	AND i.Deleted is null
	ORDER BY i.ItemID;
END
GO

CREATE PROCEDURE [dbo].[PROC_DeleteCustomer]
(
	@customerID INT	
)
AS

IF EXISTS (
	SELECT(1) FROM Customer AS c INNER JOIN Loan AS l ON c.LibraryCardNumber = l.LibraryCardNumber
	INNER JOIN LoanDetail AS ld ON l.LoanID = ld.LoanID
	WHERE c.CustomerID = @customerID and ld.ReturnDate is null)

	BEGIN
		RETURN -1;
	END
ELSE
	BEGIN
		UPDATE Customer SET deleted = CURRENT_TIMESTAMP WHERE CustomerID = @customerID
		RETURN @@ROWCOUNT
	END

GO

