using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiPlayerSnakeGame.Client
{
    public static class JSInteropFocusExtensions
    {
        public static async ValueTask FocusAsync(this IJSRuntime jsRuntime, ElementReference elementReference)
        {
            await jsRuntime.InvokeVoidAsync("BlazorFocusElement", elementReference);
        }
    }
}
