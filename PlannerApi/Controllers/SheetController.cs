using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace PlannerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SheetController : ControllerBase
    {
        private static readonly string[] scopes = { SheetsService.Scope.Spreadsheets };

        private static readonly string applicationName = "pablotobalina";

        private static readonly string spreadsheetId = "1NDkoIcWBj5DiGGj-vn4cZ9ayutOMoQJl_AgIzzHYwDo";

        private static readonly string sheet = "Sheet1";

        private static SheetsService sheetService;


        public SheetController() {
            GoogleCredential credential;
            using(var stream = new FileStream("Properties/client_secrets.json", FileMode.Open, FileAccess.Read)) {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(scopes);
            }

            sheetService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
        }

        static void ReadEntries() {
            var range = $"{sheet}!A1:J7";
            var request = sheetService.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;
            if(values != null && values.Count > 0) {
                foreach (var row in values) {
                    foreach (var cell in row) {
                    }
                }
            }
        }

        static void CreateEntry() {
            var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();

            var objectList = new List<object> { "Hello", "This", "was", "inserted", "via", "C#" };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = sheetService.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();

        }

        static void UpdateEntry() {
            var range = $"{sheet}!A1";
            var valueRange = new ValueRange();

            var objectList = new List<object> { "Updated" };
            valueRange.Values = new List<IList<object>> { objectList };

            var updateRequest = sheetService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var updateResponse = updateRequest.Execute();
        }

        static void DeleteEntry() {
            var range = $"{sheet}!J:L";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = sheetService.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
            var deleteResponse = deleteRequest.Execute();
        }


        [HttpGet]
        public void Get() {
            DeleteEntry();
            UpdateEntry();
            CreateEntry();
            ReadEntries();
        }
    }
}
