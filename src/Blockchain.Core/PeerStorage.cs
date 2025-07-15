using Newtonsoft.Json;
public class PeerStorage
{
    private readonly string _path = "peers.json";

    public List<string> Load()
    {
        if (!File.Exists(_path)) return new List<string>();
        var json = File.ReadAllText(_path);
        return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
    }

    public void Save(IEnumerable<string> peers)
    {
        var json = JsonConvert.SerializeObject(peers, Formatting.Indented);
        File.WriteAllText(_path, json);
    }
}