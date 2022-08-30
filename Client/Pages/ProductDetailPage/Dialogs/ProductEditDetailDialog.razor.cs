﻿using Client.Services;
using Client.Utils;
using Domain.InterfacesWorker;
using Domain.Models.MainDomain;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages.ProductDetailPage.Dialogs
{
    public partial class ProductEditDetailDialog : CustomComponentBase
    {
        [Inject] private IProductWorker productWorker { get; set; } = default!;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Product ProductDetail { get; set; } = default!;
        [Parameter] public bool? InsertMode { get; set; } = new();

        MudForm form = new();
        bool success;
        string[] errors = Array.Empty<string>();

        private async Task OnValidSubmit()
        {
            StateHasChanged();
            await form.Validate();

            if (form.IsValid)
            {
                ProductDetail.IdWorkshop = CurrentSession.Workshop.Id;
                if (InsertMode.HasValue && InsertMode.Value)
                    await productWorker.ProductRepository.Add(ProductDetail);
                else
                    productWorker.ProductRepository.Update(ProductDetail);

                StateHasChanged();
                productWorker.Completed();

                MudDialog.Close(DialogResult.Ok<int>(ProductDetail.Id));
            }
        }

        void Cancel()
        {
            productWorker.Rollback();
            MudDialog.Cancel();
        }
    }
}