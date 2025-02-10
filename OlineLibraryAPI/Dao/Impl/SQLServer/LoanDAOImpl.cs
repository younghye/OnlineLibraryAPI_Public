using Dapper;
using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Item_;
using OlineLibraryAPI.Models.Loan_;
namespace OlineLibraryAPI.Dao.Impl.SQLServer;

public class LoanDAOImpl : SQLServerBaseDAO, ILoanDAO
{
    private readonly string searchStr = "SELECT l.*, ld.*, i.*, b.*, d.*, s.*, ic.*, c.*, g.*, t.* " +
                "FROM Loan AS l " +
                "INNER JOIN Customer AS cu ON cu.LibraryCardNumber = l.LibraryCardNumber " +
                "INNER JOIN LoanDetail AS ld ON l.LoanID = ld.LoanID " +
                "INNER JOIN ItemCopy AS ic ON ld.Barcode = ic.Barcode " +
                "INNER JOIN Items AS i ON ic.ItemID = i.ItemID " +
                "LEFT JOIN Book AS b ON i.ItemID = b.ItemID " +
                "LEFT JOIN DVD AS d ON i.ItemID = d.ItemID " +
                "LEFT JOIN Software AS s ON i.ItemID = s.ItemID " +
                "INNER JOIN Category AS c ON i.CategoryID = c.CategoryID " +
                "INNER JOIN Genre AS g ON i.GenreID = g.GenreID " +
                "INNER JOIN Type AS t ON i.TypeID = t.TypeID ";

    public async Task<LoanDetail?> FindActiveLoanByBarcode(long barcode)
    {
        using var con = GetConnection();
        string query = searchStr + "WHERE ld.Barcode = @barcode AND ld.returnDate IS NULL AND ic.Status = '" + EnumItemStatus.CHECK_OUT + "'";
        var result = await con.QueryAsync<Loan, LoanDetail, Item, Book, DVD, Software, ItemCopy, LoanDetail>
            (query, (loan, loanDetail, item, book, dvd, software, itemCopy) =>
            {
                return SetLoanDetail(loan, loanDetail, item, book, dvd, software, itemCopy);
            },
            param: new { barcode },
            splitOn: "LoanID, LoanID, ItemID, ItemID, ItemID, ItemID,Barcode");

        return result.SingleOrDefault();
    }

    public async Task<List<LoanDetail>> FindByBarcode(long barcode)
    {
        using var con = GetConnection();
        string query = searchStr + "WHERE ld.Barcode = @barcode ORDER BY l.LoanID DESC";
        var result = await con.QueryAsync<Loan, LoanDetail, Item, Book, DVD, Software, ItemCopy, LoanDetail>
            (query, (loan, loanDetail, item, book, dvd, software, itemCopy) =>
            {
                return SetLoanDetail(loan, loanDetail, item, book, dvd, software, itemCopy);
            },
            param: new { barcode },
            splitOn: "LoanID, LoanID, ItemID, ItemID, ItemID, ItemID,Barcode");

        return result.AsList<LoanDetail>();
    }

    public async Task<List<LoanDetail>> FindByLibraryCardNumber(long libraryCardNumber)
    {
        using var con = GetConnection();
        string query = searchStr + "WHERE l.libraryCardNumber = @libraryCardNumber ORDER BY l.LoanID DESC";
        var result = await con.QueryAsync<Loan, LoanDetail, Item, Book, DVD, Software, ItemCopy, LoanDetail>
            (query, (loan, loanDetail, item, book, dvd, software, itemCopy) =>
            {
                return SetLoanDetail(loan, loanDetail, item, book, dvd, software, itemCopy);
            },
            param: new { libraryCardNumber },
            splitOn: "LoanID, LoanID, ItemID, ItemID, ItemID, ItemID,Barcode");

        return result.AsList<LoanDetail>();
    }

    public async Task<int> Update(List<LoanDetail> list)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        int affectedRows = 0;
        foreach (LoanDetail ld in list)
        {
            string ldSql = "UPDATE LoanDetail SET ReturnDate = @returnDate, Fine = @fine, Note =@note WHERE Barcode = @barcode AND LoanID=@loanID";
            string sql = "UPDATE ItemCopy SET Status = @status WHERE Barcode = @barcode;";

            DynamicParameters parameters = new();
            parameters.Add("@returnDate", ld.ReturnDate);
            parameters.Add("@loanID", ld.LoanID);
            parameters.Add("@barcode", ld.Barcode);
            parameters.Add("@fine", ld.Fine);
            parameters.Add("@note", ld.Note);
            parameters.Add("@status", ld.ItemCopy?.Status.ToString());

            int ldResult = await con.ExecuteAsync(ldSql, parameters, tran);
            int icResult = await con.ExecuteAsync(sql, parameters, tran);
            affectedRows = ldResult + icResult < 2 ? affectedRows : affectedRows + 1;
        }
        tran.Commit();
        return affectedRows;
    }

    public async Task<int> Insert(Loan r)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string rSql = "INSERT INTO Loan(LibraryCardNumber,DateOfLoan,TotalAmount,Quantities) " +
            "VALUES(@libraryCardNumber,@loanDate,@totalAmount,@quantities); SELECT SCOPE_IDENTITY();";

        DynamicParameters parameters = new();
        parameters.Add("@libraryCardNumber", r.LibraryCardNumber);
        parameters.Add("@loanDate", r.DateOfLoan);
        parameters.Add("@totalAmount", r.TotalAmount);
        parameters.Add("@quantities", r.Quantities);
        int loanID = await con.ExecuteScalarAsync<int>(rSql, parameters, tran);

        foreach (LoanDetail d in r.LoanDetails)
        {
            string rdSql = "INSERT INTO LoanDetail(LoanID, Barcode, ReturnDueDate) VALUES(@loanID, @barcode, @returnDueDate);";
            string icSql = "UPDATE ItemCopy SET Status = @status WHERE Barcode = @barcode;";

            parameters = new DynamicParameters();
            parameters.Add("@loanID", loanID);
            parameters.Add("@barcode", d.Barcode);
            parameters.Add("@returnDueDate", d.ReturnDueDate);
            parameters.Add("@status", d.ItemCopy?.Status.ToString());

            await con.ExecuteAsync(rdSql, parameters, tran);
            await con.ExecuteAsync(icSql, parameters, tran);
        }
        tran.Commit();
        return loanID;
    }

    private static LoanDetail SetLoanDetail(Loan loan, LoanDetail loanDetail, Item item, Book book, DVD dvd, Software software, ItemCopy itemCopy)
    {
        itemCopy.Item = item;
        itemCopy.Item.Book = book;
        itemCopy.Item.DVD = dvd;
        itemCopy.Item.Software = software;
        loanDetail.ItemCopy = itemCopy;
        loanDetail.Loan = loan;
        return loanDetail;
    }

}