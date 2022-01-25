using LineDC.Liff;
using LineDC.Liff.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineLogin.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        private ILiffClient Liff { get; set; }

        [Inject]
        private ProtectedSessionStorage oProtectedSessionStorage { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IDialogService oDialogService { get; set; }

        protected Color oColor = Color.Default;

        //private InterfaceDataTransfer<string> Interface = new InterfaceDataTransfer<string>();

        protected bool CheckLogIn, Like = false;

        protected Profile Profile { get; set; }
        protected LiffContext Context { get; set; }
        protected string TokenId { get; set; }
        protected string OS { get; set; }
        protected string Language { get; set; }
        protected string Version { get; set; }
        protected string IDToken { get; set; }
        protected string LineVersion { get; set; }
        protected Friendship Friendship { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //CheckLogIn = await oSessionStorageService.GetItemAsync<bool>("StatusLogin");

            StateHasChanged();
        }

        protected void LikeProfile(bool Checked)
        {
            //MemberService.GetMembers(Interface);
            if (Checked)
            {
                oColor = Color.Default;
                Like = false;
            }
            else
            {
                oColor = Color.Secondary;
                Like = true;
            }
        }
        protected async Task Login()
        {
            try
            {
                Liff.Initialized = false;
                CheckLogIn = true;
                await oProtectedSessionStorage.SetAsync("Login", CheckLogIn);
                if (!Liff.Initialized)
                {
                    await Liff.Init(JSRuntime);
                    if (!await Liff.IsLoggedIn())
                    {
                        await Liff.Login();
                        return;
                    }
                    Liff.Initialized = true;

                }
                Profile = await Liff.GetProfile();
                if (await Liff.IsInClient())
                {
                    Context = await Liff.GetContext();
                }
                var idtoken = await Liff.GetDecodedIDToken();
                TokenId = idtoken.Sub;
                OS = await Liff.GetOS();
                Language = await Liff.GetLanguage();
                Version = await Liff.GetVersion();
                LineVersion = await Liff.GetLineVersion();
                //Friendship = await Liff.GetFriendship();
                IDToken = await Liff.GetIDToken();
                StateHasChanged();
            }
            catch (Exception e)
            {
                await JSRuntime.InvokeAsync<object>("alert", e.ToString());
            }
        }
        protected void LogOut()
        {
            if (Liff.Initialized)
            {
                Confirm();
                Liff.Initialized = false;
            }
        }
        private async void Confirm()
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "ออกจากระบบเรียบร้อยแล้ว");
            parameters.Add("ButtonText", "ตกลง");
            parameters.Add("Color", Color.Success);

            var oDialog = oDialogService.Show<DialogAlert>("สำเร็จ", parameters);
            var oResult = await oDialog.Result;

            if (!oResult.Cancelled)
            {
                await Liff.Logout();
                NavigationManager.NavigateTo("/", true);
            }
            CheckLogIn = false;
            //await oProtectedSessionStorage.SetAsync("CheckLogin", CheckLogIn);

            StateHasChanged();
        }
        private async void SetProfile()
        {
            await oProtectedSessionStorage.SetAsync("DisplayNameProfile", Profile.DisplayName);
            await oProtectedSessionStorage.SetAsync("TokenIdProfile", TokenId);
        }
    }
}
