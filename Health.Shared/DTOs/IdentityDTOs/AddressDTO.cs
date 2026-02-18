namespace Health.Shared.DTOs.IdentityDTOs
{
    public class AddressDTO
    {
        public string City { get; init; } = default!;
        public string Country { get; init; } = default!;
        public string Street { get; init; } = default!;
    }
}