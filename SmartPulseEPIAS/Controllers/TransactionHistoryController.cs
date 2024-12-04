using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartPulseEPIAS.Domain.Models;
using SmartPulseEPIAS.Infrastrucator.Authorization;
using System.Text;

namespace SmartPulseEPIAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly TGTservice _transactionService;
        private readonly HttpClient _client;


        public TransactionHistoryController(TGTservice tgtservice, HttpClient client)
        {
            _transactionService = tgtservice;
            _client = client;
        }

        [HttpGet("get-transaction-history")]
        public async Task<ActionResult<TransactionHistoryGipDataDto>> PostEPIAS()
        {
            try
            {
                // Token alma ve doğrulama
                var token = await _transactionService.CacheToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized("Token alınamadı. Yetkilendirme başarısız.");
                }

                // HTTP isteğini oluştur
                var requestUrl = "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history";
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                request.Headers.Add("TGT", token);

                // JSON içeriği
                var requestBody = new
                {   endDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Bugünün tarihi
                    startDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Bugünün tarihi
                    page = new { }
                };

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                // HTTP isteğini gönder
                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Veri çekilirken hata oluştu. Durum kodu: {response.StatusCode}");
                }

                // Yanıtı işle
                string responseBody = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<TransactionHistoryModel>(responseBody);

                if (output?.Items == null || !output.Items.Any())
                {
                    return NotFound("İstenen veri bulunamadı.");
                }

                // Sonuçları döndür
                return   Ok(output.Items);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"HTTP isteği sırasında bir hata oluştu: {ex.Message}");
            }
            catch (JsonSerializationException ex)
            {
                return BadRequest($"Yanıt işlenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Beklenmeyen bir hata oluştu: {ex.Message}");
            }
        }





    }




}
