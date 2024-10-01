using Newtonsoft.Json;
using Onatrix.Dtos;
using System.Diagnostics;
using System.Text;

namespace Onatrix.Services;

public class EmailService(HttpClient httpClient)
{
	private readonly HttpClient _httpClient = httpClient;

	public async Task SendDataToAzureFunction(EmailDto dto)
	{
		try
		{
			var functionUrl = "https://emailprovider-nikesjo-win23.azurewebsites.net/api/EmailConfirmation?code=Pn8D9aTEqcnGoiDLb01m43Pt9LL3H47dFAu9JajoxKkgAzFugFueEw%3D%3D";

			var json = JsonConvert.SerializeObject(new
			{
				email = dto.Email
			});

			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(functionUrl, content);

			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Bekräftelsemail skickat via Azure Function.");
			}
			else
			{
				Debug.WriteLine($"Fel vid anrop till Azure Function: {response.StatusCode}");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Fel vid anrop till Azure Function: {ex.Message}");
		}
	}
}
