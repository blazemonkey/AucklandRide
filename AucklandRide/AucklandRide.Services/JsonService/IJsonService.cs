namespace Services.JsonService
{
    public interface IJsonService
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
    }
}
