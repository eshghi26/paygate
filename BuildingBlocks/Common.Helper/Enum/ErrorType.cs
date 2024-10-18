using System.ComponentModel;

namespace Common.Helper.Enum
{
    public enum ErrorType
    {
        [Description("toast")]
        Toast = 0,

        [Description("popup")]
        Popup = 1,

        [Description("snakeBar")]
        SnakeBar = 2
    }
}
