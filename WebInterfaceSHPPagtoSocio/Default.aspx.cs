using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using Newtonsoft.Json.Bson;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;
using NUnit.Framework;
using System.Diagnostics;
using RestSharp.Authenticators.OAuth;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Runtime.Serialization.Json;

namespace WebInterfaceSHPPagtoSocio
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // INCLUSÃO DE ALTERAÇÃO PARA TESTE DO GIT - JAIR 01/06/2020
            Label1.Text = DataFin.SelectedDate + " " + DataIni.SelectedDate;
            string wRet = GerarRegistroContasJson(DataIni.SelectedDate, DataFin.SelectedDate);
        }
        private static string POST(string json)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //KEYS

            const string consumerKey = "f705c6b2a43a7acfe9e6dda2a24389b139daab3195c6eb6418031c44e5c465ab";
            const string consumerSecret = "10e254e3158b31eea1e3f8034cbe0a3c3c0c4553639a8b81047e55afb06c8f83";
            const string accessToken = "62e84ef0542bdaf8343b469d9c703c59007577659be31a6b48928aed9132ea8a";
            const string tokenSecret = "3157071cdee78d4a438c062db9b8134d0da1ffb376957e7e8c3b6bfeec4b248d";

            //Endereço base
            const string baseUrl = "https://5022901.restlets.api.netsuite.com/app/site/hosting/restlet.nl";

            //criando a chamada com seus parametros
            var client = new RestClient(baseUrl);
            client.AddDefaultQueryParameter("script", "463");
            client.AddDefaultQueryParameter("deploy", "1");
            var request = new RestRequest(Method.POST);

            //Parametros
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("cobranca", json, ParameterType.RequestBody);//tanto faz o primeiro campo, é só para identificar


            //usando as chaves para criar o authorization
            OAuth1Authenticator auth = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken, tokenSecret);
            auth.SignatureMethod = OAuthSignatureMethod.HmacSha1;
            auth.Realm = "5022901";
            auth.ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader;
            client.Authenticator = auth;

            //chamada da api
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        public static string WDataINI(string wMensagem)
        {
            try
            {
                DateTime wDateI;
                //Console.WriteLine("\nToday is {0:d} at {0:T}.", dat);
                Console.Write(wMensagem);
                wDateI = DateTime.Parse(Console.ReadLine());
                return wDateI.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO DIGITAR DATA " + ex.Message);
                Console.ReadLine();
                return "";
            }
        }
        public static string GerarRegistroContasJson(DateTime DataIni, DateTime DataFin)
        {
            SqlConnection dataConnection = new SqlConnection();
            string data = DateTime.Now.AddDays(-1).Date.ToString("dd/MM/yyyy");

            // OPEN SQL DATABASE
            //dataConnection.ConnectionString =
            //    "User ID=sa;Password=sicom4713;Initial Catalog=PDV_Harmonia;" +
            //    "Data Source=JAIRSICOMDEV;SQLEXPRESS";

            String wCon = @"Server=JAIRSICOM\SQL2019;Database=PDVSHP;User ID=sa;Password=sicom4713; Connect Timeout=30; MultipleActiveResultSets=True";
            dataConnection.ConnectionString = wCon;
            dataConnection.Open();
            string wSql;

            SqlCommand cmdSQL = new SqlCommand();
            cmdSQL.CommandType = System.Data.CommandType.Text;
            cmdSQL.Connection = dataConnection;
            cmdSQL.Parameters.Clear();

            //string fecdata = Convert.ToDateTime(rec["lgr_data"]).ToShortDateString();

            wSql = "SELECT lgr_CodigoSocio, Sum(lgr_valor) as wValor " +
                    "FROM logRec WHERE " +
                    "lgr_data >= '" + DataIni.ToString().Substring(1, 10) +
                    "' AND lgr_data <=  '" + DataFin.ToString().Substring(1, 10) +
                    "' AND (lgr_processado IS NULL) AND lgr_status = 1 " +
                    " AND (SUBSTRING(lgr_NomeRede, 1, 4))  = 'NOTA' " +
                    "GROUP BY lgr_CodigoSocio " +
                    "ORDER BY lgr_CodigoSocio ";

            var command = new SqlCommand(wSql, dataConnection);
            command.Parameters.Clear();
            //command.Parameters.AddWithValue("@codigoBico", abastecimento.Bico);
            var consulta = command.ExecuteReader();



            while (consulta.Read())
            {
                //Log.GerarLog("Enviando / Socio " + consulta["lgr_CodigoSocio"].ToString());
                //Cobranca rc = new Cobranca();

                string wID = "0";
                string wNome = "Sem nome";
                // CARREGAR EM ARRAY OS FUNCIONARIOS COOPERADOS E AGENCIA
                wSql = "SELECT Nome, nLoja FROM Clientes WHERE codigo_cliente = " + int.Parse(consulta["lgr_CodigoSocio"].ToString());
                SqlCommand cmdSQLID = new SqlCommand();
                cmdSQLID.CommandType = System.Data.CommandType.Text;
                cmdSQLID.Connection = dataConnection;
                cmdSQLID.Parameters.Clear();
                cmdSQLID.CommandText = wSql;
                var dataID = cmdSQLID.ExecuteReader();
                if (dataID.HasRows)
                {
                    while (dataID.Read())
                    {
                        wID = dataID["nLoja"].ToString();
                        wNome = dataID["Nome"].ToString();
                    }
                }


                string json = JsonConvert.SerializeObject(//Teste enviando o json de uma cobranca
                new RootCobranca.RootCobranca()
                {
                    Cobranca = new RootCobranca.Cobranca()
                    {
                        origem = "Restaurante",
                        name = wNome,
                        titulo = consulta["lgr_CodigoSocio"].ToString(), //"102",
                        socio = wID,
                        dependente = "",
                        data = "02/12/2019",
                        item = "1755",
                        valor = consulta["wValor"].ToString().Replace(",", "."),
                        localidade = "1",
                        centrocusto = "17",
                        origemCreate = "1"
                    }
                });
                //string json = JsonConvert.SerializeObject(rc);
                string retornopost = POST(json);
                //titulo = consulta["lgr_CodigoSocio"].ToString(),
                //socio = consulta["lgr_CodigoSocio"].ToString(),
                //dependente = consulta["lgr_CodigoSocio"].ToString(),

                if (retornopost != null)
                {
                    //Log.GerarLog($"Socio " + consulta["lgr_CodigoSocio"].ToString() + "integrado com sucesso!");//Mudar para outra mensagem
                }

            };
            //            if (rc.comandas.Count > 0)
            //                return JsonConvert.SerializeObject(rc);
            return "OK";
        }
    }
}