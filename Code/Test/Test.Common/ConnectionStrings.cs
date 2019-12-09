//using SevenTiny.Bantina.Bankinate.Attributes;
//using SevenTiny.Bantina.Configuration;
//using System.Collections.Generic;

//namespace Test.Common
//{
//    public class ConnectionStrings : ConfigBase<ConnectionStrings>
//    {
//        [Column]
//        public string Key { get; set; }
//        [Column]
//        public string Value { get; set; }

//        private static Dictionary<string, string> dictionary;

//        private static void Initial()
//        {
//            dictionary = new Dictionary<string, string>();
//            foreach (var item in Configs)
//            {
//                if (dictionary.ContainsKey(item.Key))
//                    dictionary[item.Key] = item.Value;
//                else
//                    dictionary.Add(item.Key, item.Value);
//            }
//        }

//        public static string Get(string key)
//        {
//            if (dictionary != null && dictionary.ContainsKey(key))
//            {
//                return dictionary[key];
//            }
//            Initial();

//            if (dictionary.ContainsKey(key))
//                return dictionary[key];

//            return null;
//        }
//    }
//}

////"Data Source=.;Initial Catalog=SevenTinyTest;Integrated Security=True"