using Newtonsoft.Json;

namespace Entities.DTOs.Payment
{
    public class PaymentVerificationResponseDto
    {
        public int Status { get; set; }

        public long RefId { get; set; }

        public string Message { get; set; } 
    }
}
