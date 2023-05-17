namespace WebApi.Constants
{
    public class LogConstants
    {
        public static string TemplateMessage = "Acction: {Action},Module: {Module},IpAddress: {IpAddress},UserId {UserId},Username {Username}";
    }

    public class ActionConstants
    {
    }

    public class ModuleModel
    {
        public string Key { get; set; }
        public List<ObjectModel> Value { get; set; }
    }

    public class ObjectModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
