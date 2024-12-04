using SmartPulseEPIAS.Domain.Models;
using System.Text;


namespace SmartPulseEPIAS.Application.Service
{
    public class TableCreatorService
    {
        public string TableCreator(StringBuilder htmlTable,List<ResultTableModel> result)
        {


            result  = result.OrderBy(r => r.Tarih).ToList();
            htmlTable.Append($"{new string('-', 115)}\n");
            htmlTable.Append($"{"Tarih",-30}");
            htmlTable.Append($"{"Toplam İşlem Tutarı",-30}");
            htmlTable.Append($"{"Toplam İşlem Miktari",-30}");
            htmlTable.Append($"{"Ağırlıklı Ortalama Fiyat",-30}\n");
            htmlTable.Append($"{new string('-', 115)}\n");

            foreach (var item in result)
            {

                htmlTable.Append($"{item.Tarih}");
                htmlTable.Append($"{item.ToplamTutar,30:N2}");
                htmlTable.Append($"{item.ToplamMiktar,30:N2}");
                htmlTable.Append($"{item.AgirlikOrtFiyat,37:N2}\n");

            }
             

            return htmlTable.ToString();


        }
       

}
}
