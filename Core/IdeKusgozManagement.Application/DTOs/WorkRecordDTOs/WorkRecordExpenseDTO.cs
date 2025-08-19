using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeKusgozManagement.Application.DTOs.WorkRecordDTOs
{
    public class WorkRecordExpenseDTO
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptImageUrl { get; set; }
    }
}