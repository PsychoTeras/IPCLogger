namespace IPCLogger.ConfigurationService.Entities
{
    public class DBRecord
    {
        public int Id { get; set; }

        public T Clone<T>() where T: DBRecord
        {
            return (T)MemberwiseClone();
        }
    }
}
