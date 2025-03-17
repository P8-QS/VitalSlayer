namespace Effects
{
    public interface Effect
    {
        public int level { get; set; }

        public string name { get; set; } //
        public string ToString();
        public string Description();
        
    }
}