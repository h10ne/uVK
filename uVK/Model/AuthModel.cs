using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.RequestParams;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.IO;
using uVK;
namespace uVK.Model
{
    public static class AuthModel
    {
        private static ServiceCollection service;

        private static void AuthToken()
        {
            ApiDatas.api.Authorize(new ApiAuthParams
            {
                AccessToken = UserDatas.Token,
                Settings = Settings.Offline
            });
        }

        private static void Auth2Fact(string login, string password)
        {
            string trueCode;
            bool needCode = false;
            try
            {
                ApiDatas.api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = VkNet.Enums.Filters.Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        needCode = true;
                        return "0";
                    }
                });
            }
            catch
            {
                if (!needCode)
                    return;
                var input = new InputBoxWindow();
                input.ShowDialog();
                trueCode = File.ReadAllText("someFile.tempdat");

                ApiDatas.api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = VkNet.Enums.Filters.Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        string code = File.ReadAllText("someFile.tempdat");
                        return code;
                    }
                });
                File.Delete("someFile.tempdat");
            }

        }
        public static void GetAuth(string login = null, string password = null)
        {

            service = new ServiceCollection();
            service.AddAudioBypass();
            ApiDatas.api = new VkApi(service);
            if (UserDatas.Token != null)
            {
                AuthToken();
            }
            else
            {
                Auth2Fact(login, password);
                if (ApiDatas.api.IsAuthorized)
                {
                    {
                        ApiDatas.IsAuth = true;
                        UserDatasToSerialize datas = new UserDatasToSerialize();
                        datas.Token = ApiDatas.api.Token;
                        datas.User_id = ApiDatas.api.UserId.Value;
                        datas.Name = ApiDatas.api.Account.GetProfileInfo().FirstName;
                        datas.Surname = ApiDatas.api.Account.GetProfileInfo().LastName;
                        Des_Ser.Serialize(datas);
                    }
                }

                System.Windows.Forms.Application.Restart();
                System.Environment.Exit(1);
            }
        }
    }
}
