namespace HospitalManagement_BvDKAnViet.Core.IServices
{
    public class CkdPythonRequest
    {
        public double age { get; set; }
        public double bp { get; set; }
        public double sg { get; set; }
        public double al { get; set; }
        public double su { get; set; }
        public int rbc { get; set; }     // 0=normal, 1=abnormal
        public int pc { get; set; }
        public int pcc { get; set; }     // 0=notpresent, 1=present
        public int ba { get; set; }
        public double bgr { get; set; }
        public double bu { get; set; }
        public double sc { get; set; }
        public double sod { get; set; }
        public double pot { get; set; }
        public double hemo { get; set; }
        public double pcv { get; set; }
        public double wc { get; set; }
        public double rc { get; set; }
        public int htn { get; set; }     // 0=no, 1=yes
        public int dm { get; set; }
        public int cad { get; set; }
        public int appet { get; set; }   // 0=poor, 1=good
        public int pe { get; set; }
        public int ane { get; set; }
    }

    public class CkdPythonResponse
    {
        public int prediction { get; set; }
        public string? label { get; set; }
        public double probability_ckd { get; set; }
        public double probability_not_ckd { get; set; }
        public string? risk_level { get; set; }
    }

    public interface ICkdPythonService
    {
        Task<CkdPythonResponse?> PredictAsync(CkdPythonRequest request);
    }
}