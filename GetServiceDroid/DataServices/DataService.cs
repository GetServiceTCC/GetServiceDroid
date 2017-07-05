using Android.Util;
using GetServiceDroid.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GetServiceDroid.DataServices
{
    class DataService : IDisposable
    {
        protected HttpClient Client;

        public const string BASE_URL = "http://169.254.80.80:5000/";

        public DataService(Token token = null)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(BASE_URL);

            if (token != null)
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
        }

        public async Task<List<Estado>> GetEstados()
        {
            List<Estado> estados = new List<Estado>();

            try
            {
                var response = await Client.GetAsync("api/estados");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    estados = JsonConvert.DeserializeObject<List<Estado>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetEstados()", "Erro: " + e.Message);
            }

            return estados;
        }

        public async Task<List<Cidade>> GetCidades(int estadoId)
        {
            List<Cidade> cidades = new List<Cidade>();

            try
            {
                var response = await Client.GetAsync("api/Estados/" + estadoId + "/Cidades");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    cidades = JsonConvert.DeserializeObject<List<Cidade>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetCidades()", "Erro: " + e.Message);
            }

            return cidades;
        }

        public async Task<List<Categoria>> GetCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();

            try
            {
                var response = await Client.GetAsync("api/categorias");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    categorias = JsonConvert.DeserializeObject<List<Categoria>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetCategorias()", "Erro: " + e.Message);
            }

            return categorias;
        }

        public async Task<List<SubCategoria>> GetSubCategorias(int categoriaId)
        {
            List<SubCategoria> subCategorias = new List<SubCategoria>();

            try
            {
                var response = await Client.GetAsync("api/Categorias/" + categoriaId + "/SubCategorias");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    subCategorias = JsonConvert.DeserializeObject<List<SubCategoria>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetSubCategorias()", "Erro: " + e.Message);
            }

            return subCategorias;
        }

        public async Task<List<Profissional>> GetProfissionaisDestaque()
        {
            List<Profissional> profissionais = new List<Profissional>();

            try
            {
                var response = await Client.GetAsync("api/Profissionais/destaque");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    profissionais = JsonConvert.DeserializeObject<List<Profissional>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetProfissionaisDestaque()", "Erro: " + e.Message);
            }

            return profissionais;
        }

        public async Task<List<Profissional>> GetProfissionais(string query, Filtro filtro)
        {
            List<Profissional> profissionais = new List<Profissional>();

            try
            {
                string url = "api/Profissionais?q=" + query;

                if (filtro != null)
                {
                    if (filtro.Estado > 0)
                        url += "&estado=" + filtro.Estado;
                    if (filtro.Cidade > 0)
                        url += "&cidade=" + filtro.Cidade;
                    if (filtro.Categoria > 0)
                        url += "&categoria=" + filtro.Categoria;
                    if (filtro.SubCategoria > 0)
                        url += "&subCategoria=" + filtro.SubCategoria;
                }

                var response = await Client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    profissionais = JsonConvert.DeserializeObject<List<Profissional>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetProfissionais()", "Erro: " + e.Message);
            }

            return profissionais;
        }

        public async Task<Profissional> GetProfissional(string profissional)
        {
            try
            {
                var response = await Client.GetAsync("api/Profissionais/" + profissional);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Profissional>(content);
                }
                else
                {
                    throw new Exception(JObject.Parse(content).Value<string>("message"));
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetProfissional()", "Erro: " + e.Message);
                throw e;
            }
        }

        public async Task<List<Servico>> GetServicosDestaque()
        {
            List<Servico> servicos = new List<Servico>();

            try
            {
                var response = await Client.GetAsync("api/Servicos/destaque");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    servicos = JsonConvert.DeserializeObject<List<Servico>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetServicosDestaque()", "Erro: " + e.Message);
            }

            return servicos;
        }

        public async Task<List<Servico>> GetServicos(string query, Filtro filtro)
        {
            List<Servico> servicos = new List<Servico>();

            try
            {
                string url = "api/Servicos?q=" + query;

                if (filtro != null)
                {
                    if (filtro.Estado > 0)
                        url += "&estado=" + filtro.Estado;
                    if (filtro.Cidade > 0)
                        url += "&cidade=" + filtro.Cidade;
                    if (filtro.Categoria > 0)
                        url += "&categoria=" + filtro.Categoria;
                    if (filtro.SubCategoria > 0)
                        url += "&subCategoria=" + filtro.SubCategoria;
                }

                var response = await Client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    servicos = JsonConvert.DeserializeObject<List<Servico>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetServicos()", "Erro: " + e.Message);
            }

            return servicos;
        }

        public async Task<List<Servico>> GetServicosMeus()
        {
            List<Servico> servicos = new List<Servico>();

            try
            {
                var response = await Client.GetAsync("api/Servicos/meus");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    servicos = JsonConvert.DeserializeObject<List<Servico>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetServicosMeus()", "Erro: " + e.Message);
            }

            return servicos;
        }

        public async Task<List<Servico>> GetServicosProfissional(string profissional)
        {
            List<Servico> servicos = new List<Servico>();

            try
            {
                var response = await Client.GetAsync("api/Profissionais/" + profissional + "/Servicos");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    servicos = JsonConvert.DeserializeObject<List<Servico>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetServicosProfissional()", "Erro: " + e.Message);
            }

            return servicos;
        }

        public async Task<string> InserirServico(EditarServico servico)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(servico), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/Servicos", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("InserirServico()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<EditarServico> GetServico(int servicoId)
        {
            try
            {
                var response = await Client.GetAsync("api/Servicos/" + servicoId);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var servico = JsonConvert.DeserializeObject<EditarServico>(content);
                    return servico;
                }
                else
                {
                    throw new Exception(JObject.Parse(content).Value<string>("message"));
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetServico()", "Erro: " + e.Message);
                throw e;
            }
        }

        public async Task<string> EditarServico(EditarServico servico)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(servico), Encoding.UTF8, "application/json");
                var response = await Client.PutAsync("api/Servicos/" + servico.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("EditarServico()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<List<Comentario>> GetComentariosServico(int servicoId)
        {
            List<Comentario> comentarios = new List<Comentario>();

            try
            {
                var response = await Client.GetAsync("api/Servicos/" + servicoId + "/Comentarios");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    comentarios = JsonConvert.DeserializeObject<List<Comentario>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetComentariosServico()", "Erro: " + e.Message);
            }

            return comentarios;
        }

        public async Task<string> AddComentario(Comentario comentario)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(comentario), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/Comentarios", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("InserirComentario()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<Token> Login(string usuario, string senha)
        {
            try
            {
                var form = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", usuario),
                        new KeyValuePair<string, string>("password", senha),
                        new KeyValuePair<string, string>("client_id", "GetServiceMobile"),
                        new KeyValuePair<string, string>("client_secret", "mobile@123")
                    });

                var response = await Client.PostAsync("token", form);

                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var token = JsonConvert.DeserializeObject<Token>(content);
                    return token;
                }
                else
                {
                    throw new Exception(JObject.Parse(content).Value<string>("error_description"));
                }
            }
            catch (Exception e)
            {
                Log.Debug("Login()", "Erro: " + e.Message);
                throw e;
            }
        }

        public async Task<string> RegistrarUsuario(RegistraUsuario usuario)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/Conta/Registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrarUsuario()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<Usuario> GetUsuario(string token = "")
        {
            try
            {
                if (Client.DefaultRequestHeaders.Authorization == null && !string.IsNullOrEmpty(token))
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await Client.GetAsync("api/Conta/Usuario");
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var usuario = JsonConvert.DeserializeObject<Usuario>(content);
                    return usuario;
                }
                else
                {
                    throw new Exception(JObject.Parse(content).Value<string>("message"));
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetUsuario()", "Erro: " + e.Message);
                throw e;
            }
        }

        public async Task<string> AlterarUsuario(AlterarUsuario usuario)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/Conta/AlterarDados", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("AlterarUsuario()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<string> AlterarSenhaUsuario(AlterarSenhaUsuario usuario)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/Conta/AlterarSenha", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("AlterarSenhaUsuario()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<bool> AlterarFoto(MemoryStream file)
        {
            try
            {
                var content = new MultipartFormDataContent();
                var imageContent = new StreamContent(file);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "image", "image.jpg");
                var response = await Client.PostAsync("api/UsuarioFoto", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Debug("AlterarFoto()", "Erro: " + e.Message);
                return false;
            }
        }

        public async Task<bool> DeletarFoto()
        {
            try
            {
                var response = await Client.DeleteAsync("api/UsuarioFoto");
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Log.Debug("DeletarFoto()", "Erro: " + e.Message);
                return false;
            }
        }

        public async Task<string> AddContato(string nome)
        {
            try
            {
                StringContent content = new StringContent("=" + nome, Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await Client.PostAsync("api/Contatos", content);

                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(erro).Value<string>("message");
                }
            }
            catch (Exception e)
            {
                Log.Debug("AddContato()", "Erro: " + e.Message);
                return "Erro: " + e.Message;
            }
        }

        public async Task<Contato> GetContato(string nome)
        {
            try
            {
                var response = await Client.GetAsync("api/Contatos/" + nome);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Contato>(content);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetContato()", "Erro: " + e.Message);
                throw e;
            }
        }

        public async Task<List<Contato>> GetContatos()
        {
            List<Contato> contatos = new List<Contato>();

            try
            {
                var response = await Client.GetAsync("api/Contatos");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    contatos = JsonConvert.DeserializeObject<List<Contato>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetContatos()", "Erro: " + e.Message);
            }

            return contatos;
        }

        public async Task<List<Mensagem>> GetMensagens(string contato)
        {
            List<Mensagem> mensagens = new List<Mensagem>();

            try
            {
                var response = await Client.GetAsync("api/" + contato + "/Mensagens");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    mensagens = JsonConvert.DeserializeObject<List<Mensagem>>(content);
                }
            }
            catch (Exception e)
            {
                Log.Debug("GetMensagens()", "Erro: " + e.Message);
            }

            return mensagens;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}