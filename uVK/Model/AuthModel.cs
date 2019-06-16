﻿using Microsoft.Extensions.DependencyInjection;
using System;
using VkNet.Model;
using VkNet.Enums.Filters;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using System.IO;
using uVK.Helpers;

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

        public static void Restore()
        {
            System.Diagnostics.Process.Start("https://vk.com/restore");
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
        public static bool GetAuth(string login = null, string password = null)
        {

            service = new ServiceCollection();
            service.AddAudioBypass();
            ApiDatas.api = new VkApi(service);
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\uVK\\UserDatas\\data.bin"))
            {
                UserDatasToSerialize datas1 = new UserDatasToSerialize();
                Des_Ser.Deserialize(ref datas1);
                UserDatas.Name = datas1.Name;
                UserDatas.Surname = datas1.Surname;
                UserDatas.Token = datas1.Token;
                UserDatas.User_id = datas1.User_id;
            }
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

                        System.Windows.Forms.Application.Restart();
                        System.Environment.Exit(1);
                    }
                }
            }
            return false;
        }
    }
}