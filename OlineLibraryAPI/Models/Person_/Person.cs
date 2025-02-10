namespace OlineLibraryAPI.Models.Person_
{
    public class Person
    {
        public int PersonID { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }

        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
    }
}
