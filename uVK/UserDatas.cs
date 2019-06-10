﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uVK
{
    public static class UserDatas
    {
        public static long User_id { get; set; }
        public static string Token { get; set; }
        public static string Surname { get; set; }
        public static string Name { get; set; }
    }

    [Serializable]
    public class UserDatasToSerialize
    {
        public long User_id { get; set; }
        public string Token { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
    }
}
