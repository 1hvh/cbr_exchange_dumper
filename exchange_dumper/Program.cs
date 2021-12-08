using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace exchange_dumper
{
    public class Program
    {
        static string connectionString = $"datasource=127.0.0.1;port=3306;username={};password={};database=cbr";
        internal static void executeQuery(string query)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {

                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                con.Dispose();

            }
        }
        static void getExchanges()
        {
            XDocument xdoc = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");
            foreach (XElement Element in xdoc.Element("ValCurs").Elements("Valute"))
            {
                XAttribute ID = Element.Attribute("ID");
                XElement NumCode = Element.Element("NumCode");
                XElement CharCode = Element.Element("CharCode");
                XElement Nominal = Element.Element("Nominal");
                XElement Name = Element.Element("Name");
                XElement vValue = Element.Element("Value");
                float Valuef = float.Parse(vValue.Value); // потому что vValue.Value возвращает запись float через запятую, что не принимает MySql

                executeQuery($"INSERT INTO cbr.exchangerates (`StrId`, `NumCode`, `CharCode`, `Nominal`, `Name`, `Value`, `Date`) VALUES" +
                    $" ('{ID.Value}', '{NumCode.Value}', '{CharCode.Value}', '{Nominal.Value}', '{Name.Value}', '{Valuef.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture)}', NOW())");
            }
        }
        static void Main(string[] args)
        {
            getExchanges();
        }

        
    }
}
