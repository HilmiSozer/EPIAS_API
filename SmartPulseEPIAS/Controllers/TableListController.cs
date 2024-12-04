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
    public class TableListController : ControllerBase
    {
        private readonly TableCreatorService _tableCreator;
        private readonly GroupingService _service;
        private readonly TGTservice _transactionService;
        private readonly HttpClient _client;

        public TableListController(TGTservice tgtservice, HttpClient client, GroupingService groupingService, TableCreatorService tableCreator)
        {
            _transactionService = tgtservice;
            _client = client;
            _service = groupingService;
            _tableCreator = tableCreator;
        }


        [HttpGet("ListFormat-string")]
        public async Task<IActionResult> LastTable()
        {
            try
            {
                // Token'ı güvenli bir şekilde al
                var token = await _transactionService.CacheToken();

                // İstek URL'si
                var requestUrl = "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history";

                // HTTP isteğini oluştur
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

                // Güvenli token ekle
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Add("TGT", token);
                }
                else
                {
                    return Unauthorized("Token alınamadı. Yetkilendirme başarısız.");
                }

                // JSON içeriğini güvenli bir şekilde oluştur 00:00:00+03:00
                var requestBody = new
                {
                    endDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Bugünün tarihi
                 startDate = DateTime.Today.ToString("yyyy-MM-ddT00:00:00+03:00"), // Dünün tarihi
                page = new { }

                };

                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                // HTTP yanıtını al
                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Veri çekilirken hata oluştu.");
                }

                // Yanıt içeriğini işle
                var responseBody = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<TransactionHistoryModel>(responseBody);

                if (output == null || output.Items == null || !output.Items.Any())
                {
                    return NotFound("İstenen veri bulunamadı.");
                }

                // Verileri gruplandır ve yeniden adlandır
                var groupedResults = _service.GroupResult(output.Items);
                var renamedGroup = _service.RenameResult(groupedResults);

                // Tabloyu oluştur
                var table = new StringBuilder();
                var theTable = _tableCreator.TableCreator(table, groupedResults);

                // Güvenli dönüş
                return Ok(theTable);
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
