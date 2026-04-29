// File: BackgroundServices/AppointmentCancelService.cs
using HospitalManagement_BvDKAnViet.Core.Enums;
using HospitalManagement_BvDKAnViet.Core.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HospitalManagement_BvDKAnViet.Api.BackgroundServices
{
    public class AppointmentCancelService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentCancelService> _logger;

        // Chạy mỗi 5 phút
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public AppointmentCancelService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentCancelService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AppointmentCancelService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CancelExpiredAppointmentsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cancelling expired appointments");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task CancelExpiredAppointmentsAsync()
        {
            // Tạo scope vì IAppointmentRepository là Scoped service
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider
                .GetRequiredService<IAppointmentRepository>();

            var now = DateTime.Now;
            var expired = await repo.GetPendingExpiredAsync(now);
            var ids = expired.Select(a => a.AppointmentId).ToList();

            if (!ids.Any())
            {
                _logger.LogInformation("No expired appointments found at {Time}", now);
                return;
            }

            await repo.BulkUpdateStatusAsync(
                ids,
                (int)AppointmentStatus.CANCELLED,
                AppointmentStatus.CANCELLED.ToString()
            );

            _logger.LogInformation(
                "Auto-cancelled {Count} expired appointments at {Time}",
                ids.Count, now);
        }

    }
}