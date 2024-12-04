using System.Text.Json.Serialization;

namespace SmartPulseEPIAS.Domain.Models
{
    public class TransactionHistoryModel
    {
        [JsonPropertyName("items")]
        public List<TransactionHistoryGipDataDto> Items { get; set; }
        [JsonPropertyName("page")]
        public Page Pages { get; set; }
    }

    public class TransactionHistoryGipDataDto
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("hour")]
        public string Hour { get; set; }
        [JsonPropertyName("contractName")]
        public string ContractName { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class Page
    {
        [JsonPropertyName("number")]
        public long Number { get; set; }
        [JsonPropertyName("size")]
        public long Size { get; set; }
        [JsonPropertyName("total")]
        public long Total { get; set; }
        [JsonPropertyName("sort")]
        public Sort Sort { get; set; }
    }

    public class Sort
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }
        [JsonPropertyName("direction")]
        public string Direction { get; set; }
    }

   
}