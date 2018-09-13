namespace IPCLogger.ConfigurationService.Entities
{
    public enum PageType
    {
        Loggers,
        Users
    }

    public class PageModel
    {
        public string PageName;
        public PageType PageType;
        public string Caption;
        public object Model;

        public PageModel(PageType pageType, object model)
        {
            PageType = pageType;
            PageName = Caption = pageType.ToString();
            Model = model;
        }

        public static PageModel Loggers(object model)
        {
            return new PageModel(PageType.Loggers, model);
        }

        public static PageModel Users(object model)
        {
            return new PageModel(PageType.Users, model);
        }
    }
}
