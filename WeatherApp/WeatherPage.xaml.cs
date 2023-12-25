
using WeatherApp.Services;

namespace WeatherApp;

public partial class WeatherPage : ContentPage
{
	public List<Models.List> WeatherList;
	private double latitude;
	private double longitude;
	public WeatherPage()
	{
		InitializeComponent();
		WeatherList = new List<Models.List>();
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		await GetLocation();
		await GetWeatherDataByLocation(latitude, longitude);
	}
	public async Task GetLocation()
	{
		var location = await Geolocation.GetLocationAsync();
		if (location != null)
		{
            latitude = location.Latitude;
            longitude = location.Longitude;
        }
	}

	public async void TapLocation_Tapped(object sender, EventArgs e) 
	{
		await GetLocation();
		var result = await ApiService.GetWeather(latitude, longitude);
		UpdateUI(result);
	}

	private async void ImageButton_Clicked(object sender, EventArgs e)
	{
		var response = await DisplayPromptAsync(title: "", message: "", placeholder: "Search weather by city", accept: "Search", cancel: "Cancel");
		if (response != null) 
		{
			await GetWeatherDataByCity(response);
			var result = await ApiService.GetWeather(latitude, longitude);
			UpdateUI(result);
		}
	}

    public async Task GetWeatherDataByLocation(double lat,double lon)
    {
        var result = await ApiService.GetWeather(lat,lon);
        UpdateUI(result);
    }

    public async Task GetWeatherDataByCity(string city)
    {
        var result = await ApiService.GetWeatherByCity(city);

        // 更新 latitude 和 longitude 的值
        latitude = result.city.coord.lat;
        longitude = result.city.coord.lon;

        UpdateUI(result);
    }


    public async void UpdateUI(dynamic result)
    {
        // 確保 result 不為 null 並且包含 list 屬性
        if (result != null && result.list != null && result.list.Count > 0)
        {
            foreach (var item in result.list)
            {
                WeatherList.Add(item);
            }

            CvWeather.ItemsSource = WeatherList;

            lblCity.Text = result.city?.name ?? "N/A";

            // 確保 list[0].weather 不為 null 並且包含 description 屬性
            weatherDescription.Text = result.list[0]?.weather[0]?.description ?? "N/A";

            LblTemperature.Text = result.list[0]?.main?.tempearatue + "°C" ?? "N/A";
            LblHumidity.Text = result.list[0]?.main?.humidity + "%" ?? "N/A";
            LblWind.Text = result.list[0]?.wind?.speed + "m/s" ?? "N/A";

            // 確保 list[0].weather 不為 null 並且包含 fullIcon 屬性
            weatherIcon.Source = result.list[0]?.weather[0]?.fullIcon ?? "N/A";

            // 根據溫度值顯示相應的建議文字
            double temperature = result.list[0]?.main?.tempearatue ?? 0;

            if (temperature <= 5 && temperature >=0)
            {
                SuggestionLabel.Text = "穿著保暖羊毛或絨毛衣、厚外套、保暖褲、防水靴，戴上帽子和厚手套以確保舒適度。";
            }
            else if (temperature <= 10 && temperature > 5)
            {
                SuggestionLabel.Text = "穿著輕保暖的衣物，包括長袖或中層、輕褲、輕外套，戴上輕帽和手套以確保適度的保暖。";
            }
            else if (temperature <= 15 && temperature > 10)
            {
                SuggestionLabel.Text = "選擇輕薄的長袖或短袖搭配褲子，並戴上輕外套，保持舒適感。";
            }
            else if (temperature <= 20 && temperature > 15)
            {
                SuggestionLabel.Text = "穿著短袖或薄長袖搭配褲子或裙子，選擇輕薄外套，保持清涼舒適。";
            }
            else if (temperature <= 25 && temperature > 20)
            {
                SuggestionLabel.Text = "適合穿著短袖、短褲、裙子或清涼的衣物，以保持涼爽感。";
            }
            else if (temperature <= 30 && temperature > 25)
            {
                SuggestionLabel.Text = "建議穿著涼爽透氣的短袖、短褲、裙子，選擇淺色衣物以降低吸熱，並戴上帽子和太陽眼鏡保護頭部和眼睛。";
            }
            else if (temperature > 30 )
            {
                SuggestionLabel.Text = "建議穿著涼爽透氣的薄短袖、短褲、裙子，選擇淺色、寬鬆的衣物以提供更好的通風，並戴上帽子、太陽眼鏡，隨時保持充足的水分補充以應對高溫天氣。";
            }
            else if (temperature < 0)
            { 
                SuggestionLabel.Text = "建議穿著厚重的羽絨外套、保暖的毛衣、厚實的長褲，並戴上帽子、厚手套，配戴保暖的靴子，以確保足夠的保暖。";
            }
            else
            {
                SuggestionLabel.Text = "找不到溫度";
            }
        }   
    }
}