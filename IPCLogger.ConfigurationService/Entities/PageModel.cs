namespace IPCLogger.ConfigurationService.Entities
{
    public enum PageType
    {
        Loggers,
        Users
    }

    public class PageModel
    {
        public PageType PageType;
        public string Caption;
        public object Model;

        public PageModel(PageType pageType, string caption, object model)
        {
            PageType = pageType;
            Caption = caption ?? pageType.ToString();
            Model = model;
        }

        public static PageModel Loggers(object model)
        {
            return new PageModel(PageType.Loggers, null, model);
        }

        public static PageModel Users(object model)
        {
            return new PageModel(PageType.Users, null, model);
        }
    }
}
