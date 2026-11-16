namespace HelpDeskAPI
{
    // Modelo de exemplo criado pelo template; não participa do HelpDesk atual.
    public class WeatherForecast
    {
        // Data da previsão.
        public DateOnly Date { get; set; }

        // Temperatura em graus Celsius.
        public int TemperatureC { get; set; }

        // Conversão calculada para Fahrenheit.
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        // Descrição textual opcional da previsão.
        public string? Summary { get; set; }
    }
}
