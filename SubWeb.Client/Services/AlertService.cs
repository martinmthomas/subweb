using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SubWeb.Client.Services
{
    public class AlertService : IAlertService
    {
        IJSRuntime _jsRuntime;
        public AlertService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task ErrorAsync(string message)
        {
            await _jsRuntime.InvokeAsync<object>("toastr.error", message);
        }

        public async Task InfoAsync(string message)
        {
            await _jsRuntime.InvokeAsync<object>("toastr.info", message);
        }

        public async Task SuccessAsync(string message)
        {
            await _jsRuntime.InvokeAsync<object>("toastr.success", message);
        }

        public async Task WarnAsync(string message)
        {
            await _jsRuntime.InvokeAsync<object>("toastr.warning", message);
        }
    }
}
