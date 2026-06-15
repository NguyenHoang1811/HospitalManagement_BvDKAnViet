﻿using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HospitalManagement_BvDKAnViet.Data.Services
{
    public class CkdPythonService : ICkdPythonService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CkdPythonService> _logger;

        public CkdPythonService(HttpClient httpClient, ILogger<CkdPythonService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CkdPythonResponse?> PredictAsync(CkdPythonRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/predict", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CkdPythonResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi CKD Python API");
                return null;
            }
        }
    }
}