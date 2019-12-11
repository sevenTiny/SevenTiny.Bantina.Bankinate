namespace Test.Common
{
    public class ConnectionStringHelper
    {
        public static string ConnectionString_Write = "server=127.0.0.1;Port=3306;database=SevenTinyTest;uid=sa;pwd=123456;Allow User Variables=true;SslMode=none;";
        public static string[] ConnectionStrings_Read = 
            new[] {
                ConnectionString_Write,
                ConnectionString_Write,
                ConnectionString_Write
            };
    }
}
