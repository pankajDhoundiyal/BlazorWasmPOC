using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using BlazorApp1.Client;
using BlazorApp1.Client.Shared;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.DropDowns;
using BlazorApp1.Client.Services;

namespace BlazorApp1.Client.Components
{
    public partial class ToastBase : ComponentBase, IDisposable
    {
        [Inject]
        ToastService ToastService
        {
            get;
            set;
        }
        protected string Heading
        {
            get;
            set;
        }
        protected string Message
        {
            get;
            set;
        }
        protected bool IsVisible
        {
            get;
            set;
        }
        protected string BackgroundCssClass
        {
            get;
            set;
        }
        protected string IconCssClass
        {
            get;
            set;
        }
        protected override void OnInitialized()
        {
            ToastService.OnShow += ShowToast;
            ToastService.OnHide += HideToast;
        }
        private void ShowToast(string message, ToastLevel level)
        {
            BuildToastSettings(level, message);
            IsVisible = true;
            StateHasChanged();
        }
        private void HideToast()
        {
            IsVisible = false;
            StateHasChanged();
        }
        private void BuildToastSettings(ToastLevel level, string message)
        {
            switch (level)
            {
                case ToastLevel.Info:
                    BackgroundCssClass = "bg-info";
                    IconCssClass = "info";
                    Heading = "Info";
                    break;
                case ToastLevel.Success:
                    BackgroundCssClass = "bg-success";
                    IconCssClass = "check";
                    Heading = "Success";
                    break;
                case ToastLevel.Warning:
                    BackgroundCssClass = "bg-warning";
                    IconCssClass = "exclamation";
                    Heading = "Warning";
                    break;
                case ToastLevel.Error:
                    BackgroundCssClass = "bg-danger";
                    IconCssClass = "times";
                    Heading = "Error";
                    break;
            }
            Message = message;
        }
        public void Dispose()
        {
            ToastService.OnShow -= ShowToast;
        }
    }
}