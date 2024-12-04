using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartPulseEPIAS.Application.Service;
using SmartPulseEPIAS.Domain.Models;
using SmartPulseEPIAS.Infrastrucator.Authorization;
using System.Text;


namespace SmartPulseEPIAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly GroupingService _service;
        private readonly TGTservice _transactionService;
        private readonly HttpClient _client;
        public TableController(TGTservice tgtservice, HttpClient client,GroupingService groupingService)
        {
            _transactionService = tgtservice;
            _client = client;
            _service = groupingService;
        }

        [HttpGet("JsonFormat")]
        public async Task<IActionResult> Table()
        {
            try
            {
                // Token alma
                var token = await _transactionService.CacheToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized("Token alınamadı. Yetkilendirme başarısız.");
                }

                // İstek URL'si ve yapılandırması
                var requestUrl = "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

                request.Headers.Add("TGT", token);

                // JSON içeriği
                var requestBody = new
                {
                    endDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Bugünün tarihi 
                    startDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Bugünün tarihi
                    page = new { }
                };

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // HTTP isteği gönderme
                var response = await _client.SendAsync(request); ;
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Veri çekilirken hata oluştu.");
                }

                // Yanıt işleme
                string responseBody = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<TransactionHistoryModel>(responseBody);
                if (output?.Items == null || !output.Items.Any())
                {
                    return NotFound("İstenen veri bulunamadı.");
                }

                // Gruplama ve yeniden adlandırma işlemi
                var groupedResults = _service.GroupResult(output.Items);
                var renamedGroup = _service.RenameResult(groupedResults);

                // Sonuçları döndürme
                return Ok(renamedGroup);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP isteği sırasında bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }

      

    }
    }

