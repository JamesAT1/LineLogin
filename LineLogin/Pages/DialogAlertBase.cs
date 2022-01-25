using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineLogin.Pages
{
    public class DialogAlertBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string ContentText { get; set; }

        [Parameter] public string ButtonText { get; set; }

        [Parameter] public Color Color { get; set; }

        protected void Submit() => MudDialog.Close(DialogResult.Ok(true));
        protected void Cancel() => MudDialog.Cancel();
    }
}
