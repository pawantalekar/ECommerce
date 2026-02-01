namespace Ecom.Application.AuthService.Application.DTO
{
    public record PagedOrdersDto(List<AdminOrderListItemDto> Orders, int Total, int PageNumber, int PageSize)
    {
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
