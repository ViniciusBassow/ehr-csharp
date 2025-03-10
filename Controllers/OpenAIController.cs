using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity;
using RestSharp;
using System.Text;
using System;
using OpenAI.Chat;

namespace ehr_csharp.Controllers
{
    public class OpenAIController : Controller
    {
        private readonly IConfiguration _configuration;

        public OpenAIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Por favor, selecione um arquivo.";
                return Json(new { success = false, message = "Por favor, selecione um arquivo." });
            }

            // Salvar o arquivo temporariamente
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Extrair texto do PDF
            var extractedText = ExtractTextFromPdf(filePath);

            // Chamar OpenAI API para analisar os dados (exemplo)
            var analysisResult =  AnalyzeTextWithOpenAI(extractedText);
            //"{ \"Hemograma\": { \"Eritrocitos\": 5.2, \"Hemoglobina\": 15.8, \"Hematocrito\": 48.3, \"VCM\": 92.4, \"HCM\": 30.2, \"CHCM\": 32.7, \"RDW\": 13.0, \"Leucocitos\": { \"Absoluto\": 6.8, \"Relativo\": 3.6 }, \"Bastonetes\": { \"Absoluto\": 0.0, \"Relativo\": 0.0 }, \"Segmentados\": { \"Absoluto\": 4.84, \"Relativo\": 71.5 }, \"Eosinofilos\": { \"Absoluto\": 0.07, \"Relativo\": 1.0 }, \"Basofilos\": { \"Absoluto\": 0.05, \"Relativo\": 0.7 }, \"Linfocitos\": { \"Absoluto\": 1.38, \"Relativo\": 20.4 }, \"Monocitos\": { \"Absoluto\": 0.43, \"Relativo\": 6.4 }, \"Plaquetas\": 314.0, \"VPM\": 9.2, \"Glicemia\": 87.0, \"Creatinina\": 0.88, \"AcidoUrico\": 5.1, \"Prolactina\": 11.5, \"Testosterona\": 1080.0, \"ColesterolTotal\": 136.0, \"HDL\": 52.0, \"Triglicerides\": 43.0, \"LDL\": 72.0, \"NaoHDL\": 84.0 }}"

            // Deserializar a string JSON em um objeto para retorno
            var deserializedResult = System.Text.Json.JsonSerializer.Deserialize<object>(analysisResult);

            // Retornar o resultado como JSON para a View
            return Json(new { success = true, data = deserializedResult });
        }


        private string ExtractTextFromPdf(string filePath)
        {
            // Utilize uma biblioteca de extra��o de PDF, como PdfSharp ou iTextSharp, para extrair o texto.
            // Exemplo de c�digo com iTextSharp:
            string extractedText = string.Empty;

            using (var reader = new iTextSharp.text.pdf.PdfReader(filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    extractedText += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i);
                }
            }

            return extractedText;
        }

        private string AnalyzeTextWithOpenAI(string text)
        {
            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo-1106", // Ajuste o modelo conforme necess�rio
                messages = new[]
                {
            new { role = "user", content = $"Analise o texto fornecido, extra�do de um exame de hemograma em PDF. Devolva a resposta no formato de texto mas formatado como um json, contendo somente um objeto com o somente o valor.  Use esse json como template para o tipo da resposta e n�o crie nenhuma chave e nem tire uma chave somente preencha os valores, lembrando q os valores s�o do tipo double :\r\n\r\n{{\r\n  \"Hemograma\": {{\r\n    \"Eritrocitos\": \"\",\r\n    \"Hemoglobina\": \"\",\r\n    \"Hematocrito\": \"\",\r\n    \"VCM\": \"\",\r\n    \"HCM\": \"\",\r\n    \"CHCM\": \"\",\r\n    \"RDW\": \"\",\r\n    \"Leucocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Bastonetes\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Segmentados\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Eosinofilos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Basofilos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Linfocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Monocitos\": {{\r\n      \"Absoluto\": \"\",\r\n      \"Relativo\": \"\"\r\n    }},\r\n    \"Plaquetas\":  \"\",\r\n    \"VPM\":  \"\",\r\n    \"Glicemia\": \"\",\r\n    \"Creatinina\": \"\",\r\n    \"AcidoUrico\": \"\",\r\n    \"Prolactina\": \"\",\r\n    \"Testosterona\": \"\",\r\n    \"ColesterolTotal\": \"\",\r\n    \"HDL\": \"\",\r\n    \"Triglicerides\": \"\",\r\n    \"LDL\": \"\",\r\n    \"NaoHDL\": \"\"\r\n  }}\r\n}}\r\n\r\n. Utilize os dados do texto: {text}" }
        },
                temperature = 0.5,
                max_tokens = 1500,
                response_format = new { type = "json_object" } // Adicionando o response_format

            };

            request.AddJsonBody(requestBody);

            var response =  client.Execute(request);

            if (response.IsSuccessful)
            {
                // Deserializar o conte�do da resposta
                var result = JsonConvert.DeserializeObject<dynamic>(response.Content);

                // Extrair o JSON do conte�do da mensagem
                string jsonContent = result?.choices[0]?.message?.content;

                return jsonContent;
            }
            else
            {
                // Trate o erro conforme necess�rio, por exemplo, logando a resposta
                throw new Exception($"Erro na chamada para OpenAI: {response.StatusCode} - {response.Content}");
            }
        }

    }
}