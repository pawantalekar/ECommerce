using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.GlobalExceptionHandlerService.Application.DTO
{
    public class ErrorDetails
    {
        public string Message { get; set; } = "An unexpected error occurred.";
        public string? Detail { get; set; }
        public string? ErrorCode { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
