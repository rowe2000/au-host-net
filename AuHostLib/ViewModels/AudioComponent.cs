namespace AuHost.ViewModels
{
    public class AudioComponent
    {
        public string Name { get; set; }
        public string Manufacture { get; set; }
        public bool Enable { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}