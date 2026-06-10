namespace Health.Shared.DTOs.IdentityDTOs.Requests
{
    public sealed class AddressDTO
    {
        public string City { get; init; } = default!;
        public string Country { get; init; } = default!;
        public string Street { get; init; } = default!;
    }
}