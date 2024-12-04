using SmartPulseEPIAS.Domain.Models;

namespace SmartPulseEPIAS.Application.Service
{
    public class GroupingService
    {
        public List<ResultTableModel> GroupResult(List<TransactionHistoryGipDataDto> model)
        {
          return   model.GroupBy(i => i.ContractName) // contractName'e göre gruplama
           .Select(group => new ResultTableModel
           {
               Tarih = group.Key, // ContractName (tarih bilgisi) gruplama anahtarı
               ToplamTutar = group.Sum(i => (i.Price * i.Quantity) / 10), // Toplam İşlem Tutarı
               ToplamMiktar = group.Sum(i => i.Quantity) / 10, // Toplam İşlem Miktarı
               AgirlikOrtFiyat = Math.Round(group.Sum(i => (i.Price * i.Quantity)) / group.Sum(i => i.Quantity), 2) // Ağırlıklı Ortalama Fiyat
           }).ToList();

        }
        public List<ResultTableModel> RenameResult(List<ResultTableModel> model)
        {
          foreach (var item in model)
            {
                item.Tarih = item.Tarih.Substring(6, 2) + "/" + item.Tarih.Substring(4, 2) + "/" + "20" + item.Tarih.Substring(2, 2) + "/" + item.Tarih.Substring(8, 2) + ":00";
            }

          return model;
        }


    }
}
