namespace OlineLibraryAPI.Models.Person_
{
    public enum EnumEmployeeType
    {
        STAFF,
        ADMIN,
        MANAGER
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public EnumEmployeeType Role { get; set; }
        public required Person Person { get; set; }
    }
}

