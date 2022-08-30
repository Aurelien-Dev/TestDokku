﻿using Client.Pages.ProductDetailPage.Dialogs;
using Client.Utils;
using Domain.InterfacesWorker;
using Domain.Models.MainDomain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;
using Utils.Exception;

namespace Client.Pages.ProductDetailPage
{
    [Authorize]
    public partial class ProductDetailPage : CustomComponentBase
    {
        [Parameter] public int? Id { get; set; } = default!;
        [Inject] private IProductWorker worker { get; set; } = default!;

        [NotNull] public Product ProductDetail { get; set; } = new();
        [NotNull] public Material MaterialDetail { get; set; } = new();
        private ICollection<Material> Materials { get; set; } = default!;


        public ImageInstruction ImageInstruction { get; set; } = new();

        public bool ShowCarouselNavigation { get; set; } = false;
        public bool ShowEditingImageButtons { get; set; } = true;

        public int CarouselSelectedIndex { get; set; } = 0;


        protected override async Task OnInitializedAsync()
        {
            if (!Id.HasValue)
                throw new ParameterPageNullException(nameof(Id));

            await LoadData(Id.Value);

            RefreshCarouselInfo();
        }

        public void RefreshCarouselInfo()
        {
            ShowCarouselNavigation = ProductDetail.ImageInstructions.Count > 1;
            ShowEditingImageButtons = !ProductDetail.ImageInstructions.Any();
        }

        private async Task LoadData(int id)
        {
            ProductDetail = await worker.ProductRepository.Get(id, CurrentSession.Workshop.Id);
            Materials = await worker.MaterialRepository.GetAll();
        }



        #region Image traitement
        private async Task OpenEditImageProductDialog()
        {
            var parameters = new DialogParameters { ["ProductDetail"] = this.ProductDetail, ["ImageInstruction"] = this.ProductDetail.ImageInstructions.ElementAt(CarouselSelectedIndex) };

            var dialog = DialogService.Show<ProductImageEditDialog>("Modifier le commentaire de l'image", parameters, this.CommonOptionDialog);
            var result = await dialog.Result;

            if (result.Cancelled)
            {
                if (Id.HasValue)
                    await LoadData(Id.Value);
                return;
            }

            RefreshCarouselInfo();
        }

        private async Task OpenAddImageProductDialog()
        {
            var parameters = new DialogParameters { ["ProductDetail"] = this.ProductDetail };

            var dialog = DialogService.Show<ProductImageAddDialog>("Ajouter une photo", parameters, this.CommonOptionDialog);
            var result = await dialog.Result;

            if (result.Cancelled) return;

            CarouselSelectedIndex = ProductDetail.ImageInstructions.Count - 1;
            RefreshCarouselInfo();
        }

        private async Task OpenDeleteImageProductDialog()
        {
            bool? result = await DialogService.ShowMessageBox("Suppression cette photo", "Voulez-vous supprimer la photo ? suppression définitive.", yesText: "Delete!", cancelText: "Cancel");

            if (!result.HasValue) return;

            var image = ProductDetail.ImageInstructions.ElementAt(CarouselSelectedIndex);
            LoadFileFromInputFile.RemoveFileInput(image.Url);

            ProductDetail.ImageInstructions.Remove(image);
            worker.ProductRepository.Update(ProductDetail);
            worker.Completed();

            if (ProductDetail.ImageInstructions.Any())
                CarouselSelectedIndex = ProductDetail.ImageInstructions.Count - 1;

            StateHasChanged();
            RefreshCarouselInfo();
        }
        #endregion

        #region Product traitement
        private async Task OpenDeleteProductDialog()
        {
            bool? result = await DialogService.ShowMessageBox("Suppression du produit", "Voulez-vous supprimer le produit ? suppression définitive.", yesText: "Delete!", cancelText: "Cancel");

            if (!result.HasValue) return;

            worker.ProductRepository.Delete(ProductDetail);
            worker.Completed();
            NavigationManager.NavigateTo($"/");
        }

        private async Task OpenEditProductDialog()
        {
            var parameters = new DialogParameters { ["ProductDetail"] = this.ProductDetail };

            var dialog = DialogService.Show<ProductEditDetailDialog>("Modifier les détails du produit", parameters, this.CommonOptionDialog);

            var result = await dialog.Result;
            if (result.Cancelled && Id.HasValue)
            {
                await LoadData(Id.Value);
            }

            StateHasChanged();
        }
        #endregion
    }
}