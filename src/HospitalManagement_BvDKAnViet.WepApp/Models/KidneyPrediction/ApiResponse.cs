namespace HospitalManagement_BvDKAnViet.WepApp.Models.KidneyPrediction
{
    public class ApiResponse<T>
    {
        public int responseCode { get; set; }
        public string responseMessage { get; set; }
        public T data { get; set; }
    }
}
