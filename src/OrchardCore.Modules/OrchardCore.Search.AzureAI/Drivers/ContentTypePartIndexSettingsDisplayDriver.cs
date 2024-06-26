using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Search.AzureAI.Models;

namespace OrchardCore.Search.AzureAI.Drivers;

public class ContentTypePartIndexSettingsDisplayDriver(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    : ContentTypePartDefinitionDisplayDriver
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public override async Task<IDisplayResult> EditAsync(ContentTypePartDefinition contentTypePartDefinition, IUpdateModel updater)
    {
        if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, AzureAISearchIndexPermissionHelper.ManageAzureAISearchIndexes))
        {
            return null;
        }

        return Initialize<AzureAISearchContentIndexSettings>("AzureAISearchContentIndexSettings_Edit", model =>
        {
            model.Included = contentTypePartDefinition.GetSettings<AzureAISearchContentIndexSettings>().Included;
        }).Location("Content:10");
    }

    public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
    {
        if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, AzureAISearchIndexPermissionHelper.ManageAzureAISearchIndexes))
        {
            return null;
        }

        var settings = new AzureAISearchContentIndexSettings();

        await context.Updater.TryUpdateModelAsync(settings, Prefix);

        context.Builder.WithSettings(settings);

        return await EditAsync(contentTypePartDefinition, context.Updater);
    }
}
