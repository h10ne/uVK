using Microsoft.Extensions.DependencyInjection;
using System;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.IO;
using System.Windows;
using uVK.Helpers;

namespace uVK.Model
{
    public static class AuthModel
    {
        private static ServiceCollection _service;

        private static void AuthToken()
        {
            ApiDatas.Api.Authorize(new ApiAuthParams
            {
                AccessToken = UserDatas.Token,
                Settings = Settings.Offline
            });
        }

        public static void Restore()
        {
            System.Diagnostics.Process.Start("https://vk.com/restore");
        }

        private static void Auth2Fact(string login, string password)
        {
            bool needCode = false;
            try
            {
                ApiDatas.Api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Offline,
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
                var input = new PassBox.InputBoxWindow();
                input.ShowDialog();
                File.ReadAllText("someFile.tempdat");

                ApiDatas.Api.Authorize(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        string code = File.ReadAllText("someFile.tempdat");
                        return code;
                    }
                });
                File.Delete("someFile.tempdat");
            }

        }
        public static bool GetAuth(string login = null, string password = null)
        {
            try
            {
                _service = new ServiceCollection();
                _service.AddAudioBypass();
                ApiDatas.Api = new VkApi(_service);
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin"))
                {
                    UserDatasToSerialize datas1 = new UserDatasToSerialize();
                    DesSer.Deserialize(ref datas1);
                    UserDatas.Name = datas1.Name;
                    UserDatas.Surname = datas1.Surname;
                    UserDatas.Token = datas1.Token;
                    UserDatas.UserId = datas1.User_id;
                }
                if (UserDatas.Token != null)
                {
                    AuthToken();
                }
                else
                {
                    Auth2Fact(login, password);
                    if (ApiDatas.Api.IsAuthorized)
                    {
                        {
                            ApiDatas.IsAuth = true;
                            UserDatasToSerialize datas = new UserDatasToSerialize { Token = ApiDatas.Api.Token };
                            if (ApiDatas.Api.UserId != null) datas.User_id = ApiDatas.Api.UserId.Value;
                            datas.Name = ApiDatas.Api.Account.GetProfileInfo().FirstName;
                            datas.Surname = ApiDatas.Api.Account.GetProfileInfo().LastName;
                            DesSer.Serialize(datas);

                            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                            System.Windows.Application.Current.Shutdown();
                        }
                    }
                }
                return false;
            }
            catch
            {
                MessageBox.Show("Произошла ошибка. Введите свои данные еще раз.", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                File.Delete((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin"));
            }

            return false;
        }
    }
}
